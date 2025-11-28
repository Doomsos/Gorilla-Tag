using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000CD RID: 205
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetDashYoyo : SIGadget
{
	// Token: 0x17000055 RID: 85
	// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0001CC9C File Offset: 0x0001AE9C
	private int _HandIndex
	{
		get
		{
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandL) || this.gameEntity.heldByHandIndex == 0)
			{
				return 0;
			}
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandR) || this.gameEntity.heldByHandIndex == 1)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x0001CD18 File Offset: 0x0001AF18
	private void Start()
	{
		this._stateMaterials = this.m_baseStateMats;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x0001CDD0 File Offset: 0x0001AFD0
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._ResetYoYo();
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x0001CEBC File Offset: 0x0001B0BC
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		if (state - SIGadgetDashYoyo.EState.Thrown <= 2)
		{
			this.m_tetherLineRenderer.SetPosition(1, this.m_tetherLineRenderer.transform.InverseTransformPoint(this.m_yoyoTarget.position));
		}
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001CF08 File Offset: 0x0001B108
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = base.GetAttachedPlayerActorNumber();
		this._attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this._attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = gamePlayer.rig;
		VRRig attachedVRRig2 = this._attachedVRRig;
		attachedVRRig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(attachedVRRig2.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		int num = this._isTagged ? 2 : 0;
		if (num != this._attachedVRRig.setMatIndex)
		{
			this._HandleVRRigMaterialIndexChanged(num, this._attachedVRRig.setMatIndex);
		}
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001CFE4 File Offset: 0x0001B1E4
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		if (this._attachedVRRig != null)
		{
			VRRig attachedVRRig = this._attachedVRRig;
			attachedVRRig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(attachedVRRig.OnMaterialIndexChanged, new Action<int, int>(this._HandleVRRigMaterialIndexChanged));
		}
		this._attachedVRRig = null;
		if (this._isTagged)
		{
			this._HandleVRRigMaterialIndexChanged(2, 0);
		}
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetDashYoyo.EState.DashUsed || this._state == SIGadgetDashYoyo.EState.OnCooldown)
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
		}
		else
		{
			this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
		}
		GTPlayer.Instance.ResetRigidbodyInterpolation();
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x0001D088 File Offset: 0x0001B288
	private void _HandleVRRigMaterialIndexChanged(int oldMatIndex, int newMatIndex)
	{
		if (this._attachedPlayerActorNr != -1 && (newMatIndex == 2 || newMatIndex == 1) && this._hasTagUpgrade)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				this._isTagged = (this._attachedNetPlayer != null && superInfectionGame.IsInfected(this._attachedNetPlayer));
				this._OnTagStateOrUpgradesChanged();
				return;
			}
		}
		this._isTagged = false;
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x0001D0F0 File Offset: 0x0001B2F0
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._successfulYankTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (this._isActivated)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToThrow);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.OnCooldown:
			if (Time.unscaledTime > this._successfulYankTime + this._cooldownDuration)
			{
				this._PlayHaptic(0.5f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			if (!this._isActivated)
			{
				if (this._ThrowYoYoTarget())
				{
					this._PlayHaptic(0.5f);
					GTPlayer.Instance.RigidbodyInterpolation = 0;
					this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
					return;
				}
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.Thrown:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				GTPlayer.Instance.ResetRigidbodyInterpolation();
				return;
			}
			if (GTPlayer.Instance.RigidbodyInterpolation != null)
			{
				GTPlayer.Instance.RigidbodyInterpolation = 0;
			}
			if (this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.PreparedToDash);
				return;
			}
			break;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			if (Time.unscaledTime > this._timeLastThrown + this.m_waitBeforeAutoReturn)
			{
				this._PlayHaptic(0.75f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Idle);
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetDashYoyo.EState.Thrown);
				return;
			}
			this._CheckYankProgression();
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			if (Time.unscaledTime > this._successfulYankTime + this.m_postYankCooldown)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetDashYoyo.EState.OnCooldown);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x0001D2A0 File Offset: 0x0001B4A0
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetDashYoyo.EState estate = (SIGadgetDashYoyo.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001D2D1 File Offset: 0x0001B4D1
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 6L;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001D2DF File Offset: 0x0001B4DF
	private void SetStateAuthority(SIGadgetDashYoyo.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x0001D300 File Offset: 0x0001B500
	private void _SetStateShared(SIGadgetDashYoyo.EState newState)
	{
		if (newState == this._state || !SIGadgetDashYoyo._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetDashYoyo.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			if (state == SIGadgetDashYoyo.EState.OnCooldown)
			{
				this._PlayAudio(4);
			}
			else if (state == SIGadgetDashYoyo.EState.PreparedToThrow)
			{
				this._PlayAudio(5);
			}
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._PlayAudio(3);
			this._ResetYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._PlayAudio(0);
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			if (state != SIGadgetDashYoyo.EState.PreparedToDash)
			{
				this._PlayAudio(1);
			}
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._yankBeginPos = this.m_yoyoDefaultPosXform.position;
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._PlayAudio(2);
			this._FreezeYoYo();
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0001D41C File Offset: 0x0001B61C
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(true, true, sensitivity, true, true);
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001D450 File Offset: 0x0001B650
	private bool _ThrowYoYoTarget()
	{
		Vector3 vector = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (vector.magnitude < this.m_minThrowSpeed)
		{
			return false;
		}
		Vector3 handAngularVelocity = GamePlayerLocal.instance.GetHandAngularVelocity(this._HandIndex);
		GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
		vector *= this._throwMultiplier;
		vector += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
		this._LaunchYoYoShared(vector, handAngularVelocity, this.m_yoyoTargetRB.transform.position, this.m_yoyoTargetRB.transform.rotation);
		this._timeLastThrown = Time.unscaledTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return true;
		}
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone == null)
		{
			return true;
		}
		this._launchYoyoRPCArgs[0] = this.gameEntity.GetNetId();
		this._launchYoyoRPCArgs[1] = vector;
		this._launchYoyoRPCArgs[2] = handAngularVelocity;
		this._launchYoyoRPCArgs[3] = this.m_yoyoTargetRB.transform.position;
		this._launchYoyoRPCArgs[4] = this.m_yoyoTargetRB.transform.rotation;
		simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.LaunchDashYoyo, this._launchYoyoRPCArgs);
		return true;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001D59E File Offset: 0x0001B79E
	internal void RemoteThrowYoYoTarget(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this._LaunchYoYoShared(velocity, angVelocity, targetPosition, targetRotation);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001D5AC File Offset: 0x0001B7AC
	private void _LaunchYoYoShared(Vector3 velocity, Vector3 angVelocity, Vector3 targetPosition, Quaternion targetRotation)
	{
		this.m_yoyoTargetRB.transform.parent = null;
		this.m_yoyoTargetRB.transform.position = targetPosition;
		this.m_yoyoTargetRB.transform.rotation = targetRotation;
		this.m_yoyoTargetRB.gameObject.SetActive(true);
		this.m_yoyoTarget.parent = this.m_yoyoTargetRB.transform;
		this.m_yoyoTargetRB.isKinematic = false;
		this.m_yoyoTargetRB.linearVelocity = velocity;
		this.m_yoyoTargetRB.angularVelocity = angVelocity;
		this.m_tetherLineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001D649 File Offset: 0x0001B849
	private void _FreezeYoYo()
	{
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.parent = null;
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001D668 File Offset: 0x0001B868
	internal void OnHitPlayer_Authority(SuperInfectionGame siTagGameManager, NetPlayer victimNetPlayer)
	{
		bool flag = siTagGameManager.IsInfected(this._attachedNetPlayer);
		bool flag2 = siTagGameManager.IsInfected(victimNetPlayer);
		if (flag == flag2)
		{
			return;
		}
		if (this._hasTagUpgrade && !flag2)
		{
			siTagGameManager.ReportTag(victimNetPlayer, this._attachedNetPlayer);
			return;
		}
		RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, victimNetPlayer);
		RoomSystem.SendSoundEffectOnOther(5, 0.125f, victimNetPlayer, false);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x0001D6BC File Offset: 0x0001B8BC
	private void _ResetYoYo()
	{
		this.m_tetherLineRenderer.gameObject.SetActive(false);
		this.m_yoyoTargetRB.gameObject.SetActive(false);
		this.m_yoyoTarget.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTarget.transform.localPosition = Vector3.zero;
		this.m_yoyoTarget.transform.localRotation = Quaternion.identity;
		this.m_yoyoTargetRB.transform.SetParent(this.m_yoyoDefaultPosXform, false);
		this.m_yoyoTargetRB.transform.localPosition = Vector3.zero;
		this.m_yoyoTargetRB.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001D768 File Offset: 0x0001B968
	private void _SetMaterials(Material mat)
	{
		this.m_yoyoRenderer.sharedMaterial = mat;
		this.m_tetherLineRenderer.sharedMaterial = mat;
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001D784 File Offset: 0x0001B984
	private void _CheckYankProgression()
	{
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		this._maxEncounteredYankSpeed = Mathf.Max(this._maxEncounteredYankSpeed, handVelocity.magnitude);
		Vector3 vector = this._yankBeginPos - this.m_yoyoDefaultPosXform.position;
		Vector3 normalized = (-handVelocity.normalized + vector.normalized).normalized;
		Vector3 vector2 = this.m_yoyoTarget.position - this.m_yoyoDefaultPosXform.position;
		if (vector.magnitude < this.m_yankMinDistance || this._maxEncounteredYankSpeed < this.m_yankMinSpeed || Vector3.Angle(vector2, normalized) > this.m_yankMaxAngle)
		{
			return;
		}
		this._successfulYankTime = Time.unscaledTime;
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(Vector3.RotateTowards(vector2.normalized, normalized, this._maxInfluenceAngle * 0.017453292f, 0f) * num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetDashYoyo.EState.DashUsed);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001D8A0 File Offset: 0x0001BAA0
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001D8E0 File Offset: 0x0001BAE0
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController, true, true))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001D91A File Offset: 0x0001BB1A
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001D950 File Offset: 0x0001BB50
	private void _OnTagStateOrUpgradesChanged()
	{
		this._stateMaterials = (this._hasTagUpgrade ? (this._isTagged ? this.m_tagUpgradeStateMatsWhileTagged : this.m_tagUpgradeStateMatsWhileUntagged) : this.m_baseStateMats);
		switch (this._state)
		{
		case SIGadgetDashYoyo.EState.Idle:
			this._SetMaterials(this._stateMaterials.idle);
			return;
		case SIGadgetDashYoyo.EState.OnCooldown:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		case SIGadgetDashYoyo.EState.PreparedToThrow:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.Thrown:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.PreparedToDash:
			this._SetMaterials(this._stateMaterials.ready);
			return;
		case SIGadgetDashYoyo.EState.DashUsed:
			this._SetMaterials(this._stateMaterials.cooldown);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001DA1C File Offset: 0x0001BC1C
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._cooldownDuration = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Cooldown) ? this.m_cooldownDurationUpgrade : this.m_cooldownDurationDefault);
		this._throwMultiplier = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Range) ? this.m_throwMultiplierUpgrade : this.m_throwMultiplierDefault);
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._maxInfluenceAngle = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Dynamic) ? this.m_maxInfluenceAngleUpgrade : this.m_maxInfluenceAngleDefault);
		this._hasStunUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Stun);
		this._hasTagUpgrade = withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Tag);
		this._OnTagStateOrUpgradesChanged();
	}

	// Token: 0x040005EF RID: 1519
	private const string preLog = "[SIGadgetDashYoyo]  ";

	// Token: 0x040005F0 RID: 1520
	private const string preErr = "[SIGadgetDashYoyo]  ERROR!!!  ";

	// Token: 0x040005F1 RID: 1521
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x040005F2 RID: 1522
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x040005F3 RID: 1523
	[SerializeField]
	private Transform m_yoyoTarget;

	// Token: 0x040005F4 RID: 1524
	[SerializeField]
	private Rigidbody m_yoyoTargetRB;

	// Token: 0x040005F5 RID: 1525
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040005F6 RID: 1526
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x040005F7 RID: 1527
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x040005F8 RID: 1528
	private SIGadgetDashYoyo.StateMaterialsInfo _stateMaterials;

	// Token: 0x040005F9 RID: 1529
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_baseStateMats;

	// Token: 0x040005FA RID: 1530
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileTagged;

	// Token: 0x040005FB RID: 1531
	[SerializeField]
	private SIGadgetDashYoyo.StateMaterialsInfo m_tagUpgradeStateMatsWhileUntagged;

	// Token: 0x040005FC RID: 1532
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x040005FD RID: 1533
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x040005FE RID: 1534
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x040005FF RID: 1535
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000600 RID: 1536
	private float _throwMultiplier;

	// Token: 0x04000601 RID: 1537
	[SerializeField]
	private float m_throwMultiplierDefault = 1.5f;

	// Token: 0x04000602 RID: 1538
	[SerializeField]
	private float m_throwMultiplierUpgrade = 2f;

	// Token: 0x04000603 RID: 1539
	[FormerlySerializedAs("m_tether")]
	[SerializeField]
	private LineRenderer m_tetherLineRenderer;

	// Token: 0x04000604 RID: 1540
	[SerializeField]
	private float m_minThrowSpeed = 2f;

	// Token: 0x04000605 RID: 1541
	[SerializeField]
	private float m_waitBeforeAutoReturn = 3f;

	// Token: 0x04000606 RID: 1542
	[SerializeField]
	private float m_postYankCooldown = 2f;

	// Token: 0x04000607 RID: 1543
	[SerializeField]
	private float m_maxYankRecheckTime = 0.2f;

	// Token: 0x04000608 RID: 1544
	[SerializeField]
	private float m_yankMinDistance = 0.5f;

	// Token: 0x04000609 RID: 1545
	[SerializeField]
	private float m_yankMaxAngle = 60f;

	// Token: 0x0400060A RID: 1546
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x0400060B RID: 1547
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x0400060C RID: 1548
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x0400060D RID: 1549
	private float _maxDashSpeed;

	// Token: 0x0400060E RID: 1550
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x0400060F RID: 1551
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x04000610 RID: 1552
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000611 RID: 1553
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x04000612 RID: 1554
	private float _maxInfluenceAngle;

	// Token: 0x04000613 RID: 1555
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x04000614 RID: 1556
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x04000615 RID: 1557
	private float _cooldownDuration;

	// Token: 0x04000616 RID: 1558
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x04000617 RID: 1559
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x04000618 RID: 1560
	private bool _hasStunUpgrade;

	// Token: 0x04000619 RID: 1561
	private bool _hasTagUpgrade;

	// Token: 0x0400061A RID: 1562
	private bool _isActivated;

	// Token: 0x0400061B RID: 1563
	private bool _wasActivated;

	// Token: 0x0400061C RID: 1564
	private float _timeLastThrown;

	// Token: 0x0400061D RID: 1565
	private float _successfulYankTime;

	// Token: 0x0400061E RID: 1566
	private float _maxEncounteredYankSpeed;

	// Token: 0x0400061F RID: 1567
	private Vector3 _yankBeginPos;

	// Token: 0x04000620 RID: 1568
	private bool _isRecheckingYank;

	// Token: 0x04000621 RID: 1569
	private VRRig _attachedVRRig;

	// Token: 0x04000622 RID: 1570
	private int _lastAttachedPlayerActorNr;

	// Token: 0x04000623 RID: 1571
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x04000624 RID: 1572
	private NetPlayer _attachedNetPlayer;

	// Token: 0x04000625 RID: 1573
	private bool _isTagged;

	// Token: 0x04000626 RID: 1574
	private readonly object[] _launchYoyoRPCArgs = new object[5];

	// Token: 0x04000627 RID: 1575
	private SIGadgetDashYoyo.EState _state;

	// Token: 0x020000CE RID: 206
	[Serializable]
	public struct StateMaterialsInfo
	{
		// Token: 0x04000628 RID: 1576
		public Material idle;

		// Token: 0x04000629 RID: 1577
		public Material ready;

		// Token: 0x0400062A RID: 1578
		public Material cooldown;
	}

	// Token: 0x020000CF RID: 207
	private enum EState
	{
		// Token: 0x0400062C RID: 1580
		Idle,
		// Token: 0x0400062D RID: 1581
		OnCooldown,
		// Token: 0x0400062E RID: 1582
		PreparedToThrow,
		// Token: 0x0400062F RID: 1583
		Thrown,
		// Token: 0x04000630 RID: 1584
		PreparedToDash,
		// Token: 0x04000631 RID: 1585
		DashUsed,
		// Token: 0x04000632 RID: 1586
		Count
	}
}
