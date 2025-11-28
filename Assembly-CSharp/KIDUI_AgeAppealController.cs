using System;
using System.Collections.Generic;
using KID.Model;
using UnityEngine;

// Token: 0x02000A94 RID: 2708
public class KIDUI_AgeAppealController : MonoBehaviour
{
	// Token: 0x1700066B RID: 1643
	// (get) Token: 0x06004412 RID: 17426 RVA: 0x001687A0 File Offset: 0x001669A0
	public static KIDUI_AgeAppealController Instance
	{
		get
		{
			return KIDUI_AgeAppealController._instance;
		}
	}

	// Token: 0x06004413 RID: 17427 RVA: 0x001687A7 File Offset: 0x001669A7
	private void Awake()
	{
		KIDUI_AgeAppealController._instance = this;
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x06004414 RID: 17428 RVA: 0x001687C0 File Offset: 0x001669C0
	public void StartAgeAppealScreens(SessionStatus? sessionStatus)
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Age Appeal Screens", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._firstAgeAppealScreen.ShowRestrictedAccessScreen(sessionStatus);
		AgeStatusType ageStatusType;
		if (KIDManager.TryGetAgeStatusTypeFromAge(KIDAgeGate.UserAge, out ageStatusType))
		{
			TelemetryData telemetryData = default(TelemetryData);
			telemetryData.EventName = "kid_age_appeal";
			telemetryData.CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("submitted_age", ageStatusType.ToString());
			telemetryData.BodyData = dictionary;
			TelemetryData telemetryData2 = telemetryData;
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		}
	}

	// Token: 0x06004415 RID: 17429 RVA: 0x00168881 File Offset: 0x00166A81
	public void CloseKIDScreens()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		this._firstAgeAppealScreen.gameObject.SetActive(false);
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06004416 RID: 17430 RVA: 0x001688B4 File Offset: 0x00166AB4
	public void StartTooYoungToPlayScreen()
	{
		Debug.LogFormat("[KID::UI::AGEAPPEALCONTROLLER] Showing k-ID Too Young to Play Screen", Array.Empty<object>());
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(base.transform);
		this._tooYoungToPlayScreen.ShowTooYoungToPlayScreen();
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_screen_shown";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_appeal",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen", "blocked");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x06004417 RID: 17431 RVA: 0x0016895E File Offset: 0x00166B5E
	public void OnQuitGamePressed()
	{
		Application.Quit();
	}

	// Token: 0x06004418 RID: 17432 RVA: 0x0016470C File Offset: 0x0016290C
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x0400558E RID: 21902
	private static KIDUI_AgeAppealController _instance;

	// Token: 0x0400558F RID: 21903
	[SerializeField]
	private KIDUI_RestrictedAccessScreen _firstAgeAppealScreen;

	// Token: 0x04005590 RID: 21904
	[SerializeField]
	private KIDUI_TooYoungToPlay _tooYoungToPlayScreen;
}
