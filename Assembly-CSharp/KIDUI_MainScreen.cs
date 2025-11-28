using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000AB0 RID: 2736
public class KIDUI_MainScreen : MonoBehaviour
{
	// Token: 0x06004497 RID: 17559 RVA: 0x0016B2A9 File Offset: 0x001694A9
	private void Awake()
	{
		KIDUI_MainScreen._featuresList.Clear();
		if (this._setupKidScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._initialised)
		{
			return;
		}
		this.InitialiseMainScreen();
	}

	// Token: 0x06004498 RID: 17560 RVA: 0x0016B2E2 File Offset: 0x001694E2
	private void OnEnable()
	{
		KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06004499 RID: 17561 RVA: 0x0016B30C File Offset: 0x0016950C
	private void OnDisable()
	{
		KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x0600449A RID: 17562 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDestroy()
	{
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x0016B344 File Offset: 0x00169544
	private void ConstructFeatureSettings()
	{
		for (int i = 0; i < this._displayOrder.Length; i++)
		{
			for (int j = 0; j < this._featureSetups.Count; j++)
			{
				if (this._featureSetups[j].linkedFeature == this._displayOrder[i])
				{
					this.CreateNewFeatureDisplay(this._featureSetups[j]);
					break;
				}
			}
		}
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x0016B3B0 File Offset: 0x001695B0
	private void CreateNewFeatureDisplay(KIDUI_MainScreen.FeatureToggleSetup setup)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(setup.linkedFeature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID::UI::MAIN] Failed to retrieve permission data for feature; [" + setup.linkedFeature.ToString() + "]", Array.Empty<object>());
			return;
		}
		if (permissionDataByFeature.ManagedBy == 3)
		{
			return;
		}
		if (permissionDataByFeature.ManagedBy == 1)
		{
			if (permissionDataByFeature.Enabled)
			{
				return;
			}
			if (KIDManager.CheckFeatureOptIn(setup.linkedFeature, null).Item2)
			{
				return;
			}
		}
		if (setup.alwaysCheckFeatureSetting && KIDManager.CheckFeatureSettingEnabled(setup.linkedFeature))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this._featurePrefab, this._featureRootTransform);
		KIDUIFeatureSetting component = gameObject.GetComponent<KIDUIFeatureSetting>();
		if (permissionDataByFeature.ManagedBy == 2)
		{
			Debug.LogFormat(string.Format("[KID::UI::MAIN_SCREEN] Adding new Locked Feature:  {0} Is enabled: {1}", setup.linkedFeature.ToString(), permissionDataByFeature.Enabled), Array.Empty<object>());
			component.CreateNewFeatureSettingGuardianManaged(setup, permissionDataByFeature.Enabled);
			if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
			{
				KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
			}
			KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
			return;
		}
		if (setup.requiresToggle)
		{
			component.CreateNewFeatureSettingWithToggle(setup, false, setup.alwaysCheckFeatureSetting);
		}
		else
		{
			component.CreateNewFeatureSettingWithoutToggle(setup, setup.alwaysCheckFeatureSetting);
		}
		if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
		{
			KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
		}
		KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
		this.ConstructAdditionalSetup(setup.linkedFeature, gameObject);
	}

	// Token: 0x0600449D RID: 17565 RVA: 0x0016B54C File Offset: 0x0016974C
	private void ConstructAdditionalSetup(EKIDFeatures feature, GameObject featureObject)
	{
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x0016B554 File Offset: 0x00169754
	private void UpdatePermissionsAndFeaturesScreen()
	{
		int num = 0;
		Debug.LogFormat(string.Format("[KID::UI::MAIN] Updated Feature listings. To Update: [{0}]", KIDUI_MainScreen._featuresList.Count), Array.Empty<object>());
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(keyValuePair.Key);
				if (permissionDataByFeature == null)
				{
					Debug.LogErrorFormat("[KID::UI::MAIN] Failed to find permission data for feature: [" + keyValuePair.Key.ToString() + "]", Array.Empty<object>());
				}
				else if (permissionDataByFeature.ManagedBy == 2)
				{
					keyValuePair.Value[i].SetGuardianManagedState(permissionDataByFeature.Enabled);
				}
				else
				{
					bool isOptedIn = KIDManager.CheckFeatureOptIn(keyValuePair.Key, permissionDataByFeature).Item2;
					if (keyValuePair.Value[i].AlwaysCheckFeatureSetting)
					{
						isOptedIn = KIDManager.CheckFeatureSettingEnabled(keyValuePair.Key);
					}
					if (!keyValuePair.Value[i].GetHasToggle())
					{
						keyValuePair.Value[i].SetPlayerManagedState(permissionDataByFeature.Enabled, isOptedIn);
					}
				}
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair2 in KIDUI_MainScreen._featuresList)
		{
			for (int j = 0; j < keyValuePair2.Value.Count; j++)
			{
				num2++;
				Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(keyValuePair2.Key);
				if (keyValuePair2.Value[j].GetFeatureToggleState() || permissionDataByFeature2.ManagedBy == 1)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			if (!this._initialised)
			{
				this._titleFeaturePermissions.SetActive(false);
				this._titleGameFeatures.SetActive(true);
			}
			this._hasAllPermissions = true;
			this._getPermissionsButton.gameObject.SetActive(false);
			this._gettingPermissionsButton.gameObject.SetActive(false);
			this._requestPermissionsButton.gameObject.SetActive(false);
			this._permissionsTip.SetActive(false);
			this.SetButtonContainersVisibility(EGetPermissionsStatus.RequestedPermission);
		}
	}

	// Token: 0x0600449F RID: 17567 RVA: 0x0016B7DC File Offset: 0x001699DC
	private bool IsFeatureToggledOn(EKIDFeatures permissionFeature)
	{
		List<KIDUIFeatureSetting> list;
		if (!KIDUI_MainScreen._featuresList.TryGetValue(permissionFeature, ref list))
		{
			return true;
		}
		KIDUIFeatureSetting kiduifeatureSetting = Enumerable.FirstOrDefault<KIDUIFeatureSetting>(list);
		if (kiduifeatureSetting == null)
		{
			Debug.LogErrorFormat(string.Format("[KID::UI::MAIN] Empty list for permission Name [{0}]", permissionFeature), Array.Empty<object>());
			return false;
		}
		return kiduifeatureSetting.GetFeatureToggleState();
	}

	// Token: 0x060044A0 RID: 17568 RVA: 0x0016B82C File Offset: 0x00169A2C
	public void InitialiseMainScreen()
	{
		if (this._initialised)
		{
			Debug.Log("[KID::MAIN_SCREEN] Already Initialised");
			return;
		}
		this.ConstructFeatureSettings();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		this._initialised = true;
	}

	// Token: 0x060044A1 RID: 17569 RVA: 0x0016B8B0 File Offset: 0x00169AB0
	public void ShowMainScreen(EMainScreenStatus showStatus, KIDUI_Controller.Metrics_ShowReason reason)
	{
		this.ShowMainScreen(showStatus);
		this._mainScreenOpenedReason = reason;
		string text = reason.ToString().Replace("_", "-").ToLower();
		TelemetryData telemetryData = default(TelemetryData);
		telemetryData.EventName = "kid_game_settings";
		telemetryData.CustomTags = new string[]
		{
			"kid_setup",
			KIDTelemetry.GameVersionCustomTag,
			KIDTelemetry.GameEnvironment,
			KIDTelemetry.Open_MetricActionCustomTag
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("screen_shown_reason", text);
		telemetryData.BodyData = dictionary;
		TelemetryData telemetryData2 = telemetryData;
		foreach (Permission permission in KIDManager.GetAllPermissionsData())
		{
			telemetryData2.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
			telemetryData2.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
		}
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
	}

	// Token: 0x060044A2 RID: 17570 RVA: 0x0016B9FC File Offset: 0x00169BFC
	public void ShowMainScreen(EMainScreenStatus showStatus)
	{
		KIDUI_MainScreen.ShownSettingsScreen = true;
		base.gameObject.SetActive(true);
		this.ConfigurePermissionsButtons();
		this.UpdateScreenStatus(showStatus, false);
	}

	// Token: 0x060044A3 RID: 17571 RVA: 0x0016BA20 File Offset: 0x00169C20
	public void UpdateScreenStatus(EMainScreenStatus showStatus, bool sendMetrics = false)
	{
		if (sendMetrics && showStatus == EMainScreenStatus.Updated)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = default(TelemetryData);
			telemetryData.EventName = "kid_game_settings";
			telemetryData.CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment,
				KIDTelemetry.Updated_MetricActionCustomTag
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("screen_shown_reason", text);
			telemetryData.BodyData = dictionary;
			TelemetryData telemetryData2 = telemetryData;
			foreach (Permission permission in KIDManager.GetAllPermissionsData())
			{
				telemetryData2.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
				telemetryData2.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
			}
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		}
		GameObject activeStatusObject = this.GetActiveStatusObject();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		switch (showStatus)
		{
		default:
			if (!this._hasAllPermissions)
			{
				this._missingStatus.SetActive(true);
			}
			else if (this._hasAllPermissions)
			{
				this._fullPlayerControlStatus.SetActive(true);
			}
			else
			{
				this._screenStatus = showStatus;
			}
			break;
		case EMainScreenStatus.Declined:
			this._declinedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Pending:
			this._pendingStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Timedout:
			this._timeoutStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Setup:
			this._setupRequiredStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Previous:
			if (activeStatusObject != null)
			{
				activeStatusObject.SetActive(true);
			}
			else
			{
				this._updatedStatus.SetActive(true);
			}
			break;
		case EMainScreenStatus.FullControl:
			this._fullPlayerControlStatus.SetActive(true);
			break;
		}
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x060044A4 RID: 17572 RVA: 0x000396A0 File Offset: 0x000378A0
	public void HideMainScreen()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060044A5 RID: 17573 RVA: 0x0016BCBC File Offset: 0x00169EBC
	public void OnAskForPermission()
	{
		KIDUI_MainScreen.<OnAskForPermission>d__52 <OnAskForPermission>d__;
		<OnAskForPermission>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnAskForPermission>d__.<>4__this = this;
		<OnAskForPermission>d__.<>1__state = -1;
		<OnAskForPermission>d__.<>t__builder.Start<KIDUI_MainScreen.<OnAskForPermission>d__52>(ref <OnAskForPermission>d__);
	}

	// Token: 0x060044A6 RID: 17574 RVA: 0x0016BCF4 File Offset: 0x00169EF4
	public void OnSaveAndExit()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::KID_UI_MAINSCREEN] There is no session as such cannot opt into anything");
			KIDUI_Controller.Instance.CloseKIDScreens();
			return;
		}
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		for (int i = 0; i < allPermissionsData.Count; i++)
		{
			string name = allPermissionsData[i].Name;
			if (!(name == "multiplayer"))
			{
				if (!(name == "mods"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "voice-chat"))
						{
							if (!(name == "custom-username"))
							{
								Debug.LogError("[KID::UI::MainScreen] Unhandled permission when saving and exiting: [" + allPermissionsData[i].Name + "]");
							}
							else
							{
								this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Custom_Nametags, delegate(bool b, Permission p, bool hasOptedInPreviously)
								{
									GorillaComputer.instance.SetNametagSetting(b, p.ManagedBy, hasOptedInPreviously);
								});
							}
						}
						else
						{
							this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Voice_Chat, delegate(bool b, Permission p, bool hasOptedInPreviously)
							{
								GorillaComputer.instance.KID_SetVoiceChatSettingOnStart(b, p.ManagedBy, hasOptedInPreviously);
							});
						}
					}
				}
				else
				{
					this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Mods, null);
				}
			}
			else
			{
				this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Multiplayer, null);
			}
		}
		KIDManager.SendOptInPermissions();
		if (this._screenStatus != EMainScreenStatus.None)
		{
			string text = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = default(TelemetryData);
			telemetryData.EventName = "kid_game_settings";
			telemetryData.CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("screen_shown_reason", text);
			dictionary.Add("kid_status", this._screenStatus.ToString().ToLower());
			dictionary.Add("button_pressed", "save_and_continue");
			telemetryData.BodyData = dictionary;
			TelemetryData telemetryData2 = telemetryData;
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData2.EventName, telemetryData2.BodyData, telemetryData2.CustomTags);
		}
		else
		{
			Debug.LogError("[KID::UI::MAIN_SCREEN] Trying to close k-ID Main Screen, but screen status is set to [None] - Invalid status, will not submit analytics");
		}
		KIDUI_Controller.Instance.CloseKIDScreens();
	}

	// Token: 0x060044A7 RID: 17575 RVA: 0x0016BF20 File Offset: 0x0016A120
	public int GetFeatureListingCount()
	{
		int num = 0;
		foreach (List<KIDUIFeatureSetting> list in KIDUI_MainScreen._featuresList.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x060044A8 RID: 17576 RVA: 0x0016BF7C File Offset: 0x0016A17C
	private Task<bool> UpdateAndCheckForMissingPermissions()
	{
		KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__55 <UpdateAndCheckForMissingPermissions>d__;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateAndCheckForMissingPermissions>d__.<>4__this = this;
		<UpdateAndCheckForMissingPermissions>d__.<>1__state = -1;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder.Start<KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__55>(ref <UpdateAndCheckForMissingPermissions>d__);
		return <UpdateAndCheckForMissingPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x060044A9 RID: 17577 RVA: 0x0016BFC0 File Offset: 0x0016A1C0
	private void OnLanguageChanged()
	{
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			List<KIDUIFeatureSetting> value = keyValuePair.Value;
			if (value != null)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i] != null)
					{
						value[i].RefreshTextOnLanguageChanged();
					}
				}
			}
		}
	}

	// Token: 0x060044AA RID: 17578 RVA: 0x0016C044 File Offset: 0x0016A244
	private void UpdateOptInSetting(Permission permissionData, EKIDFeatures feature, Action<bool, Permission, bool> onOptedIn)
	{
		bool item = KIDManager.CheckFeatureOptIn(feature, permissionData).Item2;
		bool flag = this.IsFeatureToggledOn(feature);
		Debug.Log(string.Format("[KID::UI::MainScreen] Update opt in for {0}. Has opted in: {1}. Toggled on: {2}", feature.ToString(), item, flag));
		KIDManager.SetFeatureOptIn(feature, flag);
		if (onOptedIn != null)
		{
			onOptedIn.Invoke(flag, permissionData, item);
		}
	}

	// Token: 0x060044AB RID: 17579 RVA: 0x0016C0A1 File Offset: 0x0016A2A1
	public void OnConfirmedEmailAddress(string emailAddress)
	{
		this._emailAddress = emailAddress;
		Debug.LogFormat("[KID::UI::Main] Email has been confirmed: " + this._emailAddress, Array.Empty<object>());
	}

	// Token: 0x060044AC RID: 17580 RVA: 0x0016C0C4 File Offset: 0x0016A2C4
	private IEnumerable<string> CollectPermissionsToUpgrade()
	{
		return Enumerable.Select<Permission, string>(Enumerable.Where<Permission>(KIDManager.GetAllPermissionsData(), (Permission permission) => permission.ManagedBy == 2 && !permission.Enabled), (Permission permission) => permission.Name);
	}

	// Token: 0x060044AD RID: 17581 RVA: 0x0016C120 File Offset: 0x0016A320
	private void ConfigurePermissionsButtons()
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS");
		if (!this._getPermissionsButton.gameObject.activeSelf && !this._gettingPermissionsButton.gameObject.activeSelf)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - GET PERMISSIONS IS DISABLED");
			return;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - CHECK SESSION STATUS: Is Default: [" + KIDManager.CurrentSession.IsDefault.ToString() + "]");
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x060044AE RID: 17582 RVA: 0x0016C194 File Offset: 0x0016A394
	private void SetButtonContainersVisibility(EGetPermissionsStatus permissionStatus)
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - PERMISSION STATE: [" + permissionStatus.ToString() + "]");
		this._defaultButtonsContainer.SetActive(permissionStatus == EGetPermissionsStatus.GetPermission);
		this._permissionsRequestingButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestingPermission);
		this._permissionsRequestedButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestedPermission);
	}

	// Token: 0x060044AF RID: 17583 RVA: 0x0016C1F0 File Offset: 0x0016A3F0
	private GameObject GetActiveStatusObject()
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(this._declinedStatus);
		list.Add(this._timeoutStatus);
		list.Add(this._pendingStatus);
		list.Add(this._updatedStatus);
		list.Add(this._setupRequiredStatus);
		list.Add(this._fullPlayerControlStatus);
		foreach (GameObject gameObject in list)
		{
			if (gameObject.activeInHierarchy)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x060044B0 RID: 17584 RVA: 0x0016C294 File Offset: 0x0016A494
	private static EGetPermissionsStatus GetPermissionState()
	{
		if (!KIDManager.CurrentSession.IsDefault)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW REQUESTED");
			return EGetPermissionsStatus.RequestedPermission;
		}
		if (PlayerPrefs.GetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 0) == 0)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW DEFAULT");
			return EGetPermissionsStatus.GetPermission;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW SWAPPED DEFAULT");
		return EGetPermissionsStatus.RequestingPermission;
	}

	// Token: 0x060044B1 RID: 17585 RVA: 0x0016C2D4 File Offset: 0x0016A4D4
	private void OnFeatureToggleChanged(EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			this.OnMultiplayerToggled();
			return;
		case EKIDFeatures.Custom_Nametags:
			this.OnCustomNametagsToggled();
			return;
		case EKIDFeatures.Voice_Chat:
			this.OnVoiceChatToggled();
			return;
		case EKIDFeatures.Mods:
			this.OnModToggleChanged();
			return;
		case EKIDFeatures.Groups:
			this.OnGroupToggleChanged();
			return;
		default:
			Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] Toggle NOT YET IMPLEMENTED for Feature: " + feature.ToString() + ".", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x060044B2 RID: 17586 RVA: 0x0016C346 File Offset: 0x0016A546
	private void OnMultiplayerToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MULTIPLAYER Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x060044B3 RID: 17587 RVA: 0x0016C357 File Offset: 0x0016A557
	private void OnVoiceChatToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] VOICE CHAT Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x060044B4 RID: 17588 RVA: 0x0016C368 File Offset: 0x0016A568
	private void OnGroupToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] GROUPS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x060044B5 RID: 17589 RVA: 0x0016C379 File Offset: 0x0016A579
	private void OnModToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MODS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x060044B6 RID: 17590 RVA: 0x0016C38A File Offset: 0x0016A58A
	private void OnCustomNametagsToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] CUSTOM USERNAMES Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x0400563D RID: 22077
	public const string OPT_IN_SUFFIX = "-opt-in";

	// Token: 0x0400563E RID: 22078
	public static bool ShownSettingsScreen = false;

	// Token: 0x0400563F RID: 22079
	[SerializeField]
	private GameObject _kidScreensGroup;

	// Token: 0x04005640 RID: 22080
	[SerializeField]
	private KIDUI_SetupScreen _setupKidScreen;

	// Token: 0x04005641 RID: 22081
	[SerializeField]
	private KIDUI_SendUpgradeEmailScreen _sendUpgradeEmailScreen;

	// Token: 0x04005642 RID: 22082
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005643 RID: 22083
	[Header("Permission Request Buttons")]
	[SerializeField]
	private KIDUIButton _getPermissionsButton;

	// Token: 0x04005644 RID: 22084
	[SerializeField]
	private KIDUIButton _gettingPermissionsButton;

	// Token: 0x04005645 RID: 22085
	[SerializeField]
	private KIDUIButton _requestPermissionsButton;

	// Token: 0x04005646 RID: 22086
	[SerializeField]
	private GameObject _defaultButtonsContainer;

	// Token: 0x04005647 RID: 22087
	[SerializeField]
	private GameObject _permissionsRequestingButtonContainer;

	// Token: 0x04005648 RID: 22088
	[SerializeField]
	private GameObject _permissionsRequestedButtonContainer;

	// Token: 0x04005649 RID: 22089
	private bool _hasAllPermissions;

	// Token: 0x0400564A RID: 22090
	[Header("Dynamic Feature Settings Setup")]
	[SerializeField]
	private GameObject _featurePrefab;

	// Token: 0x0400564B RID: 22091
	[SerializeField]
	private Transform _featureRootTransform;

	// Token: 0x0400564C RID: 22092
	[SerializeField]
	private EKIDFeatures[] _displayOrder = new EKIDFeatures[4];

	// Token: 0x0400564D RID: 22093
	[SerializeField]
	private List<KIDUI_MainScreen.FeatureToggleSetup> _featureSetups = new List<KIDUI_MainScreen.FeatureToggleSetup>();

	// Token: 0x0400564E RID: 22094
	[Header("Additional Feature-Specific Setup")]
	[SerializeField]
	private GameObject _voiceChatLabel;

	// Token: 0x0400564F RID: 22095
	[Header("Hide Permissions Tip")]
	[SerializeField]
	private GameObject _permissionsTip;

	// Token: 0x04005650 RID: 22096
	[Header("Titles")]
	[SerializeField]
	private GameObject _titleFeaturePermissions;

	// Token: 0x04005651 RID: 22097
	[SerializeField]
	private GameObject _titleGameFeatures;

	// Token: 0x04005652 RID: 22098
	[Header("Game Status Setup")]
	[SerializeField]
	private GameObject _missingStatus;

	// Token: 0x04005653 RID: 22099
	[SerializeField]
	private GameObject _updatedStatus;

	// Token: 0x04005654 RID: 22100
	[SerializeField]
	private GameObject _declinedStatus;

	// Token: 0x04005655 RID: 22101
	[SerializeField]
	private GameObject _pendingStatus;

	// Token: 0x04005656 RID: 22102
	[SerializeField]
	private GameObject _timeoutStatus;

	// Token: 0x04005657 RID: 22103
	[SerializeField]
	private GameObject _setupRequiredStatus;

	// Token: 0x04005658 RID: 22104
	[SerializeField]
	private GameObject _fullPlayerControlStatus;

	// Token: 0x04005659 RID: 22105
	private string _emailAddress;

	// Token: 0x0400565A RID: 22106
	private bool _multiplayerEnabled;

	// Token: 0x0400565B RID: 22107
	private bool _customNameEnabled;

	// Token: 0x0400565C RID: 22108
	private bool _voiceChatEnabled;

	// Token: 0x0400565D RID: 22109
	private bool _initialised;

	// Token: 0x0400565E RID: 22110
	private KIDUI_Controller.Metrics_ShowReason _mainScreenOpenedReason;

	// Token: 0x0400565F RID: 22111
	private EMainScreenStatus _screenStatus;

	// Token: 0x04005660 RID: 22112
	private GameObject _eventSystemObj;

	// Token: 0x04005661 RID: 22113
	private static Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>> _featuresList = new Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>>();

	// Token: 0x02000AB1 RID: 2737
	[Serializable]
	public struct FeatureToggleSetup
	{
		// Token: 0x04005662 RID: 22114
		public EKIDFeatures linkedFeature;

		// Token: 0x04005663 RID: 22115
		public string permissionName;

		// Token: 0x04005664 RID: 22116
		public LocalizedString featureName;

		// Token: 0x04005665 RID: 22117
		public bool requiresToggle;

		// Token: 0x04005666 RID: 22118
		public bool alwaysCheckFeatureSetting;

		// Token: 0x04005667 RID: 22119
		public LocalizedString enabledText;

		// Token: 0x04005668 RID: 22120
		public LocalizedString disabledText;
	}
}
