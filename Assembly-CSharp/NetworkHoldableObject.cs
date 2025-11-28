using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200048C RID: 1164
[NetworkBehaviourWeaved(0)]
public abstract class NetworkHoldableObject : NetworkComponent, IHoldableObject
{
	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001DCA RID: 7626 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001DCB RID: 7627
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001DCC RID: 7628
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001DCD RID: 7629
	public abstract void DropItemCleanup();

	// Token: 0x06001DCE RID: 7630 RVA: 0x0009CB80 File Offset: 0x0009AD80
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06001DCF RID: 7631 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06001DD1 RID: 7633 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001DD2 RID: 7634 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x00013E3B File Offset: 0x0001203B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x00013E43 File Offset: 0x00012043
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
