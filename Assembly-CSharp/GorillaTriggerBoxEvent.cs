using System;
using UnityEngine.Events;

// Token: 0x02000528 RID: 1320
public class GorillaTriggerBoxEvent : GorillaTriggerBox
{
	// Token: 0x0600216B RID: 8555 RVA: 0x000AF6C9 File Offset: 0x000AD8C9
	public override void OnBoxTriggered()
	{
		if (this.onBoxTriggered != null)
		{
			this.onBoxTriggered.Invoke();
		}
	}

	// Token: 0x04002C25 RID: 11301
	public UnityEvent onBoxTriggered;
}
