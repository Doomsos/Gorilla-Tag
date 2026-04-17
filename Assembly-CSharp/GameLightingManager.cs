using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameLightingManager : MonoBehaviourTick, IGorillaSliceableSimple
{
	private struct LightInput
	{
		public Color color;

		public float intensity;

		public float intensityMult;
	}

	private struct LightDataPacked
	{
		public uint posXY;

		public uint posZW;

		public uint colorRG;

		public uint colorBA;
	}

	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	public const int MAX_VERTEX_LIGHTS = 50;

	public const int USE_MAX_VERTEX_LIGHTS = 20;

	public const int MAX_UPDATE_LIGHTS_PER_FRAME = 10;

	public Transform testLightsCenter;

	[ColorUsage(true, true)]
	public Color testAmbience = Color.black;

	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	public float testLightBrightness = 10f;

	public float testLightRadius = 2f;

	public int maxUseTestLights = 1;

	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	private bool customVertexLightingEnabled;

	private bool desaturateAndTintEnabled;

	private Transform mainCameraTransform;

	private int zoneDynamicLightingEnableCount;

	private float[] sortKeys;

	private GameLight[] sortValues;

	private NativeArray<LightDataPacked> lightData;

	private GraphicsBuffer lightDataBuffer;

	private bool skipNextSlice;

	private bool immediateSort;

	private int nextLightUpdate;

	private int nextLightCacheUpdate;

	[SerializeField]
	private Light _GR_NearsightedDimLight;

	private static readonly int _shaderPropId_GameLight_UseMaxLights = Shader.PropertyToID("_GT_GameLight_UseMaxLights");

	private static readonly int _shaderPropId_DesaturateAndTint_TintColor = Shader.PropertyToID("_GT_DesaturateAndTint_TintColor");

	private static readonly int _shaderPropId_DesaturateAndTint_TintAmount = Shader.PropertyToID("_GT_DesaturateAndTint_TintAmount");

	private static readonly int _shaderPropId_GameLight_Ambient_Color = Shader.PropertyToID("_GT_GameLight_Ambient_Color");

	private static readonly int _shaderPropId_GameLight_Lights = Shader.PropertyToID("_GT_GameLight_Lights");

	public bool IsDynamicLightingEnabled => customVertexLightingEnabled;

	public Light GR_NearsightedDimLight => _GR_NearsightedDimLight;

	private static uint PackHalf2(float a, float b)
	{
		return (uint)(Mathf.FloatToHalf(a) | (Mathf.FloatToHalf(b) << 16));
	}

	private void Awake()
	{
		InitData();
	}

	private void InitData()
	{
		instance = this;
		gameLights = new List<GameLight>(512);
		sortKeys = new float[512];
		sortValues = new GameLight[512];
		lightDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 50, UnsafeUtility.SizeOf<LightDataPacked>());
		lightData = new NativeArray<LightDataPacked>(50, Allocator.Persistent);
		nextLightUpdate = 0;
		ClearGameLights();
		SetDesaturateAndTintEnabled(enable: false, Color.black);
		SetAmbientLightDynamic(Color.black);
		SetCustomDynamicLightingEnabled(enable: false);
		SetMaxLights(20);
	}

	private void OnDestroy()
	{
		ClearGameLights();
		SetDesaturateAndTintEnabled(enable: false, Color.black);
		SetAmbientLightDynamic(Color.black);
		SetCustomDynamicLightingEnabled(enable: false);
		lightDataBuffer?.Dispose();
		if (lightData.IsCreated)
		{
			lightData.Dispose();
		}
	}

	public new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public void ZoneEnableCustomDynamicLighting(bool enable)
	{
		if (enable)
		{
			if (zoneDynamicLightingEnableCount == 0)
			{
				SetCustomDynamicLightingEnabled(enable: true);
			}
			zoneDynamicLightingEnableCount++;
			return;
		}
		zoneDynamicLightingEnableCount--;
		if (zoneDynamicLightingEnableCount == 0)
		{
			SetCustomDynamicLightingEnabled(enable: false);
		}
		if (zoneDynamicLightingEnableCount < 0)
		{
			Debug.LogErrorFormat("Zone Dynamic Lighting Ref count is {0} and should never be less that 0", zoneDynamicLightingEnableCount);
			zoneDynamicLightingEnableCount = 0;
		}
	}

	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		customVertexLightingEnabled = enable;
		if (customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		}
		else
		{
			Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
		}
	}

	public void ToggleCustomDynamicLightingEnabled()
	{
		SetCustomDynamicLightingEnabled(!customVertexLightingEnabled);
	}

	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor(_shaderPropId_GameLight_Ambient_Color, color);
	}

	public void SetMaxLights(int maxLights)
	{
		maxLights = Mathf.Min(maxLights, 50);
		maxUseTestLights = maxLights;
		Shader.SetGlobalInteger(_shaderPropId_GameLight_UseMaxLights, maxLights);
	}

	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor(_shaderPropId_DesaturateAndTint_TintColor, tint);
		Shader.SetGlobalFloat(_shaderPropId_DesaturateAndTint_TintAmount, enable ? 1f : 0f);
		desaturateAndTintEnabled = enable;
	}

	public void SliceUpdate()
	{
		if (skipNextSlice)
		{
			skipNextSlice = false;
			return;
		}
		immediateSort = false;
		SortLights();
	}

	public void SortLights()
	{
		int count = gameLights.Count;
		if (count <= maxUseTestLights)
		{
			return;
		}
		if (mainCameraTransform == null)
		{
			mainCameraTransform = Camera.main.transform;
		}
		Vector3 position = mainCameraTransform.position;
		if (sortKeys == null || sortKeys.Length < count)
		{
			int num = Mathf.Max(count, (sortKeys != null) ? (sortKeys.Length * 2) : 64);
			sortKeys = new float[num];
			sortValues = new GameLight[num];
		}
		for (int i = 0; i < count; i++)
		{
			GameLight gameLight = gameLights[i];
			if (gameLight == null || gameLight.light == null)
			{
				sortKeys[i] = float.MaxValue;
			}
			else
			{
				float num2 = Mathf.Clamp(gameLight.cachedColorAndIntensity.x + gameLight.cachedColorAndIntensity.y + gameLight.cachedColorAndIntensity.z, 0.01f, 6f);
				Vector3 vector = position - gameLight.cachedPosition;
				sortKeys[i] = (vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) / num2;
			}
			sortValues[i] = gameLight;
		}
		Array.Sort(sortKeys, sortValues, 0, count);
		for (int j = 0; j < count; j++)
		{
			gameLights[j] = sortValues[j];
		}
	}

	public override void Tick()
	{
		RefreshLightData();
	}

	private void RefreshLightData()
	{
		if (lightDataBuffer != null && customVertexLightingEnabled)
		{
			int numLightsToPull = 10;
			if (immediateSort)
			{
				immediateSort = false;
				skipNextSlice = true;
				CacheAllLightData();
				SortLights();
				numLightsToPull = maxUseTestLights;
			}
			else
			{
				int numLightsToUpdateCache = 5;
				CacheLightDataForNonCloseLights(numLightsToUpdateCache);
			}
			PullLightData(numLightsToPull);
			int num = Mathf.Min(gameLights.Count, maxUseTestLights);
			if (num > 0)
			{
				lightDataBuffer.SetData(lightData, 0, 0, num);
				Shader.SetGlobalBuffer(_shaderPropId_GameLight_Lights, lightDataBuffer);
				Shader.SetGlobalInteger(_shaderPropId_GameLight_UseMaxLights, num);
			}
		}
	}

	public void CacheAllLightData()
	{
		for (int i = 0; i < gameLights.Count; i++)
		{
			GameLight gameLight = gameLights[i];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
			}
		}
	}

	public void CacheLightDataForNonCloseLights(int numLightsToUpdateCache)
	{
		int num = gameLights.Count - maxUseTestLights;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < numLightsToUpdateCache; i++)
		{
			nextLightCacheUpdate = (nextLightCacheUpdate + 1) % num;
			GameLight gameLight = gameLights[maxUseTestLights + nextLightCacheUpdate];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
			}
		}
	}

	public void PullLightData(int numLightsToPull)
	{
		for (int i = 0; i < maxUseTestLights; i++)
		{
			if (i < gameLights.Count && gameLights[i] != null && gameLights[i].isHighPriorityPlayerLight)
			{
				GetFromLight(i, i);
			}
		}
		for (int j = 0; j < numLightsToPull; j++)
		{
			nextLightUpdate = (nextLightUpdate + 1) % maxUseTestLights;
			if (nextLightUpdate < gameLights.Count)
			{
				GetFromLight(nextLightUpdate, nextLightUpdate);
				if (gameLights[nextLightUpdate] != null && !gameLights[nextLightUpdate].isHighPriorityPlayerLight)
				{
				}
			}
			else
			{
				ResetLight(nextLightUpdate);
			}
		}
	}

	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (light.IsRegistered)
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		gameLights.Add(light);
		immediateSort = true;
		return gameLights.Count - 1;
	}

	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		if (light != null)
		{
			light.lightId = -1;
		}
		int num = gameLights.IndexOf(light);
		if (num >= 0)
		{
			gameLights.RemoveAt(num);
		}
	}

	public void ClearGameLights()
	{
		if (gameLights != null)
		{
			gameLights.Clear();
		}
		if (lightDataBuffer != null)
		{
			for (int i = 0; i < 50; i++)
			{
				ResetLight(i);
			}
			lightDataBuffer.SetData(lightData);
			Shader.SetGlobalBuffer(_shaderPropId_GameLight_Lights, lightDataBuffer);
		}
	}

	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		if (lightDataBuffer != null)
		{
			GameLight gameLight = null;
			if (gameLightIndex >= 0 && gameLightIndex < gameLights.Count)
			{
				gameLight = gameLights[gameLightIndex];
			}
			if (!(gameLight == null) && !(gameLight.light == null))
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? (-1f) : 1f) * gameLight.light.color;
				Vector3 cachedPosition = gameLight.cachedPosition;
				Vector4 cachedColorAndIntensity = gameLight.cachedColorAndIntensity;
				lightData[lightIndex] = new LightDataPacked
				{
					posXY = PackHalf2(cachedPosition.x, cachedPosition.y),
					posZW = PackHalf2(cachedPosition.z, 1f),
					colorRG = PackHalf2(cachedColorAndIntensity.x, cachedColorAndIntensity.y),
					colorBA = PackHalf2(cachedColorAndIntensity.z, cachedColorAndIntensity.w)
				};
			}
		}
	}

	private void ResetLight(int lightIndex)
	{
		lightData[lightIndex] = default(LightDataPacked);
	}
}
