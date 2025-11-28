using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x02000623 RID: 1571
public class GameLightingManager : MonoBehaviourTick, IGorillaSliceableSimple
{
	// Token: 0x060027EE RID: 10222 RVA: 0x000D4733 File Offset: 0x000D2933
	private void Awake()
	{
		this.InitData();
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000D473C File Offset: 0x000D293C
	private void InitData()
	{
		GameLightingManager.instance = this;
		this.gameLights = new List<GameLight>(512);
		for (int i = 0; i < this.lightDistanceBins.Length; i++)
		{
			this.lightDistanceBins[i] = new List<GameLight>();
		}
		this.lightDataBuffer = new GraphicsBuffer(16, 50, UnsafeUtility.SizeOf<GameLightingManager.LightData>());
		this.lightData = new NativeArray<GameLightingManager.LightData>(50, 4, 1);
		this.nextLightUpdate = 0;
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		this.SetMaxLights(20);
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000D47D7 File Offset: 0x000D29D7
	private void OnDestroy()
	{
		this.ClearGameLights();
		this.SetDesaturateAndTintEnabled(false, Color.black);
		this.SetAmbientLightDynamic(Color.black);
		this.SetCustomDynamicLightingEnabled(false);
		this.lightData.Dispose();
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000D4808 File Offset: 0x000D2A08
	public new void OnEnable()
	{
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060027F2 RID: 10226 RVA: 0x000D4817 File Offset: 0x000D2A17
	public new void OnDisable()
	{
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060027F3 RID: 10227 RVA: 0x000D4828 File Offset: 0x000D2A28
	public void ZoneEnableCustomDynamicLighting(bool enable)
	{
		if (enable)
		{
			if (this.zoneDynamicLightingEnableCount == 0)
			{
				this.SetCustomDynamicLightingEnabled(true);
			}
			this.zoneDynamicLightingEnableCount++;
			return;
		}
		this.zoneDynamicLightingEnableCount--;
		if (this.zoneDynamicLightingEnableCount == 0)
		{
			this.SetCustomDynamicLightingEnabled(false);
		}
		if (this.zoneDynamicLightingEnableCount < 0)
		{
			Debug.LogErrorFormat("Zone Dynamic Lighting Ref count is {0} and should never be less that 0", new object[]
			{
				this.zoneDynamicLightingEnableCount
			});
			this.zoneDynamicLightingEnableCount = 0;
		}
	}

	// Token: 0x060027F4 RID: 10228 RVA: 0x000D48A1 File Offset: 0x000D2AA1
	public void SetCustomDynamicLightingEnabled(bool enable)
	{
		this.customVertexLightingEnabled = enable;
		if (this.customVertexLightingEnabled)
		{
			Shader.EnableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
			return;
		}
		Shader.DisableKeyword("_ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX");
	}

	// Token: 0x060027F5 RID: 10229 RVA: 0x000D48C7 File Offset: 0x000D2AC7
	public void SetAmbientLightDynamic(Color color)
	{
		Shader.SetGlobalColor("_GT_GameLight_Ambient_Color", color);
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x000D48D4 File Offset: 0x000D2AD4
	public void SetMaxLights(int maxLights)
	{
		maxLights = Mathf.Min(maxLights, 50);
		this.maxUseTestLights = maxLights;
		Shader.SetGlobalInteger("_GT_GameLight_UseMaxLights", maxLights);
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x000D48F2 File Offset: 0x000D2AF2
	public void SetDesaturateAndTintEnabled(bool enable, Color tint)
	{
		Shader.SetGlobalColor("_GT_DesaturateAndTint_TintColor", tint);
		Shader.SetGlobalFloat("_GT_DesaturateAndTint_TintAmount", enable ? 1f : 0f);
		this.desaturateAndTintEnabled = enable;
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000D491F File Offset: 0x000D2B1F
	public void SliceUpdate()
	{
		if (this.skipNextSlice)
		{
			this.skipNextSlice = false;
			return;
		}
		this.immediateSort = false;
		this.SortLights();
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000D4940 File Offset: 0x000D2B40
	public void SortLights()
	{
		if (this.gameLights.Count <= this.maxUseTestLights)
		{
			return;
		}
		if (this.mainCameraTransform == null)
		{
			this.mainCameraTransform = Camera.main.transform;
		}
		this.cameraPosForSort = this.mainCameraTransform.position;
		this.gameLights.Sort(new Comparison<GameLight>(this.CompareDistFromCamera));
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000D49A8 File Offset: 0x000D2BA8
	private int CompareDistFromCamera(GameLight a, GameLight b)
	{
		if (a == null || a.light == null)
		{
			if (b == null || b.light == null)
			{
				return 0;
			}
			return -1;
		}
		else
		{
			if (b == null || b.light == null)
			{
				return 1;
			}
			float num = Mathf.Clamp(a.cachedColorAndIntensity.x + a.cachedColorAndIntensity.y + a.cachedColorAndIntensity.z, 0.01f, 6f);
			float num2 = Mathf.Clamp(b.cachedColorAndIntensity.x + b.cachedColorAndIntensity.y + b.cachedColorAndIntensity.z, 0.01f, 6f);
			float num3 = (this.cameraPosForSort - a.cachedPosition).sqrMagnitude / num;
			float num4 = (this.cameraPosForSort - b.cachedPosition).sqrMagnitude / num2;
			return num3.CompareTo(num4);
		}
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000D4AA8 File Offset: 0x000D2CA8
	public override void Tick()
	{
		this.RefreshLightData();
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x000D4AB0 File Offset: 0x000D2CB0
	private void RefreshLightData()
	{
		NativeArray<GameLightingManager.LightData> nativeArray = this.lightData;
		if (this.customVertexLightingEnabled)
		{
			int numLightsToPull = 10;
			if (this.immediateSort)
			{
				this.immediateSort = false;
				this.skipNextSlice = true;
				this.CacheAllLightData();
				this.SortLights();
				numLightsToPull = this.maxUseTestLights;
			}
			else
			{
				int numLightsToUpdateCache = 5;
				this.CacheLightDataForNonCloseLights(numLightsToUpdateCache);
			}
			this.PullLightData(numLightsToPull);
			this.lightDataBuffer.SetData<GameLightingManager.LightData>(this.lightData);
			Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
		}
	}

	// Token: 0x060027FD RID: 10237 RVA: 0x000D4B2C File Offset: 0x000D2D2C
	public void CacheAllLightData()
	{
		for (int i = 0; i < this.gameLights.Count; i++)
		{
			GameLight gameLight = this.gameLights[i];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x060027FE RID: 10238 RVA: 0x000D4BCC File Offset: 0x000D2DCC
	public void CacheLightDataForNonCloseLights(int numLightsToUpdateCache)
	{
		int num = this.gameLights.Count - this.maxUseTestLights;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < numLightsToUpdateCache; i++)
		{
			this.nextLightCacheUpdate = (this.nextLightCacheUpdate + 1) % num;
			GameLight gameLight = this.gameLights[this.maxUseTestLights + this.nextLightCacheUpdate];
			if (gameLight != null && gameLight.light != null)
			{
				gameLight.cachedPosition = gameLight.transform.position;
				gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
			}
		}
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000D4C98 File Offset: 0x000D2E98
	public void PullLightData(int numLightsToPull)
	{
		for (int i = 0; i < this.maxUseTestLights; i++)
		{
			if (i < this.gameLights.Count && this.gameLights[i] != null && this.gameLights[i].isHighPriorityPlayerLight)
			{
				this.GetFromLight(i, i);
			}
		}
		for (int j = 0; j < numLightsToPull; j++)
		{
			this.nextLightUpdate = (this.nextLightUpdate + 1) % this.maxUseTestLights;
			if (this.nextLightUpdate < this.gameLights.Count)
			{
				this.GetFromLight(this.nextLightUpdate, this.nextLightUpdate);
				if (this.gameLights[this.nextLightUpdate] != null && this.gameLights[this.nextLightUpdate].isHighPriorityPlayerLight)
				{
				}
			}
			else
			{
				this.ResetLight(this.nextLightUpdate);
			}
		}
	}

	// Token: 0x06002800 RID: 10240 RVA: 0x000D4D7C File Offset: 0x000D2F7C
	public int AddGameLight(GameLight light, bool ignoreUnityLightDisable = false)
	{
		if (light == null || !light.gameObject.activeInHierarchy || light.light == null || !light.light.enabled)
		{
			return -1;
		}
		if (this.gameLights.Contains(light))
		{
			return -1;
		}
		if (!ignoreUnityLightDisable)
		{
			light.light.enabled = false;
		}
		this.gameLights.Add(light);
		this.immediateSort = true;
		return this.gameLights.Count - 1;
	}

	// Token: 0x06002801 RID: 10241 RVA: 0x000D4DFC File Offset: 0x000D2FFC
	public void RemoveGameLight(GameLight light)
	{
		if (light != null && light.light != null)
		{
			light.light.enabled = true;
		}
		int num = this.gameLights.IndexOf(light);
		if (num >= 0)
		{
			this.gameLights.RemoveAt(num);
		}
	}

	// Token: 0x06002802 RID: 10242 RVA: 0x000D4E4C File Offset: 0x000D304C
	public void ClearGameLights()
	{
		if (this.gameLights != null)
		{
			this.gameLights.Clear();
		}
		NativeArray<GameLightingManager.LightData> nativeArray = this.lightData;
		for (int i = 0; i < this.lightData.Length; i++)
		{
			this.ResetLight(i);
		}
		this.lightDataBuffer.SetData<GameLightingManager.LightData>(this.lightData);
		Shader.SetGlobalBuffer("_GT_GameLight_Lights", this.lightDataBuffer);
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000D4EB4 File Offset: 0x000D30B4
	public void GetFromLight(int lightIndex, int gameLightIndex)
	{
		NativeArray<GameLightingManager.LightData> nativeArray = this.lightData;
		GameLight gameLight = null;
		if (gameLightIndex >= 0 && gameLightIndex < this.gameLights.Count)
		{
			gameLight = this.gameLights[gameLightIndex];
		}
		if (gameLight == null || gameLight.light == null)
		{
			return;
		}
		gameLight.cachedPosition = gameLight.transform.position;
		gameLight.cachedColorAndIntensity = (float)gameLight.intensityMult * gameLight.light.intensity * (gameLight.negativeLight ? -1f : 1f) * gameLight.light.color;
		Vector4 lightPos = gameLight.cachedPosition;
		lightPos.w = 1f;
		Vector4 cachedColorAndIntensity = gameLight.cachedColorAndIntensity;
		Vector3 zero = Vector3.zero;
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = lightPos,
			lightColor = cachedColorAndIntensity,
			lightDirection = zero
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000D4FB4 File Offset: 0x000D31B4
	private void ResetLight(int lightIndex)
	{
		GameLightingManager.LightData lightData = new GameLightingManager.LightData
		{
			lightPos = Vector4.zero,
			lightColor = Color.black,
			lightDirection = Vector4.zero
		};
		this.lightData[lightIndex] = lightData;
	}

	// Token: 0x04003368 RID: 13160
	[OnEnterPlay_SetNull]
	public static volatile GameLightingManager instance;

	// Token: 0x04003369 RID: 13161
	public const int MAX_VERTEX_LIGHTS = 50;

	// Token: 0x0400336A RID: 13162
	public const int USE_MAX_VERTEX_LIGHTS = 20;

	// Token: 0x0400336B RID: 13163
	public const int MAX_UPDATE_LIGHTS_PER_FRAME = 10;

	// Token: 0x0400336C RID: 13164
	private const int MAX_LIGHT_POWER = 100;

	// Token: 0x0400336D RID: 13165
	private const int LIGHT_POWER_BIN_SIZE = 5;

	// Token: 0x0400336E RID: 13166
	public Transform testLightsCenter;

	// Token: 0x0400336F RID: 13167
	[ColorUsage(true, true)]
	public Color testAmbience = Color.black;

	// Token: 0x04003370 RID: 13168
	[ColorUsage(true, true)]
	public Color testLightColor = Color.white;

	// Token: 0x04003371 RID: 13169
	public float testLightBrightness = 10f;

	// Token: 0x04003372 RID: 13170
	public float testLightRadius = 2f;

	// Token: 0x04003373 RID: 13171
	public int maxUseTestLights = 1;

	// Token: 0x04003374 RID: 13172
	[ReadOnly]
	[SerializeField]
	private List<GameLight> gameLights;

	// Token: 0x04003375 RID: 13173
	private bool customVertexLightingEnabled;

	// Token: 0x04003376 RID: 13174
	private bool desaturateAndTintEnabled;

	// Token: 0x04003377 RID: 13175
	private Transform mainCameraTransform;

	// Token: 0x04003378 RID: 13176
	private int zoneDynamicLightingEnableCount;

	// Token: 0x04003379 RID: 13177
	private List<GameLight>[] lightDistanceBins = new List<GameLight>[20];

	// Token: 0x0400337A RID: 13178
	private NativeArray<GameLightingManager.LightData> lightData;

	// Token: 0x0400337B RID: 13179
	private GraphicsBuffer lightDataBuffer;

	// Token: 0x0400337C RID: 13180
	private Vector3 cameraPosForSort;

	// Token: 0x0400337D RID: 13181
	private bool skipNextSlice;

	// Token: 0x0400337E RID: 13182
	private bool immediateSort;

	// Token: 0x0400337F RID: 13183
	private int nextLightUpdate;

	// Token: 0x04003380 RID: 13184
	private int nextLightCacheUpdate;

	// Token: 0x02000624 RID: 1572
	public struct LightInput
	{
		// Token: 0x04003381 RID: 13185
		public Color color;

		// Token: 0x04003382 RID: 13186
		public float intensity;

		// Token: 0x04003383 RID: 13187
		public float intensityMult;
	}

	// Token: 0x02000625 RID: 1573
	public struct LightData
	{
		// Token: 0x04003384 RID: 13188
		public Vector4 lightPos;

		// Token: 0x04003385 RID: 13189
		public Vector4 lightColor;

		// Token: 0x04003386 RID: 13190
		public Vector4 lightDirection;
	}
}
