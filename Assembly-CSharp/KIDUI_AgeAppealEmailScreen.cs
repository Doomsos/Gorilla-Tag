using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000A95 RID: 2709
public class KIDUI_AgeAppealEmailScreen : MonoBehaviour
{
	// Token: 0x0600441A RID: 17434 RVA: 0x00168948 File Offset: 0x00166B48
	public void ShowAgeAppealEmailScreen(bool receivedChallenge, int newAge)
	{
		this.newAgeToAppeal = newAge;
		base.gameObject.SetActive(true);
		this.hasChallenge = receivedChallenge;
		this._enterEmailText.text = (this.hasChallenge ? this.PARENT_EMAIL_DESCRIPTION : this.VERIFY_AGE_EMAIL_DESCRIPTION);
		if (this._parentPermissionNotice)
		{
			this._parentPermissionNotice.SetActive(this.hasChallenge);
		}
		this.OnInputChanged(this._emailText.text);
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_age_appeal_enter_email";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_appeal",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("email_type", this.hasChallenge ? "under_dac" : "over_dac");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x0600441B RID: 17435 RVA: 0x00168A3C File Offset: 0x00166C3C
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x0600441C RID: 17436 RVA: 0x00168A70 File Offset: 0x00166C70
	public void OnConfirmPressed()
	{
		if (string.IsNullOrEmpty(this._emailText.text))
		{
			Debug.LogError("[KID::UI::APPEAL_AGE_EMAIL] Age Appeal Email Text is empty");
			return;
		}
		this._confirmationScreen.ShowAgeAppealConfirmationScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005591 RID: 21905
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005592 RID: 21906
	[SerializeField]
	private KIDUI_AgeAppealEmailConfirmation _confirmationScreen;

	// Token: 0x04005593 RID: 21907
	[SerializeField]
	private TMP_Text _enterEmailText;

	// Token: 0x04005594 RID: 21908
	[SerializeField]
	private TMP_InputField _emailText;

	// Token: 0x04005595 RID: 21909
	[SerializeField]
	private GameObject _parentPermissionNotice;

	// Token: 0x04005596 RID: 21910
	private string PARENT_EMAIL_DESCRIPTION = "Enter your parent or guardian's email address below.";

	// Token: 0x04005597 RID: 21911
	private string VERIFY_AGE_EMAIL_DESCRIPTION = "Enter your email address below";

	// Token: 0x04005598 RID: 21912
	private bool hasChallenge = true;

	// Token: 0x04005599 RID: 21913
	private int newAgeToAppeal;
}
