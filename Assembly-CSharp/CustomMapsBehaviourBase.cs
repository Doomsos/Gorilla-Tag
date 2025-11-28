using System;
using UnityEngine;

// Token: 0x0200095D RID: 2397
public abstract class CustomMapsBehaviourBase
{
	// Token: 0x06003D60 RID: 15712
	public abstract bool CanExecute();

	// Token: 0x06003D61 RID: 15713
	public abstract void Execute();

	// Token: 0x06003D62 RID: 15714
	public abstract void NetExecute();

	// Token: 0x06003D63 RID: 15715
	public abstract void ResetBehavior();

	// Token: 0x06003D64 RID: 15716
	public abstract bool CanContinueExecuting();

	// Token: 0x06003D65 RID: 15717
	public abstract void OnTriggerEnter(Collider otherCollider);
}
