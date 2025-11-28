using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000AA8 RID: 2728
public class KIDUI_Controller : MonoBehaviour
{
	// Token: 0x17000670 RID: 1648
	// (get) Token: 0x06004467 RID: 17511 RVA: 0x0016A4C6 File Offset: 0x001686C6
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	// Token: 0x17000671 RID: 1649
	// (get) Token: 0x06004468 RID: 17512 RVA: 0x0016A4CD File Offset: 0x001686CD
	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	// Token: 0x17000672 RID: 1650
	// (get) Token: 0x06004469 RID: 17513 RVA: 0x0016A4E8 File Offset: 0x001686E8
	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	// Token: 0x0600446A RID: 17514 RVA: 0x0016A516 File Offset: 0x00168716
	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x0600446B RID: 17515 RVA: 0x0016A52D File Offset: 0x0016872D
	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x0600446C RID: 17516 RVA: 0x0016A550 File Offset: 0x00168750
	public Task StartKIDScreens(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<StartKIDScreens>d__20 <StartKIDScreens>d__;
		<StartKIDScreens>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDScreens>d__.<>4__this = this;
		<StartKIDScreens>d__.cancellationToken = cancellationToken;
		<StartKIDScreens>d__.<>1__state = -1;
		<StartKIDScreens>d__.<>t__builder.Start<KIDUI_Controller.<StartKIDScreens>d__20>(ref <StartKIDScreens>d__);
		return <StartKIDScreens>d__.<>t__builder.Task;
	}

	// Token: 0x0600446D RID: 17517 RVA: 0x0016A59C File Offset: 0x0016879C
	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x0600446E RID: 17518 RVA: 0x0016A614 File Offset: 0x00168814
	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	// Token: 0x0600446F RID: 17519 RVA: 0x0016A63C File Offset: 0x0016883C
	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	// Token: 0x06004470 RID: 17520 RVA: 0x0016A68C File Offset: 0x0016888C
	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus result;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				result = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				result = EMainScreenStatus.Declined;
			}
			else
			{
				result = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			result = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				result = EMainScreenStatus.Setup;
			}
			else
			{
				result = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			result = EMainScreenStatus.None;
			break;
		}
		return result;
	}

	// Token: 0x06004471 RID: 17521 RVA: 0x0016A718 File Offset: 0x00168918
	private Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<ShouldShowKIDScreen>d__25 <ShouldShowKIDScreen>d__;
		<ShouldShowKIDScreen>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ShouldShowKIDScreen>d__.<>4__this = this;
		<ShouldShowKIDScreen>d__.cancellationToken = cancellationToken;
		<ShouldShowKIDScreen>d__.<>1__state = -1;
		<ShouldShowKIDScreen>d__.<>t__builder.Start<KIDUI_Controller.<ShouldShowKIDScreen>d__25>(ref <ShouldShowKIDScreen>d__);
		return <ShouldShowKIDScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06004472 RID: 17522 RVA: 0x0016A763 File Offset: 0x00168963
	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	// Token: 0x06004473 RID: 17523 RVA: 0x0016A796 File Offset: 0x00168996
	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	// Token: 0x06004474 RID: 17524 RVA: 0x0016A7A7 File Offset: 0x001689A7
	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	// Token: 0x06004475 RID: 17525 RVA: 0x0016470C File Offset: 0x0016290C
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004476 RID: 17526 RVA: 0x0016A7D4 File Offset: 0x001689D4
	public KIDUI_Controller()
	{
		List<EKIDFeatures> list = new List<EKIDFeatures>();
		list.Add(EKIDFeatures.Multiplayer);
		list.Add(EKIDFeatures.Mods);
		this._inaccessibleSettings = list;
		base..ctor();
	}

	// Token: 0x0400560B RID: 22027
	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	// Token: 0x0400560C RID: 22028
	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	// Token: 0x0400560D RID: 22029
	private static KIDUI_Controller _instance;

	// Token: 0x0400560E RID: 22030
	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	// Token: 0x0400560F RID: 22031
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04005610 RID: 22032
	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	// Token: 0x04005611 RID: 22033
	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings;

	// Token: 0x04005612 RID: 22034
	private KIDUI_Controller.Metrics_ShowReason _showReason;

	// Token: 0x04005613 RID: 22035
	private bool _isKidUIActive;

	// Token: 0x04005614 RID: 22036
	private static string etagOnCloseBlackScreenPlayerPrefStr;

	// Token: 0x04005615 RID: 22037
	private string _lastEtagOnClose;

	// Token: 0x02000AA9 RID: 2729
	public enum Metrics_ShowReason
	{
		// Token: 0x04005617 RID: 22039
		None,
		// Token: 0x04005618 RID: 22040
		Inaccessible,
		// Token: 0x04005619 RID: 22041
		Guardian_Disabled,
		// Token: 0x0400561A RID: 22042
		Permissions_Changed,
		// Token: 0x0400561B RID: 22043
		Default_Session,
		// Token: 0x0400561C RID: 22044
		No_Session
	}
}
