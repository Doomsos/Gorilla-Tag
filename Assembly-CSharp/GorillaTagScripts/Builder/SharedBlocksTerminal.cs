using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E8E RID: 3726
	public class SharedBlocksTerminal : MonoBehaviour
	{
		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06005D1C RID: 23836 RVA: 0x001DE217 File Offset: 0x001DC417
		public SharedBlocksManager.SharedBlocksMap SelectedMap
		{
			get
			{
				return this.selectedMap;
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06005D1D RID: 23837 RVA: 0x001DE21F File Offset: 0x001DC41F
		public bool IsTerminalLocked
		{
			get
			{
				return this.isTerminalLocked;
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x06005D1E RID: 23838 RVA: 0x001DE227 File Offset: 0x001DC427
		private int playersInLobby
		{
			get
			{
				return this.lobbyTrigger.playerIDsCurrentlyTouching.Count;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x06005D1F RID: 23839 RVA: 0x001DE239 File Offset: 0x001DC439
		public bool IsDriver
		{
			get
			{
				return this.localState.driverID == NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x001DE257 File Offset: 0x001DC457
		public BuilderTable GetTable()
		{
			return this.linkedTable;
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06005D21 RID: 23841 RVA: 0x001DE25F File Offset: 0x001DC45F
		public int GetDriverID
		{
			get
			{
				return this.localState.driverID;
			}
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x001DE26C File Offset: 0x001DC46C
		public static string MapIDToDisplayedString(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return "____-____";
			}
			int num = 4;
			SharedBlocksTerminal.sb.Clear();
			if (mapID.Length > num)
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0, num));
				SharedBlocksTerminal.sb.Append("-");
				SharedBlocksTerminal.sb.Append(mapID.Substring(num));
				int num2 = 9 - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num2);
			}
			else
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0));
				int num3 = num - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', num3);
				SharedBlocksTerminal.sb.Append("-____");
			}
			return SharedBlocksTerminal.sb.ToString();
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x001DE338 File Offset: 0x001DC538
		public void Init(BuilderTable table)
		{
			if (this.hasInitialized)
			{
				return;
			}
			this.localState = new SharedBlocksTerminal.SharedBlocksTerminalState
			{
				state = SharedBlocksTerminal.TerminalState.NoStatus,
				driverID = -2
			};
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.AddListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			this.terminalControlButton.onPressButton.AddListener(new UnityAction(this.OnTerminalControlPressed));
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.linkedTable = table;
			table.linkedTerminal = this;
			this.linkedTable.OnMapLoaded.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
			this.linkedTable.OnMapLoadFailed.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
			this.linkedTable.OnMapCleared.AddListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnReturnedToSinglePlayer);
			this.hasInitialized = true;
		}

		// Token: 0x06005D24 RID: 23844 RVA: 0x001DE454 File Offset: 0x001DC654
		private void Start()
		{
			BuilderTable table;
			if (!this.hasInitialized && BuilderTable.TryGetBuilderTableForZone(this.tableZone, out table))
			{
				this.Init(table);
				return;
			}
			Debug.LogWarning("Could not find builder table for zone " + this.tableZone.ToString());
		}

		// Token: 0x06005D25 RID: 23845 RVA: 0x001DE4A0 File Offset: 0x001DC6A0
		private void LateUpdate()
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
			{
				return;
			}
			this.useNametags = GorillaComputer.instance.NametagsEnabled;
			this.RefreshDriverNickname();
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x001DE4FC File Offset: 0x001DC6FC
		private void OnDestroy()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.RemoveListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			if (this.terminalControlButton != null)
			{
				this.terminalControlButton.onPressButton.RemoveListener(new UnityAction(this.OnTerminalControlPressed));
			}
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.OnJoinedRoom);
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnReturnedToSinglePlayer);
			}
			if (this.linkedTable != null)
			{
				this.linkedTable.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
				this.linkedTable.OnMapLoadFailed.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
				this.linkedTable.OnMapCleared.RemoveListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			}
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x001DE5FC File Offset: 0x001DC7FC
		private void RefreshActiveScreen()
		{
			if (this.localState.driverID == -2)
			{
				if (this.currentScreen != this.noDriverScreen)
				{
					if (this.currentScreen != null)
					{
						this.currentScreen.Hide();
					}
					this.currentScreen = this.noDriverScreen;
					this.currentScreen.Show();
				}
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			if (this.currentScreen != this.searchScreen)
			{
				if (this.currentScreen != null)
				{
					this.currentScreen.Hide();
				}
				this.currentScreen = this.searchScreen;
				this.currentScreen.Show();
			}
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x001DE6B0 File Offset: 0x001DC8B0
		private void SetTerminalState(SharedBlocksTerminal.TerminalState state)
		{
			this.localState.state = state;
			string statusText = "";
			if (this.localState.driverID == -2)
			{
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			switch (state)
			{
			case SharedBlocksTerminal.TerminalState.NoStatus:
				this.statusMessageText.gameObject.SetActive(false);
				return;
			case SharedBlocksTerminal.TerminalState.Searching:
			{
				string defaultResult = "SEARCHING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_SEARCH", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_SEARCH]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.NotFound:
			{
				string defaultResult = "MAP NOT FOUND";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Found:
			{
				string defaultResult = "MAP FOUND. PRESS 'ENTER' TO LOAD";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Loading:
			{
				string defaultResult = "LOADING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadSuccess:
			{
				string defaultResult = "LOAD SUCCESS";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadFail:
			{
				string defaultResult = "LOAD FAILED";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED]");
				}
				this.SetStatusText(statusText);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x001DE817 File Offset: 0x001DCA17
		public void SelectMapIDAndOpenInfo(string mapID)
		{
			if (this.awaitingWebRequest)
			{
				return;
			}
			this.selectedMap = null;
			this.awaitingWebRequest = true;
			this.requestedMapID = mapID;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.OnPlayerMapRequestComplete));
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x001DE858 File Offset: 0x001DCA58
		private void OnPlayerMapRequestComplete(SharedBlocksManager.SharedBlocksMap response)
		{
			if (this.awaitingWebRequest)
			{
				this.awaitingWebRequest = false;
				this.requestedMapID = null;
				if (this.IsDriver)
				{
					if (response == null || response.MapID == null)
					{
						this.SetTerminalState(SharedBlocksTerminal.TerminalState.NotFound);
						return;
					}
					this.selectedMap = response;
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Found);
				}
			}
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x001DE8A4 File Offset: 0x001DCAA4
		private bool CanChangeMapState(bool load, out string disallowedReason)
		{
			disallowedReason = "";
			if (!NetworkSystem.Instance.InRoom)
			{
				disallowedReason = "MUST BE IN A ROOM BEFORE  " + (load ? "" : "UN") + "LOADING A MAP.";
				string text = load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD";
				string text2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out text2, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text + "]");
				}
				disallowedReason = text2;
				return false;
			}
			this.RefreshLobbyCount();
			if (!this.AreAllPlayersInLobby())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE LOBBY BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				string text3 = load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD";
				string text4;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text3, out text4, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text3 + "]");
				}
				disallowedReason = text4;
				return false;
			}
			return true;
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x001DE97F File Offset: 0x001DCB7F
		public void SetStatusText(string text)
		{
			this.statusMessageText.text = text;
			this.statusMessageText.gameObject.SetActive(true);
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001DE99E File Offset: 0x001DCB9E
		private bool IsLocalPlayerInLobby()
		{
			return base.isActiveAndEnabled && this.lobbyTrigger.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001DE9CE File Offset: 0x001DCBCE
		public bool AreAllPlayersInLobby()
		{
			return base.isActiveAndEnabled && this.playersInLobby == this.playersInRoom;
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001DE9E8 File Offset: 0x001DCBE8
		public string GetLobbyText()
		{
			string defaultResult = "PLAYERS IN ROOM {0}\nPLAYERS IN LOBBY {1}";
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT", out text, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT]");
			}
			return string.Format(text, this.playersInRoom, this.playersInLobby);
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001DEA30 File Offset: 0x001DCC30
		public void RefreshLobbyCount()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.playersInRoom = NetworkSystem.Instance.RoomPlayerCount;
				return;
			}
			this.playersInRoom = 0;
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x001DEA64 File Offset: 0x001DCC64
		public void PressButton(SharedBlocksKeyboardBindings buttonPressed)
		{
			if (!this.IsDriver)
			{
				string statusText;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out statusText, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(statusText);
				return;
			}
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.up)
			{
				this.OnUpButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.down)
			{
				this.OnDownButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.delete)
			{
				this.OnDeleteButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.enter)
			{
				this.OnSelectButtonPressed();
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.zero && buttonPressed <= SharedBlocksKeyboardBindings.nine)
			{
				this.OnNumberPressed((int)buttonPressed);
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.A && buttonPressed <= SharedBlocksKeyboardBindings.Z)
			{
				this.OnLetterPressed(buttonPressed.ToString());
			}
		}

		// Token: 0x06005D32 RID: 23858 RVA: 0x001DEB19 File Offset: 0x001DCD19
		private void OnUpButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnUpPressed();
			}
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x001DEB34 File Offset: 0x001DCD34
		private void OnDownButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDownPressed();
			}
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001DEB4F File Offset: 0x001DCD4F
		private void OnSelectButtonPressed()
		{
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Found)
			{
				this.OnLoadMapPressed();
				return;
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnSelectPressed();
			}
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001DEB7F File Offset: 0x001DCD7F
		private void OnDeleteButtonPressed()
		{
			if (this.localState.state != SharedBlocksTerminal.TerminalState.Loading && this.localState.state != SharedBlocksTerminal.TerminalState.Searching)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDeletePressed();
			}
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x00002789 File Offset: 0x00000989
		private void OnBackButtonPressed()
		{
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001DEBBD File Offset: 0x001DCDBD
		private void OnNumberPressed(int number)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnNumberPressed(number);
			}
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x001DEBD9 File Offset: 0x001DCDD9
		private void OnLetterPressed(string letter)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnLetterPressed(letter);
			}
		}

		// Token: 0x06005D39 RID: 23865 RVA: 0x001DEBF8 File Offset: 0x001DCDF8
		private void OnTerminalControlPressed()
		{
			if (this.isTerminalLocked)
			{
				if (this.IsDriver)
				{
					if (NetworkSystem.Instance.InRoom)
					{
						this.linkedTable.builderNetworking.RequestBlocksTerminalControl(false);
						return;
					}
					this.SetTerminalDriver(-2);
					return;
				}
			}
			else
			{
				if (NetworkSystem.Instance.InRoom)
				{
					this.linkedTable.builderNetworking.RequestBlocksTerminalControl(true);
					return;
				}
				this.SetTerminalDriver(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x001DEC70 File Offset: 0x001DCE70
		public void OnLoadMapPressed()
		{
			if (!this.IsDriver)
			{
				string statusText;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out statusText, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(statusText);
				return;
			}
			if (this.currentScreen == null || this.selectedMap == null)
			{
				string statusText2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION", out statusText2, "NO MAP SELECTED"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION]");
				}
				this.SetStatusText(statusText2);
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS", out text, "BLOCKS LOAD ALREADY IN PROGRESS"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS]");
				}
				this.SetStatusText("BLOCKS LOAD ALREADY IN PROGRESS");
				return;
			}
			string statusText3;
			if (!this.CanChangeMapState(true, out statusText3))
			{
				this.SetStatusText(statusText3);
				return;
			}
			if (this.linkedTable != null)
			{
				if (Time.time > this.lastLoadTime + this.loadMapCooldown)
				{
					string text2;
					if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out text2, "LOADING BLOCKS ..."))
					{
						Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
					}
					this.SetStatusText("LOADING BLOCKS ...");
					this.isLoadingMap = true;
					this.lastLoadTime = Time.time;
					this.linkedTable.LoadSharedMap(this.selectedMap);
					return;
				}
				int num = Mathf.RoundToInt(this.lastLoadTime + this.loadMapCooldown - Time.time);
				string defaultResult = string.Format("PLEASE WAIT {0} SECONDS BEFORE LOADING ANOTHER MAP", num);
				string text3;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_WAIT", out text3, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				text3 = text3.Replace("{time}", num.ToString());
				this.SetStatusText(text3);
			}
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x001DEE06 File Offset: 0x001DD006
		public bool IsPlayerDriver(Player player)
		{
			return player.ActorNumber == this.localState.driverID;
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001DEE1B File Offset: 0x001DD01B
		public bool ValidateTerminalControlRequest(bool locked, int playerNumber)
		{
			if (locked && playerNumber == -2)
			{
				return false;
			}
			if (this.localState.driverID == -2)
			{
				return locked;
			}
			return this.localState.driverID == playerNumber;
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001DEE46 File Offset: 0x001DD046
		private void OnDriverNameChanged()
		{
			this.RefreshDriverNickname();
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001DEE50 File Offset: 0x001DD050
		public void SetTerminalDriver(int playerNum)
		{
			if (playerNum != -2)
			{
				if (this.localState.driverID != -2 && this.localState.driverID != playerNum)
				{
					GTDev.LogWarning<string>(string.Format("Shared BlocksTerminal SetTerminalDriver cannot set {0} as driver while {1} is driver", playerNum, this.localState.driverID), null);
					return;
				}
				this.localState.driverID = playerNum;
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerNum);
				RigContainer rigContainer;
				if (netPlayerByID != null && VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					this.driverRig = rigContainer.Rig;
					this.driverRig.OnPlayerNameVisibleChanged += new Action(this.OnDriverNameChanged);
				}
				this.isTerminalLocked = true;
				this.UpdateTerminalButton();
				this.RefreshActiveScreen();
				this.searchScreen.SetInputTextEnabled(this.IsDriver);
				if (this.IsDriver && this.awaitingWebRequest)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
					this.searchScreen.SetMapCode(this.requestedMapID);
				}
				else if (this.isLoadingMap)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
					this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				}
				else
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				}
			}
			else
			{
				if (this.driverRig != null)
				{
					this.driverRig.OnPlayerNameVisibleChanged -= new Action(this.OnDriverNameChanged);
					this.driverRig = null;
				}
				this.localState.driverID = -2;
				this.isTerminalLocked = false;
				this.UpdateTerminalButton();
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				this.RefreshActiveScreen();
			}
			this.RefreshDriverNickname();
		}

		// Token: 0x06005D3F RID: 23871 RVA: 0x001DEFD0 File Offset: 0x001DD1D0
		private void RefreshDriverNickname()
		{
			StringVariable stringVariable = this._currentDriverLoc.StringReference["playerName"] as StringVariable;
			if (this.localState.driverID == -2)
			{
				this.currentDriverLabel.gameObject.SetActive(false);
				stringVariable.Value = "";
				this.currentDriverText.text = "";
				this.currentDriverText.gameObject.SetActive(false);
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			if (NetworkSystem.Instance.InRoom)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(this.localState.driverID);
				if (player != null && this.useNametags && flag)
				{
					RigContainer rigContainer;
					if (player.IsLocal)
					{
						stringVariable.Value = player.NickName;
						this.currentDriverText.text = player.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						stringVariable.Value = rigContainer.Rig.playerNameVisible;
						this.currentDriverText.text = rigContainer.Rig.playerNameVisible;
					}
					else
					{
						stringVariable.Value = player.DefaultName;
						this.currentDriverText.text = player.DefaultName;
					}
				}
				else
				{
					stringVariable.Value = "";
					this.currentDriverText.text = "";
				}
			}
			else
			{
				stringVariable.Value = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
				this.currentDriverText.text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
			}
			this.currentDriverLabel.gameObject.SetActive(true);
		}

		// Token: 0x06005D40 RID: 23872 RVA: 0x001DF19E File Offset: 0x001DD39E
		public bool ValidateLoadMapRequest(string mapID, int playerNum)
		{
			return playerNum == this.localState.driverID && this.AreAllPlayersInLobby() && SharedBlocksManager.IsMapIDValid(mapID);
		}

		// Token: 0x06005D41 RID: 23873 RVA: 0x001DF1C0 File Offset: 0x001DD3C0
		private void OnJoinedRoom()
		{
			GTDev.Log<string>("[SharedBlocksTerminal::OnJoinedRoom] Joined a multiplayer room, resetting terminal control", null);
			this.cachedLocalPlayerID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.ResetTerminalControl();
		}

		// Token: 0x06005D42 RID: 23874 RVA: 0x001DF1E8 File Offset: 0x001DD3E8
		private void OnReturnedToSinglePlayer()
		{
			if (this.localState.driverID != this.cachedLocalPlayerID)
			{
				this.ResetTerminalControl();
			}
			else
			{
				this.localState.driverID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
			this.cachedLocalPlayerID = -1;
		}

		// Token: 0x06005D43 RID: 23875 RVA: 0x001DF226 File Offset: 0x001DD426
		public void ResetTerminalControl()
		{
			this.localState.driverID = -2;
			this.isTerminalLocked = false;
			this.selectedMap = null;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.UpdateTerminalButton();
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x001DF256 File Offset: 0x001DD456
		private void UpdateTerminalButton()
		{
			this.terminalControlButton.isOn = this.isTerminalLocked;
			this.terminalControlButton.UpdateColor();
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x001DF274 File Offset: 0x001DD474
		private void OnSharedBlocksMapLoaded(string mapID)
		{
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(mapID);
			}
			if (SharedBlocksManager.IsMapIDValid(mapID))
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadSuccess);
			}
			else if (this.localState.state != SharedBlocksTerminal.TerminalState.LoadFail)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			}
			this.isLoadingMap = false;
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x001DF2C2 File Offset: 0x001DD4C2
		private void OnSharedBlocksMapLoadFailed(string message)
		{
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			this.SetStatusText(message);
			this.isLoadingMap = false;
		}

		// Token: 0x06005D47 RID: 23879 RVA: 0x001DF2DC File Offset: 0x001DD4DC
		private void OnSharedBlocksMapLoadStart()
		{
			if (this.linkedTable == null)
			{
				return;
			}
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
				this.isLoadingMap = true;
				this.lastLoadTime = Time.time;
			}
		}

		// Token: 0x04006AC7 RID: 27335
		public const string SHARE_BLOCKS_TERMINAL_PROMPT_KEY = "SHARE_BLOCKS_TERMINAL_PROMPT";

		// Token: 0x04006AC8 RID: 27336
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON";

		// Token: 0x04006AC9 RID: 27337
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE";

		// Token: 0x04006ACA RID: 27338
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED";

		// Token: 0x04006ACB RID: 27339
		public const string SHARE_BLOCKS_TERMINAL_STATUS_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_SEARCH";

		// Token: 0x04006ACC RID: 27340
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND";

		// Token: 0x04006ACD RID: 27341
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND";

		// Token: 0x04006ACE RID: 27342
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOADING_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOADING";

		// Token: 0x04006ACF RID: 27343
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS";

		// Token: 0x04006AD0 RID: 27344
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED";

		// Token: 0x04006AD1 RID: 27345
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER";

		// Token: 0x04006AD2 RID: 27346
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION";

		// Token: 0x04006AD3 RID: 27347
		public const string SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS";

		// Token: 0x04006AD4 RID: 27348
		public const string SHARE_BLOCKS_TERMINAL_STATUS_WAIT_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_WAIT";

		// Token: 0x04006AD5 RID: 27349
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD";

		// Token: 0x04006AD6 RID: 27350
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD";

		// Token: 0x04006AD7 RID: 27351
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD";

		// Token: 0x04006AD8 RID: 27352
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD";

		// Token: 0x04006AD9 RID: 27353
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL";

		// Token: 0x04006ADA RID: 27354
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE";

		// Token: 0x04006ADB RID: 27355
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH";

		// Token: 0x04006ADC RID: 27356
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_VOTES_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_VOTES";

		// Token: 0x04006ADD RID: 27357
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL";

		// Token: 0x04006ADE RID: 27358
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT";

		// Token: 0x04006ADF RID: 27359
		public const string SHARE_BLOCKS_TERMINAL_ERROR_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_TITLE";

		// Token: 0x04006AE0 RID: 27360
		public const string SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS";

		// Token: 0x04006AE1 RID: 27361
		public const string SHARE_BLOCKS_TERMINAL_ERROR_BACK_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_BACK";

		// Token: 0x04006AE2 RID: 27362
		public const string SHARE_BLOCKS_TERMINAL_INFO_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_INFO_TITLE";

		// Token: 0x04006AE3 RID: 27363
		public const string SHARE_BLOCKS_TERMINAL_INFO_DATA_KEY = "SHARE_BLOCKS_TERMINAL_INFO_DATA";

		// Token: 0x04006AE4 RID: 27364
		public const string SHARE_BLOCKS_TERMINAL_INFO_ENTER_KEY = "SHARE_BLOCKS_TERMINAL_INFO_ENTER";

		// Token: 0x04006AE5 RID: 27365
		public const string SHARE_BLOCKS_TERMINAL_OTHER_DRIVER_KEY = "SHARE_BLOCKS_TERMINAL_OTHER_DRIVER";

		// Token: 0x04006AE6 RID: 27366
		public const string SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL";

		// Token: 0x04006AE7 RID: 27367
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT";

		// Token: 0x04006AE8 RID: 27368
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH";

		// Token: 0x04006AE9 RID: 27369
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID";

		// Token: 0x04006AEA RID: 27370
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04006AEB RID: 27371
		[SerializeField]
		private TMP_Text currentMapSelectionText;

		// Token: 0x04006AEC RID: 27372
		[SerializeField]
		private TMP_Text statusMessageText;

		// Token: 0x04006AED RID: 27373
		[SerializeField]
		private TMP_Text currentDriverText;

		// Token: 0x04006AEE RID: 27374
		[SerializeField]
		private TMP_Text currentDriverLabel;

		// Token: 0x04006AEF RID: 27375
		[SerializeField]
		private LocalizedText _currentDriverLoc;

		// Token: 0x04006AF0 RID: 27376
		[SerializeField]
		private SharedBlocksScreen noDriverScreen;

		// Token: 0x04006AF1 RID: 27377
		[SerializeField]
		private SharedBlocksScreenSearch searchScreen;

		// Token: 0x04006AF2 RID: 27378
		[SerializeField]
		private GorillaPressableButton terminalControlButton;

		// Token: 0x04006AF3 RID: 27379
		[SerializeField]
		private float loadMapCooldown = 30f;

		// Token: 0x04006AF4 RID: 27380
		[SerializeField]
		private GorillaFriendCollider lobbyTrigger;

		// Token: 0x04006AF5 RID: 27381
		private SharedBlocksManager.SharedBlocksMap selectedMap;

		// Token: 0x04006AF6 RID: 27382
		private SharedBlocksScreen currentScreen;

		// Token: 0x04006AF7 RID: 27383
		private BuilderTable linkedTable;

		// Token: 0x04006AF8 RID: 27384
		public const int NO_DRIVER_ID = -2;

		// Token: 0x04006AF9 RID: 27385
		private bool awaitingWebRequest;

		// Token: 0x04006AFA RID: 27386
		private string requestedMapID;

		// Token: 0x04006AFB RID: 27387
		public const string POINTER = "> ";

		// Token: 0x04006AFC RID: 27388
		public Action<bool> OnMapLoadComplete;

		// Token: 0x04006AFD RID: 27389
		private bool isTerminalLocked;

		// Token: 0x04006AFE RID: 27390
		private SharedBlocksTerminal.SharedBlocksTerminalState localState;

		// Token: 0x04006AFF RID: 27391
		private int cachedLocalPlayerID = -1;

		// Token: 0x04006B00 RID: 27392
		private bool isLoadingMap;

		// Token: 0x04006B01 RID: 27393
		private float lastLoadTime;

		// Token: 0x04006B02 RID: 27394
		private bool useNametags;

		// Token: 0x04006B03 RID: 27395
		private bool hasInitialized;

		// Token: 0x04006B04 RID: 27396
		private static StringBuilder sb = new StringBuilder();

		// Token: 0x04006B05 RID: 27397
		private VRRig driverRig;

		// Token: 0x04006B06 RID: 27398
		private static List<VRRig> tempRigs = new List<VRRig>(16);

		// Token: 0x04006B07 RID: 27399
		private int playersInRoom;

		// Token: 0x02000E8F RID: 3727
		public enum ScreenType
		{
			// Token: 0x04006B09 RID: 27401
			NO_DRIVER,
			// Token: 0x04006B0A RID: 27402
			SEARCH,
			// Token: 0x04006B0B RID: 27403
			LOADING,
			// Token: 0x04006B0C RID: 27404
			ERROR,
			// Token: 0x04006B0D RID: 27405
			SCAN_INFO,
			// Token: 0x04006B0E RID: 27406
			OTHER_DRIVER
		}

		// Token: 0x02000E90 RID: 3728
		public enum TerminalState
		{
			// Token: 0x04006B10 RID: 27408
			NoStatus,
			// Token: 0x04006B11 RID: 27409
			Searching,
			// Token: 0x04006B12 RID: 27410
			NotFound,
			// Token: 0x04006B13 RID: 27411
			Found,
			// Token: 0x04006B14 RID: 27412
			Loading,
			// Token: 0x04006B15 RID: 27413
			LoadSuccess,
			// Token: 0x04006B16 RID: 27414
			LoadFail
		}

		// Token: 0x02000E91 RID: 3729
		public class SharedBlocksTerminalState
		{
			// Token: 0x04006B17 RID: 27415
			public SharedBlocksTerminal.ScreenType currentScreen;

			// Token: 0x04006B18 RID: 27416
			public SharedBlocksTerminal.TerminalState state;

			// Token: 0x04006B19 RID: 27417
			public int driverID;
		}
	}
}
