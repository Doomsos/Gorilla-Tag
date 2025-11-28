using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using GorillaGameModes;
using GorillaTag;
using GorillaTag.Audio;
using Photon.Realtime;
using Photon.Voice.Unity;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003AD RID: 941
public class NetworkSystemFusion : NetworkSystem
{
	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001697 RID: 5783 RVA: 0x0007CA56 File Offset: 0x0007AC56
	// (set) Token: 0x06001698 RID: 5784 RVA: 0x0007CA5E File Offset: 0x0007AC5E
	public NetworkRunner runner { get; private set; }

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06001699 RID: 5785 RVA: 0x0007CA67 File Offset: 0x0007AC67
	public override bool IsOnline
	{
		get
		{
			return this.runner != null && !this.runner.IsSinglePlayer;
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x0600169A RID: 5786 RVA: 0x0007CA87 File Offset: 0x0007AC87
	public override bool InRoom
	{
		get
		{
			return this.runner != null && this.runner.State != 3 && !this.runner.IsSinglePlayer && this.runner.IsConnectedToServer;
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x0600169B RID: 5787 RVA: 0x0007CABF File Offset: 0x0007ACBF
	public override string RoomName
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Name;
		}
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x0007CAD8 File Offset: 0x0007ACD8
	public override string RoomStringStripped()
	{
		SessionInfo sessionInfo = this.runner.SessionInfo;
		NetworkSystem.reusableSB.Clear();
		NetworkSystem.reusableSB.AppendFormat("Room: '{0}' ", (sessionInfo.Name.Length < 20) ? sessionInfo.Name : sessionInfo.Name.Remove(20));
		NetworkSystem.reusableSB.AppendFormat("{0},{1} {3}/{2} players.", new object[]
		{
			sessionInfo.IsVisible ? "visible" : "hidden",
			sessionInfo.IsOpen ? "open" : "closed",
			sessionInfo.MaxPlayers,
			sessionInfo.PlayerCount
		});
		NetworkSystem.reusableSB.Append("\ncustomProps: {");
		NetworkSystem.reusableSB.AppendFormat("joinedGameMode={0}, ", (RoomSystem.RoomGameMode.Length < 50) ? RoomSystem.RoomGameMode : RoomSystem.RoomGameMode.Remove(50));
		IDictionary properties = sessionInfo.Properties;
		Debug.Log(RoomSystem.RoomGameMode.ToString());
		if (properties.Contains("gameMode"))
		{
			object obj = properties["gameMode"];
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

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x0600169D RID: 5789 RVA: 0x0007CC74 File Offset: 0x0007AE74
	public override string GameModeString
	{
		get
		{
			SessionProperty sessionProperty;
			this.runner.SessionInfo.Properties.TryGetValue("gameMode", ref sessionProperty);
			if (sessionProperty != null)
			{
				return (string)sessionProperty.PropertyValue;
			}
			return null;
		}
	}

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x0600169E RID: 5790 RVA: 0x0007CCAE File Offset: 0x0007AEAE
	public override string CurrentRegion
	{
		get
		{
			SessionInfo sessionInfo = this.runner.SessionInfo;
			if (sessionInfo == null)
			{
				return null;
			}
			return sessionInfo.Region;
		}
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600169F RID: 5791 RVA: 0x0007CCC8 File Offset: 0x0007AEC8
	public override bool SessionIsPrivate
	{
		get
		{
			NetworkRunner runner = this.runner;
			bool? flag;
			if (runner == null)
			{
				flag = default(bool?);
			}
			else
			{
				SessionInfo sessionInfo = runner.SessionInfo;
				flag = ((sessionInfo != null) ? new bool?(!sessionInfo.IsVisible) : default(bool?));
			}
			bool? flag2 = flag;
			return flag2.GetValueOrDefault();
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x060016A0 RID: 5792 RVA: 0x0007CD14 File Offset: 0x0007AF14
	public override int LocalPlayerID
	{
		get
		{
			return this.runner.LocalPlayer.PlayerId;
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x060016A1 RID: 5793 RVA: 0x0007CD34 File Offset: 0x0007AF34
	public override string CurrentPhotonBackend
	{
		get
		{
			return "Fusion";
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x060016A2 RID: 5794 RVA: 0x0007CD3B File Offset: 0x0007AF3B
	public override double SimTime
	{
		get
		{
			return (double)this.runner.SimulationTime;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x060016A3 RID: 5795 RVA: 0x0007CD49 File Offset: 0x0007AF49
	public override float SimDeltaTime
	{
		get
		{
			return this.runner.DeltaTime;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x060016A4 RID: 5796 RVA: 0x0007CD56 File Offset: 0x0007AF56
	public override int SimTick
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x060016A5 RID: 5797 RVA: 0x0007CD68 File Offset: 0x0007AF68
	public override int TickRate
	{
		get
		{
			return this.runner.TickRate;
		}
	}

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x060016A6 RID: 5798 RVA: 0x0007CD56 File Offset: 0x0007AF56
	public override int ServerTimestamp
	{
		get
		{
			return this.runner.Tick.Raw;
		}
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x060016A7 RID: 5799 RVA: 0x0007CD75 File Offset: 0x0007AF75
	public override int RoomPlayerCount
	{
		get
		{
			return this.runner.SessionInfo.PlayerCount;
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x060016A8 RID: 5800 RVA: 0x0007CD87 File Offset: 0x0007AF87
	public override VoiceConnection VoiceConnection
	{
		get
		{
			return this.FusionVoice;
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x060016A9 RID: 5801 RVA: 0x0007CD8F File Offset: 0x0007AF8F
	public override bool IsMasterClient
	{
		get
		{
			NetworkRunner runner = this.runner;
			return runner == null || runner.IsSharedModeMasterClient;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x060016AA RID: 5802 RVA: 0x0007CDA4 File Offset: 0x0007AFA4
	public override NetPlayer MasterClient
	{
		get
		{
			if (this.runner != null && this.runner.IsSharedModeMasterClient)
			{
				return base.GetPlayer(this.runner.LocalPlayer);
			}
			if (!(GameMode.ActiveNetworkHandler != null))
			{
				return null;
			}
			return base.GetPlayer(GameMode.ActiveNetworkHandler.Object.StateAuthority);
		}
	}

	// Token: 0x060016AB RID: 5803 RVA: 0x0007CE04 File Offset: 0x0007B004
	public override void Initialise()
	{
		NetworkSystemFusion.<Initialise>d__53 <Initialise>d__;
		<Initialise>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialise>d__.<>4__this = this;
		<Initialise>d__.<>1__state = -1;
		<Initialise>d__.<>t__builder.Start<NetworkSystemFusion.<Initialise>d__53>(ref <Initialise>d__);
	}

	// Token: 0x060016AC RID: 5804 RVA: 0x0007CE3C File Offset: 0x0007B03C
	private void CreateRegionCrawler()
	{
		GameObject gameObject = new GameObject("[Network Crawler]");
		gameObject.transform.SetParent(base.transform);
		this.regionCrawler = gameObject.AddComponent<FusionRegionCrawler>();
	}

	// Token: 0x060016AD RID: 5805 RVA: 0x0007CE74 File Offset: 0x0007B074
	private Task AwaitAuth()
	{
		NetworkSystemFusion.<AwaitAuth>d__55 <AwaitAuth>d__;
		<AwaitAuth>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitAuth>d__.<>4__this = this;
		<AwaitAuth>d__.<>1__state = -1;
		<AwaitAuth>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitAuth>d__55>(ref <AwaitAuth>d__);
		return <AwaitAuth>d__.<>t__builder.Task;
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x0007CEB7 File Offset: 0x0007B0B7
	public override void FinishAuthenticating()
	{
		if (this.cachedPlayfabAuth != null)
		{
			Debug.Log("AUTHED");
			return;
		}
		Debug.LogError("Authentication Failed");
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x0007CED8 File Offset: 0x0007B0D8
	public override Task<NetJoinResult> ConnectToRoom(string roomName, RoomConfig opts, int regionIndex = -1)
	{
		NetworkSystemFusion.<ConnectToRoom>d__57 <ConnectToRoom>d__;
		<ConnectToRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<ConnectToRoom>d__.<>4__this = this;
		<ConnectToRoom>d__.roomName = roomName;
		<ConnectToRoom>d__.opts = opts;
		<ConnectToRoom>d__.<>1__state = -1;
		<ConnectToRoom>d__.<>t__builder.Start<NetworkSystemFusion.<ConnectToRoom>d__57>(ref <ConnectToRoom>d__);
		return <ConnectToRoom>d__.<>t__builder.Task;
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x0007CF2C File Offset: 0x0007B12C
	private Task<bool> Connect(GameMode mode, string targetSessionName, RoomConfig opts)
	{
		NetworkSystemFusion.<Connect>d__58 <Connect>d__;
		<Connect>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Connect>d__.<>4__this = this;
		<Connect>d__.mode = mode;
		<Connect>d__.targetSessionName = targetSessionName;
		<Connect>d__.opts = opts;
		<Connect>d__.<>1__state = -1;
		<Connect>d__.<>t__builder.Start<NetworkSystemFusion.<Connect>d__58>(ref <Connect>d__);
		return <Connect>d__.<>t__builder.Task;
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x0007CF88 File Offset: 0x0007B188
	private Task<NetJoinResult> MakeOrJoinRoom(string roomName, RoomConfig opts)
	{
		NetworkSystemFusion.<MakeOrJoinRoom>d__59 <MakeOrJoinRoom>d__;
		<MakeOrJoinRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<MakeOrJoinRoom>d__.<>4__this = this;
		<MakeOrJoinRoom>d__.roomName = roomName;
		<MakeOrJoinRoom>d__.opts = opts;
		<MakeOrJoinRoom>d__.<>1__state = -1;
		<MakeOrJoinRoom>d__.<>t__builder.Start<NetworkSystemFusion.<MakeOrJoinRoom>d__59>(ref <MakeOrJoinRoom>d__);
		return <MakeOrJoinRoom>d__.<>t__builder.Task;
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x0007CFDC File Offset: 0x0007B1DC
	private Task<NetJoinResult> JoinRandomPublicRoom(RoomConfig opts)
	{
		NetworkSystemFusion.<JoinRandomPublicRoom>d__60 <JoinRandomPublicRoom>d__;
		<JoinRandomPublicRoom>d__.<>t__builder = AsyncTaskMethodBuilder<NetJoinResult>.Create();
		<JoinRandomPublicRoom>d__.<>4__this = this;
		<JoinRandomPublicRoom>d__.opts = opts;
		<JoinRandomPublicRoom>d__.<>1__state = -1;
		<JoinRandomPublicRoom>d__.<>t__builder.Start<NetworkSystemFusion.<JoinRandomPublicRoom>d__60>(ref <JoinRandomPublicRoom>d__);
		return <JoinRandomPublicRoom>d__.<>t__builder.Task;
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0007D028 File Offset: 0x0007B228
	public override Task JoinFriendsRoom(string userID, int actorIDToFollow, string keyToFollow, string shufflerToFollow)
	{
		NetworkSystemFusion.<JoinFriendsRoom>d__61 <JoinFriendsRoom>d__;
		<JoinFriendsRoom>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<JoinFriendsRoom>d__.<>4__this = this;
		<JoinFriendsRoom>d__.userID = userID;
		<JoinFriendsRoom>d__.actorIDToFollow = actorIDToFollow;
		<JoinFriendsRoom>d__.keyToFollow = keyToFollow;
		<JoinFriendsRoom>d__.shufflerToFollow = shufflerToFollow;
		<JoinFriendsRoom>d__.<>1__state = -1;
		<JoinFriendsRoom>d__.<>t__builder.Start<NetworkSystemFusion.<JoinFriendsRoom>d__61>(ref <JoinFriendsRoom>d__);
		return <JoinFriendsRoom>d__.<>t__builder.Task;
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000029BC File Offset: 0x00000BBC
	public override void JoinPubWithFriends()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x0007D08C File Offset: 0x0007B28C
	public override Task ReturnToSinglePlayer()
	{
		NetworkSystemFusion.<ReturnToSinglePlayer>d__63 <ReturnToSinglePlayer>d__;
		<ReturnToSinglePlayer>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ReturnToSinglePlayer>d__.<>4__this = this;
		<ReturnToSinglePlayer>d__.<>1__state = -1;
		<ReturnToSinglePlayer>d__.<>t__builder.Start<NetworkSystemFusion.<ReturnToSinglePlayer>d__63>(ref <ReturnToSinglePlayer>d__);
		return <ReturnToSinglePlayer>d__.<>t__builder.Task;
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x0007D0D0 File Offset: 0x0007B2D0
	private Task CloseRunner(ShutdownReason reason = 0)
	{
		NetworkSystemFusion.<CloseRunner>d__64 <CloseRunner>d__;
		<CloseRunner>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CloseRunner>d__.<>4__this = this;
		<CloseRunner>d__.reason = reason;
		<CloseRunner>d__.<>1__state = -1;
		<CloseRunner>d__.<>t__builder.Start<NetworkSystemFusion.<CloseRunner>d__64>(ref <CloseRunner>d__);
		return <CloseRunner>d__.<>t__builder.Task;
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x0007D11C File Offset: 0x0007B31C
	public void MigrateHost(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		NetworkSystemFusion.<MigrateHost>d__65 <MigrateHost>d__;
		<MigrateHost>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<MigrateHost>d__.<>4__this = this;
		<MigrateHost>d__.<>1__state = -1;
		<MigrateHost>d__.<>t__builder.Start<NetworkSystemFusion.<MigrateHost>d__65>(ref <MigrateHost>d__);
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x0007D154 File Offset: 0x0007B354
	public void ResetSystem()
	{
		NetworkSystemFusion.<ResetSystem>d__66 <ResetSystem>d__;
		<ResetSystem>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ResetSystem>d__.<>4__this = this;
		<ResetSystem>d__.<>1__state = -1;
		<ResetSystem>d__.<>t__builder.Start<NetworkSystemFusion.<ResetSystem>d__66>(ref <ResetSystem>d__);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x0007D18B File Offset: 0x0007B38B
	private void AddVoice()
	{
		this.SetupVoice();
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x0007D194 File Offset: 0x0007B394
	private void SetupVoice()
	{
		Utils.Log("<color=orange>Adding Voice Stuff</color>");
		this.FusionVoice = this.volatileNetObj.AddComponent<VoiceConnection>();
		this.FusionVoice.LogLevel = this.VoiceSettings.LogLevel;
		this.FusionVoice.GlobalRecordersLogLevel = this.VoiceSettings.GlobalRecordersLogLevel;
		this.FusionVoice.GlobalSpeakersLogLevel = this.VoiceSettings.GlobalSpeakersLogLevel;
		this.FusionVoice.AutoCreateSpeakerIfNotFound = this.VoiceSettings.CreateSpeakerIfNotFound;
		AppSettings appSettings = new AppSettings();
		appSettings.AppIdFusion = PhotonAppSettings.Global.AppSettings.AppIdFusion;
		appSettings.AppIdVoice = PhotonAppSettings.Global.AppSettings.AppIdVoice;
		this.FusionVoice.Settings = appSettings;
		this.remoteVoiceAddedCallbacks.ForEach(delegate(Action<RemoteVoiceLink> callback)
		{
			this.FusionVoice.RemoteVoiceAdded += callback;
		});
		this.localRecorder = this.volatileNetObj.AddComponent<Recorder>();
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
		this.localRecorder.UserData = this.runner.UserId;
		this.FusionVoice.PrimaryRecorder = this.localRecorder;
		this.volatileNetObj.AddComponent<VoiceToLoudness>();
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x0007D437 File Offset: 0x0007B637
	public override void AddRemoteVoiceAddedCallback(Action<RemoteVoiceLink> callback)
	{
		this.remoteVoiceAddedCallbacks.Add(callback);
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0007D445 File Offset: 0x0007B645
	private void AttachCallbackTargets()
	{
		this.runner.AddCallbacks(this.objectsThatNeedCallbacks.ToArray());
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x0007D45D File Offset: 0x0007B65D
	public void RegisterForNetworkCallbacks(INetworkRunnerCallbacks callbacks)
	{
		if (!this.objectsThatNeedCallbacks.Contains(callbacks))
		{
			this.objectsThatNeedCallbacks.Add(callbacks);
		}
		if (this.runner != null)
		{
			this.runner.AddCallbacks(new INetworkRunnerCallbacks[]
			{
				callbacks
			});
		}
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x0007D49C File Offset: 0x0007B69C
	private void AttachSceneObjects(bool onlyCached = false)
	{
		NetworkSystemFusion.<AttachSceneObjects>d__74 <AttachSceneObjects>d__;
		<AttachSceneObjects>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AttachSceneObjects>d__.<>4__this = this;
		<AttachSceneObjects>d__.onlyCached = onlyCached;
		<AttachSceneObjects>d__.<>1__state = -1;
		<AttachSceneObjects>d__.<>t__builder.Start<NetworkSystemFusion.<AttachSceneObjects>d__74>(ref <AttachSceneObjects>d__);
	}

	// Token: 0x060016BF RID: 5823 RVA: 0x0007D4DC File Offset: 0x0007B6DC
	public override void AttachObjectInGame(GameObject item)
	{
		base.AttachObjectInGame(item);
		NetworkObject component = item.GetComponent<NetworkObject>();
		if ((component != null && !this.cachedNetSceneObjects.Contains(component)) || !component.IsValid)
		{
			this.cachedNetSceneObjects.AddIfNew(component);
			this.registrationQueue.Enqueue(component);
			this.ProcessRegistrationQueue();
		}
	}

	// Token: 0x060016C0 RID: 5824 RVA: 0x0007D534 File Offset: 0x0007B734
	private void ProcessRegistrationQueue()
	{
		if (this.isProcessingQueue)
		{
			Debug.LogError("Queue is still processing");
			return;
		}
		this.isProcessingQueue = true;
		List<NetworkObject> list = new List<NetworkObject>();
		SceneRef sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
		while (this.registrationQueue.Count > 0)
		{
			NetworkObject networkObject = this.registrationQueue.Dequeue();
			if (this.InRoom && !networkObject.IsValid && !networkObject.Id.IsValid && networkObject.Runner == null)
			{
				try
				{
					list.Add(networkObject);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					this.isProcessingQueue = false;
					this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
					this.ProcessRegistrationQueue();
					break;
				}
			}
		}
		this.runner.RegisterSceneObjects(sceneRef, list.ToArray(), default(NetworkSceneLoadId));
		this.isProcessingQueue = false;
	}

	// Token: 0x060016C1 RID: 5825 RVA: 0x0007D630 File Offset: 0x0007B830
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject = false)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		try
		{
			return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), null, 0).gameObject;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
		return null;
	}

	// Token: 0x060016C2 RID: 5826 RVA: 0x0007D6A0 File Offset: 0x0007B8A0
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, int playerAuthID, bool isRoomObject = false)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (playerRef.PlayerId == playerAuthID)
			{
				Utils.Log("Net instantiate Fusion: " + prefab.name);
				return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(playerRef), null, 0).gameObject;
			}
		}
		Debug.LogError(string.Format("Couldn't find player with ID: {0}, cancelling requested spawn...", playerAuthID));
		return null;
	}

	// Token: 0x060016C3 RID: 5827 RVA: 0x0007D74C File Offset: 0x0007B94C
	public override GameObject NetInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, bool isRoomObject, byte group = 0, object[] data = null, NetworkRunner.OnBeforeSpawned callback = null)
	{
		Utils.Log("Net instantiate Fusion: " + prefab.name);
		return this.runner.Spawn(prefab, new Vector3?(position), new Quaternion?(rotation), new PlayerRef?(this.runner.LocalPlayer), callback, 0).gameObject;
	}

	// Token: 0x060016C4 RID: 5828 RVA: 0x0007D7A0 File Offset: 0x0007B9A0
	public override void NetDestroy(GameObject instance)
	{
		NetworkObject networkObject;
		if (instance.TryGetComponent<NetworkObject>(ref networkObject))
		{
			this.runner.Despawn(networkObject);
			return;
		}
		Object.Destroy(instance);
	}

	// Token: 0x060016C5 RID: 5829 RVA: 0x0007D7CC File Offset: 0x0007B9CC
	public override bool ShouldSpawnLocally(int playerID)
	{
		if (this.runner.GameMode == 2)
		{
			return this.runner.LocalPlayer.PlayerId == playerID || (playerID == -1 && this.runner.IsSharedModeMasterClient);
		}
		return this.runner.GameMode != 5;
	}

	// Token: 0x060016C6 RID: 5830 RVA: 0x0007D824 File Offset: 0x0007BA24
	public override void CallRPC(MonoBehaviour component, NetworkSystem.RPC rpcMethod, bool sendToSelf = true)
	{
		Utils.Log(WsaReflectionExtensions.GetDelegateName(rpcMethod) + "RPC called!");
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x060016C7 RID: 5831 RVA: 0x0007D89C File Offset: 0x0007BA9C
	public override void CallRPC<T>(MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args, bool sendToSelf = true)
	{
		Utils.Log(WsaReflectionExtensions.GetDelegateName(rpcMethod) + "RPC called!");
		ref args.SerializeToRPCData<T>();
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x060016C8 RID: 5832 RVA: 0x0007D91C File Offset: 0x0007BB1C
	public override void CallRPC(MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message, bool sendToSelf = true)
	{
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			if (!sendToSelf)
			{
				playerRef != this.runner.LocalPlayer;
			}
		}
	}

	// Token: 0x060016C9 RID: 5833 RVA: 0x0007D980 File Offset: 0x0007BB80
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod)
	{
		this.GetPlayerRef(targetPlayerID);
		Utils.Log(WsaReflectionExtensions.GetDelegateName(rpcMethod) + "RPC called!");
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x0007D99F File Offset: 0x0007BB9F
	public override void CallRPC<T>(int targetPlayerID, MonoBehaviour component, NetworkSystem.RPC rpcMethod, RPCArgBuffer<T> args)
	{
		Utils.Log(WsaReflectionExtensions.GetDelegateName(rpcMethod) + "RPC called!");
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0007D9BE File Offset: 0x0007BBBE
	public override void CallRPC(int targetPlayerID, MonoBehaviour component, NetworkSystem.StringRPC rpcMethod, string message)
	{
		this.GetPlayerRef(targetPlayerID);
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x0007D9C8 File Offset: 0x0007BBC8
	public override void NetRaiseEventReliable(byte eventCode, object data)
	{
		byte[] byteData = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, byteData, false, null, default(RpcInfo));
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x0007D9F4 File Offset: 0x0007BBF4
	public override void NetRaiseEventUnreliable(byte eventCode, object data)
	{
		byte[] byteData = data.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, byteData, false, null, default(RpcInfo));
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x0007DA20 File Offset: 0x0007BC20
	public override void NetRaiseEventReliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] byteData = data.ByteSerialize();
		byte[] netOptsData = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedReliable(this.runner, eventCode, byteData, true, netOptsData, default(RpcInfo));
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x0007DA54 File Offset: 0x0007BC54
	public override void NetRaiseEventUnreliable(byte eventCode, object data, NetEventOptions opts)
	{
		byte[] byteData = data.ByteSerialize();
		byte[] netOptsData = opts.ByteSerialize();
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(this.runner, eventCode, byteData, true, netOptsData, default(RpcInfo));
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x000029BC File Offset: 0x00000BBC
	public override string GetRandomWeightedRegion()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0007DA88 File Offset: 0x0007BC88
	public override Task AwaitSceneReady()
	{
		NetworkSystemFusion.<AwaitSceneReady>d__93 <AwaitSceneReady>d__;
		<AwaitSceneReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitSceneReady>d__.<>4__this = this;
		<AwaitSceneReady>d__.<>1__state = -1;
		<AwaitSceneReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitSceneReady>d__93>(ref <AwaitSceneReady>d__);
		return <AwaitSceneReady>d__.<>t__builder.Task;
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x00002789 File Offset: 0x00000989
	public void OnJoinedSession()
	{
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x0007DACB File Offset: 0x0007BCCB
	public void OnJoinFailed(NetConnectFailedReason reason)
	{
		switch (reason)
		{
		case 1:
		case 3:
			break;
		case 2:
			this.lastConnectAttempt_WasFull = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x060016D4 RID: 5844 RVA: 0x0007DAE9 File Offset: 0x0007BCE9
	public void OnDisconnectedFromSession()
	{
		Utils.Log("On Disconnected");
		this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		base.UpdatePlayers();
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x0007DB03 File Offset: 0x0007BD03
	public void OnRunnerShutDown()
	{
		Utils.Log("Runner shutdown callback");
		if (this.internalState == NetworkSystemFusion.InternalState.Disconnecting)
		{
			this.internalState = NetworkSystemFusion.InternalState.Disconnected;
		}
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x0007DB21 File Offset: 0x0007BD21
	public void OnFusionPlayerJoined(PlayerRef player)
	{
		this.AwaitJoiningPlayerClientReady(player);
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x0007DB2C File Offset: 0x0007BD2C
	private Task AwaitJoiningPlayerClientReady(PlayerRef player)
	{
		NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__99 <AwaitJoiningPlayerClientReady>d__;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<AwaitJoiningPlayerClientReady>d__.<>4__this = this;
		<AwaitJoiningPlayerClientReady>d__.player = player;
		<AwaitJoiningPlayerClientReady>d__.<>1__state = -1;
		<AwaitJoiningPlayerClientReady>d__.<>t__builder.Start<NetworkSystemFusion.<AwaitJoiningPlayerClientReady>d__99>(ref <AwaitJoiningPlayerClientReady>d__);
		return <AwaitJoiningPlayerClientReady>d__.<>t__builder.Task;
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0007DB78 File Offset: 0x0007BD78
	public void OnFusionPlayerLeft(PlayerRef player)
	{
		if (this.IsTotalAuthority())
		{
			NetworkObject playerObject = this.runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				Utils.Log("Destroying player object for leaving player!");
				this.NetDestroy(playerObject.gameObject);
			}
			else
			{
				Utils.Log("Player left without destroying an avatar for it somehow?");
			}
		}
		NetPlayer player2 = base.GetPlayer(player);
		if (player2 == null)
		{
			Debug.LogError("Joining player doesnt have a NetPlayer somehow, this shouldnt happen");
		}
		base.PlayerLeft(player2);
		base.UpdatePlayers();
	}

	// Token: 0x060016D9 RID: 5849 RVA: 0x0007DBE8 File Offset: 0x0007BDE8
	protected override void UpdateNetPlayerList()
	{
		if (this.runner == null)
		{
			if (this.netPlayerCache.Count <= 1)
			{
				if (this.netPlayerCache.Exists((NetPlayer p) => p.IsLocal))
				{
					goto IL_84;
				}
			}
			this.netPlayerCache.ForEach(delegate(NetPlayer p)
			{
				this.playerPool.Return((FusionNetPlayer)p);
			});
			this.netPlayerCache.Clear();
			this.netPlayerCache.Add(new FusionNetPlayer(default(PlayerRef)));
			return;
		}
		IL_84:
		NetPlayer[] array;
		if (this.runner.IsSinglePlayer)
		{
			if (this.netPlayerCache.Count == 1 && this.netPlayerCache[0].IsLocal)
			{
				return;
			}
			bool flag = false;
			array = this.netPlayerCache.ToArray();
			if (this.netPlayerCache.Count > 0)
			{
				foreach (NetPlayer netPlayer in array)
				{
					if (((FusionNetPlayer)netPlayer).PlayerRef == this.runner.LocalPlayer)
					{
						flag = true;
					}
					else
					{
						this.playerPool.Return((FusionNetPlayer)netPlayer);
						this.netPlayerCache.Remove(netPlayer);
					}
				}
			}
			if (!flag)
			{
				FusionNetPlayer fusionNetPlayer = this.playerPool.Take();
				fusionNetPlayer.InitPlayer(this.runner.LocalPlayer);
				this.netPlayerCache.Add(fusionNetPlayer);
			}
		}
		foreach (PlayerRef playerRef in this.runner.ActivePlayers)
		{
			bool flag2 = false;
			for (int j = 0; j < this.netPlayerCache.Count; j++)
			{
				if (playerRef == ((FusionNetPlayer)this.netPlayerCache[j]).PlayerRef)
				{
					flag2 = true;
				}
			}
			if (!flag2)
			{
				FusionNetPlayer fusionNetPlayer2 = this.playerPool.Take();
				fusionNetPlayer2.InitPlayer(playerRef);
				this.netPlayerCache.Add(fusionNetPlayer2);
			}
		}
		array = this.netPlayerCache.ToArray();
		foreach (NetPlayer netPlayer2 in array)
		{
			bool flag3 = false;
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == ((FusionNetPlayer)netPlayer2).PlayerRef)
					{
						flag3 = true;
					}
				}
			}
			if (!flag3)
			{
				this.playerPool.Return((FusionNetPlayer)netPlayer2);
				this.netPlayerCache.Remove(netPlayer2);
			}
		}
	}

	// Token: 0x060016DA RID: 5850 RVA: 0x0007DEA4 File Offset: 0x0007C0A4
	public override void SetPlayerObject(GameObject playerInstance, int? owningPlayerID = null)
	{
		PlayerRef playerRef = this.runner.LocalPlayer;
		if (owningPlayerID != null)
		{
			playerRef = this.GetPlayerRef(owningPlayerID.Value);
		}
		this.runner.SetPlayerObject(playerRef, playerInstance.GetComponent<NetworkObject>());
	}

	// Token: 0x060016DB RID: 5851 RVA: 0x0007DEE8 File Offset: 0x0007C0E8
	private PlayerRef GetPlayerRef(int playerID)
	{
		if (this.runner == null)
		{
			Debug.LogWarning("There is no runner yet - returning default player ref");
			return default(PlayerRef);
		}
		foreach (PlayerRef result in this.runner.ActivePlayers)
		{
			if (result.PlayerId == playerID)
			{
				return result;
			}
		}
		Debug.LogWarning(string.Format("GetPlayerRef - Couldn't find active player with ID #{0}", playerID));
		return default(PlayerRef);
	}

	// Token: 0x060016DC RID: 5852 RVA: 0x0007DF84 File Offset: 0x0007C184
	public override NetPlayer GetLocalPlayer()
	{
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
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
		Debug.LogError("Somehow there is no local NetPlayer. This shoulnd't happen.");
		return null;
	}

	// Token: 0x060016DD RID: 5853 RVA: 0x0007E01C File Offset: 0x0007C21C
	public override NetPlayer GetPlayer(int PlayerID)
	{
		if (PlayerID == -1)
		{
			Debug.LogWarning("Attempting to get NetPlayer for local -1 ID.");
			return null;
		}
		foreach (NetPlayer netPlayer in this.netPlayerCache)
		{
			if (netPlayer.ActorNumber == PlayerID)
			{
				return netPlayer;
			}
		}
		if (this.netPlayerCache.Count == 0 || this.netPlayerCache.Count != this.runner.SessionInfo.PlayerCount)
		{
			base.UpdatePlayers();
			foreach (NetPlayer netPlayer2 in this.netPlayerCache)
			{
				if (netPlayer2.ActorNumber == PlayerID)
				{
					return netPlayer2;
				}
			}
		}
		Debug.LogError("Failed to find the player, before and after resyncing the player cache, this probably shoulnd't happen...");
		return null;
	}

	// Token: 0x060016DE RID: 5854 RVA: 0x0007E110 File Offset: 0x0007C310
	public override void SetMyNickName(string name)
	{
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags) && !name.StartsWith("gorilla"))
		{
			Debug.Log("[KID] Trying to set custom nickname but that permission has been disallowed");
			if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
			{
				GorillaTagger.Instance.rigSerializer.nickName = "gorilla";
			}
			return;
		}
		PlayerPrefs.SetString("playerName", name);
		if (this.InRoom && GorillaTagger.Instance.rigSerializer != null)
		{
			GorillaTagger.Instance.rigSerializer.nickName = name;
		}
	}

	// Token: 0x060016DF RID: 5855 RVA: 0x0007E1AA File Offset: 0x0007C3AA
	public override string GetMyNickName()
	{
		return PlayerPrefs.GetString("playerName");
	}

	// Token: 0x060016E0 RID: 5856 RVA: 0x0007E1B8 File Offset: 0x0007C3B8
	public override string GetMyDefaultName()
	{
		return "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x0007E1EC File Offset: 0x0007C3EC
	public override string GetNickName(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		return this.GetNickName(player);
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0007E208 File Offset: 0x0007C408
	public override string GetNickName(NetPlayer player)
	{
		if (player == null)
		{
			Debug.LogError("Cant get nick name as playerID doesnt have a NetPlayer...");
			return "";
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
		{
			return rigContainer.Rig.rigSerializer.defaultName.Value ?? "";
		}
		return rigContainer.Rig.rigSerializer.nickName.Value ?? "";
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x0007E281 File Offset: 0x0007C481
	public override string GetMyUserID()
	{
		return this.runner.GetPlayerUserId(this.runner.LocalPlayer);
	}

	// Token: 0x060016E4 RID: 5860 RVA: 0x0007E299 File Offset: 0x0007C499
	public override string GetUserID(int playerID)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(this.GetPlayerRef(playerID));
	}

	// Token: 0x060016E5 RID: 5861 RVA: 0x0007E2C1 File Offset: 0x0007C4C1
	public override string GetUserID(NetPlayer player)
	{
		if (this.runner == null)
		{
			return string.Empty;
		}
		return this.runner.GetPlayerUserId(((FusionNetPlayer)player).PlayerRef);
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x0007E2ED File Offset: 0x0007C4ED
	public override void SetMyTutorialComplete()
	{
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x0007E31E File Offset: 0x0007C51E
	public override bool GetMyTutorialCompletion()
	{
		return PlayerPrefs.GetString("didTutorial", "nope") == "done";
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x0007E33C File Offset: 0x0007C53C
	public override bool GetPlayerTutorialCompletion(int playerID)
	{
		NetPlayer player = this.GetPlayer(playerID);
		if (player == null)
		{
			Debug.LogError("Player not found");
			return false;
		}
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer == null)
		{
			Debug.LogError("VRRig not found for player");
			return false;
		}
		if (rigContainer.Rig.rigSerializer == null)
		{
			Debug.LogWarning("Vr rig serializer is not set up on the rig yet");
			return false;
		}
		return rigContainer.Rig.rigSerializer.tutorialComplete;
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x0007E3B2 File Offset: 0x0007C5B2
	public override int GlobalPlayerCount()
	{
		if (this.regionCrawler == null)
		{
			return 0;
		}
		return this.regionCrawler.PlayerCountGlobal;
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x0007E3D0 File Offset: 0x0007C5D0
	public override int GetOwningPlayerID(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(ref networkObject))
		{
			return -1;
		}
		if (this.runner.GameMode == 2)
		{
			return networkObject.StateAuthority.PlayerId;
		}
		return networkObject.InputAuthority.PlayerId;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x0007E414 File Offset: 0x0007C614
	public override bool IsObjectLocallyOwned(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(ref networkObject))
		{
			return false;
		}
		if (this.runner.GameMode == 2)
		{
			return networkObject.StateAuthority == this.runner.LocalPlayer;
		}
		return networkObject.InputAuthority == this.runner.LocalPlayer;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x0007E468 File Offset: 0x0007C668
	public override bool IsTotalAuthority()
	{
		return this.runner.Mode == 1 || this.runner.Mode == 2 || this.runner.GameMode == 1 || this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0007E4A4 File Offset: 0x0007C6A4
	public override bool ShouldWriteObjectData(GameObject obj)
	{
		NetworkObject networkObject;
		return obj.TryGetComponent<NetworkObject>(ref networkObject) && networkObject.HasStateAuthority;
	}

	// Token: 0x060016EE RID: 5870 RVA: 0x0007E4C4 File Offset: 0x0007C6C4
	public override bool ShouldUpdateObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (!obj.TryGetComponent<NetworkObject>(ref networkObject))
		{
			return true;
		}
		if (this.IsTotalAuthority())
		{
			return true;
		}
		if (networkObject.InputAuthority.IsRealPlayer && !networkObject.InputAuthority.IsRealPlayer)
		{
			return networkObject.InputAuthority == this.runner.LocalPlayer;
		}
		return this.runner.IsSharedModeMasterClient;
	}

	// Token: 0x060016EF RID: 5871 RVA: 0x0007E52C File Offset: 0x0007C72C
	public override bool IsObjectRoomObject(GameObject obj)
	{
		NetworkObject networkObject;
		if (obj.TryGetComponent<NetworkObject>(ref networkObject))
		{
			Debug.LogWarning("Fusion currently automatically passes false for roomobject check.");
			return false;
		}
		return false;
	}

	// Token: 0x060016F0 RID: 5872 RVA: 0x0007E550 File Offset: 0x0007C750
	private void OnMasterSwitch(NetPlayer player)
	{
		if (this.runner.IsSharedModeMasterClient)
		{
			Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
			dictionary.Add("MasterClient", base.LocalPlayer.ActorNumber);
			Dictionary<string, SessionProperty> dictionary2 = dictionary;
			this.runner.SessionInfo.UpdateCustomProperties(dictionary2);
		}
	}

	// Token: 0x0400208E RID: 8334
	private NetworkSystemFusion.InternalState internalState;

	// Token: 0x0400208F RID: 8335
	private FusionInternalRPCs internalRPCProvider;

	// Token: 0x04002090 RID: 8336
	private FusionCallbackHandler callbackHandler;

	// Token: 0x04002091 RID: 8337
	private FusionRegionCrawler regionCrawler;

	// Token: 0x04002092 RID: 8338
	private GameObject volatileNetObj;

	// Token: 0x04002093 RID: 8339
	private AuthenticationValues cachedPlayfabAuth;

	// Token: 0x04002094 RID: 8340
	private const string playerPropertiesPath = "P_FusionProperties";

	// Token: 0x04002095 RID: 8341
	private bool lastConnectAttempt_WasFull;

	// Token: 0x04002096 RID: 8342
	private VoiceConnection FusionVoice;

	// Token: 0x04002097 RID: 8343
	private CustomObjectProvider myObjectProvider;

	// Token: 0x04002098 RID: 8344
	private ObjectPool<FusionNetPlayer> playerPool;

	// Token: 0x04002099 RID: 8345
	public List<NetworkObject> cachedNetSceneObjects = new List<NetworkObject>();

	// Token: 0x0400209A RID: 8346
	private List<INetworkRunnerCallbacks> objectsThatNeedCallbacks = new List<INetworkRunnerCallbacks>();

	// Token: 0x0400209B RID: 8347
	private Queue<NetworkObject> registrationQueue = new Queue<NetworkObject>();

	// Token: 0x0400209C RID: 8348
	private bool isProcessingQueue;

	// Token: 0x020003AE RID: 942
	private enum InternalState
	{
		// Token: 0x0400209E RID: 8350
		AwaitingAuth,
		// Token: 0x0400209F RID: 8351
		Idle,
		// Token: 0x040020A0 RID: 8352
		Searching_Joining,
		// Token: 0x040020A1 RID: 8353
		Searching_Joined,
		// Token: 0x040020A2 RID: 8354
		Searching_JoinFailed,
		// Token: 0x040020A3 RID: 8355
		Searching_Disconnecting,
		// Token: 0x040020A4 RID: 8356
		Searching_Disconnected,
		// Token: 0x040020A5 RID: 8357
		ConnectingToRoom,
		// Token: 0x040020A6 RID: 8358
		ConnectedToRoom,
		// Token: 0x040020A7 RID: 8359
		JoinRoomFailed,
		// Token: 0x040020A8 RID: 8360
		Disconnecting,
		// Token: 0x040020A9 RID: 8361
		Disconnected,
		// Token: 0x040020AA RID: 8362
		StateCheckFailed
	}
}
