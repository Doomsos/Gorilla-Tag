using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000AAD RID: 2733
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x06004481 RID: 17537 RVA: 0x0016AC6C File Offset: 0x00168E6C
	public void ShowSuccessScreen(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_screen_shown";
		telemetryData.CustomTags = new string[]
		{
			"kid_setup",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen", "email_sent");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x06004482 RID: 17538 RVA: 0x0016AD00 File Offset: 0x00168F00
	public void ShowSuccessScreenAppeal(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_screen_shown";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_appeal",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen", "age_appeal_email_sent");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x06004483 RID: 17539 RVA: 0x0016AD93 File Offset: 0x00168F93
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06004484 RID: 17540 RVA: 0x0016895E File Offset: 0x00166B5E
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x04005629 RID: 22057
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x0400562A RID: 22058
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
