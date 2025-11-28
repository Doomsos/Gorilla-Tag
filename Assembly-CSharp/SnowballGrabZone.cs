using System;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class SnowballGrabZone : HoldableObject
{
	// Token: 0x06000AFD RID: 2813 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003BCD0 File Offset: 0x00039ED0
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		SnowballThrowable snowballThrowable;
		((grabbingHand == EquipmentInteractor.instance.leftHand) ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out snowballThrowable);
	}

	// Token: 0x04000D6C RID: 3436
	[GorillaSoundLookup]
	public int materialIndex;
}
