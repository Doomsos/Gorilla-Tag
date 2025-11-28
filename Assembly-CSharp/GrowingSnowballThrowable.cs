using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class GrowingSnowballThrowable : SnowballThrowable
{
	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x0003AD02 File Offset: 0x00038F02
	public int SizeLevel
	{
		get
		{
			return this.sizeLevel;
		}
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x0003AD0A File Offset: 0x00038F0A
	public int MaxSizeLevel
	{
		get
		{
			return Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		}
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x0003AD20 File Offset: 0x00038F20
	public float CurrentSnowballRadius
	{
		get
		{
			if (this.snowballSizeLevels.Count > 0 && this.sizeLevel > -1 && this.sizeLevel < this.snowballSizeLevels.Count)
			{
				return this.snowballSizeLevels[this.sizeLevel].snowballScale * this.modelRadius * base.transform.lossyScale.x;
			}
			return this.modelRadius * base.transform.lossyScale.x;
		}
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x0003ADA0 File Offset: 0x00038FA0
	protected override void Awake()
	{
		base.Awake();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.StartedMultiplayerSession);
		}
		else
		{
			Debug.LogError("NetworkSystem.Instance was null in SnowballThrowable Awake");
		}
		VRRigCache.OnRigActivated += new Action<RigContainer>(this.VRRigActivated);
		VRRigCache.OnRigDeactivated += new Action<RigContainer>(this.VRRigDeactivated);
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003AE10 File Offset: 0x00039010
	public override void OnEnable()
	{
		base.OnEnable();
		this.snowballModelParentTransform.localPosition = this.modelParentOffset;
		this.snowballModelTransform.localPosition = this.modelOffset;
		this.otherHandSnowball = (this.isLeftHanded ? (EquipmentInteractor.instance.rightHandHeldEquipment as GrowingSnowballThrowable) : (EquipmentInteractor.instance.leftHandHeldEquipment as GrowingSnowballThrowable));
		if (Time.time > this.maintainSizeLevelUntilLocalTime)
		{
			this.SetSizeLevelLocal(0);
		}
		this.CreatePhotonEventsIfNull();
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003AE91 File Offset: 0x00039091
	protected override void OnDestroy()
	{
		this.DestroyPhotonEvents();
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003AE9C File Offset: 0x0003909C
	private void VRRigActivated(RigContainer rigContainer)
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (rigContainer.Rig == this.targetRig)
		{
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003AEF1 File Offset: 0x000390F1
	private void VRRigDeactivated(RigContainer rigContainer)
	{
		if (rigContainer.Rig == this.targetRig)
		{
			this.DestroyPhotonEvents();
		}
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003AF0C File Offset: 0x0003910C
	private void StartedMultiplayerSession()
	{
		this.targetRig = base.GetComponentInParent<VRRig>(true);
		this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		if (this.isOfflineRig)
		{
			this.DestroyPhotonEvents();
			this.CreatePhotonEventsIfNull();
		}
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003AF5C File Offset: 0x0003915C
	private void CreatePhotonEventsIfNull()
	{
		if (this.targetRig == null)
		{
			this.targetRig = base.GetComponentInParent<VRRig>(true);
			this.isOfflineRig = (this.targetRig != null && this.targetRig.isOfflineVRRig);
		}
		if (this.targetRig == null || this.targetRig.netView == null)
		{
			return;
		}
		if (this.changeSizeEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "ChangeSizeEventLeft" : "ChangeSizeEventRight", this.targetRig.netView.ViewID.ToString());
			this.changeSizeEvent = new PhotonEvent(eventId);
			this.changeSizeEvent.reliable = true;
			this.changeSizeEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
		}
		if (this.snowballThrowEvent == null)
		{
			"SnowballThrowable" + base.gameObject.name + (this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight") + this.targetRig.netView.ViewID.ToString();
			int eventId2 = StaticHash.Compute("SnowballThrowable", base.gameObject.name, this.isLeftHanded ? "SnowballThrowEventLeft" : "SnowballThrowEventRight", this.targetRig.netView.ViewID.ToString());
			this.snowballThrowEvent = new PhotonEvent(eventId2);
			this.snowballThrowEvent.reliable = true;
			this.snowballThrowEvent += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003B15C File Offset: 0x0003935C
	private void DestroyPhotonEvents()
	{
		if (this.changeSizeEvent != null)
		{
			this.changeSizeEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ChangeSizeEventReceiver);
			this.changeSizeEvent.Dispose();
			this.changeSizeEvent = null;
		}
		if (this.snowballThrowEvent != null)
		{
			this.snowballThrowEvent -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SnowballThrowEventReceiver);
			this.snowballThrowEvent.Dispose();
			this.snowballThrowEvent = null;
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003B1E3 File Offset: 0x000393E3
	public void IncreaseSize(int increase)
	{
		this.SetSizeLevelAuthority(this.sizeLevel + increase);
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003B1F4 File Offset: 0x000393F4
	private void SetSizeLevelAuthority(int sizeLevel)
	{
		if (this.targetRig != null && this.targetRig.creator != null && this.targetRig.creator.IsLocal)
		{
			int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
			if (validSizeLevel > this.sizeLevel)
			{
				this.sizeIncreaseSoundBankPlayer.Play();
			}
			this.SetSizeLevelLocal(validSizeLevel);
			PhotonEvent photonEvent = this.changeSizeEvent;
			if (photonEvent == null)
			{
				return;
			}
			photonEvent.RaiseOthers(new object[]
			{
				validSizeLevel
			});
		}
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003B270 File Offset: 0x00039470
	private int GetValidSizeLevel(int inputSizeLevel)
	{
		int num = Mathf.Max(this.snowballSizeLevels.Count - 1, 0);
		return Mathf.Clamp(inputSizeLevel, 0, num);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003B29C File Offset: 0x0003949C
	private void SetSizeLevelLocal(int sizeLevel)
	{
		int validSizeLevel = this.GetValidSizeLevel(sizeLevel);
		if (validSizeLevel >= 0 && validSizeLevel != this.sizeLevel)
		{
			this.sizeLevel = validSizeLevel;
			this.snowballModelParentTransform.localScale = Vector3.one * this.snowballSizeLevels[this.sizeLevel].snowballScale;
		}
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0003B2F0 File Offset: 0x000394F0
	private void ChangeSizeEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 1)
		{
			return;
		}
		int num = (this.targetRig != null && this.targetRig.gameObject.activeInHierarchy && this.targetRig.netView != null && this.targetRig.netView.Owner != null) ? this.targetRig.netView.Owner.ActorNumber : -1;
		if (info.senderID != num)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "ChangeSizeEventReceiver");
		int num2 = (int)args[0];
		if (this.GetValidSizeLevel(num2) > this.sizeLevel && this.sizeIncreaseSoundBankPlayer.gameObject.activeInHierarchy)
		{
			this.sizeIncreaseSoundBankPlayer.Play();
		}
		this.SetSizeLevelLocal(num2);
		if (!base.gameObject.activeSelf)
		{
			this.maintainSizeLevelUntilLocalTime = Time.time + 0.1f;
		}
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003B3DC File Offset: 0x000395DC
	private void SnowballThrowEventReceiver(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (sender != receiver)
		{
			return;
		}
		if (args == null || args.Length < 3)
		{
			return;
		}
		if (this.targetRig.IsNull() || !this.targetRig.gameObject.activeSelf)
		{
			return;
		}
		NetPlayer creator = this.targetRig.creator;
		if (info.senderID != this.targetRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SnowballThrowEventReceiver");
		if (!this.snowballThrowCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		object obj = args[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = args[1];
			if (obj is Vector3)
			{
				Vector3 inVel = (Vector3)obj;
				obj = args[2];
				if (obj is int)
				{
					int index = (int)obj;
					Vector3 velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f, 100f);
					float x = this.snowballModelTransform.lossyScale.x;
					float num = 10000f;
					if (!vector.IsValid(num) || !this.targetRig.IsPositionInRange(vector, 4f))
					{
						return;
					}
					this.LaunchSnowballRemote(vector, velocity, x, index, info);
					return;
				}
			}
		}
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0003B500 File Offset: 0x00039700
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (GrowingSnowballThrowable.twoHandedSnowballGrowing)
		{
			if (this.otherHandSnowball != null && this.otherHandSnowball.isActiveAndEnabled)
			{
				IHoldableObject holdableObject = this.isLeftHanded ? EquipmentInteractor.instance.rightHandHeldEquipment : EquipmentInteractor.instance.leftHandHeldEquipment;
				if (holdableObject != null && this.otherHandSnowball != (GrowingSnowballThrowable)holdableObject)
				{
					this.otherHandSnowball = null;
					return;
				}
				float num = this.otherHandSnowball.CurrentSnowballRadius + this.CurrentSnowballRadius;
				if (this.SizeLevel < this.MaxSizeLevel && this.otherHandSnowball.SizeLevel < this.otherHandSnowball.MaxSizeLevel && (this.otherHandSnowball.snowballModelTransform.position - this.snowballModelTransform.position).sqrMagnitude < num * num)
				{
					int num2 = this.SizeLevel - this.otherHandSnowball.SizeLevel;
					float magnitude = this.velocityEstimator.linearVelocity.magnitude;
					float magnitude2 = this.otherHandSnowball.velocityEstimator.linearVelocity.magnitude;
					bool flag;
					if (Mathf.Abs(magnitude - magnitude2) > this.combineBasedOnSpeedThreshold || num2 == 0)
					{
						flag = (magnitude > magnitude2);
					}
					else
					{
						flag = (num2 < 0);
					}
					if (flag)
					{
						this.otherHandSnowball.IncreaseSize(this.sizeLevel + 1);
						GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
						base.SetSnowballActiveLocal(false);
						return;
					}
					this.IncreaseSize(this.otherHandSnowball.SizeLevel + 1);
					GorillaTagger.Instance.StartVibration(this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.otherHandSnowball.SetSnowballActiveLocal(false);
					return;
				}
			}
			else
			{
				this.otherHandSnowball = null;
			}
		}
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003B706 File Offset: 0x00039906
	protected override void OnSnowballRelease()
	{
		if (base.isActiveAndEnabled)
		{
			this.PerformSnowballThrowAuthority();
		}
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003B718 File Offset: 0x00039918
	protected override void PerformSnowballThrowAuthority()
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Rigidbody component = GorillaTagger.Instance.GetComponent<Rigidbody>();
		if (component != null)
		{
			vector = component.linearVelocity;
		}
		Vector3 vector2 = this.velocityEstimator.linearVelocity - vector;
		float magnitude = vector2.magnitude;
		if (magnitude > 0.001f)
		{
			float num = Mathf.Clamp(magnitude * this.linSpeedMultiplier, 0f, this.maxLinSpeed);
			vector2 *= num / magnitude;
		}
		Vector3 vector3 = vector2 + vector;
		this.targetRig.GetThrowableProjectileColor(this.isLeftHanded);
		Transform transform = this.snowballModelTransform;
		Vector3 position = transform.position;
		float x = transform.lossyScale.x;
		SlingshotProjectile slingshotProjectile = this.LaunchSnowballLocal(position, vector3, x);
		base.SetSnowballActiveLocal(false);
		if (this.randModelIndex > -1 && this.randModelIndex < this.localModels.Count && this.localModels[this.randModelIndex].destroyAfterRelease)
		{
			slingshotProjectile.DestroyAfterRelease();
		}
		PhotonEvent photonEvent = this.snowballThrowEvent;
		if (photonEvent == null)
		{
			return;
		}
		photonEvent.RaiseOthers(new object[]
		{
			position,
			vector3,
			slingshotProjectile.myProjectileCount
		});
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0003B87C File Offset: 0x00039A7C
	protected virtual SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale)
	{
		return this.LaunchSnowballLocal(location, velocity, scale, false, Color.white);
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0003B890 File Offset: 0x00039A90
	protected override SlingshotProjectile LaunchSnowballLocal(Vector3 location, Vector3 velocity, float scale, bool randomizeColour, Color colour)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		int projectileCount = ProjectileTracker.AddAndIncrementLocalProjectile(slingshotProjectile, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileCount, scale, randomizeColour, colour);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0003B907 File Offset: 0x00039B07
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, PhotonMessageInfoWrapped info)
	{
		return this.LaunchSnowballRemote(location, velocity, scale, index, false, Color.white, info);
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003B91C File Offset: 0x00039B1C
	protected virtual SlingshotProjectile LaunchSnowballRemote(Vector3 location, Vector3 velocity, float scale, int index, bool randomizeColour, Color colour, PhotonMessageInfoWrapped info)
	{
		SlingshotProjectile slingshotProjectile = this.SpawnGrowingSnowball(ref velocity, scale);
		ProjectileTracker.AddRemotePlayerProjectile(info.Sender, slingshotProjectile, index, info.SentServerTime, velocity, location, scale);
		slingshotProjectile.Launch(location, velocity, info.Sender, false, false, index, scale, randomizeColour, Color.white);
		if (string.IsNullOrEmpty(this.throwEventName))
		{
			PlayerGameEvents.LaunchedProjectile(this.projectilePrefab.name);
		}
		else
		{
			PlayerGameEvents.LaunchedProjectile(this.throwEventName);
		}
		slingshotProjectile.OnImpact += this.OnProjectileImpact;
		return slingshotProjectile;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003B9A8 File Offset: 0x00039BA8
	private SlingshotProjectile SpawnGrowingSnowball(ref Vector3 velocity, float scale)
	{
		SlingshotProjectile component = ObjectPools.instance.Instantiate(this.randomModelSelection ? this.localModels[this.randModelIndex].projectilePrefab : this.projectilePrefab, true).GetComponent<SlingshotProjectile>();
		if (this.snowballSizeLevels.Count > 0 && this.sizeLevel >= 0 && this.sizeLevel < this.snowballSizeLevels.Count)
		{
			float num = scale / this.snowballSizeLevels[this.sizeLevel].snowballScale;
			SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig = this.snowballSizeLevels[this.sizeLevel].aoeKnockbackConfig;
			aoeKnockbackConfig.aeoInnerRadius *= num;
			aoeKnockbackConfig.aeoOuterRadius *= num;
			aoeKnockbackConfig.knockbackVelocity *= num;
			aoeKnockbackConfig.impactVelocityThreshold *= num;
			velocity *= this.snowballSizeLevels[this.sizeLevel].throwSpeedMultiplier;
			component.gravityMultiplier = this.snowballSizeLevels[this.sizeLevel].gravityMultiplier;
			component.impactEffectScaleMultiplier = this.snowballSizeLevels[this.sizeLevel].impactEffectScale;
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(aoeKnockbackConfig);
			component.impactSoundVolumeOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundVolume);
			component.impactSoundPitchOverride = new float?(this.snowballSizeLevels[this.sizeLevel].impactSoundPitch);
		}
		return component;
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0003BB30 File Offset: 0x00039D30
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.targetRig != null) || this.targetRig.creator == null || !this.targetRig.creator.IsLocal)
		{
			return;
		}
		SnowballThrowable snowballThrowable;
		if (((this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment == null) || (!this.isLeftHanded && grabbingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)) && (this.isLeftHanded ? SnowballMaker.rightHandInstance : SnowballMaker.leftHandInstance).TryCreateSnowball(this.matDataIndexes[0], out snowballThrowable))
		{
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable as GrowingSnowballThrowable;
			if (growingSnowballThrowable != null)
			{
				growingSnowballThrowable.IncreaseSize(this.sizeLevel);
				GorillaTagger.Instance.StartVibration(!this.isLeftHanded, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				base.SetSnowballActiveLocal(false);
			}
		}
	}

	// Token: 0x04000D4F RID: 3407
	public Transform snowballModelParentTransform;

	// Token: 0x04000D50 RID: 3408
	public Transform snowballModelTransform;

	// Token: 0x04000D51 RID: 3409
	public Vector3 modelParentOffset = Vector3.zero;

	// Token: 0x04000D52 RID: 3410
	public Vector3 modelOffset = Vector3.zero;

	// Token: 0x04000D53 RID: 3411
	public float modelRadius = 0.055f;

	// Token: 0x04000D54 RID: 3412
	[Tooltip("Snowballs will combine into the larger snowball unless they are moving faster than this threshold.Then the faster moving snowball will go in to the more stationary hand")]
	public float combineBasedOnSpeedThreshold = 0.5f;

	// Token: 0x04000D55 RID: 3413
	public SoundBankPlayer sizeIncreaseSoundBankPlayer;

	// Token: 0x04000D56 RID: 3414
	public List<GrowingSnowballThrowable.SizeParameters> snowballSizeLevels = new List<GrowingSnowballThrowable.SizeParameters>();

	// Token: 0x04000D57 RID: 3415
	private int sizeLevel;

	// Token: 0x04000D58 RID: 3416
	private float maintainSizeLevelUntilLocalTime;

	// Token: 0x04000D59 RID: 3417
	private PhotonEvent changeSizeEvent;

	// Token: 0x04000D5A RID: 3418
	private PhotonEvent snowballThrowEvent;

	// Token: 0x04000D5B RID: 3419
	private CallLimiterWithCooldown snowballThrowCallLimit = new CallLimiterWithCooldown(10f, 10, 2f);

	// Token: 0x04000D5C RID: 3420
	[HideInInspector]
	public static bool debugDrawAOERange = false;

	// Token: 0x04000D5D RID: 3421
	[HideInInspector]
	public static bool twoHandedSnowballGrowing = true;

	// Token: 0x04000D5E RID: 3422
	private Queue<GrowingSnowballThrowable.AOERangeDebugDraw> aoeRangeDebugDrawQueue = new Queue<GrowingSnowballThrowable.AOERangeDebugDraw>();

	// Token: 0x04000D5F RID: 3423
	private GrowingSnowballThrowable otherHandSnowball;

	// Token: 0x04000D60 RID: 3424
	private float debugDrawAOERangeTime = 1.5f;

	// Token: 0x02000198 RID: 408
	[Serializable]
	public struct SizeParameters
	{
		// Token: 0x04000D61 RID: 3425
		public float snowballScale;

		// Token: 0x04000D62 RID: 3426
		public float impactEffectScale;

		// Token: 0x04000D63 RID: 3427
		public float impactSoundVolume;

		// Token: 0x04000D64 RID: 3428
		public float impactSoundPitch;

		// Token: 0x04000D65 RID: 3429
		public float throwSpeedMultiplier;

		// Token: 0x04000D66 RID: 3430
		public float gravityMultiplier;

		// Token: 0x04000D67 RID: 3431
		public SlingshotProjectile.AOEKnockbackConfig aoeKnockbackConfig;
	}

	// Token: 0x02000199 RID: 409
	private struct AOERangeDebugDraw
	{
		// Token: 0x04000D68 RID: 3432
		public float impactTime;

		// Token: 0x04000D69 RID: 3433
		public Vector3 position;

		// Token: 0x04000D6A RID: 3434
		public float innerRadius;

		// Token: 0x04000D6B RID: 3435
		public float outerRadius;
	}
}
