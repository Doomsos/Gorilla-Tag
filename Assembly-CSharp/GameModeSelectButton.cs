using System;
using UnityEngine;

// Token: 0x02000519 RID: 1305
public class GameModeSelectButton : GorillaPressableButton
{
	// Token: 0x06002144 RID: 8516 RVA: 0x000AF1F7 File Offset: 0x000AD3F7
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.SelectEntryOnPage(this.buttonIndex);
	}

	// Token: 0x04002BCC RID: 11212
	[SerializeField]
	internal GameModePages selector;

	// Token: 0x04002BCD RID: 11213
	[SerializeField]
	internal int buttonIndex;
}
