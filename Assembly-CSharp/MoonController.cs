using System;
using System.Collections.Generic;
using CjLib;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class MoonController : MonoBehaviour
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000C27 RID: 3111 RVA: 0x00041D7B File Offset: 0x0003FF7B
	public float Distance
	{
		get
		{
			return this.distance;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000C28 RID: 3112 RVA: 0x00041D83 File Offset: 0x0003FF83
	private float TimeOfDay
	{
		get
		{
			if (this.debugOverrideTimeOfDay)
			{
				return Mathf.Repeat(this.timeOfDayOverride, 1f);
			}
			if (!(BetterDayNightManager.instance != null))
			{
				return 1f;
			}
			return BetterDayNightManager.instance.NormalizedTimeOfDay;
		}
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x00041DBF File Offset: 0x0003FFBF
	public void SetEyeOpenAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x00041DD3 File Offset: 0x0003FFD3
	public void StartEyeCloseAnimation()
	{
		this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x00041DE8 File Offset: 0x0003FFE8
	private void Start()
	{
		this.eyeOpenHash = Animator.StringToHash("EyeOpen");
		this.zoneToSceneMapping.Add(GTZone.forest, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.city, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.basement, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.canyon, MoonController.Scenes.Canyon);
		this.zoneToSceneMapping.Add(GTZone.beach, MoonController.Scenes.Beach);
		this.zoneToSceneMapping.Add(GTZone.mountain, MoonController.Scenes.Mountain);
		this.zoneToSceneMapping.Add(GTZone.skyJungle, MoonController.Scenes.Clouds);
		this.zoneToSceneMapping.Add(GTZone.cave, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.cityWithSkyJungle, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.tutorial, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.rotating, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.none, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.Metropolis, MoonController.Scenes.Metropolis);
		this.zoneToSceneMapping.Add(GTZone.cityNoBuildings, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.attic, MoonController.Scenes.Forest);
		this.zoneToSceneMapping.Add(GTZone.arcade, MoonController.Scenes.City);
		this.zoneToSceneMapping.Add(GTZone.bayou, MoonController.Scenes.Bayou);
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.RegisterMoon(this);
		}
		this.crackStartDayOfYear = new DateTime(2024, 10, 4).DayOfYear;
		this.crackEndDayOfYear = new DateTime(2024, 10, 25).DayOfYear;
		if (this.crackRenderer != null)
		{
			this.currentlySetCrackProgress = 1f;
			this.crackMaterialPropertyBlock = new MaterialPropertyBlock();
			this.crackRenderer.GetPropertyBlock(this.crackMaterialPropertyBlock);
			this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
			this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
		}
		this.orbitAngle = 0f;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00041FE3 File Offset: 0x000401E3
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.UnregisterMoon(this);
		}
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x00042004 File Offset: 0x00040204
	private void OnZoneChanged()
	{
		ZoneManagement instance = ZoneManagement.instance;
		MoonController.Scenes scenes = MoonController.Scenes.Forest;
		for (int i = 0; i < instance.activeZones.Count; i++)
		{
			MoonController.Scenes scenes2;
			if (this.zoneToSceneMapping.TryGetValue(instance.activeZones[i], ref scenes2) && scenes2 > scenes)
			{
				scenes = scenes2;
			}
		}
		this.UpdateActiveScene(scenes);
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x00042057 File Offset: 0x00040257
	private void UpdateActiveScene(MoonController.Scenes nextScene)
	{
		this.activeScene = nextScene;
		this.UpdateCrack();
		this.UpdatePlacement();
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0004206C File Offset: 0x0004026C
	private void Update()
	{
		this.UpdateCrack();
		if (!this.alwaysInTheSky)
		{
			float timeOfDay = this.TimeOfDay;
			bool flag = timeOfDay > 0.53999996f && timeOfDay < 0.6733333f;
			bool flag2 = timeOfDay > 0.086666666f && timeOfDay < 0.22f;
			bool flag3 = timeOfDay <= 0.086666666f || timeOfDay >= 0.6733333f;
			if (timeOfDay >= 0.22f)
			{
				bool flag4 = timeOfDay <= 0.53999996f;
			}
			float num = this.orbitAngle;
			if (flag)
			{
				num = Mathf.Lerp(3.1415927f, 0f, (timeOfDay - 0.53999996f) / 0.13333333f);
			}
			else if (flag2)
			{
				num = Mathf.Lerp(0f, -3.1415927f, (timeOfDay - 0.086666666f) / 0.13333333f);
			}
			else if (flag3)
			{
				num = 0f;
			}
			else
			{
				num = 3.1415927f;
			}
			if (this.orbitAngle != num)
			{
				this.orbitAngle = num;
				this.UpdateCrack();
				this.UpdatePlacement();
			}
		}
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0004215D File Offset: 0x0004035D
	public void UpdateDistance(float nextDistance)
	{
		this.distance = nextDistance;
		this.UpdateVisualState();
		this.UpdatePlacement();
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x00042174 File Offset: 0x00040374
	public void UpdateVisualState()
	{
		bool flag = false;
		if (GreyZoneManager.Instance != null)
		{
			flag = GreyZoneManager.Instance.GreyZoneActive;
		}
		if (flag && this.openEyeModelEnabled && this.distance < this.eyeOpenDistThreshold && !this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, true);
			return;
		}
		if (!flag && this.distance > this.eyeCloseDistThreshold && this.openMoonAnimator.GetBool(this.eyeOpenHash))
		{
			this.openMoonAnimator.SetBool(this.eyeOpenHash, false);
		}
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x00042214 File Offset: 0x00040414
	public void UpdatePlacement()
	{
		if (this.alwaysInTheSky)
		{
			this.UpdatePlacementSimple();
			return;
		}
		this.UpdatePlacementOrbit();
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0004222C File Offset: 0x0004042C
	private void UpdatePlacementSimple()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float num = Mathf.Lerp(placement.heightRange.x, placement.heightRange.y, this.distance);
		float num2 = Mathf.Lerp(placement.radiusRange.x, placement.radiusRange.y, this.distance);
		float num3 = Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		float restAngle = placement.restAngle;
		Vector3 position = referencePoint.position;
		position.y += num;
		position.x += num2 * Mathf.Cos(restAngle * 0.017453292f);
		position.z += num2 * Mathf.Sin(restAngle * 0.017453292f);
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * num3;
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x00042370 File Offset: 0x00040570
	public void UpdatePlacementOrbit()
	{
		MoonController.SceneData sceneData = this.scenes[(int)this.activeScene];
		Transform referencePoint = sceneData.referencePoint;
		MoonController.Placement placement = sceneData.overridePlacement ? sceneData.PlacementOverride : this.defaultPlacement;
		float y = placement.heightRange.y;
		float y2 = placement.radiusRange.y;
		Vector3 position = referencePoint.position;
		position.y += y;
		position.x += y2 * Mathf.Cos(placement.restAngle * 0.017453292f);
		position.z += y2 * Mathf.Sin(placement.restAngle * 0.017453292f);
		float num = Mathf.Sqrt(y * y + y2 * y2);
		float num2 = Mathf.Atan2(y, y2);
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * num2, Vector3.Cross(position - referencePoint.position, Vector3.up));
		float num3 = placement.restAngle * 0.017453292f + this.orbitAngle;
		Vector3 vector = referencePoint.position + quaternion * new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)) * num;
		if (this.distance < 1f)
		{
			Vector3 position2 = referencePoint.position;
			position2.y += placement.heightRange.x;
			position2.x += placement.radiusRange.x * Mathf.Cos(placement.restAngle * 0.017453292f);
			position2.z += placement.radiusRange.x * Mathf.Sin(placement.restAngle * 0.017453292f);
			if (Mathf.Abs(this.orbitAngle) < 0.9424779f)
			{
				vector = Vector3.Lerp(position2, vector, this.distance);
			}
			else
			{
				vector = Vector3.Lerp(position2, position, this.distance);
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(referencePoint.position - base.transform.position);
		base.transform.localScale = Vector3.one * Mathf.Lerp(placement.scaleRange.x, placement.scaleRange.y, this.distance);
		if (this.debugDrawOrbit)
		{
			int num4 = 32;
			float timeOfDay = this.TimeOfDay;
			float num5 = 0.086666666f;
			float num6 = 0.24666667f;
			float num7 = 0.6333333f;
			float num8 = 0.76f;
			bool flag = timeOfDay > num5 && timeOfDay < num6;
			bool flag2 = timeOfDay > num7 && timeOfDay < num8;
			bool flag3 = timeOfDay <= num5 || timeOfDay >= num8;
			if (timeOfDay >= num6)
			{
				bool flag4 = timeOfDay <= num7;
			}
			Color color = flag2 ? Color.red : (flag3 ? Color.green : (flag ? Color.yellow : Color.blue));
			Vector3 v = referencePoint.position + quaternion * new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * num;
			for (int i = 1; i <= num4; i++)
			{
				float num9 = (float)i / (float)num4;
				Vector3 vector2 = referencePoint.position + quaternion * new Vector3(Mathf.Cos(6.2831855f * num9), 0f, Mathf.Sin(6.2831855f * num9)) * num;
				DebugUtil.DrawLine(v, vector2, color, false);
				v = vector2;
			}
		}
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x00042700 File Offset: 0x00040900
	private void UpdateCrack()
	{
		bool flag = GreyZoneManager.Instance != null && GreyZoneManager.Instance.GreyZoneAvailable;
		if (flag && !this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = true;
			this.defaultMoon.gameObject.SetActive(false);
			this.openMoon.gameObject.SetActive(true);
		}
		else if (!flag && this.openEyeModelEnabled)
		{
			this.openEyeModelEnabled = false;
			this.defaultMoon.gameObject.SetActive(true);
			this.openMoon.gameObject.SetActive(false);
		}
		if (!flag && GorillaComputer.instance != null)
		{
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			if (this.debugOverrideCrackDayInOctober)
			{
				serverTime..ctor(2024, 10, Mathf.Clamp(this.crackDayInOctoberOverride, 1, 31));
			}
			float num = Mathf.InverseLerp((float)this.crackStartDayOfYear, (float)this.crackEndDayOfYear, (float)serverTime.DayOfYear);
			if (this.debugOverrideCrackProgress)
			{
				num = this.crackProgress;
			}
			float num2 = 1f - Mathf.Clamp01(num);
			if (this.crackRenderer != null && Mathf.Abs(num2 - this.currentlySetCrackProgress) > Mathf.Epsilon)
			{
				this.currentlySetCrackProgress = num2;
				this.crackMaterialPropertyBlock.SetFloat(ShaderProps._Progress, this.currentlySetCrackProgress);
				this.crackRenderer.SetPropertyBlock(this.crackMaterialPropertyBlock);
			}
		}
	}

	// Token: 0x04000EF6 RID: 3830
	[SerializeField]
	private List<MoonController.SceneData> scenes = new List<MoonController.SceneData>();

	// Token: 0x04000EF7 RID: 3831
	[SerializeField]
	private MoonController.Scenes activeScene;

	// Token: 0x04000EF8 RID: 3832
	[SerializeField]
	private MoonController.Placement defaultPlacement;

	// Token: 0x04000EF9 RID: 3833
	[SerializeField]
	[Range(0f, 1f)]
	private float distance;

	// Token: 0x04000EFA RID: 3834
	[SerializeField]
	private bool alwaysInTheSky;

	// Token: 0x04000EFB RID: 3835
	[Header("Model Swap")]
	[SerializeField]
	private Transform defaultMoon;

	// Token: 0x04000EFC RID: 3836
	[SerializeField]
	private Transform openMoon;

	// Token: 0x04000EFD RID: 3837
	[Header("Animation")]
	[SerializeField]
	private Animator openMoonAnimator;

	// Token: 0x04000EFE RID: 3838
	[SerializeField]
	private float eyeOpenDistThreshold = 0.9f;

	// Token: 0x04000EFF RID: 3839
	[SerializeField]
	private float eyeCloseDistThreshold = 0.05f;

	// Token: 0x04000F00 RID: 3840
	[Header("Debug")]
	[SerializeField]
	private bool debugOverrideTimeOfDay;

	// Token: 0x04000F01 RID: 3841
	[SerializeField]
	[Range(0f, 4f)]
	private float timeOfDayOverride;

	// Token: 0x04000F02 RID: 3842
	[SerializeField]
	private bool debugOverrideCrackProgress;

	// Token: 0x04000F03 RID: 3843
	[SerializeField]
	[Range(0f, 1f)]
	private float crackProgress;

	// Token: 0x04000F04 RID: 3844
	[SerializeField]
	private bool debugOverrideCrackDayInOctober;

	// Token: 0x04000F05 RID: 3845
	[SerializeField]
	[Range(1f, 31f)]
	private int crackDayInOctoberOverride = 4;

	// Token: 0x04000F06 RID: 3846
	[SerializeField]
	private MeshRenderer crackRenderer;

	// Token: 0x04000F07 RID: 3847
	private int crackStartDayOfYear;

	// Token: 0x04000F08 RID: 3848
	private int crackEndDayOfYear;

	// Token: 0x04000F09 RID: 3849
	private float orbitAngle;

	// Token: 0x04000F0A RID: 3850
	private int eyeOpenHash;

	// Token: 0x04000F0B RID: 3851
	private bool openEyeModelEnabled;

	// Token: 0x04000F0C RID: 3852
	private float currentlySetCrackProgress;

	// Token: 0x04000F0D RID: 3853
	private MaterialPropertyBlock crackMaterialPropertyBlock;

	// Token: 0x04000F0E RID: 3854
	private bool debugDrawOrbit;

	// Token: 0x04000F0F RID: 3855
	private Dictionary<GTZone, MoonController.Scenes> zoneToSceneMapping = new Dictionary<GTZone, MoonController.Scenes>();

	// Token: 0x04000F10 RID: 3856
	private const float moonFallStart = 0.086666666f;

	// Token: 0x04000F11 RID: 3857
	private const float moonFallEnd = 0.22f;

	// Token: 0x04000F12 RID: 3858
	private const float moonRiseStart = 0.53999996f;

	// Token: 0x04000F13 RID: 3859
	private const float moonRiseEnd = 0.6733333f;

	// Token: 0x020001C4 RID: 452
	public enum Scenes
	{
		// Token: 0x04000F15 RID: 3861
		Forest,
		// Token: 0x04000F16 RID: 3862
		Bayou,
		// Token: 0x04000F17 RID: 3863
		Beach,
		// Token: 0x04000F18 RID: 3864
		Canyon,
		// Token: 0x04000F19 RID: 3865
		Clouds,
		// Token: 0x04000F1A RID: 3866
		City,
		// Token: 0x04000F1B RID: 3867
		Metropolis,
		// Token: 0x04000F1C RID: 3868
		Mountain
	}

	// Token: 0x020001C5 RID: 453
	[Serializable]
	public struct SceneData
	{
		// Token: 0x04000F1D RID: 3869
		public MoonController.Scenes scene;

		// Token: 0x04000F1E RID: 3870
		public Transform referencePoint;

		// Token: 0x04000F1F RID: 3871
		public bool overridePlacement;

		// Token: 0x04000F20 RID: 3872
		public MoonController.Placement PlacementOverride;
	}

	// Token: 0x020001C6 RID: 454
	[Serializable]
	public struct Placement
	{
		// Token: 0x04000F21 RID: 3873
		public Vector2 radiusRange;

		// Token: 0x04000F22 RID: 3874
		public Vector2 heightRange;

		// Token: 0x04000F23 RID: 3875
		public Vector2 scaleRange;

		// Token: 0x04000F24 RID: 3876
		public float restAngle;
	}
}
