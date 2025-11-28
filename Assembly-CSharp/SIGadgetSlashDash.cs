using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D1 RID: 209
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetSlashDash : SIGadget
{
	// Token: 0x17000056 RID: 86
	// (get) Token: 0x06000515 RID: 1301 RVA: 0x0001DCC8 File Offset: 0x0001BEC8
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

	// Token: 0x06000516 RID: 1302 RVA: 0x0001DD44 File Offset: 0x0001BF44
	private void Start()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		this._fxGObj = this.m_particleSystem.gameObject;
		this._fxXform = this.m_particleSystem.transform;
		this._fxMain = this.m_particleSystem.main;
		this._fxGObj.SetActive(false);
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001DE2C File Offset: 0x0001C02C
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
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001DEDD File Offset: 0x0001C0DD
	private void _HandleStartInteraction()
	{
		bool isQuitting = ApplicationQuittingState.IsQuitting;
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001DEE5 File Offset: 0x0001C0E5
	private void _HandleStopInteraction()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state != SIGadgetSlashDash_EState.DashUsed)
		{
			this._SetStateAuthority(SIGadgetSlashDash_EState.Idle);
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001DF08 File Offset: 0x0001C108
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._dashStartTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetSlashDash_EState.Idle:
			if (this._isActivated)
			{
				this._PlayHaptic(0.1f);
				this._SetStateAuthority(SIGadgetSlashDash_EState.TriggerPressHold);
			}
			break;
		case SIGadgetSlashDash_EState.TriggerPressHold:
			if (!this._isActivated)
			{
				this._DoDash();
			}
			break;
		case SIGadgetSlashDash_EState.DashUsed:
			if (GTPlayer.Instance.LastTouchedGroundAtNetworkTime > this._dashStartNetworkTime)
			{
				this._SetStateAuthority(SIGadgetSlashDash_EState.Idle);
			}
			break;
		}
		this._OnUpdateShared();
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x0001DFCC File Offset: 0x0001C1CC
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetSlashDash_EState newState = (SIGadgetSlashDash_EState)this.gameEntity.GetState();
		this._TrySetStateShared(newState);
		this._OnUpdateShared();
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001DFFC File Offset: 0x0001C1FC
	private void _OnUpdateShared()
	{
		switch (this._state)
		{
		case SIGadgetSlashDash_EState.Idle:
			this._fxGObj.SetActive(false);
			return;
		case SIGadgetSlashDash_EState.TriggerPressHold:
			this._fxGObj.SetActive(true);
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.gray3);
			this._UpdateFxRotation();
			return;
		case SIGadgetSlashDash_EState.DashUsed:
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.white);
			this._UpdateFxRotation();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001E074 File Offset: 0x0001C274
	private void _UpdateFxRotation()
	{
		Vector3 vector = this._fxXform.rotation.eulerAngles * 0.017453292f;
		this._fxMain.startRotationX = new ParticleSystem.MinMaxCurve(vector.x);
		this._fxMain.startRotationY = new ParticleSystem.MinMaxCurve(vector.y);
		this._fxMain.startRotationZ = new ParticleSystem.MinMaxCurve(vector.z);
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001E0E1 File Offset: 0x0001C2E1
	private void _SetStateAuthority(SIGadgetSlashDash_EState newState)
	{
		if (this._TrySetStateShared(newState))
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001E104 File Offset: 0x0001C304
	private bool _TrySetStateShared(SIGadgetSlashDash_EState newState)
	{
		long num = (long)newState;
		if (newState == this._state || num < 0L || num >= 3L)
		{
			return false;
		}
		this._state = newState;
		return true;
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0001E134 File Offset: 0x0001C334
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(true, true, sensitivity, true, true);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0001E168 File Offset: 0x0001C368
	private void _DoDash()
	{
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (handVelocity.magnitude < this.m_handMinSpeed)
		{
			return;
		}
		this._dashStartTime = Time.unscaledTime;
		this._dashStartNetworkTime = (float)PhotonNetwork.Time;
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		Vector3 normalized = handVelocity.normalized;
		instance.SetVelocity(normalized * -num);
		this._PlayHaptic(2f);
		this._SetStateAuthority(SIGadgetSlashDash_EState.DashUsed);
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001E1F0 File Offset: 0x0001C3F0
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_handMinSpeed, this.m_handMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x0001E230 File Offset: 0x0001C430
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController, true, true))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x0001E26C File Offset: 0x0001C46C
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Slash_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._coolDown = (withUpgrades.Contains(SIUpgradeType.Dash_Slash_Cooldown) ? this.m_coolDownUpgraded : this.m_coolDownDefault);
	}

	// Token: 0x04000634 RID: 1588
	private const string preLog = "[SIGadgetSlashDash]  ";

	// Token: 0x04000635 RID: 1589
	private const string preErr = "[SIGadgetSlashDash]  ERROR!!!  ";

	// Token: 0x04000636 RID: 1590
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x04000637 RID: 1591
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x04000638 RID: 1592
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x04000639 RID: 1593
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x0400063A RID: 1594
	[Tooltip("Hand min speed: How fast you have to be moving your hand for the dash to trigger.")]
	[SerializeField]
	private float m_handMinSpeed = 2f;

	// Token: 0x0400063B RID: 1595
	[Tooltip("Hand move max speed: The fastest hand speed that will be registered.")]
	[SerializeField]
	private float m_handMaxSpeed = 8f;

	// Token: 0x0400063C RID: 1596
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x0400063D RID: 1597
	private float _maxDashSpeed;

	// Token: 0x0400063E RID: 1598
	[SerializeField]
	private float m_maxDashSpeedDefault = 5f;

	// Token: 0x0400063F RID: 1599
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 7f;

	// Token: 0x04000640 RID: 1600
	private float _coolDown;

	// Token: 0x04000641 RID: 1601
	[SerializeField]
	private float m_coolDownDefault = 1f;

	// Token: 0x04000642 RID: 1602
	[SerializeField]
	private float m_coolDownUpgraded = 0.5f;

	// Token: 0x04000643 RID: 1603
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000644 RID: 1604
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x04000645 RID: 1605
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x04000646 RID: 1606
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x04000647 RID: 1607
	[SerializeField]
	private ParticleSystem m_particleSystem;

	// Token: 0x04000648 RID: 1608
	private GameObject _fxGObj;

	// Token: 0x04000649 RID: 1609
	private Transform _fxXform;

	// Token: 0x0400064A RID: 1610
	private ParticleSystem.MainModule _fxMain;

	// Token: 0x0400064B RID: 1611
	private bool _isActivated;

	// Token: 0x0400064C RID: 1612
	private bool _wasActivated;

	// Token: 0x0400064D RID: 1613
	private float _dashStartTime;

	// Token: 0x0400064E RID: 1614
	private float _dashStartNetworkTime;

	// Token: 0x0400064F RID: 1615
	private Vector3 _airReleaseVector;

	// Token: 0x04000650 RID: 1616
	private bool _isTagged;

	// Token: 0x04000651 RID: 1617
	private SIGadgetSlashDash_EState _state;
}
