using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;

// Token: 0x02000999 RID: 2457
public class CustomMapsKeyToggleButton : CustomMapsKeyButton
{
	// Token: 0x06003EAA RID: 16042 RVA: 0x00002789 File Offset: 0x00000989
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x06003EAB RID: 16043 RVA: 0x0014F458 File Offset: 0x0014D658
	public void SetButtonStatus(bool newIsPressed)
	{
		if (this.isPressed == newIsPressed)
		{
			return;
		}
		this.isPressed = newIsPressed;
		this.propBlock.SetColor("_BaseColor", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.propBlock.SetColor("_Color", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
	}

	// Token: 0x04004FA2 RID: 20386
	private bool isPressed;
}
