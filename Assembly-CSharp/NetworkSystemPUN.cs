using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020003E0 RID: 992
[RequireComponent(typeof(PUNCallbackNotifier))]
public class NetworkSystemPUN : NetworkSystem
{
	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06001846 RID: 6214 RVA: 0x0008202A File Offset: 0x0008022A
	public override NetPlayer[] AllNetPlayers
	{
		get
		{
			return this.m_allNetPlayers;
		}
	}

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06001847 RID: 6215 RVA: 0x00082032 File Offset: 0x00080232
	public override NetPlayer[] PlayerListOthers
	{
		get
		{
			return this.m_otherNetPlayers;
		}
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06001848 RID: 6216 RVA: 0x0008203A File Offset: 0x0008023A
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.punVoice;
		}
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06001849 RID: 6217 RVA: 0x00082044 File Offset: 0x00080244
	private int lowestPingRegionIndex
	{
		get
		{
			int num = 9999;
			int result = -1;
			for (int i = 0; i < this.regionData.Length; i++)
			{
				if (this.regionData[i].pingToRegion < num)
				{
					num = this.regionData[i].pingToRegion;
					result = i;
				}
			}
			return result;
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x0600184A RID: 6218 RVA: 0x0008208D File Offset: 0x0008028D
	// (set) Token: 0x0600184B RID: 6219 RVA: 0x00082095 File Offset: 0x00080295
	private NetworkSystemPUN.InternalState internalState
	{
		get
		{
			return this.currentState;
		}
		set
		{
			this.currentState = value;
		}
	}

	// Token: 0x17000293 RID: 659
	// (get) Token: 0x0600184C RID: 6220 RVA: 0x0008209E File Offset: 0x0008029E
	public override string CurrentPhotonBackend
	{
		get
		{
			return "PUN";
		}
	}

	// Token: 0x17000294 RID: 660
	// (get) Token: 0x0600184D RID: 6221 RVA: 0x000820A5 File Offset: 0x000802A5
	public override bool IsOnline
	{
		get
		{
			return this.InRoom;
		}
	}

	// Token: 0x17000295 RID: 661
	// (get) Token: 0x0600184E RID: 6222 RVA: 0x000820AD File Offset: 0x000802AD
	public override bool InRoom
	{
		get
		{
			return PhotonNetwork.InRoom;
		}
	}

	// Token: 0x17000296 RID: 662
	// (get) Token: 0x0600184F RID: 6223 RVA: 0x000820B4 File Offset: 0x000802B4
	public override string RoomName
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return ((currentRoom != null) ? currentRoom.Name : null) ?? string.Empty;
		}
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x000820D0 File Offset: 0x000802D0
	public override string RoomStringStripped()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (currentRoom.Name.Length < 20) ? currentRoom.Name : currentRoom.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			currentRoom.IsVisible ? "visible" : "hidden",
			currentRoom.IsOpen ? "open" : "closed",
			currentRoom.MaxPlayers,
			currentRoom.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary customProperties = currentRoom.CustomProperties;
		if (customProperties.Contains("gameMode"))
		{
			object obj = customProperties["gameMode"];
			if (obj == null)
			{
				NetworkSystem.reusableSB.AppendFormat("gameMode=null}", Array.Empty<object>());
			}
			else
			{
				string text = obj as string;
				if (text != null)
				{
					NetworkSystem.reusableSB.AppendFormat("gameMode={0}", (text.Length < 50) ? text : text.Remove(50));
				}
			}
		}
		NetworkSystem.reusableSB.Append("}");
		Debug.Log(NetworkSystem.reusableSB.ToString());
		return NetworkSystem.reusableSB.ToString();
	}

	// Token: 0x17000297 RID: 663
	// (get) Token: 0x06001851 RID: 6225 RVA: 0x00082258 File Offset: 0x00080458
	public override string GameModeString
	{
		get
		{
			object obj;
			PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", ref obj);
			if (obj != null)
			{
				return obj.ToString();
			}
			return null;
		}
	}

	// Token: 0x17000298 RID: 664
	// (get) Token: 0x06001852 RID: 6226 RVA: 0x00082287 File Offset: 0x00080487
	public override string CurrentRegion
	{
		get
		{
			return PhotonNetwork.CloudRegion;
		}
	}

	// Token: 0x17000299 RID: 665
	// (get) Token: 0x06001853 RID: 6227 RVA: 0x0008228E File Offset: 0x0008048E
	public override bool SessionIsPrivate
	{
		get
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			return currentRoom != null && !currentRoom.IsVisible;
		}
	}

	// Token: 0x1700029A RID: 666
	// (get) Token: 0x06001854 RID: 6228 RVA: 0x000822A3 File Offset: 0x000804A3
	public override int LocalPlayerID
	{
		get
		{
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06001855 RID: 6229 RVA: 0x000822AF File Offset: 0x000804AF
	public override int ServerTimestamp
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06001856 RID: 6230 RVA: 0x000822B6 File Offset: 0x000804B6
	public override double SimTime
	{
		get
		{
			return PhotonNetwork.Time;
		}
	}

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06001857 RID: 6231 RVA: 0x000822BD File Offset: 0x000804BD
	public override float SimDeltaTime
	{
		get
		{
			return Time.deltaTime;
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06001858 RID: 6232 RVA: 0x000822AF File Offset: 0x000804AF
	public override int SimTick
	{
		get
		{
			return PhotonNetwork.ServerTimestamp;
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001859 RID: 6233 RVA: 0x000822C4 File Offset: 0x000804C4
	public override int TickRate
	{
		get
		{
			return PhotonNetwork.SerializationRate;
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x0600185A RID: 6234 RVA: 0x000822CB File Offset: 0x000804CB
	public override int RoomPlayerCount
	{
		get
		{
			return (int)PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x0600185B RID: 6235 RVA: 0x000822D7 File Offset: 0x000804D7
	public override bool IsMasterClient
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x000822E0 File Offset: 0x000804E0
	public override void Initialise()
	{
		NetworkSystemPUN.<Initialise>d__53 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemPUN.<Initialise>d__53>(ref <Initialise>d__);
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x00082318 File Offset: 0x00080518
	private Task CacheRegionInfo()
	{
		NetworkSystemPUN.<CacheRegionInfo>d__54 <CacheRegionInfo>d__;
		<CacheRegionInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CacheRegionInfo>d__.<>4__this = this;
		<CacheRegionInfo>d__.<>1__state = -1;
		<CacheRegionInfo>d__.<>t__builder.Start<NetworkSystemPUN.<CacheRegionInfo>d__54>(ref <CacheRegionInfo>d__);
		return <CacheRegionInfo>d__.<>t__builder.Task;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x0008235B File Offset: 0x0008055B
	public override AuthenticationValues GetAuthenticationValues()
	{
		return PhotonNetwork.AuthValues;
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x00082362 File Offset: 0x00080562
	public override void SetAuthenticationValues(AuthenticationValues authValues)
	{
		PhotonNetwork.AuthValues = authValues;
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x0008236A File Offset: 0x0008056A
	public override void FinishAuthenticating()
	{
		this.internalState = NetworkSystemPUN.InternalState.Authenticated;
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x00082374 File Offset: 0x00080574
	private Task WaitForState(CancellationToken ct, params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForState>d__58 <WaitForState>d__;
		<WaitForState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForState>d__.<>4__this = this;
		<WaitForState>d__.ct = ct;
		<WaitForState>d__.desiredStates = desiredStates;
		<WaitForState>d__.<>1__state = -1;
		<WaitForState>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForState>d__58>(ref <WaitForState>d__);
		return <WaitForState>d__.<>t__builder.Task;
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000823C8 File Offset: 0x000805C8
	private Task<bool> WaitForStateCheck(params NetworkSystemPUN.InternalState[] desiredStates)
	{
		NetworkSystemPUN.<WaitForStateCheck>d__59 <WaitForStateCheck>d__;
		<WaitForStateCheck>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForStateCheck>d__.<>4__this = this;
		<WaitForStateCheck>d__.desiredStates = desiredStates;
		<WaitForStateCheck>d__.<>1__state = -1;
		<WaitForStateCheck>d__.<>t__builder.Start<NetworkSystemPUN.<WaitForStateCheck>d__59>(ref <WaitForStateCheck>d__);
		return <WaitForStateCheck>d__.<>t__builder.Task;
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x00082414 File Offset: 0x00080614
	private Task<NetJoinResult> MakeOrFindRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<MakeOrFindRoom>d__60 <MakeOrFindRoom>d__;
		<MakeOrFindRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrFindRoom>d__.<>4__this = this;
		<MakeOrFindRoom>d__.roomName = roomName;
		<MakeOrFindRoom>d__.opts = opts;
		<MakeOrFindRoom>d__.regionIndex = regionIndex;
		<MakeOrFindRoom>d__.<>1__state = -1;
		<MakeOrFindRoom>d__.<>t__builder.Start<NetworkSystemPUN.<MakeOrFindRoom>d__60>(ref <MakeOrFindRoom>d__);
		return <MakeOrFindRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x00082470 File Offset: 0x00080670
	private Task<bool> TryJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryJoinRoom>d__61 <TryJoinRoom>d__;
		<TryJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoom>d__.<>4__this = this;
		<TryJoinRoom>d__.roomName = roomName;
		<TryJoinRoom>d__.opts = opts;
		<TryJoinRoom>d__.<>1__state = -1;
		<TryJoinRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoom>d__61>(ref <TryJoinRoom>d__);
		return <TryJoinRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x000824C4 File Offset: 0x000806C4
	private Task<bool> TryJoinRoomInRegion(string roomName, RoomConfig opts, int regionIndex)
	{
		NetworkSystemPUN.<TryJoinRoomInRegion>d__62 <TryJoinRoomInRegion>d__;
		<TryJoinRoomInRegion>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryJoinRoomInRegion>d__.<>4__this = this;
		<TryJoinRoomInRegion>d__.roomName = roomName;
		<TryJoinRoomInRegion>d__.opts = opts;
		<TryJoinRoomInRegion>d__.regionIndex = regionIndex;
		<TryJoinRoomInRegion>d__.<>1__state = -1;
		<TryJoinRoomInRegion>d__.<>t__builder.Start<NetworkSystemPUN.<TryJoinRoomInRegion>d__62>(ref <TryJoinRoomInRegion>d__);
		return <TryJoinRoomInRegion>d__.<>t__builder.Task;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x00082520 File Offset: 0x00080720
	private Task<NetJoinResult> TryCreateRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemPUN.<TryCreateRoom>d__63 <TryCreateRoom>d__;
		<TryCreateRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<TryCreateRoom>d__.<>4__this = this;
		<TryCreateRoom>d__.roomName = roomName;
		<TryCreateRoom>d__.opts = opts;
		<TryCreateRoom>d__.<>1__state = -1;
		<TryCreateRoom>d__.<>t__builder.Start<NetworkSystemPUN.<TryCreateRoom>d__63>(ref <TryCreateRoom>d__);
		return <TryCreateRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x00082574 File Offset: 0x00080774
	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemPUN.<JoinRandomPublicRoom>d__64 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinRandomPublicRoom>d__64>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x000825C0 File Offset: 0x000807C0
	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemPUN.<ConnectToRoom>d__65 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.regionIndex = regionIndex;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemPUN.<ConnectToRoom>d__65>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x0008261C File Offset: 0x0008081C
	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemPUN.<JoinFriendsRoom>d__66 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemPUN.<JoinFriendsRoom>d__66>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x000029BC File Offset: 0x00000BBC
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x00082680 File Offset: 0x00080880
	public override string GetRandomWeightedRegion()
	{
		float value = Random.value;
		int num = 0;
		for (int i = 0; i < this.regionData.Length; i++)
		{
			num += this.regionData[i].playersInRegion;
		}
		float num2 = 0f;
		int num3 = -1;
		while (num2 < value && num3 < this.regionData.Length - 1)
		{
			num3++;
			num2 += (float)this.regionData[num3].playersInRegion / (float)num;
		}
		return this.regionNames[num3];
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x000826F8 File Offset: 0x000808F8
	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemPUN.<ReturnToSinglePlayer>d__69 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemPUN.<ReturnToSinglePlayer>d__69>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x0008273C File Offset: 0x0008093C
	private Task InternalDisconnect()
	{
		NetworkSystemPUN.<InternalDisconnect>d__70 <InternalDisconnect>d__;
		<InternalDisconnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InternalDisconnect>d__.<>4__this = this;
		<InternalDisconnect>d__.<>1__state = -1;
		<InternalDisconnect>d__.<>t__builder.Start<NetworkSystemPUN.<InternalDisconnect>d__70>(ref <InternalDisconnect>d__);
		return <InternalDisconnect>d__.<>t__builder.Task;
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x0008277F File Offset: 0x0008097F
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x00082788 File Offset: 0x00080988
	private void SetupVoice()
	{
		try
		{
			this.punVoice = PhotonVoiceNetwork.Instance;
			this.VoiceNetworkObject = this.punVoice.gameObject;
			this.VoiceNetworkObject.name = "VoiceNetworkObject";
			this.VoiceNetworkObject.transform.parent = base.transform;
			this.VoiceNetworkObject.transform.localPosition = Vector3.zero;
			this.punVoice.LogLevel = this.VoiceSettings.LogLevel;
			this.punVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
			this.punVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
			this.punVoice.AutoConnectAndJoin = this.VoiceSettings.AutoConnectAndJoin;
			this.punVoice.AutoLeaveAndDisconnect = this.VoiceSettings.AutoLeaveAndDisconnect;
			this.punVoice.WorkInOfflineMode = this.VoiceSettings.WorkInOfflineMode;
			this.punVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
			AppSettings appSettings = new AppSettings();
			appSettings.AppIdRealtime = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
			appSettings.AppIdVoice = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;
			this.punVoice.Settings = appSettings;
			this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
			{
				this.punVoice.RemoteVoiceAdded += callback;
			});
			this.localRecorder = this.VoiceNetworkObject.GetComponent<GTRecorder>();
			if (this.localRecorder == null)
			{
				this.localRecorder = this.VoiceNetworkObject.AddComponent<GTRecorder>();
				if (VRRigCache.Instance != null && VRRigCache.Instance.localRig != null)
				{
					LoudSpeakerActivator[] componentsInChildren = VRRigCache.Instance.localRig.GetComponentsInChildren<LoudSpeakerActivator>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].SetRecorder((GTRecorder)this.localRecorder);
					}
				}
			}
			this.localRecorder.LogLevel = this.VoiceSettings.LogLevel;
			this.localRecorder.RecordOnlyWhenEnabled = this.VoiceSettings.RecordOnlyWhenEnabled;
			this.localRecorder.RecordOnlyWhenJoined = this.VoiceSettings.RecordOnlyWhenJoined;
			this.localRecorder.StopRecordingWhenPaused = this.VoiceSettings.StopRecordingWhenPaused;
			this.localRecorder.TransmitEnabled = this.VoiceSettings.TransmitEnabled;
			this.localRecorder.AutoStart = this.VoiceSettings.AutoStart;
			this.localRecorder.Encrypt = this.VoiceSettings.Encrypt;
			this.localRecorder.FrameDuration = this.VoiceSettings.FrameDuration;
			this.localRecorder.SamplingRate = this.VoiceSettings.SamplingRate;
			this.localRecorder.InterestGroup = this.VoiceSettings.InterestGroup;
			this.localRecorder.SourceType = this.VoiceSettings.InputSourceType;
			this.localRecorder.MicrophoneType = this.VoiceSettings.MicrophoneType;
			this.localRecorder.UseMicrophoneTypeFallback = this.VoiceSettings.UseFallback;
			this.localRecorder.VoiceDetection = this.VoiceSettings.Detect;
			this.localRecorder.VoiceDetectionThreshold = this.VoiceSettings.Threshold;
			this.localRecorder.Bitrate = this.VoiceSettings.Bitrate;
			this.localRecorder.VoiceDetectionDelayMs = this.VoiceSettings.Delay;
			this.localRecorder.DebugEchoMode = this.VoiceSettings.DebugEcho;
			this.punVoice.PrimaryRecorder = this.localRecorder;
			this.VoiceNetworkObject.AddComponent<VoiceToLoudness>();
		}
		catch (Exception ex)
		{
			Debug.LogError("An exception was thrown when trying to setup photon voice, please check microphone permissions.:/n" + ex.ToString());
		}
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0007D437 File Offset: 0x0007B637
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x00082B38 File Offset: 0x00080D38
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, 0, null);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0, null);
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x00082B66 File Offset: 0x00080D66
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		return this.NetInstantiate(prefab, position, rotation, isRoomObject);
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00082B73 File Offset: 0x00080D73
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return null;
		}
		if (isRoomObject)
		{
			return PhotonNetwork.InstantiateRoomObject(prefab.name, position, rotation, group, data);
		}
		return PhotonNetwork.Instantiate(prefab.name, position, rotation, group, data);
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00082BA8 File Offset: 0x00080DA8
	public override void NetDestroy(GameObject instance)
	{
		PhotonView photonView;
		if (instance.TryGetComponent<PhotonView>(ref photonView) && photonView.AmOwner)
		{
			PhotonNetwork.Destroy(instance);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x00002789 File Offset: 0x00000989
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x00082BD4 File Offset: 0x00080DD4
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = sendToSelf ? 0 : 1;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x00082C10 File Offset: 0x00080E10
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = sendToSelf ? 0 : 1;
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[]
		{
			args.Data
		});
	}

	// Token: 0x06001878 RID: 6264 RVA: 0x00082C54 File Offset: 0x00080E54
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		RpcTarget rpcTarget = sendToSelf ? 0 : 1;
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, rpcTarget, new object[]
		{
			message
		});
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x00082C8C File Offset: 0x00080E8C
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			NetworkSystem.EmptyArgs
		});
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x00082CCC File Offset: 0x00080ECC
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		ref args.SerializeToRPCData<T>();
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			args.Data
		});
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x00082D14 File Offset: 0x00080F14
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(targetPlayerID, false);
		PhotonView.Get(component).RPC(rpcMethod.Method.Name, player, new object[]
		{
			message
		});
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x00082D50 File Offset: 0x00080F50
	public override Task AwaitSceneReady()
	{
		NetworkSystemPUN.<AwaitSceneReady>d__85 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemPUN.<AwaitSceneReady>d__85>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00082D8C File Offset: 0x00080F8C
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0)
		{
			base.UpdatePlayers();
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.IsLocal)
			{
				return netPlayer;
			}
		}
		Debug.LogError("Somehow no local net players found. This shouldn't happen");
		return null;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x00082E04 File Offset: 0x00081004
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (this.InRoom && !PhotonNetwork.CurrentRoom.Players.ContainsKey(PlayerID))
		{
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		base.UpdatePlayers();
		foreach (NetPlayer netPlayer2 in this.netPlayerCache)
		{
			if (netPlayer2.ActorNumber == PlayerID)
			{
				return netPlayer2;
			}
		}
		GTDev.LogWarning<string>("There is no NetPlayer with this ID currently in game. Passed ID: " + PlayerID.ToString(), null);
		return null;
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x00082EE4 File Offset: 0x000810E4
	public override void SetMyNickName(string id)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !id.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			PhotonNetwork.LocalPlayer.NickName = "gorilla";
			return;
		}
		PlayerPrefs.SetString("playerName", id);
		PhotonNetwork.LocalPlayer.NickName = id;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x00082F36 File Offset: 0x00081136
	public override string GetMyNickName()
	{
		return PhotonNetwork.LocalPlayer.NickName;
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x00082F42 File Offset: 0x00081142
	public override string GetMyDefaultName()
	{
		return PhotonNetwork.LocalPlayer.DefaultName;
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x00082F50 File Offset: 0x00081150
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.NickName;
		}
		return null;
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x00082F70 File Offset: 0x00081170
	public override string GetNickName(NetPlayer player)
	{
		return player.NickName;
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x00082F78 File Offset: 0x00081178
	public override void SetMyTutorialComplete()
	{
		bool flag = PlayerPrefs.GetString("didTutorial", "nope") == "done";
		if (!flag)
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
		Hashtable hashtable = new Hashtable();
		hashtable.Add("didTutorial", flag);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0007E31E File Offset: 0x0007C51E
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x00082FDC File Offset: 0x000811DC
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			return false;
		}
		Player player2 = PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber, false);
		if (player2 == null)
		{
			return false;
		}
		object obj;
		if (player2.CustomProperties.TryGetValue("didTutorial", ref obj))
		{
			bool flag;
			bool flag2;
			if (obj is bool)
			{
				flag = (bool)obj;
				flag2 = (1 == 0);
			}
			else
			{
				flag2 = true;
			}
			return flag2 || flag;
		}
		return false;
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0008303B File Offset: 0x0008123B
	public override string GetMyUserID()
	{
		return PhotonNetwork.LocalPlayer.UserId;
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x00083048 File Offset: 0x00081248
	public override string GetUserID(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player != null)
		{
			return player.UserId;
		}
		return null;
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x00083068 File Offset: 0x00081268
	public override string GetUserID(NetPlayer netPlayer)
	{
		Player playerRef = ((PunNetPlayer)netPlayer).PlayerRef;
		if (playerRef != null)
		{
			return playerRef.UserId;
		}
		return null;
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0008308C File Offset: 0x0008128C
	public override int GlobalPlayerCount()
	{
		int num = 0;
		foreach (NetworkRegionInfo networkRegionInfo in this.regionData)
		{
			num += networkRegionInfo.playersInRegion;
		}
		return num;
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x000830C0 File Offset: 0x000812C0
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		PhotonView photonView;
		return !this.IsOnline || !obj.TryGetComponent<PhotonView>(ref photonView) || photonView.IsMine;
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x000830EC File Offset: 0x000812EC
	protected override void UpdateNetPlayerList()
	{
		if (!this.IsOnline)
		{
			bool flag = false;
			PunNetPlayer punNetPlayer = null;
			if (this.netPlayerCache.Count > 0)
			{
				for (int i = 0; i < this.netPlayerCache.Count; i++)
				{
					NetPlayer netPlayer = this.netPlayerCache[i];
					if (netPlayer.IsLocal)
					{
						punNetPlayer = (PunNetPlayer)netPlayer;
						flag = true;
					}
					else
					{
						this.playerPool.Return((PunNetPlayer)netPlayer);
					}
				}
				this.netPlayerCache.Clear();
			}
			if (!flag)
			{
				punNetPlayer = this.playerPool.Take();
				punNetPlayer.InitPlayer(PhotonNetwork.LocalPlayer);
			}
			this.netPlayerCache.Add(punNetPlayer);
		}
		else
		{
			Dictionary<int, Player>.ValueCollection values = PhotonNetwork.CurrentRoom.Players.Values;
			foreach (Player player in values)
			{
				bool flag2 = false;
				for (int j = 0; j < this.netPlayerCache.Count; j++)
				{
					if (player == ((PunNetPlayer)this.netPlayerCache[j]).PlayerRef)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					PunNetPlayer punNetPlayer2 = this.playerPool.Take();
					punNetPlayer2.InitPlayer(player);
					this.netPlayerCache.Add(punNetPlayer2);
				}
			}
			for (int k = 0; k < this.netPlayerCache.Count; k++)
			{
				PunNetPlayer punNetPlayer3 = (PunNetPlayer)this.netPlayerCache[k];
				bool flag3 = false;
				using (Dictionary<int, Player>.ValueCollection.Enumerator enumerator = values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == punNetPlayer3.PlayerRef)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this.playerPool.Return(punNetPlayer3);
					this.netPlayerCache.Remove(punNetPlayer3);
				}
			}
		}
		this.m_allNetPlayers = this.netPlayerCache.ToArray();
		this.m_otherNetPlayers = new NetPlayer[this.m_allNetPlayers.Length - 1];
		int num = 0;
		for (int l = 0; l < this.m_allNetPlayers.Length; l++)
		{
			NetPlayer netPlayer2 = this.m_allNetPlayers[l];
			if (netPlayer2.IsLocal)
			{
				num++;
			}
			else
			{
				int num2 = l - num;
				if (num2 == this.m_otherNetPlayers.Length)
				{
					break;
				}
				this.m_otherNetPlayers[num2] = netPlayer2;
			}
		}
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x00083358 File Offset: 0x00081558
	public override bool IsObjectRoomObject(GameObject obj)
	{
		PhotonView component = obj.GetComponent<PhotonView>();
		if (component == null)
		{
			Debug.LogError("No photonview found on this Object, this shouldn't happen");
			return false;
		}
		return component.IsRoomView;
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00083387 File Offset: 0x00081587
	public override bool ShouldUpdateObject(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x0600188F RID: 6287 RVA: 0x00083387 File Offset: 0x00081587
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		return this.IsObjectLocallyOwned(obj);
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x00083390 File Offset: 0x00081590
	public override int GetOwningPlayerID(GameObject obj)
	{
		PhotonView photonView;
		if (obj.TryGetComponent<PhotonView>(ref photonView) && photonView.Owner != null)
		{
			return photonView.Owner.ActorNumber;
		}
		return -1;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x000833BC File Offset: 0x000815BC
	public override bool ShouldSpawnLocally(int playerID)
	{
		return this.LocalPlayerID == playerID || (playerID == -1 && PhotonNetwork.MasterClient.IsLocal);
	}

	// Token: 0x06001892 RID: 6290 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsTotalAuthority()
	{
		return false;
	}

	// Token: 0x06001893 RID: 6291 RVA: 0x000833D9 File Offset: 0x000815D9
	public void OnConnectedtoMaster()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.ConnectingToMaster)
		{
			this.internalState = NetworkSystemPUN.InternalState.ConnectedToMaster;
		}
		base.UpdatePlayers();
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x000833F1 File Offset: 0x000815F1
	public void OnJoinedRoom()
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Joined;
		}
		else if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_Created;
		}
		this.AddVoice();
		base.UpdatePlayers();
		base.JoinedNetworkRoom();
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x0008342B File Offset: 0x0008162B
	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.Log("onJoinRoomFailed " + returnCode.ToString() + message);
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Joining)
		{
			if (returnCode == 32765)
			{
				this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed_Full;
				return;
			}
			this.internalState = NetworkSystemPUN.InternalState.Searching_JoinFailed;
		}
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x00083467 File Offset: 0x00081667
	public void OnCreateRoomFailed(short returnCode, string message)
	{
		if (this.internalState == NetworkSystemPUN.InternalState.Searching_Creating)
		{
			this.internalState = NetworkSystemPUN.InternalState.Searching_CreateFailed;
		}
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x0008347C File Offset: 0x0008167C
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.UpdatePlayers();
		NetPlayer player = base.GetPlayer(newPlayer);
		base.PlayerJoined(player);
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x000834A0 File Offset: 0x000816A0
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		NetPlayer player = base.GetPlayer(otherPlayer);
		base.UpdatePlayers();
		base.PlayerLeft(player);
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x000834C4 File Offset: 0x000816C4
	public void OnDisconnected(DisconnectCause cause)
	{
		NetworkSystemPUN.<OnDisconnected>d__114 <OnDisconnected>d__;
		<OnDisconnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnDisconnected>d__.<>4__this = this;
		<OnDisconnected>d__.<>1__state = -1;
		<OnDisconnected>d__.<>t__builder.Start<NetworkSystemPUN.<OnDisconnected>d__114>(ref <OnDisconnected>d__);
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x000834FB File Offset: 0x000816FB
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitchedCallback(newMasterClient);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x0008350C File Offset: 0x0008170C
	private ValueTuple<CancellationTokenSource, CancellationToken> GetCancellationToken()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationToken token = cancellationTokenSource.Token;
		this._taskCancelTokens.Add(cancellationTokenSource);
		return new ValueTuple<CancellationTokenSource, CancellationToken>(cancellationTokenSource, token);
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0008353C File Offset: 0x0008173C
	public void ResetSystem()
	{
		if (this.VoiceNetworkObject)
		{
			Object.Destroy(this.VoiceNetworkObject);
		}
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = this.regionNames[this.lowestPingRegionIndex];
		this.currentRegionIndex = this.lowestPingRegionIndex;
		PhotonNetwork.Disconnect();
		this._taskCancelTokens.ForEach(delegate(CancellationTokenSource token)
		{
			token.Cancel();
			token.Dispose();
		});
		this._taskCancelTokens.Clear();
		this.internalState = NetworkSystemPUN.InternalState.Idle;
		base.netState = NetSystemState.Idle;
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x000835D4 File Offset: 0x000817D4
	private void UpdateZoneInfo(bool roomIsPublic, string zoneName = null)
	{
		AuthenticationValues authenticationValues = this.GetAuthenticationValues();
		Dictionary<string, object> dictionary = ((authenticationValues != null) ? authenticationValues.AuthPostData : null) as Dictionary<string, object>;
		if (dictionary != null)
		{
			dictionary["Zone"] = ((zoneName != null) ? zoneName : ((ZoneManagement.instance.activeZones.Count > 0) ? Enumerable.First<GTZone>(ZoneManagement.instance.activeZones).GetName<GTZone>() : ""));
			dictionary["SubZone"] = GTSubZone.none.GetName<GTSubZone>();
			dictionary["IsPublic"] = roomIsPublic;
			authenticationValues.SetAuthPostData(dictionary);
			this.SetAuthenticationValues(authenticationValues);
		}
	}

	// Token: 0x040021B1 RID: 8625
	private NetworkRegionInfo[] regionData;

	// Token: 0x040021B2 RID: 8626
	private Task<NetJoinResult> roomTask;

	// Token: 0x040021B3 RID: 8627
	private ObjectPool<PunNetPlayer> playerPool;

	// Token: 0x040021B4 RID: 8628
	private NetPlayer[] m_allNetPlayers = new NetPlayer[0];

	// Token: 0x040021B5 RID: 8629
	private NetPlayer[] m_otherNetPlayers = new NetPlayer[0];

	// Token: 0x040021B6 RID: 8630
	private List<CancellationTokenSource> _taskCancelTokens = new List<CancellationTokenSource>();

	// Token: 0x040021B7 RID: 8631
	private PhotonVoiceNetwork punVoice;

	// Token: 0x040021B8 RID: 8632
	private GameObject VoiceNetworkObject;

	// Token: 0x040021B9 RID: 8633
	private NetworkSystemPUN.InternalState currentState;

	// Token: 0x040021BA RID: 8634
	private bool firstRoomJoin;

	// Token: 0x020003E1 RID: 993
	private enum InternalState
	{
		// Token: 0x040021BC RID: 8636
		AwaitingAuth,
		// Token: 0x040021BD RID: 8637
		Authenticated,
		// Token: 0x040021BE RID: 8638
		PingGathering,
		// Token: 0x040021BF RID: 8639
		StateCheckFailed,
		// Token: 0x040021C0 RID: 8640
		ConnectingToMaster,
		// Token: 0x040021C1 RID: 8641
		ConnectedToMaster,
		// Token: 0x040021C2 RID: 8642
		Idle,
		// Token: 0x040021C3 RID: 8643
		Internal_Disconnecting,
		// Token: 0x040021C4 RID: 8644
		Internal_Disconnected,
		// Token: 0x040021C5 RID: 8645
		Searching_Connecting,
		// Token: 0x040021C6 RID: 8646
		Searching_Connected,
		// Token: 0x040021C7 RID: 8647
		Searching_Joining,
		// Token: 0x040021C8 RID: 8648
		Searching_Joined,
		// Token: 0x040021C9 RID: 8649
		Searching_JoinFailed,
		// Token: 0x040021CA RID: 8650
		Searching_JoinFailed_Full,
		// Token: 0x040021CB RID: 8651
		Searching_Creating,
		// Token: 0x040021CC RID: 8652
		Searching_Created,
		// Token: 0x040021CD RID: 8653
		Searching_CreateFailed,
		// Token: 0x040021CE RID: 8654
		Searching_Disconnecting,
		// Token: 0x040021CF RID: 8655
		Searching_Disconnected
	}
}
