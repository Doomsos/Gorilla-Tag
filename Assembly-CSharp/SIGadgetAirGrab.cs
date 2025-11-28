using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000D5 RID: 213
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetAirGrab : SIGadget
{
	// Token: 0x17000057 RID: 87
	// (get) Token: 0x0600052F RID: 1327 RVA: 0x0001E570 File Offset: 0x0001C770
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

	// Token: 0x06000530 RID: 1328 RVA: 0x0001E5EC File Offset: 0x0001C7EC
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
		foreach (AudioClip audioClip in this.m_clips)
		{
			if (audioClip)
			{
				audioClip.LoadAudioData();
			}
		}
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001E6C0 File Offset: 0x0001C8C0
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

	// Token: 0x06000532 RID: 1330 RVA: 0x0001E774 File Offset: 0x0001C974
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
		this._attachedVRRig = gamePlayer.rig;
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001E7C7 File Offset: 0x0001C9C7
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		this._attachedVRRig = null;
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetAirGrab.EState.DashUsed)
		{
			this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
			return;
		}
		this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001E804 File Offset: 0x0001CA04
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._airGrabTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
			if (this._isActivated)
			{
				this._PlayHaptic(0.1f);
				this.SetStateAuthority(SIGadgetAirGrab.EState.StartAirGrabbing);
				return;
			}
			break;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (this._isActivated)
			{
				this._airReleaseSpeed = 0f;
				if (this.m_airGrabXform != null)
				{
					this.m_airGrabXform.SetParent(null, false);
					this.m_airGrabXform.position = ((this._HandIndex == 0) ? this._attachedVRRig.leftHand.overrideTarget.position : this._attachedVRRig.rightHand.overrideTarget.position);
					this.m_airGrabXform.gameObject.SetActive(true);
				}
				this.SetStateAuthority(SIGadgetAirGrab.EState.PreparedToDash);
				return;
			}
			if (this.m_airGrabXform != null)
			{
				this.m_airGrabXform.transform.parent = base.transform;
				this.m_airGrabXform.gameObject.SetActive(false);
				return;
			}
			break;
		case SIGadgetAirGrab.EState.PreparedToDash:
			if (!this._isActivated)
			{
				this._DoDash();
				return;
			}
			this._DoAirGrab();
			return;
		case SIGadgetAirGrab.EState.DashUsed:
			this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001E974 File Offset: 0x0001CB74
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetAirGrab.EState estate = (SIGadgetAirGrab.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001B48E File Offset: 0x0001968E
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001E9A5 File Offset: 0x0001CBA5
	private void SetStateAuthority(SIGadgetAirGrab.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001E9C8 File Offset: 0x0001CBC8
	private void _SetStateShared(SIGadgetAirGrab.EState newState)
	{
		if (newState == this._state || !SIGadgetAirGrab._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetAirGrab.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
		case SIGadgetAirGrab.EState.PreparedToDash:
			break;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (state != SIGadgetAirGrab.EState.PreparedToDash)
			{
				this._PlayAudio(1);
				return;
			}
			break;
		case SIGadgetAirGrab.EState.DashUsed:
			this._PlayAudio(2);
			break;
		default:
			return;
		}
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0001EA28 File Offset: 0x0001CC28
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(true, true, sensitivity, true, true);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001EA5C File Offset: 0x0001CC5C
	private void _DoAirGrab()
	{
		GTPlayer instance = GTPlayer.Instance;
		Transform transform = (this._HandIndex == 0) ? instance.LeftHand.controllerTransform : instance.RightHand.controllerTransform;
		Vector3 vector = this.m_airGrabXform.position - transform.position;
		instance.RigidbodyMovePosition(instance.transform.position + vector);
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001EADC File Offset: 0x0001CCDC
	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(handVelocity.normalized * -num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001EB48 File Offset: 0x0001CD48
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001EB88 File Offset: 0x0001CD88
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController, true, true))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001EBC2 File Offset: 0x0001CDC2
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001EBF5 File Offset: 0x0001CDF5
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	// Token: 0x04000665 RID: 1637
	private const string preLog = "[SIGadgetAirGrab]  ";

	// Token: 0x04000666 RID: 1638
	private const string preErr = "[SIGadgetAirGrab]  ERROR!!!  ";

	// Token: 0x04000667 RID: 1639
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x04000668 RID: 1640
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400066A RID: 1642
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x0400066C RID: 1644
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x0400066D RID: 1645
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x0400066E RID: 1646
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x0400066F RID: 1647
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000670 RID: 1648
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x04000671 RID: 1649
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x04000672 RID: 1650
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x04000673 RID: 1651
	private float _maxDashSpeed;

	// Token: 0x04000674 RID: 1652
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x04000675 RID: 1653
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x04000676 RID: 1654
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000677 RID: 1655
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x04000678 RID: 1656
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x04000679 RID: 1657
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x0400067A RID: 1658
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x0400067B RID: 1659
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x0400067C RID: 1660
	[SerializeField]
	private Transform m_airGrabXform;

	// Token: 0x0400067D RID: 1661
	private bool _isActivated;

	// Token: 0x0400067E RID: 1662
	private bool _wasActivated;

	// Token: 0x0400067F RID: 1663
	private float _airGrabTime;

	// Token: 0x04000680 RID: 1664
	private float _airReleaseSpeed;

	// Token: 0x04000681 RID: 1665
	private Vector3 _airReleaseVector;

	// Token: 0x04000682 RID: 1666
	private VRRig _attachedVRRig;

	// Token: 0x04000683 RID: 1667
	private int _lastAttachedPlayerActorNr;

	// Token: 0x04000684 RID: 1668
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x04000685 RID: 1669
	private NetPlayer _attachedNetPlayer;

	// Token: 0x04000686 RID: 1670
	private bool _isTagged;

	// Token: 0x04000687 RID: 1671
	private readonly object[] _launchYoyoRPCArgs = new object[5];

	// Token: 0x04000688 RID: 1672
	private SIGadgetAirGrab.EState _state;

	// Token: 0x020000D6 RID: 214
	private enum EState
	{
		// Token: 0x0400068A RID: 1674
		Idle,
		// Token: 0x0400068B RID: 1675
		StartAirGrabbing,
		// Token: 0x0400068C RID: 1676
		PreparedToDash,
		// Token: 0x0400068D RID: 1677
		DashUsed,
		// Token: 0x0400068E RID: 1678
		Count
	}
}
