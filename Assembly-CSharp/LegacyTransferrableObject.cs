using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000482 RID: 1154
public class LegacyTransferrableObject : HoldableObject
{
	// Token: 0x06001D5F RID: 7519 RVA: 0x0009A9D8 File Offset: 0x00098BD8
	protected void Awake()
	{
		this.latched = false;
		this.initOffset = base.transform.localPosition;
		this.initRotation = base.transform.localRotation;
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x0009AA04 File Offset: 0x00098C04
	protected virtual void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x0009AA64 File Offset: 0x00098C64
	public void OnEnable()
	{
		if (this.myRig == null && this.myOnlineRig != null && this.myOnlineRig.netView != null && this.myOnlineRig.netView.IsMine)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.myRig == null && this.myOnlineRig == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.objectIndex = this.targetDock.ReturnTransferrableItemIndex(this.myIndex);
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightArm;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.LeftBack;
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				this.storedZone = BodyDockPositions.DropPositions.RightBack;
			}
			else
			{
				this.storedZone = BodyDockPositions.DropPositions.Chest;
			}
		}
		if (this.objectIndex == -1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
		{
			Transform transform = this.GetAnchor(this.currentState);
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		this.initState = this.currentState;
		this.enabledOnFrame = Time.frameCount;
		this.SpawnShareableObject();
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x0009ABEA File Offset: 0x00098DEA
	public void OnDisable()
	{
		this.enabledOnFrame = -1;
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x0009ABF4 File Offset: 0x00098DF4
	private void SpawnShareableObject()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		if (this.worldShareableInstance != null)
		{
			return;
		}
		object[] array = new object[]
		{
			this.myIndex,
			PhotonNetwork.LocalPlayer
		};
		this.worldShareableInstance = PhotonNetwork.Instantiate("Objects/equipment/WorldShareableItem", base.transform.position, base.transform.rotation, 0, array);
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x0009AC90 File Offset: 0x00098E90
	public void OnJoinedRoom()
	{
		Debug.Log("Here");
		this.SpawnShareableObject();
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x0009ACA2 File Offset: 0x00098EA2
	public void OnLeftRoom()
	{
		if (this.worldShareableInstance != null)
		{
			PhotonNetwork.Destroy(this.worldShareableInstance);
		}
		this.OnWorldShareableItemDeallocated(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x0009ACCD File Offset: 0x00098ECD
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.OnWorldShareableItemDeallocated(otherPlayer);
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0009ACD6 File Offset: 0x00098ED6
	public void SetWorldShareableItem(GameObject item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnWorldShareableItemDeallocated(NetPlayer player)
	{
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x0009ACE8 File Offset: 0x00098EE8
	public virtual void LateUpdate()
	{
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
		this.previousState = this.currentState;
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x0009AD38 File Offset: 0x00098F38
	protected Transform DefaultAnchor()
	{
		if (!(this.anchor == null))
		{
			return this.anchor;
		}
		return base.transform;
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x0009AD55 File Offset: 0x00098F55
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x0009AD84 File Offset: 0x00098F84
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x0009ADB4 File Offset: 0x00098FB4
	private void UpdateFollowXform()
	{
		if (this.targetRig == null)
		{
			return;
		}
		if (this.targetDock == null)
		{
			this.targetDock = this.targetRig.GetComponent<BodyDockPositions>();
		}
		if (this.anchorOverrides == null)
		{
			this.anchorOverrides = this.targetRig.GetComponent<VRRigAnchorOverrides>();
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState <= TransferrableObject.PositionState.InRightHand)
		{
			switch (positionState)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftArmTransform);
				break;
			case TransferrableObject.PositionState.OnRightArm:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightArmTransform);
				break;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftHandTransform);
				break;
			default:
				if (positionState == TransferrableObject.PositionState.InRightHand)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightHandTransform);
				}
				break;
			}
		}
		else if (positionState != TransferrableObject.PositionState.OnChest)
		{
			if (positionState != TransferrableObject.PositionState.OnLeftShoulder)
			{
				if (positionState == TransferrableObject.PositionState.OnRightShoulder)
				{
					transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.rightBackTransform);
				}
			}
			else
			{
				transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.leftBackTransform);
			}
		}
		else
		{
			transform2 = this.anchorOverrides.AnchorOverride(this.currentState, this.targetDock.chestTransform);
		}
		LegacyTransferrableObject.InterpolateState interpolateState = this.interpState;
		if (interpolateState != LegacyTransferrableObject.InterpolateState.None)
		{
			if (interpolateState != LegacyTransferrableObject.InterpolateState.Interpolating)
			{
				return;
			}
			float num = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			transform.transform.position = Vector3.Lerp(this.interpStartPos, transform2.transform.position, num);
			transform.transform.rotation = Quaternion.Slerp(this.interpStartRot, transform2.transform.rotation, num);
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = LegacyTransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
			}
		}
		else if (transform2 != transform.parent)
		{
			if (Time.frameCount == this.enabledOnFrame)
			{
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				return;
			}
			this.interpState = LegacyTransferrableObject.InterpolateState.Interpolating;
			this.interpDt = this.interpTime;
			this.interpStartPos = transform.transform.position;
			this.interpStartRot = transform.transform.rotation;
			return;
		}
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x0009B0D9 File Offset: 0x000992D9
	public void DropItem()
	{
		base.transform.parent = null;
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x0009B0E8 File Offset: 0x000992E8
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		for (int i = 0; i < this.targetRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.targetRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				this.disableItem = false;
				break;
			}
		}
		if (this.disableItem)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.previousState != this.currentState && this.detatchOnGrab && this.InHand())
		{
			base.transform.parent = null;
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped)
		{
			this.UpdateFollowXform();
			return;
		}
		if (this.canDrop)
		{
			this.DropItem();
		}
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x0009B190 File Offset: 0x00099390
	protected void ResetXf()
	{
		if (this.canDrop)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			base.transform.localPosition = this.initOffset;
			base.transform.localRotation = this.initRotation;
		}
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x0009B1FB File Offset: 0x000993FB
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		this.ResetXf();
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x0009B218 File Offset: 0x00099418
	private void HandleLocalInput()
	{
		GameObject[] array;
		if (!this.InHand())
		{
			array = this.gameObjectsActiveOnlyWhileHeld;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			return;
		}
		array = this.gameObjectsActiveOnlyWhileHeld;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		XRNode node = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? 4 : 5;
		this.indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x0009B304 File Offset: 0x00099504
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		if (PhotonNetwork.InRoom)
		{
			this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
			this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
		}
		this.targetRig = this.myRig;
		this.HandleLocalInput();
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x0009B36C File Offset: 0x0009956C
	protected virtual void LateUpdateReplicated()
	{
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (this.currentState == TransferrableObject.PositionState.Dropped && !this.canDrop && !this.shareable)
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.SetActive(false);
			}
			this.currentState = this.previousState;
		}
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.targetRig = this.myOnlineRig;
		if (this.myOnlineRig != null)
		{
			bool flag = true;
			for (int i = 0; i < this.myOnlineRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					flag = false;
					GameObject[] array = this.gameObjectsActiveOnlyWhileHeld;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].SetActive(this.InHand());
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x0009B45E File Offset: 0x0009965E
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		this.ResetXf();
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x0009B484 File Offset: 0x00099684
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!(grabbingHand == this.interactor.leftHand) || this.currentState == TransferrableObject.PositionState.OnLeftArm)
		{
			if (grabbingHand == this.interactor.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
			{
				if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
				{
					return;
				}
				this.canAutoGrabRight = false;
				this.currentState = TransferrableObject.PositionState.InRightHand;
				EquipmentInteractor.instance.UpdateHandEquipment(this, false);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			}
			return;
		}
		if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
		{
			return;
		}
		this.canAutoGrabLeft = false;
		this.currentState = TransferrableObject.PositionState.InLeftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, true);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x0009B58C File Offset: 0x0009978C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (this.IsHeld() && ((releasingHand == EquipmentInteractor.instance.rightHand && EquipmentInteractor.instance.rightHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment) || (releasingHand == EquipmentInteractor.instance.leftHand && EquipmentInteractor.instance.leftHandHeldEquipment != null && this == (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment)))
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				this.canAutoGrabLeft = true;
			}
			else
			{
				this.canAutoGrabRight = true;
			}
			if (zoneReleased != null)
			{
				bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
				bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
				if (flag || flag2)
				{
					return false;
				}
				if (this.targetDock.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDock && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
				{
					this.storedZone = zoneReleased.dropPosition;
				}
			}
			this.DropItemCleanup();
			EquipmentInteractor.instance.UpdateHandEquipment(null, releasingHand == EquipmentInteractor.instance.leftHand);
			return true;
		}
		return false;
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x0009B700 File Offset: 0x00099900
	public override void DropItemCleanup()
	{
		if (this.canDrop)
		{
			this.currentState = TransferrableObject.PositionState.Dropped;
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x0009B774 File Offset: 0x00099974
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x0009B7D8 File Offset: 0x000999D8
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		VRRig vrrig = this.targetRig;
		if ((vrrig != null) ? vrrig.netView : null)
		{
			this.targetRig.rigSerializer.RPC_PlayHandTap(soundIndex, flag, 0.1f, default(PhotonMessageInfo));
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x0009B844 File Offset: 0x00099A44
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x0009B856 File Offset: 0x00099A56
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x0009B85F File Offset: 0x00099A5F
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x06001D82 RID: 7554 RVA: 0x0009B868 File Offset: 0x00099A68
	public virtual bool IsMyItem()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x0009B888 File Offset: 0x00099A88
	protected virtual bool IsHeld()
	{
		return (EquipmentInteractor.instance.leftHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.leftHandHeldEquipment == this) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && (LegacyTransferrableObject)EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06001D84 RID: 7556 RVA: 0x0009B8E5 File Offset: 0x00099AE5
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001D85 RID: 7557 RVA: 0x0009B8FB File Offset: 0x00099AFB
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x0009B90A File Offset: 0x00099B0A
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x0009B915 File Offset: 0x00099B15
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x0009B920 File Offset: 0x00099B20
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x0009B92C File Offset: 0x00099B2C
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x0009B944 File Offset: 0x00099B44
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x04002758 RID: 10072
	protected EquipmentInteractor interactor;

	// Token: 0x04002759 RID: 10073
	public VRRig myRig;

	// Token: 0x0400275A RID: 10074
	public VRRig myOnlineRig;

	// Token: 0x0400275B RID: 10075
	public bool latched;

	// Token: 0x0400275C RID: 10076
	private float indexTrigger;

	// Token: 0x0400275D RID: 10077
	public bool testActivate;

	// Token: 0x0400275E RID: 10078
	public bool testDeactivate;

	// Token: 0x0400275F RID: 10079
	public float myThreshold = 0.8f;

	// Token: 0x04002760 RID: 10080
	public float hysterisis = 0.05f;

	// Token: 0x04002761 RID: 10081
	public bool flipOnXForLeftHand;

	// Token: 0x04002762 RID: 10082
	public bool flipOnYForLeftHand;

	// Token: 0x04002763 RID: 10083
	public bool flipOnXForLeftArm;

	// Token: 0x04002764 RID: 10084
	public bool disableStealing;

	// Token: 0x04002765 RID: 10085
	private TransferrableObject.PositionState initState;

	// Token: 0x04002766 RID: 10086
	public TransferrableObject.ItemStates itemState;

	// Token: 0x04002767 RID: 10087
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x04002768 RID: 10088
	protected TransferrableObject.PositionState previousState;

	// Token: 0x04002769 RID: 10089
	public TransferrableObject.PositionState currentState;

	// Token: 0x0400276A RID: 10090
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x0400276B RID: 10091
	public VRRig targetRig;

	// Token: 0x0400276C RID: 10092
	public BodyDockPositions targetDock;

	// Token: 0x0400276D RID: 10093
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x0400276E RID: 10094
	public bool canAutoGrabLeft;

	// Token: 0x0400276F RID: 10095
	public bool canAutoGrabRight;

	// Token: 0x04002770 RID: 10096
	public int objectIndex;

	// Token: 0x04002771 RID: 10097
	[Tooltip("In Holdables.prefab, assign to the parent of this transform.\nExample: 'Holdables/YellowHandBootsRight' is the anchor of 'Holdables/YellowHandBootsRight/YELLOW HAND BOOTS'")]
	public Transform anchor;

	// Token: 0x04002772 RID: 10098
	[Tooltip("In Holdables.prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04002773 RID: 10099
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x04002774 RID: 10100
	public int myIndex;

	// Token: 0x04002775 RID: 10101
	[Tooltip("(Optional)")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x04002776 RID: 10102
	protected GameObject worldShareableInstance;

	// Token: 0x04002777 RID: 10103
	private float interpTime = 0.1f;

	// Token: 0x04002778 RID: 10104
	private float interpDt;

	// Token: 0x04002779 RID: 10105
	private Vector3 interpStartPos;

	// Token: 0x0400277A RID: 10106
	private Quaternion interpStartRot;

	// Token: 0x0400277B RID: 10107
	protected int enabledOnFrame = -1;

	// Token: 0x0400277C RID: 10108
	private Vector3 initOffset;

	// Token: 0x0400277D RID: 10109
	private Quaternion initRotation;

	// Token: 0x0400277E RID: 10110
	public bool canDrop;

	// Token: 0x0400277F RID: 10111
	public bool shareable;

	// Token: 0x04002780 RID: 10112
	public bool detatchOnGrab;

	// Token: 0x04002781 RID: 10113
	private bool wasHover;

	// Token: 0x04002782 RID: 10114
	private bool isHover;

	// Token: 0x04002783 RID: 10115
	private bool disableItem;

	// Token: 0x04002784 RID: 10116
	public const int kPositionStateCount = 8;

	// Token: 0x04002785 RID: 10117
	public LegacyTransferrableObject.InterpolateState interpState;

	// Token: 0x02000483 RID: 1155
	public enum InterpolateState
	{
		// Token: 0x04002787 RID: 10119
		None,
		// Token: 0x04002788 RID: 10120
		Interpolating
	}
}
