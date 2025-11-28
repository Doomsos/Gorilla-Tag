using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000A31 RID: 2609
public class KIDAgeGate : MonoBehaviour
{
	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x06004214 RID: 16916 RVA: 0x0015D9CE File Offset: 0x0015BBCE
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	// Token: 0x1700063A RID: 1594
	// (get) Token: 0x06004215 RID: 16917 RVA: 0x0015D9D5 File Offset: 0x0015BBD5
	// (set) Token: 0x06004216 RID: 16918 RVA: 0x0015D9DC File Offset: 0x0015BBDC
	public static bool DisplayedScreen { get; private set; }

	// Token: 0x06004217 RID: 16919 RVA: 0x0015D9E4 File Offset: 0x0015BBE4
	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	// Token: 0x06004218 RID: 16920 RVA: 0x0015DA10 File Offset: 0x0015BC10
	private void Start()
	{
		KIDAgeGate.<Start>d__29 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDAgeGate.<Start>d__29>(ref <Start>d__);
	}

	// Token: 0x06004219 RID: 16921 RVA: 0x0015DA3F File Offset: 0x0015BC3F
	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	// Token: 0x0600421A RID: 16922 RVA: 0x0015DA4C File Offset: 0x0015BC4C
	public static Task BeginAgeGate()
	{
		KIDAgeGate.<BeginAgeGate>d__31 <BeginAgeGate>d__;
		<BeginAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<BeginAgeGate>d__.<>1__state = -1;
		<BeginAgeGate>d__.<>t__builder.Start<KIDAgeGate.<BeginAgeGate>d__31>(ref <BeginAgeGate>d__);
		return <BeginAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600421B RID: 16923 RVA: 0x0015DA88 File Offset: 0x0015BC88
	private Task StartAgeGate()
	{
		KIDAgeGate.<StartAgeGate>d__32 <StartAgeGate>d__;
		<StartAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeGate>d__.<>4__this = this;
		<StartAgeGate>d__.<>1__state = -1;
		<StartAgeGate>d__.<>t__builder.Start<KIDAgeGate.<StartAgeGate>d__32>(ref <StartAgeGate>d__);
		return <StartAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600421C RID: 16924 RVA: 0x0015DACC File Offset: 0x0015BCCC
	private Task InitialiseAgeGate()
	{
		KIDAgeGate.<InitialiseAgeGate>d__33 <InitialiseAgeGate>d__;
		<InitialiseAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitialiseAgeGate>d__.<>4__this = this;
		<InitialiseAgeGate>d__.<>1__state = -1;
		<InitialiseAgeGate>d__.<>t__builder.Start<KIDAgeGate.<InitialiseAgeGate>d__33>(ref <InitialiseAgeGate>d__);
		return <InitialiseAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600421D RID: 16925 RVA: 0x0015DB10 File Offset: 0x0015BD10
	private Task ProcessAgeGate()
	{
		KIDAgeGate.<ProcessAgeGate>d__34 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ProcessAgeGate>d__.<>4__this = this;
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGate>d__34>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x0015DB54 File Offset: 0x0015BD54
	private Task<bool> ProcessAgeGateConfirmation()
	{
		KIDAgeGate.<ProcessAgeGateConfirmation>d__35 <ProcessAgeGateConfirmation>d__;
		<ProcessAgeGateConfirmation>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ProcessAgeGateConfirmation>d__.<>4__this = this;
		<ProcessAgeGateConfirmation>d__.<>1__state = -1;
		<ProcessAgeGateConfirmation>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGateConfirmation>d__35>(ref <ProcessAgeGateConfirmation>d__);
		return <ProcessAgeGateConfirmation>d__.<>t__builder.Task;
	}

	// Token: 0x0600421F RID: 16927 RVA: 0x0015DB98 File Offset: 0x0015BD98
	private Task WaitForAgeChoice()
	{
		KIDAgeGate.<WaitForAgeChoice>d__36 <WaitForAgeChoice>d__;
		<WaitForAgeChoice>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAgeChoice>d__.<>4__this = this;
		<WaitForAgeChoice>d__.<>1__state = -1;
		<WaitForAgeChoice>d__.<>t__builder.Start<KIDAgeGate.<WaitForAgeChoice>d__36>(ref <WaitForAgeChoice>d__);
		return <WaitForAgeChoice>d__.<>t__builder.Task;
	}

	// Token: 0x06004220 RID: 16928 RVA: 0x0015DBDB File Offset: 0x0015BDDB
	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	// Token: 0x06004221 RID: 16929 RVA: 0x0015DBE3 File Offset: 0x0015BDE3
	private void OnAgeGateCompleted()
	{
		this.FinaliseAgeGateAndContinue();
	}

	// Token: 0x06004222 RID: 16930 RVA: 0x0015DBEB File Offset: 0x0015BDEB
	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06004223 RID: 16931 RVA: 0x0015DC10 File Offset: 0x0015BE10
	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	// Token: 0x06004224 RID: 16932 RVA: 0x0015DC24 File Offset: 0x0015BE24
	private void AppealAge()
	{
		KIDAgeGate.<AppealAge>d__41 <AppealAge>d__;
		<AppealAge>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AppealAge>d__.<>4__this = this;
		<AppealAge>d__.<>1__state = -1;
		<AppealAge>d__.<>t__builder.Start<KIDAgeGate.<AppealAge>d__41>(ref <AppealAge>d__);
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x0015DC5C File Offset: 0x0015BE5C
	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string messageTitle = "UNDER AGE";
		string messageBody = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string messageConfirmation = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(messageTitle, messageBody, messageConfirmation, new Action(this.AppealAge), 0.25f, 0f);
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x00002789 File Offset: 0x00000989
	private void RefreshChallengeStatus()
	{
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x0015DCA9 File Offset: 0x0015BEA9
	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x0015DCB4 File Offset: 0x0015BEB4
	public void OnWhyAgeGateButtonPressed()
	{
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_screen_shown";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_gate",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen", "why_age_gate");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x0015DD57 File Offset: 0x0015BF57
	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x0015DD84 File Offset: 0x0015BF84
	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_screen_shown";
		telemetryData.CustomTags = new string[]
		{
			"kid_age_gate",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen", "learn_more_url");
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		Application.OpenURL("https://whyagegate.com/");
	}

	// Token: 0x04005325 RID: 21285
	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	// Token: 0x04005326 RID: 21286
	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	// Token: 0x04005327 RID: 21287
	private const int MINIMUM_PLATFORM_AGE = 13;

	// Token: 0x04005328 RID: 21288
	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	// Token: 0x04005329 RID: 21289
	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	// Token: 0x0400532A RID: 21290
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x0400532B RID: 21291
	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	// Token: 0x0400532C RID: 21292
	[SerializeField]
	private GameObject _confirmationUI;

	// Token: 0x0400532D RID: 21293
	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	// Token: 0x0400532E RID: 21294
	[SerializeField]
	private TMP_Text _confirmationAgeText;

	// Token: 0x0400532F RID: 21295
	[SerializeField]
	private GameObject _whyAgeGateScreen;

	// Token: 0x04005330 RID: 21296
	private const string strBlockAccessTitle = "UNDER AGE";

	// Token: 0x04005331 RID: 21297
	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	// Token: 0x04005332 RID: 21298
	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	// Token: 0x04005333 RID: 21299
	private const string strVerifyAgeTitle = "VERIFY AGE";

	// Token: 0x04005334 RID: 21300
	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	// Token: 0x04005335 RID: 21301
	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	// Token: 0x04005336 RID: 21302
	private static KIDAgeGate _activeReference;

	// Token: 0x04005337 RID: 21303
	private static GetRequirementsData _ageGateConfig;

	// Token: 0x04005338 RID: 21304
	private static int _ageValue;

	// Token: 0x04005339 RID: 21305
	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	// Token: 0x0400533A RID: 21306
	private static bool _hasChosenAge;

	// Token: 0x0400533C RID: 21308
	private bool _metrics_LearnMorePressed;
}
