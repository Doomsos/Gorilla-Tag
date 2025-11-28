using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using Photon.Pun;
using UnityEngine;

// Token: 0x020009B3 RID: 2483
internal class VirtualStumpSerializer : GorillaSerializer
{
	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x06003F60 RID: 16224 RVA: 0x00109E41 File Offset: 0x00108041
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06003F61 RID: 16225 RVA: 0x00154110 File Offset: 0x00152310
	protected void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnLeftRoom);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06003F62 RID: 16226 RVA: 0x00154180 File Offset: 0x00152380
	private void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int driverID = CustomMapsTerminal.GetDriverID();
		if (leavingPlayer.ActorNumber == driverID)
		{
			CustomMapsTerminal.SetTerminalControlStatus(false, -2, true);
		}
	}

	// Token: 0x06003F63 RID: 16227 RVA: 0x001541B2 File Offset: 0x001523B2
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			VirtualStumpSerializer.roomInitialized = true;
			return;
		}
		Debug.Log("[VirtualStumpSerializer::OnJoinedRoom] Requesting Room Initialization...");
		VirtualStumpSerializer.waitingForRoomInitialization = true;
		base.SendRPC("RequestRoomInitialization_RPC", false, Array.Empty<object>());
	}

	// Token: 0x06003F64 RID: 16228 RVA: 0x001541E8 File Offset: 0x001523E8
	private void OnLeftRoom()
	{
		Debug.Log("[VirtualStumpSerializer::OnLeftRoom]...");
		VirtualStumpSerializer.roomInitialized = false;
	}

	// Token: 0x06003F65 RID: 16229 RVA: 0x001541FA File Offset: 0x001523FA
	public static bool IsWaitingForRoomInit()
	{
		return VirtualStumpSerializer.waitingForRoomInitialization || !VirtualStumpSerializer.roomInitialized;
	}

	// Token: 0x06003F66 RID: 16230 RVA: 0x00154210 File Offset: 0x00152410
	[PunRPC]
	private void RequestRoomInitialization_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestRoomInitialization_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization);
		long id = CustomMapManager.GetRoomMapId()._id;
		base.SendRPC("InitializeRoom_RPC", info.Sender, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.GetDriverID(),
			CustomMapsTerminal.LocalModDetailsID,
			id
		});
	}

	// Token: 0x06003F67 RID: 16231 RVA: 0x001542AC File Offset: 0x001524AC
	[PunRPC]
	private void InitializeRoom_RPC(int currentScreen, int driverID, long modDetailsID, long loadedMapModID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "InitializeRoom_RPC");
		if (!info.Sender.IsMasterClient || !VirtualStumpSerializer.waitingForRoomInitialization)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
		if (loadedMapModID > 0L)
		{
			CustomMapManager.SetRoomMap(loadedMapModID);
		}
		VirtualStumpSerializer.roomInitialized = true;
		VirtualStumpSerializer.waitingForRoomInitialization = false;
		Debug.Log("[VStumpSerializer.InitializeRPC] Room initialization finished.");
	}

	// Token: 0x06003F68 RID: 16232 RVA: 0x0015431C File Offset: 0x0015251C
	public void LoadMapSynced(long modId)
	{
		CustomMapManager.SetRoomMap(modId);
		CustomMapManager.LoadMap(new ModId(modId));
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("SetRoomMap_RPC", true, new object[]
			{
				modId
			});
		}
	}

	// Token: 0x06003F69 RID: 16233 RVA: 0x0015436E File Offset: 0x0015256E
	public void UnloadMapSynced()
	{
		CustomMapManager.UnloadMap(true);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("UnloadMap_RPC", true, Array.Empty<object>());
		}
	}

	// Token: 0x06003F6A RID: 16234 RVA: 0x001543A0 File Offset: 0x001525A0
	[PunRPC]
	private void SetRoomMap_RPC(long modId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetRoomMap_RPC");
		if (modId <= 0L)
		{
			return;
		}
		if (info.Sender.ActorNumber != this.photonView.OwnerActorNr && info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (modId != this.detailsScreen.currentMapMod.Id._id)
		{
			return;
		}
		CustomMapManager.SetRoomMap(modId);
	}

	// Token: 0x06003F6B RID: 16235 RVA: 0x00154408 File Offset: 0x00152608
	[PunRPC]
	private void UnloadMap_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UnloadMap_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (!CustomMapManager.AreAllPlayersInVirtualStump())
		{
			return;
		}
		CustomMapManager.UnloadMap(true);
	}

	// Token: 0x06003F6C RID: 16236 RVA: 0x00154437 File Offset: 0x00152637
	public void RequestTerminalControlStatusChange(bool lockedStatus)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RequestTerminalControlStatusChange_RPC", false, new object[]
		{
			lockedStatus
		});
	}

	// Token: 0x06003F6D RID: 16237 RVA: 0x00154464 File Offset: 0x00152664
	[PunRPC]
	private void RequestTerminalControlStatusChange_RPC(bool lockedStatus, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestTerminalControlStatusChange_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[19].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!player.IsNull && CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(lockedStatus, info.Sender.ActorNumber);
		}
	}

	// Token: 0x06003F6E RID: 16238 RVA: 0x001544F9 File Offset: 0x001526F9
	public void SetTerminalControlStatus(bool locked, int playerID)
	{
		if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		base.SendRPC("SetTerminalControlStatus_RPC", true, new object[]
		{
			locked,
			playerID
		});
	}

	// Token: 0x06003F6F RID: 16239 RVA: 0x00154538 File Offset: 0x00152738
	[PunRPC]
	private void SetTerminalControlStatus_RPC(bool locked, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetTerminalControlStatus_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[16].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(locked, driverID, false);
	}

	// Token: 0x06003F70 RID: 16240 RVA: 0x001545BE File Offset: 0x001527BE
	public void SendTerminalStatus()
	{
		if (!NetworkSystem.Instance.InRoom || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.statusUpdateCoroutine != null)
		{
			base.StopCoroutine(this.statusUpdateCoroutine);
		}
		this.statusUpdateCoroutine = base.StartCoroutine(this.WaitToSendStatus());
	}

	// Token: 0x06003F71 RID: 16241 RVA: 0x001545FA File Offset: 0x001527FA
	private IEnumerator WaitToSendStatus()
	{
		yield return new WaitForSeconds(0.5f);
		base.SendRPC("UpdateScreen_RPC", true, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.LocalModDetailsID,
			CustomMapsTerminal.GetDriverID()
		});
		yield break;
	}

	// Token: 0x06003F72 RID: 16242 RVA: 0x0015460C File Offset: 0x0015280C
	[PunRPC]
	private void UpdateScreen_RPC(int currentScreen, long modDetailsID, int driverID, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "UpdateScreen_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID() || !CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			return;
		}
		if (currentScreen < -1 || currentScreen > 6)
		{
			return;
		}
		if (NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[17].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
	}

	// Token: 0x06003F73 RID: 16243 RVA: 0x001546B1 File Offset: 0x001528B1
	public void RefreshDriverNickName()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RefreshDriverNickName_RPC", true, Array.Empty<object>());
	}

	// Token: 0x06003F74 RID: 16244 RVA: 0x001546D4 File Offset: 0x001528D4
	[PunRPC]
	private void RefreshDriverNickName_RPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RefreshDriverNickName_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[18].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x04005098 RID: 20632
	[SerializeField]
	private VirtualStumpBarrierSFX barrierSFX;

	// Token: 0x04005099 RID: 20633
	[SerializeField]
	private CustomMapsDisplayScreen detailsScreen;

	// Token: 0x0400509A RID: 20634
	private static bool waitingForRoomInitialization;

	// Token: 0x0400509B RID: 20635
	private static bool roomInitialized;

	// Token: 0x0400509C RID: 20636
	private bool sendModList;

	// Token: 0x0400509D RID: 20637
	private bool forceNewSearch;

	// Token: 0x0400509E RID: 20638
	private bool waitToSendStatus;

	// Token: 0x0400509F RID: 20639
	private bool sendNewStatus;

	// Token: 0x040050A0 RID: 20640
	private const float STATUS_UPDATE_INTERVAL = 0.5f;

	// Token: 0x040050A1 RID: 20641
	private Coroutine statusUpdateCoroutine;
}
