using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using GorillaNetworking;
using GorillaTag;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;

// Token: 0x020003CE RID: 974
public abstract class NetworkSystem : MonoBehaviour
{
	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600177C RID: 6012 RVA: 0x00080DC0 File Offset: 0x0007EFC0
	// (set) Token: 0x0600177D RID: 6013 RVA: 0x00080DC8 File Offset: 0x0007EFC8
	public bool groupJoinInProgress { get; protected set; }

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x0600177E RID: 6014 RVA: 0x00080DD1 File Offset: 0x0007EFD1
	// (set) Token: 0x0600177F RID: 6015 RVA: 0x00080DD9 File Offset: 0x0007EFD9
	public NetSystemState netState
	{
		get
		{
			return this.testState;
		}
		protected set
		{
			Debug.Log("netstate set to:" + value.ToString());
			this.testState = value;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001780 RID: 6016 RVA: 0x00080DFE File Offset: 0x0007EFFE
	public NetPlayer LocalPlayer
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsLocal);
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001781 RID: 6017 RVA: 0x00080E2A File Offset: 0x0007F02A
	public virtual bool IsMasterClient { get; }

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001782 RID: 6018 RVA: 0x00080E32 File Offset: 0x0007F032
	public virtual NetPlayer MasterClient
	{
		get
		{
			return this.netPlayerCache.Find((NetPlayer p) => p.IsMasterClient);
		}
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001783 RID: 6019 RVA: 0x00080E5E File Offset: 0x0007F05E
	public Recorder LocalRecorder
	{
		get
		{
			return this.localRecorder;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001784 RID: 6020 RVA: 0x00080E66 File Offset: 0x0007F066
	public Speaker LocalSpeaker
	{
		get
		{
			return this.localSpeaker;
		}
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x00080E6E File Offset: 0x0007F06E
	protected void JoinedNetworkRoom()
	{
		VRRigCache.Instance.OnJoinedRoom();
		DelegateListProcessor onJoinedRoomEvent = this.OnJoinedRoomEvent;
		if (onJoinedRoomEvent == null)
		{
			return;
		}
		onJoinedRoomEvent.InvokeSafe();
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x00080E8A File Offset: 0x0007F08A
	internal void MultiplayerStarted()
	{
		DelegateListProcessor onMultiplayerStarted = this.OnMultiplayerStarted;
		if (onMultiplayerStarted == null)
		{
			return;
		}
		onMultiplayerStarted.InvokeSafe();
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00080E9C File Offset: 0x0007F09C
	protected void SinglePlayerStarted()
	{
		try
		{
			DelegateListProcessor onReturnedToSinglePlayer = this.OnReturnedToSinglePlayer;
			if (onReturnedToSinglePlayer != null)
			{
				onReturnedToSinglePlayer.InvokeSafe();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnLeftRoom();
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x00080EE0 File Offset: 0x0007F0E0
	protected void PlayerJoined(NetPlayer netPlayer)
	{
		if (this.IsOnline)
		{
			VRRigCache.Instance.OnPlayerEnteredRoom(netPlayer);
			DelegateListProcessor<NetPlayer> onPlayerJoined = this.OnPlayerJoined;
			if (onPlayerJoined == null)
			{
				return;
			}
			onPlayerJoined.InvokeSafe(netPlayer);
		}
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x00080F08 File Offset: 0x0007F108
	protected void PlayerLeft(NetPlayer netPlayer)
	{
		try
		{
			DelegateListProcessor<NetPlayer> onPlayerLeft = this.OnPlayerLeft;
			if (onPlayerLeft != null)
			{
				onPlayerLeft.InvokeSafe(netPlayer);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		VRRigCache.Instance.OnPlayerLeftRoom(netPlayer);
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x00080F4C File Offset: 0x0007F14C
	protected void OnMasterClientSwitchedCallback(NetPlayer nMaster)
	{
		DelegateListProcessor<NetPlayer> onMasterClientSwitchedEvent = this.OnMasterClientSwitchedEvent;
		if (onMasterClientSwitchedEvent == null)
		{
			return;
		}
		onMasterClientSwitchedEvent.InvokeSafe(nMaster);
	}

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x0600178B RID: 6027 RVA: 0x00080F60 File Offset: 0x0007F160
	// (remove) Token: 0x0600178C RID: 6028 RVA: 0x00080F98 File Offset: 0x0007F198
	public event Action<byte, object, int> OnRaiseEvent;

	// Token: 0x0600178D RID: 6029 RVA: 0x00080FCD File Offset: 0x0007F1CD
	internal void RaiseEvent(byte eventCode, object data, int source)
	{
		Action<byte, object, int> onRaiseEvent = this.OnRaiseEvent;
		if (onRaiseEvent == null)
		{
			return;
		}
		onRaiseEvent.Invoke(eventCode, data, source);
	}

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x0600178E RID: 6030 RVA: 0x00080FE4 File Offset: 0x0007F1E4
	// (remove) Token: 0x0600178F RID: 6031 RVA: 0x0008101C File Offset: 0x0007F21C
	public event Action<Dictionary<string, object>> OnCustomAuthenticationResponse;

	// Token: 0x06001790 RID: 6032 RVA: 0x00081051 File Offset: 0x0007F251
	internal void CustomAuthenticationResponse(Dictionary<string, object> response)
	{
		Action<Dictionary<string, object>> onCustomAuthenticationResponse = this.OnCustomAuthenticationResponse;
		if (onCustomAuthenticationResponse == null)
		{
			return;
		}
		onCustomAuthenticationResponse.Invoke(response);
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x00081064 File Offset: 0x0007F264
	public virtual void Initialise()
	{
		Debug.Log("INITIALISING NETWORKSYSTEMS");
		if (NetworkSystem.Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NetworkSystem.Instance = this;
		NetCrossoverUtils.Prewarm();
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void Update()
	{
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x00081093 File Offset: 0x0007F293
	public void RegisterSceneNetworkItem(GameObject item)
	{
		if (!this.SceneObjectsToAttach.Contains(item))
		{
			this.SceneObjectsToAttach.Add(item);
		}
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x000810AF File Offset: 0x0007F2AF
	public virtual void AttachObjectInGame(GameObject item)
	{
		this.RegisterSceneNetworkItem(item);
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void DetatchSceneObjectInGame(GameObject item)
	{
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x000810B8 File Offset: 0x0007F2B8
	public virtual AuthenticationValues GetAuthenticationValues()
	{
		Debug.LogWarning("NetworkSystem.GetAuthenticationValues should be overridden");
		return new AuthenticationValues();
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x000810C9 File Offset: 0x0007F2C9
	public virtual void SetAuthenticationValues(AuthenticationValues authValues)
	{
		Debug.LogWarning("NetworkSystem.SetAuthenticationValues should be overridden");
	}

	// Token: 0x06001798 RID: 6040
	public abstract void FinishAuthenticating();

	// Token: 0x06001799 RID: 6041
	public abstract Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1);

	// Token: 0x0600179A RID: 6042
	public abstract Task JoinFriendsRoom(string userID, int actorID, string keyToFollow, string shufflerToFollow);

	// Token: 0x0600179B RID: 6043
	public abstract Task ReturnToSinglePlayer();

	// Token: 0x0600179C RID: 6044
	public abstract void JoinPubWithFriends();

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x0600179D RID: 6045 RVA: 0x000810D5 File Offset: 0x0007F2D5
	public bool WrongVersion
	{
		get
		{
			return this.isWrongVersion;
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x000810DD File Offset: 0x0007F2DD
	public void SetWrongVersion()
	{
		this.isWrongVersion = true;
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x000810E6 File Offset: 0x0007F2E6
	public GameObject NetInstantiate(GameObject prefab, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, false);
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x000810FA File Offset: 0x0007F2FA
	public GameObject NetInstantiate(GameObject prefab, Vector3 position, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, Quaternion.identity, false);
	}

	// Token: 0x060017A1 RID: 6049
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false);

	// Token: 0x060017A2 RID: 6050
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false);

	// Token: 0x060017A3 RID: 6051
	public abstract GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null);

	// Token: 0x060017A4 RID: 6052
	public abstract void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null);

	// Token: 0x060017A5 RID: 6053
	public abstract void NetDestroy(GameObject instance);

	// Token: 0x060017A6 RID: 6054
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true);

	// Token: 0x060017A7 RID: 6055
	public abstract void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true) where T : struct;

	// Token: 0x060017A8 RID: 6056
	public abstract void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true);

	// Token: 0x060017A9 RID: 6057
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod);

	// Token: 0x060017AA RID: 6058
	public abstract void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args) where T : struct;

	// Token: 0x060017AB RID: 6059
	public abstract void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message);

	// Token: 0x060017AC RID: 6060 RVA: 0x0008110C File Offset: 0x0007F30C
	public static string GetRandomRoomName()
	{
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			text = GorillaComputer.instance.VStumpRoomPrepend + text;
		}
		if (GorillaComputer.instance.CheckAutoBanListForName(text))
		{
			return text;
		}
		return NetworkSystem.GetRandomRoomName();
	}

	// Token: 0x060017AD RID: 6061
	public abstract string GetRandomWeightedRegion();

	// Token: 0x060017AE RID: 6062 RVA: 0x00081184 File Offset: 0x0007F384
	protected Task RefreshNonce()
	{
		NetworkSystem.<RefreshNonce>d__89 <RefreshNonce>d__;
		<RefreshNonce>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshNonce>d__.<>4__this = this;
		<RefreshNonce>d__.<>1__state = -1;
		<RefreshNonce>d__.<>t__builder.Start<NetworkSystem.<RefreshNonce>d__89>(ref <RefreshNonce>d__);
		return <RefreshNonce>d__.<>t__builder.Task;
	}

	// Token: 0x060017AF RID: 6063 RVA: 0x000811C8 File Offset: 0x0007F3C8
	private void GetSteamAuthTicketSuccessCallback(string ticket)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Nonce"] = ticket;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
			this.nonceRefreshed = true;
		}
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x00081212 File Offset: 0x0007F412
	private void GetSteamAuthTicketFailureCallback(EResult result)
	{
		base.StartCoroutine(this.ReGetNonce());
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x00081221 File Offset: 0x0007F421
	private IEnumerator ReGetNonce()
	{
		yield return new WaitForSeconds(3f);
		PlayFabAuthenticator.instance.RefreshSteamAuthTicketForPhoton(new Action<string>(this.GetSteamAuthTicketSuccessCallback), new Action<EResult>(this.GetSteamAuthTicketFailureCallback));
		yield return null;
		yield break;
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00081230 File Offset: 0x0007F430
	public void BroadcastMyRoom(bool create, string key, string shuffler)
	{
		string text = NetworkSystem.ShuffleRoomName(NetworkSystem.Instance.RoomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		Debug.Log(string.Format("Broadcasting room {0} region {1}({2}). Create: {3} key: {4} (shuffler {5}) shuffled: {6}", new object[]
		{
			NetworkSystem.Instance.RoomName,
			NetworkSystem.Instance.currentRegionIndex,
			NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex],
			create,
			key,
			shuffler,
			text
		}));
		GorillaServer instance = GorillaServer.Instance;
		BroadcastMyRoomRequest broadcastMyRoomRequest = new BroadcastMyRoomRequest();
		broadcastMyRoomRequest.KeyToFollow = key;
		broadcastMyRoomRequest.RoomToJoin = text;
		broadcastMyRoomRequest.Set = create;
		instance.BroadcastMyRoom(broadcastMyRoomRequest, delegate(ExecuteFunctionResult result)
		{
		}, delegate(PlayFabError error)
		{
		});
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x00081348 File Offset: 0x0007F548
	public bool InstantCheckGroupData(string userID, string keyToFollow)
	{
		bool success = false;
		GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
		List<string> list = new List<string>();
		list.Add(keyToFollow);
		getSharedGroupDataRequest.Keys = list;
		getSharedGroupDataRequest.SharedGroupId = userID;
		PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
		{
			Debug.Log("Get Shared Group Data returned a success");
			Debug.Log(Extensions.ToStringFull(result.Data));
			if (result.Data.Count > 0)
			{
				success = true;
				return;
			}
			Debug.Log("RESULT returned but no DATA");
		}, delegate(PlayFabError error)
		{
			Debug.Log("ERROR - no group data found");
		}, null, null);
		return success;
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x000813B8 File Offset: 0x0007F5B8
	public NetPlayer GetNetPlayerByID(int playerActorNumber)
	{
		return this.netPlayerCache.Find((NetPlayer a) => a.ActorNumber == playerActorNumber);
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void NetRaiseEventReliable(byte eventCode, object data)
	{
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data)
	{
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions options)
	{
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x000813EC File Offset: 0x0007F5EC
	public static string ShuffleRoomName(string room, string shuffle, bool encode)
	{
		NetworkSystem.shuffleStringBuilder.Clear();
		int num;
		if (!int.TryParse(shuffle, ref num))
		{
			Debug.Log("Shuffle room failed");
			return "";
		}
		for (int i = 0; i < room.Length; i++)
		{
			int num2 = int.Parse(shuffle.Substring(i * 2 % (shuffle.Length - 1), 2));
			int num3 = NetworkSystem.mod("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".IndexOf(room.get_Chars(i)) + (encode ? num2 : (-num2)), "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Length);
			NetworkSystem.shuffleStringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".get_Chars(num3));
		}
		return NetworkSystem.shuffleStringBuilder.ToString();
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x0003D571 File Offset: 0x0003B771
	public static int mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	// Token: 0x060017BB RID: 6075
	public abstract Task AwaitSceneReady();

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x060017BC RID: 6076
	public abstract string CurrentPhotonBackend { get; }

	// Token: 0x060017BD RID: 6077
	public abstract NetPlayer GetLocalPlayer();

	// Token: 0x060017BE RID: 6078
	public abstract NetPlayer GetPlayer(int PlayerID);

	// Token: 0x060017BF RID: 6079 RVA: 0x00081494 File Offset: 0x0007F694
	public NetPlayer GetPlayer(Player punPlayer)
	{
		if (punPlayer == null)
		{
			return null;
		}
		NetPlayer netPlayer = this.FindPlayer(punPlayer);
		if (netPlayer == null)
		{
			this.UpdatePlayers();
			netPlayer = this.FindPlayer(punPlayer);
			if (netPlayer == null)
			{
				Debug.LogError(string.Format("There is no NetPlayer with this ID currently in game. Passed ID: {0} nickname {1}", punPlayer.ActorNumber, punPlayer.NickName));
				return null;
			}
		}
		return netPlayer;
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x000814E8 File Offset: 0x0007F6E8
	private NetPlayer FindPlayer(Player punPlayer)
	{
		for (int i = 0; i < this.netPlayerCache.Count; i++)
		{
			if (this.netPlayerCache[i].GetPlayerRef() == punPlayer)
			{
				return this.netPlayerCache[i];
			}
		}
		return null;
	}

	// Token: 0x060017C1 RID: 6081 RVA: 0x000743B1 File Offset: 0x000725B1
	public NetPlayer GetPlayer(PlayerRef playerRef)
	{
		return null;
	}

	// Token: 0x060017C2 RID: 6082
	public abstract void SetMyNickName(string name);

	// Token: 0x060017C3 RID: 6083
	public abstract string GetMyNickName();

	// Token: 0x060017C4 RID: 6084
	public abstract string GetMyDefaultName();

	// Token: 0x060017C5 RID: 6085
	public abstract string GetNickName(int playerID);

	// Token: 0x060017C6 RID: 6086
	public abstract string GetNickName(NetPlayer player);

	// Token: 0x060017C7 RID: 6087
	public abstract string GetMyUserID();

	// Token: 0x060017C8 RID: 6088
	public abstract string GetUserID(int playerID);

	// Token: 0x060017C9 RID: 6089
	public abstract string GetUserID(NetPlayer player);

	// Token: 0x060017CA RID: 6090
	public abstract void SetMyTutorialComplete();

	// Token: 0x060017CB RID: 6091
	public abstract bool GetMyTutorialCompletion();

	// Token: 0x060017CC RID: 6092
	public abstract bool GetPlayerTutorialCompletion(int playerID);

	// Token: 0x060017CD RID: 6093 RVA: 0x0008152D File Offset: 0x0007F72D
	public void AddVoiceSettings(SO_NetworkVoiceSettings settings)
	{
		this.VoiceSettings = settings;
	}

	// Token: 0x060017CE RID: 6094
	public abstract void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback);

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x060017CF RID: 6095
	public abstract VoiceConnection VoiceConnection { get; }

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x060017D0 RID: 6096
	public abstract bool IsOnline { get; }

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x060017D1 RID: 6097
	public abstract bool InRoom { get; }

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x060017D2 RID: 6098
	public abstract string RoomName { get; }

	// Token: 0x060017D3 RID: 6099
	public abstract string RoomStringStripped();

	// Token: 0x060017D4 RID: 6100 RVA: 0x00081538 File Offset: 0x0007F738
	public string RoomString()
	{
		return string.Format("Room: '{0}' {1},{2} {4}/{3} players.\ncustomProps: {5}", new object[]
		{
			this.RoomName,
			this.CurrentRoom.isPublic ? "visible" : "hidden",
			this.CurrentRoom.isJoinable ? "open" : "closed",
			this.CurrentRoom.MaxPlayers,
			this.RoomPlayerCount,
			Extensions.ToStringFull(this.CurrentRoom.CustomProps)
		});
	}

	// Token: 0x17000273 RID: 627
	// (get) Token: 0x060017D5 RID: 6101
	public abstract string GameModeString { get; }

	// Token: 0x17000274 RID: 628
	// (get) Token: 0x060017D6 RID: 6102
	public abstract string CurrentRegion { get; }

	// Token: 0x17000275 RID: 629
	// (get) Token: 0x060017D7 RID: 6103
	public abstract bool SessionIsPrivate { get; }

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x060017D8 RID: 6104
	public abstract int LocalPlayerID { get; }

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x060017D9 RID: 6105 RVA: 0x000815CA File Offset: 0x0007F7CA
	public virtual NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.netPlayerCache.ToArray();
		}
	}

	// Token: 0x17000278 RID: 632
	// (get) Token: 0x060017DA RID: 6106 RVA: 0x000815D7 File Offset: 0x0007F7D7
	public virtual NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.netPlayerCache.FindAll((NetPlayer p) => !p.IsLocal).ToArray();
		}
	}

	// Token: 0x060017DB RID: 6107
	protected abstract void UpdateNetPlayerList();

	// Token: 0x060017DC RID: 6108 RVA: 0x00081608 File Offset: 0x0007F808
	public void UpdatePlayers()
	{
		this.UpdateNetPlayerList();
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x060017DD RID: 6109
	public abstract double SimTime { get; }

	// Token: 0x1700027A RID: 634
	// (get) Token: 0x060017DE RID: 6110
	public abstract float SimDeltaTime { get; }

	// Token: 0x1700027B RID: 635
	// (get) Token: 0x060017DF RID: 6111
	public abstract int SimTick { get; }

	// Token: 0x1700027C RID: 636
	// (get) Token: 0x060017E0 RID: 6112
	public abstract int TickRate { get; }

	// Token: 0x1700027D RID: 637
	// (get) Token: 0x060017E1 RID: 6113
	public abstract int ServerTimestamp { get; }

	// Token: 0x1700027E RID: 638
	// (get) Token: 0x060017E2 RID: 6114
	public abstract int RoomPlayerCount { get; }

	// Token: 0x060017E3 RID: 6115
	public abstract int GlobalPlayerCount();

	// Token: 0x1700027F RID: 639
	// (get) Token: 0x060017E4 RID: 6116 RVA: 0x00081610 File Offset: 0x0007F810
	// (set) Token: 0x060017E5 RID: 6117 RVA: 0x00081618 File Offset: 0x0007F818
	public RoomConfig CurrentRoom { get; protected set; }

	// Token: 0x060017E6 RID: 6118
	public abstract bool IsObjectLocallyOwned(GameObject obj);

	// Token: 0x060017E7 RID: 6119
	public abstract bool IsObjectRoomObject(GameObject obj);

	// Token: 0x060017E8 RID: 6120
	public abstract bool ShouldUpdateObject(GameObject obj);

	// Token: 0x060017E9 RID: 6121
	public abstract bool ShouldWriteObjectData(GameObject obj);

	// Token: 0x060017EA RID: 6122
	public abstract int GetOwningPlayerID(GameObject obj);

	// Token: 0x060017EB RID: 6123
	public abstract bool ShouldSpawnLocally(int playerID);

	// Token: 0x060017EC RID: 6124
	public abstract bool IsTotalAuthority();

	// Token: 0x04002158 RID: 8536
	public static NetworkSystem Instance;

	// Token: 0x04002159 RID: 8537
	public NetworkSystemConfig config;

	// Token: 0x0400215A RID: 8538
	public bool changingSceneManually;

	// Token: 0x0400215B RID: 8539
	public string[] regionNames;

	// Token: 0x0400215C RID: 8540
	public int currentRegionIndex;

	// Token: 0x0400215E RID: 8542
	private bool nonceRefreshed;

	// Token: 0x0400215F RID: 8543
	protected bool isWrongVersion;

	// Token: 0x04002160 RID: 8544
	private NetSystemState testState;

	// Token: 0x04002161 RID: 8545
	protected List<NetPlayer> netPlayerCache = new List<NetPlayer>();

	// Token: 0x04002162 RID: 8546
	protected Recorder localRecorder;

	// Token: 0x04002163 RID: 8547
	protected Speaker localSpeaker;

	// Token: 0x04002165 RID: 8549
	public List<GameObject> SceneObjectsToAttach = new List<GameObject>();

	// Token: 0x04002166 RID: 8550
	protected SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x04002167 RID: 8551
	protected List<Action<RemoteVoiceLink>> remoteVoiceAddedCallbacks = new List<Action<RemoteVoiceLink>>();

	// Token: 0x04002168 RID: 8552
	public DelegateListProcessor OnJoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x04002169 RID: 8553
	public DelegateListProcessor OnMultiplayerStarted = new DelegateListProcessor();

	// Token: 0x0400216A RID: 8554
	public DelegateListProcessor OnReturnedToSinglePlayer = new DelegateListProcessor();

	// Token: 0x0400216B RID: 8555
	public DelegateListProcessor<NetPlayer> OnPlayerJoined = new DelegateListProcessor<NetPlayer>();

	// Token: 0x0400216C RID: 8556
	public DelegateListProcessor<NetPlayer> OnPlayerLeft = new DelegateListProcessor<NetPlayer>();

	// Token: 0x0400216D RID: 8557
	internal DelegateListProcessor<NetPlayer> OnMasterClientSwitchedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04002170 RID: 8560
	protected static readonly byte[] EmptyArgs = new byte[0];

	// Token: 0x04002171 RID: 8561
	public const string roomCharacters = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";

	// Token: 0x04002172 RID: 8562
	public const string shuffleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

	// Token: 0x04002173 RID: 8563
	private static StringBuilder shuffleStringBuilder = new StringBuilder(4);

	// Token: 0x04002174 RID: 8564
	protected static StringBuilder reusableSB = new StringBuilder();

	// Token: 0x020003CF RID: 975
	// (Invoke) Token: 0x060017F0 RID: 6128
	public delegate void RPC(byte[] data);

	// Token: 0x020003D0 RID: 976
	// (Invoke) Token: 0x060017F4 RID: 6132
	public delegate void StringRPC(string message);

	// Token: 0x020003D1 RID: 977
	// (Invoke) Token: 0x060017F8 RID: 6136
	public delegate void StaticRPC(byte[] data);

	// Token: 0x020003D2 RID: 978
	// (Invoke) Token: 0x060017FC RID: 6140
	public delegate void StaticRPCPlaceholder(byte[] args);
}
