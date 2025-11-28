using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000459 RID: 1113
public class CosmeticBoundaryTrigger : GorillaTriggerBox
{
	// Token: 0x06001C51 RID: 7249 RVA: 0x00096630 File Offset: 0x00094830
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		if (CosmeticBoundaryTrigger.sinceLastTryOnEvent.HasElapsed(0.5f, true))
		{
			GorillaTelemetry.PostShopEvent(this.rigRef, GTShopEventType.item_try_on, this.rigRef.tryOnSet.items);
		}
		this.rigRef.inTryOnRoom = true;
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x000966E0 File Offset: 0x000948E0
	public void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		this.rigRef = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (this.rigRef == null)
		{
			return;
		}
		this.rigRef.inTryOnRoom = false;
		if (this.rigRef.isOfflineVRRig)
		{
			this.rigRef.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
		}
		this.rigRef.LocalUpdateCosmeticsWithTryon(this.rigRef.cosmeticSet, this.rigRef.tryOnSet, false);
		this.rigRef.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x04002656 RID: 9814
	public VRRig rigRef;

	// Token: 0x04002657 RID: 9815
	private static TimeSince sinceLastTryOnEvent = 0f;
}
