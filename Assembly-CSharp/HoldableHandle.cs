using System;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public class HoldableHandle : InteractionPoint
{
	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001D4A RID: 7498 RVA: 0x0009A94F File Offset: 0x00098B4F
	public new HoldableObject Holdable
	{
		get
		{
			return this.holdable;
		}
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001D4B RID: 7499 RVA: 0x0009A957 File Offset: 0x00098B57
	public CapsuleCollider Capsule
	{
		get
		{
			return this.handleCapsuleTrigger;
		}
	}

	// Token: 0x04002756 RID: 10070
	[SerializeField]
	private HoldableObject holdable;

	// Token: 0x04002757 RID: 10071
	[SerializeField]
	private CapsuleCollider handleCapsuleTrigger;
}
