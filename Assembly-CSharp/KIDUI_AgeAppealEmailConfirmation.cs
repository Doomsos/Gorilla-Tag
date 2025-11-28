using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000A96 RID: 2710
public class KIDUI_AgeAppealEmailConfirmation : MonoBehaviour
{
	// Token: 0x0600441F RID: 17439 RVA: 0x00168AED File Offset: 0x00166CED
	private void OnEnable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004420 RID: 17440 RVA: 0x00168B0F File Offset: 0x00166D0F
	private void OnDisable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x00168B44 File Offset: 0x00166D44
	public void ShowAgeAppealConfirmationScreen(bool hasChallenge, int newAge, string emailToConfirm)
	{
		this.hasChallenge = hasChallenge;
		this.newAgeToAppeal = newAge;
		this._confirmText.text = (this.hasChallenge ? this.CONFIRM_PARENT_EMAIL : this.CONFIRM_YOUR_EMAIL);
		this._emailText.text = emailToConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x00168B98 File Offset: 0x00166D98
	public void OnConfirmPressed()
	{
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_age_appeal_confirm_email";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_appeal",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("email_type", this.hasChallenge ? "under_dac" : "over_dac");
		dictionary.Add("button_pressed", "confirm");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		if (this.hasChallenge)
		{
			this.StartAgeAppealChallengeEmail();
			return;
		}
		this.StartAgeAppealEmail();
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x00168C48 File Offset: 0x00166E48
	public void OnBackPressed()
	{
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_age_appeal_confirm_email";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_appeal",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("email_type", this.hasChallenge ? "under_dac" : "over_dac");
		dictionary.Add("button_pressed", "go_back");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAgeToAppeal);
	}

	// Token: 0x06004424 RID: 17444 RVA: 0x00168D08 File Offset: 0x00166F08
	private void StartAgeAppealChallengeEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16 <StartAgeAppealChallengeEmail>d__;
		<StartAgeAppealChallengeEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartAgeAppealChallengeEmail>d__.<>4__this = this;
		<StartAgeAppealChallengeEmail>d__.<>1__state = -1;
		<StartAgeAppealChallengeEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16>(ref <StartAgeAppealChallengeEmail>d__);
	}

	// Token: 0x06004425 RID: 17445 RVA: 0x00168D40 File Offset: 0x00166F40
	private Task StartAgeAppealEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17 <StartAgeAppealEmail>d__;
		<StartAgeAppealEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeAppealEmail>d__.<>4__this = this;
		<StartAgeAppealEmail>d__.<>1__state = -1;
		<StartAgeAppealEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17>(ref <StartAgeAppealEmail>d__);
		return <StartAgeAppealEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06004426 RID: 17446 RVA: 0x00168D84 File Offset: 0x00166F84
	private void NotifyOfEmailResult(bool success)
	{
		if (this._successScreen == null)
		{
			Debug.LogError("[KID::AGE_APPEAL_EMAIL] _successScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		this._hasCompletedSendEmailRequest = true;
		if (success)
		{
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
			return;
		}
	}

	// Token: 0x06004427 RID: 17447 RVA: 0x00168DD7 File Offset: 0x00166FD7
	private void ShowErrorScreen()
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		base.gameObject.SetActive(false);
		this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
	}

	// Token: 0x0400559A RID: 21914
	[SerializeField]
	private TMP_Text _confirmText;

	// Token: 0x0400559B RID: 21915
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x0400559C RID: 21916
	private string CONFIRM_PARENT_EMAIL = "Please confirm your parent or guardian's email address.";

	// Token: 0x0400559D RID: 21917
	private string CONFIRM_YOUR_EMAIL = "Please confirm your email address.";

	// Token: 0x0400559E RID: 21918
	private bool hasChallenge = true;

	// Token: 0x0400559F RID: 21919
	private int newAgeToAppeal;

	// Token: 0x040055A0 RID: 21920
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x040055A1 RID: 21921
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x040055A2 RID: 21922
	[SerializeField]
	private KIDUI_AgeAppealEmailError _errorScreen;

	// Token: 0x040055A3 RID: 21923
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x040055A4 RID: 21924
	[SerializeField]
	private int _minimumDelay = 1000;
}
