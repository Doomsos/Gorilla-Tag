using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000ABF RID: 2751
public class WarningScreens : MonoBehaviour
{
	// Token: 0x060044E3 RID: 17635 RVA: 0x0016CF09 File Offset: 0x0016B109
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	// Token: 0x060044E4 RID: 17636 RVA: 0x0016CF30 File Offset: 0x0016B130
	private Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreenInternal>d__14 <StartWarningScreenInternal>d__;
		<StartWarningScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreenInternal>d__.<>4__this = this;
		<StartWarningScreenInternal>d__.cancellationToken = cancellationToken;
		<StartWarningScreenInternal>d__.<>1__state = -1;
		<StartWarningScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartWarningScreenInternal>d__14>(ref <StartWarningScreenInternal>d__);
		return <StartWarningScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x060044E5 RID: 17637 RVA: 0x0016CF7C File Offset: 0x0016B17C
	private Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreenInternal>d__15 <StartOptInFollowUpScreenInternal>d__;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreenInternal>d__.<>4__this = this;
		<StartOptInFollowUpScreenInternal>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreenInternal>d__.<>1__state = -1;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreenInternal>d__15>(ref <StartOptInFollowUpScreenInternal>d__);
		return <StartOptInFollowUpScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x060044E6 RID: 17638 RVA: 0x0016CFC8 File Offset: 0x0016B1C8
	public static Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreen>d__16 <StartWarningScreen>d__;
		<StartWarningScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreen>d__.cancellationToken = cancellationToken;
		<StartWarningScreen>d__.<>1__state = -1;
		<StartWarningScreen>d__.<>t__builder.Start<WarningScreens.<StartWarningScreen>d__16>(ref <StartWarningScreen>d__);
		return <StartWarningScreen>d__.<>t__builder.Task;
	}

	// Token: 0x060044E7 RID: 17639 RVA: 0x0016D00C File Offset: 0x0016B20C
	public static Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreen>d__17 <StartOptInFollowUpScreen>d__;
		<StartOptInFollowUpScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreen>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreen>d__.<>1__state = -1;
		<StartOptInFollowUpScreen>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreen>d__17>(ref <StartOptInFollowUpScreen>d__);
		return <StartOptInFollowUpScreen>d__.<>t__builder.Task;
	}

	// Token: 0x060044E8 RID: 17640 RVA: 0x0016D050 File Offset: 0x0016B250
	private static Task WaitForResponse(CancellationToken cancellationToken)
	{
		WarningScreens.<WaitForResponse>d__18 <WaitForResponse>d__;
		<WaitForResponse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForResponse>d__.cancellationToken = cancellationToken;
		<WaitForResponse>d__.<>1__state = -1;
		<WaitForResponse>d__.<>t__builder.Start<WarningScreens.<WaitForResponse>d__18>(ref <WaitForResponse>d__);
		return <WaitForResponse>d__.<>t__builder.Task;
	}

	// Token: 0x060044E9 RID: 17641 RVA: 0x0016470C File Offset: 0x0016290C
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x060044EA RID: 17642 RVA: 0x0016D093 File Offset: 0x0016B293
	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction.Invoke();
	}

	// Token: 0x060044EB RID: 17643 RVA: 0x0016D0BE File Offset: 0x0016B2BE
	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction.Invoke();
	}

	// Token: 0x04005698 RID: 22168
	private static WarningScreens _activeReference;

	// Token: 0x04005699 RID: 22169
	[SerializeField]
	private MessageBox _messageBox;

	// Token: 0x0400569A RID: 22170
	[SerializeField]
	private GameObject _imageContainerAfter;

	// Token: 0x0400569B RID: 22171
	[SerializeField]
	private GameObject _imageContainerBefore;

	// Token: 0x0400569C RID: 22172
	[SerializeField]
	private TMP_Text _withImageTextBefore;

	// Token: 0x0400569D RID: 22173
	[SerializeField]
	private TMP_Text _withImageTextAfter;

	// Token: 0x0400569E RID: 22174
	[SerializeField]
	private TMP_Text _noImageText;

	// Token: 0x0400569F RID: 22175
	private Action _onLeftButtonPressedAction;

	// Token: 0x040056A0 RID: 22176
	private Action _onRightButtonPressedAction;

	// Token: 0x040056A1 RID: 22177
	private static WarningButtonResult _result;

	// Token: 0x040056A2 RID: 22178
	private static WarningButtonResult _leftButtonResult;

	// Token: 0x040056A3 RID: 22179
	private static WarningButtonResult _rightButtonResult;

	// Token: 0x040056A4 RID: 22180
	private static bool _closedMessageBox;
}
