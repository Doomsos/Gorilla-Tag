using System;
using Drawing;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000D8 RID: 216
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetSlipMitt : SIGadget
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600054B RID: 1355 RVA: 0x0001F11C File Offset: 0x0001D31C
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

	// Token: 0x0600054C RID: 1356 RVA: 0x0001F198 File Offset: 0x0001D398
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

	// Token: 0x0600054D RID: 1357 RVA: 0x0001F26C File Offset: 0x0001D46C
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

	// Token: 0x0600054E RID: 1358 RVA: 0x0001F320 File Offset: 0x0001D520
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = base.GetAttachedPlayerActorNumber();
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		this._attachedVRRig = gamePlayer.rig;
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001F35D File Offset: 0x0001D55D
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedVRRig = null;
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001F384 File Offset: 0x0001D584
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
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
			if (state != SIGadgetSlipMitt.EState.Slip)
			{
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
				GTPlayer.Instance.UnsetGravityOverride(this);
				return;
			}
			this._airReleaseSpeed = 0f;
			if (this._HandIndex == 0)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
				this._attachedHandState = GTPlayer.Instance.LeftHand;
				return;
			}
			GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			this._attachedHandState = GTPlayer.Instance.RightHand;
		}
		else if (this._isActivated)
		{
			this._PlayHaptic(0.1f);
			GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this._HandleGTPlayerOnUpdateGravity));
			this.SetStateAuthority(SIGadgetSlipMitt.EState.Slip);
			return;
		}
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x0001F488 File Offset: 0x0001D688
	private void _HandleGTPlayerOnUpdateGravity(GTPlayer gtPlayer)
	{
		Transform handFollower = this._attachedHandState.handFollower;
		Ray ray;
		ray..ctor(handFollower.position, handFollower.forward);
		int value = gtPlayer.locomotionEnabledLayers.value;
		float num = 1f;
		float num2 = 20f;
		int num3 = Physics.RaycastNonAlloc(ray, this._raycastHitResults, num, value, 1);
		RaycastHit[] raycastHitResults = this._raycastHitResults;
		Vector3 gravity = Physics.gravity;
		Vector3 vector = ray.direction * num2;
		Vector3 vector2 = (num3 > 0) ? vector : gravity;
		Draw.ingame.Arrow(ray.origin, ray.origin + ray.direction);
		gtPlayer.AddForce(vector2 * gtPlayer.scale, 5);
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001F550 File Offset: 0x0001D750
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetSlipMitt.EState estate = (SIGadgetSlipMitt.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001F581 File Offset: 0x0001D781
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001F58F File Offset: 0x0001D78F
	private void SetStateAuthority(SIGadgetSlipMitt.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001F5B0 File Offset: 0x0001D7B0
	private void _SetStateShared(SIGadgetSlipMitt.EState newState)
	{
		if (newState == this._state || !SIGadgetSlipMitt._CanChangeState((long)newState))
		{
			return;
		}
		this._state = newState;
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001F5E8 File Offset: 0x0001D7E8
	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(true, true, sensitivity, true, true);
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001F61C File Offset: 0x0001D81C
	private void _DoAirGrab()
	{
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001F644 File Offset: 0x0001D844
	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(handVelocity.normalized * -num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetSlipMitt.EState.DashUsed);
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001F6B0 File Offset: 0x0001D8B0
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001F6F0 File Offset: 0x0001D8F0
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController, true, true))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001F72A File Offset: 0x0001D92A
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001F75D File Offset: 0x0001D95D
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	// Token: 0x0400069C RID: 1692
	private const string preLog = "[SIGadgetSlipMitt]  ";

	// Token: 0x0400069D RID: 1693
	private const string preErr = "[SIGadgetSlipMitt]  ERROR!!!  ";

	// Token: 0x0400069E RID: 1694
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x0400069F RID: 1695
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x040006A0 RID: 1696
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040006A1 RID: 1697
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x040006A2 RID: 1698
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x040006A3 RID: 1699
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x040006A4 RID: 1700
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x040006A5 RID: 1701
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x040006A6 RID: 1702
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x040006A7 RID: 1703
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x040006A8 RID: 1704
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x040006A9 RID: 1705
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x040006AA RID: 1706
	private float _maxDashSpeed;

	// Token: 0x040006AB RID: 1707
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x040006AC RID: 1708
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x040006AD RID: 1709
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040006AE RID: 1710
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x040006AF RID: 1711
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x040006B0 RID: 1712
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x040006B1 RID: 1713
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x040006B2 RID: 1714
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x040006B3 RID: 1715
	[SerializeField]
	private Transform m_airGrabXform;

	// Token: 0x040006B4 RID: 1716
	private bool _isActivated;

	// Token: 0x040006B5 RID: 1717
	private bool _wasActivated;

	// Token: 0x040006B6 RID: 1718
	private float _airGrabTime;

	// Token: 0x040006B7 RID: 1719
	private float _airReleaseSpeed;

	// Token: 0x040006B8 RID: 1720
	private Vector3 _airReleaseVector;

	// Token: 0x040006B9 RID: 1721
	private VRRig _attachedVRRig;

	// Token: 0x040006BA RID: 1722
	private GTPlayer.HandState _attachedHandState;

	// Token: 0x040006BB RID: 1723
	private int _lastAttachedPlayerActorNr;

	// Token: 0x040006BC RID: 1724
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x040006BD RID: 1725
	private bool _isTagged;

	// Token: 0x040006BE RID: 1726
	private SIGadgetSlipMitt.EState _state;

	// Token: 0x040006BF RID: 1727
	private RaycastHit[] _raycastHitResults = new RaycastHit[1];

	// Token: 0x020000D9 RID: 217
	private enum EState
	{
		// Token: 0x040006C1 RID: 1729
		Idle,
		// Token: 0x040006C2 RID: 1730
		Slip,
		// Token: 0x040006C3 RID: 1731
		DashUsed,
		// Token: 0x040006C4 RID: 1732
		Count
	}
}
