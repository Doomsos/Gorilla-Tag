using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200020C RID: 524
public class PropHuntGrabbableProp : HoldableObject
{
	// Token: 0x06000E6A RID: 3690 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x0004C2DC File Offset: 0x0004A4DC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		this.handFollower.SwitchHand(flag);
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x0004C318 File Offset: 0x0004A518
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x04001169 RID: 4457
	public PropHuntHandFollower handFollower;

	// Token: 0x0400116A RID: 4458
	public Vector3 offset;

	// Token: 0x0400116B RID: 4459
	public List<InteractionPoint> interactionPoints;
}
