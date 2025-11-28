using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000A7F RID: 2687
public class KIDMessagingController : MonoBehaviour
{
	// Token: 0x17000662 RID: 1634
	// (get) Token: 0x06004385 RID: 17285 RVA: 0x00166493 File Offset: 0x00164693
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x001664AB File Offset: 0x001646AB
	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	// Token: 0x06004387 RID: 17287 RVA: 0x001664B4 File Offset: 0x001646B4
	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	// Token: 0x06004388 RID: 17288 RVA: 0x001664DA File Offset: 0x001646DA
	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault;
	}

	// Token: 0x06004389 RID: 17289 RVA: 0x001664EC File Offset: 0x001646EC
	private Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18 <StartKIDConfirmationScreenInternal>d__;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreenInternal>d__.<>4__this = this;
		<StartKIDConfirmationScreenInternal>d__.token = token;
		<StartKIDConfirmationScreenInternal>d__.<>1__state = -1;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18>(ref <StartKIDConfirmationScreenInternal>d__);
		return <StartKIDConfirmationScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager kidaudioManager = KIDAudioManager.Instance;
		if (kidaudioManager == null)
		{
			return;
		}
		kidaudioManager.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x00166538 File Offset: 0x00164738
	public static Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreen>d__20 <StartKIDConfirmationScreen>d__;
		<StartKIDConfirmationScreen>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreen>d__.token = token;
		<StartKIDConfirmationScreen>d__.<>1__state = -1;
		<StartKIDConfirmationScreen>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreen>d__20>(ref <StartKIDConfirmationScreen>d__);
		return <StartKIDConfirmationScreen>d__.<>t__builder.Task;
	}

	// Token: 0x0600438C RID: 17292 RVA: 0x0016657C File Offset: 0x0016477C
	private static Task<string> GetSetupConfirmationMessage()
	{
		KIDMessagingController.<GetSetupConfirmationMessage>d__21 <GetSetupConfirmationMessage>d__;
		<GetSetupConfirmationMessage>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetSetupConfirmationMessage>d__.<>1__state = -1;
		<GetSetupConfirmationMessage>d__.<>t__builder.Start<KIDMessagingController.<GetSetupConfirmationMessage>d__21>(ref <GetSetupConfirmationMessage>d__);
		return <GetSetupConfirmationMessage>d__.<>t__builder.Task;
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x001665B8 File Offset: 0x001647B8
	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x0016661C File Offset: 0x0016481C
	public static void ShowConnectionErrorScreen()
	{
		if (KIDMessagingController.instance == null || KIDMessagingController.instance.messageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] No message box");
			return;
		}
		KIDMessagingController.instance._closeMessageBox = false;
		KIDMessagingController.instance.messageBox.Header = "Connection Error";
		KIDMessagingController.instance.messageBox.Body = "Unable to connect to the internet. Please restart the game and try again.";
		KIDMessagingController.instance.messageBox.RightButton = "Quit";
		KIDMessagingController.instance.messageBox.ShowQuitButtonAsPrimary();
		KIDMessagingController.instance.messageBox.RightButtonCallback.RemoveAllListeners();
		KIDMessagingController.instance.messageBox.RightButtonCallback.AddListener(new UnityAction(Application.Quit));
		KIDMessagingController.instance.messageBox.gameObject.SetActive(true);
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(KIDMessagingController.instance.transform);
	}

	// Token: 0x04005506 RID: 21766
	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	// Token: 0x04005507 RID: 21767
	private const string CONFIRMATION_HEADER = "Thank you";

	// Token: 0x04005508 RID: 21768
	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	// Token: 0x04005509 RID: 21769
	private const string CONFIRMATION_BUTTON = "Continue";

	// Token: 0x0400550A RID: 21770
	private const string KID_SETUP_CONFIRMATION_TITLE_KEY = "KID_SETUP_CONFIRMATION_TITLE";

	// Token: 0x0400550B RID: 21771
	private const string KID_SETUP_CONFIRMATION_BODY_KEY = "KID_SETUP_CONFIRMATION_BODY";

	// Token: 0x0400550C RID: 21772
	private const string KID_SETUP_CONFIRMATION_BUTTON_KEY = "KID_SETUP_CONFIRMATION_BUTTON";

	// Token: 0x0400550D RID: 21773
	private static KIDMessagingController instance;

	// Token: 0x0400550E RID: 21774
	[SerializeField]
	private MessageBox messageBox;

	// Token: 0x0400550F RID: 21775
	private const string CONNECTION_ERROR_HEADER = "Connection Error";

	// Token: 0x04005510 RID: 21776
	private const string CONNECTION_ERROR_BODY = "Unable to connect to the internet. Please restart the game and try again.";

	// Token: 0x04005511 RID: 21777
	private const string CONNECTION_ERROR_BUTTON = "Quit";

	// Token: 0x04005512 RID: 21778
	private bool _closeMessageBox;
}
