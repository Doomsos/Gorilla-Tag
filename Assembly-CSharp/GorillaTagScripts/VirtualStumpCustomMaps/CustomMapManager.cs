using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.UI.ModIO;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E18 RID: 3608
	public class CustomMapManager : MonoBehaviour, IBuildValidation
	{
		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x060059F7 RID: 23031 RVA: 0x001CC473 File Offset: 0x001CA673
		public static bool WaitingForRoomJoin
		{
			get
			{
				return CustomMapManager.waitingForRoomJoin;
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x060059F8 RID: 23032 RVA: 0x001CC47A File Offset: 0x001CA67A
		public static bool WaitingForDisconnect
		{
			get
			{
				return CustomMapManager.waitingForDisconnect;
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x060059F9 RID: 23033 RVA: 0x001CC481 File Offset: 0x001CA681
		public static long LoadingMapId
		{
			get
			{
				return CustomMapManager.loadingMapId;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x060059FA RID: 23034 RVA: 0x001CC48D File Offset: 0x001CA68D
		public static long UnloadingMapId
		{
			get
			{
				return CustomMapManager.unloadingMapId;
			}
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x001CC499 File Offset: 0x001CA699
		public bool BuildValidationCheck()
		{
			if (this.defaultTeleporter.IsNull())
			{
				Debug.LogError("CustomMapManager does not have its \"Default Teleporter\" property.");
				return false;
			}
			return true;
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x001CC4B5 File Offset: 0x001CA6B5
		private void Awake()
		{
			if (CustomMapManager.instance == null)
			{
				CustomMapManager.instance = this;
				CustomMapManager.hasInstance = true;
				return;
			}
			if (CustomMapManager.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060059FD RID: 23037 RVA: 0x001CC4F0 File Offset: 0x001CA6F0
		public void OnEnable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			CMSSerializer.OnTriggerHistoryProcessedForScene.AddListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnected);
			NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnDisconnected);
		}

		// Token: 0x060059FE RID: 23038 RVA: 0x001CC614 File Offset: 0x001CA814
		public void OnDisable()
		{
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnected);
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x00002789 File Offset: 0x00000989
		private void OnUGCEnabled()
		{
		}

		// Token: 0x06005A00 RID: 23040 RVA: 0x00002789 File Offset: 0x00000989
		private void OnUGCDisabled()
		{
		}

		// Token: 0x06005A01 RID: 23041 RVA: 0x001CC6AC File Offset: 0x001CA8AC
		private void Start()
		{
			CustomMapLoader.Initialize(new Action<MapLoadStatus, int, string>(CustomMapManager.OnMapLoadProgress), new Action<bool>(CustomMapManager.OnMapLoadFinished), new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
			for (int i = this.virtualStumpTeleportLocations.Count - 1; i >= 0; i--)
			{
				if (this.virtualStumpTeleportLocations[i] == null)
				{
					this.virtualStumpTeleportLocations.RemoveAt(i);
				}
			}
			if (this.defaultTeleporter.IsNull())
			{
				GTDev.LogError<string>("[CustomMapManager::Start] \"Default Teleporter\" property is invalid.", null);
			}
			this.virtualStumpToggleableRoot.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x06005A02 RID: 23042 RVA: 0x001CC758 File Offset: 0x001CA958
		private void OnDestroy()
		{
			if (CustomMapManager.instance == this)
			{
				CustomMapManager.instance = null;
				CustomMapManager.hasInstance = false;
			}
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
			CMSSerializer.OnTriggerHistoryProcessedForScene.RemoveListener(new UnityAction<string>(CustomMapManager.OnSceneTriggerHistoryProcessed));
			ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnected);
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x001CC80C File Offset: 0x001CAA0C
		private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
		{
			if (CustomMapManager.waitingForModInstall && CustomMapManager.waitingForModInstallId == mod.Id)
			{
				if (CustomMapManager.abortModLoadIds.Contains(mod.Id))
				{
					CustomMapManager.abortModLoadIds.Remove(mod.Id);
					if (CustomMapManager.waitingForModInstallId.Equals(mod.Id))
					{
						CustomMapManager.waitingForModInstall = false;
						CustomMapManager.waitingForModDownload = false;
						CustomMapManager.waitingForModInstallId = ModId.Null;
					}
					return;
				}
				switch (modfile.State)
				{
				case 2:
				case 6:
					CustomMapManager.waitingForModDownload = true;
					return;
				case 3:
					CustomMapManager.waitingForModDownload = false;
					return;
				case 4:
				case 7:
					break;
				case 5:
					CustomMapManager.waitingForModDownload = false;
					this.LoadInstalledMap(mod);
					break;
				case 8:
					switch (jobType)
					{
					case 0:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to download map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO DOWNLOAD MAP: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.waitingForModDownload = false;
						return;
					case 1:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to install map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO INSTALL MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					case 2:
						Debug.LogError("[CustomMapManager::HandleModManagementEvent] Failed to update map with modID " + mod.Id.ToString() + ", error: " + modfile.FileStateErrorCause.GetMessage());
						CustomMapManager.HandleMapLoadFailed("FAILED TO UPDATE MAP: " + modfile.FileStateErrorCause.GetMessage());
						return;
					default:
						return;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x001CC9DC File Offset: 0x001CABDC
		internal static void TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			if (UGCPermissionManager.IsUGCDisabled)
			{
				return;
			}
			if (!CustomMapManager.hasInstance || fromTeleporter == null)
			{
				if (callback != null)
				{
					callback.Invoke(false);
				}
				return;
			}
			CustomMapManager.instance.gameObject.SetActive(true);
			CustomMapManager.instance.StartCoroutine(CustomMapManager.Internal_TeleportToVirtualStump(fromTeleporter, callback));
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x001CCA32 File Offset: 0x001CAC32
		private static IEnumerator Internal_TeleportToVirtualStump(VirtualStumpTeleporter fromTeleporter, Action<bool> callback)
		{
			CustomMapManager.lastUsedTeleporter = fromTeleporter;
			CustomMapManager.preVStumpGamemode = GorillaComputer.instance.currentGameMode.Value;
			if (CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetAutoLoadGamemode().ToString());
			}
			GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Teleporting to Virtual Stump...", null);
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			GreyZoneManager greyZoneManager = GreyZoneManager.Instance;
			if (greyZoneManager != null)
			{
				greyZoneManager.ForceStopGreyZone();
			}
			if (CustomMapManager.instance.virtualStumpTeleportLocations.Count > 0)
			{
				int num = Random.Range(0, CustomMapManager.instance.virtualStumpTeleportLocations.Count);
				Transform randTeleportTarget = CustomMapManager.instance.virtualStumpTeleportLocations[num];
				CustomMapManager.instance.EnableTeleportHUD(true);
				CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, true, CustomMapManager.instance.localTeleportSFXSource, true);
				yield return new WaitForSeconds(0.75f);
				CosmeticsController.instance.ClearCheckoutAndCart(false);
				CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(true);
				GTPlayer.Instance.TeleportTo(randTeleportTarget, true, false);
				GorillaComputer.instance.SetInVirtualStump(true);
				yield return null;
				if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
				{
					VRRig.LocalRig.zoneEntity.DisableZoneChanges();
				}
				ZoneManagement.SetActiveZone(GTZone.customMaps);
				foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
				{
					if (gameObject != null)
					{
						gameObject.gameObject.SetActive(false);
					}
				}
				if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
				{
					CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				}
				else
				{
					ZoneShaderSettings.ActivateDefaultSettings();
				}
				CustomMapManager.instance.ghostReactorManager.reactor.EnableGhostReactorForVirtualStump();
				CustomMapManager.currentTeleportCallback = callback;
				CustomMapManager.pendingNewPrivateRoomName = "";
				CustomMapManager.preTeleportInPrivateRoom = false;
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						CustomMapManager.preTeleportInPrivateRoom = true;
						CustomMapManager.waitingForRoomJoin = true;
						CustomMapManager.pendingNewPrivateRoomName = GorillaComputer.instance.VStumpRoomPrepend + NetworkSystem.Instance.RoomName;
					}
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Returning to singleplayer...", null);
					CustomMapManager.waitingForLoginDisconnect = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Attempting auto-login to mod.io...", null);
					CustomMapManager.AttemptAutoLogin();
				}
				randTeleportTarget = null;
			}
			else
			{
				GTDev.Log<string>("[CustomMapManager::TeleportToVirtualStump] Not Teleporting, virtualStumpTeleportLocations is empty!", null);
				CustomMapManager.EndTeleport(false);
			}
			yield break;
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x001CCA48 File Offset: 0x001CAC48
		private static void OnAutoLoginComplete(Error error)
		{
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Error: {0}", error), null);
			if (!CustomMapManager.hasInstance)
			{
				Debug.LogError("[CustomMapManager::OnAutoLoginComplete] CustomMapManager not initialized!");
				return;
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Needs to rejoin private room: {0}", CustomMapManager.preTeleportInPrivateRoom), null);
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				if (NetworkSystem.Instance.netState != NetSystemState.Idle)
				{
					GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Netstate not Idle, delaying join attempt. CurrentStatus: {0}", NetworkSystem.Instance.netState), null);
					CustomMapManager.delayedJoinCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedJoinVStumpPrivateRoom());
				}
				else
				{
					GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				}
			}
			GTDev.Log<string>(string.Format("[CustomMapManager::OnAutoLoginComplete] Waiting For D/C? {0}", CustomMapManager.waitingForDisconnect), null);
			if (!CustomMapManager.preTeleportInPrivateRoom && !CustomMapManager.waitingForDisconnect)
			{
				GTDev.Log<string>("[CustomMapManager::OnAutoLoginComplete] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
			CustomMapManager.preTeleportInPrivateRoom = false;
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x001CCB4F File Offset: 0x001CAD4F
		private static IEnumerator DelayedJoinVStumpPrivateRoom()
		{
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] waiting for netstate to be Idle", null);
			while (NetworkSystem.Instance.netState != NetSystemState.Idle)
			{
				yield return null;
			}
			GTDev.Log<string>("[CustomMapManager::DelayedJoinVStumpPrivateRoom] joining @ version of private room: " + CustomMapManager.pendingNewPrivateRoomName, null);
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
			yield break;
		}

		// Token: 0x06005A08 RID: 23048 RVA: 0x001CCB58 File Offset: 0x001CAD58
		public static void ExitVirtualStump(Action<bool> callback)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.lastUsedTeleporter.IsNull())
			{
				if (CustomMapManager.instance.defaultTeleporter.IsNull())
				{
					if (callback != null)
					{
						callback.Invoke(false);
					}
				}
				else
				{
					CustomMapManager.lastUsedTeleporter = CustomMapManager.instance.defaultTeleporter;
				}
			}
			if (CustomMapManager.delayedJoinCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
				CustomMapManager.delayedJoinCoroutine = null;
			}
			if (CustomMapManager.delayedTryAutoLoadCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedTryAutoLoadCoroutine);
				CustomMapManager.delayedTryAutoLoadCoroutine = null;
			}
			CustomMapManager.instance.dayNightManager.RequestRepopulateLightmaps();
			PrivateUIRoom.ForceStartOverlay();
			GorillaTagger.Instance.overrideNotInFocus = true;
			CustomMapManager.instance.EnableTeleportHUD(false);
			CustomMapManager.currentTeleportCallback = callback;
			CustomMapManager.exitVirtualStumpPending = true;
			if (!CustomMapManager.UnloadMap(false))
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x06005A09 RID: 23049 RVA: 0x001CCC2C File Offset: 0x001CAE2C
		private static void FinalizeExitVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTPlayer.Instance.SetHoverActive(false);
			VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			RoomSystem.ClearOverridenRoomSize();
			CosmeticsController.instance.ClearCheckoutAndCart(false);
			foreach (GameObject gameObject in CustomMapManager.instance.rootObjectsToDeactivateAfterTeleport)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(true);
				}
			}
			if (CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.None && CustomMapManager.lastUsedTeleporter.GetReturnGamemode() != GameModeType.Count)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.lastUsedTeleporter.GetReturnGamemode().ToString());
			}
			else if (CustomMapManager.preVStumpGamemode != "")
			{
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapManager.preVStumpGamemode);
				CustomMapManager.preVStumpGamemode = "";
			}
			if (VRRig.LocalRig.IsNotNull())
			{
				GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
				if (component != null && component.State == GRPlayer.GRPlayerState.Ghost)
				{
					CustomMapManager.instance.defaultReviveStation.RevivePlayer(component);
				}
			}
			ZoneManagement.SetActiveZone(CustomMapManager.lastUsedTeleporter.GetZone());
			if (VRRig.LocalRig.IsNotNull() && VRRig.LocalRig.zoneEntity.IsNotNull())
			{
				VRRig.LocalRig.zoneEntity.EnableZoneChanges();
			}
			GorillaComputer.instance.SetInVirtualStump(false);
			GTPlayer.Instance.TeleportTo(CustomMapManager.lastUsedTeleporter.GetReturnTransform(), true, false);
			CustomMapManager.instance.virtualStumpToggleableRoot.SetActive(false);
			ZoneShaderSettings.ActivateDefaultSettings();
			VRRig.LocalRig.EnableVStumpReturnWatch(false);
			GTPlayer.Instance.SetHoverAllowed(false, true);
			CustomMapManager.exitVirtualStumpPending = false;
			if (CustomMapManager.delayedEndTeleportCoroutine != null)
			{
				CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
			}
			CustomMapManager.delayedEndTeleportCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedEndTeleport());
			if (CustomMapManager.preTeleportInPrivateRoom)
			{
				CustomMapManager.waitingForRoomJoin = true;
				CustomMapManager.pendingNewPrivateRoomName = CustomMapManager.pendingNewPrivateRoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, 5);
				PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					CustomMapManager.waitingForRoomJoin = true;
					CustomMapManager.pendingNewPrivateRoomName = NetworkSystem.Instance.RoomName.RemoveAll(GorillaComputer.instance.VStumpRoomPrepend, 5);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResult));
					return;
				}
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					CustomMapManager.waitingForRoomJoin = true;
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null);
					return;
				}
			}
			else
			{
				if (CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger() != null)
				{
					GorillaComputer.instance.allowedMapsToJoin = CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger().myCollider.myAllowedMapsToJoin;
					Debug.Log(string.Format("[CustomMapManager::FinalizeExit] allowedMaps: {0}", GorillaComputer.instance.allowedMapsToJoin));
					CustomMapManager.waitingForRoomJoin = true;
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(CustomMapManager.lastUsedTeleporter.GetExitVStumpJoinTrigger(), JoinType.Solo, null);
					return;
				}
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x001CCF98 File Offset: 0x001CB198
		private static void OnJoinSpecificRoomResult(NetJoinResult result)
		{
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Failed_Full:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			case NetJoinResult.Failed_Other:
				GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResult] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed, marking for retry... ", null);
				CustomMapManager.waitingForDisconnect = true;
				CustomMapManager.shouldRetryJoin = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x001CD018 File Offset: 0x001CB218
		private static void OnJoinSpecificRoomResultFailureAllowed(NetJoinResult result)
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			GTDev.Log<string>("[CustomMapManager::OnJoinSpecificRoomResultFailureAllowed] Result: " + result.ToString(), null);
			switch (result)
			{
			case NetJoinResult.Success:
			case NetJoinResult.FallbackCreated:
				return;
			case NetJoinResult.Failed_Full:
			case NetJoinResult.Failed_Other:
				CustomMapManager.instance.OnJoinRoomFailed();
				return;
			case NetJoinResult.AlreadyInRoom:
				CustomMapManager.instance.OnJoinedRoom();
				return;
			default:
				return;
			}
		}

		// Token: 0x06005A0C RID: 23052 RVA: 0x001CD080 File Offset: 0x001CB280
		public static bool AreAllPlayersInVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return false;
			}
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				if (!CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(vrrig.creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005A0D RID: 23053 RVA: 0x001CD108 File Offset: 0x001CB308
		public static bool IsRemotePlayerInVirtualStump(string playerID)
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(playerID);
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x001CD140 File Offset: 0x001CB340
		public static bool IsLocalPlayerInVirtualStump()
		{
			return CustomMapManager.hasInstance && !CustomMapManager.instance.virtualStumpPlayerDetector.IsNull() && !VRRig.LocalRig.IsNull() && CustomMapManager.instance.virtualStumpPlayerDetector.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x001CD1A0 File Offset: 0x001CB3A0
		private void OnDisconnected()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			CustomMapManager.ClearRoomMap();
			if (CustomMapManager.waitingForLoginDisconnect)
			{
				CustomMapManager.waitingForLoginDisconnect = false;
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Attempting auto-login to mod.io...", null);
				CustomMapManager.AttemptAutoLogin();
				return;
			}
			if (CustomMapManager.waitingForDisconnect)
			{
				CustomMapManager.waitingForDisconnect = false;
				if (CustomMapManager.shouldRetryJoin)
				{
					CustomMapManager.shouldRetryJoin = false;
					GTDev.Log<string>("[CustomMapManager::OnDisconnected] Joining " + CustomMapManager.pendingNewPrivateRoomName + " failed previously, retrying once... ", null);
					PhotonNetworkController.Instance.AttemptToJoinSpecificRoomWithCallback(CustomMapManager.pendingNewPrivateRoomName, JoinType.Solo, new Action<NetJoinResult>(CustomMapManager.OnJoinSpecificRoomResultFailureAllowed));
					return;
				}
				GTDev.Log<string>("[CustomMapManager::OnDisconnected] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
			}
		}

		// Token: 0x06005A10 RID: 23056 RVA: 0x001CD240 File Offset: 0x001CB440
		private static Task AttemptAutoLogin()
		{
			CustomMapManager.<AttemptAutoLogin>d__78 <AttemptAutoLogin>d__;
			<AttemptAutoLogin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptAutoLogin>d__.<>1__state = -1;
			<AttemptAutoLogin>d__.<>t__builder.Start<CustomMapManager.<AttemptAutoLogin>d__78>(ref <AttemptAutoLogin>d__);
			return <AttemptAutoLogin>d__.<>t__builder.Task;
		}

		// Token: 0x06005A11 RID: 23057 RVA: 0x001CD27B File Offset: 0x001CB47B
		private void OnJoinRoomFailed()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				GTDev.Log<string>("[CustomMapManager::OnJoinRoomFailed] Currently waiting for room join, resetting state, ending teleport...", null);
				CustomMapManager.waitingForRoomJoin = false;
				CustomMapManager.EndTeleport(false);
			}
		}

		// Token: 0x06005A12 RID: 23058 RVA: 0x001CD2A4 File Offset: 0x001CB4A4
		private static void EndTeleport(bool teleportSuccessful)
		{
			if (CustomMapManager.hasInstance)
			{
				if (CustomMapManager.delayedEndTeleportCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedEndTeleportCoroutine);
					CustomMapManager.delayedEndTeleportCoroutine = null;
				}
				if (CustomMapManager.delayedJoinCoroutine != null)
				{
					CustomMapManager.instance.StopCoroutine(CustomMapManager.delayedJoinCoroutine);
					CustomMapManager.delayedJoinCoroutine = null;
				}
			}
			CustomMapManager.DisableTeleportHUD();
			GorillaTagger.Instance.overrideNotInFocus = false;
			PrivateUIRoom.StopForcedOverlay();
			Action<bool> action = CustomMapManager.currentTeleportCallback;
			if (action != null)
			{
				action.Invoke(teleportSuccessful);
			}
			CustomMapManager.currentTeleportCallback = null;
			if (CustomMapManager.hasInstance && !GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				GTDev.Log<string>("[CustomMapManager::EndTeleport] Player is not in VStump, disabling VStump_Lobby GameObject", null);
				CustomMapManager.instance.gameObject.SetActive(false);
			}
			if (teleportSuccessful && GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId() != ModId.Null)
			{
				bool flag = false;
				if (CustomMapManager.waitingForRoomJoin)
				{
					GTDev.Log<string>("[CustomMapManager::EndTeleport] Still waiting for room join, delaying auto-load...", null);
					flag = true;
				}
				else if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient && VirtualStumpSerializer.IsWaitingForRoomInit())
				{
					GTDev.Log<string>("[CustomMapManager::EndTeleport] Still waiting for room init, delaying auto-load...", null);
					flag = true;
				}
				if (flag)
				{
					CustomMapManager.delayedTryAutoLoadCoroutine = CustomMapManager.instance.StartCoroutine(CustomMapManager.DelayedTryAutoLoad());
					return;
				}
				GTDev.Log<string>("[CustomMapManager::EndTeleport] Attempting auto-load...", null);
				if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
				{
					CustomMapManager.SetRoomMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
					CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
					return;
				}
				if (CustomMapManager.GetRoomMapId() == CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId())
				{
					CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
				}
			}
		}

		// Token: 0x06005A13 RID: 23059 RVA: 0x001CD469 File Offset: 0x001CB669
		private static IEnumerator DelayedEndTeleport()
		{
			yield return new WaitForSecondsRealtime(CustomMapManager.instance.maxPostTeleportRoomProcessingTime);
			GTDev.Log<string>("[CustomMapManager::DelayedEndTeleport] Timer expired, force ending teleport...", null);
			CustomMapManager.EndTeleport(false);
			yield break;
		}

		// Token: 0x06005A14 RID: 23060 RVA: 0x001CD471 File Offset: 0x001CB671
		private static IEnumerator DelayedTryAutoLoad()
		{
			while (CustomMapManager.waitingForRoomJoin || VirtualStumpSerializer.IsWaitingForRoomInit())
			{
				yield return new WaitForSeconds(0.1f);
			}
			GTDev.Log<string>("[CustomMapManager::DelayedTryAutoLoad] Room Init finished, attempting auto-load...", null);
			if (!NetworkSystem.Instance.InRoom || (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient))
			{
				CustomMapManager.SetRoomMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
				CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
			}
			else if (CustomMapManager.GetRoomMapId() == CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId())
			{
				CustomMapManager.LoadMap(CustomMapManager.lastUsedTeleporter.GetAutoLoadMapModId());
			}
			yield break;
		}

		// Token: 0x06005A15 RID: 23061 RVA: 0x001CD47C File Offset: 0x001CB67C
		private void OnJoinedRoom()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (CustomMapManager.waitingForRoomJoin)
			{
				CustomMapManager.waitingForRoomJoin = false;
				GTDev.Log<string>("[CustomMapManager::OnJoinedRoom] Ending teleport...", null);
				CustomMapManager.EndTeleport(true);
				if (CustomMapManager.lastUsedTeleporter.IsNotNull())
				{
					CustomMapManager.lastUsedTeleporter.PlayTeleportEffects(true, false, null, true);
				}
			}
		}

		// Token: 0x06005A16 RID: 23062 RVA: 0x001CD4CC File Offset: 0x001CB6CC
		public static bool UnloadMap(bool returnToSinglePlayerIfInPublic = true)
		{
			if (CustomMapManager.unloadInProgress)
			{
				return false;
			}
			if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.IsLoading())
			{
				if (CustomMapManager.loadInProgress)
				{
					GTDev.Log<string>("[CustomMapManager::UnloadMap] Map load is currently in progress... aborting...", null);
					CustomMapManager.abortModLoadIds.AddIfNew(CustomMapManager.loadingMapId);
					bool flag = CustomMapManager.waitingForModDownload;
					CustomMapManager.loadInProgress = false;
					CustomMapManager.loadingMapId = ModId.Null;
					CustomMapManager.waitingForModDownload = false;
					CustomMapManager.waitingForModInstall = false;
					CustomMapManager.waitingForModInstallId = ModId.Null;
					CustomMapManager.ClearRoomMap();
				}
				else
				{
					CustomMapManager.ClearRoomMap();
				}
				return false;
			}
			CustomMapManager.unloadInProgress = true;
			CustomMapManager.unloadingMapId = new ModId(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : CustomMapLoader.GetLoadingMapModId());
			CustomMapManager.OnMapLoadProgress(MapLoadStatus.Unloading, 0, "");
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.ClearRoomMap();
			CustomGameMode.LuaScript = "";
			if (CustomGameMode.gameScriptRunner != null)
			{
				CustomGameMode.StopScript();
			}
			CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = false;
			CustomMapManager.customMapDefaultZoneShaderProperties = default(CMSZoneShaderSettings.CMSZoneShaderProperties);
			CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = null;
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(CustomMapManager.instance.virtualStumpZoneShaderSettings, false);
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(false);
				CustomMapManager.allCustomMapZoneShaderSettings.Clear();
			}
			CustomMapLoader.CloseDoorAndUnloadMap(new Action(CustomMapManager.OnMapUnloadCompleted));
			if (returnToSinglePlayerIfInPublic && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			return true;
		}

		// Token: 0x06005A17 RID: 23063 RVA: 0x001CD650 File Offset: 0x001CB850
		private static void OnMapUnloadCompleted()
		{
			CustomMapManager.unloadInProgress = false;
			CustomMapManager.OnMapUnloadComplete.Invoke();
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
			if (CustomMapManager.exitVirtualStumpPending)
			{
				CustomMapManager.FinalizeExitVirtualStump();
			}
		}

		// Token: 0x06005A18 RID: 23064 RVA: 0x001CD690 File Offset: 0x001CB890
		public static Task LoadMap(ModId modId)
		{
			CustomMapManager.<LoadMap>d__86 <LoadMap>d__;
			<LoadMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadMap>d__.modId = modId;
			<LoadMap>d__.<>1__state = -1;
			<LoadMap>d__.<>t__builder.Start<CustomMapManager.<LoadMap>d__86>(ref <LoadMap>d__);
			return <LoadMap>d__.<>t__builder.Task;
		}

		// Token: 0x06005A19 RID: 23065 RVA: 0x001CD6D4 File Offset: 0x001CB8D4
		private Task LoadInstalledMap(Mod installedMod)
		{
			CustomMapManager.<LoadInstalledMap>d__87 <LoadInstalledMap>d__;
			<LoadInstalledMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadInstalledMap>d__.installedMod = installedMod;
			<LoadInstalledMap>d__.<>1__state = -1;
			<LoadInstalledMap>d__.<>t__builder.Start<CustomMapManager.<LoadInstalledMap>d__87>(ref <LoadInstalledMap>d__);
			return <LoadInstalledMap>d__.<>t__builder.Task;
		}

		// Token: 0x06005A1A RID: 23066 RVA: 0x001CD717 File Offset: 0x001CB917
		private static void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
		{
			CustomMapManager.OnMapLoadStatusChanged.Invoke(loadStatus, progress, message);
		}

		// Token: 0x06005A1B RID: 23067 RVA: 0x001CD728 File Offset: 0x001CB928
		private static void OnMapLoadFinished(bool success)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModDownload = false;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			if (success)
			{
				CustomMapLoader.OpenDoorToMap();
				if (!CustomMapLoader.GetLuauGamemodeScript().IsNullOrEmpty())
				{
					CustomGameMode.LuaScript = CustomMapLoader.GetLuauGamemodeScript();
					if (CustomGameMode.LuaScript != "" && CustomGameMode.GameModeInitialized && CustomGameMode.gameScriptRunner == null)
					{
						CustomGameMode.LuaStart();
					}
				}
			}
			CustomMapManager.OnMapLoadComplete.Invoke(success);
		}

		// Token: 0x06005A1C RID: 23068 RVA: 0x001CD7AC File Offset: 0x001CB9AC
		private static void HandleMapLoadFailed(string message = null)
		{
			CustomMapManager.loadInProgress = false;
			CustomMapManager.loadingMapId = ModId.Null;
			CustomMapManager.waitingForModInstall = false;
			CustomMapManager.waitingForModInstallId = ModId.Null;
			CustomMapManager.OnMapLoadStatusChanged.Invoke(MapLoadStatus.Error, 0, message ?? "UNKNOWN ERROR");
			CustomMapManager.OnMapLoadComplete.Invoke(false);
		}

		// Token: 0x06005A1D RID: 23069 RVA: 0x001CD7FA File Offset: 0x001CB9FA
		public static bool IsUnloading()
		{
			return CustomMapManager.unloadInProgress;
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x001CD801 File Offset: 0x001CBA01
		public static bool IsLoading()
		{
			return CustomMapManager.IsLoading(ModId.Null);
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x001CD80D File Offset: 0x001CBA0D
		public static bool IsLoading(ModId modId)
		{
			if (!modId.IsValid())
			{
				return CustomMapManager.loadInProgress || CustomMapLoader.IsLoading();
			}
			return CustomMapManager.loadInProgress && CustomMapManager.loadingMapId == modId;
		}

		// Token: 0x06005A20 RID: 23072 RVA: 0x001CD83C File Offset: 0x001CBA3C
		public static ModId GetRoomMapId()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (CustomMapManager.currentRoomMapModId == ModId.Null && NetworkSystem.Instance.IsMasterClient && CustomMapLoader.IsMapLoaded())
				{
					CustomMapManager.currentRoomMapModId = new ModId(CustomMapLoader.LoadedMapModId);
				}
				return CustomMapManager.currentRoomMapModId;
			}
			if (CustomMapManager.IsLoading())
			{
				return CustomMapManager.loadingMapId;
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				return new ModId(CustomMapLoader.LoadedMapModId);
			}
			return ModId.Null;
		}

		// Token: 0x06005A21 RID: 23073 RVA: 0x001CD8BC File Offset: 0x001CBABC
		public static void SetRoomMap(long modId)
		{
			if (!CustomMapManager.hasInstance || modId == CustomMapManager.currentRoomMapModId._id)
			{
				return;
			}
			CustomMapManager.currentRoomMapModId = new ModId(modId);
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x001CD8F4 File Offset: 0x001CBAF4
		public static void ClearRoomMap()
		{
			if (!CustomMapManager.hasInstance || CustomMapManager.currentRoomMapModId.Equals(ModId.Null))
			{
				return;
			}
			CustomMapManager.currentRoomMapModId = ModId.Null;
			CustomMapManager.currentRoomMapApproved = false;
			CustomMapManager.OnRoomMapChanged.Invoke(ModId.Null);
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x001CD944 File Offset: 0x001CBB44
		public static bool CanLoadRoomMap()
		{
			return CustomMapManager.currentRoomMapModId != ModId.Null;
		}

		// Token: 0x06005A24 RID: 23076 RVA: 0x001CD95A File Offset: 0x001CBB5A
		public static void ApproveAndLoadRoomMap()
		{
			CustomMapManager.currentRoomMapApproved = true;
			CMSSerializer.ResetSyncedMapObjects();
			CustomMapManager.LoadMap(CustomMapManager.currentRoomMapModId);
		}

		// Token: 0x06005A25 RID: 23077 RVA: 0x001CD972 File Offset: 0x001CBB72
		public static void RequestEnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.EnableTeleportHUD(enteringVirtualStump);
			}
		}

		// Token: 0x06005A26 RID: 23078 RVA: 0x001CD988 File Offset: 0x001CBB88
		private void EnableTeleportHUD(bool enteringVirtualStump)
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(true);
				CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
				return;
			}
			if (this.teleportingHUDPrefab != null)
			{
				Camera main = Camera.main;
				if (main != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.teleportingHUDPrefab, main.transform);
					if (gameObject != null)
					{
						CustomMapManager.teleportingHUD = gameObject.GetComponent<VirtualStumpTeleportingHUD>();
						if (CustomMapManager.teleportingHUD != null)
						{
							CustomMapManager.teleportingHUD.Initialize(enteringVirtualStump);
						}
					}
				}
			}
		}

		// Token: 0x06005A27 RID: 23079 RVA: 0x001CDA19 File Offset: 0x001CBC19
		public static void DisableTeleportHUD()
		{
			if (CustomMapManager.teleportingHUD != null)
			{
				CustomMapManager.teleportingHUD.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005A28 RID: 23080 RVA: 0x001CDA38 File Offset: 0x001CBC38
		public static void LoadZoneTriggered(int[] scenesToLoad, int[] scenesToUnload)
		{
			CustomMapLoader.LoadZoneTriggered(scenesToLoad, scenesToUnload, new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
		}

		// Token: 0x06005A29 RID: 23081 RVA: 0x001CDA59 File Offset: 0x001CBC59
		private static void OnSceneLoaded(string sceneName)
		{
			CMSSerializer.ProcessSceneLoad(sceneName);
			CustomMapManager.ProcessZoneShaderSettings(sceneName);
		}

		// Token: 0x06005A2A RID: 23082 RVA: 0x001CDA68 File Offset: 0x001CBC68
		private static void OnSceneUnloaded(string sceneName)
		{
			CMSSerializer.UnregisterTriggers(sceneName);
			for (int i = CustomMapManager.allCustomMapZoneShaderSettings.Count - 1; i >= 0; i--)
			{
				if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNull())
				{
					CustomMapManager.allCustomMapZoneShaderSettings.RemoveAt(i);
				}
			}
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x001CDAB0 File Offset: 0x001CBCB0
		private static void OnSceneTriggerHistoryProcessed(string sceneName)
		{
			CapsuleCollider bodyCollider = GTPlayer.Instance.bodyCollider;
			SphereCollider headCollider = GTPlayer.Instance.headCollider;
			Vector3 vector = bodyCollider.transform.TransformPoint(bodyCollider.center);
			float num = Mathf.Max(bodyCollider.height, bodyCollider.radius) * GTPlayer.Instance.scale;
			Collider[] array = new Collider[100];
			Physics.OverlapSphereNonAlloc(vector, num, array);
			foreach (Collider collider in array)
			{
				if (collider != null && collider.gameObject.scene.name.Equals(sceneName))
				{
					CMSTrigger[] components = collider.gameObject.GetComponents<CMSTrigger>();
					for (int j = 0; j < components.Length; j++)
					{
						if (components[j] != null)
						{
							components[j].OnTriggerEnter(bodyCollider);
							components[j].OnTriggerEnter(headCollider);
						}
					}
					CMSLoadingZone[] components2 = collider.gameObject.GetComponents<CMSLoadingZone>();
					for (int k = 0; k < components2.Length; k++)
					{
						if (components2[k] != null)
						{
							components2[k].OnTriggerEnter(bodyCollider);
						}
					}
					CMSZoneShaderSettingsTrigger[] components3 = collider.gameObject.GetComponents<CMSZoneShaderSettingsTrigger>();
					for (int l = 0; l < components3.Length; l++)
					{
						if (components3[l] != null)
						{
							components3[l].OnTriggerEnter(bodyCollider);
						}
					}
					HoverboardAreaTrigger[] components4 = collider.gameObject.GetComponents<HoverboardAreaTrigger>();
					for (int m = 0; m < components4.Length; m++)
					{
						if (components4[m] != null)
						{
							components4[m].OnTriggerEnter(headCollider);
						}
					}
					WaterVolume[] components5 = collider.gameObject.GetComponents<WaterVolume>();
					for (int n = 0; n < components5.Length; n++)
					{
						if (components5[n] != null)
						{
							components5[n].OnTriggerEnter(bodyCollider);
							components5[n].OnTriggerEnter(headCollider);
						}
					}
				}
			}
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x001CDC8F File Offset: 0x001CBE8F
		public static void SetDefaultZoneShaderSettings(ZoneShaderSettings defaultCustomMapShaderSettings, CMSZoneShaderSettings.CMSZoneShaderProperties defaultZoneShaderProperties)
		{
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.CopySettings(defaultCustomMapShaderSettings, true);
				CustomMapManager.loadedCustomMapDefaultZoneShaderSettings = defaultCustomMapShaderSettings;
				CustomMapManager.customMapDefaultZoneShaderProperties = defaultZoneShaderProperties;
				CustomMapManager.customMapDefaultZoneShaderSettingsInitialized = true;
			}
		}

		// Token: 0x06005A2D RID: 23085 RVA: 0x001CDCC0 File Offset: 0x001CBEC0
		private static void ProcessZoneShaderSettings(string loadedSceneName)
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized && CustomMapManager.customMapDefaultZoneShaderProperties.isInitialized)
			{
				for (int i = 0; i < CustomMapManager.allCustomMapZoneShaderSettings.Count; i++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[i].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[i] != CustomMapManager.loadedCustomMapDefaultZoneShaderSettings && CustomMapManager.allCustomMapZoneShaderSettings[i].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[i].ReplaceDefaultValues(CustomMapManager.customMapDefaultZoneShaderProperties, true);
					}
				}
				return;
			}
			if (CustomMapManager.hasInstance && CustomMapManager.instance.virtualStumpZoneShaderSettings.IsNotNull())
			{
				for (int j = 0; j < CustomMapManager.allCustomMapZoneShaderSettings.Count; j++)
				{
					if (CustomMapManager.allCustomMapZoneShaderSettings[j].IsNotNull() && CustomMapManager.allCustomMapZoneShaderSettings[j].gameObject.scene.name.Equals(loadedSceneName))
					{
						CustomMapManager.allCustomMapZoneShaderSettings[j].ReplaceDefaultValues(CustomMapManager.instance.virtualStumpZoneShaderSettings, true);
					}
				}
			}
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x001CDDEA File Offset: 0x001CBFEA
		public static void AddZoneShaderSettings(ZoneShaderSettings zoneShaderSettings)
		{
			CustomMapManager.allCustomMapZoneShaderSettings.AddIfNew(zoneShaderSettings);
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x001CDDF7 File Offset: 0x001CBFF7
		public static void ActivateDefaultZoneShaderSettings()
		{
			if (CustomMapManager.hasInstance && CustomMapManager.customMapDefaultZoneShaderSettingsInitialized)
			{
				CustomMapManager.instance.customMapDefaultZoneShaderSettings.BecomeActiveInstance(true);
				return;
			}
			if (CustomMapManager.hasInstance)
			{
				CustomMapManager.instance.virtualStumpZoneShaderSettings.BecomeActiveInstance(true);
			}
		}

		// Token: 0x06005A30 RID: 23088 RVA: 0x001CDE34 File Offset: 0x001CC034
		public static void ReturnToVirtualStump()
		{
			if (!CustomMapManager.hasInstance)
			{
				return;
			}
			if (!GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				return;
			}
			if (CustomMapManager.instance.returnToVirtualStumpTeleportLocation.IsNotNull())
			{
				GTPlayer gtplayer = GTPlayer.Instance;
				if (gtplayer != null)
				{
					CustomMapLoader.ResetToInitialZone(new Action<string>(CustomMapManager.OnSceneLoaded), new Action<string>(CustomMapManager.OnSceneUnloaded));
					gtplayer.TeleportTo(CustomMapManager.instance.returnToVirtualStumpTeleportLocation, true, false);
				}
			}
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x001CDEAB File Offset: 0x001CC0AB
		public static bool WantsHoldingHandsDisabled()
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				if (!CustomMapLoader.IsMapLoaded())
				{
					return true;
				}
				if (CustomMapLoader.LoadedMapWantsHoldingHandsDisabled())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04006708 RID: 26376
		[OnEnterPlay_SetNull]
		private static volatile CustomMapManager instance;

		// Token: 0x04006709 RID: 26377
		[OnEnterPlay_Set(false)]
		private static bool hasInstance = false;

		// Token: 0x0400670A RID: 26378
		[SerializeField]
		private GameObject virtualStumpToggleableRoot;

		// Token: 0x0400670B RID: 26379
		[SerializeField]
		private Transform returnToVirtualStumpTeleportLocation;

		// Token: 0x0400670C RID: 26380
		[SerializeField]
		private List<Transform> virtualStumpTeleportLocations;

		// Token: 0x0400670D RID: 26381
		[SerializeField]
		private GameObject[] rootObjectsToDeactivateAfterTeleport;

		// Token: 0x0400670E RID: 26382
		[SerializeField]
		private GorillaFriendCollider virtualStumpPlayerDetector;

		// Token: 0x0400670F RID: 26383
		[SerializeField]
		private ZoneShaderSettings virtualStumpZoneShaderSettings;

		// Token: 0x04006710 RID: 26384
		[SerializeField]
		private BetterDayNightManager dayNightManager;

		// Token: 0x04006711 RID: 26385
		[SerializeField]
		private GhostReactorManager ghostReactorManager;

		// Token: 0x04006712 RID: 26386
		[SerializeField]
		private GRReviveStation defaultReviveStation;

		// Token: 0x04006713 RID: 26387
		[SerializeField]
		private ZoneShaderSettings customMapDefaultZoneShaderSettings;

		// Token: 0x04006714 RID: 26388
		[SerializeField]
		private GameObject teleportingHUDPrefab;

		// Token: 0x04006715 RID: 26389
		[SerializeField]
		private AudioSource localTeleportSFXSource;

		// Token: 0x04006716 RID: 26390
		[SerializeField]
		private VirtualStumpTeleporter defaultTeleporter;

		// Token: 0x04006717 RID: 26391
		[SerializeField]
		private float maxPostTeleportRoomProcessingTime = 15f;

		// Token: 0x04006718 RID: 26392
		private static VirtualStumpTeleporter lastUsedTeleporter;

		// Token: 0x04006719 RID: 26393
		private static string preVStumpGamemode = "";

		// Token: 0x0400671A RID: 26394
		private static bool customMapDefaultZoneShaderSettingsInitialized;

		// Token: 0x0400671B RID: 26395
		private static ZoneShaderSettings loadedCustomMapDefaultZoneShaderSettings;

		// Token: 0x0400671C RID: 26396
		private static CMSZoneShaderSettings.CMSZoneShaderProperties customMapDefaultZoneShaderProperties;

		// Token: 0x0400671D RID: 26397
		private static readonly List<ZoneShaderSettings> allCustomMapZoneShaderSettings = new List<ZoneShaderSettings>();

		// Token: 0x0400671E RID: 26398
		private static bool loadInProgress = false;

		// Token: 0x0400671F RID: 26399
		private static ModId loadingMapId = ModId.Null;

		// Token: 0x04006720 RID: 26400
		private static bool unloadInProgress = false;

		// Token: 0x04006721 RID: 26401
		private static ModId unloadingMapId = ModId.Null;

		// Token: 0x04006722 RID: 26402
		private static List<ModId> abortModLoadIds = new List<ModId>();

		// Token: 0x04006723 RID: 26403
		private static bool waitingForModDownload = false;

		// Token: 0x04006724 RID: 26404
		private static bool waitingForModInstall = false;

		// Token: 0x04006725 RID: 26405
		private static ModId waitingForModInstallId = ModId.Null;

		// Token: 0x04006726 RID: 26406
		private static bool preTeleportInPrivateRoom = false;

		// Token: 0x04006727 RID: 26407
		private static string pendingNewPrivateRoomName = "";

		// Token: 0x04006728 RID: 26408
		private static Action<bool> currentTeleportCallback;

		// Token: 0x04006729 RID: 26409
		private static bool waitingForLoginDisconnect = false;

		// Token: 0x0400672A RID: 26410
		private static bool waitingForDisconnect = false;

		// Token: 0x0400672B RID: 26411
		private static bool waitingForRoomJoin = false;

		// Token: 0x0400672C RID: 26412
		private static bool shouldRetryJoin = false;

		// Token: 0x0400672D RID: 26413
		private static short pendingTeleportVFXIdx = -1;

		// Token: 0x0400672E RID: 26414
		private static bool exitVirtualStumpPending = false;

		// Token: 0x0400672F RID: 26415
		private static ModId currentRoomMapModId = ModId.Null;

		// Token: 0x04006730 RID: 26416
		private static bool currentRoomMapApproved = false;

		// Token: 0x04006731 RID: 26417
		private static VirtualStumpTeleportingHUD teleportingHUD;

		// Token: 0x04006732 RID: 26418
		private static Coroutine delayedEndTeleportCoroutine;

		// Token: 0x04006733 RID: 26419
		private static Coroutine delayedJoinCoroutine;

		// Token: 0x04006734 RID: 26420
		private static Coroutine delayedTryAutoLoadCoroutine;

		// Token: 0x04006735 RID: 26421
		public static UnityEvent<ModId> OnRoomMapChanged = new UnityEvent<ModId>();

		// Token: 0x04006736 RID: 26422
		public static UnityEvent<MapLoadStatus, int, string> OnMapLoadStatusChanged = new UnityEvent<MapLoadStatus, int, string>();

		// Token: 0x04006737 RID: 26423
		public static UnityEvent<bool> OnMapLoadComplete = new UnityEvent<bool>();

		// Token: 0x04006738 RID: 26424
		public static UnityEvent OnMapUnloadComplete = new UnityEvent();
	}
}
