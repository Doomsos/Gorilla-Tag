using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon;
using Fusion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020007A6 RID: 1958
public class GorillaNot : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x0600331E RID: 13086 RVA: 0x0007BEF1 File Offset: 0x0007A0F1
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x0600331F RID: 13087 RVA: 0x00113D8F File Offset: 0x00111F8F
	// (set) Token: 0x06003320 RID: 13088 RVA: 0x00113D97 File Offset: 0x00111F97
	private bool sendReport
	{
		get
		{
			return this._sendReport;
		}
		set
		{
			if (!this._sendReport)
			{
				this._sendReport = true;
			}
		}
	}

	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x06003321 RID: 13089 RVA: 0x00113DA8 File Offset: 0x00111FA8
	// (set) Token: 0x06003322 RID: 13090 RVA: 0x00113DB0 File Offset: 0x00111FB0
	private string suspiciousPlayerId
	{
		get
		{
			return this._suspiciousPlayerId;
		}
		set
		{
			if (this._suspiciousPlayerId == "")
			{
				this._suspiciousPlayerId = value;
			}
		}
	}

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x06003323 RID: 13091 RVA: 0x00113DCB File Offset: 0x00111FCB
	// (set) Token: 0x06003324 RID: 13092 RVA: 0x00113DD3 File Offset: 0x00111FD3
	private string suspiciousPlayerName
	{
		get
		{
			return this._suspiciousPlayerName;
		}
		set
		{
			if (this._suspiciousPlayerName == "")
			{
				this._suspiciousPlayerName = value;
			}
		}
	}

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06003325 RID: 13093 RVA: 0x00113DEE File Offset: 0x00111FEE
	// (set) Token: 0x06003326 RID: 13094 RVA: 0x00113DF6 File Offset: 0x00111FF6
	private string suspiciousReason
	{
		get
		{
			return this._suspiciousReason;
		}
		set
		{
			if (this._suspiciousReason == "")
			{
				this._suspiciousReason = value;
			}
		}
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x00113E11 File Offset: 0x00112011
	public void SliceUpdate()
	{
		this.CheckReports();
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x00113E1C File Offset: 0x0011201C
	private void Start()
	{
		if (GorillaNot.instance == null)
		{
			GorillaNot.instance = this;
		}
		else if (GorillaNot.instance != this)
		{
			Object.Destroy(this);
		}
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.JoinedRoomEvent += delegate
		{
			this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		};
		this.logErrorCount = 0;
		Application.logMessageReceived += new Application.LogCallback(this.LogErrorCount);
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x00113EC0 File Offset: 0x001120C0
	private void OnApplicationPause(bool paused)
	{
		if (paused || !RoomSystem.JoinedRoom)
		{
			return;
		}
		this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
		this.RefreshRPCs();
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x00113EE4 File Offset: 0x001120E4
	public void LogErrorCount(string logString, string stackTrace, LogType type)
	{
		if (type == null)
		{
			this.logErrorCount++;
			this.stringIndex = logString.LastIndexOf("Sender is ");
			if (logString.Contains("RPC") && this.stringIndex >= 0)
			{
				this.playerID = logString.Substring(this.stringIndex + 10);
				this.tempPlayer = null;
				for (int i = 0; i < this.cachedPlayerList.Length; i++)
				{
					if (this.cachedPlayerList[i].UserId == this.playerID)
					{
						this.tempPlayer = this.cachedPlayerList[i];
						break;
					}
				}
				string text = "invalid RPC stuff";
				if (!this.IncrementRPCTracker(this.tempPlayer, text, this.rpcErrorMax))
				{
					this.SendReport("invalid RPC stuff", this.tempPlayer.UserId, this.tempPlayer.NickName);
				}
				this.tempPlayer = null;
			}
			if (this.logErrorCount > this.logErrorMax)
			{
				Debug.unityLogger.logEnabled = false;
			}
		}
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x00113FE8 File Offset: 0x001121E8
	public void SendReport(string susReason, string susId, string susNick)
	{
		this.suspiciousReason = susReason;
		this.suspiciousPlayerId = susId;
		this.suspiciousPlayerName = susNick;
		this.sendReport = true;
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x00114008 File Offset: 0x00112208
	[MethodImpl(256)]
	private void DispatchReport()
	{
		if ((this.sendReport || this.testAssault) && this.suspiciousPlayerId != "" && this.reportedPlayers.IndexOf(this.suspiciousPlayerId) == -1)
		{
			if (this._suspiciousPlayerName.Length > 12)
			{
				this._suspiciousPlayerName = this._suspiciousPlayerName.Remove(12);
			}
			this.reportedPlayers.Add(this.suspiciousPlayerId);
			this.testAssault = false;
			WebFlags flags = new WebFlags(3);
			NetEventOptions options = new NetEventOptions
			{
				TargetActors = GorillaNot.targetActors,
				Reciever = NetEventOptions.RecieverTarget.master,
				Flags = flags
			};
			string[] array = new string[this.cachedPlayerList.Length];
			int num = 0;
			foreach (NetPlayer netPlayer in this.cachedPlayerList)
			{
				array[num] = netPlayer.UserId;
				num++;
			}
			object[] data = new object[]
			{
				NetworkSystem.Instance.RoomStringStripped(),
				array,
				NetworkSystem.Instance.MasterClient.UserId,
				this.suspiciousPlayerId,
				this.suspiciousPlayerName,
				this.suspiciousReason,
				NetworkSystemConfig.AppVersion
			};
			NetworkSystemRaiseEvent.RaiseEvent(8, data, options, true);
			if (this.ShouldDisconnectFromRoom())
			{
				base.StartCoroutine(this.QuitDelay());
			}
		}
		this._sendReport = false;
		this._suspiciousPlayerId = "";
		this._suspiciousPlayerName = "";
		this._suspiciousReason = "";
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x0011418C File Offset: 0x0011238C
	private void CheckReports()
	{
		if (Time.time < this.lastCheck + this.reportCheckCooldown)
		{
			return;
		}
		this.lastCheck = Time.time;
		try
		{
			this.logErrorCount = 0;
			if (NetworkSystem.Instance.InRoom)
			{
				this.lastCheck = Time.time;
				this.lastServerTimestamp = NetworkSystem.Instance.SimTick;
				if (!PhotonNetwork.CurrentRoom.PublishUserId)
				{
					this.sendReport = true;
					this.suspiciousReason = "missing player ids";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.cachedPlayerList.Length > (int)RoomSystem.GetRoomSize(PhotonNetworkController.Instance.currentGameType))
				{
					this.sendReport = true;
					this.suspiciousReason = "too many players";
					this.SetToRoomCreatorIfHere();
					this.CloseInvalidRoom();
				}
				if (this.currentMasterClient != NetworkSystem.Instance.MasterClient || this.LowestActorNumber() != NetworkSystem.Instance.MasterClient.ActorNumber)
				{
					foreach (NetPlayer netPlayer in this.cachedPlayerList)
					{
						if (this.currentMasterClient == netPlayer)
						{
							this.sendReport = true;
							this.suspiciousReason = "room host force changed";
							this.suspiciousPlayerId = NetworkSystem.Instance.MasterClient.UserId;
							this.suspiciousPlayerName = NetworkSystem.Instance.MasterClient.NickName;
						}
					}
					this.currentMasterClient = NetworkSystem.Instance.MasterClient;
				}
				this.RefreshRPCs();
				this.DispatchReport();
			}
		}
		catch
		{
		}
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x00114314 File Offset: 0x00112514
	private void RefreshRPCs()
	{
		foreach (Dictionary<string, GorillaNot.RPCCallTracker> dictionary in this.userRPCCalls.Values)
		{
			foreach (GorillaNot.RPCCallTracker rpccallTracker in dictionary.Values)
			{
				rpccallTracker.RPCCalls = 0;
			}
		}
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x001143A4 File Offset: 0x001125A4
	private int LowestActorNumber()
	{
		this.lowestActorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		foreach (NetPlayer netPlayer in this.cachedPlayerList)
		{
			if (netPlayer.ActorNumber < this.lowestActorNumber)
			{
				this.lowestActorNumber = netPlayer.ActorNumber;
			}
		}
		return this.lowestActorNumber;
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x001143FF File Offset: 0x001125FF
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x0011441C File Offset: 0x0011261C
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.cachedPlayerList = (NetworkSystem.Instance.AllNetPlayers ?? new NetPlayer[0]);
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (this.userRPCCalls.TryGetValue(otherPlayer.UserId, ref dictionary))
		{
			this.userRPCCalls.Remove(otherPlayer.UserId);
		}
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x0011446A File Offset: 0x0011266A
	public static void IncrementRPCCall(PhotonMessageInfo info, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), callingMethod);
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x00114478 File Offset: 0x00112678
	public static void IncrementRPCCall(PhotonMessageInfoWrapped infoWrapped, [CallerMemberName] string callingMethod = "")
	{
		GorillaNot.instance.IncrementRPCCallLocal(infoWrapped, callingMethod);
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00114488 File Offset: 0x00112688
	private void IncrementRPCCallLocal(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
	{
		if (infoWrapped.sentTick < this.lastServerTimestamp)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(infoWrapped.senderID);
		if (player == null)
		{
			return;
		}
		string userId = player.UserId;
		if (!this.IncrementRPCTracker(userId, rpcFunction, this.rpcCallLimit))
		{
			this.SendReport("too many rpc calls! " + rpcFunction, player.UserId, player.NickName);
			return;
		}
	}

	// Token: 0x06003337 RID: 13111 RVA: 0x001144F0 File Offset: 0x001126F0
	private bool IncrementRPCTracker(in NetPlayer sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06003338 RID: 13112 RVA: 0x00114510 File Offset: 0x00112710
	private bool IncrementRPCTracker(in Player sender, in string rpcFunction, in int callLimit)
	{
		string userId = sender.UserId;
		return this.IncrementRPCTracker(userId, rpcFunction, callLimit);
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x00114530 File Offset: 0x00112730
	private bool IncrementRPCTracker(in string userId, in string rpcFunction, in int callLimit)
	{
		GorillaNot.RPCCallTracker rpccallTracker = this.GetRPCCallTracker(userId, rpcFunction);
		if (rpccallTracker == null)
		{
			return true;
		}
		rpccallTracker.RPCCalls++;
		if (rpccallTracker.RPCCalls > rpccallTracker.RPCCallsMax)
		{
			rpccallTracker.RPCCallsMax = rpccallTracker.RPCCalls;
		}
		return rpccallTracker.RPCCalls <= callLimit;
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x00114584 File Offset: 0x00112784
	private GorillaNot.RPCCallTracker GetRPCCallTracker(string userID, string rpcFunction)
	{
		if (userID == null)
		{
			return null;
		}
		GorillaNot.RPCCallTracker rpccallTracker = null;
		Dictionary<string, GorillaNot.RPCCallTracker> dictionary;
		if (!this.userRPCCalls.TryGetValue(userID, ref dictionary))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			Dictionary<string, GorillaNot.RPCCallTracker> dictionary2 = new Dictionary<string, GorillaNot.RPCCallTracker>();
			dictionary2.Add(rpcFunction, rpccallTracker);
			this.userRPCCalls.Add(userID, dictionary2);
		}
		else if (!dictionary.TryGetValue(rpcFunction, ref rpccallTracker))
		{
			rpccallTracker = new GorillaNot.RPCCallTracker
			{
				RPCCalls = 0,
				RPCCallsMax = 0
			};
			dictionary.Add(rpcFunction, rpccallTracker);
		}
		return rpccallTracker;
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x00114601 File Offset: 0x00112801
	private IEnumerator QuitDelay()
	{
		yield return new WaitForSeconds(1f);
		NetworkSystem.Instance.ReturnToSinglePlayer();
		yield break;
	}

	// Token: 0x0600333C RID: 13116 RVA: 0x0011460C File Offset: 0x0011280C
	private void SetToRoomCreatorIfHere()
	{
		this.tempPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1, false);
		if (this.tempPlayer != null)
		{
			this.suspiciousPlayerId = this.tempPlayer.UserId;
			this.suspiciousPlayerName = this.tempPlayer.NickName;
			return;
		}
		this.suspiciousPlayerId = "n/a";
		this.suspiciousPlayerName = "n/a";
	}

	// Token: 0x0600333D RID: 13117 RVA: 0x00114674 File Offset: 0x00112874
	private bool ShouldDisconnectFromRoom()
	{
		return this._suspiciousReason.Contains("too many players") || this._suspiciousReason.Contains("invalid room name") || this._suspiciousReason.Contains("invalid game mode") || this._suspiciousReason.Contains("missing player ids");
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x001146C9 File Offset: 0x001128C9
	private void CloseInvalidRoom()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.MaxPlayers = RoomSystem.GetRoomSize(PhotonNetworkController.Instance.currentGameType);
	}

	// Token: 0x040041A3 RID: 16803
	[OnEnterPlay_SetNull]
	public static volatile GorillaNot instance;

	// Token: 0x040041A4 RID: 16804
	private bool _sendReport;

	// Token: 0x040041A5 RID: 16805
	private string _suspiciousPlayerId = "";

	// Token: 0x040041A6 RID: 16806
	private string _suspiciousPlayerName = "";

	// Token: 0x040041A7 RID: 16807
	private string _suspiciousReason = "";

	// Token: 0x040041A8 RID: 16808
	internal List<string> reportedPlayers = new List<string>();

	// Token: 0x040041A9 RID: 16809
	public byte roomSize;

	// Token: 0x040041AA RID: 16810
	public float lastCheck;

	// Token: 0x040041AB RID: 16811
	public float userDecayTime = 15f;

	// Token: 0x040041AC RID: 16812
	public NetPlayer currentMasterClient;

	// Token: 0x040041AD RID: 16813
	public bool testAssault;

	// Token: 0x040041AE RID: 16814
	private const byte ReportAssault = 8;

	// Token: 0x040041AF RID: 16815
	private int lowestActorNumber;

	// Token: 0x040041B0 RID: 16816
	private int calls;

	// Token: 0x040041B1 RID: 16817
	public int rpcCallLimit = 50;

	// Token: 0x040041B2 RID: 16818
	public int logErrorMax = 50;

	// Token: 0x040041B3 RID: 16819
	public int rpcErrorMax = 10;

	// Token: 0x040041B4 RID: 16820
	private object outObj;

	// Token: 0x040041B5 RID: 16821
	private NetPlayer tempPlayer;

	// Token: 0x040041B6 RID: 16822
	private int logErrorCount;

	// Token: 0x040041B7 RID: 16823
	private int stringIndex;

	// Token: 0x040041B8 RID: 16824
	private string playerID;

	// Token: 0x040041B9 RID: 16825
	private string playerNick;

	// Token: 0x040041BA RID: 16826
	private int lastServerTimestamp;

	// Token: 0x040041BB RID: 16827
	private const string InvalidRPC = "invalid RPC stuff";

	// Token: 0x040041BC RID: 16828
	public NetPlayer[] cachedPlayerList;

	// Token: 0x040041BD RID: 16829
	private float lastReportChecked;

	// Token: 0x040041BE RID: 16830
	private float reportCheckCooldown = 1f;

	// Token: 0x040041BF RID: 16831
	private static int[] targetActors = new int[]
	{
		-1
	};

	// Token: 0x040041C0 RID: 16832
	private Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>> userRPCCalls = new Dictionary<string, Dictionary<string, GorillaNot.RPCCallTracker>>();

	// Token: 0x040041C1 RID: 16833
	private Hashtable hashTable;

	// Token: 0x020007A7 RID: 1959
	private class RPCCallTracker
	{
		// Token: 0x040041C2 RID: 16834
		public int RPCCalls;

		// Token: 0x040041C3 RID: 16835
		public int RPCCallsMax;
	}
}
