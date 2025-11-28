using System;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

// Token: 0x02000AA4 RID: 2724
public class KIDUI_ConfirmScreen : MonoBehaviour
{
	// Token: 0x06004458 RID: 17496 RVA: 0x00169D58 File Offset: 0x00167F58
	private void Awake()
	{
		if (this._emailToConfirmTxt == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email To Confirm Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._setupScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	// Token: 0x06004459 RID: 17497 RVA: 0x00169DCA File Offset: 0x00167FCA
	private void OnEnable()
	{
		this._confirmButton.interactable = true;
		this._backButton.interactable = true;
	}

	// Token: 0x0600445A RID: 17498 RVA: 0x00169DE4 File Offset: 0x00167FE4
	public void OnEmailSubmitted(string emailAddress)
	{
		this._submittedEmailAddress = emailAddress;
		this._emailToConfirmTxt.text = this._submittedEmailAddress;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600445B RID: 17499 RVA: 0x00169E0C File Offset: 0x0016800C
	public void OnConfirmPressed()
	{
		KIDUI_ConfirmScreen.<OnConfirmPressed>d__16 <OnConfirmPressed>d__;
		<OnConfirmPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnConfirmPressed>d__.<>4__this = this;
		<OnConfirmPressed>d__.<>1__state = -1;
		<OnConfirmPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnConfirmPressed>d__16>(ref <OnConfirmPressed>d__);
	}

	// Token: 0x0600445C RID: 17500 RVA: 0x00169E44 File Offset: 0x00168044
	public void OnBackPressed()
	{
		KIDUI_ConfirmScreen.<OnBackPressed>d__17 <OnBackPressed>d__;
		<OnBackPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnBackPressed>d__.<>4__this = this;
		<OnBackPressed>d__.<>1__state = -1;
		<OnBackPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnBackPressed>d__17>(ref <OnBackPressed>d__);
	}

	// Token: 0x0600445D RID: 17501 RVA: 0x00169E7B File Offset: 0x0016807B
	public void NotifyOfResult(bool success)
	{
		this._hasCompletedSendEmailRequest = true;
		this._emailRequestResult = success;
	}

	// Token: 0x0600445E RID: 17502 RVA: 0x00169E8C File Offset: 0x0016808C
	private void ShowErrorScreen(string errorMessage)
	{
		KIDUI_ConfirmScreen.<ShowErrorScreen>d__19 <ShowErrorScreen>d__;
		<ShowErrorScreen>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ShowErrorScreen>d__.<>4__this = this;
		<ShowErrorScreen>d__.errorMessage = errorMessage;
		<ShowErrorScreen>d__.<>1__state = -1;
		<ShowErrorScreen>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<ShowErrorScreen>d__19>(ref <ShowErrorScreen>d__);
	}

	// Token: 0x0600445F RID: 17503 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x040055EE RID: 21998
	[SerializeField]
	private TMP_Text _emailToConfirmTxt;

	// Token: 0x040055EF RID: 21999
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x040055F0 RID: 22000
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;

	// Token: 0x040055F1 RID: 22001
	[SerializeField]
	private KIDUI_ErrorScreen _errorScreen;

	// Token: 0x040055F2 RID: 22002
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x040055F3 RID: 22003
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x040055F4 RID: 22004
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x040055F5 RID: 22005
	[SerializeField]
	private KIDUIButton _backButton;

	// Token: 0x040055F6 RID: 22006
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x040055F7 RID: 22007
	private string _submittedEmailAddress;

	// Token: 0x040055F8 RID: 22008
	private CancellationTokenSource _cancellationTokenSource;

	// Token: 0x040055F9 RID: 22009
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x040055FA RID: 22010
	private bool _emailRequestResult;
}
