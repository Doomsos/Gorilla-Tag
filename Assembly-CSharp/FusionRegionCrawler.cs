using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

// Token: 0x020003A7 RID: 935
public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x0600166C RID: 5740 RVA: 0x0007C5D8 File Offset: 0x0007A7D8
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x0007C5E0 File Offset: 0x0007A7E0
	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x0007C615 File Offset: 0x0007A815
	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x0007C624 File Offset: 0x0007A824
	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string fixedRegion in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new FusionAppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = fixedRegion;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(1, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false), default(CancellationToken), true);
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x0007C633 File Offset: 0x0007A833
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001675 RID: 5749 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06001676 RID: 5750 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	// Token: 0x0600167D RID: 5757 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x0600167E RID: 5758 RVA: 0x00002789 File Offset: 0x00000989
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0400206A RID: 8298
	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	// Token: 0x0400206B RID: 8299
	private NetworkRunner regionRunner;

	// Token: 0x0400206C RID: 8300
	private List<SessionInfo> sessionInfoCache;

	// Token: 0x0400206D RID: 8301
	private bool waitingForSessionListUpdate;

	// Token: 0x0400206E RID: 8302
	private int globalPlayerCount;

	// Token: 0x0400206F RID: 8303
	private float UpdateFrequency = 10f;

	// Token: 0x04002070 RID: 8304
	private bool refreshPlayerCountAutomatically = true;

	// Token: 0x04002071 RID: 8305
	private int tempSessionPlayerCount;

	// Token: 0x020003A8 RID: 936
	// (Invoke) Token: 0x06001686 RID: 5766
	public delegate void PlayerCountUpdated(int playerCount);
}
