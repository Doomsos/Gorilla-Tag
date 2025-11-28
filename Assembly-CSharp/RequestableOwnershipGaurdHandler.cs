using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x0200030D RID: 781
internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x060012E5 RID: 4837 RVA: 0x0006E320 File Offset: 0x0006C520
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x060012E6 RID: 4838 RVA: 0x0006E34A File Offset: 0x0006C54A
	internal static void RegisterView(NetworkView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x0006E37B File Offset: 0x0006C57B
	internal static void RemoveView(NetworkView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x0006E3A0 File Offset: 0x0006C5A0
	internal static void RegisterViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x0006E3C8 File Offset: 0x0006C5C8
	public static void RemoveViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x0006E3F0 File Offset: 0x0006C5F0
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		NetworkView networkView = Enumerable.FirstOrDefault<NetworkView>(RequestableOwnershipGaurdHandler.gaurdedViews, (NetworkView p) => p.GetView == targetView);
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (networkView.IsNull() || !RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, ref requestableOwnershipGuard) || requestableOwnershipGuard.IsNull())
		{
			return;
		}
		NetPlayer currentOwner = requestableOwnershipGuard.currentOwner;
		Player player = (currentOwner != null) ? currentOwner.GetPlayerRef() : null;
		int num = (player != null) ? player.ActorNumber : 0;
		if (num == 0 || previousOwner != player)
		{
			GTDev.LogWarning<string>("Ownership transferred but the previous owner didn't initiate the request, Switching back", null);
			targetView.OwnerActorNr = num;
			targetView.ControllerActorNr = num;
		}
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x0006E48F File Offset: 0x0006C68F
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x0006E48F File Offset: 0x0006C68F
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x0006E498 File Offset: 0x0006C698
	private void OnHostChangedShared()
	{
		foreach (NetworkView networkView in RequestableOwnershipGaurdHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, ref requestableOwnershipGuard))
			{
				break;
			}
			if (networkView.Owner != null && requestableOwnershipGuard.currentOwner != null && !object.Equals(networkView.Owner, requestableOwnershipGuard.currentOwner))
			{
				networkView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				networkView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00002789 File Offset: 0x00000989
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00002789 File Offset: 0x00000989
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00002789 File Offset: 0x00000989
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x00002789 File Offset: 0x00000989
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x00002789 File Offset: 0x00000989
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x00002789 File Offset: 0x00000989
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x00002789 File Offset: 0x00000989
	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x00002789 File Offset: 0x00000989
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x00002789 File Offset: 0x00000989
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x00002789 File Offset: 0x00000989
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06001301 RID: 4865 RVA: 0x00002789 File Offset: 0x00000989
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001303 RID: 4867 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x06001304 RID: 4868 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06001305 RID: 4869 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x04001CA0 RID: 7328
	private static HashSet<NetworkView> gaurdedViews = new HashSet<NetworkView>();

	// Token: 0x04001CA1 RID: 7329
	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	// Token: 0x04001CA2 RID: 7330
	private static Dictionary<NetworkView, RequestableOwnershipGuard> guardingLookup = new Dictionary<NetworkView, RequestableOwnershipGuard>();
}
