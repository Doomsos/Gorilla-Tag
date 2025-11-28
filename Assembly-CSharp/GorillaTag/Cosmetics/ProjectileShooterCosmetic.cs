using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200110F RID: 4367
	public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06006D56 RID: 27990 RVA: 0x0023E1E8 File Offset: 0x0023C3E8
		private bool IsMovementShoot()
		{
			return this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold;
		}

		// Token: 0x06006D57 RID: 27991 RVA: 0x0023E1F3 File Offset: 0x0023C3F3
		private bool IsRigDirection()
		{
			return this.shootDirectionType == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform;
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06006D58 RID: 27992 RVA: 0x0023E1FE File Offset: 0x0023C3FE
		// (set) Token: 0x06006D59 RID: 27993 RVA: 0x0023E206 File Offset: 0x0023C406
		public bool shootingAllowed { get; set; } = true;

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06006D5A RID: 27994 RVA: 0x0023E20F File Offset: 0x0023C40F
		private bool IsCoolingDown
		{
			get
			{
				return this.cooldownRemaining > 0f;
			}
		}

		// Token: 0x06006D5B RID: 27995 RVA: 0x0023E220 File Offset: 0x0023C420
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
			this.rig = ((this.transferrableObject == null) ? base.GetComponentInParent<VRRig>() : this.transferrableObject.ownerRig);
			UnityEvent<int> unityEvent = this.onMovedToNextStep;
			if (unityEvent != null)
			{
				unityEvent.Invoke(this.currentStep);
			}
			this.isLocal = ((this.transferrableObject != null && this.transferrableObject.IsMyItem()) || (this.rig != null && this.rig == GorillaTagger.Instance.offlineVRRig));
		}

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x06006D5C RID: 27996 RVA: 0x0023E2C1 File Offset: 0x0023C4C1
		// (set) Token: 0x06006D5D RID: 27997 RVA: 0x0023E2C9 File Offset: 0x0023C4C9
		public bool TickRunning { get; set; }

		// Token: 0x06006D5E RID: 27998 RVA: 0x0023E2D4 File Offset: 0x0023C4D4
		public void Tick()
		{
			if (this.IsCoolingDown)
			{
				this.cooldownRemaining -= Time.deltaTime;
				if (this.cooldownRemaining <= 0f)
				{
					this.cooldownRemaining = 0f;
					UnityEvent unityEvent = this.onCooldownFinished;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					if (this.isPressed)
					{
						this.SetPressState(true);
					}
					if (!this.allowCharging && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						TickSystem<object>.RemoveTickCallback(this);
					}
				}
			}
			if (!this.IsCoolingDown && this.allowCharging)
			{
				if (this.isPressed)
				{
					if (this.chargeTime < this.maxChargeSeconds)
					{
						this.chargeTime += Time.deltaTime;
						if (this.chargeTime >= this.maxChargeSeconds || this.chargeTime >= this.snapToMaxChargeAt)
						{
							this.chargeTime = this.maxChargeSeconds;
							UnityEvent unityEvent2 = this.onMaxCharge;
							if (unityEvent2 != null)
							{
								unityEvent2.Invoke();
							}
						}
					}
					float chargeFrac = this.GetChargeFrac();
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(chargeFrac);
					}
					UnityEvent<float> unityEvent3 = this.whileCharging;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(chargeFrac);
					}
					this.TryRunHaptics((chargeFrac >= 1f) ? this.maxChargeHapticsIntensity : (chargeFrac * this.chargeHapticsIntensity), Time.deltaTime);
					this.lastStep = this.currentStep;
					this.currentStep = Mathf.Clamp(Mathf.FloorToInt(chargeFrac * (float)this.numberOfProgressSteps), 0, this.numberOfProgressSteps - 1);
					if (this.currentStep >= 0 && this.currentStep != this.lastStep)
					{
						UnityEvent<int> unityEvent4 = this.onMovedToNextStep;
						if (unityEvent4 != null)
						{
							unityEvent4.Invoke(this.currentStep);
						}
						if (this.currentStep == this.numberOfProgressSteps - 1)
						{
							UnityEvent<int> unityEvent5 = this.onReachedLastProgressStep;
							if (unityEvent5 != null)
							{
								unityEvent5.Invoke(this.currentStep);
							}
						}
					}
					if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
					{
						Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
						float num = linearVelocity.magnitude;
						float num2 = Vector3.Dot(linearVelocity / num, this.GetVectorFromBodyToLaunchPosition().normalized);
						num *= Mathf.Ceil(num2 - this.velocityEstimatorMinRigDotProduct);
						if (num >= this.velocityEstimatorStartGestureSpeed)
						{
							this.velocityEstimatorThresholdMet = true;
							return;
						}
						if (this.velocityEstimatorThresholdMet && num < this.velocityEstimatorStopGestureSpeed)
						{
							this.TryShoot();
							return;
						}
					}
				}
				else if (this.chargeTime > 0f)
				{
					this.chargeTime -= Time.deltaTime * this.chargeDecaySpeed;
					if (this.chargeTime <= 0f)
					{
						this.chargeTime = 0f;
						TickSystem<object>.RemoveTickCallback(this);
						ContinuousPropertyArray continuousPropertyArray2 = this.continuousChargingProperties;
						if (continuousPropertyArray2 != null)
						{
							continuousPropertyArray2.ApplyAll(0f);
						}
						UnityEvent<float> unityEvent6 = this.whileCharging;
						if (unityEvent6 == null)
						{
							return;
						}
						unityEvent6.Invoke(0f);
						return;
					}
					else
					{
						float chargeFrac2 = this.GetChargeFrac();
						ContinuousPropertyArray continuousPropertyArray3 = this.continuousChargingProperties;
						if (continuousPropertyArray3 != null)
						{
							continuousPropertyArray3.ApplyAll(chargeFrac2);
						}
						UnityEvent<float> unityEvent7 = this.whileCharging;
						if (unityEvent7 == null)
						{
							return;
						}
						unityEvent7.Invoke(chargeFrac2);
					}
				}
			}
		}

		// Token: 0x06006D5F RID: 27999 RVA: 0x0023E5B1 File Offset: 0x0023C7B1
		private Vector3 GetVectorFromBodyToLaunchPosition()
		{
			return this.shootFromTransform.position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition);
		}

		// Token: 0x06006D60 RID: 28000 RVA: 0x0023E5DC File Offset: 0x0023C7DC
		private void GetShootPositionAndRotation(out Vector3 position, out Quaternion rotation)
		{
			ProjectileShooterCosmetic.ShootDirection shootDirection = this.shootDirectionType;
			if (shootDirection != ProjectileShooterCosmetic.ShootDirection.LaunchTransformRotation && shootDirection == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform)
			{
				position = this.shootFromTransform.position;
				rotation = Quaternion.LookRotation(position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition));
				return;
			}
			this.shootFromTransform.GetPositionAndRotation(ref position, ref rotation);
		}

		// Token: 0x06006D61 RID: 28001 RVA: 0x0023E644 File Offset: 0x0023C844
		private void Shoot()
		{
			float chargeFrac = this.GetChargeFrac();
			float num = Mathf.Lerp(this.shootMinSpeed, this.shootMaxSpeed, this.chargeToShotSpeedCurve.Evaluate(chargeFrac));
			GameObject gameObject = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
			gameObject.transform.localScale = Vector3.one * this.rig.scaleFactor;
			IProjectile component = gameObject.GetComponent<IProjectile>();
			if (component != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.GetShootPositionAndRotation(out vector, out quaternion);
				Vector3 velocity = quaternion * Vector3.forward * (num * this.rig.scaleFactor);
				component.Launch(vector, quaternion, velocity, chargeFrac, this.rig, this.currentStep);
				if (this.projectileTrailPrefab != -1)
				{
					this.AttachTrail(this.projectileTrailPrefab, gameObject, vector, false, false);
				}
			}
			UnityEvent<float> unityEvent = this.onShoot;
			if (unityEvent != null)
			{
				unityEvent.Invoke(chargeFrac);
			}
			this.continuousChargingProperties.ApplyAll(0f);
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			if (this.isLocal)
			{
				UnityEvent<float> unityEvent3 = this.onShootLocal;
				if (unityEvent3 != null)
				{
					unityEvent3.Invoke(chargeFrac);
				}
			}
			if (this.allowCharging && this.runChargeCancelledEventOnShoot)
			{
				UnityEvent unityEvent4 = this.onChargeCancelled;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			this.TryRunHaptics(chargeFrac * this.shootHapticsIntensity, this.shootHapticsDuration);
			this.SetPressState(false);
			this.cooldownRemaining = this.cooldownSeconds;
			this.chargeTime = 0f;
			this.currentStep = -1;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006D62 RID: 28002 RVA: 0x0023E7D0 File Offset: 0x0023C9D0
		private bool TryShoot()
		{
			if ((!this.IsCoolingDown && this.shootingAllowed && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge && this.chargeTime >= this.maxChargeSeconds))
			{
				this.Shoot();
				return true;
			}
			return false;
		}

		// Token: 0x06006D63 RID: 28003 RVA: 0x0023E80C File Offset: 0x0023CA0C
		private void TryRunHaptics(float intensity, float duration)
		{
			if (!this.enableHaptics || !this.isLocal || intensity <= 0f)
			{
				return;
			}
			bool flag = this.transferrableObject != null && this.transferrableObject.InLeftHand();
			GorillaTagger.Instance.StartVibration(flag, intensity, duration);
			if (this.hapticsBothHands)
			{
				GorillaTagger.Instance.StartVibration(!flag, intensity, duration);
			}
		}

		// Token: 0x06006D64 RID: 28004 RVA: 0x0023E874 File Offset: 0x0023CA74
		private float GetChargeFrac()
		{
			if (!this.allowCharging)
			{
				return 1f;
			}
			if (this.chargeTime <= 0f)
			{
				return 0f;
			}
			if (this.chargeTime < this.maxChargeSeconds)
			{
				return this.chargeRateCurve.Evaluate(this.chargeTime / this.maxChargeSeconds);
			}
			return 1f;
		}

		// Token: 0x06006D65 RID: 28005 RVA: 0x0023E8CE File Offset: 0x0023CACE
		private void SetPressState(bool pressed)
		{
			this.isPressed = pressed;
			this.velocityEstimatorThresholdMet = false;
		}

		// Token: 0x06006D66 RID: 28006 RVA: 0x0023E8DE File Offset: 0x0023CADE
		public void OnButtonPressed()
		{
			this.SetPressState(true);
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonPressed)
			{
				this.TryShoot();
				return;
			}
			if (this.allowCharging || this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006D67 RID: 28007 RVA: 0x0023E910 File Offset: 0x0023CB10
		public void OnButtonReleased()
		{
			if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold && this.velocityEstimatorThresholdMet)
			{
				return;
			}
			ProjectileShooterCosmetic.ShootActivator shootActivator = this.shootActivatorType;
			if ((shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleased && shootActivator != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) || !this.TryShoot())
			{
				this.SetPressState(false);
				if (this.allowCharging)
				{
					ContinuousPropertyArray continuousPropertyArray = this.continuousChargingProperties;
					if (continuousPropertyArray != null)
					{
						continuousPropertyArray.ApplyAll(0f);
					}
					UnityEvent<float> unityEvent = this.whileCharging;
					if (unityEvent != null)
					{
						unityEvent.Invoke(0f);
					}
					UnityEvent unityEvent2 = this.onChargeCancelled;
					if (unityEvent2 == null)
					{
						return;
					}
					unityEvent2.Invoke();
				}
			}
		}

		// Token: 0x06006D68 RID: 28008 RVA: 0x0023E993 File Offset: 0x0023CB93
		public void ResetShoot()
		{
			this.isPressed = false;
			this.velocityEstimatorThresholdMet = false;
			this.currentStep = -1;
			this.lastStep = -1;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006D69 RID: 28009 RVA: 0x0023E9B8 File Offset: 0x0023CBB8
		private void AttachTrail(int trailHash, GameObject newProjectile, Vector3 location, bool blueTeam, bool orangeTeam)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(trailHash, true);
			SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
			if (component.IsNull())
			{
				ObjectPools.instance.Destroy(gameObject);
			}
			newProjectile.transform.position = location;
			component.AttachTrail(newProjectile, blueTeam, orangeTeam, false, default(Color));
		}

		// Token: 0x04007E72 RID: 32370
		private const string CHARGE_STR = "allowCharging";

		// Token: 0x04007E73 RID: 32371
		private const string CHARGE_MSG = "only enabled when allowCharging is true.";

		// Token: 0x04007E74 RID: 32372
		private const string HAPTICS_STR = "enableHaptics";

		// Token: 0x04007E75 RID: 32373
		private const string MOVE_STR = "IsMovementShoot";

		// Token: 0x04007E76 RID: 32374
		[SerializeField]
		private HashWrapper projectilePrefab;

		// Token: 0x04007E77 RID: 32375
		[SerializeField]
		private HashWrapper projectileTrailPrefab;

		// Token: 0x04007E78 RID: 32376
		[FormerlySerializedAs("launchActivatorType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootActivator shootActivatorType;

		// Token: 0x04007E79 RID: 32377
		[FormerlySerializedAs("launchDirectionType")]
		[SerializeField]
		private ProjectileShooterCosmetic.ShootDirection shootDirectionType;

		// Token: 0x04007E7A RID: 32378
		[SerializeField]
		private Vector3 offsetRigPosition;

		// Token: 0x04007E7B RID: 32379
		[FormerlySerializedAs("launchTransform")]
		[SerializeField]
		private Transform shootFromTransform;

		// Token: 0x04007E7C RID: 32380
		[SerializeField]
		private bool drawShootVector;

		// Token: 0x04007E7D RID: 32381
		[FormerlySerializedAs("cooldown")]
		[SerializeField]
		private float cooldownSeconds;

		// Token: 0x04007E7E RID: 32382
		[Space]
		[SerializeField]
		private bool enableHaptics = true;

		// Token: 0x04007E7F RID: 32383
		[FormerlySerializedAs("hapticsIntensity")]
		[SerializeField]
		private float shootHapticsIntensity = 0.5f;

		// Token: 0x04007E80 RID: 32384
		[FormerlySerializedAs("hapticsDuration")]
		[SerializeField]
		private float shootHapticsDuration = 0.2f;

		// Token: 0x04007E81 RID: 32385
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float chargeHapticsIntensity = 0.3f;

		// Token: 0x04007E82 RID: 32386
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float maxChargeHapticsIntensity = 0.3f;

		// Token: 0x04007E83 RID: 32387
		[SerializeField]
		private bool hapticsBothHands;

		// Token: 0x04007E84 RID: 32388
		[Space]
		[SerializeField]
		private GorillaVelocityEstimator velocityEstimator;

		// Token: 0x04007E85 RID: 32389
		[SerializeField]
		private float velocityEstimatorStartGestureSpeed = 0.5f;

		// Token: 0x04007E86 RID: 32390
		[SerializeField]
		private float velocityEstimatorStopGestureSpeed = 0.2f;

		// Token: 0x04007E87 RID: 32391
		[SerializeField]
		private float velocityEstimatorMinRigDotProduct = 0.5f;

		// Token: 0x04007E88 RID: 32392
		[SerializeField]
		private bool logVelocityEstimatorSpeed;

		// Token: 0x04007E89 RID: 32393
		[FormerlySerializedAs("launchMinSpeed")]
		[SerializeField]
		[Tooltip("only enabled when allowCharging is true.")]
		private float shootMinSpeed;

		// Token: 0x04007E8A RID: 32394
		[FormerlySerializedAs("launchMaxSpeed")]
		[SerializeField]
		private float shootMaxSpeed;

		// Token: 0x04007E8B RID: 32395
		[SerializeField]
		private bool allowCharging;

		// Token: 0x04007E8C RID: 32396
		[SerializeField]
		private float maxChargeSeconds = 2f;

		// Token: 0x04007E8D RID: 32397
		[SerializeField]
		private float snapToMaxChargeAt = 9999999f;

		// Token: 0x04007E8E RID: 32398
		[SerializeField]
		private float chargeDecaySpeed = 9999999f;

		// Token: 0x04007E8F RID: 32399
		[SerializeField]
		private bool runChargeCancelledEventOnShoot;

		// Token: 0x04007E90 RID: 32400
		[SerializeField]
		private AnimationCurve chargeRateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007E91 RID: 32401
		[SerializeField]
		private AnimationCurve chargeToShotSpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007E92 RID: 32402
		[FormerlySerializedAs("onReadyToShoot")]
		public UnityEvent onCooldownFinished;

		// Token: 0x04007E93 RID: 32403
		public ContinuousPropertyArray continuousChargingProperties;

		// Token: 0x04007E94 RID: 32404
		public UnityEvent<float> whileCharging;

		// Token: 0x04007E95 RID: 32405
		public UnityEvent onMaxCharge;

		// Token: 0x04007E96 RID: 32406
		public UnityEvent onChargeCancelled;

		// Token: 0x04007E97 RID: 32407
		[FormerlySerializedAs("onLaunchProjectileShared")]
		public UnityEvent<float> onShoot;

		// Token: 0x04007E98 RID: 32408
		[FormerlySerializedAs("onOwnerLaunchProjectile")]
		public UnityEvent<float> onShootLocal;

		// Token: 0x04007E99 RID: 32409
		[SerializeField]
		private int numberOfProgressSteps;

		// Token: 0x04007E9A RID: 32410
		public UnityEvent<int> onMovedToNextStep;

		// Token: 0x04007E9B RID: 32411
		public UnityEvent<int> onReachedLastProgressStep;

		// Token: 0x04007E9C RID: 32412
		private int currentStep = -1;

		// Token: 0x04007E9D RID: 32413
		private int lastStep = -1;

		// Token: 0x04007E9F RID: 32415
		private bool isPressed;

		// Token: 0x04007EA0 RID: 32416
		private bool velocityEstimatorThresholdMet;

		// Token: 0x04007EA1 RID: 32417
		private float cooldownRemaining;

		// Token: 0x04007EA2 RID: 32418
		private float chargeTime;

		// Token: 0x04007EA3 RID: 32419
		private TransferrableObject transferrableObject;

		// Token: 0x04007EA4 RID: 32420
		private VRRig rig;

		// Token: 0x04007EA5 RID: 32421
		private bool isLocal;

		// Token: 0x04007EA6 RID: 32422
		private Transform debugShootDirection;

		// Token: 0x02001110 RID: 4368
		private enum ShootActivator
		{
			// Token: 0x04007EA9 RID: 32425
			ButtonReleased,
			// Token: 0x04007EAA RID: 32426
			ButtonPressed,
			// Token: 0x04007EAB RID: 32427
			ButtonStayed,
			// Token: 0x04007EAC RID: 32428
			VelocityEstimatorThreshold,
			// Token: 0x04007EAD RID: 32429
			ButtonReleasedFullCharge
		}

		// Token: 0x02001111 RID: 4369
		private enum ShootDirection
		{
			// Token: 0x04007EAF RID: 32431
			LaunchTransformRotation,
			// Token: 0x04007EB0 RID: 32432
			LineFromRigToLaunchTransform
		}
	}
}
