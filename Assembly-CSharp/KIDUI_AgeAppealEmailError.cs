using System;
using TMPro;
using UnityEngine;

// Token: 0x02000A99 RID: 2713
public class KIDUI_AgeAppealEmailError : MonoBehaviour
{
	// Token: 0x0600442D RID: 17453 RVA: 0x001691D2 File Offset: 0x001673D2
	public void ShowAgeAppealEmailErrorScreen(bool hasChallenge, int newAge, string email)
	{
		this.hasChallenge = hasChallenge;
		this.newAge = newAge;
		this._emailText.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x001691FA File Offset: 0x001673FA
	public void onBackPressed()
	{
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAge);
	}

	// Token: 0x040055B0 RID: 21936
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x040055B1 RID: 21937
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x040055B2 RID: 21938
	private bool hasChallenge;

	// Token: 0x040055B3 RID: 21939
	private int newAge;
}
