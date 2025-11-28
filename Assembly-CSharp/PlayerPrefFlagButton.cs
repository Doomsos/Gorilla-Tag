using System;
using UnityEngine;

// Token: 0x020003FE RID: 1022
public class PlayerPrefFlagButton : GorillaPressableButton
{
	// Token: 0x060018FB RID: 6395 RVA: 0x00085AC5 File Offset: 0x00083CC5
	protected override void OnEnable()
	{
		base.OnEnable();
		this.isOn = PlayerPrefFlags.Check(this.flag);
		this.UpdateColor();
	}

	// Token: 0x060018FC RID: 6396 RVA: 0x00085AE4 File Offset: 0x00083CE4
	public override void ButtonActivation()
	{
		PlayerPrefFlagButton.ButtonMode buttonMode = this.mode;
		if (buttonMode == PlayerPrefFlagButton.ButtonMode.SET_VALUE)
		{
			PlayerPrefFlags.Set(this.flag, this.value);
			this.isOn = this.value;
			this.UpdateColor();
			return;
		}
		if (buttonMode != PlayerPrefFlagButton.ButtonMode.TOGGLE)
		{
			return;
		}
		this.isOn = PlayerPrefFlags.Flip(this.flag);
		this.UpdateColor();
	}

	// Token: 0x0400224F RID: 8783
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x04002250 RID: 8784
	[SerializeField]
	private PlayerPrefFlagButton.ButtonMode mode;

	// Token: 0x04002251 RID: 8785
	[SerializeField]
	private bool value;

	// Token: 0x020003FF RID: 1023
	private enum ButtonMode
	{
		// Token: 0x04002253 RID: 8787
		SET_VALUE,
		// Token: 0x04002254 RID: 8788
		TOGGLE
	}
}
