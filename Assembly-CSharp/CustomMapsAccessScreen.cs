using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000992 RID: 2450
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x06003E50 RID: 15952 RVA: 0x0014C524 File Offset: 0x0014A724
	private void LateUpdate()
	{
		if (CustomMapsTerminal.GetDriverID() == -2)
		{
			return;
		}
		if (CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
		{
			return;
		}
		this.useNametags = GorillaComputer.instance.NametagsEnabled;
		this.SetDriverName();
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x00002789 File Offset: 0x00000989
	public override void Initialize()
	{
	}

	// Token: 0x06003E52 RID: 15954 RVA: 0x0014C580 File Offset: 0x0014A780
	public override void Show()
	{
		base.Show();
		if (this.displayedText == string.Empty)
		{
			this.displayedText = this.defaultText;
		}
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x06003E53 RID: 15955 RVA: 0x0014C5E4 File Offset: 0x0014A7E4
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		base.Hide();
	}

	// Token: 0x06003E54 RID: 15956 RVA: 0x0014C60E File Offset: 0x0014A80E
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.displayedText = this.defaultText;
	}

	// Token: 0x06003E55 RID: 15957 RVA: 0x0014C63E File Offset: 0x0014A83E
	public void SetDetailsScreenForDriver()
	{
		this.displayedText = this.detailsScreenText;
	}

	// Token: 0x06003E56 RID: 15958 RVA: 0x0014C64C File Offset: 0x0014A84C
	public void SetDriverName()
	{
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		string text;
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.GetDriverID());
			text = netPlayerByID.DefaultName;
			if (this.useNametags && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		this.displayedText = "TERMINAL CONTROLLED BY: " + text;
		if (!this.isControlScreen)
		{
			this.displayedText += this.detailsScreenText;
		}
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x0014C727 File Offset: 0x0014A927
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x04004F3C RID: 20284
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04004F3D RID: 20285
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x04004F3E RID: 20286
	[SerializeField]
	private bool isControlScreen = true;

	// Token: 0x04004F3F RID: 20287
	[SerializeField]
	private string defaultText = "PRESS THE 'TERMINAL AVAILABLE' BUTTON TO PROCEED.";

	// Token: 0x04004F40 RID: 20288
	private string detailsScreenText = "\nMAP DETAILS WILL APPEAR HERE WHEN A MAP IS SELECTED.";

	// Token: 0x04004F41 RID: 20289
	private string displayedText = string.Empty;

	// Token: 0x04004F42 RID: 20290
	private bool useNametags;
}
