using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000801 RID: 2049
public class HandLink : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x060035DB RID: 13787 RVA: 0x001240EB File Offset: 0x001222EB
	// (set) Token: 0x060035DC RID: 13788 RVA: 0x001240F3 File Offset: 0x001222F3
	public bool IsLocal { get; private set; }

	// Token: 0x060035DD RID: 13789 RVA: 0x001240FC File Offset: 0x001222FC
	private void Start()
	{
		this.myOtherHandLink = (this.isLeftHand ? this.myRig.rightHandLink : this.myRig.leftHandLink);
		if (this.myRig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			this.IsLocal = true;
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x060035DE RID: 13790 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060035DF RID: 13791 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x0012416C File Offset: 0x0012236C
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (this.canBeGrabbed && (this.myRig.transform.position - VRRig.LocalRig.transform.position).sqrMagnitude < 9f);
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x001241C4 File Offset: 0x001223C4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.CanBeGrabbed())
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			(this.isLeftHand ? this.myRig.leftHolds : this.myRig.rightHolds).OnGrab(pointGrabbed, grabbingHand);
			return;
		}
		HandLink handLink = (grabbingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (handLink.canBeGrabbed && Time.time - handLink.gripPressedAtTimestamp < 0.1f)
		{
			handLink.CreateLink(this);
		}
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x00124270 File Offset: 0x00122470
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.myRig.isOfflineVRRig)
		{
			HandLink handLink = (releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			bool flag = false;
			HandLinkAuthorityStatus selfHandLinkAuthority = GTPlayer.Instance.GetSelfHandLinkAuthority();
			int num;
			HandLinkAuthorityStatus chainAuthority = handLink.GetChainAuthority(out num);
			if (selfHandLinkAuthority.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < selfHandLinkAuthority.type)
			{
				flag = true;
			}
			else if (handLink.myOtherHandLink.grabbedLink != null)
			{
				int num2;
				HandLinkAuthorityStatus chainAuthority2 = handLink.myOtherHandLink.GetChainAuthority(out num2);
				if (chainAuthority2.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < chainAuthority2.type)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(handLink.isLeftHand).GetAverageVelocity(true, 0.15f, false);
				this.myRig.netView.SendRPC("DroppedByPlayer", this.myRig.OwningNetPlayer, new object[]
				{
					averageVelocity
				});
				this.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
			}
			handLink.BreakLink();
		}
		return true;
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x00124397 File Offset: 0x00122597
	public override void DropItemCleanup()
	{
		if (this.grabbedLink != null)
		{
			this.grabbedLink.BreakLink();
		}
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x001243B2 File Offset: 0x001225B2
	public bool CanBeGrabbed()
	{
		return (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()) && Time.time >= this.rejectGrabsUntilTimestamp && this.canBeGrabbed && this.grabbedPlayer == null;
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x001243ED File Offset: 0x001225ED
	public bool IsLinkActive()
	{
		return this.grabbedLink != null;
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x001243FC File Offset: 0x001225FC
	private void CreateLink(HandLink remoteLink)
	{
		if (this.grabbedPlayer != null || !this.myRig.isLocal)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(remoteLink, this.isLeftHand);
		this.grabbedLink = remoteLink;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
		(this.isLeftHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer).GTPlayOneShot(this.audioOnGrab, 1f);
		Action onHandLinkChanged = HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged.Invoke();
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x001244F2 File Offset: 0x001226F2
	public void BreakLinkTo(HandLink targetLink)
	{
		if (this.grabbedLink == targetLink)
		{
			this.BreakLink();
		}
	}

	// Token: 0x060035E9 RID: 13801 RVA: 0x00124508 File Offset: 0x00122708
	public void BreakLink()
	{
		if (this.grabbedPlayer == null || this.grabbedLink == null)
		{
			return;
		}
		Vector3 velocity = this.myRig.LatestVelocity();
		GTPlayer.Instance.SetVelocity(velocity);
		this.grabbedLink = null;
		this.grabbedPlayer = null;
		this.grabbedHandIsLeft = false;
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isLeftHand);
		Action onHandLinkChanged = HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged.Invoke();
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x0012457C File Offset: 0x0012277C
	public static bool IsHandInChainWithOtherPlayer(HandLink startingLink, int targetPlayer)
	{
		HandLink handLink = startingLink;
		int num = 0;
		int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		while (handLink != null && num < roomPlayerCount)
		{
			if (handLink.myRig == null || handLink.myRig.creator == null)
			{
				return false;
			}
			if (handLink.myRig.creator.ActorNumber == targetPlayer)
			{
				return true;
			}
			HandLink handLink2 = null;
			RigContainer rigContainer;
			if (handLink.grabbedLink != null && handLink.grabbedLink.myOtherHandLink != null)
			{
				handLink2 = handLink.grabbedLink.myOtherHandLink;
			}
			else if (handLink.grabbedPlayer != null && VRRigCache.Instance.TryGetVrrig(handLink.grabbedPlayer, out rigContainer))
			{
				HandLink handLink3 = handLink.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (handLink3 != null && handLink3.myOtherHandLink != null)
				{
					handLink2 = handLink3.myOtherHandLink;
				}
			}
			handLink = handLink2;
			num++;
		}
		return false;
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x00124678 File Offset: 0x00122878
	public void LocalUpdate(bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool canBeGrabbed)
	{
		if (isGripPressed && !this.wasGripPressed)
		{
			this.gripPressedAtTimestamp = Time.time;
		}
		this.wasGripPressed = isGripPressed;
		this.canBeGrabbed = canBeGrabbed;
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		if (this.grabbedLink != null)
		{
			if (!this.grabbedLink.canBeGrabbed && this.grabbedLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				this.BreakLink();
				return;
			}
			if (!isGripPressed || !this.grabbedLink.myRig.gameObject.activeSelf)
			{
				this.BreakLink();
				return;
			}
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(this.grabbedPlayer))
			{
				this.BreakLink();
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.grabbedLink.myRig);
			GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
			if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
			{
				this.BreakLink();
				return;
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
			{
				this.BreakLink();
				return;
			}
		}
	}

	// Token: 0x060035EC RID: 13804 RVA: 0x001247A2 File Offset: 0x001229A2
	public void RejectGrabsFor(float duration)
	{
		this.rejectGrabsUntilTimestamp = Mathf.Max(this.rejectGrabsUntilTimestamp, Time.time + duration);
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x001247BC File Offset: 0x001229BC
	public void Write(out bool isGroundedHand, out bool isGroundedButt, out int grabbedPlayerActorNumber, out bool grabbedHandIsLeft)
	{
		isGroundedHand = this.isGroundedHand;
		isGroundedButt = this.isGroundedButt;
		if (this.grabbedPlayer != null)
		{
			grabbedPlayerActorNumber = this.grabbedPlayer.ActorNumber;
			grabbedHandIsLeft = this.grabbedHandIsLeft;
			return;
		}
		grabbedPlayerActorNumber = 0;
		grabbedHandIsLeft = false;
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x001247F4 File Offset: 0x001229F4
	public void Read(Vector3 remoteHandLocalPos, Quaternion remoteBodyWorldRot, Vector3 remoteBodyWorldPos, bool isGroundedHand, bool isGroundedButt, bool isGripReady, int grabbedPlayerActorNumber, bool grabbedHandIsLeft)
	{
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		this.canBeGrabbed = isGripReady;
		if (grabbedPlayerActorNumber == 0)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
			}
			bool flag = this.grabbedPlayer != null;
			this.grabbedPlayer = null;
			this.grabbedLink = null;
			if (flag)
			{
				Action onHandLinkChanged = HandLink.OnHandLinkChanged;
				if (onHandLinkChanged != null)
				{
					onHandLinkChanged.Invoke();
				}
			}
		}
		else if (this.lastReadGrabbedPlayerActorNumber == grabbedPlayerActorNumber)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsValid && this.grabbedPlayer.ActorNumber == grabbedPlayerActorNumber && this.grabbedPlayer.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 7f))
			{
				if (this.grabbedHandIsLeft)
				{
					VRRig.LocalRig.leftHandLink.BreakLink();
				}
				else
				{
					VRRig.LocalRig.rightHandLink.BreakLink();
				}
			}
		}
		else
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this);
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(grabbedPlayerActorNumber);
			if (player != null)
			{
				if (player.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 0.25f))
				{
					bool flag2 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag2)
					{
						Action onHandLinkChanged2 = HandLink.OnHandLinkChanged;
						if (onHandLinkChanged2 != null)
						{
							onHandLinkChanged2.Invoke();
						}
					}
				}
				else if (player == this.myRig.OwningNetPlayer)
				{
					bool flag3 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag3)
					{
						Action onHandLinkChanged3 = HandLink.OnHandLinkChanged;
						if (onHandLinkChanged3 != null)
						{
							onHandLinkChanged3.Invoke();
						}
					}
				}
				else
				{
					this.grabbedPlayer = player;
					this.grabbedHandIsLeft = grabbedHandIsLeft;
					this.CheckFormLinkWithRemoteGrab();
					Action onHandLinkChanged4 = HandLink.OnHandLinkChanged;
					if (onHandLinkChanged4 != null)
					{
						onHandLinkChanged4.Invoke();
					}
				}
			}
			else
			{
				bool flag4 = this.grabbedPlayer != null;
				this.grabbedPlayer = null;
				this.grabbedLink = null;
				if (flag4)
				{
					Action onHandLinkChanged5 = HandLink.OnHandLinkChanged;
					if (onHandLinkChanged5 != null)
					{
						onHandLinkChanged5.Invoke();
					}
				}
			}
		}
		this.lastReadGrabbedPlayerActorNumber = grabbedPlayerActorNumber;
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x00124A3B File Offset: 0x00122C3B
	private bool IsLocalGrabInRange(bool grabbedLeftHand, Vector3 handLocalPos, Quaternion bodyWorldRot, Vector3 bodyWorldPos, float tolerance)
	{
		return ((grabbedLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).transform.position - (bodyWorldPos + bodyWorldRot * handLocalPos)).IsShorterThan(tolerance);
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x00124A7C File Offset: 0x00122C7C
	private void CheckFormLinkWithRemoteGrab()
	{
		RigContainer rigContainer;
		if (this.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			HandLink handLink = this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			if (handLink.canBeGrabbed && Time.time > handLink.rejectGrabsUntilTimestamp)
			{
				handLink.CreateLink(this);
				return;
			}
		}
		else if (VRRigCache.Instance.TryGetVrrig(this.grabbedPlayer, out rigContainer))
		{
			HandLink handLink2 = this.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
			if (handLink2.grabbedPlayer == this.myRig.creator)
			{
				this.grabbedLink = handLink2;
				this.grabbedLink.grabbedLink = this;
			}
		}
	}

	// Token: 0x060035F1 RID: 13809 RVA: 0x00124B34 File Offset: 0x00122D34
	public HandLinkAuthorityStatus GetChainAuthority(out int stepsToAuth)
	{
		HandLink handLink = this.grabbedLink;
		int num = 1;
		HandLinkAuthorityStatus handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, -1f, -1);
		stepsToAuth = -1;
		while (handLink != null && num < 10 && !handLink.IsLocal)
		{
			if (handLink.isGroundedHand)
			{
				stepsToAuth = num;
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded, -1f, -1);
			}
			if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ResidualHandGrounded && (double)(handLink.myRig.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, handLink.myRig.LastHandTouchedGroundAtNetworkTime, handLink.myRig.OwningNetPlayer.ActorNumber);
			}
			else if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ButtGrounded && handLink.isGroundedButt)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded, -1f, -1);
			}
			else if (handLinkAuthorityStatus.type == HandLinkAuthorityType.None)
			{
				HandLinkAuthorityStatus handLinkAuthorityStatus2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, handLink.myRig.LastTouchedGroundAtNetworkTime, handLink.myRig.OwningNetPlayer.ActorNumber);
				if (handLinkAuthorityStatus2 > handLinkAuthorityStatus)
				{
					stepsToAuth = num;
					handLinkAuthorityStatus = handLinkAuthorityStatus2;
				}
			}
			num++;
			handLink = handLink.myOtherHandLink.grabbedLink;
		}
		return handLinkAuthorityStatus;
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x00124C4C File Offset: 0x00122E4C
	public void SnapHandsTogether()
	{
		if (this.grabbedLink == null)
		{
			return;
		}
		if (this.grabbedLink.snapPositionCalculatedAtFrame == Time.frameCount)
		{
			this.snapPositionCalculatedAtFrame = Time.frameCount;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = this.grabbedLink.transform.position;
		Vector3 vector = (position + position2) / 2f;
		Vector3 vector2 = (this.isLeftHand ? this.myRig.leftHand.rigTarget : this.myRig.rightHand.rigTarget).position - position;
		Vector3 vector3 = (this.grabbedLink.isLeftHand ? this.grabbedLink.myRig.leftHand.rigTarget : this.grabbedLink.myRig.rightHand.rigTarget).position - position2;
		Vector3 targetWorldPos = vector + vector2;
		Vector3 targetWorldPos2 = vector + vector3;
		this.myIK.OverrideTargetPos(this.isLeftHand, targetWorldPos);
		this.grabbedLink.myIK.OverrideTargetPos(this.grabbedLink.isLeftHand, targetWorldPos2);
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x00124D70 File Offset: 0x00122F70
	public void PlayVicariousTapHaptic()
	{
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnVicariousTap, this.hapticDurationOnVicariousTap);
	}

	// Token: 0x0400452D RID: 17709
	[FormerlySerializedAs("myPlayer")]
	[SerializeField]
	public VRRig myRig;

	// Token: 0x0400452E RID: 17710
	[FormerlySerializedAs("leftHand")]
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x0400452F RID: 17711
	[SerializeField]
	public GorillaIK myIK;

	// Token: 0x04004530 RID: 17712
	private HandLink myOtherHandLink;

	// Token: 0x04004531 RID: 17713
	private bool canBeGrabbed;

	// Token: 0x04004532 RID: 17714
	public bool isGroundedHand;

	// Token: 0x04004533 RID: 17715
	public bool isGroundedButt;

	// Token: 0x04004534 RID: 17716
	private bool wasGripPressed;

	// Token: 0x04004535 RID: 17717
	private float gripPressedAtTimestamp;

	// Token: 0x04004536 RID: 17718
	private float rejectGrabsUntilTimestamp;

	// Token: 0x04004537 RID: 17719
	public HandLink grabbedLink;

	// Token: 0x04004538 RID: 17720
	public NetPlayer grabbedPlayer;

	// Token: 0x04004539 RID: 17721
	public bool grabbedHandIsLeft;

	// Token: 0x0400453B RID: 17723
	private const bool DEBUG_GRAB_ANYONE = false;

	// Token: 0x0400453C RID: 17724
	[SerializeField]
	private float hapticStrengthOnGrab;

	// Token: 0x0400453D RID: 17725
	[SerializeField]
	private float hapticDurationOnGrab;

	// Token: 0x0400453E RID: 17726
	[SerializeField]
	private float hapticStrengthOnVicariousTap;

	// Token: 0x0400453F RID: 17727
	[SerializeField]
	private float hapticDurationOnVicariousTap;

	// Token: 0x04004540 RID: 17728
	[SerializeField]
	private AudioClip audioOnGrab;

	// Token: 0x04004541 RID: 17729
	public InteractionPoint interactionPoint;

	// Token: 0x04004542 RID: 17730
	public static Action OnHandLinkChanged;

	// Token: 0x04004543 RID: 17731
	private int lastReadGrabbedPlayerActorNumber;

	// Token: 0x04004544 RID: 17732
	private int snapPositionCalculatedAtFrame = -1;
}
