using System;
using TMPro;
using UnityEngine;

// Token: 0x02000AB6 RID: 2742
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x060044C5 RID: 17605 RVA: 0x0016C8EE File Offset: 0x0016AAEE
	public void Show(string errorMessage)
	{
		base.gameObject.SetActive(true);
		if (errorMessage != null && errorMessage.Length > 0)
		{
			this._errorTxt.text = errorMessage;
		}
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x0016C914 File Offset: 0x0016AB14
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x0016470C File Offset: 0x0016290C
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x0400567C RID: 22140
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x0400567D RID: 22141
	[SerializeField]
	private TMP_Text _errorTxt;
}
