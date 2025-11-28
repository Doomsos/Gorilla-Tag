using System;
using UnityEngine;

// Token: 0x02000399 RID: 921
public class NativeSizeChangerButton : GorillaPressableButton
{
	// Token: 0x06001606 RID: 5638 RVA: 0x0007ABB4 File Offset: 0x00078DB4
	public override void ButtonActivation()
	{
		this.nativeSizeChanger.Activate(this.settings);
	}

	// Token: 0x0400204B RID: 8267
	[SerializeField]
	private NativeSizeChanger nativeSizeChanger;

	// Token: 0x0400204C RID: 8268
	[SerializeField]
	private NativeSizeChangerSettings settings;
}
