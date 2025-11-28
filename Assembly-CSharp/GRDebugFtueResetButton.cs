using System;
using UnityEngine;

// Token: 0x02000696 RID: 1686
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x06002B00 RID: 11008 RVA: 0x000E7625 File Offset: 0x000E5825
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002B01 RID: 11009 RVA: 0x000E763B File Offset: 0x000E583B
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002B02 RID: 11010 RVA: 0x000E7651 File Offset: 0x000E5851
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x06002B03 RID: 11011 RVA: 0x000E7666 File Offset: 0x000E5866
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x0400378B RID: 14219
	public bool availableOnLive;
}
