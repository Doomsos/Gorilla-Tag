using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000ABA RID: 2746
public class KIDUI_SetupScreen : MonoBehaviour
{
	// Token: 0x060044D4 RID: 17620 RVA: 0x0016CC98 File Offset: 0x0016AE98
	private void Awake()
	{
		if (this._emailInputField == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email Input Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._confirmScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Confirm Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x060044D5 RID: 17621 RVA: 0x0016CD00 File Offset: 0x0016AF00
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "");
		this._emailInputField.text = @string;
		this._confirmButton.ResetButton();
		this.OnInputChanged(@string);
	}

	// Token: 0x060044D6 RID: 17622 RVA: 0x0016CD3B File Offset: 0x0016AF3B
	private void OnDisable()
	{
		if (this._keyboard == null)
		{
			return;
		}
		this._keyboard.active = false;
	}

	// Token: 0x060044D7 RID: 17623 RVA: 0x0016CD54 File Offset: 0x0016AF54
	public void OnStartSetup()
	{
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
		dictionary.Add("screen", "enter_email");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x060044D8 RID: 17624 RVA: 0x0016CDDB File Offset: 0x0016AFDB
	public void OnInputSelected()
	{
		Debug.LogFormat("[KID::UI::SETUP] Email Input Selected!", Array.Empty<object>());
	}

	// Token: 0x060044D9 RID: 17625 RVA: 0x0016CDEC File Offset: 0x0016AFEC
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x060044DA RID: 17626 RVA: 0x0016CE1E File Offset: 0x0016B01E
	public void OnSubmitEmailPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._confirmScreen.OnEmailSubmitted(this._emailInputField.text);
	}

	// Token: 0x060044DB RID: 17627 RVA: 0x0016CE5C File Offset: 0x0016B05C
	public void OnBackPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Previous);
	}

	// Token: 0x0400568B RID: 22155
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x0400568C RID: 22156
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x0400568D RID: 22157
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x0400568E RID: 22158
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x0400568F RID: 22159
	[SerializeField]
	private TMP_Text _riftKeyboardMessage;

	// Token: 0x04005690 RID: 22160
	private string _emailStr = string.Empty;

	// Token: 0x04005691 RID: 22161
	private TouchScreenKeyboard _keyboard;
}
