using System;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public interface IHoldableObject
{
	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001D57 RID: 7511
	GameObject gameObject { get; }

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001D58 RID: 7512
	// (set) Token: 0x06001D59 RID: 7513
	string name { get; set; }

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001D5A RID: 7514
	bool TwoHanded { get; }

	// Token: 0x06001D5B RID: 7515
	void OnHover(InteractionPoint pointHovered, GameObject hoveringHand);

	// Token: 0x06001D5C RID: 7516
	void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand);

	// Token: 0x06001D5D RID: 7517
	bool OnRelease(DropZone zoneReleased, GameObject releasingHand);

	// Token: 0x06001D5E RID: 7518
	void DropItemCleanup();
}
