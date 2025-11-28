using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001114 RID: 4372
	public class ShakeReactorCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x06006D70 RID: 28016 RVA: 0x0023ED40 File Offset: 0x0023CF40
		private void OnEnable()
		{
			this.lastReversalTime = Time.time;
			this.pathSinceLastReversal = 0f;
			this.recentHalfCycleDurations.Clear();
			this.hasLastDir = false;
			this.lastPosition = ((this.speedTracker != null) ? this.speedTracker.transform.position : base.transform.position);
			this.isShaking = false;
			this.debugCurrentHalfCycleDistance = 0f;
			this.debugCurrentRateHz = 0f;
			this.lastAmplitudeMeters = 0f;
			this.nextAllowedShakeStartTime = Time.time;
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = (this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (!this.subscribed && this._events.Activate != null)
			{
				this._events.Activate.reliable = true;
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShake);
				this.subscribed = true;
			}
		}

		// Token: 0x06006D71 RID: 28017 RVA: 0x0023EEAC File Offset: 0x0023D0AC
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShake);
				this.subscribed = false;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006D72 RID: 28018 RVA: 0x0023EF04 File Offset: 0x0023D104
		private void Update()
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			if (this.speedTracker == null)
			{
				if (this.isShaking)
				{
					this.isShaking = false;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeEndShared = this.ShakeEndShared;
					if (shakeEndShared != null)
					{
						shakeEndShared.Invoke();
					}
					UnityEvent shakeEndLocal = this.ShakeEndLocal;
					if (shakeEndLocal != null)
					{
						shakeEndLocal.Invoke();
					}
					this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
				}
				return;
			}
			Vector3 position = this.speedTracker.transform.position;
			float magnitude = (position - this.lastPosition).magnitude;
			if (magnitude > 0f)
			{
				this.pathSinceLastReversal += magnitude;
				this.debugCurrentHalfCycleDistance = this.pathSinceLastReversal;
			}
			Vector3 worldVelocity = this.speedTracker.GetWorldVelocity();
			float magnitude2 = worldVelocity.magnitude;
			Vector3 vector = (worldVelocity.sqrMagnitude > 1E-06f) ? worldVelocity.normalized : this.lastVelocityDir;
			bool flag = false;
			if (this.hasLastDir)
			{
				if (Vector3.Angle(this.lastVelocityDir, vector) >= this.angleToleranceDeg && magnitude2 >= this.minSpeedForReversal)
				{
					float num = Time.time - this.lastReversalTime;
					if (num > 0.0005f)
					{
						this.EnqueueHalfCycle(num);
						this.lastAmplitudeMeters = this.pathSinceLastReversal;
						this.lastReversalTime = Time.time;
						this.pathSinceLastReversal = 0f;
						flag = true;
					}
				}
			}
			else
			{
				this.hasLastDir = true;
				this.lastVelocityDir = vector;
				this.lastReversalTime = Time.time;
			}
			this.lastVelocityDir = vector;
			this.lastPosition = position;
			float averageHalfCycleDuration = this.GetAverageHalfCycleDuration();
			float num2 = Time.time - this.lastReversalTime;
			float num3 = Mathf.Max((averageHalfCycleDuration > 1E-05f) ? averageHalfCycleDuration : float.PositiveInfinity, num2);
			float num4 = (num3 < float.PositiveInfinity) ? (0.5f / num3) : 0f;
			this.debugCurrentRateHz = num4;
			bool flag2 = num4 >= this.shakeRateThreshold;
			bool flag3 = this.lastAmplitudeMeters >= this.shakeAmplitudeThreshold;
			if (!this.isShaking)
			{
				if (Time.time >= this.nextAllowedShakeStartTime && flag2 && flag3)
				{
					this.isShaking = true;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeStartLocal = this.ShakeStartLocal;
					if (shakeStartLocal != null)
					{
						shakeStartLocal.Invoke();
					}
					UnityEvent shakeStartShared = this.ShakeStartShared;
					if (shakeStartShared != null)
					{
						shakeStartShared.Invoke();
					}
				}
			}
			else
			{
				float num5 = (this.shakeRateThreshold > 1E-05f) ? (0.5f / this.shakeRateThreshold) : float.PositiveInfinity;
				float num6 = 1f * num5;
				bool flag4 = Time.time - this.lastReversalTime > num6;
				if ((!flag2 && !flag) || flag4)
				{
					this.isShaking = false;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeEndLocal2 = this.ShakeEndLocal;
					if (shakeEndLocal2 != null)
					{
						shakeEndLocal2.Invoke();
					}
					UnityEvent shakeEndShared2 = this.ShakeEndShared;
					if (shakeEndShared2 != null)
					{
						shakeEndShared2.Invoke();
					}
					this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
				}
			}
			if (this.useMaxes && this.isShaking)
			{
				bool flag5 = num4 >= this.maxShakeRate;
				bool flag6 = this.lastAmplitudeMeters >= this.maxShakeAmplitude;
				if (flag5 || flag6)
				{
					UnityEvent maxShake = this.MaxShake;
					if (maxShake != null)
					{
						maxShake.Invoke();
					}
				}
			}
			float strength = 0f;
			if (this.isShaking)
			{
				float num7 = Mathf.Max(1E-05f, this.shakeAmplitudeThreshold);
				if (this.useMaxes && this.maxShakeAmplitude > num7)
				{
					strength = Mathf.InverseLerp(num7, this.maxShakeAmplitude, this.lastAmplitudeMeters);
				}
				else
				{
					float num8 = Mathf.Max(num7, this.shakeAmplitudeThreshold * Mathf.Max(1f, this.softMaxMultiplier));
					strength = Mathf.InverseLerp(num7, num8, this.lastAmplitudeMeters);
				}
			}
			this.ApplyStrength(strength);
		}

		// Token: 0x06006D73 RID: 28019 RVA: 0x0023F3B4 File Offset: 0x0023D5B4
		private void EnqueueHalfCycle(float duration)
		{
			this.recentHalfCycleDurations.Enqueue(duration);
			while (this.recentHalfCycleDurations.Count > Mathf.Max(1, 1))
			{
				this.recentHalfCycleDurations.Dequeue();
			}
		}

		// Token: 0x06006D74 RID: 28020 RVA: 0x0023F3E4 File Offset: 0x0023D5E4
		private float GetAverageHalfCycleDuration()
		{
			if (this.recentHalfCycleDurations.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in this.recentHalfCycleDurations)
			{
				num += num2;
			}
			return num / (float)this.recentHalfCycleDurations.Count;
		}

		// Token: 0x06006D75 RID: 28021 RVA: 0x0023F45C File Offset: 0x0023D65C
		private void ApplyStrength(float strength01)
		{
			if (this.continuousProperties != null)
			{
				this.continuousProperties.ApplyAll(strength01);
			}
		}

		// Token: 0x06006D76 RID: 28022 RVA: 0x0023F474 File Offset: 0x0023D674
		private void OnShake(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShake");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent shakeStartShared = this.ShakeStartShared;
				if (shakeStartShared == null)
				{
					return;
				}
				shakeStartShared.Invoke();
				return;
			}
			else
			{
				UnityEvent shakeEndShared = this.ShakeEndShared;
				if (shakeEndShared == null)
				{
					return;
				}
				shakeEndShared.Invoke();
				return;
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06006D77 RID: 28023 RVA: 0x0023F4FF File Offset: 0x0023D6FF
		// (set) Token: 0x06006D78 RID: 28024 RVA: 0x0023F507 File Offset: 0x0023D707
		public bool IsSpawned { get; set; }

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06006D79 RID: 28025 RVA: 0x0023F510 File Offset: 0x0023D710
		// (set) Token: 0x06006D7A RID: 28026 RVA: 0x0023F518 File Offset: 0x0023D718
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006D7B RID: 28027 RVA: 0x0023F521 File Offset: 0x0023D721
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06006D7C RID: 28028 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x04007EB9 RID: 32441
		[Header("Speed Source")]
		[Tooltip("Speed component provider")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x04007EBA RID: 32442
		[Header("Settings")]
		[Tooltip("Minimum reversals-per-second required to consider motion a shake - Hz.")]
		[SerializeField]
		private float shakeRateThreshold = 1f;

		// Token: 0x04007EBB RID: 32443
		[Tooltip("Minimum distance traveled between direction reversals to count as a valid half-cycle.")]
		[SerializeField]
		private float shakeAmplitudeThreshold = 0.1f;

		// Token: 0x04007EBC RID: 32444
		[Tooltip("Minimum angle change (degrees) between consecutive lobes to register a reversal. Higher = stricter.")]
		[SerializeField]
		[Range(10f, 170f)]
		private float angleToleranceDeg = 120f;

		// Token: 0x04007EBD RID: 32445
		[Tooltip("Minimum speed required to accept a direction reversal, ignores tiny jitter near stop.")]
		[SerializeField]
		private float minSpeedForReversal = 0.2f;

		// Token: 0x04007EBE RID: 32446
		[Tooltip("After a shake ends, how long to wait before ShakeStartLocal can fire again")]
		[SerializeField]
		private float startCooldownSeconds = 0.2f;

		// Token: 0x04007EBF RID: 32447
		[SerializeField]
		private bool useMaxes;

		// Token: 0x04007EC0 RID: 32448
		[Tooltip("If enabled, exceeding this rate is considered a max shake.")]
		[SerializeField]
		private float maxShakeRate = 6f;

		// Token: 0x04007EC1 RID: 32449
		[Tooltip("If enabled, exceeding this amplitude per half cycle is considered a max shake.")]
		[SerializeField]
		private float maxShakeAmplitude = 0.3f;

		// Token: 0x04007EC2 RID: 32450
		[Header("Continuous Output")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04007EC3 RID: 32451
		[Header("Advanced")]
		[Tooltip("When no hard max amplitude is defined, strength is mapped to Threshold × this multiplier.")]
		[SerializeField]
		private float softMaxMultiplier = 3f;

		// Token: 0x04007EC4 RID: 32452
		[FormerlySerializedAs("ShakeStart")]
		[Header("Events")]
		public UnityEvent ShakeStartLocal;

		// Token: 0x04007EC5 RID: 32453
		public UnityEvent ShakeStartShared;

		// Token: 0x04007EC6 RID: 32454
		[FormerlySerializedAs("ShakeEnd")]
		public UnityEvent ShakeEndLocal;

		// Token: 0x04007EC7 RID: 32455
		public UnityEvent ShakeEndShared;

		// Token: 0x04007EC8 RID: 32456
		public UnityEvent MaxShake;

		// Token: 0x04007EC9 RID: 32457
		[Header("Debug")]
		public bool isShaking;

		// Token: 0x04007ECA RID: 32458
		public float lastAmplitudeMeters;

		// Token: 0x04007ECB RID: 32459
		public float debugCurrentHalfCycleDistance;

		// Token: 0x04007ECC RID: 32460
		public float debugCurrentRateHz;

		// Token: 0x04007ECD RID: 32461
		private const int kFrequencyHistoryCount = 1;

		// Token: 0x04007ECE RID: 32462
		private const float kNoReversalGraceMultiplier = 1f;

		// Token: 0x04007ECF RID: 32463
		private readonly Queue<float> recentHalfCycleDurations = new Queue<float>();

		// Token: 0x04007ED0 RID: 32464
		private Vector3 lastVelocityDir;

		// Token: 0x04007ED1 RID: 32465
		private bool hasLastDir;

		// Token: 0x04007ED2 RID: 32466
		private float lastReversalTime;

		// Token: 0x04007ED3 RID: 32467
		private Vector3 lastPosition;

		// Token: 0x04007ED4 RID: 32468
		private float pathSinceLastReversal;

		// Token: 0x04007ED5 RID: 32469
		private float nextAllowedShakeStartTime;

		// Token: 0x04007ED6 RID: 32470
		private const float kEpsilon = 1E-05f;

		// Token: 0x04007ED7 RID: 32471
		private const float kTinyVelocitySqr = 1E-06f;

		// Token: 0x04007ED8 RID: 32472
		private const float kMinHalfCycleDuration = 0.0005f;

		// Token: 0x04007ED9 RID: 32473
		private const float kHalfPerCycle = 0.5f;

		// Token: 0x04007EDA RID: 32474
		private RubberDuckEvents _events;

		// Token: 0x04007EDB RID: 32475
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04007EDC RID: 32476
		private VRRig myRig;

		// Token: 0x04007EDD RID: 32477
		private bool subscribed;
	}
}
