using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200080F RID: 2063
public class HoverboardHandle : HoldableObject
{
	// Token: 0x06003646 RID: 13894 RVA: 0x001264F8 File Offset: 0x001246F8
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		if (Time.frameCount > this.noHapticsUntilFrame)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.noHapticsUntilFrame = Time.frameCount + 1;
	}

	// Token: 0x06003647 RID: 13895 RVA: 0x00126568 File Offset: 0x00124768
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
		Quaternion localRotation;
		Vector3 localPosition;
		if (!this.parentVisual.IsHeld)
		{
			localRotation = (flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight);
			localPosition = (flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight);
		}
		else
		{
			localRotation = transform.InverseTransformRotation(this.parentVisual.transform.rotation);
			localPosition = transform.InverseTransformPoint(this.parentVisual.transform.position);
		}
		this.parentVisual.SetIsHeld(flag, localPosition, localRotation, this.parentVisual.boardColor);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06003648 RID: 13896 RVA: 0x00126641 File Offset: 0x00124841
	public override void DropItemCleanup()
	{
		if (this.parentVisual.gameObject.activeSelf)
		{
			this.parentVisual.DropFreeBoard();
		}
		this.parentVisual.SetNotHeld();
	}

	// Token: 0x06003649 RID: 13897 RVA: 0x0012666C File Offset: 0x0012486C
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
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.parentVisual.IsLeftHanded);
		this.parentVisual.SetNotHeld();
		return true;
	}

	// Token: 0x040045A7 RID: 17831
	[SerializeField]
	private HoverboardVisual parentVisual;

	// Token: 0x040045A8 RID: 17832
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x040045A9 RID: 17833
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x040045AA RID: 17834
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x040045AB RID: 17835
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x040045AC RID: 17836
	private int noHapticsUntilFrame = -1;
}
