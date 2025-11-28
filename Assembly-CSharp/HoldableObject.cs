using System;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public abstract class HoldableObject : MonoBehaviour, IHoldableObject
{
	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001D4D RID: 7501 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x0009A967 File Offset: 0x00098B67
	protected void OnDestroy()
	{
		if (EquipmentInteractor.hasInstance)
		{
			EquipmentInteractor.instance.ForceDropEquipment(this);
		}
	}

	// Token: 0x06001D4F RID: 7503
	public abstract void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001D50 RID: 7504
	public abstract void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001D51 RID: 7505
	public abstract void DropItemCleanup();

	// Token: 0x06001D52 RID: 7506 RVA: 0x0009A980 File Offset: 0x00098B80
	public virtual bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return (EquipmentInteractor.instance.rightHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.rightHand)) && (EquipmentInteractor.instance.leftHandHeldEquipment != this || !(releasingHand != EquipmentInteractor.instance.leftHand));
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x00013E3B File Offset: 0x0001203B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x00013E43 File Offset: 0x00012043
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}
}
