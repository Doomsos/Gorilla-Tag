using System;
using TMPro;
using UnityEngine;

// Token: 0x0200093D RID: 2365
public class CustomMapsTerminalControlButton : CustomMapsScreenTouchPoint
{
	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x06003C67 RID: 15463 RVA: 0x0013F057 File Offset: 0x0013D257
	// (set) Token: 0x06003C68 RID: 15464 RVA: 0x0013F05F File Offset: 0x0013D25F
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		set
		{
			this.isLocked = value;
		}
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x0013F068 File Offset: 0x0013D268
	protected override void OnButtonPressedEvent()
	{
		GTDev.Log<string>("terminal control pressed", null);
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x0013F08F File Offset: 0x0013D28F
	public void LockTerminalControl()
	{
		if (this.IsLocked)
		{
			return;
		}
		this.IsLocked = true;
		this.PressButtonColourUpdate();
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x0013F0A7 File Offset: 0x0013D2A7
	public void UnlockTerminalControl()
	{
		if (!this.IsLocked)
		{
			return;
		}
		this.IsLocked = false;
		this.PressButtonColourUpdate();
	}

	// Token: 0x06003C6C RID: 15468 RVA: 0x0013F0C0 File Offset: 0x0013D2C0
	public override void PressButtonColourUpdate()
	{
		this.bttnText.fontSize = (this.isLocked ? this.lockedFontSize : this.unlockedFontSize);
		this.bttnText.text = (this.isLocked ? this.lockedText : this.unlockedText);
		this.bttnText.color = (this.isLocked ? this.lockedTextColor : this.unlockedTextColor);
		this.touchPointRenderer.color = (this.isLocked ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x04004D0A RID: 19722
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x04004D0B RID: 19723
	[SerializeField]
	private string unlockedText = "TERMINAL AVAILABLE";

	// Token: 0x04004D0C RID: 19724
	[SerializeField]
	private string lockedText = "TERMINAL UNAVAILABLE";

	// Token: 0x04004D0D RID: 19725
	[SerializeField]
	private float unlockedFontSize = 30f;

	// Token: 0x04004D0E RID: 19726
	[SerializeField]
	private float lockedFontSize = 30f;

	// Token: 0x04004D0F RID: 19727
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x04004D10 RID: 19728
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x04004D11 RID: 19729
	private bool isLocked;

	// Token: 0x04004D12 RID: 19730
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
