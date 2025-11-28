using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class PlayableBoundaryManager : MonoBehaviour
{
	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0004B982 File Offset: 0x00049B82
	// (set) Token: 0x06000E42 RID: 3650 RVA: 0x0004B99A File Offset: 0x00049B9A
	public static bool ShouldRender
	{
		get
		{
			return Shader.GetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled) > 0f;
		}
		set
		{
			Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_IsEnabled, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0004B9B3 File Offset: 0x00049BB3
	protected void Awake()
	{
		if (!Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x0004B9C4 File Offset: 0x00049BC4
	public void Setup()
	{
		Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
		Vector3 position = base.transform.position;
		SRand srand = new SRand(StaticHash.Compute(position.x, position.y, position.z));
		this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
		this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
		for (int i = 1; i < 8; i++)
		{
			Vector3 vector = position + srand.NextPointInsideSphere(this.m_bigCylinderRadius * this.radiusScale);
			this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
			this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
		}
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x0004BADC File Offset: 0x00049CDC
	private void OnEnable()
	{
		PlayableBoundaryManager.ShouldRender = true;
		this.Setup();
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0004BAEA File Offset: 0x00049CEA
	private void OnDisable()
	{
		PlayableBoundaryManager.ShouldRender = false;
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x0004BAF4 File Offset: 0x00049CF4
	public unsafe void UpdateSim()
	{
		if (Time.frameCount == this._lastFrameUpdated)
		{
			return;
		}
		this._lastFrameUpdated = Time.frameCount;
		Vector4[] array = this._cylinders_centers;
		if (array != null && array.Length == 8)
		{
			array = this._cylinders_radiusHeights;
			if (array != null && array.Length == 8)
			{
				if (this.m_smallCylindersMoveTimeScale > 0.0)
				{
					Vector3 position = base.transform.position;
					float num = (float)((double)(GTTime.TimeAsMilliseconds() % 86400000L) * this.m_smallCylindersMoveTimeScale / 1000.0);
					this._cylinders_centers[0] = new Vector3(position.x, position.y, position.z);
					this._cylinders_radiusHeights[0] = new Vector2(this.m_bigCylinderRadius * this.radiusScale, 100f);
					for (int i = 1; i < 8; i++)
					{
						float num2 = (float)i * 0.125f;
						Vector3 v = *PlayableBoundaryManager.Hash3(num2 * 1.17f) + *PlayableBoundaryManager.Hash3(num2 * 13.7f) * num;
						Vector3 vector = position + v.Sin() * this.m_bigCylinderRadius * this.radiusScale;
						this._cylinders_centers[i] = new Vector4(vector.x, vector.y, vector.z, 0f);
						this._cylinders_radiusHeights[i] = new Vector4(this.m_smallCylindersRadius * this.radiusScale, 100f, 0f, 0f);
					}
				}
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_Centers, this._cylinders_centers);
				Shader.SetGlobalVectorArray(PlayableBoundaryManager._GTGameModes_PlayableBoundary_Cylinders_RadiusHeights, this._cylinders_radiusHeights);
				for (int j = 0; j < this.tracked.Count; j++)
				{
					PlayableBoundaryTracker playableBoundaryTracker = this.tracked[j];
					if (playableBoundaryTracker)
					{
						playableBoundaryTracker.UpdateSignedDistanceToBoundary(this._GetSignedDistanceToBoundary(playableBoundaryTracker.transform.position, playableBoundaryTracker.radius), Time.deltaTime);
					}
				}
				Shader.SetGlobalFloat(PlayableBoundaryManager._GTGameModes_PlayableBoundary_NonZeroSmoothRadius, this.m_smoothFactor);
				return;
			}
		}
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0004BD30 File Offset: 0x00049F30
	[MethodImpl(256)]
	private float _GetSignedDistanceToBoundary(float3 tracked_center, float tracked_radius)
	{
		float num = float.MaxValue;
		float smoothFactor = this.GetSmoothFactor();
		for (int i = 0; i < 8; i++)
		{
			float3 @float = this._cylinders_centers[i].xyz - tracked_center;
			float x = this._cylinders_radiusHeights[i].x;
			float signedDist = math.length(@float.xz) - x;
			num = this.SDFSmoothMerge(num, signedDist, smoothFactor);
		}
		return num - tracked_radius;
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0004BDAC File Offset: 0x00049FAC
	[MethodImpl(256)]
	private float SDFSmoothMerge(float signedDist1, float signedDist2, float smoothRadius)
	{
		float num = -math.length(math.min(new float2(signedDist1 - smoothRadius, signedDist2 - smoothRadius), new float2(0f, 0f)));
		float num2 = math.max(math.min(signedDist1, signedDist2), smoothRadius);
		return num + num2;
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x0004BDF0 File Offset: 0x00049FF0
	private static ref Vector3 Hash3(float n)
	{
		PlayableBoundaryManager.kHashVec.x = Mathf.Sin(n) * 43758.547f % 1f;
		PlayableBoundaryManager.kHashVec.y = Mathf.Sin(n + 1f) * 22578.146f % 1f;
		PlayableBoundaryManager.kHashVec.z = Mathf.Sin(n + 2f) * 19642.35f % 1f;
		return ref PlayableBoundaryManager.kHashVec;
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x0004BE64 File Offset: 0x0004A064
	private float GetSmoothFactor()
	{
		float num = this.m_smoothFactor;
		if (this.m_bigCylinderRadius <= 1f)
		{
			num *= math.max(this.m_bigCylinderRadius, 0f);
		}
		return math.max(num, 1E-06f);
	}

	// Token: 0x04001149 RID: 4425
	public List<PlayableBoundaryTracker> tracked = new List<PlayableBoundaryTracker>(10);

	// Token: 0x0400114A RID: 4426
	[Space]
	[Range(0f, 128f)]
	public float m_bigCylinderRadius = 8f;

	// Token: 0x0400114B RID: 4427
	public float m_smoothFactor = 1.5f;

	// Token: 0x0400114C RID: 4428
	public float m_smallCylindersRadius = 3f;

	// Token: 0x0400114D RID: 4429
	[SerializeField]
	private double m_smallCylindersMoveTimeScale = 0.25;

	// Token: 0x0400114E RID: 4430
	[Space]
	private readonly Vector4[] _cylinders_centers = new Vector4[8];

	// Token: 0x0400114F RID: 4431
	private readonly Vector4[] _cylinders_radiusHeights = new Vector4[8];

	// Token: 0x04001150 RID: 4432
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_Centers = "_GTGameModes_PlayableBoundary_Cylinders_Centers";

	// Token: 0x04001151 RID: 4433
	private static ShaderHashId _GTGameModes_PlayableBoundary_Cylinders_RadiusHeights = "_GTGameModes_PlayableBoundary_Cylinders_RadiusHeights";

	// Token: 0x04001152 RID: 4434
	private static ShaderHashId _GTGameModes_PlayableBoundary_NonZeroSmoothRadius = "_GTGameModes_PlayableBoundary_NonZeroSmoothRadius";

	// Token: 0x04001153 RID: 4435
	private static ShaderHashId _GTGameModes_PlayableBoundary_IsEnabled = "_GTGameModes_PlayableBoundary_IsEnabled";

	// Token: 0x04001154 RID: 4436
	private const int _k_cylinders_count = 8;

	// Token: 0x04001155 RID: 4437
	[NonSerialized]
	public float radiusScale = 1f;

	// Token: 0x04001156 RID: 4438
	private int _lastFrameUpdated = -1;

	// Token: 0x04001157 RID: 4439
	private static Vector3 kHashVec = Vector3.zero;
}
