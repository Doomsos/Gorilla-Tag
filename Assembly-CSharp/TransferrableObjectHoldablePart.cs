using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000431 RID: 1073
public class TransferrableObjectHoldablePart : HoldableObject, ITickSystemTick
{
	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06001A51 RID: 6737 RVA: 0x0008C007 File Offset: 0x0008A207
	// (set) Token: 0x06001A52 RID: 6738 RVA: 0x0008C00F File Offset: 0x0008A20F
	public bool TickRunning { get; set; }

	// Token: 0x06001A53 RID: 6739 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001A54 RID: 6740 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06001A55 RID: 6741 RVA: 0x0008C018 File Offset: 0x0008A218
	public void Tick()
	{
		VRRig rig;
		if (!this.transferrableParentObject.IsLocalObject())
		{
			rig = this.transferrableParentObject.myOnlineRig;
			this.isHeld = ((this.transferrableParentObject.itemState & this.heldBit) > (TransferrableObject.ItemStates)0);
			TransferrableObject.PositionState currentState = this.transferrableParentObject.currentState;
			if (currentState == TransferrableObject.PositionState.OnRightArm || currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.isHeldLeftHand = this.isHeld;
			}
			else
			{
				this.isHeldLeftHand = false;
			}
		}
		else
		{
			rig = VRRig.LocalRig;
		}
		if (this.isHeld)
		{
			if (this.transferrableParentObject.InHand())
			{
				this.UpdateHeld(rig, this.isHeldLeftHand);
				return;
			}
			if (this.transferrableParentObject.IsLocalObject())
			{
				this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			}
		}
	}

	// Token: 0x06001A56 RID: 6742 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
	}

	// Token: 0x06001A57 RID: 6743 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001A58 RID: 6744 RVA: 0x0008C0E4 File Offset: 0x0008A2E4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.transferrableParentObject.ownerRig && !this.transferrableParentObject.ownerRig.isLocal)
		{
			return;
		}
		this.isHeld = true;
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		this.transferrableParentObject.itemState |= this.heldBit;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x06001A59 RID: 6745 RVA: 0x0008C170 File Offset: 0x0008A370
	public override void DropItemCleanup()
	{
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
	}

	// Token: 0x06001A5A RID: 6746 RVA: 0x0008C19C File Offset: 0x0008A39C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		this.transferrableParentObject.itemState &= ~this.heldBit;
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		return true;
	}

	// Token: 0x040023DB RID: 9179
	[SerializeField]
	protected TransferrableObject transferrableParentObject;

	// Token: 0x040023DC RID: 9180
	[SerializeField]
	private TransferrableObject.ItemStates heldBit = TransferrableObject.ItemStates.Part0Held;

	// Token: 0x040023DD RID: 9181
	private bool isHeld;

	// Token: 0x040023DE RID: 9182
	protected bool isHeldLeftHand;

	// Token: 0x040023DF RID: 9183
	public UnityEvent onGrab;

	// Token: 0x040023E0 RID: 9184
	public UnityEvent onRelease;

	// Token: 0x040023E1 RID: 9185
	public UnityEvent onDrop;
}
