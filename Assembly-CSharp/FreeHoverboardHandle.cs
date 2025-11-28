using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000808 RID: 2056
public class FreeHoverboardHandle : HoldableObject
{
	// Token: 0x0600361C RID: 13852 RVA: 0x00125988 File Offset: 0x00123B88
	private void Awake()
	{
		this.hasParentBoard = (this.parentFreeBoard != null);
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x0012599C File Offset: 0x00123B9C
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

	// Token: 0x0600361E RID: 13854 RVA: 0x00125A0C File Offset: 0x00123C0C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!GTPlayer.Instance.isHoverAllowed)
		{
			return;
		}
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.hasParentBoard)
		{
			FreeHoverboardManager.instance.SendGrabBoardRPC(this.parentFreeBoard);
			Transform transform = flag ? VRRig.LocalRig.leftHand.rigTarget : VRRig.LocalRig.rightHand.rigTarget;
			Quaternion rot = transform.InverseTransformRotation(base.transform.rotation);
			Vector3 pos = transform.InverseTransformPoint(base.transform.position);
			GTPlayer.Instance.GrabPersonalHoverboard(flag, pos, rot, this.parentFreeBoard.boardColor);
			return;
		}
		Quaternion rot2 = flag ? this.defaultHoldAngleLeft : this.defaultHoldAngleRight;
		Vector3 pos2 = flag ? this.defaultHoldPosLeft : this.defaultHoldPosRight;
		GTPlayer.Instance.GrabPersonalHoverboard(flag, pos2, rot2, VRRig.LocalRig.playerColor);
	}

	// Token: 0x0600361F RID: 13855 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x000029BC File Offset: 0x00000BBC
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0400457C RID: 17788
	[SerializeField]
	private FreeHoverboardInstance parentFreeBoard;

	// Token: 0x0400457D RID: 17789
	private bool hasParentBoard;

	// Token: 0x0400457E RID: 17790
	[SerializeField]
	private Vector3 defaultHoldPosLeft;

	// Token: 0x0400457F RID: 17791
	[SerializeField]
	private Vector3 defaultHoldPosRight;

	// Token: 0x04004580 RID: 17792
	[SerializeField]
	private Quaternion defaultHoldAngleLeft;

	// Token: 0x04004581 RID: 17793
	[SerializeField]
	private Quaternion defaultHoldAngleRight;

	// Token: 0x04004582 RID: 17794
	private int noHapticsUntilFrame = -1;
}
