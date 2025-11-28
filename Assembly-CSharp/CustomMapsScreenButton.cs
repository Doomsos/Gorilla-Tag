using System;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020009A3 RID: 2467
public class CustomMapsScreenButton : CustomMapsScreenTouchPoint
{
	// Token: 0x06003EF4 RID: 16116 RVA: 0x00151956 File Offset: 0x0014FB56
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.isToggle)
		{
			this.SetButtonActive(this.isActive);
			return;
		}
		this.isActive = false;
	}

	// Token: 0x06003EF5 RID: 16117 RVA: 0x0015197A File Offset: 0x0014FB7A
	public void SetButtonText(string text)
	{
		if (this.bttnText.IsNull())
		{
			return;
		}
		this.bttnText.text = text;
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x00151996 File Offset: 0x0014FB96
	public void SetButtonActive(bool active)
	{
		this.isActive = active;
		this.touchPointRenderer.color = (this.isActive ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x06003EF7 RID: 16119 RVA: 0x001519CA File Offset: 0x0014FBCA
	public override void PressButtonColourUpdate()
	{
		if (!this.isToggle)
		{
			base.PressButtonColourUpdate();
			return;
		}
	}

	// Token: 0x06003EF8 RID: 16120 RVA: 0x001519DB File Offset: 0x0014FBDB
	protected override void OnButtonPressedEvent()
	{
		this.isActive = !this.isActive;
	}

	// Token: 0x04005023 RID: 20515
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x04005024 RID: 20516
	[SerializeField]
	private bool isToggle;

	// Token: 0x04005025 RID: 20517
	private bool isActive;
}
