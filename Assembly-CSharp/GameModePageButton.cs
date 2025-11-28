using System;
using UnityEngine;

// Token: 0x02000517 RID: 1303
public class GameModePageButton : GorillaPressableButton
{
	// Token: 0x06002135 RID: 8501 RVA: 0x000AEF40 File Offset: 0x000AD140
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.selector.ChangePage(this.left);
	}

	// Token: 0x04002BC3 RID: 11203
	[SerializeField]
	private GameModePages selector;

	// Token: 0x04002BC4 RID: 11204
	[SerializeField]
	private bool left;
}
