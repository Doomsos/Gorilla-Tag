using System;
using TMPro;
using UnityEngine;

// Token: 0x02000AAE RID: 2734
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x06004486 RID: 17542 RVA: 0x0016ADAD File Offset: 0x00168FAD
	public void ShowErrorScreen(string title, string email, string errorMessage)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		this._errorTxt.text = errorMessage;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004487 RID: 17543 RVA: 0x0016ADDF File Offset: 0x00168FDF
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06004488 RID: 17544 RVA: 0x0016895E File Offset: 0x00166B5E
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06004489 RID: 17545 RVA: 0x0016ADF9 File Offset: 0x00168FF9
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x0400562B RID: 22059
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x0400562C RID: 22060
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x0400562D RID: 22061
	[SerializeField]
	private TMP_Text _errorTxt;

	// Token: 0x0400562E RID: 22062
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x0400562F RID: 22063
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
