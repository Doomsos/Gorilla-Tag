using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000452 RID: 1106
[RequireComponent(typeof(RigContainer))]
public class VRRigEvents : MonoBehaviour, IPreDisable
{
	// Token: 0x06001C19 RID: 7193 RVA: 0x000956D6 File Offset: 0x000938D6
	public void PreDisable()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.disableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(this.rigRef);
	}

	// Token: 0x06001C1A RID: 7194 RVA: 0x000956EE File Offset: 0x000938EE
	public void SendPostEnableEvent()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.enableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(this.rigRef);
	}

	// Token: 0x0400261B RID: 9755
	[SerializeField]
	private RigContainer rigRef;

	// Token: 0x0400261C RID: 9756
	public DelegateListProcessor<RigContainer> disableEvent = new DelegateListProcessor<RigContainer>(5);

	// Token: 0x0400261D RID: 9757
	public DelegateListProcessor<RigContainer> enableEvent = new DelegateListProcessor<RigContainer>(5);
}
