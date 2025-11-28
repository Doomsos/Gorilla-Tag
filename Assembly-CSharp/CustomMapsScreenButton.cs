using System;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x020009A3 RID: 2467
public class CustomMapsScreenButton : CustomMapsScreenTouchPoint
{
	// Token: 0x06003EF4 RID: 16116 RVA: 0x00151976 File Offset: 0x0014FB76
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

	// Token: 0x06003EF5 RID: 16117 RVA: 0x0015199A File Offset: 0x0014FB9A
	public void SetButtonText(string text)
	{
		if (this.bttnText.IsNull())
		{
			return;
		}
		this.bttnText.text = text;
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x001519B6 File Offset: 0x0014FBB6
	public void SetButtonActive(bool active)
	{
		this.isActive = active;
		this.touchPointRenderer.color = (this.isActive ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x06003EF7 RID: 16119 RVA: 0x001519EA File Offset: 0x0014FBEA
	public override void PressButtonColourUpdate()
	{
		if (!this.isToggle)
		{
			base.PressButtonColourUpdate();
			return;
		}
	}

	// Token: 0x06003EF8 RID: 16120 RVA: 0x001519FB File Offset: 0x0014FBFB
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
