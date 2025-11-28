using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000A40 RID: 2624
public class KIDManager : MonoBehaviour
{
	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x0600425B RID: 16987 RVA: 0x0015ED33 File Offset: 0x0015CF33
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x0600425C RID: 16988 RVA: 0x0015ED3A File Offset: 0x0015CF3A
	// (set) Token: 0x0600425D RID: 16989 RVA: 0x0015ED41 File Offset: 0x0015CF41
	public static bool InitialisationComplete { get; private set; } = false;

	// Token: 0x17000644 RID: 1604
	// (get) Token: 0x0600425E RID: 16990 RVA: 0x0015ED49 File Offset: 0x0015CF49
	// (set) Token: 0x0600425F RID: 16991 RVA: 0x0015ED50 File Offset: 0x0015CF50
	public static bool InitialisationSuccessful { get; private set; } = false;

	// Token: 0x17000645 RID: 1605
	// (get) Token: 0x06004260 RID: 16992 RVA: 0x0015ED58 File Offset: 0x0015CF58
	// (set) Token: 0x06004261 RID: 16993 RVA: 0x0015ED5F File Offset: 0x0015CF5F
	public static TMPSession CurrentSession { get; private set; }

	// Token: 0x17000646 RID: 1606
	// (get) Token: 0x06004262 RID: 16994 RVA: 0x0015ED67 File Offset: 0x0015CF67
	// (set) Token: 0x06004263 RID: 16995 RVA: 0x0015ED6E File Offset: 0x0015CF6E
	public static SessionStatus PreviousStatus { get; private set; }

	// Token: 0x17000647 RID: 1607
	// (get) Token: 0x06004264 RID: 16996 RVA: 0x0015ED76 File Offset: 0x0015CF76
	// (set) Token: 0x06004265 RID: 16997 RVA: 0x0015ED7D File Offset: 0x0015CF7D
	public static GetRequirementsData _ageGateRequirements { get; private set; }

	// Token: 0x17000648 RID: 1608
	// (get) Token: 0x06004266 RID: 16998 RVA: 0x0015ED85 File Offset: 0x0015CF85
	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	// Token: 0x17000649 RID: 1609
	// (get) Token: 0x06004267 RID: 16999 RVA: 0x0015ED8C File Offset: 0x0015CF8C
	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	// Token: 0x1700064A RID: 1610
	// (get) Token: 0x06004268 RID: 17000 RVA: 0x0015ED9C File Offset: 0x0015CF9C
	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x06004269 RID: 17001 RVA: 0x0015EDAC File Offset: 0x0015CFAC
	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x0600426A RID: 17002 RVA: 0x0015EDCB File Offset: 0x0015CFCB
	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x1700064D RID: 1613
	// (get) Token: 0x0600426B RID: 17003 RVA: 0x0015EDE3 File Offset: 0x0015CFE3
	// (set) Token: 0x0600426C RID: 17004 RVA: 0x0015EDEA File Offset: 0x0015CFEA
	public static bool HasOptedInToKID { get; private set; }

	// Token: 0x1700064E RID: 1614
	// (get) Token: 0x0600426D RID: 17005 RVA: 0x0015EDF2 File Offset: 0x0015CFF2
	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	// Token: 0x1700064F RID: 1615
	// (get) Token: 0x0600426E RID: 17006 RVA: 0x0015EDF9 File Offset: 0x0015CFF9
	// (set) Token: 0x0600426F RID: 17007 RVA: 0x0015EE00 File Offset: 0x0015D000
	public static string DbgLocale { get; set; }

	// Token: 0x17000650 RID: 1616
	// (get) Token: 0x06004270 RID: 17008 RVA: 0x0015EE08 File Offset: 0x0015D008
	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	// Token: 0x17000651 RID: 1617
	// (get) Token: 0x06004271 RID: 17009 RVA: 0x0015EE0F File Offset: 0x0015D00F
	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	// Token: 0x17000652 RID: 1618
	// (get) Token: 0x06004272 RID: 17010 RVA: 0x0015EE3D File Offset: 0x0015D03D
	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004273 RID: 17011 RVA: 0x0015EE58 File Offset: 0x0015D058
	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		Debug.Log("[KID] INIT");
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	// Token: 0x06004274 RID: 17012 RVA: 0x0015EEC4 File Offset: 0x0015D0C4
	private void Start()
	{
		KIDManager.<Start>d__70 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDManager.<Start>d__70>(ref <Start>d__);
	}

	// Token: 0x06004275 RID: 17013 RVA: 0x0015EEF3 File Offset: 0x0015D0F3
	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06004276 RID: 17014 RVA: 0x0015EF00 File Offset: 0x0015D100
	public static string GetActiveAccountStatusNiceString()
	{
		switch (KIDManager.GetActiveAccountStatus())
		{
		case 1:
			return "Digital Minor";
		case 2:
			return "Digital Youth";
		case 3:
			return "Legal Adult";
		default:
			return "UNKNOWN";
		}
	}

	// Token: 0x06004277 RID: 17015 RVA: 0x0015EF40 File Offset: 0x0015D140
	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return 3;
		}
		return 1;
	}

	// Token: 0x06004278 RID: 17016 RVA: 0x0015EF65 File Offset: 0x0015D165
	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	// Token: 0x06004279 RID: 17017 RVA: 0x0015EF88 File Offset: 0x0015D188
	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = 1;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = 1;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = 2;
			return true;
		}
		ageType = 3;
		return true;
	}

	// Token: 0x0600427A RID: 17018 RVA: 0x0015EFE0 File Offset: 0x0015D1E0
	[return: TupleElementNames(new string[]
	{
		"requiresOptIn",
		"hasOptedInPreviously"
	})]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == 3)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		bool flag = true;
		if (KIDManager.CurrentSession != null)
		{
			flag = KIDManager.CurrentSession.HasOptedInToPermission(feature);
		}
		if (permissionData.ManagedBy == 2)
		{
			return new ValueTuple<bool, bool>(false, flag);
		}
		if (permissionData.ManagedBy == 1 && permissionData.Enabled)
		{
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, flag);
	}

	// Token: 0x0600427B RID: 17019 RVA: 0x0015F074 File Offset: 0x0015D274
	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] CurrentSession is null, cannot set feature opt-in. Returning.");
			return;
		}
		switch (permissionDataByFeature.ManagedBy)
		{
		case 1:
			KIDManager.CurrentSession.OptInToPermission(feature, optedIn);
			return;
		case 2:
			KIDManager.CurrentSession.OptInToPermission(feature, permissionDataByFeature.Enabled);
			return;
		case 3:
			KIDManager.CurrentSession.OptInToPermission(feature, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600427C RID: 17020 RVA: 0x0015F104 File Offset: 0x0015D304
	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == 3)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != 2 || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	// Token: 0x0600427D RID: 17021 RVA: 0x0015F1C0 File Offset: 0x0015D3C0
	private static Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		KIDManager.<TryGetPlayerData>d__81 <TryGetPlayerData>d__;
		<TryGetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<TryGetPlayerData>d__.forceRefresh = forceRefresh;
		<TryGetPlayerData>d__.<>1__state = -1;
		<TryGetPlayerData>d__.<>t__builder.Start<KIDManager.<TryGetPlayerData>d__81>(ref <TryGetPlayerData>d__);
		return <TryGetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x0600427E RID: 17022 RVA: 0x0015F204 File Offset: 0x0015D404
	private static Task<GetRequirementsData> TryGetRequirements()
	{
		KIDManager.<TryGetRequirements>d__82 <TryGetRequirements>d__;
		<TryGetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<TryGetRequirements>d__.<>1__state = -1;
		<TryGetRequirements>d__.<>t__builder.Start<KIDManager.<TryGetRequirements>d__82>(ref <TryGetRequirements>d__);
		return <TryGetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x0600427F RID: 17023 RVA: 0x0015F240 File Offset: 0x0015D440
	private static Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		KIDManager.<TryVerifyAgeResponse>d__83 <TryVerifyAgeResponse>d__;
		<TryVerifyAgeResponse>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<TryVerifyAgeResponse>d__.<>1__state = -1;
		<TryVerifyAgeResponse>d__.<>t__builder.Start<KIDManager.<TryVerifyAgeResponse>d__83>(ref <TryVerifyAgeResponse>d__);
		return <TryVerifyAgeResponse>d__.<>t__builder.Task;
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x0015F27C File Offset: 0x0015D47C
	[return: TupleElementNames(new string[]
	{
		"success",
		"exception"
	})]
	private static Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest()
	{
		KIDManager.<TrySendChallengeEmailRequest>d__84 <TrySendChallengeEmailRequest>d__;
		<TrySendChallengeEmailRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendChallengeEmailRequest>d__.<>1__state = -1;
		<TrySendChallengeEmailRequest>d__.<>t__builder.Start<KIDManager.<TrySendChallengeEmailRequest>d__84>(ref <TrySendChallengeEmailRequest>d__);
		return <TrySendChallengeEmailRequest>d__.<>t__builder.Task;
	}

	// Token: 0x06004281 RID: 17025 RVA: 0x0015F2B8 File Offset: 0x0015D4B8
	private static Task<bool> TrySendOptInPermissions()
	{
		KIDManager.<TrySendOptInPermissions>d__85 <TrySendOptInPermissions>d__;
		<TrySendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySendOptInPermissions>d__.<>1__state = -1;
		<TrySendOptInPermissions>d__.<>t__builder.Start<KIDManager.<TrySendOptInPermissions>d__85>(ref <TrySendOptInPermissions>d__);
		return <TrySendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x06004282 RID: 17026 RVA: 0x0015F2F4 File Offset: 0x0015D4F4
	public static Task<ValueTuple<bool, string>> TrySendUpgradeSessionChallengeEmail()
	{
		KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86 <TrySendUpgradeSessionChallengeEmail>d__;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendUpgradeSessionChallengeEmail>d__.<>1__state = -1;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Start<KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86>(ref <TrySendUpgradeSessionChallengeEmail>d__);
		return <TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06004283 RID: 17027 RVA: 0x0015F330 File Offset: 0x0015D530
	public static Task<bool> TrySetHasConfirmedStatus()
	{
		KIDManager.<TrySetHasConfirmedStatus>d__87 <TrySetHasConfirmedStatus>d__;
		<TrySetHasConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySetHasConfirmedStatus>d__.<>1__state = -1;
		<TrySetHasConfirmedStatus>d__.<>t__builder.Start<KIDManager.<TrySetHasConfirmedStatus>d__87>(ref <TrySetHasConfirmedStatus>d__);
		return <TrySetHasConfirmedStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06004284 RID: 17028 RVA: 0x0015F36C File Offset: 0x0015D56C
	public static Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		KIDManager.<TryUpgradeSession>d__88 <TryUpgradeSession>d__;
		<TryUpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<TryUpgradeSession>d__.requestedPermissions = requestedPermissions;
		<TryUpgradeSession>d__.<>1__state = -1;
		<TryUpgradeSession>d__.<>t__builder.Start<KIDManager.<TryUpgradeSession>d__88>(ref <TryUpgradeSession>d__);
		return <TryUpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x06004285 RID: 17029 RVA: 0x0015F3B0 File Offset: 0x0015D5B0
	public static Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		KIDManager.<TryAttemptAgeUpdate>d__89 <TryAttemptAgeUpdate>d__;
		<TryAttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<TryAttemptAgeUpdate>d__.age = age;
		<TryAttemptAgeUpdate>d__.<>1__state = -1;
		<TryAttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<TryAttemptAgeUpdate>d__89>(ref <TryAttemptAgeUpdate>d__);
		return <TryAttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x06004286 RID: 17030 RVA: 0x0015F3F4 File Offset: 0x0015D5F4
	public static Task<bool> TryAppealAge(string email, int newAge)
	{
		KIDManager.<TryAppealAge>d__90 <TryAppealAge>d__;
		<TryAppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryAppealAge>d__.email = email;
		<TryAppealAge>d__.newAge = newAge;
		<TryAppealAge>d__.<>1__state = -1;
		<TryAppealAge>d__.<>t__builder.Start<KIDManager.<TryAppealAge>d__90>(ref <TryAppealAge>d__);
		return <TryAppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x06004287 RID: 17031 RVA: 0x0015F440 File Offset: 0x0015D640
	public static Task UpdateSession(Action<bool> getDataCompleted = null)
	{
		KIDManager.<UpdateSession>d__91 <UpdateSession>d__;
		<UpdateSession>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateSession>d__.getDataCompleted = getDataCompleted;
		<UpdateSession>d__.<>1__state = -1;
		<UpdateSession>d__.<>t__builder.Start<KIDManager.<UpdateSession>d__91>(ref <UpdateSession>d__);
		return <UpdateSession>d__.<>t__builder.Task;
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x0015F484 File Offset: 0x0015D684
	private static Task<bool> CheckWarningScreensOptedIn()
	{
		KIDManager.<CheckWarningScreensOptedIn>d__92 <CheckWarningScreensOptedIn>d__;
		<CheckWarningScreensOptedIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<CheckWarningScreensOptedIn>d__.<>1__state = -1;
		<CheckWarningScreensOptedIn>d__.<>t__builder.Start<KIDManager.<CheckWarningScreensOptedIn>d__92>(ref <CheckWarningScreensOptedIn>d__);
		return <CheckWarningScreensOptedIn>d__.<>t__builder.Task;
	}

	// Token: 0x06004289 RID: 17033 RVA: 0x0015F4BF File Offset: 0x0015D6BF
	[RuntimeInitializeOnLoadMethod(0)]
	public static void InitialiseBootFlow()
	{
		Debug.Log("[KID::MANAGER] PHASE ZERO -- START -- Checking K-ID Flag");
		if (PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0) != 0)
		{
			return;
		}
		Debug.Log("[KID::MANAGER] INITIALISE BOOT FLOW - Force Starting Overlay");
		PrivateUIRoom.ForceStartOverlay();
	}

	// Token: 0x0600428A RID: 17034 RVA: 0x0015F4E8 File Offset: 0x0015D6E8
	[RuntimeInitializeOnLoadMethod(2)]
	public static void InitialiseKID()
	{
		KIDManager.<InitialiseKID>d__94 <InitialiseKID>d__;
		<InitialiseKID>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<InitialiseKID>d__.<>1__state = -1;
		<InitialiseKID>d__.<>t__builder.Start<KIDManager.<InitialiseKID>d__94>(ref <InitialiseKID>d__);
	}

	// Token: 0x0600428B RID: 17035 RVA: 0x0015F518 File Offset: 0x0015D718
	private static bool UpdatePermissions(TMPSession newSession)
	{
		Debug.Log("[KID::MANAGER] Updating Permissions to reflect session.");
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		KIDManager.OnSessionUpdated();
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		return true;
	}

	// Token: 0x0600428C RID: 17036 RVA: 0x0015F5AE File Offset: 0x0015D7AE
	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	// Token: 0x0600428D RID: 17037 RVA: 0x00002789 File Offset: 0x00000989
	private static void DeleteStoredPermissions()
	{
	}

	// Token: 0x0600428E RID: 17038 RVA: 0x0015F5BB File Offset: 0x0015D7BB
	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	// Token: 0x0600428F RID: 17039 RVA: 0x0015F5D8 File Offset: 0x0015D7D8
	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, 1);
			}
			return new Permission(feature.ToStandardisedString(), false, 2);
		}
		else
		{
			Permission result;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out result))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, 2);
			}
			return result;
		}
	}

	// Token: 0x06004290 RID: 17040 RVA: 0x0015EEF3 File Offset: 0x0015D0F3
	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x06004291 RID: 17041 RVA: 0x0015F654 File Offset: 0x0015D854
	public static Task<bool> UseKID()
	{
		KIDManager.<UseKID>d__101 <UseKID>d__;
		<UseKID>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UseKID>d__.<>1__state = -1;
		<UseKID>d__.<>t__builder.Start<KIDManager.<UseKID>d__101>(ref <UseKID>d__);
		return <UseKID>d__.<>t__builder.Task;
	}

	// Token: 0x06004292 RID: 17042 RVA: 0x0015F690 File Offset: 0x0015D890
	public static Task<int> CheckKIDPhase()
	{
		KIDManager.<CheckKIDPhase>d__102 <CheckKIDPhase>d__;
		<CheckKIDPhase>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<CheckKIDPhase>d__.<>1__state = -1;
		<CheckKIDPhase>d__.<>t__builder.Start<KIDManager.<CheckKIDPhase>d__102>(ref <CheckKIDPhase>d__);
		return <CheckKIDPhase>d__.<>t__builder.Task;
	}

	// Token: 0x06004293 RID: 17043 RVA: 0x0015F6CC File Offset: 0x0015D8CC
	public static Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		KIDManager.<CheckKIDNewPlayerDateTime>d__103 <CheckKIDNewPlayerDateTime>d__;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
		<CheckKIDNewPlayerDateTime>d__.<>1__state = -1;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder.Start<KIDManager.<CheckKIDNewPlayerDateTime>d__103>(ref <CheckKIDNewPlayerDateTime>d__);
		return <CheckKIDNewPlayerDateTime>d__.<>t__builder.Task;
	}

	// Token: 0x06004294 RID: 17044 RVA: 0x0015F708 File Offset: 0x0015D908
	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool result;
		if (!bool.TryParse(kidtitleData.KIDEnabled, ref result))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return result;
	}

	// Token: 0x06004295 RID: 17045 RVA: 0x0015F750 File Offset: 0x0015D950
	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	// Token: 0x06004296 RID: 17046 RVA: 0x0015F780 File Offset: 0x0015D980
	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return default(DateTime?);
		}
		DateTime dateTime;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, 128, ref dateTime))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return default(DateTime?);
		}
		return new DateTime?(dateTime);
	}

	// Token: 0x06004297 RID: 17047 RVA: 0x0015F7E4 File Offset: 0x0015D9E4
	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == 3;
	}

	// Token: 0x06004298 RID: 17048 RVA: 0x0015F804 File Offset: 0x0015DA04
	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == 2 || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004299 RID: 17049 RVA: 0x0015F850 File Offset: 0x0015DA50
	public static Task<bool> SetKIDOptIn()
	{
		KIDManager.<SetKIDOptIn>d__109 <SetKIDOptIn>d__;
		<SetKIDOptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SetKIDOptIn>d__.<>1__state = -1;
		<SetKIDOptIn>d__.<>t__builder.Start<KIDManager.<SetKIDOptIn>d__109>(ref <SetKIDOptIn>d__);
		return <SetKIDOptIn>d__.<>t__builder.Task;
	}

	// Token: 0x0600429A RID: 17050 RVA: 0x0015F88C File Offset: 0x0015DA8C
	[return: TupleElementNames(new string[]
	{
		"success",
		"message"
	})]
	public static Task<ValueTuple<bool, string>> SetAndSendEmail(string email)
	{
		KIDManager.<SetAndSendEmail>d__110 <SetAndSendEmail>d__;
		<SetAndSendEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<SetAndSendEmail>d__.email = email;
		<SetAndSendEmail>d__.<>1__state = -1;
		<SetAndSendEmail>d__.<>t__builder.Start<KIDManager.<SetAndSendEmail>d__110>(ref <SetAndSendEmail>d__);
		return <SetAndSendEmail>d__.<>t__builder.Task;
	}

	// Token: 0x0600429B RID: 17051 RVA: 0x0015F8D0 File Offset: 0x0015DAD0
	public static Task<bool> SendOptInPermissions()
	{
		KIDManager.<SendOptInPermissions>d__111 <SendOptInPermissions>d__;
		<SendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SendOptInPermissions>d__.<>1__state = -1;
		<SendOptInPermissions>d__.<>t__builder.Start<KIDManager.<SendOptInPermissions>d__111>(ref <SendOptInPermissions>d__);
		return <SendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x0600429C RID: 17052 RVA: 0x0015F90C File Offset: 0x0015DB0C
	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == 1) && permissionDataByFeature.ManagedBy != 3;
	}

	// Token: 0x0600429D RID: 17053 RVA: 0x0015F958 File Offset: 0x0015DB58
	private static Task<bool> WaitForAuthentication()
	{
		KIDManager.<WaitForAuthentication>d__113 <WaitForAuthentication>d__;
		<WaitForAuthentication>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAuthentication>d__.<>1__state = -1;
		<WaitForAuthentication>d__.<>t__builder.Start<KIDManager.<WaitForAuthentication>d__113>(ref <WaitForAuthentication>d__);
		return <WaitForAuthentication>d__.<>t__builder.Task;
	}

	// Token: 0x0600429E RID: 17054 RVA: 0x0015F994 File Offset: 0x0015DB94
	[return: TupleElementNames(new string[]
	{
		"ageStatus",
		"resp"
	})]
	private static Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		KIDManager.<AgeGateFlow>d__114 <AgeGateFlow>d__;
		<AgeGateFlow>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<AgeStatusType, TMPSession>>.Create();
		<AgeGateFlow>d__.newPlayerData = newPlayerData;
		<AgeGateFlow>d__.<>1__state = -1;
		<AgeGateFlow>d__.<>t__builder.Start<KIDManager.<AgeGateFlow>d__114>(ref <AgeGateFlow>d__);
		return <AgeGateFlow>d__.<>t__builder.Task;
	}

	// Token: 0x0600429F RID: 17055 RVA: 0x0015F9D8 File Offset: 0x0015DBD8
	private static Task<VerifyAgeData> ProcessAgeGate()
	{
		KIDManager.<ProcessAgeGate>d__115 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDManager.<ProcessAgeGate>d__115>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x060042A0 RID: 17056 RVA: 0x0015FA13 File Offset: 0x0015DC13
	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	// Token: 0x060042A1 RID: 17057 RVA: 0x0015FA34 File Offset: 0x0015DC34
	private static Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		KIDManager.<Server_GetPlayerData>d__130 <Server_GetPlayerData>d__;
		<Server_GetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<Server_GetPlayerData>d__.forceRefresh = forceRefresh;
		<Server_GetPlayerData>d__.failureCallback = failureCallback;
		<Server_GetPlayerData>d__.<>1__state = -1;
		<Server_GetPlayerData>d__.<>t__builder.Start<KIDManager.<Server_GetPlayerData>d__130>(ref <Server_GetPlayerData>d__);
		return <Server_GetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x060042A2 RID: 17058 RVA: 0x0015FA80 File Offset: 0x0015DC80
	private static Task<bool> Server_SetConfirmedStatus()
	{
		KIDManager.<Server_SetConfirmedStatus>d__131 <Server_SetConfirmedStatus>d__;
		<Server_SetConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetConfirmedStatus>d__.<>1__state = -1;
		<Server_SetConfirmedStatus>d__.<>t__builder.Start<KIDManager.<Server_SetConfirmedStatus>d__131>(ref <Server_SetConfirmedStatus>d__);
		return <Server_SetConfirmedStatus>d__.<>t__builder.Task;
	}

	// Token: 0x060042A3 RID: 17059 RVA: 0x0015FABC File Offset: 0x0015DCBC
	private static Task<UpgradeSessionData> Server_UpgradeSession(UpgradeSessionRequest request)
	{
		KIDManager.<Server_UpgradeSession>d__132 <Server_UpgradeSession>d__;
		<Server_UpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<Server_UpgradeSession>d__.request = request;
		<Server_UpgradeSession>d__.<>1__state = -1;
		<Server_UpgradeSession>d__.<>t__builder.Start<KIDManager.<Server_UpgradeSession>d__132>(ref <Server_UpgradeSession>d__);
		return <Server_UpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x060042A4 RID: 17060 RVA: 0x0015FB00 File Offset: 0x0015DD00
	private static Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_VerifyAge>d__133 <Server_VerifyAge>d__;
		<Server_VerifyAge>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<Server_VerifyAge>d__.request = request;
		<Server_VerifyAge>d__.failureCallback = failureCallback;
		<Server_VerifyAge>d__.<>1__state = -1;
		<Server_VerifyAge>d__.<>t__builder.Start<KIDManager.<Server_VerifyAge>d__133>(ref <Server_VerifyAge>d__);
		return <Server_VerifyAge>d__.<>t__builder.Task;
	}

	// Token: 0x060042A5 RID: 17061 RVA: 0x0015FB4C File Offset: 0x0015DD4C
	private static Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		KIDManager.<Server_AttemptAgeUpdate>d__134 <Server_AttemptAgeUpdate>d__;
		<Server_AttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<Server_AttemptAgeUpdate>d__.request = request;
		<Server_AttemptAgeUpdate>d__.<>1__state = -1;
		<Server_AttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<Server_AttemptAgeUpdate>d__134>(ref <Server_AttemptAgeUpdate>d__);
		return <Server_AttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x060042A6 RID: 17062 RVA: 0x0015FB90 File Offset: 0x0015DD90
	private static Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_AppealAge>d__135 <Server_AppealAge>d__;
		<Server_AppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_AppealAge>d__.request = request;
		<Server_AppealAge>d__.<>1__state = -1;
		<Server_AppealAge>d__.<>t__builder.Start<KIDManager.<Server_AppealAge>d__135>(ref <Server_AppealAge>d__);
		return <Server_AppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x060042A7 RID: 17063 RVA: 0x0015FBD4 File Offset: 0x0015DDD4
	private static Task<ValueTuple<bool, string>> Server_SendChallengeEmail(SendChallengeEmailRequest request)
	{
		KIDManager.<Server_SendChallengeEmail>d__136 <Server_SendChallengeEmail>d__;
		<Server_SendChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<Server_SendChallengeEmail>d__.request = request;
		<Server_SendChallengeEmail>d__.<>1__state = -1;
		<Server_SendChallengeEmail>d__.<>t__builder.Start<KIDManager.<Server_SendChallengeEmail>d__136>(ref <Server_SendChallengeEmail>d__);
		return <Server_SendChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x060042A8 RID: 17064 RVA: 0x0015FC18 File Offset: 0x0015DE18
	private static Task<bool> Server_SetOptInPermissions(SetOptInPermissionsRequest request, Action failureCallback)
	{
		KIDManager.<Server_SetOptInPermissions>d__137 <Server_SetOptInPermissions>d__;
		<Server_SetOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetOptInPermissions>d__.request = request;
		<Server_SetOptInPermissions>d__.failureCallback = failureCallback;
		<Server_SetOptInPermissions>d__.<>1__state = -1;
		<Server_SetOptInPermissions>d__.<>t__builder.Start<KIDManager.<Server_SetOptInPermissions>d__137>(ref <Server_SetOptInPermissions>d__);
		return <Server_SetOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x060042A9 RID: 17065 RVA: 0x0015FC64 File Offset: 0x0015DE64
	private static Task<bool> Server_OptIn()
	{
		KIDManager.<Server_OptIn>d__138 <Server_OptIn>d__;
		<Server_OptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_OptIn>d__.<>1__state = -1;
		<Server_OptIn>d__.<>t__builder.Start<KIDManager.<Server_OptIn>d__138>(ref <Server_OptIn>d__);
		return <Server_OptIn>d__.<>t__builder.Task;
	}

	// Token: 0x060042AA RID: 17066 RVA: 0x0015FCA0 File Offset: 0x0015DEA0
	private static Task<GetRequirementsData> Server_GetRequirements()
	{
		KIDManager.<Server_GetRequirements>d__139 <Server_GetRequirements>d__;
		<Server_GetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<Server_GetRequirements>d__.<>1__state = -1;
		<Server_GetRequirements>d__.<>t__builder.Start<KIDManager.<Server_GetRequirements>d__139>(ref <Server_GetRequirements>d__);
		return <Server_GetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x060042AB RID: 17067 RVA: 0x0015FCDC File Offset: 0x0015DEDC
	[return: TupleElementNames(new string[]
	{
		"code",
		"responseModel",
		"errorMessage"
	})]
	private static Task<ValueTuple<long, T, string>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequest>d__140<T, Q> <KIDServerWebRequest>d__;
		<KIDServerWebRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<long, T, string>>.Create();
		<KIDServerWebRequest>d__.endpoint = endpoint;
		<KIDServerWebRequest>d__.operationType = operationType;
		<KIDServerWebRequest>d__.requestData = requestData;
		<KIDServerWebRequest>d__.queryParams = queryParams;
		<KIDServerWebRequest>d__.maxRetries = maxRetries;
		<KIDServerWebRequest>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequest>d__.<>1__state = -1;
		<KIDServerWebRequest>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequest>d__140<T, Q>>(ref <KIDServerWebRequest>d__);
		return <KIDServerWebRequest>d__.<>t__builder.Task;
	}

	// Token: 0x060042AC RID: 17068 RVA: 0x0015FD4C File Offset: 0x0015DF4C
	private static Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequestNoResponse>d__141<Q> <KIDServerWebRequestNoResponse>d__;
		<KIDServerWebRequestNoResponse>d__.<>t__builder = AsyncTaskMethodBuilder<long>.Create();
		<KIDServerWebRequestNoResponse>d__.endpoint = endpoint;
		<KIDServerWebRequestNoResponse>d__.operationType = operationType;
		<KIDServerWebRequestNoResponse>d__.requestData = requestData;
		<KIDServerWebRequestNoResponse>d__.maxRetries = maxRetries;
		<KIDServerWebRequestNoResponse>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequestNoResponse>d__.<>1__state = -1;
		<KIDServerWebRequestNoResponse>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequestNoResponse>d__141<Q>>(ref <KIDServerWebRequestNoResponse>d__);
		return <KIDServerWebRequestNoResponse>d__.<>t__builder.Task;
	}

	// Token: 0x060042AD RID: 17069 RVA: 0x0015FDB0 File Offset: 0x0015DFB0
	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x060042AE RID: 17070 RVA: 0x0015FDD1 File Offset: 0x0015DFD1
	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully unregistered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x060042AF RID: 17071 RVA: 0x0015FDF2 File Offset: 0x0015DFF2
	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x060042B0 RID: 17072 RVA: 0x0015FE13 File Offset: 0x0015E013
	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x060042B1 RID: 17073 RVA: 0x0015FE34 File Offset: 0x0015E034
	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x060042B2 RID: 17074 RVA: 0x0015FE55 File Offset: 0x0015E055
	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x060042B3 RID: 17075 RVA: 0x0015FE76 File Offset: 0x0015E076
	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x060042B4 RID: 17076 RVA: 0x0015FE97 File Offset: 0x0015E097
	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x060042B5 RID: 17077 RVA: 0x0015FEB8 File Offset: 0x0015E0B8
	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x060042B6 RID: 17078 RVA: 0x0015FED9 File Offset: 0x0015E0D9
	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x060042B7 RID: 17079 RVA: 0x0015FEFA File Offset: 0x0015E0FA
	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the UGC permission");
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	// Token: 0x060042B8 RID: 17080 RVA: 0x0015FF1C File Offset: 0x0015E11C
	public static Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		KIDManager.<WaitForAndUpdateNewSession>d__168 <WaitForAndUpdateNewSession>d__;
		<WaitForAndUpdateNewSession>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAndUpdateNewSession>d__.forceRefresh = forceRefresh;
		<WaitForAndUpdateNewSession>d__.<>1__state = -1;
		<WaitForAndUpdateNewSession>d__.<>t__builder.Start<KIDManager.<WaitForAndUpdateNewSession>d__168>(ref <WaitForAndUpdateNewSession>d__);
		return <WaitForAndUpdateNewSession>d__.<>t__builder.Task;
	}

	// Token: 0x060042B9 RID: 17081 RVA: 0x0015FF60 File Offset: 0x0015E160
	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	// Token: 0x060042BA RID: 17082 RVA: 0x0015FFD4 File Offset: 0x0015E1D4
	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission.Invoke();
		}
		bool voiceChatEnabled = false;
		bool joinGroupsEnabled = false;
		bool customUsernamesEnabled = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC.Invoke(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer.Invoke(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms.Invoke(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						joinGroupsEnabled = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames.Invoke(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					customUsernamesEnabled = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat.Invoke(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				voiceChatEnabled = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(joinGroupsEnabled, voiceChatEnabled, customUsernamesEnabled, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x00160208 File Offset: 0x0015E408
	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, ref permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	// Token: 0x0400538C RID: 21388
	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	// Token: 0x0400538D RID: 21389
	public const string UGC_PERMISSION_NAME = "mods";

	// Token: 0x0400538E RID: 21390
	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	// Token: 0x0400538F RID: 21391
	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	// Token: 0x04005390 RID: 21392
	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	// Token: 0x04005391 RID: 21393
	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	// Token: 0x04005392 RID: 21394
	public const string KID_DATA_KEY = "KIDData";

	// Token: 0x04005393 RID: 21395
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	// Token: 0x04005394 RID: 21396
	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	// Token: 0x04005395 RID: 21397
	private const string KID_SETUP_FLAG = "KID-Setup-";

	// Token: 0x04005396 RID: 21398
	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	// Token: 0x0400539B RID: 21403
	private static string _emailAddress;

	// Token: 0x0400539C RID: 21404
	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	// Token: 0x0400539D RID: 21405
	private static bool _titleDataReady = false;

	// Token: 0x0400539E RID: 21406
	private static bool _useKid = false;

	// Token: 0x0400539F RID: 21407
	private static int _kIDPhase = 0;

	// Token: 0x040053A0 RID: 21408
	private static DateTime? _kIDNewPlayerDateTime = default(DateTime?);

	// Token: 0x040053A4 RID: 21412
	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	// Token: 0x040053A5 RID: 21413
	private static string parentEmailForUserPlayerPrefRef;

	// Token: 0x040053A6 RID: 21414
	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	// Token: 0x040053A7 RID: 21415
	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	// Token: 0x040053A8 RID: 21416
	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	// Token: 0x040053A9 RID: 21417
	private const string KID_GET_SESSION = "GetPlayerData";

	// Token: 0x040053AA RID: 21418
	private const string KID_VERIFY_AGE = "VerifyAge";

	// Token: 0x040053AB RID: 21419
	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	// Token: 0x040053AC RID: 21420
	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	// Token: 0x040053AD RID: 21421
	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	// Token: 0x040053AE RID: 21422
	private const string KID_APPEAL_AGE = "AppealAge";

	// Token: 0x040053AF RID: 21423
	private const string KID_OPT_IN = "OptIn";

	// Token: 0x040053B0 RID: 21424
	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	// Token: 0x040053B1 RID: 21425
	private const string KID_SET_CONFIRMED_STATUS = "SetConfirmedStatus";

	// Token: 0x040053B2 RID: 21426
	private const string KID_SET_OPT_IN_PERMISSIONS = "SetOptInPermissions";

	// Token: 0x040053B3 RID: 21427
	private const string KID_FORCE_REFRESH = "sessionRefresh";

	// Token: 0x040053B4 RID: 21428
	private const int MAX_RETRIES_FOR_CRITICAL_KID_SERVER_REQUESTS = 3;

	// Token: 0x040053B5 RID: 21429
	private const int MAX_RETRIES_FOR_NORMAL_KID_SERVER_REQUESTS = 2;

	// Token: 0x040053B6 RID: 21430
	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	// Token: 0x040053B7 RID: 21431
	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	// Token: 0x040053B8 RID: 21432
	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	// Token: 0x040053B9 RID: 21433
	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	// Token: 0x040053BA RID: 21434
	public const string KID_PERMISSION__UGC = "mods";

	// Token: 0x040053BB RID: 21435
	private const float MAX_SESSION_UPDATE_TIME = 600f;

	// Token: 0x040053BC RID: 21436
	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	// Token: 0x040053BD RID: 21437
	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	// Token: 0x040053BE RID: 21438
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	// Token: 0x040053BF RID: 21439
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	// Token: 0x040053C0 RID: 21440
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	// Token: 0x040053C1 RID: 21441
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	// Token: 0x040053C2 RID: 21442
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	// Token: 0x040053C3 RID: 21443
	private static bool _isUpdatingNewSession = false;

	// Token: 0x040053C4 RID: 21444
	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	// Token: 0x02000A41 RID: 2625
	// (Invoke) Token: 0x060042BF RID: 17087
	public delegate void OnEmailResultReceived(bool result);
}
