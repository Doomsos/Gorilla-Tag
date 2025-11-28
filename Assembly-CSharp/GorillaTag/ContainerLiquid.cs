using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FCD RID: 4045
	[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
	[ExecuteInEditMode]
	public class ContainerLiquid : MonoBehaviour
	{
		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06006681 RID: 26241 RVA: 0x002163C8 File Offset: 0x002145C8
		[DebugReadout]
		public bool isEmpty
		{
			get
			{
				return this.fillAmount <= this.refillThreshold;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06006682 RID: 26242 RVA: 0x002163DB File Offset: 0x002145DB
		// (set) Token: 0x06006683 RID: 26243 RVA: 0x002163E3 File Offset: 0x002145E3
		public Vector3 cupTopWorldPos { get; private set; }

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06006684 RID: 26244 RVA: 0x002163EC File Offset: 0x002145EC
		// (set) Token: 0x06006685 RID: 26245 RVA: 0x002163F4 File Offset: 0x002145F4
		public Vector3 bottomLipWorldPos { get; private set; }

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06006686 RID: 26246 RVA: 0x002163FD File Offset: 0x002145FD
		// (set) Token: 0x06006687 RID: 26247 RVA: 0x00216405 File Offset: 0x00214605
		public Vector3 liquidPlaneWorldPos { get; private set; }

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06006688 RID: 26248 RVA: 0x0021640E File Offset: 0x0021460E
		// (set) Token: 0x06006689 RID: 26249 RVA: 0x00216416 File Offset: 0x00214616
		public Vector3 liquidPlaneWorldNormal { get; private set; }

		// Token: 0x0600668A RID: 26250 RVA: 0x00216420 File Offset: 0x00214620
		protected bool IsValidLiquidSurfaceValues()
		{
			return this.meshRenderer != null && this.meshFilter != null && this.spillParticleSystem != null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
		}

		// Token: 0x0600668B RID: 26251 RVA: 0x00216484 File Offset: 0x00214684
		protected void InitializeLiquidSurface()
		{
			this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
			this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
			this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
			this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
		}

		// Token: 0x0600668C RID: 26252 RVA: 0x002164DC File Offset: 0x002146DC
		protected void InitializeParticleSystem()
		{
			this.spillParticleSystem.main.startColor = this.liquidColor;
		}

		// Token: 0x0600668D RID: 26253 RVA: 0x00216507 File Offset: 0x00214707
		protected void Awake()
		{
			this.matPropBlock = new MaterialPropertyBlock();
			this.topVerts = this.GetTopVerts();
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x00216520 File Offset: 0x00214720
		protected void OnEnable()
		{
			if (Application.isPlaying)
			{
				base.enabled = (this.useLiquidShader && this.IsValidLiquidSurfaceValues());
				if (base.enabled)
				{
					this.InitializeLiquidSurface();
				}
				this.InitializeParticleSystem();
				this.useFloater = (this.floater != null);
			}
		}

		// Token: 0x0600668F RID: 26255 RVA: 0x00216574 File Offset: 0x00214774
		protected void LateUpdate()
		{
			this.UpdateRefillTimer();
			Transform transform = base.transform;
			Vector3 position = transform.position;
			Quaternion rotation = transform.rotation;
			Bounds bounds = this.meshRenderer.bounds;
			Vector3 vector;
			vector..ctor(bounds.center.x, bounds.min.y, bounds.center.z);
			Vector3 vector2;
			vector2..ctor(bounds.center.x, bounds.max.y, bounds.center.z);
			this.liquidPlaneWorldPos = Vector3.Lerp(vector, vector2, this.fillAmount);
			Vector3 vector3 = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
			float deltaTime = Time.deltaTime;
			this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
			float num = 6.2831855f * this.wobbleFrequency;
			float num2 = Mathf.Lerp(this.lastSineWave, Mathf.Sin(num * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
			Vector2 vector4 = this.temporalWobbleAmp * num2;
			this.liquidPlaneWorldNormal = new Vector3(vector4.x, -1f, vector4.y).normalized;
			Vector3 vector5 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
			if (this.useLiquidShader)
			{
				this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, vector5);
				this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, vector3);
				this.matPropBlock.SetVector(this.liquidColorShaderProp, this.liquidColor.linear);
				if (this.useLiquidVolume)
				{
					float num3 = MathUtils.Linear(this.fillAmount, 0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
					this.matPropBlock.SetFloat(ShaderProps._LiquidFill, num3);
				}
				this.meshRenderer.SetPropertyBlock(this.matPropBlock);
			}
			if (this.useFloater)
			{
				float y = Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount);
				this.floater.localPosition = this.floater.localPosition.WithY(y);
			}
			Vector3 vector6 = (this.lastPos - position) / deltaTime;
			Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
			this.temporalWobbleAmp.x = this.temporalWobbleAmp.x + Mathf.Clamp((vector6.x + vector6.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.temporalWobbleAmp.y = this.temporalWobbleAmp.y + Mathf.Clamp((vector6.z + vector6.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
			this.lastPos = position;
			this.lastRot = rotation;
			this.lastSineWave = num2;
			this.lastVelocity = vector6;
			this.lastAngularVelocity = angularVelocity;
			this.meshRenderer.enabled = (!this.keepMeshHidden && !this.isEmpty);
			float x = transform.lossyScale.x;
			float num4 = this.localMeshBounds.extents.x * x;
			float y2 = this.localMeshBounds.extents.y;
			Vector3 vector7 = this.localMeshBounds.center + new Vector3(0f, y2, 0f);
			this.cupTopWorldPos = transform.TransformPoint(vector7);
			Vector3 up = transform.up;
			Vector3 vector8 = transform.InverseTransformDirection(Vector3.down);
			float num5 = float.MinValue;
			Vector3 vector9 = Vector3.zero;
			for (int i = 0; i < this.topVerts.Length; i++)
			{
				float num6 = Vector3.Dot(this.topVerts[i], vector8);
				if (num6 > num5)
				{
					num5 = num6;
					vector9 = this.topVerts[i];
				}
			}
			this.bottomLipWorldPos = transform.TransformPoint(vector9);
			float num7 = Mathf.Clamp01((this.liquidPlaneWorldPos.y - this.bottomLipWorldPos.y) / (num4 * 2f));
			bool flag = num7 > 1E-05f;
			ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission;
			emission.enabled = flag;
			if (flag)
			{
				if (!this.spillSoundBankPlayer.isPlaying)
				{
					this.spillSoundBankPlayer.Play();
				}
				this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, num7);
				this.spillParticleSystem.shape.radius = num4 * num7;
				ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
				float num8 = num7 * this.maxSpillRate;
				rateOverTime.constant = num8;
				emission.rateOverTime = rateOverTime;
				this.fillAmount -= num8 * deltaTime * 0.01f;
			}
			if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
			{
				this.emptySoundBankPlayer.Play();
			}
			else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
			{
				this.refillSoundBankPlayer.Play();
			}
			this.wasEmptyLastFrame = this.isEmpty;
		}

		// Token: 0x06006690 RID: 26256 RVA: 0x00216B00 File Offset: 0x00214D00
		public void UpdateRefillTimer()
		{
			if (this.refillDelay < 0f || !this.isEmpty)
			{
				return;
			}
			if (this.refillTimer < 0f)
			{
				this.refillTimer = this.refillDelay;
				this.fillAmount = this.refillAmount;
				return;
			}
			this.refillTimer -= Time.deltaTime;
		}

		// Token: 0x06006691 RID: 26257 RVA: 0x00216B5C File Offset: 0x00214D5C
		private Vector3[] GetTopVerts()
		{
			Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
			List<Vector3> list = new List<Vector3>(vertices.Length);
			float num = float.MinValue;
			foreach (Vector3 vector in vertices)
			{
				if (vector.y > num)
				{
					num = vector.y;
				}
			}
			foreach (Vector3 vector2 in vertices)
			{
				if (Mathf.Abs(vector2.y - num) < 0.001f)
				{
					list.Add(vector2);
				}
			}
			return list.ToArray();
		}

		// Token: 0x04007522 RID: 29986
		[Tooltip("Used to determine the world space bounds of the container.")]
		public MeshRenderer meshRenderer;

		// Token: 0x04007523 RID: 29987
		[Tooltip("Used to determine the local space bounds of the container.")]
		public MeshFilter meshFilter;

		// Token: 0x04007524 RID: 29988
		[Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
		public bool keepMeshHidden;

		// Token: 0x04007525 RID: 29989
		[Tooltip("The object that will float on top of the liquid.")]
		public Transform floater;

		// Token: 0x04007526 RID: 29990
		public bool useLiquidShader = true;

		// Token: 0x04007527 RID: 29991
		public bool useLiquidVolume;

		// Token: 0x04007528 RID: 29992
		public Vector2 liquidVolumeMinMax = Vector2.up;

		// Token: 0x04007529 RID: 29993
		public string liquidColorShaderPropertyName = "_BaseColor";

		// Token: 0x0400752A RID: 29994
		public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";

		// Token: 0x0400752B RID: 29995
		public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";

		// Token: 0x0400752C RID: 29996
		[Tooltip("Emits drips when pouring.")]
		public ParticleSystem spillParticleSystem;

		// Token: 0x0400752D RID: 29997
		[SoundBankInfo]
		public SoundBankPlayer emptySoundBankPlayer;

		// Token: 0x0400752E RID: 29998
		[SoundBankInfo]
		public SoundBankPlayer refillSoundBankPlayer;

		// Token: 0x0400752F RID: 29999
		[SoundBankInfo]
		public SoundBankPlayer spillSoundBankPlayer;

		// Token: 0x04007530 RID: 30000
		public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);

		// Token: 0x04007531 RID: 30001
		[Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
		[Range(0f, 1f)]
		public float fillAmount = 0.85f;

		// Token: 0x04007532 RID: 30002
		[Tooltip("This is what fillAmount will be after automatic refilling.")]
		public float refillAmount = 0.85f;

		// Token: 0x04007533 RID: 30003
		[Tooltip("Set to a negative value to disable.")]
		public float refillDelay = 10f;

		// Token: 0x04007534 RID: 30004
		[Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
		public float refillThreshold = 0.1f;

		// Token: 0x04007535 RID: 30005
		public float wobbleMax = 0.2f;

		// Token: 0x04007536 RID: 30006
		public float wobbleFrequency = 1f;

		// Token: 0x04007537 RID: 30007
		public float recovery = 1f;

		// Token: 0x04007538 RID: 30008
		public float thickness = 1f;

		// Token: 0x04007539 RID: 30009
		public float maxSpillRate = 100f;

		// Token: 0x0400753E RID: 30014
		[DebugReadout]
		private bool wasEmptyLastFrame;

		// Token: 0x0400753F RID: 30015
		private int liquidColorShaderProp;

		// Token: 0x04007540 RID: 30016
		private int liquidPlaneNormalShaderProp;

		// Token: 0x04007541 RID: 30017
		private int liquidPlanePositionShaderProp;

		// Token: 0x04007542 RID: 30018
		private float refillTimer;

		// Token: 0x04007543 RID: 30019
		private float lastSineWave;

		// Token: 0x04007544 RID: 30020
		private float lastWobble;

		// Token: 0x04007545 RID: 30021
		private Vector2 temporalWobbleAmp;

		// Token: 0x04007546 RID: 30022
		private Vector3 lastPos;

		// Token: 0x04007547 RID: 30023
		private Vector3 lastVelocity;

		// Token: 0x04007548 RID: 30024
		private Vector3 lastAngularVelocity;

		// Token: 0x04007549 RID: 30025
		private Quaternion lastRot;

		// Token: 0x0400754A RID: 30026
		private MaterialPropertyBlock matPropBlock;

		// Token: 0x0400754B RID: 30027
		private Bounds localMeshBounds;

		// Token: 0x0400754C RID: 30028
		private bool useFloater;

		// Token: 0x0400754D RID: 30029
		private Vector3[] topVerts;
	}
}
