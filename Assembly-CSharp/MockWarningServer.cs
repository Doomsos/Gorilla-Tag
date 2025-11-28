using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A6E RID: 2670
internal class MockWarningServer : WarningsServer
{
	// Token: 0x1700065F RID: 1631
	// (get) Token: 0x06004332 RID: 17202 RVA: 0x0016473C File Offset: 0x0016293C
	public static string ShownScreenPlayerPref
	{
		get
		{
			return "screen-shown-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004333 RID: 17203 RVA: 0x00164754 File Offset: 0x00162954
	private void Awake()
	{
		if (WarningsServer.Instance == null)
		{
			WarningsServer.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004334 RID: 17204 RVA: 0x00164774 File Offset: 0x00162974
	private PlayerAgeGateWarningStatus CreateWarningStatus(string header, string body, MockWarningServer.ButtonSetup? leftButtonSetup, MockWarningServer.ButtonSetup? rightButtonSetup, EImageVisibility showImage, Action leftButtonCallback, Action rightButtonCallback)
	{
		PlayerAgeGateWarningStatus result;
		result.header = header;
		result.body = body;
		result.leftButtonText = string.Empty;
		result.rightButtonText = string.Empty;
		result.leftButtonResult = WarningButtonResult.None;
		result.rightButtonResult = WarningButtonResult.None;
		result.noWarningResult = WarningButtonResult.None;
		result.showImage = showImage;
		result.onLeftButtonPressedAction = leftButtonCallback;
		result.onRightButtonPressedAction = rightButtonCallback;
		if (leftButtonSetup != null)
		{
			result.leftButtonText = leftButtonSetup.Value.buttonText;
			result.leftButtonResult = leftButtonSetup.Value.buttonResult;
		}
		if (rightButtonSetup != null)
		{
			result.rightButtonText = rightButtonSetup.Value.buttonText;
			result.rightButtonResult = rightButtonSetup.Value.buttonResult;
		}
		return result;
	}

	// Token: 0x06004335 RID: 17205 RVA: 0x0016483C File Offset: 0x00162A3C
	public override Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token)
	{
		MockWarningServer.<FetchPlayerData>d__12 <FetchPlayerData>d__;
		<FetchPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<FetchPlayerData>d__.<>4__this = this;
		<FetchPlayerData>d__.token = token;
		<FetchPlayerData>d__.<>1__state = -1;
		<FetchPlayerData>d__.<>t__builder.Start<MockWarningServer.<FetchPlayerData>d__12>(ref <FetchPlayerData>d__);
		return <FetchPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06004336 RID: 17206 RVA: 0x00164888 File Offset: 0x00162A88
	public override Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token)
	{
		MockWarningServer.<GetOptInFollowUpMessage>d__13 <GetOptInFollowUpMessage>d__;
		<GetOptInFollowUpMessage>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<GetOptInFollowUpMessage>d__.<>4__this = this;
		<GetOptInFollowUpMessage>d__.token = token;
		<GetOptInFollowUpMessage>d__.<>1__state = -1;
		<GetOptInFollowUpMessage>d__.<>t__builder.Start<MockWarningServer.<GetOptInFollowUpMessage>d__13>(ref <GetOptInFollowUpMessage>d__);
		return <GetOptInFollowUpMessage>d__.<>t__builder.Task;
	}

	// Token: 0x06004337 RID: 17207 RVA: 0x001648D3 File Offset: 0x00162AD3
	private bool ShouldShowWarningScreen(int phase, bool inOptInCohort)
	{
		if (PlayerPrefs.GetInt(string.Format("phase-{0}-{1}", phase, MockWarningServer.ShownScreenPlayerPref), 0) == 0)
		{
			return true;
		}
		switch (phase)
		{
		default:
			return false;
		case 2:
			return inOptInCohort;
		case 3:
		case 4:
			return true;
		}
	}

	// Token: 0x040054B0 RID: 21680
	private const string SHOWN_SCREEN_PREFIX = "screen-shown-";

	// Token: 0x040054B1 RID: 21681
	private const string KID_WARNING_TITLE_KEY = "KID_WARNING_TITLE";

	// Token: 0x040054B2 RID: 21682
	private const string KID_WARNING_CONTINUE_KEY = "KID_WARNING_CONTINUE";

	// Token: 0x040054B3 RID: 21683
	private const string KID_WARNING_PHASE_THREE_IN_COHORT_KEY = "KID_WARNING_PHASE_THREE_IN_COHORT";

	// Token: 0x040054B4 RID: 21684
	private const string KID_WARNING_PHASE_FOUR_RETURNING_PLAYER_KEY = "KID_WARNING_PHASE_FOUR_RETURNING_PLAYER";

	// Token: 0x040054B5 RID: 21685
	private const string KID_WARNING_OPT_IN_FOLLOW_MESSAGE_KEY = "KID_WARNING_OPT_IN_FOLLOW_MESSAGE";

	// Token: 0x040054B6 RID: 21686
	private const string KID_WARNING_FOLLOW_UP_YAY_KEY = "KID_WARNING_FOLLOW_UP_YAY";

	// Token: 0x02000A6F RID: 2671
	public struct ButtonSetup
	{
		// Token: 0x06004339 RID: 17209 RVA: 0x00164919 File Offset: 0x00162B19
		public ButtonSetup(string txt, WarningButtonResult result)
		{
			this.buttonText = txt;
			this.buttonResult = result;
		}

		// Token: 0x040054B7 RID: 21687
		public string buttonText;

		// Token: 0x040054B8 RID: 21688
		public WarningButtonResult buttonResult;
	}
}
