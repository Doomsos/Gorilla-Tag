using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x02000EE3 RID: 3811
	public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
	{
		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06005F38 RID: 24376 RVA: 0x001E9964 File Offset: 0x001E7B64
		public string versionMismatch
		{
			get
			{
				if (this._lastLocaleChecked_Version != null && this._lastLocaleChecked_Version == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedVersionMismatch))
				{
					return this._cachedVersionMismatch;
				}
				string defaultResult = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
				string cachedVersionMismatch;
				LocalisationManager.TryGetKeyForCurrentLocale("VERSION_MISMATCH", out cachedVersionMismatch, defaultResult);
				this._lastLocaleChecked_Version = LocalisationManager.CurrentLanguage;
				this._cachedVersionMismatch = cachedVersionMismatch;
				return this._cachedVersionMismatch;
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x06005F39 RID: 24377 RVA: 0x001E99D4 File Offset: 0x001E7BD4
		public string unableToConnect
		{
			get
			{
				if (this._lastLocaleChecked_Connect != null && this._lastLocaleChecked_Connect == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedUnableToConnect))
				{
					return this._cachedUnableToConnect;
				}
				string defaultResult = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
				string cachedUnableToConnect;
				LocalisationManager.TryGetKeyForCurrentLocale("CONNECTION_ISSUE", out cachedUnableToConnect, defaultResult);
				this._lastLocaleChecked_Connect = LocalisationManager.CurrentLanguage;
				this._cachedUnableToConnect = cachedUnableToConnect;
				return this._cachedUnableToConnect;
			}
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x001E9A41 File Offset: 0x001E7C41
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06005F3B RID: 24379 RVA: 0x001E9A59 File Offset: 0x001E7C59
		// (set) Token: 0x06005F3C RID: 24380 RVA: 0x001E9A61 File Offset: 0x001E7C61
		public string[] allowedMapsToJoin
		{
			get
			{
				return this._allowedMapsToJoin;
			}
			set
			{
				this._allowedMapsToJoin = value;
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06005F3D RID: 24381 RVA: 0x001E9A6A File Offset: 0x001E7C6A
		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06005F3E RID: 24382 RVA: 0x001E9A74 File Offset: 0x001E7C74
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState result;
				this.stateStack.TryPeek(ref result);
				return result;
			}
		}

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06005F3F RID: 24383 RVA: 0x001E9A90 File Offset: 0x001E7C90
		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06005F40 RID: 24384 RVA: 0x001E9AC7 File Offset: 0x001E7CC7
		// (set) Token: 0x06005F41 RID: 24385 RVA: 0x001E9ACF File Offset: 0x001E7CCF
		public bool NametagsEnabled { get; private set; }

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06005F42 RID: 24386 RVA: 0x001E9AD8 File Offset: 0x001E7CD8
		// (set) Token: 0x06005F43 RID: 24387 RVA: 0x001E9AE0 File Offset: 0x001E7CE0
		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06005F44 RID: 24388 RVA: 0x001E9AEF File Offset: 0x001E7CEF
		// (set) Token: 0x06005F45 RID: 24389 RVA: 0x001E9AF7 File Offset: 0x001E7CF7
		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		// Token: 0x06005F46 RID: 24390 RVA: 0x001E9B00 File Offset: 0x001E7D00
		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			Debug.Log(string.Concat(new string[]
			{
				"==== GORILLA TAG - VERSION: ",
				this.version,
				", BUILD NUMBER: ",
				this.buildCode,
				", BUILD DATE: ",
				this.buildDate,
				" ====\r\n\r\n      ___   ___\r\n     /   ---   \\\r\n    C|  @   @  |D\r\n      \\  . .  /\r\n       |     |\r\n       | _._ |\r\n       \\_____/\r\n\r\n\r\n"
			}));
			this._activeOrderList = this.OrderList;
			this.defaultUpdateCooldown = this.updateCooldown;
		}

		// Token: 0x06005F47 RID: 24391 RVA: 0x001E9BA9 File Offset: 0x001E7DA9
		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		// Token: 0x06005F48 RID: 24392 RVA: 0x001E9BBB File Offset: 0x001E7DBB
		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005F49 RID: 24393 RVA: 0x001E9BE6 File Offset: 0x001E7DE6
		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005F4A RID: 24394 RVA: 0x001E9C11 File Offset: 0x001E7E11
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x001E9C44 File Offset: 0x001E7E44
		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.time < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
				{
					this.deltaTime = Time.time - this.lastUpdateTime;
					this.lastUpdateTime = Time.time;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.time;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				string defaultResult = "NO WIFI OR LAN CONNECTION DETECTED.";
				string failMessage;
				LocalisationManager.TryGetKeyForCurrentLocale("NO_CONNECTION", out failMessage, defaultResult);
				this.UpdateFailureText(failMessage);
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.time > this.lastUpdateTime + this.updateCooldown)
			{
				this.deltaTime = Time.time - this.lastUpdateTime;
				this.lastUpdateTime = Time.time;
				this.UpdateScreen();
			}
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x001E9D74 File Offset: 0x001E7F74
		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.LeftRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			LocalisationManager.RegisterOnLanguageChanged(delegate
			{
				this.RefreshFunctionNames();
				this.UpdateGameModeText();
			});
			this.RefreshFunctionNames();
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] array = new byte[]
			{
				Convert.ToByte(64)
			};
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(array);
			this.initialized = true;
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x001E9E5C File Offset: 0x001E805C
		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x001E9EB4 File Offset: 0x001E80B4
		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		// Token: 0x06005F4F RID: 24399 RVA: 0x001E9F04 File Offset: 0x001E8104
		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitialiseLanguageScreen();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x00002789 File Offset: 0x00000989
		private void InitializeStartupState()
		{
		}

		// Token: 0x06005F51 RID: 24401 RVA: 0x00002789 File Offset: 0x00000989
		private void InitializeRoomState()
		{
		}

		// Token: 0x06005F52 RID: 24402 RVA: 0x001E9F88 File Offset: 0x001E8188
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x06005F53 RID: 24403 RVA: 0x001EA054 File Offset: 0x001E8254
		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case 1:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = (@int > 0);
				}
				break;
			case 2:
				this.NametagsEnabled = (permissionDataByFeature.Enabled && @int > 0);
				break;
			case 3:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', 0);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', 0);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', 0);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
		}

		// Token: 0x06005F54 RID: 24404 RVA: 0x001EA220 File Offset: 0x001E8420
		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		// Token: 0x06005F55 RID: 24405 RVA: 0x001EA228 File Offset: 0x001E8428
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "OPEN MIC");
			if (this.pttType == "ALL CHAT")
			{
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06005F56 RID: 24406 RVA: 0x001EA27C File Offset: 0x001E847C
		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		// Token: 0x06005F57 RID: 24407 RVA: 0x001EA2C4 File Offset: 0x001E84C4
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = (PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1);
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x06005F58 RID: 24408 RVA: 0x001EA333 File Offset: 0x001E8533
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x06005F59 RID: 24409 RVA: 0x001EA36C File Offset: 0x001E856C
		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			if (!this.rememberTroopQueueState)
			{
				bool flag2 = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
				bool flag3 = this.currentQueue != "DEFAULT" && this.currentQueue != "COMPETITIVE" && this.currentQueue != "MINIGAMES";
				if (flag2 || flag3)
				{
					this.currentQueue = "DEFAULT";
					PlayerPrefs.SetInt("troopQueueActive", 0);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
					PlayerPrefs.Save();
				}
			}
			this.troopQueueActive = (PlayerPrefs.GetInt("troopQueueActive", 0) == 1);
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06005F5A RID: 24410 RVA: 0x001EA497 File Offset: 0x001E8697
		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		// Token: 0x06005F5B RID: 24411 RVA: 0x001EA4A8 File Offset: 0x001E86A8
		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string text2 = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case 1:
				if (string.IsNullOrEmpty(text))
				{
					text2 = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					text2 = text;
				}
				break;
			case 2:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					text2 = text;
				}
				else
				{
					text2 = "FALSE";
				}
				break;
			case 3:
				text2 = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", text2);
		}

		// Token: 0x06005F5C RID: 24412 RVA: 0x001EA54E File Offset: 0x001E874E
		public void InitializeGameMode(string gameMode)
		{
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(gameMode, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(gameMode);
			this.didInitializeGameMode = true;
		}

		// Token: 0x06005F5D RID: 24413 RVA: 0x001EA580 File Offset: 0x001E8780
		private void InitializeGameMode()
		{
			if (this.didInitializeGameMode)
			{
				return;
			}
			string text = PlayerPrefs.GetString("currentGameModePostSI", GameModeType.SuperInfect.ToString());
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.SuperInfect;
				text = GameModeType.SuperInfect.ToString();
			}
			if (gameModeType != GameModeType.Casual && gameModeType != GameModeType.Infection && gameModeType != GameModeType.HuntDown && gameModeType != GameModeType.Paintbrawl && gameModeType != GameModeType.Ambush && gameModeType != GameModeType.PropHunt && gameModeType != GameModeType.SuperInfect)
			{
				PlayerPrefs.SetString("currentGameModePostSI", GameModeType.SuperInfect.ToString());
				PlayerPrefs.Save();
				text = GameModeType.SuperInfect.ToString();
			}
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		// Token: 0x06005F5E RID: 24414 RVA: 0x00002789 File Offset: 0x00000989
		private void InitializeCreditsState()
		{
		}

		// Token: 0x06005F5F RID: 24415 RVA: 0x001EA658 File Offset: 0x001E8858
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06005F60 RID: 24416 RVA: 0x001EA667 File Offset: 0x001E8867
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06005F61 RID: 24417 RVA: 0x001EA670 File Offset: 0x001E8870
		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		// Token: 0x06005F62 RID: 24418 RVA: 0x001EA6C4 File Offset: 0x001E88C4
		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		// Token: 0x06005F63 RID: 24419 RVA: 0x001EA6CD File Offset: 0x001E88CD
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > 0;
		}

		// Token: 0x06005F64 RID: 24420 RVA: 0x001EA6D8 File Offset: 0x001E88D8
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				RuntimePlatform platform = Application.platform;
				this.SaveModAccountData();
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		// Token: 0x06005F65 RID: 24421 RVA: 0x001EA798 File Offset: 0x001E8998
		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", ref obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", ref obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", ref obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", ref obj) && ((JsonObject)obj).TryGetValue("TopTroops", ref obj))
			{
				this.topTroops.Clear();
				foreach (object obj2 in ((JsonArray)obj))
				{
					this.topTroops.Add(obj2.ToString());
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", ref obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		// Token: 0x06005F66 RID: 24422 RVA: 0x001EA8D4 File Offset: 0x001E8AD4
		public void SaveModAccountData()
		{
			string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
			if (File.Exists(path))
			{
				return;
			}
			GorillaServer.Instance.ReturnMyOculusHash(delegate(ExecuteFunctionResult result)
			{
				object obj;
				if (((JsonObject)result.FunctionResult).TryGetValue("oculusHash", ref obj))
				{
					StreamWriter streamWriter = new StreamWriter(path);
					streamWriter.Write(PlayFabAuthenticator.instance.GetPlayFabPlayerId() + "." + (string)obj);
					streamWriter.Close();
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			});
		}

		// Token: 0x06005F67 RID: 24423 RVA: 0x001EA944 File Offset: 0x001E8B44
		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Language:
					this.ProcessLanguageState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06005F68 RID: 24424 RVA: 0x001EAA8C File Offset: 0x001E8C8C
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			PlayerPrefs.SetString("currentGameModePostSI", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06005F69 RID: 24425 RVA: 0x001EAAF0 File Offset: 0x001E8CF0
		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		// Token: 0x06005F6A RID: 24426 RVA: 0x001EAB10 File Offset: 0x001E8D10
		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		// Token: 0x06005F6B RID: 24427 RVA: 0x001EAB24 File Offset: 0x001E8D24
		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], ref result);
			return result;
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x001EAB5C File Offset: 0x001E8D5C
		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(zone, ref result);
			return result;
		}

		// Token: 0x06005F6D RID: 24429 RVA: 0x001EAB7C File Offset: 0x001E8D7C
		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06005F6E RID: 24430 RVA: 0x001EABE4 File Offset: 0x001E8DE4
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.FriendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string text in this.networkController.FriendIDList)
					{
						Debug.Log("Friend ID:" + text);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby, null);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty, null);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		// Token: 0x06005F6F RID: 24431 RVA: 0x001EADD4 File Offset: 0x001E8FD4
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
			}
		}

		// Token: 0x06005F70 RID: 24432 RVA: 0x001EAE08 File Offset: 0x001E9008
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.currentComputerState == GorillaComputer.ComputerState.Mic && this.currentComputerState != newState)
			{
				this.updateCooldown = this.defaultUpdateCooldown;
			}
			else if (newState == GorillaComputer.ComputerState.Mic)
			{
				this.updateCooldown = this.micUpdateCooldown;
			}
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		// Token: 0x06005F71 RID: 24433 RVA: 0x001EAE94 File Offset: 0x001E9094
		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06005F72 RID: 24434 RVA: 0x001EAECD File Offset: 0x001E90CD
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06005F73 RID: 24435 RVA: 0x001EAEE3 File Offset: 0x001E90E3
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06005F74 RID: 24436 RVA: 0x001EAEEE File Offset: 0x001E90EE
		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005F75 RID: 24437 RVA: 0x001EAF04 File Offset: 0x001E9104
		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
						{
							this.redValue,
							this.greenValue,
							this.blueValue
						});
					}
				}
				return;
			}
			}
		}

		// Token: 0x06005F76 RID: 24438 RVA: 0x001EB064 File Offset: 0x001E9264
		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string text = this.currentName;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.currentName = text + text2;
					}
					break;
				}
			}
		}

		// Token: 0x06005F77 RID: 24439 RVA: 0x001EB178 File Offset: 0x001E9378
		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > 1) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > 1)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			case GorillaKeyboardBindings.option2:
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				break;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string text = this.roomToJoin;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.roomToJoin = text + text2;
				}
				break;
			}
		}

		// Token: 0x06005F78 RID: 24440 RVA: 0x001EB2D4 File Offset: 0x001E94D4
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaComputer.<DisconnectAfterDelay>d__360 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaComputer.<DisconnectAfterDelay>d__360>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x06005F79 RID: 24441 RVA: 0x001EB30C File Offset: 0x001E950C
		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		// Token: 0x06005F7A RID: 24442 RVA: 0x001EB36C File Offset: 0x001E956C
		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		// Token: 0x06005F7B RID: 24443 RVA: 0x001EB3F4 File Offset: 0x001E95F4
		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x001EB450 File Offset: 0x001E9650
		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x001EB4B9 File Offset: 0x001E96B9
		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x001EB4EC File Offset: 0x001E96EC
		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.time)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.time + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", ref obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x001EB56A File Offset: 0x001E976A
		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		// Token: 0x06005F80 RID: 24448 RVA: 0x001EB578 File Offset: 0x001E9778
		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06005F81 RID: 24449 RVA: 0x001EB5D4 File Offset: 0x001E97D4
		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		// Token: 0x06005F82 RID: 24450 RVA: 0x001EB5EB File Offset: 0x001E97EB
		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		// Token: 0x06005F83 RID: 24451 RVA: 0x001EB600 File Offset: 0x001E9800
		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		// Token: 0x06005F84 RID: 24452 RVA: 0x001EB650 File Offset: 0x001E9850
		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.groupMapJoin = "FOREST";
				this.groupMapJoinIndex = 0;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.two:
				this.groupMapJoin = "CAVE";
				this.groupMapJoinIndex = 1;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.three:
				this.groupMapJoin = "CANYON";
				this.groupMapJoinIndex = 2;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.four:
				this.groupMapJoin = "CITY";
				this.groupMapJoinIndex = 3;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.five:
				this.groupMapJoin = "CLOUDS";
				this.groupMapJoinIndex = 4;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		// Token: 0x06005F85 RID: 24453 RVA: 0x001EB7E0 File Offset: 0x001E99E0
		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string text = this.troopToJoin;
						string text2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							text2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							text2 = num.ToString();
						}
						this.troopToJoin = text + text2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005F86 RID: 24454 RVA: 0x001EB909 File Offset: 0x001E9B09
		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		// Token: 0x06005F87 RID: 24455 RVA: 0x0001FA1B File Offset: 0x0001DC1B
		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		// Token: 0x06005F88 RID: 24456 RVA: 0x001EB934 File Offset: 0x001E9B34
		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		// Token: 0x06005F89 RID: 24457 RVA: 0x001EB9A4 File Offset: 0x001E9BA4
		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		// Token: 0x06005F8A RID: 24458 RVA: 0x001EBA34 File Offset: 0x001E9C34
		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.option1)
			{
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				return;
			}
			this.disableParticles = true;
			PlayerPrefs.SetString("disableParticles", "TRUE");
			PlayerPrefs.Save();
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
		}

		// Token: 0x06005F8B RID: 24459 RVA: 0x001EBAD4 File Offset: 0x001E9CD4
		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		// Token: 0x06005F8C RID: 24460 RVA: 0x001EBAE7 File Offset: 0x001E9CE7
		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		// Token: 0x06005F8D RID: 24461 RVA: 0x001EBAF8 File Offset: 0x001E9CF8
		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string text = this.redemptionCode;
					string text2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						text2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						text2 = num.ToString();
					}
					this.redemptionCode = text + text2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		// Token: 0x06005F8E RID: 24462 RVA: 0x001EBBE4 File Offset: 0x001E9DE4
		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		// Token: 0x06005F8F RID: 24463 RVA: 0x001EBC70 File Offset: 0x001E9E70
		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				case GorillaComputer.ComputerState.Language:
					this.LanguageScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		// Token: 0x06005F90 RID: 24464 RVA: 0x001EBDA8 File Offset: 0x001E9FA8
		private void LoadingScreen()
		{
			GorillaComputer.<>c__DisplayClass386_0 CS$<>8__locals1 = new GorillaComputer.<>c__DisplayClass386_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.tmp = "LOADING";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out text, CS$<>8__locals1.tmp);
			this.screenText.Text = text;
			this.LoadingRoutine = base.StartCoroutine(CS$<>8__locals1.<LoadingScreen>g__LoadingScreenLocal|0());
		}

		// Token: 0x06005F91 RID: 24465 RVA: 0x001EBE00 File Offset: 0x001EA000
		private void NameWarningScreen()
		{
			string defaultResult = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN", out text, defaultResult);
			this.screenText.Text = text;
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				defaultResult = "\n\nPRESS ANY KEY TO CONTINUE";
				LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_CONFIRMATION", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text;
				return;
			}
			defaultResult = "\n\nTYPE 'YES' TO CONFIRM:";
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_TYPE_YES", out text, defaultResult);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text.TrailingSpace();
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += this.warningConfirmationInputString;
		}

		// Token: 0x06005F92 RID: 24466 RVA: 0x001EBEBC File Offset: 0x001EA0BC
		private void SupportScreen()
		{
			this.screenText.Text = "";
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				string key;
				if (!(text == "OCULUS PC"))
				{
					if (!(text == "STEAM"))
					{
						if (!(text == "PSVR"))
						{
							if (!(text == "PICO"))
							{
								if (!(text == "QUEST"))
								{
									key = "UNKNOWN_PLATFORM";
								}
								else
								{
									key = "PLATFORM_QUEST";
								}
							}
							else
							{
								key = "PLATFORM_PICO";
							}
						}
						else
						{
							key = "PLATFORM_PSVR";
						}
					}
					else
					{
						key = "PLATFORM_STEAM";
					}
				}
				else
				{
					key = "PLATFORM_OCULUS_PC";
				}
				string text3;
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text3, text);
				text = text3;
				string defaultResult = "SUPPORT";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text3, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text3;
				defaultResult = "\n\nPLAYERID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLAYERID", out text3, defaultResult);
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text = gorillaText2.Text + text3 + "   ";
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += PlayFabAuthenticator.instance.GetPlayFabPlayerId();
				defaultResult = "\nVERSION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_VERSION", out text3, defaultResult);
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text = gorillaText4.Text + text3 + "    ";
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += this.version.ToUpper();
				defaultResult = "\nPLATFORM";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLATFORM", out text3, defaultResult);
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text = gorillaText6.Text + text3 + "   ";
				GorillaText gorillaText7 = this.screenText;
				gorillaText7.Text += text;
				defaultResult = "\nBUILD DATE";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_BUILD_DATE", out text3, defaultResult);
				GorillaText gorillaText8 = this.screenText;
				gorillaText8.Text = gorillaText8.Text + text3 + " ";
				GorillaText gorillaText9 = this.screenText;
				gorillaText9.Text += this.buildDate;
				if (KIDManager.KidEnabled)
				{
					defaultResult = "\nk-ID ACCOUNT TYPE:";
					LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_KID_ACCOUNT_TYPE", out text3, defaultResult);
					GorillaText gorillaText10 = this.screenText;
					gorillaText10.Text += text3.TrailingSpace();
					GorillaText gorillaText11 = this.screenText;
					gorillaText11.Text += KIDManager.GetActiveAccountStatusNiceString().ToUpper();
					return;
				}
			}
			else
			{
				string defaultResult2 = "SUPPORT";
				string text4;
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text4, defaultResult2);
				GorillaText gorillaText12 = this.screenText;
				gorillaText12.Text += text4;
				defaultResult2 = "\n\nPRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL", out text4, defaultResult2);
				GorillaText gorillaText13 = this.screenText;
				gorillaText13.Text += text4;
				defaultResult2 = "\n\n\n\n<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER THAN ANOTHER AXIOM</color>";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL_WARNING", out text4, defaultResult2);
				GorillaText gorillaText14 = this.screenText;
				gorillaText14.Text += text4;
			}
		}

		// Token: 0x06005F93 RID: 24467 RVA: 0x001EC1E0 File Offset: 0x001EA3E0
		private void TimeScreen()
		{
			string defaultResult = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: {currentSetting}.\nTIME OF DAY: {currentTimeOfDay}.\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("TIME_SCREEN", out text, defaultResult);
			text = text.Replace("{currentSetting}", BetterDayNightManager.instance.currentSetting.ToString().ToUpper()).Replace("{currentTimeOfDay}", BetterDayNightManager.instance.currentTimeOfDay.ToUpper());
			this.screenText.Text = text;
		}

		// Token: 0x06005F94 RID: 24468 RVA: 0x001EC250 File Offset: 0x001EA450
		private void CreditsScreen()
		{
			this.screenText.Text = this.creditsView.GetScreenText();
		}

		// Token: 0x06005F95 RID: 24469 RVA: 0x001EC268 File Offset: 0x001EA468
		private void VisualsScreen()
		{
			string defaultResult = "UPDATE ITEMS SETTINGS.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Text = text.TrailingSpace();
			defaultResult = "PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_OPTIONS", out text, defaultResult);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
			defaultResult = "\n\nITEM PARTICLES ON:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_CURRENT", out text, defaultResult);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text.TrailingSpace();
			string text2 = this.disableParticles ? "FALSE" : "TRUE";
			LocalisationManager.TryGetKeyForCurrentLocale(text2, out text, text2);
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += text;
			defaultResult = "\nINSTRUMENT VOLUME:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_VOLUME", out text, defaultResult);
			GorillaText gorillaText4 = this.screenText;
			gorillaText4.Text += text.TrailingSpace();
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += Mathf.CeilToInt(this.instrumentVolume * 50f).ToString();
		}

		// Token: 0x06005F96 RID: 24470 RVA: 0x001EC38C File Offset: 0x001EA58C
		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				string defaultResult = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK.";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Text = text;
				defaultResult = "\nPRESS OPTION 1 = HUMAN VOICES.\nPRESS OPTION 2 = MONKE VOICES.";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_OPTIONS", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text;
				defaultResult = "\n\nVOICE TYPE:";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_CURRENT", out text, defaultResult);
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += text.TrailingSpace();
				string key = (this.voiceChatOn == "TRUE") ? "VOICE_OPTION_HUMAN" : ((this.voiceChatOn == "FALSE") ? "VOICE_OPTION_MONKE" : "VOICE_OPTION_OFF");
				defaultResult = ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += text;
				return;
			}
			if (permissionDataByFeature.ManagedBy == 3)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		// Token: 0x06005F97 RID: 24471 RVA: 0x001EC4CC File Offset: 0x001EA6CC
		private void AutomuteScreen()
		{
			string defaultResult = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Text = text;
			defaultResult = "\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_OPTIONS", out text, defaultResult);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
			defaultResult = "\n\nCURRENT AUTOMOD LEVEL: ";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_CURRENT", out text, defaultResult);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text.TrailingSpace();
			string key = "AUTOMOD_OFF";
			string text2 = this.autoMuteType;
			if (!(text2 == "OFF"))
			{
				if (!(text2 == "MODERATE"))
				{
					if (text2 == "AGGRESSIVE")
					{
						key = "AUTOMOD_AGGRESSIVE";
					}
				}
				else
				{
					key = "AUTOMOD_MODERATE";
				}
			}
			else
			{
				key = "AUTOMOD_OFF";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, this.autoMuteType);
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += text;
		}

		// Token: 0x06005F98 RID: 24472 RVA: 0x001EC5C8 File Offset: 0x001EA7C8
		private void GroupScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string text = "";
			string text2 = (this.allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper();
			string text3 = "";
			string defaultResult;
			if (this.allowedMapsToJoin.Length > 1)
			{
				defaultResult = "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_DESTINATIONS", out text, defaultResult);
				text3 = text;
			}
			defaultResult = "\n\nACTIVE ZONE WILL BE:";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ACTIVE_ZONES", out text, defaultResult);
			string text4 = text.TrailingSpace();
			text4 = text4 + text2 + text3;
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
				string text5 = "";
				if (selectedMapJoinTrigger.CanPartyJoin())
				{
					defaultResult = "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
					LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_CANNOT_JOIN", out text, defaultResult);
					text5 = text;
				}
				defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_PARTY", out text, defaultResult);
				this.screenText.Text = text;
				text4 += text5;
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text4;
				return;
			}
			defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_NOPARTY", out text, defaultResult);
			this.screenText.Text = text;
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text4;
		}

		// Token: 0x06005F99 RID: 24473 RVA: 0x001EC714 File Offset: 0x001EA914
		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == 3)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			bool flag = false;
			string text = "";
			if (Microphone.devices.Length == 0)
			{
				flag = true;
				text = "NO MICROPHONE DETECTED";
			}
			if (flag)
			{
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_MIC_DISABLED", out text2, "MIC DISABLED: ");
				this.screenText.Text = text2 + text;
				return;
			}
			string defaultResult = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.";
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_OPTIONS", out text3, defaultResult);
			this.screenText.Text = text3;
			defaultResult = "\n\nCURRENT MIC SETTING:";
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_CURRENT", out text3, defaultResult);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text3.TrailingSpace();
			string key = "";
			string text4 = this.pttType;
			if (!(text4 == "PUSH TO MUTE"))
			{
				if (!(text4 == "PUSH TO TALK"))
				{
					if (!(text4 == "OPEN MIC"))
					{
						if (text4 == "ALL CHAT")
						{
							key = "OPEN_MIC";
						}
					}
					else
					{
						key = "OPEN_MIC";
					}
				}
				else
				{
					key = "PUSH_TO_TALK_MIC";
				}
			}
			else
			{
				key = "PUSH_TO_MUTE_MIC";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text3, this.pttType);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text3;
			if (this.pttType == "PUSH TO MUTE")
			{
				defaultResult = "- MIC IS OPEN.\n- HOLD ANY FACE BUTTON TO MUTE.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP", out text3, defaultResult);
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += text3;
			}
			else if (this.pttType == "PUSH TO TALK")
			{
				defaultResult = "- MIC IS MUTED.\n- HOLD ANY FACE BUTTON TO TALK.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_TALK_TOOLTIP", out text3, defaultResult);
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += text3;
			}
			else
			{
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += "\n\n\n";
			}
			if (this.speakerLoudness == null)
			{
				this.speakerLoudness = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
			if (this.speakerLoudness != null)
			{
				float num = Mathf.Sqrt(this.speakerLoudness.LoudnessNormalized);
				Debug.Log("Loudness: " + num.ToString());
				if (num <= 0.01f)
				{
					this.micInputTestTimer += this.deltaTime;
				}
				else
				{
					this.micInputTestTimer = 0f;
				}
				if (this.pttType != "OPEN MIC")
				{
					bool flag2 = ControllerInputPoller.PrimaryButtonPress(5);
					bool flag3 = ControllerInputPoller.SecondaryButtonPress(5);
					bool flag4 = ControllerInputPoller.PrimaryButtonPress(4);
					bool flag5 = ControllerInputPoller.SecondaryButtonPress(4);
					bool flag6 = flag2 || flag3 || flag4 || flag5;
					if (flag6 && this.pttType == "PUSH TO MUTE")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text3, defaultResult);
						GorillaText gorillaText6 = this.screenText;
						gorillaText6.Text += text3;
						return;
					}
					if (!flag6 && this.pttType == "PUSH TO TALK")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text3, defaultResult);
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text += text3;
						return;
					}
				}
				if (this.micInputTestTimer >= this.micInputTestTimerThreshold)
				{
					defaultResult = "NO MIC INPUT DETECTED. CHECK MIC SETTINGS IN THE OPERATING SYSTEM.";
					LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_NO_MIC", out text3, defaultResult);
					GorillaText gorillaText8 = this.screenText;
					gorillaText8.Text += text3;
					return;
				}
				defaultResult = "INPUT TEST: ";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text3, defaultResult);
				GorillaText gorillaText9 = this.screenText;
				gorillaText9.Text += text3;
				for (int i = 0; i < Mathf.FloorToInt(num * 50f); i++)
				{
					GorillaText gorillaText10 = this.screenText;
					gorillaText10.Text += "|";
				}
			}
		}

		// Token: 0x06005F9A RID: 24474 RVA: 0x001ECAE8 File Offset: 0x001EACE8
		private void QueueScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string defaultResult = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN", out text, defaultResult);
			this.screenText.Text = text.TrailingSpace();
			if (this.allowedInCompetitive)
			{
				defaultResult = "COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN.";
				LocalisationManager.TryGetKeyForCurrentLocale("COMPETITIVE_DESC", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text.TrailingSpace();
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_ALL_QUEUES", out text, defaultResult);
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += text;
			}
			else
			{
				defaultResult = "BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY.";
				LocalisationManager.TryGetKeyForCurrentLocale("BEAT_OBSTACLE_COURSE", out text, defaultResult);
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += text.TrailingSpace();
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_DEFAULT_QUEUES", out text, defaultResult);
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += text;
			}
			defaultResult = "\n\nCURRENT QUEUE:";
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_QUEUE", out text, defaultResult);
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += text.TrailingSpace();
			string text2 = this.currentQueue;
			string key;
			if (!(text2 == "DEFAULT"))
			{
				if (text2 == "COMPETITIVE")
				{
					key = "COMPETITIVE_QUEUE";
					goto IL_16E;
				}
				if (text2 == "MINIGAMES")
				{
					key = "MINIGAMES_QUEUE";
					goto IL_16E;
				}
			}
			key = "DEFAULT_QUEUE";
			IL_16E:
			defaultResult = this.currentQueue;
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
			GorillaText gorillaText6 = this.screenText;
			gorillaText6.Text += text;
		}

		// Token: 0x06005F9B RID: 24475 RVA: 0x001ECC8C File Offset: 0x001EAE8C
		private void TroopScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Text = string.Empty;
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Text = text;
				if (!flag2)
				{
					defaultResult = " PRESS ENTER TO JOIN OR CREATE A TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INSTRUCTIONS", out text, defaultResult);
					GorillaText gorillaText = this.screenText;
					gorillaText.Text += text;
				}
			}
			defaultResult = "\n\nCURRENT TROOP: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_TROOP", out text, defaultResult);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text.TrailingSpace();
			if (flag2)
			{
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += this.troopName;
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						defaultResult = "\n  -IN TROOP QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_IN_QUEUE", out text, defaultResult);
						GorillaText gorillaText4 = this.screenText;
						gorillaText4.Text += text;
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							GorillaText gorillaText5 = this.screenText;
							gorillaText5.Text += text.TrailingSpace();
							GorillaText gorillaText6 = this.screenText;
							gorillaText6.Text += Mathf.Max(1, this.currentTroopPopulation).ToString();
						}
						defaultResult = "\n\nPRESS OPTION 2 FOR DEFAULT QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DEFAULT_QUEUE", out text, defaultResult);
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text += text;
					}
					else
					{
						defaultResult = "\n  -IN {currentQueue} QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_QUEUE", out text, defaultResult);
						string text2 = this.currentQueue;
						string key;
						if (!(text2 == "DEFAULT"))
						{
							if (text2 == "MINIGAMES")
							{
								key = "MINIGAMES_QUEUE";
								goto IL_24D;
							}
							if (text2 == "COMPETITIVE")
							{
								key = "COMPETITIVE_QUEUE";
								goto IL_24D;
							}
						}
						key = "DEFAULT_QUEUE";
						IL_24D:
						defaultResult = this.currentQueue;
						string text3;
						LocalisationManager.TryGetKeyForCurrentLocale(key, out text3, defaultResult);
						text = text.Replace("{currentQueue}", text3);
						GorillaText gorillaText8 = this.screenText;
						gorillaText8.Text += text;
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							GorillaText gorillaText9 = this.screenText;
							gorillaText9.Text += text.TrailingSpace();
							GorillaText gorillaText10 = this.screenText;
							gorillaText10.Text += Mathf.Max(1, this.currentTroopPopulation).ToString();
						}
						defaultResult = "\n\nPRESS OPTION 1 FOR TROOP QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_TROOP_QUEUE", out text, defaultResult);
						GorillaText gorillaText11 = this.screenText;
						gorillaText11.Text += text;
					}
					defaultResult = "\nPRESS OPTION 3 TO LEAVE YOUR TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_LEAVE", out text, defaultResult);
					GorillaText gorillaText12 = this.screenText;
					gorillaText12.Text += text;
				}
			}
			else
			{
				defaultResult = "-NOT IN TROOP-";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_NOT_IN_TROOP", out text, defaultResult);
				GorillaText gorillaText13 = this.screenText;
				gorillaText13.Text += text;
			}
			if (flag)
			{
				if (!flag2)
				{
					defaultResult = "\n\nTROOP TO JOIN: ";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_JOIN_TROOP", out text, defaultResult);
					GorillaText gorillaText14 = this.screenText;
					gorillaText14.Text += text.TrailingSpace();
					GorillaText gorillaText15 = this.screenText;
					gorillaText15.Text += this.troopToJoin;
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == 3 || permissionDataByFeature2.ManagedBy == 3)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		// Token: 0x06005F9C RID: 24476 RVA: 0x001ED088 File Offset: 0x001EB288
		private void TurnScreen()
		{
			string defaultResult = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING.";
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN", out text2, defaultResult);
			text += text2.TrailingSpace();
			defaultResult = "PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURNING_SPEED", out text2, defaultResult);
			text += text2;
			defaultResult = "\n CURRENT TURN TYPE: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_TYPE", out text2, defaultResult);
			text += text2;
			string key = "TURN_TYPE_NO_TURN";
			string turnType = GorillaSnapTurn.CachedSnapTurnRef.turnType;
			if (!(turnType == "SNAP"))
			{
				if (!(turnType == "SMOOTH"))
				{
					if (!(turnType == "NONE"))
					{
						Debug.LogError("[LOCALIZATION::GORILLA_COMPUTER::TURN] Could not match [" + GorillaSnapTurn.CachedSnapTurnRef.turnType + "] to any case. Defaulting to NO_TURN");
					}
					else
					{
						key = "TURN_TYPE_NO_TURN";
					}
				}
				else
				{
					key = "TURN_TYPE_SMOOTH_TURN";
				}
			}
			else
			{
				key = "TURN_TYPE_SNAP_TURN";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text2, GorillaSnapTurn.CachedSnapTurnRef.turnType);
			text += text2;
			defaultResult = "\nCURRENT TURN SPEED: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_SPEED", out text2, defaultResult);
			text += text2;
			text += GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
			this.screenText.Text = text;
		}

		// Token: 0x06005F9D RID: 24477 RVA: 0x001ED1C8 File Offset: 0x001EB3C8
		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				string defaultResult = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\n";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN", out text, defaultResult);
				this.screenText.Text = text;
				defaultResult = "CURRENT NAME: ";
				LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_NAME", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text.TrailingSpace();
				GorillaText gorillaText2 = this.screenText;
				gorillaText2.Text += this.savedName;
				if (this.NametagsEnabled)
				{
					defaultResult = "NEW NAME: ";
					LocalisationManager.TryGetKeyForCurrentLocale("NEW_NAME", out text, defaultResult);
					GorillaText gorillaText3 = this.screenText;
					gorillaText3.Text += text.TrailingSpace();
					GorillaText gorillaText4 = this.screenText;
					gorillaText4.Text += this.currentName;
				}
				defaultResult = "PRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: ";
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_TOGGLE_NAMETAGS", out text, defaultResult);
				string key = this.NametagsEnabled ? "ON_KEY" : "OFF_KEY";
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += text.TrailingSpace();
				defaultResult = (this.NametagsEnabled ? "ON" : "OFF");
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text += text;
				return;
			}
			if (permissionDataByFeature.ManagedBy == 3)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		// Token: 0x06005F9E RID: 24478 RVA: 0x001ED338 File Offset: 0x001EB538
		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == 1)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
				string text2;
				if (LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_MANAGED", out text2, text))
				{
					text = text2;
				}
			}
			string empty = string.Empty;
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_INTRO", out text3, "GORILLA OS\n\n");
			this.screenText.Text = text3;
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PLAYERS_ONLINE", out text3, "{playersOnline} PLAYERS ONLINE\n\n");
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text3.Replace("{playersOnline}", HowManyMonke.ThisMany.ToString());
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_USERS_BANNED", out text3, "{usersBanned} USERS BANNED YESTERDAY\n\n");
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += text3.Replace("{usersBanned}", this.usersBanned.ToString());
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PRESS_KEY", out text3, "PRESS ANY KEY TO BEGIN");
			GorillaText gorillaText4 = this.screenText;
			gorillaText4.Text += text3;
		}

		// Token: 0x06005F9F RID: 24479 RVA: 0x001ED44C File Offset: 0x001EB64C
		private void ColourScreen()
		{
			this.screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_SELECT_INTRO", out text, this.screenText.Text);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_RED", out text, this.screenText.Text);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += "\n\n";
			GorillaText gorillaText3 = this.screenText;
			gorillaText3.Text += text;
			GorillaText gorillaText4 = this.screenText;
			gorillaText4.Text = gorillaText4.Text + Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : "");
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_GREEN", out text, this.screenText.Text);
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += "\n\n";
			GorillaText gorillaText6 = this.screenText;
			gorillaText6.Text += text;
			GorillaText gorillaText7 = this.screenText;
			gorillaText7.Text = gorillaText7.Text + Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : "");
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_BLUE", out text, this.screenText.Text);
			GorillaText gorillaText8 = this.screenText;
			gorillaText8.Text += "\n\n";
			GorillaText gorillaText9 = this.screenText;
			gorillaText9.Text += text;
			GorillaText gorillaText10 = this.screenText;
			gorillaText10.Text = gorillaText10.Text + Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : "");
		}

		// Token: 0x06005FA0 RID: 24480 RVA: 0x001ED644 File Offset: 0x001EB844
		private void RoomScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2;
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer) && item;
			this.screenText.Text = "";
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE.";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_INTRO", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text.TrailingSpace();
			}
			defaultResult = "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_OPTION", out text, defaultResult);
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += text.TrailingSpace();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
				{
					defaultResult = "YOUR GROUP WILL TRAVEL WITH YOU.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_GROUP_TRAVEL", out text, defaultResult);
					GorillaText gorillaText3 = this.screenText;
					gorillaText3.Text += text.TrailingSpace();
				}
				else
				{
					defaultResult = "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_PARTY_WARNING", out text, defaultResult);
					GorillaText gorillaText4 = this.screenText;
					gorillaText4.Text += text;
				}
			}
			defaultResult = "\n\nCURRENT ROOM:";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TEXT_CURRENT_ROOM", out text, defaultResult);
			GorillaText gorillaText5 = this.screenText;
			gorillaText5.Text += text.TrailingSpace();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text += NetworkSystem.Instance.RoomName.TrailingSpace();
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text2 = (activeGameMode != null) ? activeGameMode.GameModeNameRoomLabel() : null;
					if (!string.IsNullOrEmpty(text2))
					{
						GorillaText gorillaText7 = this.screenText;
						gorillaText7.Text += text2;
					}
				}
				defaultResult = "\n\nPLAYERS IN ROOM:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_IN_ROOM", out text, defaultResult);
				GorillaText gorillaText8 = this.screenText;
				gorillaText8.Text += text.TrailingSpace();
				GorillaText gorillaText9 = this.screenText;
				gorillaText9.Text += NetworkSystem.Instance.RoomPlayerCount.ToString();
			}
			else
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				GorillaText gorillaText10 = this.screenText;
				gorillaText10.Text += text;
				defaultResult = "\n\nPLAYERS ONLINE:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_ONLINE", out text, defaultResult);
				GorillaText gorillaText11 = this.screenText;
				gorillaText11.Text += text.TrailingSpace();
				GorillaText gorillaText12 = this.screenText;
				gorillaText12.Text += HowManyMonke.ThisMany.ToString();
			}
			if (flag)
			{
				defaultResult = "\n\nROOM TO JOIN:";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TO_JOIN", out text, defaultResult);
				GorillaText gorillaText13 = this.screenText;
				gorillaText13.Text += text.TrailingSpace();
				GorillaText gorillaText14 = this.screenText;
				gorillaText14.Text += this.roomToJoin;
				if (this.roomFull)
				{
					defaultResult = "\n\nROOM FULL. JOIN ROOM FAILED.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_FULL", out text, defaultResult);
					GorillaText gorillaText15 = this.screenText;
					gorillaText15.Text += text;
					return;
				}
				if (this.roomNotAllowed)
				{
					defaultResult = "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_JOIN_NOT_ALLOWED", out text, defaultResult);
					GorillaText gorillaText16 = this.screenText;
					gorillaText16.Text += text;
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == 3 || permissionDataByFeature2.ManagedBy == 3)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		// Token: 0x06005FA1 RID: 24481 RVA: 0x001ED9F4 File Offset: 0x001EBBF4
		private void RedemptionScreen()
		{
			string defaultResult = "TYPE REDEMPTION CODE AND PRESS ENTER";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_INTRO", out text, defaultResult);
			this.screenText.Text = text;
			defaultResult = "\n\nCODE: " + this.redemptionCode;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_LABEL", out text, defaultResult);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text.TrailingSpace();
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text += this.redemptionCode;
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
			{
				defaultResult = "\n\nINVALID CODE";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_INVALID", out text, defaultResult);
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += text;
				return;
			}
			case GorillaComputer.RedemptionResult.Checking:
			{
				defaultResult = "\n\nVALIDATING...";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_VALIDATING", out text, defaultResult);
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += text;
				return;
			}
			case GorillaComputer.RedemptionResult.AlreadyUsed:
			{
				defaultResult = "\n\nCODE ALREADY CLAIMED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_USED", out text, defaultResult);
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += text;
				return;
			}
			case GorillaComputer.RedemptionResult.Success:
			{
				defaultResult = "\n\nSUCCESSFULLY CLAIMED!";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_SUCCESS", out text, defaultResult);
				GorillaText gorillaText6 = this.screenText;
				gorillaText6.Text += text;
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06005FA2 RID: 24482 RVA: 0x001EDB4C File Offset: 0x001EBD4C
		private void LimitedOnlineFunctionalityScreen()
		{
			string defaultResult = "NOT AVAILABLE IN RANKED PLAY";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("LIMITED_ONLINE_FUNC", out text, defaultResult);
			this.screenText.Text = text;
		}

		// Token: 0x06005FA3 RID: 24483 RVA: 0x001EDB7C File Offset: 0x001EBD7C
		private void UpdateGameModeText()
		{
			string defaultResult = "CURRENT MODE";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_MODE", out text, defaultResult);
			this.currentGameModeText.Value = text;
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				WatchableStringSO watchableStringSO = this.currentGameModeText;
				watchableStringSO.Value += text;
				return;
			}
			WatchableStringSO watchableStringSO2 = this.currentGameModeText;
			watchableStringSO2.Value = watchableStringSO2.Value + "\n" + GorillaGameManager.instance.GameModeName();
		}

		// Token: 0x06005FA4 RID: 24484 RVA: 0x001EDC13 File Offset: 0x001EBE13
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Text = this.GetOrderListForScreen(this.currentState);
		}

		// Token: 0x06005FA5 RID: 24485 RVA: 0x001EDC2C File Offset: 0x001EBE2C
		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadRoomName(nameToCheck);
		}

		// Token: 0x06005FA6 RID: 24486 RVA: 0x001EDC3B File Offset: 0x001EBE3B
		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadPlayerName(nameToCheck);
		}

		// Token: 0x06005FA7 RID: 24487 RVA: 0x001EDC4A File Offset: 0x001EBE4A
		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.CheckForBadTroopName(nameToCheck);
			}
		}

		// Token: 0x06005FA8 RID: 24488 RVA: 0x001EDC67 File Offset: 0x001EBE67
		private void CheckForBadRoomName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = true,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06005FA9 RID: 24489 RVA: 0x001EDCA7 File Offset: 0x001EBEA7
		private void CheckForBadPlayerName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06005FAA RID: 24490 RVA: 0x001EDCE7 File Offset: 0x001EBEE7
		private void CheckForBadTroopName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = true
			}, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x06005FAB RID: 24491 RVA: 0x001EDD28 File Offset: 0x001EBF28
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", ref obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						CustomMapManager.UnloadMap(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06005FAC RID: 24492 RVA: 0x001EDE50 File Offset: 0x001EC050
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", ref obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06005FAD RID: 24493 RVA: 0x001EDF8C File Offset: 0x001EC18C
		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", ref obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x001EE013 File Offset: 0x001EC213
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x001EE02C File Offset: 0x001EC22C
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string text in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(text) >= 0)
				{
					return false;
				}
			}
			foreach (string text2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(text2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x001EE0EC File Offset: 0x001EC2EC
		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x001EE13B File Offset: 0x001EC33B
		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x001EE16C File Offset: 0x001EC36C
		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x001EE19A File Offset: 0x001EC39A
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x001EE1BC File Offset: 0x001EC3BC
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == 1074)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == 1002)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x001EE414 File Offset: 0x001EC614
		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x001EE478 File Offset: 0x001EC678
		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x001EE4DC File Offset: 0x001EC6DC
		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState state;
			try
			{
				state = this._activeOrderList[index].State;
			}
			catch
			{
				state = this._activeOrderList[0].State;
			}
			return state;
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x001EE524 File Offset: 0x001EC724
		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x001EE558 File Offset: 0x001EC758
		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x001EE5C5 File Offset: 0x001EC7C5
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x001EE5F4 File Offset: 0x001EC7F4
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated.Invoke();
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x001EE65C File Offset: 0x001EC85C
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated.Invoke();
			}
			if (error.Error == 1074)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == 1002)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		// Token: 0x06005FBD RID: 24509 RVA: 0x001EE6EF File Offset: 0x001EC8EF
		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x001EE6F8 File Offset: 0x001EC8F8
		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001EE7D2 File Offset: 0x001EC9D2
		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x001EE7E0 File Offset: 0x001EC9E0
		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int num3 = l;
					functionNames[num3] += " ";
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x001EE972 File Offset: 0x001ECB72
		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == 3)
			{
				return;
			}
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously);
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x001EE984 File Offset: 0x001ECB84
		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, null).Item2)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
				KIDManager.SendOptInPermissions();
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x001EE9DD File Offset: 0x001ECBDD
		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x001EE9F0 File Offset: 0x001ECBF0
		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool isSafety = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(isSafety, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						text = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case 1:
				if (string.IsNullOrEmpty(text))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = text;
				}
				break;
			case 2:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(text))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = text;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case 3:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x001EEB1C File Offset: 0x001ECD1C
		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == 3)
			{
				return;
			}
			if (managedBy == 2)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = (setting && @int == 1);
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = (PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1);
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x001EEB78 File Offset: 0x001ECD78
		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06005FC7 RID: 24519 RVA: 0x001EEB8F File Offset: 0x001ECD8F
		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x001EEBA8 File Offset: 0x001ECDA8
		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action.Invoke(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int num = this.NametagsEnabled ? 1 : 0;
			PlayerPrefs.SetInt(this.NameTagPlayerPref, num);
			PlayerPrefs.Save();
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnLeftRoom()
		{
		}

		// Token: 0x06005FCF RID: 24527 RVA: 0x00002789 File Offset: 0x00000989
		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x001EEC73 File Offset: 0x001ECE73
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
			if (returnCode == 32765)
			{
				this.roomFull = true;
			}
		}

		// Token: 0x06005FD1 RID: 24529 RVA: 0x001EEC84 File Offset: 0x001ECE84
		public void SetInVirtualStump(bool inVirtualStump)
		{
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.virtualStumpRoomPrepend + this.roomToJoin) : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend, 5));
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x001EECC0 File Offset: 0x001ECEC0
		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x001EECC8 File Offset: 0x001ECEC8
		public void SetLimitOnlineScreens(bool isLimited)
		{
			this.limitOnlineScreens = isLimited;
			this.UpdateScreen();
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x001EECD7 File Offset: 0x001ECED7
		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x001EECEA File Offset: 0x001ECEEA
		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x001EECF3 File Offset: 0x001ECEF3
		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.time < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x001EED24 File Offset: 0x001ECF24
		private void UpdateSession()
		{
			GorillaComputer.<UpdateSession>d__468 <UpdateSession>d__;
			<UpdateSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateSession>d__.<>4__this = this;
			<UpdateSession>d__.<>1__state = -1;
			<UpdateSession>d__.<>t__builder.Start<GorillaComputer.<UpdateSession>d__468>(ref <UpdateSession>d__);
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x001EED5B File Offset: 0x001ECF5B
		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001EED69 File Offset: 0x001ECF69
		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		// Token: 0x06005FDA RID: 24538 RVA: 0x001EED80 File Offset: 0x001ECF80
		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			string defaultResult = "PARENT/GUARDIAN PERMISSION REQUIRED TO ";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PERMISSION_NEEDED", out text, defaultResult);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
			GorillaText gorillaText2 = this.screenText;
			gorillaText2.Text = gorillaText2.Text + featureDescription + "!";
			if (this._waitingForUpdatedSession)
			{
				defaultResult = "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_WAITING_PERMISSION", out text, defaultResult);
				GorillaText gorillaText3 = this.screenText;
				gorillaText3.Text += text;
				return true;
			}
			if (Time.time >= this._nextUpdateAttemptTime)
			{
				defaultResult = "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_REFRESH_PERMISSIONS", out text, defaultResult);
				GorillaText gorillaText4 = this.screenText;
				gorillaText4.Text += text;
			}
			else
			{
				defaultResult = "CHECK AGAIN IN {time} SECONDS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_CHECK_AGAIN_COOLDOWN", out text, defaultResult);
				text = text.Replace("{time}", ((int)(this._nextUpdateAttemptTime - Time.time)).ToString());
				GorillaText gorillaText5 = this.screenText;
				gorillaText5.Text += text;
			}
			return false;
		}

		// Token: 0x06005FDB RID: 24539 RVA: 0x001EEE90 File Offset: 0x001ED090
		private void ProhibitedMessage(string verb)
		{
			"\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PROHIBITED_MESSAGE", out text, "SET CUSTOM NICKNAMES");
			text = text.Replace("{verb}", verb);
			GorillaText gorillaText = this.screenText;
			gorillaText.Text += text;
		}

		// Token: 0x06005FDC RID: 24540 RVA: 0x001EEEE4 File Offset: 0x001ED0E4
		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_DISABLED", out text, defaultResult);
				this.screenText.Text = text;
				return;
			}
			this.screenText.Text = "";
			string defaultResult2 = "CREATE OR JOIN PRIVATE ROOMS";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out featureDescription, defaultResult2);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x001EEF4C File Offset: 0x001ED14C
		private void RoomScreen_KIdProhibited()
		{
			string defaultResult = "CREATE OR JOIN PRIVATE ROOMS";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		// Token: 0x06005FDE RID: 24542 RVA: 0x001EEF74 File Offset: 0x001ED174
		private void VoiceScreen_Permission()
		{
			string defaultResult = "VOICE TYPE: \"MONKE\"\n\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_CURRENT_VOICE", out text, defaultResult);
			this.screenText.Text = text;
			if (!KIDManager.KidEnabled)
			{
				defaultResult = "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_DISABLED", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text;
				return;
			}
			defaultResult = "ENABLE HUMAN VOICE CHAT";
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out text, defaultResult);
			this.GuardianConsentMessage("OPTION 3", text);
		}

		// Token: 0x06005FDF RID: 24543 RVA: 0x001EEFF8 File Offset: 0x001ED1F8
		private void VoiceScreen_KIdProhibited()
		{
			string defaultResult = "USE THE VOICE CHAT";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		// Token: 0x06005FE0 RID: 24544 RVA: 0x001EF020 File Offset: 0x001ED220
		private void MicScreen_Permission()
		{
			this.screenText.Text = "";
			string defaultResult = "ENABLE HUMAN VOICE CHAT";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out featureDescription, defaultResult);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x06005FE1 RID: 24545 RVA: 0x001EF05E File Offset: 0x001ED25E
		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		// Token: 0x06005FE2 RID: 24546 RVA: 0x001EF068 File Offset: 0x001ED268
		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_DISABLED", out text, defaultResult);
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text;
				return;
			}
			this.screenText.Text = "";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out featureDescription, "SET CUSTOM NICKNAMES");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x001EF0D8 File Offset: 0x001ED2D8
		private void NameScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out verb, "SET CUSTOM NICKNAMES");
			this.ProhibitedMessage(verb);
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x001EF100 File Offset: 0x001ED300
		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == 1) && managedBy != 3;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Name
			}, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case 1:
				if (showCustomNames)
				{
					this.NametagsEnabled = (@int == -1 || flag2);
				}
				else
				{
					this.NametagsEnabled = (@int != -1 && flag2);
				}
				break;
			case 2:
				this.NametagsEnabled = (showCustomNames && (flag2 || @int == -1));
				break;
			case 3:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action.Invoke(this.NametagsEnabled);
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x001EF1CC File Offset: 0x001ED3CC
		private void TroopScreen_Permission()
		{
			this.screenText.Text = "";
			if (!KIDManager.KidEnabled)
			{
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DISABLED", out text, "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW");
				GorillaText gorillaText = this.screenText;
				gorillaText.Text += text;
				return;
			}
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_DESC", out featureDescription, "JOIN TROOPS");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x06005FE6 RID: 24550 RVA: 0x001EF23C File Offset: 0x001ED43C
		private void TroopScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_PROHIBITED_VERB", out verb, "CREATE OR JOIN TROOPS");
			this.ProhibitedMessage(verb);
		}

		// Token: 0x06005FE7 RID: 24551 RVA: 0x001EF262 File Offset: 0x001ED462
		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		// Token: 0x06005FE8 RID: 24552 RVA: 0x001EF277 File Offset: 0x001ED477
		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x001EF2A0 File Offset: 0x001ED4A0
		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string text = (!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + text);
			if (activeAccountStatus == null)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Text = stringBuilder.ToString();
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (Enumerable.Contains<string>(this._interestedPermissionNames, allPermissionsData[i].Name))
				{
					string text2 = allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">";
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text2,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Text = stringBuilder.ToString();
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x001EF405 File Offset: 0x001ED605
		private string GetLocalisedLanguageScreen()
		{
			return this.GetLanguageScreenLocalisation();
		}

		// Token: 0x06005FEB RID: 24555 RVA: 0x001EF410 File Offset: 0x001ED610
		private void GetLangaugesList(ref string langStr)
		{
			this._languagesDisplaySB.Clear();
			int maxLength = 12;
			int num = 3;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, Locale> keyValuePair in LocalisationManager.GetAllBindings())
			{
				num2++;
				string text = LocalisationManager.LocaleToFriendlyString(keyValuePair.Value, false).ToUpper();
				string text2 = string.Format("{0}) {1}", keyValuePair.Key, text);
				stringBuilder.Append(text2);
				int remainingChars = this.GetRemainingChars(text, maxLength);
				stringBuilder.Append(' ', remainingChars);
				if (num2 >= num)
				{
					this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
					stringBuilder.Clear();
					num2 = 0;
				}
			}
			this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
			langStr = langStr + this._languagesDisplaySB.ToString() + "\n";
		}

		// Token: 0x06005FEC RID: 24556 RVA: 0x001EF514 File Offset: 0x001ED714
		private int GetRemainingChars(string value, int maxLength)
		{
			int result;
			if (value == "日本語")
			{
				result = ((LocalisationManager.CurrentLanguage.Identifier.Code == "ja") ? 7 : 7);
			}
			else
			{
				result = Mathf.Clamp(maxLength - value.Length, 0, maxLength);
			}
			return result;
		}

		// Token: 0x06005FED RID: 24557 RVA: 0x001EF568 File Offset: 0x001ED768
		private string GetLanguageScreenLocalisation()
		{
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_TITLE", out text2, "CHOOSE YOUR LANGUAGE\n");
			text += text2;
			this.GetLangaugesList(ref text);
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_INSTRUCTIONS", out text2, "PRESS NUMBER KEYS TO CHOOSE A LANGUAGE\n");
			text += text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_CURRENT_LANGUAGE", out text2, "CURRENT LANGUAGE: ");
			text = text + text2.TrailingSpace() + LocalisationManager.LocaleToFriendlyString(null, false).ToUpper();
			return text;
		}

		// Token: 0x06005FEE RID: 24558 RVA: 0x001EF5E3 File Offset: 0x001ED7E3
		private void InitialiseLanguageScreen()
		{
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x06005FEF RID: 24559 RVA: 0x001EF601 File Offset: 0x001ED801
		private void LanguageScreen()
		{
			this.screenText.Text = this.GetLocalisedLanguageScreen();
		}

		// Token: 0x06005FF0 RID: 24560 RVA: 0x001EF614 File Offset: 0x001ED814
		private void ProcessLanguageState(GorillaKeyboardBindings buttonPressed)
		{
			int binding;
			if (!buttonPressed.FromNumberBindingToInt(out binding))
			{
				return;
			}
			Locale locale;
			if (!LocalisationManager.TryGetLocaleBinding(binding, out locale))
			{
				return;
			}
			LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, true);
			this.RefreshFunctionNames();
		}

		// Token: 0x06005FF1 RID: 24561 RVA: 0x001EF658 File Offset: 0x001ED858
		private void OnLanguageChanged()
		{
			if (this._previousLocalisationSetting == LocalisationManager.CurrentLanguage)
			{
				Debug.Log("[LOCALISATION::GORILLA_COMPUTER] Language changed, but no different to previous setting [" + this._previousLocalisationSetting.ToString() + "]");
				return;
			}
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			this.RefreshFunctionNames();
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x001EF6A8 File Offset: 0x001ED8A8
		private void RefreshFunctionNames()
		{
			this.FunctionNames.Clear();
			this.FunctionsCount = this.OrderList.Count;
			this.highestCharacterCount = int.MinValue;
			this.OrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				int num = this.highestCharacterCount - this.FunctionNames[i].Length;
				for (int j = 0; j < num; j++)
				{
					List<string> functionNames = this.FunctionNames;
					int num2 = i;
					functionNames[num2] += " ";
				}
			}
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x001EF750 File Offset: 0x001ED950
		public GorillaComputer()
		{
			List<GorillaComputer.StateOrderItem> list = new List<GorillaComputer.StateOrderItem>();
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Language, "Lang"));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits));
			list.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support));
			this.OrderList = list;
			this.Pointer = "<-";
			this.FunctionNames = new List<string>();
			this.micInputTestTimerThreshold = 10f;
			this.primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();
			this._filteredStates = new List<GorillaComputer.ComputerState>();
			this._activeOrderList = new List<GorillaComputer.StateOrderItem>();
			this.stateStack = new Stack<GorillaComputer.ComputerState>();
			this.warningConfirmationInputString = string.Empty;
			this.redemptionCode = "";
			this.virtualStumpRoomPrepend = "";
			this.waitOneSecond = new WaitForSeconds(1f);
			this.topTroops = new List<string>();
			this.currentTroopPopulation = -1;
			this.checkIfDisconnectedSeconds = 10f;
			this.checkIfConnectedSeconds = 1f;
			this.troopPopulationCheckCooldown = 3f;
			this._updateAttemptCooldown = 15f;
			this._currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;
			this._interestedPermissionNames = new string[]
			{
				"custom-username",
				"voice-chat",
				"join-groups"
			};
			this._languagesDisplaySB = new StringBuilder();
			base..ctor();
		}

		// Token: 0x04006D37 RID: 27959
		private const string VERSION_MISMATCH_KEY = "VERSION_MISMATCH";

		// Token: 0x04006D38 RID: 27960
		private const string CONNECTION_ISSUE_KEY = "CONNECTION_ISSUE";

		// Token: 0x04006D39 RID: 27961
		private const string NO_CONNECTION_KEY = "NO_CONNECTION";

		// Token: 0x04006D3A RID: 27962
		private const string STARTUP_INTRO_KEY = "STARTUP_INTRO";

		// Token: 0x04006D3B RID: 27963
		private const string STARTUP_PLAYERS_ONLINE_KEY = "STARTUP_PLAYERS_ONLINE";

		// Token: 0x04006D3C RID: 27964
		private const string STARTUP_USERS_BANNED_KEY = "STARTUP_USERS_BANNED";

		// Token: 0x04006D3D RID: 27965
		private const string STARTUP_PRESS_KEY_KEY = "STARTUP_PRESS_KEY";

		// Token: 0x04006D3E RID: 27966
		private const string STARTUP_PRESS_KEY_SHORT_KEY = "STARTUP_PRESS_KEY_SHORT";

		// Token: 0x04006D3F RID: 27967
		private const string STARTUP_MANAGED_KEY = "STARTUP_MANAGED";

		// Token: 0x04006D40 RID: 27968
		private const string COLOR_SELECT_INTRO_KEY = "COLOR_SELECT_INTRO";

		// Token: 0x04006D41 RID: 27969
		private const string CURRENT_SELECTED_LANGUAGE_KEY = "CURRENT_SELECTED_LANGUAGE";

		// Token: 0x04006D42 RID: 27970
		private const string CHANGE_TO_KEY = "CHANGE_TO";

		// Token: 0x04006D43 RID: 27971
		private const string CONFIRM_LANGUAGE_KEY = "CONFIRM_LANGUAGE";

		// Token: 0x04006D44 RID: 27972
		private const string COLOR_RED_KEY = "COLOR_RED";

		// Token: 0x04006D45 RID: 27973
		private const string COLOR_GREEN_KEY = "COLOR_GREEN";

		// Token: 0x04006D46 RID: 27974
		private const string COLOR_BLUE_KEY = "COLOR_BLUE";

		// Token: 0x04006D47 RID: 27975
		private const string ROOM_INTRO_KEY = "ROOM_INTRO";

		// Token: 0x04006D48 RID: 27976
		private const string ROOM_OPTION_KEY = "ROOM_OPTION";

		// Token: 0x04006D49 RID: 27977
		private const string ROOM_TEXT_CURRENT_ROOM_KEY = "ROOM_TEXT_CURRENT_ROOM";

		// Token: 0x04006D4A RID: 27978
		private const string PLAYERS_IN_ROOM_KEY = "PLAYERS_IN_ROOM";

		// Token: 0x04006D4B RID: 27979
		private const string NOT_IN_ROOM_KEY = "NOT_IN_ROOM";

		// Token: 0x04006D4C RID: 27980
		private const string PLAYERS_ONLINE_KEY = "PLAYERS_ONLINE";

		// Token: 0x04006D4D RID: 27981
		private const string ROOM_TO_JOIN_KEY = "ROOM_TO_JOIN";

		// Token: 0x04006D4E RID: 27982
		private const string ROOM_FULL_KEY = "ROOM_FULL";

		// Token: 0x04006D4F RID: 27983
		private const string ROOM_JOIN_NOT_ALLOWED_KEY = "ROOM_JOIN_NOT_ALLOWED";

		// Token: 0x04006D50 RID: 27984
		private const string LANGUAGE_KEY = "LANGUAGE";

		// Token: 0x04006D51 RID: 27985
		private const string NAME_SCREEN_KEY = "NAME_SCREEN";

		// Token: 0x04006D52 RID: 27986
		private const string CURRENT_NAME_KEY = "CURRENT_NAME";

		// Token: 0x04006D53 RID: 27987
		private const string NEW_NAME_KEY = "NEW_NAME";

		// Token: 0x04006D54 RID: 27988
		private const string TURN_SCREEN_KEY = "TURN_SCREEN";

		// Token: 0x04006D55 RID: 27989
		private const string TURN_SCREEN_TURNING_SPEED_KEY = "TURN_SCREEN_TURNING_SPEED";

		// Token: 0x04006D56 RID: 27990
		private const string TURN_SCREEN_TURN_TYPE_KEY = "TURN_SCREEN_TURN_TYPE";

		// Token: 0x04006D57 RID: 27991
		private const string TURN_SCREEN_TURN_SPEED_KEY = "TURN_SCREEN_TURN_SPEED";

		// Token: 0x04006D58 RID: 27992
		private const string TURN_TYPE_SNAP_TURN_KEY = "TURN_TYPE_SNAP_TURN";

		// Token: 0x04006D59 RID: 27993
		private const string TURN_TYPE_SMOOTH_TURN_KEY = "TURN_TYPE_SMOOTH_TURN";

		// Token: 0x04006D5A RID: 27994
		private const string TURN_TYPE_NO_TURN_KEY = "TURN_TYPE_NO_TURN";

		// Token: 0x04006D5B RID: 27995
		private const string QUEUE_SCREEN_KEY = "QUEUE_SCREEN";

		// Token: 0x04006D5C RID: 27996
		private const string BEAT_OBSTACLE_COURSE_KEY = "BEAT_OBSTACLE_COURSE";

		// Token: 0x04006D5D RID: 27997
		private const string COMPETITIVE_DESC_KEY = "COMPETITIVE_DESC";

		// Token: 0x04006D5E RID: 27998
		private const string QUEUE_SCREEN_ALL_QUEUES_KEY = "QUEUE_SCREEN_ALL_QUEUES";

		// Token: 0x04006D5F RID: 27999
		private const string QUEUE_SCREEN_DEFAULT_QUEUES_KEY = "QUEUE_SCREEN_DEFAULT_QUEUES";

		// Token: 0x04006D60 RID: 28000
		private const string CURRENT_QUEUE_KEY = "CURRENT_QUEUE";

		// Token: 0x04006D61 RID: 28001
		private const string DEFAULT_QUEUE_KEY = "DEFAULT_QUEUE";

		// Token: 0x04006D62 RID: 28002
		private const string MINIGAMES_QUEUE_KEY = "MINIGAMES_QUEUE";

		// Token: 0x04006D63 RID: 28003
		private const string COMPETITIVE_QUEUE_KEY = "COMPETITIVE_QUEUE";

		// Token: 0x04006D64 RID: 28004
		private const string MIC_SCREEN_INTRO_KEY = "MIC_SCREEN_INTRO";

		// Token: 0x04006D65 RID: 28005
		private const string MIC_SCREEN_OPTIONS_KEY = "MIC_SCREEN_OPTIONS";

		// Token: 0x04006D66 RID: 28006
		private const string MIC_SCREEN_CURRENT_KEY = "MIC_SCREEN_CURRENT";

		// Token: 0x04006D67 RID: 28007
		private const string MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP";

		// Token: 0x04006D68 RID: 28008
		private const string MIC_SCREEN_MIC_DISABLED_KEY = "MIC_SCREEN_MIC_DISABLED";

		// Token: 0x04006D69 RID: 28009
		private const string MIC_SCREEN_NO_MIC_KEY = "MIC_SCREEN_NO_MIC";

		// Token: 0x04006D6A RID: 28010
		private const string MIC_SCREEN_NO_PERMISSIONS_KEY = "MIC_SCREEN_NO_PERMISSIONS";

		// Token: 0x04006D6B RID: 28011
		private const string MIC_SCREEN_PUSH_TO_TALK_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_TALK_TOOLTIP";

		// Token: 0x04006D6C RID: 28012
		private const string MIC_SCREEN_INPUT_TEST_LABEL_KEY = "MIC_SCREEN_INPUT_TEST_LABEL";

		// Token: 0x04006D6D RID: 28013
		private const string MIC_SCREEN_INPUT_TEST_NO_MIC_KEY = "MIC_SCREEN_INPUT_TEST_NO_MIC";

		// Token: 0x04006D6E RID: 28014
		private const string ALL_CHAT_MIC_KEY = "ALL_CHAT_MIC";

		// Token: 0x04006D6F RID: 28015
		private const string PUSH_TO_TALK_MIC_KEY = "PUSH_TO_TALK_MIC";

		// Token: 0x04006D70 RID: 28016
		private const string PUSH_TO_MUTE_MIC_KEY = "PUSH_TO_MUTE_MIC";

		// Token: 0x04006D71 RID: 28017
		private const string OPEN_MIC_KEY = "OPEN_MIC";

		// Token: 0x04006D72 RID: 28018
		private const string AUTOMOD_SCREEN_INTRO_KEY = "AUTOMOD_SCREEN_INTRO";

		// Token: 0x04006D73 RID: 28019
		private const string AUTOMOD_SCREEN_OPTIONS_KEY = "AUTOMOD_SCREEN_OPTIONS";

		// Token: 0x04006D74 RID: 28020
		private const string AUTOMOD_SCREEN_CURRENT_KEY = "AUTOMOD_SCREEN_CURRENT";

		// Token: 0x04006D75 RID: 28021
		private const string AUTOMOD_AGGRESSIVE_KEY = "AUTOMOD_AGGRESSIVE";

		// Token: 0x04006D76 RID: 28022
		private const string AUTOMOD_MODERATE_KEY = "AUTOMOD_MODERATE";

		// Token: 0x04006D77 RID: 28023
		private const string AUTOMOD_OFF_KEY = "AUTOMOD_OFF";

		// Token: 0x04006D78 RID: 28024
		private const string VOICE_CHAT_SCREEN_INTRO_OLD_KEY = "VOICE_CHAT_SCREEN_INTRO_OLD";

		// Token: 0x04006D79 RID: 28025
		private const string VOICE_CHAT_SCREEN_OPTIONS_OLD_KEY = "VOICE_CHAT_SCREEN_OPTIONS_OLD";

		// Token: 0x04006D7A RID: 28026
		private const string VOICE_CHAT_SCREEN_CURRENT_OLD_KEY = "VOICE_CHAT_SCREEN_CURRENT_OLD";

		// Token: 0x04006D7B RID: 28027
		private const string TRUE_KEY = "TRUE";

		// Token: 0x04006D7C RID: 28028
		private const string FALSE_KEY = "FALSE";

		// Token: 0x04006D7D RID: 28029
		private const string VOICE_CHAT_SCREEN_INTRO_KEY = "VOICE_CHAT_SCREEN_INTRO";

		// Token: 0x04006D7E RID: 28030
		private const string VOICE_CHAT_SCREEN_OPTIONS_KEY = "VOICE_CHAT_SCREEN_OPTIONS";

		// Token: 0x04006D7F RID: 28031
		private const string VOICE_CHAT_SCREEN_CURRENT_KEY = "VOICE_CHAT_SCREEN_CURRENT";

		// Token: 0x04006D80 RID: 28032
		private const string VOICE_OPTION_HUMAN_KEY = "VOICE_OPTION_HUMAN";

		// Token: 0x04006D81 RID: 28033
		private const string VOICE_OPTION_MONKE_KEY = "VOICE_OPTION_MONKE";

		// Token: 0x04006D82 RID: 28034
		private const string VOICE_OPTION_OFF_KEY = "VOICE_OPTION_OFF";

		// Token: 0x04006D83 RID: 28035
		private const string VISUALS_SCREEN_INTRO_KEY = "VISUALS_SCREEN_INTRO";

		// Token: 0x04006D84 RID: 28036
		private const string VISUALS_SCREEN_OPTIONS_KEY = "VISUALS_SCREEN_OPTIONS";

		// Token: 0x04006D85 RID: 28037
		private const string VISUALS_SCREEN_CURRENT_KEY = "VISUALS_SCREEN_CURRENT";

		// Token: 0x04006D86 RID: 28038
		private const string VISUALS_SCREEN_VOLUME_KEY = "VISUALS_SCREEN_VOLUME";

		// Token: 0x04006D87 RID: 28039
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x04006D88 RID: 28040
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x04006D89 RID: 28041
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04006D8A RID: 28042
		private const string TIME_SCREEN_KEY = "TIME_SCREEN";

		// Token: 0x04006D8B RID: 28043
		private const string GROUP_SCREEN_LIMITED_OLD_KEY = "GROUP_SCREEN_LIMITED_OLD";

		// Token: 0x04006D8C RID: 28044
		private const string GROUP_SCREEN_FULL_OLD_KEY = "GROUP_SCREEN_FULL_OLD";

		// Token: 0x04006D8D RID: 28045
		private const string GROUP_SCREEN_SELECTION_OLD_KEY = "GROUP_SCREEN_SELECTION_OLD";

		// Token: 0x04006D8E RID: 28046
		private const string PLATFORM_STEAM_KEY = "PLATFORM_STEAM";

		// Token: 0x04006D8F RID: 28047
		private const string PLATFORM_QUEST_KEY = "PLATFORM_QUEST";

		// Token: 0x04006D90 RID: 28048
		private const string PLATFORM_PSVR_KEY = "PLATFORM_PSVR";

		// Token: 0x04006D91 RID: 28049
		private const string PLATFORM_PICO_KEY = "PLATFORM_PICO";

		// Token: 0x04006D92 RID: 28050
		private const string PLATFORM_OCULUS_PC_KEY = "PLATFORM_OCULUS_PC";

		// Token: 0x04006D93 RID: 28051
		private const string SUPPORT_SCREEN_INTRO_KEY = "SUPPORT_SCREEN_INTRO";

		// Token: 0x04006D94 RID: 28052
		private const string SUPPORT_SCREEN_DETAILS_PLAYERID_KEY = "SUPPORT_SCREEN_DETAILS_PLAYERID";

		// Token: 0x04006D95 RID: 28053
		private const string SUPPORT_SCREEN_DETAILS_VERSION_KEY = "SUPPORT_SCREEN_DETAILS_VERSION";

		// Token: 0x04006D96 RID: 28054
		private const string SUPPORT_SCREEN_DETAILS_PLATFORM_KEY = "SUPPORT_SCREEN_DETAILS_PLATFORM";

		// Token: 0x04006D97 RID: 28055
		private const string SUPPORT_SCREEN_DETAILS_BUILD_DATE_KEY = "SUPPORT_SCREEN_DETAILS_BUILD_DATE";

		// Token: 0x04006D98 RID: 28056
		private const string SUPPORT_SCREEN_INITIAL_KEY = "SUPPORT_SCREEN_INITIAL";

		// Token: 0x04006D99 RID: 28057
		private const string SUPPORT_SCREEN_INITIAL_WARNING_KEY = "SUPPORT_SCREEN_INITIAL_WARNING";

		// Token: 0x04006D9A RID: 28058
		private const string OCULUS_BUILD_CODE_KEY = "OCULUS_BUILD_CODE";

		// Token: 0x04006D9B RID: 28059
		private const string LOADING_SCREEN_KEY = "LOADING_SCREEN";

		// Token: 0x04006D9C RID: 28060
		private const string WARNING_SCREEN_KEY = "WARNING_SCREEN";

		// Token: 0x04006D9D RID: 28061
		private const string WARNING_SCREEN_CONFIRMATION_KEY = "WARNING_SCREEN_CONFIRMATION";

		// Token: 0x04006D9E RID: 28062
		private const string WARNING_SCREEN_TYPE_YES_KEY = "WARNING_SCREEN_TYPE_YES";

		// Token: 0x04006D9F RID: 28063
		private const string FUNCTION_ROOM_KEY = "FUNCTION_ROOM";

		// Token: 0x04006DA0 RID: 28064
		private const string FUNCTION_NAME_KEY = "FUNCTION_NAME";

		// Token: 0x04006DA1 RID: 28065
		private const string FUNCTION_COLOR_KEY = "FUNCTION_COLOR";

		// Token: 0x04006DA2 RID: 28066
		private const string FUNCTION_TURN_KEY = "FUNCTION_TURN";

		// Token: 0x04006DA3 RID: 28067
		private const string FUNCTION_MIC_KEY = "FUNCTION_MIC";

		// Token: 0x04006DA4 RID: 28068
		private const string FUNCTION_QUEUE_KEY = "FUNCTION_QUEUE";

		// Token: 0x04006DA5 RID: 28069
		private const string FUNCTION_GROUP_KEY = "FUNCTION_GROUP";

		// Token: 0x04006DA6 RID: 28070
		private const string FUNCTION_VOICE_KEY = "FUNCTION_VOICE";

		// Token: 0x04006DA7 RID: 28071
		private const string FUNCTION_AUTOMOD_KEY = "FUNCTION_AUTOMOD";

		// Token: 0x04006DA8 RID: 28072
		private const string FUNCTION_ITEMS_KEY = "FUNCTION_ITEMS";

		// Token: 0x04006DA9 RID: 28073
		private const string FUNCTION_CREDITS_KEY = "FUNCTION_CREDITS";

		// Token: 0x04006DAA RID: 28074
		private const string FUNCTION_LANGUAGE_KEY = "FUNCTION_LANGUAGE";

		// Token: 0x04006DAB RID: 28075
		private const string FUNCTION_SUPPORT_KEY = "FUNCTION_SUPPORT";

		// Token: 0x04006DAC RID: 28076
		private const string COMPUTER_KEYBOARD_DELETE_KEY = "COMPUTER_KEYBOARD_DELETE";

		// Token: 0x04006DAD RID: 28077
		private const string COMPUTER_KEYBOARD_ENTER_KEY = "COMPUTER_KEYBOARD_ENTER";

		// Token: 0x04006DAE RID: 28078
		private const string COMPUTER_KEYBOARD_OPTION1_KEY = "COMPUTER_KEYBOARD_OPTION1";

		// Token: 0x04006DAF RID: 28079
		private const string COMPUTER_KEYBOARD_OPTION2_KEY = "COMPUTER_KEYBOARD_OPTION2";

		// Token: 0x04006DB0 RID: 28080
		private const string COMPUTER_KEYBOARD_OPTION3_KEY = "COMPUTER_KEYBOARD_OPTION3";

		// Token: 0x04006DB1 RID: 28081
		private const string WARNING_SCREEN_YES_INPUT_KEY = "WARNING_SCREEN_YES_INPUT";

		// Token: 0x04006DB2 RID: 28082
		private const string GROUP_SCREEN_ENTER_PARTY_KEY = "GROUP_SCREEN_ENTER_PARTY";

		// Token: 0x04006DB3 RID: 28083
		private const string GROUP_SCREEN_ENTER_NOPARTY_KEY = "GROUP_SCREEN_ENTER_NOPARTY";

		// Token: 0x04006DB4 RID: 28084
		private const string GROUP_SCREEN_CANNOT_JOIN_KEY = "GROUP_SCREEN_CANNOT_JOIN";

		// Token: 0x04006DB5 RID: 28085
		private const string GROUP_SCREEN_ACTIVE_ZONES_KEY = "GROUP_SCREEN_ACTIVE_ZONES";

		// Token: 0x04006DB6 RID: 28086
		private const string GROUP_SCREEN_DESTINATIONS_KEY = "GROUP_SCREEN_DESTINATIONS";

		// Token: 0x04006DB7 RID: 28087
		private const string NAME_SCREEN_TOGGLE_NAMETAGS_KEY = "NAME_SCREEN_TOGGLE_NAMETAGS";

		// Token: 0x04006DB8 RID: 28088
		private const string NAME_SCREEN_KID_PROHIBITED_VERB_KEY = "NAME_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04006DB9 RID: 28089
		private const string NAME_SCREEN_DISABLED_KEY = "NAME_SCREEN_DISABLED";

		// Token: 0x04006DBA RID: 28090
		private const string ON_KEY = "ON_KEY";

		// Token: 0x04006DBB RID: 28091
		private const string OFF_KEY = "OFF_KEY";

		// Token: 0x04006DBC RID: 28092
		private const string KID_PROHIBITED_MESSAGE_KEY = "KID_PROHIBITED_MESSAGE";

		// Token: 0x04006DBD RID: 28093
		private const string KID_PERMISSION_NEEDED_KEY = "KID_PERMISSION_NEEDED";

		// Token: 0x04006DBE RID: 28094
		private const string KID_WAITING_PERMISSION_KEY = "KID_WAITING_PERMISSION";

		// Token: 0x04006DBF RID: 28095
		private const string KID_REFRESH_PERMISSIONS_KEY = "KID_REFRESH_PERMISSIONS";

		// Token: 0x04006DC0 RID: 28096
		private const string KID_CHECK_AGAIN_COOLDOWN_KEY = "KID_CHECK_AGAIN_COOLDOWN";

		// Token: 0x04006DC1 RID: 28097
		private const string STARTUP_TROOP_TEXT_KEY = "STARTUP_TROOP_TEXT";

		// Token: 0x04006DC2 RID: 28098
		private const string ROOM_GROUP_TRAVEL_KEY = "ROOM_GROUP_TRAVEL";

		// Token: 0x04006DC3 RID: 28099
		private const string ROOM_PARTY_WARNING_KEY = "ROOM_PARTY_WARNING";

		// Token: 0x04006DC4 RID: 28100
		private const string ROOM_GAME_LABEL_KEY = "ROOM_GAME_LABEL";

		// Token: 0x04006DC5 RID: 28101
		private const string ROOM_SCREEN_KID_PROHIBITED_VERB_KEY = "ROOM_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04006DC6 RID: 28102
		private const string ROOM_SCREEN_DISABLED_KEY = "ROOM_SCREEN_DISABLED";

		// Token: 0x04006DC7 RID: 28103
		private const string REDEMPTION_INTRO_KEY = "REDEMPTION_INTRO";

		// Token: 0x04006DC8 RID: 28104
		private const string REDEMPTION_CODE_LABEL_KEY = "REDEMPTION_CODE_LABEL";

		// Token: 0x04006DC9 RID: 28105
		private const string REDEMPTION_CODE_INVALID_KEY = "REDEMPTION_CODE_INVALID";

		// Token: 0x04006DCA RID: 28106
		private const string REDEMPTION_CODE_VALIDATING_KEY = "REDEMPTION_CODE_VALIDATING";

		// Token: 0x04006DCB RID: 28107
		private const string REDEMPTION_CODE_ALREADY_USED_KEY = "REDEMPTION_CODE_ALREADY_USED";

		// Token: 0x04006DCC RID: 28108
		private const string REDEMPTION_CODE_SUCCESS_KEY = "REDEMPTION_CODE_SUCCESS";

		// Token: 0x04006DCD RID: 28109
		private const string LIMITED_ONLINE_FUNC_KEY = "LIMITED_ONLINE_FUNC";

		// Token: 0x04006DCE RID: 28110
		private const string CURRENT_MODE_KEY = "CURRENT_MODE";

		// Token: 0x04006DCF RID: 28111
		private const string SUPPORT_META_ACCOUNT_TYPE_KEY = "SUPPORT_META_ACCOUNT_TYPE";

		// Token: 0x04006DD0 RID: 28112
		private const string SUPPORT_FINAL_QUEST_ONE_KEY = "SUPPORT_FINAL_QUEST_ONE";

		// Token: 0x04006DD1 RID: 28113
		private const string SUPPORT_KID_ACCOUNT_TYPE_KEY = "SUPPORT_KID_ACCOUNT_TYPE";

		// Token: 0x04006DD2 RID: 28114
		private const string VOICE_SCREEN_KID_PROHIBITED_VERB_KEY = "VOICE_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04006DD3 RID: 28115
		private const string VOICE_SCREEN_DISABLED_KEY = "VOICE_SCREEN_DISABLED";

		// Token: 0x04006DD4 RID: 28116
		private const string MIC_SCREEN_GUARDIAN_FEATURE_DESC_KEY = "VOICE_SCREEN_GUARDIAN_FEATURE_DESC";

		// Token: 0x04006DD5 RID: 28117
		private const string VOICE_SCREEN_KID_CURRENT_VOICE_KEY = "VOICE_SCREEN_KID_CURRENT_VOICE";

		// Token: 0x04006DD6 RID: 28118
		private const string MIC_SCREEN_PUSH_KEY_INSTRUCTIONS_KEY = "MIC_SCREEN_PUSH_KEY_INSTRUCTIONS";

		// Token: 0x04006DD7 RID: 28119
		private const string TROOP_SCREEN_INTRO_KEY = "TROOP_SCREEN_INTRO";

		// Token: 0x04006DD8 RID: 28120
		private const string TROOP_SCREEN_INSTRUCTIONS_KEY = "TROOP_SCREEN_INSTRUCTIONS";

		// Token: 0x04006DD9 RID: 28121
		private const string TROOP_SCREEN_CURRENT_TROOP_KEY = "TROOP_SCREEN_CURRENT_TROOP";

		// Token: 0x04006DDA RID: 28122
		private const string TROOP_SCREEN_IN_QUEUE_KEY = "TROOP_SCREEN_IN_QUEUE";

		// Token: 0x04006DDB RID: 28123
		private const string TROOP_SCREEN_PLAYERS_IN_TROOP_KEY = "TROOP_SCREEN_PLAYERS_IN_TROOP";

		// Token: 0x04006DDC RID: 28124
		private const string TROOP_SCREEN_DEFAULT_QUEUE_KEY = "TROOP_SCREEN_DEFAULT_QUEUE";

		// Token: 0x04006DDD RID: 28125
		private const string TROOP_SCREEN_CURRENT_QUEUE_KEY = "TROOP_SCREEN_CURRENT_QUEUE";

		// Token: 0x04006DDE RID: 28126
		private const string TROOP_SCREEN_TROOP_QUEUE_KEY = "TROOP_SCREEN_TROOP_QUEUE";

		// Token: 0x04006DDF RID: 28127
		private const string TROOP_SCREEN_LEAVE_KEY = "TROOP_SCREEN_LEAVE";

		// Token: 0x04006DE0 RID: 28128
		private const string TROOP_SCREEN_NOT_IN_TROOP_KEY = "TROOP_SCREEN_NOT_IN_TROOP";

		// Token: 0x04006DE1 RID: 28129
		private const string TROOP_SCREEN_JOIN_TROOP_KEY = "TROOP_SCREEN_JOIN_TROOP";

		// Token: 0x04006DE2 RID: 28130
		private const string TROOP_SCREEN_KID_PROHIBITED_VERB_KEY = "TROOP_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04006DE3 RID: 28131
		private const string TROOP_SCREEN_DISABLED_KEY = "TROOP_SCREEN_DISABLED";

		// Token: 0x04006DE4 RID: 28132
		private const string TROOP_SCREEN_KID_DESC_KEY = "TROOP_SCREEN_KID_DESC";

		// Token: 0x04006DE5 RID: 28133
		private const bool HIDE_SCREENS = false;

		// Token: 0x04006DE6 RID: 28134
		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		// Token: 0x04006DE7 RID: 28135
		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		// Token: 0x04006DE8 RID: 28136
		[OnEnterPlay_Set(false)]
		public static bool hasInstance;

		// Token: 0x04006DE9 RID: 28137
		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		// Token: 0x04006DEA RID: 28138
		public bool tryGetTimeAgain;

		// Token: 0x04006DEB RID: 28139
		public Material unpressedMaterial;

		// Token: 0x04006DEC RID: 28140
		public Material pressedMaterial;

		// Token: 0x04006DED RID: 28141
		public string currentTextField;

		// Token: 0x04006DEE RID: 28142
		public float buttonFadeTime;

		// Token: 0x04006DEF RID: 28143
		public string offlineTextInitialString;

		// Token: 0x04006DF0 RID: 28144
		public GorillaText screenText;

		// Token: 0x04006DF1 RID: 28145
		public GorillaText functionSelectText;

		// Token: 0x04006DF2 RID: 28146
		public GorillaText wallScreenText;

		// Token: 0x04006DF3 RID: 28147
		private Locale _lastLocaleChecked_Version;

		// Token: 0x04006DF4 RID: 28148
		private Locale _lastLocaleChecked_Connect;

		// Token: 0x04006DF5 RID: 28149
		private string _cachedVersionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x04006DF6 RID: 28150
		private string _cachedUnableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04006DF7 RID: 28151
		public Material wrongVersionMaterial;

		// Token: 0x04006DF8 RID: 28152
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04006DF9 RID: 28153
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04006DFA RID: 28154
		public long startupMillis;

		// Token: 0x04006DFB RID: 28155
		public DateTime startupTime;

		// Token: 0x04006DFC RID: 28156
		public string lastPressedGameMode;

		// Token: 0x04006DFD RID: 28157
		public WatchableStringSO currentGameMode;

		// Token: 0x04006DFE RID: 28158
		public WatchableStringSO currentGameModeText;

		// Token: 0x04006DFF RID: 28159
		public int includeUpdatedServerSynchTest;

		// Token: 0x04006E00 RID: 28160
		public PhotonNetworkController networkController;

		// Token: 0x04006E01 RID: 28161
		public float updateCooldown = 1f;

		// Token: 0x04006E02 RID: 28162
		private float defaultUpdateCooldown;

		// Token: 0x04006E03 RID: 28163
		private float micUpdateCooldown = 0.01f;

		// Token: 0x04006E04 RID: 28164
		public float lastUpdateTime;

		// Token: 0x04006E05 RID: 28165
		private float deltaTime;

		// Token: 0x04006E06 RID: 28166
		public bool isConnectedToMaster;

		// Token: 0x04006E07 RID: 28167
		public bool internetFailure;

		// Token: 0x04006E08 RID: 28168
		public string[] _allowedMapsToJoin;

		// Token: 0x04006E09 RID: 28169
		public bool limitOnlineScreens;

		// Token: 0x04006E0A RID: 28170
		[Header("State vars")]
		public bool stateUpdated;

		// Token: 0x04006E0B RID: 28171
		public bool screenChanged;

		// Token: 0x04006E0C RID: 28172
		public bool initialized;

		// Token: 0x04006E0D RID: 28173
		public List<GorillaComputer.StateOrderItem> OrderList;

		// Token: 0x04006E0E RID: 28174
		public string Pointer;

		// Token: 0x04006E0F RID: 28175
		public int highestCharacterCount;

		// Token: 0x04006E10 RID: 28176
		public List<string> FunctionNames;

		// Token: 0x04006E11 RID: 28177
		public int FunctionsCount;

		// Token: 0x04006E12 RID: 28178
		[Header("Room vars")]
		public string roomToJoin;

		// Token: 0x04006E13 RID: 28179
		public bool roomFull;

		// Token: 0x04006E14 RID: 28180
		public bool roomNotAllowed;

		// Token: 0x04006E15 RID: 28181
		[Header("Mic vars")]
		public string pttType;

		// Token: 0x04006E16 RID: 28182
		private GorillaSpeakerLoudness speakerLoudness;

		// Token: 0x04006E17 RID: 28183
		private float micInputTestTimer;

		// Token: 0x04006E18 RID: 28184
		public float micInputTestTimerThreshold;

		// Token: 0x04006E19 RID: 28185
		[Header("Automute vars")]
		public string autoMuteType;

		// Token: 0x04006E1A RID: 28186
		[Header("Queue vars")]
		public string currentQueue;

		// Token: 0x04006E1B RID: 28187
		public bool allowedInCompetitive;

		// Token: 0x04006E1C RID: 28188
		[Header("Group Vars")]
		public string groupMapJoin;

		// Token: 0x04006E1D RID: 28189
		public int groupMapJoinIndex;

		// Token: 0x04006E1E RID: 28190
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04006E1F RID: 28191
		[Header("Troop vars")]
		public string troopName;

		// Token: 0x04006E20 RID: 28192
		public bool troopQueueActive;

		// Token: 0x04006E21 RID: 28193
		public string troopToJoin;

		// Token: 0x04006E22 RID: 28194
		private bool rememberTroopQueueState;

		// Token: 0x04006E23 RID: 28195
		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone;

		// Token: 0x04006E24 RID: 28196
		public string voiceChatOn;

		// Token: 0x04006E25 RID: 28197
		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x04006E26 RID: 28198
		public string version;

		// Token: 0x04006E27 RID: 28199
		public string buildDate;

		// Token: 0x04006E28 RID: 28200
		public string buildCode;

		// Token: 0x04006E29 RID: 28201
		[Header("Cosmetics")]
		public bool disableParticles;

		// Token: 0x04006E2A RID: 28202
		public float instrumentVolume;

		// Token: 0x04006E2B RID: 28203
		[Header("Credits")]
		public CreditsView creditsView;

		// Token: 0x04006E2C RID: 28204
		[Header("Handedness")]
		public bool leftHanded;

		// Token: 0x04006E2D RID: 28205
		[Header("Name state vars")]
		public string savedName;

		// Token: 0x04006E2E RID: 28206
		public string currentName;

		// Token: 0x04006E2F RID: 28207
		public TextAsset exactOneWeekFile;

		// Token: 0x04006E30 RID: 28208
		public TextAsset anywhereOneWeekFile;

		// Token: 0x04006E31 RID: 28209
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x04006E32 RID: 28210
		private List<GorillaComputer.ComputerState> _filteredStates;

		// Token: 0x04006E33 RID: 28211
		private List<GorillaComputer.StateOrderItem> _activeOrderList;

		// Token: 0x04006E34 RID: 28212
		private Stack<GorillaComputer.ComputerState> stateStack;

		// Token: 0x04006E35 RID: 28213
		private GorillaComputer.ComputerState currentComputerState;

		// Token: 0x04006E36 RID: 28214
		private GorillaComputer.ComputerState previousComputerState;

		// Token: 0x04006E37 RID: 28215
		private int currentStateIndex;

		// Token: 0x04006E38 RID: 28216
		private int usersBanned;

		// Token: 0x04006E39 RID: 28217
		private float redValue;

		// Token: 0x04006E3A RID: 28218
		private string redText;

		// Token: 0x04006E3B RID: 28219
		private float blueValue;

		// Token: 0x04006E3C RID: 28220
		private string blueText;

		// Token: 0x04006E3D RID: 28221
		private float greenValue;

		// Token: 0x04006E3E RID: 28222
		private string greenText;

		// Token: 0x04006E3F RID: 28223
		private int colorCursorLine;

		// Token: 0x04006E40 RID: 28224
		private string warningConfirmationInputString;

		// Token: 0x04006E41 RID: 28225
		private bool displaySupport;

		// Token: 0x04006E42 RID: 28226
		private string[] exactOneWeek;

		// Token: 0x04006E43 RID: 28227
		private string[] anywhereOneWeek;

		// Token: 0x04006E44 RID: 28228
		private string[] anywhereTwoWeek;

		// Token: 0x04006E45 RID: 28229
		private GorillaComputer.RedemptionResult redemptionResult;

		// Token: 0x04006E46 RID: 28230
		private string redemptionCode;

		// Token: 0x04006E47 RID: 28231
		private bool playerInVirtualStump;

		// Token: 0x04006E48 RID: 28232
		private string virtualStumpRoomPrepend;

		// Token: 0x04006E49 RID: 28233
		private WaitForSeconds waitOneSecond;

		// Token: 0x04006E4A RID: 28234
		private Coroutine LoadingRoutine;

		// Token: 0x04006E4B RID: 28235
		private List<string> topTroops;

		// Token: 0x04006E4C RID: 28236
		private bool hasRequestedInitialTroopPopulation;

		// Token: 0x04006E4D RID: 28237
		private int currentTroopPopulation;

		// Token: 0x04006E4F RID: 28239
		private float lastCheckedWifi;

		// Token: 0x04006E50 RID: 28240
		private float checkIfDisconnectedSeconds;

		// Token: 0x04006E51 RID: 28241
		private float checkIfConnectedSeconds;

		// Token: 0x04006E52 RID: 28242
		private bool didInitializeGameMode;

		// Token: 0x04006E53 RID: 28243
		private float troopPopulationCheckCooldown;

		// Token: 0x04006E54 RID: 28244
		private float nextPopulationCheckTime;

		// Token: 0x04006E55 RID: 28245
		public Action OnServerTimeUpdated;

		// Token: 0x04006E56 RID: 28246
		private const string ENABLED_COLOUR = "#85ffa5";

		// Token: 0x04006E57 RID: 28247
		private const string DISABLED_COLOUR = "\"RED\"";

		// Token: 0x04006E58 RID: 28248
		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		// Token: 0x04006E59 RID: 28249
		private float _updateAttemptCooldown;

		// Token: 0x04006E5A RID: 28250
		private float _nextUpdateAttemptTime;

		// Token: 0x04006E5B RID: 28251
		private bool _waitingForUpdatedSession;

		// Token: 0x04006E5C RID: 28252
		private GorillaComputer.EKidScreenState _currentScreentState;

		// Token: 0x04006E5D RID: 28253
		private string[] _interestedPermissionNames;

		// Token: 0x04006E5E RID: 28254
		private const string LANG_SCREEN_TITLE_KEY = "LANG_SCREEN_TITLE";

		// Token: 0x04006E5F RID: 28255
		private const string LANG_SCREEN_INSTRUCTIONS_KEY = "LANG_SCREEN_INSTRUCTIONS";

		// Token: 0x04006E60 RID: 28256
		private const string LANG_SCREEN_CURRENT_LANGUAGE_KEY = "LANG_SCREEN_CURRENT_LANGUAGE";

		// Token: 0x04006E61 RID: 28257
		private StringBuilder _languagesDisplaySB;

		// Token: 0x04006E62 RID: 28258
		private Locale _previousLocalisationSetting;

		// Token: 0x02000EE4 RID: 3812
		public enum ComputerState
		{
			// Token: 0x04006E64 RID: 28260
			Startup,
			// Token: 0x04006E65 RID: 28261
			Color,
			// Token: 0x04006E66 RID: 28262
			Name,
			// Token: 0x04006E67 RID: 28263
			Turn,
			// Token: 0x04006E68 RID: 28264
			Mic,
			// Token: 0x04006E69 RID: 28265
			Room,
			// Token: 0x04006E6A RID: 28266
			Queue,
			// Token: 0x04006E6B RID: 28267
			Group,
			// Token: 0x04006E6C RID: 28268
			Voice,
			// Token: 0x04006E6D RID: 28269
			AutoMute,
			// Token: 0x04006E6E RID: 28270
			Credits,
			// Token: 0x04006E6F RID: 28271
			Visuals,
			// Token: 0x04006E70 RID: 28272
			Time,
			// Token: 0x04006E71 RID: 28273
			NameWarning,
			// Token: 0x04006E72 RID: 28274
			Loading,
			// Token: 0x04006E73 RID: 28275
			Support,
			// Token: 0x04006E74 RID: 28276
			Troop,
			// Token: 0x04006E75 RID: 28277
			KID,
			// Token: 0x04006E76 RID: 28278
			Redemption,
			// Token: 0x04006E77 RID: 28279
			Language
		}

		// Token: 0x02000EE5 RID: 3813
		private enum NameCheckResult
		{
			// Token: 0x04006E79 RID: 28281
			Success,
			// Token: 0x04006E7A RID: 28282
			Warning,
			// Token: 0x04006E7B RID: 28283
			Ban
		}

		// Token: 0x02000EE6 RID: 3814
		public enum RedemptionResult
		{
			// Token: 0x04006E7D RID: 28285
			Empty,
			// Token: 0x04006E7E RID: 28286
			Invalid,
			// Token: 0x04006E7F RID: 28287
			Checking,
			// Token: 0x04006E80 RID: 28288
			AlreadyUsed,
			// Token: 0x04006E81 RID: 28289
			Success
		}

		// Token: 0x02000EE7 RID: 3815
		[Serializable]
		public class StateOrderItem
		{
			// Token: 0x06005FF9 RID: 24569 RVA: 0x001EFA36 File Offset: 0x001EDC36
			public StateOrderItem()
			{
			}

			// Token: 0x06005FFA RID: 24570 RVA: 0x001EFA54 File Offset: 0x001EDC54
			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			// Token: 0x06005FFB RID: 24571 RVA: 0x001EFA79 File Offset: 0x001EDC79
			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			// Token: 0x06005FFC RID: 24572 RVA: 0x001EFAA8 File Offset: 0x001EDCA8
			public string GetName()
			{
				if (this._previousLocale == LocalizationSettings.SelectedLocale && !string.IsNullOrEmpty(this._cachedTranslation))
				{
					return this._cachedTranslation;
				}
				if (this.StringReference == null || this.StringReference.IsEmpty)
				{
					return this.GetPreLocalisedName();
				}
				this._previousLocale = LocalizationSettings.SelectedLocale;
				string localizedString = this.StringReference.GetLocalizedString();
				this._cachedTranslation = ((localizedString != null) ? localizedString.ToUpper() : null);
				if (string.IsNullOrEmpty(this._cachedTranslation))
				{
					if (LocalisationManager.ApplicationRunning)
					{
						string[] array = new string[5];
						array[0] = "[LOCALIZATION::STATE_ORDER_ITEM] Failed to get translation for selected locale [";
						int num = 1;
						Locale previousLocale = this._previousLocale;
						array[num] = (((previousLocale != null) ? previousLocale.LocaleName : null) ?? "NULL");
						array[2] = ", for item [";
						array[3] = this.State.GetName<GorillaComputer.ComputerState>();
						array[4] = "]";
						Debug.LogError(string.Concat(array));
					}
					this._cachedTranslation = "";
				}
				return this._cachedTranslation;
			}

			// Token: 0x06005FFD RID: 24573 RVA: 0x001EFB98 File Offset: 0x001EDD98
			public string GetPreLocalisedName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			// Token: 0x04006E82 RID: 28290
			public GorillaComputer.ComputerState State;

			// Token: 0x04006E83 RID: 28291
			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";

			// Token: 0x04006E84 RID: 28292
			public LocalizedString StringReference;

			// Token: 0x04006E85 RID: 28293
			private Locale _previousLocale;

			// Token: 0x04006E86 RID: 28294
			private string _cachedTranslation = "";
		}

		// Token: 0x02000EE8 RID: 3816
		private enum EKidScreenState
		{
			// Token: 0x04006E88 RID: 28296
			Ready,
			// Token: 0x04006E89 RID: 28297
			Show_OTP,
			// Token: 0x04006E8A RID: 28298
			Show_Setup_Screen
		}
	}
}
