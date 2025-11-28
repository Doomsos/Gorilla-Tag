using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002C2 RID: 706
public class GorillaActionButton : GorillaPressableButton
{
	// Token: 0x0600115D RID: 4445 RVA: 0x0005C36F File Offset: 0x0005A56F
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.onPress.Invoke();
	}

	// Token: 0x040015F9 RID: 5625
	[SerializeField]
	public UnityEvent onPress;
}
