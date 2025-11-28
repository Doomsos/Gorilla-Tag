using System;
using System.Collections.Generic;
using System.Timers;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTagScripts;
using Photon.Pun;
using TagEffects;
using UnityEngine;

// Token: 0x02000BF0 RID: 3056
internal class RoomSystem : MonoBehaviour
{
	// Token: 0x06004B74 RID: 19316 RVA: 0x00189CE8 File Offset: 0x00187EE8
	internal static void DeserializeLaunchProjectile(object[] projectileData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "LaunchSlingshotProjectile");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		byte b = Convert.ToByte(projectileData[5]);
		byte b2 = Convert.ToByte(projectileData[6]);
		byte b3 = Convert.ToByte(projectileData[7]);
		byte b4 = Convert.ToByte(projectileData[8]);
		Color32 color;
		color..ctor(b, b2, b3, b4);
		Vector3 position = (Vector3)projectileData[0];
		Vector3 velocity = (Vector3)projectileData[1];
		float num = 10000f;
		if (position.IsValid(num))
		{
			float num2 = 10000f;
			if (velocity.IsValid(num2) && float.IsFinite((float)b) && float.IsFinite((float)b2) && float.IsFinite((float)b3) && float.IsFinite((float)b4))
			{
				RoomSystem.ProjectileSource projectileSource = (RoomSystem.ProjectileSource)Convert.ToInt32(projectileData[2]);
				int projectileIndex = Convert.ToInt32(projectileData[3]);
				bool overridecolour = Convert.ToBoolean(projectileData[4]);
				VRRig rig = rigContainer.Rig;
				if (rig.isOfflineVRRig || rig.IsPositionInRange(position, 4f))
				{
					RoomSystem.launchProjectile.targetRig = rig;
					RoomSystem.launchProjectile.position = position;
					RoomSystem.launchProjectile.velocity = velocity;
					RoomSystem.launchProjectile.overridecolour = overridecolour;
					RoomSystem.launchProjectile.colour = color;
					RoomSystem.launchProjectile.projectileIndex = projectileIndex;
					RoomSystem.launchProjectile.projectileSource = projectileSource;
					RoomSystem.launchProjectile.messageInfo = info;
					FXSystem.PlayFXForRig(FXType.Projectile, RoomSystem.launchProjectile, info);
				}
				return;
			}
		}
		GorillaNot.instance.SendReport("invalid projectile state", player.UserId, player.NickName);
	}

	// Token: 0x06004B75 RID: 19317 RVA: 0x00189E80 File Offset: 0x00188080
	internal static void SendLaunchProjectile(Vector3 position, Vector3 velocity, RoomSystem.ProjectileSource projectileSource, int projectileCount, bool randomColour, byte r, byte g, byte b, byte a)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		RoomSystem.projectileSendData[0] = position;
		RoomSystem.projectileSendData[1] = velocity;
		RoomSystem.projectileSendData[2] = projectileSource;
		RoomSystem.projectileSendData[3] = projectileCount;
		RoomSystem.projectileSendData[4] = randomColour;
		RoomSystem.projectileSendData[5] = r;
		RoomSystem.projectileSendData[6] = g;
		RoomSystem.projectileSendData[7] = b;
		RoomSystem.projectileSendData[8] = a;
		byte b2 = 0;
		object obj = RoomSystem.projectileSendData;
		RoomSystem.SendEvent(b2, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004B76 RID: 19318 RVA: 0x00189F28 File Offset: 0x00188128
	internal static void ImpactEffect(VRRig targetRig, Vector3 position, float r, float g, float b, float a, int projectileCount, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		RoomSystem.impactEffect.targetRig = targetRig;
		RoomSystem.impactEffect.position = position;
		RoomSystem.impactEffect.colour = new Color(r, g, b, a);
		RoomSystem.impactEffect.projectileIndex = projectileCount;
		FXSystem.PlayFXForRig(FXType.Impact, RoomSystem.impactEffect, info);
	}

	// Token: 0x06004B77 RID: 19319 RVA: 0x00189F7C File Offset: 0x0018817C
	internal static void DeserializeImpactEffect(object[] impactData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "SpawnSlingshotPlayerImpactEffect");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || rigContainer.Rig.projectileWeapon.IsNull())
		{
			return;
		}
		float num = Convert.ToSingle(impactData[1]);
		float num2 = Convert.ToSingle(impactData[2]);
		float num3 = Convert.ToSingle(impactData[3]);
		float num4 = Convert.ToSingle(impactData[4]);
		Vector3 position = (Vector3)impactData[0];
		float num5 = 10000f;
		if (!position.IsValid(num5) || !float.IsFinite(num) || !float.IsFinite(num2) || !float.IsFinite(num3) || !float.IsFinite(num4))
		{
			GorillaNot.instance.SendReport("invalid impact state", player.UserId, player.NickName);
			return;
		}
		int projectileCount = Convert.ToInt32(impactData[5]);
		RoomSystem.ImpactEffect(rigContainer.Rig, position, num, num2, num3, num4, projectileCount, info);
	}

	// Token: 0x06004B78 RID: 19320 RVA: 0x0018A06C File Offset: 0x0018826C
	internal static void SendImpactEffect(Vector3 position, float r, float g, float b, float a, int projectileCount)
	{
		RoomSystem.ImpactEffect(VRRigCache.Instance.localRig.Rig, position, r, g, b, a, projectileCount, default(PhotonMessageInfoWrapped));
		if (RoomSystem.joinedRoom)
		{
			RoomSystem.impactSendData[0] = position;
			RoomSystem.impactSendData[1] = r;
			RoomSystem.impactSendData[2] = g;
			RoomSystem.impactSendData[3] = b;
			RoomSystem.impactSendData[4] = a;
			RoomSystem.impactSendData[5] = projectileCount;
			byte b2 = 1;
			object obj = RoomSystem.impactSendData;
			RoomSystem.SendEvent(b2, obj, NetworkSystemRaiseEvent.neoOthers, false);
		}
	}

	// Token: 0x06004B79 RID: 19321 RVA: 0x0018A10C File Offset: 0x0018830C
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
		RoomSystem.playerImpactEffectPrefab = this.roomSettings.PlayerImpactEffect;
		RoomSystem.callbackInstance = this;
		RoomSystem.disconnectTimer.Interval = (double)(this.roomSettings.PausedDCTimer * 1000);
		RoomSystem.playerEffectDictionary.Clear();
		foreach (RoomSystem.PlayerEffectConfig playerEffectConfig in this.roomSettings.PlayerEffects)
		{
			RoomSystem.playerEffectDictionary.Add(playerEffectConfig.type, playerEffectConfig);
		}
		this.roomSettings.ResyncNetworkTimeTimer.callback = new Action(PhotonNetwork.FetchServerTimestamp);
	}

	// Token: 0x06004B7A RID: 19322 RVA: 0x0018A1DC File Offset: 0x001883DC
	private void Start()
	{
		List<PhotonView> list = new List<PhotonView>(20);
		foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
		{
			if (photonView.IsRoomView)
			{
				list.Add(photonView);
			}
		}
		RoomSystem.sceneViews = list.ToArray();
		NetworkSystem.Instance.OnRaiseEvent += new Action<byte, object, int>(RoomSystem.OnEvent);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnLeftRoom);
	}

	// Token: 0x06004B7B RID: 19323 RVA: 0x0018A2E8 File Offset: 0x001884E8
	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			RoomSystem.disconnectTimer.Stop();
			return;
		}
		if (RoomSystem.JoinedRoom)
		{
			RoomSystem.disconnectTimer.Start();
		}
	}

	// Token: 0x06004B7C RID: 19324 RVA: 0x0018A30C File Offset: 0x0018850C
	private void OnJoinedRoom()
	{
		RoomSystem.joinedRoom = true;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RoomSystem.netPlayersInRoom.Add(netPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(RoomSystem.netPlayersInRoom);
		RoomSystem.roomGameMode = NetworkSystem.Instance.GameModeString;
		RoomSystem.WasRoomPrivate = NetworkSystem.Instance.SessionIsPrivate;
		RoomSystem.IsVStumpRoom = NetworkSystem.Instance.RoomName.StartsWith(GorillaComputer.instance.VStumpRoomPrepend);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int j = 0; j < this.prefabsToInstantiateByPath.Length; j++)
			{
				this.prefabsInstantiated.Add(NetworkSystem.Instance.NetInstantiate(this.prefabsToInstantiate[j], Vector3.zero, Quaternion.identity, true));
			}
		}
		try
		{
			RoomSystem.m_roomSizeOnJoin = PhotonNetwork.CurrentRoom.MaxPlayers;
			this.roomSettings.ExpectedUsersTimer.Start();
			this.roomSettings.ResyncNetworkTimeTimer.Start();
			DelegateListProcessor joinedRoomEvent = RoomSystem.JoinedRoomEvent;
			if (joinedRoomEvent != null)
			{
				joinedRoomEvent.InvokeSafe();
			}
			this.roomSettings.ResyncNetworkTimeTimer.OnTimedEvent();
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x06004B7D RID: 19325 RVA: 0x0018A448 File Offset: 0x00188648
	private void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.IsLocal)
		{
			return;
		}
		if (!RoomSystem.netPlayersInRoom.Contains(newPlayer))
		{
			RoomSystem.netPlayersInRoom.Add(newPlayer);
		}
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(newPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerJoinedEvent = RoomSystem.PlayerJoinedEvent;
			if (playerJoinedEvent != null)
			{
				playerJoinedEvent.InvokeSafe(newPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x06004B7E RID: 19326 RVA: 0x0018A4C0 File Offset: 0x001886C0
	private void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.joinedRoom = false;
		RoomSystem.netPlayersInRoom.Clear();
		RoomSystem.roomGameMode = "";
		PlayerCosmeticsSystem.StaticReset();
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < RoomSystem.sceneViews.Length; i++)
		{
			RoomSystem.sceneViews[i].ControllerActorNr = actorNumber;
			RoomSystem.sceneViews[i].OwnerActorNr = actorNumber;
		}
		this.roomSettings.StatusEffectLimiter.Reset();
		this.roomSettings.SoundEffectLimiter.Reset();
		this.roomSettings.SoundEffectOtherLimiter.Reset();
		this.roomSettings.PlayerEffectLimiter.Reset();
		try
		{
			RoomSystem.m_roomSizeOnJoin = 0;
			this.roomSettings.ExpectedUsersTimer.Stop();
			this.roomSettings.ResyncNetworkTimeTimer.Stop();
			DelegateListProcessor leftRoomEvent = RoomSystem.LeftRoomEvent;
			if (leftRoomEvent != null)
			{
				leftRoomEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
		GC.Collect(0);
	}

	// Token: 0x06004B7F RID: 19327 RVA: 0x0018A5CC File Offset: 0x001887CC
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Player how left doesnt have a reference somehow");
		}
		foreach (NetPlayer netPlayer2 in RoomSystem.netPlayersInRoom)
		{
			if (netPlayer2 == netPlayer)
			{
				RoomSystem.netPlayersInRoom.Remove(netPlayer2);
				break;
			}
		}
		RoomSystem.netPlayersInRoom.Remove(netPlayer);
		try
		{
			DelegateListProcessor<NetPlayer> playerLeftEvent = RoomSystem.PlayerLeftEvent;
			if (playerLeftEvent != null)
			{
				playerLeftEvent.InvokeSafe(netPlayer);
			}
			DelegateListProcessor playersChangedEvent = RoomSystem.PlayersChangedEvent;
			if (playersChangedEvent != null)
			{
				playersChangedEvent.InvokeSafe();
			}
		}
		catch (Exception)
		{
			Debug.LogError("RoomSystem failed invoking event");
		}
	}

	// Token: 0x17000708 RID: 1800
	// (get) Token: 0x06004B80 RID: 19328 RVA: 0x0018A680 File Offset: 0x00188880
	// (set) Token: 0x06004B81 RID: 19329 RVA: 0x0018A687 File Offset: 0x00188887
	private static bool UseRoomSizeOverride { get; set; }

	// Token: 0x17000709 RID: 1801
	// (get) Token: 0x06004B82 RID: 19330 RVA: 0x0018A68F File Offset: 0x0018888F
	// (set) Token: 0x06004B83 RID: 19331 RVA: 0x0018A696 File Offset: 0x00188896
	public static byte RoomSizeOverride { get; set; }

	// Token: 0x1700070A RID: 1802
	// (get) Token: 0x06004B84 RID: 19332 RVA: 0x0018A69E File Offset: 0x0018889E
	public static List<NetPlayer> PlayersInRoom
	{
		get
		{
			return RoomSystem.netPlayersInRoom;
		}
	}

	// Token: 0x1700070B RID: 1803
	// (get) Token: 0x06004B85 RID: 19333 RVA: 0x0018A6A5 File Offset: 0x001888A5
	public static string RoomGameMode
	{
		get
		{
			return RoomSystem.roomGameMode;
		}
	}

	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x06004B86 RID: 19334 RVA: 0x0018A6AC File Offset: 0x001888AC
	public static bool JoinedRoom
	{
		get
		{
			return NetworkSystem.Instance.InRoom && RoomSystem.joinedRoom;
		}
	}

	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x06004B87 RID: 19335 RVA: 0x0018A6C1 File Offset: 0x001888C1
	public static bool AmITheHost
	{
		get
		{
			return NetworkSystem.Instance.IsMasterClient || !NetworkSystem.Instance.InRoom;
		}
	}

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x06004B88 RID: 19336 RVA: 0x0018A6DE File Offset: 0x001888DE
	// (set) Token: 0x06004B89 RID: 19337 RVA: 0x0018A6E5 File Offset: 0x001888E5
	public static bool IsVStumpRoom { get; private set; }

	// Token: 0x1700070F RID: 1807
	// (get) Token: 0x06004B8A RID: 19338 RVA: 0x0018A6ED File Offset: 0x001888ED
	// (set) Token: 0x06004B8B RID: 19339 RVA: 0x0018A6F4 File Offset: 0x001888F4
	public static bool WasRoomPrivate { get; private set; }

	// Token: 0x06004B8C RID: 19340 RVA: 0x0018A6FC File Offset: 0x001888FC
	static RoomSystem()
	{
		RoomSystem.disconnectTimer.Elapsed += new ElapsedEventHandler(RoomSystem.TimerDC);
		RoomSystem.disconnectTimer.AutoReset = false;
		RoomSystem.StaticLoad();
	}

	// Token: 0x06004B8D RID: 19341 RVA: 0x0018A85C File Offset: 0x00188A5C
	[OnEnterPlay_Run]
	private static void StaticLoad()
	{
		RoomSystem.netEventCallbacks[0] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeLaunchProjectile);
		RoomSystem.netEventCallbacks[1] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeImpactEffect);
		RoomSystem.netEventCallbacks[4] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForNearby);
		RoomSystem.netEventCallbacks[7] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForParty);
		RoomSystem.netEventCallbacks[10] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForElevator);
		RoomSystem.netEventCallbacks[11] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.SearchForShuttle);
		RoomSystem.netEventCallbacks[2] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeStatusEffect);
		RoomSystem.netEventCallbacks[3] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeSoundEffect);
		RoomSystem.netEventCallbacks[5] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializeReportTouch);
		RoomSystem.netEventCallbacks[8] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerLaunched);
		RoomSystem.netEventCallbacks[6] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerEffect);
		RoomSystem.netEventCallbacks[9] = new Action<object[], PhotonMessageInfoWrapped>(RoomSystem.DeserializePlayerHit);
		RoomSystem.soundEffectCallback = new Action<RoomSystem.SoundEffect, NetPlayer>(RoomSystem.OnPlaySoundEffect);
		RoomSystem.statusEffectCallback = new Action<RoomSystem.StatusEffects>(RoomSystem.OnStatusEffect);
	}

	// Token: 0x06004B8E RID: 19342 RVA: 0x0018A9A2 File Offset: 0x00188BA2
	private static void TimerDC(object sender, ElapsedEventArgs args)
	{
		RoomSystem.disconnectTimer.Stop();
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		PhotonNetwork.Disconnect();
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06004B8F RID: 19343 RVA: 0x0018A9C0 File Offset: 0x00188BC0
	public static byte GetRoomSize(string gameMode = "")
	{
		if (RoomSystem.joinedRoom)
		{
			if (RoomSystem.m_roomSizeOnJoin > 10)
			{
				return 10;
			}
			return RoomSystem.m_roomSizeOnJoin;
		}
		else
		{
			if (RoomSystem.UseRoomSizeOverride)
			{
				return RoomSystem.RoomSizeOverride;
			}
			return 10;
		}
	}

	// Token: 0x06004B90 RID: 19344 RVA: 0x0018A9EA File Offset: 0x00188BEA
	public static byte GetRoomSizeForCreate(string gameMode = "")
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x06004B91 RID: 19345 RVA: 0x0018A9FB File Offset: 0x00188BFB
	public static void OverrideRoomSize(byte roomSize)
	{
		if (roomSize < 1)
		{
			roomSize = 1;
		}
		if (roomSize > 10)
		{
			roomSize = 10;
		}
		if (roomSize == 10)
		{
			RoomSystem.UseRoomSizeOverride = false;
			RoomSystem.RoomSizeOverride = 10;
			return;
		}
		RoomSystem.UseRoomSizeOverride = true;
		RoomSystem.RoomSizeOverride = roomSize;
	}

	// Token: 0x06004B92 RID: 19346 RVA: 0x0018A9EA File Offset: 0x00188BEA
	public static byte GetOverridenRoomSize()
	{
		if (RoomSystem.UseRoomSizeOverride)
		{
			return RoomSystem.RoomSizeOverride;
		}
		return 10;
	}

	// Token: 0x06004B93 RID: 19347 RVA: 0x0018AA2C File Offset: 0x00188C2C
	public static void ClearOverridenRoomSize()
	{
		RoomSystem.UseRoomSizeOverride = false;
		RoomSystem.RoomSizeOverride = 10;
	}

	// Token: 0x06004B94 RID: 19348 RVA: 0x0018AA3B File Offset: 0x00188C3B
	public static void MakeRoomMultiplayer(byte roomSize)
	{
		if (!RoomSystem.joinedRoom || RoomSystem.m_roomSizeOnJoin > 1)
		{
			return;
		}
		if (roomSize > 10)
		{
			roomSize = 10;
		}
		RoomSystem.m_roomSizeOnJoin = roomSize;
		PhotonNetwork.CurrentRoom.MaxPlayers = roomSize;
	}

	// Token: 0x06004B95 RID: 19349 RVA: 0x0018AA67 File Offset: 0x00188C67
	internal static void SendEvent(in byte code, in object evData, in NetPlayer target, bool reliable)
	{
		NetworkSystemRaiseEvent.neoTarget.TargetActors[0] = target.ActorNumber;
		RoomSystem.SendEvent(code, evData, NetworkSystemRaiseEvent.neoTarget, reliable);
	}

	// Token: 0x06004B96 RID: 19350 RVA: 0x0018AA89 File Offset: 0x00188C89
	internal static void SendEvent(in byte code, in object evData, in NetEventOptions neo, bool reliable)
	{
		RoomSystem.sendEventData[0] = NetworkSystem.Instance.ServerTimestamp;
		RoomSystem.sendEventData[1] = code;
		RoomSystem.sendEventData[2] = evData;
		NetworkSystemRaiseEvent.RaiseEvent(3, RoomSystem.sendEventData, neo, reliable);
	}

	// Token: 0x06004B97 RID: 19351 RVA: 0x0018AAC6 File Offset: 0x00188CC6
	private static void OnEvent(EventData data)
	{
		RoomSystem.OnEvent(data.Code, data.CustomData, data.Sender);
	}

	// Token: 0x06004B98 RID: 19352 RVA: 0x0018AAE0 File Offset: 0x00188CE0
	private static void OnEvent(byte code, object data, int source)
	{
		NetPlayer netPlayer;
		if (code != 3 || !Utils.PlayerInRoom(source, out netPlayer))
		{
			return;
		}
		try
		{
			object[] array = (object[])data;
			int tick = Convert.ToInt32(array[0]);
			byte b = Convert.ToByte(array[1]);
			object[] array2 = null;
			if (array.Length > 2)
			{
				object obj = array[2];
				array2 = ((obj == null) ? null : ((object[])obj));
			}
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(netPlayer.ActorNumber, tick);
			Action<object[], PhotonMessageInfoWrapped> action;
			if (RoomSystem.netEventCallbacks.TryGetValue(b, ref action))
			{
				action.Invoke(array2, photonMessageInfoWrapped);
			}
		}
		catch
		{
		}
	}

	// Token: 0x06004B99 RID: 19353 RVA: 0x0018AB74 File Offset: 0x00188D74
	internal static void SearchForNearby(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "JoinPubWithNearby");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (!GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			GorillaNot.instance.SendReport("possible kick attempt", player.UserId, player.NickName);
			return;
		}
		if (!flag || !RoomSystem.WasRoomPrivate)
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingNearby);
	}

	// Token: 0x06004B9A RID: 19354 RVA: 0x0018AC54 File Offset: 0x00188E54
	internal static void SearchForParty(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "PARTY_JOIN");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		if (!FriendshipGroupDetection.Instance.IsInMyGroup(info.Sender.UserId))
		{
			GorillaNot.instance.SendReport("possible kick attempt", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (PlayFabAuthenticator.instance.GetSafety())
		{
			return;
		}
		PhotonNetworkController.Instance.AttemptToFollowIntoPub(info.Sender.UserId, info.Sender.ActorNumber, newKeyStr, shufflerStr, JoinType.FollowingParty);
	}

	// Token: 0x06004B9B RID: 19355 RVA: 0x0018AD24 File Offset: 0x00188F24
	internal static void SearchForElevator(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		if (GRElevatorManager.ValidElevatorNetworking(info.Sender.ActorNumber) && GRElevatorManager.ValidElevatorNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber))
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x06004B9C RID: 19356 RVA: 0x0018ADE4 File Offset: 0x00188FE4
	internal static void SearchForShuttle(object[] shuffleData, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "JoinPubWithElevator");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 23, NetworkSystem.Instance.SimTime))
		{
			return;
		}
		string shufflerStr = (string)shuffleData[0];
		string newKeyStr = (string)shuffleData[1];
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
		bool flag2 = GRElevatorManager.ValidShuttleNetworking(info.Sender.ActorNumber);
		bool flag3 = GRElevatorManager.ValidShuttleNetworking(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (flag2 && flag3)
		{
			if (!flag)
			{
				GRElevatorManager.JoinPublicRoom();
				return;
			}
			PhotonNetworkController.Instance.AttemptToFollowIntoPub(player.UserId, player.ActorNumber, newKeyStr, shufflerStr, JoinType.JoinWithElevator);
		}
	}

	// Token: 0x06004B9D RID: 19357 RVA: 0x0018AEA8 File Offset: 0x001890A8
	internal static void SendNearbyFollowCommand(GorillaFriendCollider friendCollider, string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				byte b = 4;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x06004B9E RID: 19358 RVA: 0x0018AF58 File Offset: 0x00189158
	internal static void SendPartyFollowCommand(string shuffler, string keyStr)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			if (vrrig.IsLocalPartyMember && vrrig.creator != NetworkSystem.Instance.LocalPlayer)
			{
				netEventOptions.TargetActors[0] = vrrig.creator.ActorNumber;
				Debug.Log(string.Format("SendGroupFollowCommand - sendEvent to {0} from {1}, shuffler {2} key {3}", new object[]
				{
					vrrig.creator.NickName,
					NetworkSystem.Instance.LocalPlayer.UserId,
					RoomSystem.groupJoinSendData[0],
					RoomSystem.groupJoinSendData[1]
				}));
				byte b = 7;
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(b, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x06004B9F RID: 19359 RVA: 0x0018B060 File Offset: 0x00189260
	internal static void SendElevatorFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(10, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x06004BA0 RID: 19360 RVA: 0x0018B06D File Offset: 0x0018926D
	internal static void SendShuttleFollowCommand(string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.SendGroupJoinFollowCommand(11, shuffler, keyStr, sourceFriendCollider, targetFriendCollider);
	}

	// Token: 0x06004BA1 RID: 19361 RVA: 0x0018B07C File Offset: 0x0018927C
	internal static void SendGroupJoinFollowCommand(byte eventType, string shuffler, string keyStr, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider targetFriendCollider)
	{
		RoomSystem.groupJoinSendData[0] = shuffler;
		RoomSystem.groupJoinSendData[1] = keyStr;
		NetEventOptions netEventOptions = new NetEventOptions
		{
			TargetActors = new int[1]
		};
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || (targetFriendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) && netPlayer != NetworkSystem.Instance.LocalPlayer))
			{
				netEventOptions.TargetActors[0] = netPlayer.ActorNumber;
				Debug.Log(string.Format("SendElevatorFollowCommand - sendEvent to {0} from {1}, shuffler {2} key {3}", new object[]
				{
					netPlayer.NickName,
					NetworkSystem.Instance.LocalPlayer.UserId,
					RoomSystem.groupJoinSendData[0],
					RoomSystem.groupJoinSendData[1]
				}));
				object obj = RoomSystem.groupJoinSendData;
				RoomSystem.SendEvent(eventType, obj, netEventOptions, false);
			}
		}
	}

	// Token: 0x06004BA2 RID: 19362 RVA: 0x0018B188 File Offset: 0x00189388
	private static void DeserializeReportTouch(object[] data, PhotonMessageInfoWrapped info)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer netPlayer = (NetPlayer)data[0];
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action.Invoke(netPlayer, player);
	}

	// Token: 0x06004BA3 RID: 19363 RVA: 0x0018B1D0 File Offset: 0x001893D0
	internal static void SendReportTouch(NetPlayer touchedNetPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			RoomSystem.reportTouchSendData[0] = touchedNetPlayer;
			byte b = 5;
			object obj = RoomSystem.reportTouchSendData;
			RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoMaster, false);
			return;
		}
		Action<NetPlayer, NetPlayer> action = RoomSystem.playerTouchedCallback;
		if (action == null)
		{
			return;
		}
		action.Invoke(touchedNetPlayer, NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06004BA4 RID: 19364 RVA: 0x0018B224 File Offset: 0x00189424
	internal static void LaunchPlayer(NetPlayer player, Vector3 velocity)
	{
		RoomSystem.reportTouchSendData[0] = velocity;
		byte b = 8;
		object obj = RoomSystem.reportTouchSendData;
		RoomSystem.SendEvent(b, obj, player, false);
	}

	// Token: 0x06004BA5 RID: 19365 RVA: 0x0018B254 File Offset: 0x00189454
	private static void DeserializePlayerLaunched(object[] data, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializePlayerLaunched");
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (activeGameMode != null && activeGameMode.GameType() == GameModeType.Guardian && info.Sender == NetworkSystem.Instance.MasterClient)
		{
			object obj = data[0];
			if (obj is Vector3)
			{
				Vector3 velocity = (Vector3)obj;
				float num = 10000f;
				if (velocity.IsValid(num) && velocity.magnitude <= 20f && RoomSystem.playerLaunchedCallLimiter.CheckCallTime(Time.time))
				{
					GTPlayer.Instance.DoLaunch(velocity);
					return;
				}
			}
		}
	}

	// Token: 0x06004BA6 RID: 19366 RVA: 0x0018B2E8 File Offset: 0x001894E8
	internal static void HitPlayer(NetPlayer player, Vector3 direction, float strength)
	{
		RoomSystem.reportHitSendData[0] = direction;
		RoomSystem.reportHitSendData[1] = strength;
		RoomSystem.reportHitSendData[2] = player.ActorNumber;
		byte b = 9;
		object obj = RoomSystem.reportHitSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			rigContainer.Rig.DisableHitWithKnockBack();
		}
	}

	// Token: 0x06004BA7 RID: 19367 RVA: 0x0018B354 File Offset: 0x00189554
	private static void DeserializePlayerHit(object[] data, PhotonMessageInfoWrapped info)
	{
		object obj = data[0];
		if (obj is Vector3)
		{
			Vector3 vector = (Vector3)obj;
			obj = data[1];
			if (obj is float)
			{
				float value = (float)obj;
				obj = data[2];
				if (obj is int)
				{
					int num = (int)obj;
					float num2 = 10000f;
					RigContainer rigContainer;
					if (vector.IsValid(num2) && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) && FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 20, info.SentServerTime))
					{
						float num3 = value.ClampSafe(0f, 10f);
						GorillaNot.IncrementRPCCall(info, "DeserializePlayerHit");
						if (num == NetworkSystem.Instance.LocalPlayer.ActorNumber)
						{
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
							CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect2;
							if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, ref cosmeticEffect))
							{
								if (!cosmeticEffect.IsGameModeAllowed())
								{
									return;
								}
								float num4 = (num3 * cosmeticEffect.knockbackStrength * cosmeticEffect.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect.minKnockbackStrength, cosmeticEffect.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num4 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num4, cosmeticEffect.forceOffTheGround);
							}
							else if (GorillaTagger.Instance.offlineVRRig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback, ref cosmeticEffect2))
							{
								if (!cosmeticEffect2.IsGameModeAllowed())
								{
									return;
								}
								float num5 = (num3 * cosmeticEffect2.knockbackStrength * cosmeticEffect2.knockbackStrengthMultiplier).ClampSafe(cosmeticEffect2.minKnockbackStrength, cosmeticEffect2.maxKnockbackStrength);
								if (cosmeticEffect.applyScaleToKnockbackStrength)
								{
									num5 *= GTPlayer.Instance.scale;
								}
								GTPlayer.Instance.ApplyKnockback(vector.normalized, num5, cosmeticEffect2.forceOffTheGround);
							}
						}
						NetPlayer player = NetworkSystem.Instance.GetPlayer(num);
						RigContainer rigContainer2;
						if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer2))
						{
							rigContainer2.Rig.DisableHitWithKnockBack();
						}
						return;
					}
				}
			}
		}
	}

	// Token: 0x06004BA8 RID: 19368 RVA: 0x0018B540 File Offset: 0x00189740
	private static void SetSlowedTime()
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Slowed)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		}
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, GorillaTagger.Instance.slowCooldown);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06004BA9 RID: 19369 RVA: 0x0018B5BC File Offset: 0x001897BC
	private static void SetTaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
	}

	// Token: 0x06004BAA RID: 19370 RVA: 0x0018B62C File Offset: 0x0018982C
	private static void SetFrozenTime()
	{
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null)
		{
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Slowed, gorillaFreezeTagManager.freezeDuration);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
		}
	}

	// Token: 0x06004BAB RID: 19371 RVA: 0x0018B6A5 File Offset: 0x001898A5
	private static void SetJoinedTaggedTime()
	{
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06004BAC RID: 19372 RVA: 0x0018B6E8 File Offset: 0x001898E8
	private static void SetUntaggedTime()
	{
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.None, 0f);
		GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
		GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
	}

	// Token: 0x06004BAD RID: 19373 RVA: 0x0018B743 File Offset: 0x00189943
	private static void OnStatusEffect(RoomSystem.StatusEffects status)
	{
		switch (status)
		{
		case RoomSystem.StatusEffects.TaggedTime:
			RoomSystem.SetTaggedTime();
			return;
		case RoomSystem.StatusEffects.JoinedTaggedTime:
			RoomSystem.SetJoinedTaggedTime();
			return;
		case RoomSystem.StatusEffects.SetSlowedTime:
			RoomSystem.SetSlowedTime();
			return;
		case RoomSystem.StatusEffects.UnTagged:
			RoomSystem.SetUntaggedTime();
			return;
		case RoomSystem.StatusEffects.FrozenTime:
			RoomSystem.SetFrozenTime();
			return;
		default:
			return;
		}
	}

	// Token: 0x06004BAE RID: 19374 RVA: 0x0018B780 File Offset: 0x00189980
	private static void DeserializeStatusEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "DeserializeStatusEffect");
		if (!player.IsMasterClient)
		{
			GorillaNot.instance.SendReport("invalid status", player.UserId, player.NickName);
			return;
		}
		if (!RoomSystem.callbackInstance.roomSettings.StatusEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		RoomSystem.StatusEffects statusEffects = (RoomSystem.StatusEffects)Convert.ToInt32(data[0]);
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action.Invoke(statusEffects);
	}

	// Token: 0x06004BAF RID: 19375 RVA: 0x0018B808 File Offset: 0x00189A08
	internal static void SendStatusEffectAll(RoomSystem.StatusEffects status)
	{
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action != null)
		{
			action.Invoke(status);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.statusSendData[0] = (int)status;
		byte b = 2;
		object obj = RoomSystem.statusSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004BB0 RID: 19376 RVA: 0x0018B854 File Offset: 0x00189A54
	internal static void SendStatusEffectToPlayer(RoomSystem.StatusEffects status, NetPlayer target)
	{
		if (!target.IsLocal)
		{
			RoomSystem.statusSendData[0] = (int)status;
			byte b = 2;
			object obj = RoomSystem.statusSendData;
			RoomSystem.SendEvent(b, obj, target, false);
			return;
		}
		Action<RoomSystem.StatusEffects> action = RoomSystem.statusEffectCallback;
		if (action == null)
		{
			return;
		}
		action.Invoke(status);
	}

	// Token: 0x06004BB1 RID: 19377 RVA: 0x0018B89B File Offset: 0x00189A9B
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		VRRigCache.Instance.localRig.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
	}

	// Token: 0x06004BB2 RID: 19378 RVA: 0x0018B8B4 File Offset: 0x00189AB4
	internal static void PlaySoundEffect(int soundIndex, float soundVolume, bool stopCurrentAudio, NetPlayer target)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(target, out rigContainer))
		{
			rigContainer.Rig.PlayTagSoundLocal(soundIndex, soundVolume, stopCurrentAudio);
		}
	}

	// Token: 0x06004BB3 RID: 19379 RVA: 0x0018B8DE File Offset: 0x00189ADE
	private static void OnPlaySoundEffect(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		if (target.IsLocal)
		{
			RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio);
			return;
		}
		RoomSystem.PlaySoundEffect(sound.id, sound.volume, sound.stopCurrentAudio, target);
	}

	// Token: 0x06004BB4 RID: 19380 RVA: 0x0018B918 File Offset: 0x00189B18
	private static void DeserializeSoundEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		GorillaNot.IncrementRPCCall(info, "DeserializeSoundEffect");
		if (!player.Equals(NetworkSystem.Instance.MasterClient))
		{
			GorillaNot.instance.SendReport("invalid sound effect", player.UserId, player.NickName);
			return;
		}
		RoomSystem.SoundEffect soundEffect;
		soundEffect.id = Convert.ToInt32(data[0]);
		soundEffect.volume = Convert.ToSingle(data[1]);
		soundEffect.stopCurrentAudio = Convert.ToBoolean(data[2]);
		if (!float.IsFinite(soundEffect.volume))
		{
			return;
		}
		NetPlayer netPlayer;
		if (data.Length > 3)
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectOtherLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			int playerID = Convert.ToInt32(data[3]);
			netPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		}
		else
		{
			if (!RoomSystem.callbackInstance.roomSettings.SoundEffectLimiter.CheckCallServerTime(info.SentServerTime))
			{
				return;
			}
			netPlayer = NetworkSystem.Instance.LocalPlayer;
		}
		RoomSystem.soundEffectCallback.Invoke(soundEffect, netPlayer);
	}

	// Token: 0x06004BB5 RID: 19381 RVA: 0x0018BA1E File Offset: 0x00189C1E
	internal static void SendSoundEffectAll(int soundIndex, float soundVolume, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectAll(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio));
	}

	// Token: 0x06004BB6 RID: 19382 RVA: 0x0018BA30 File Offset: 0x00189C30
	internal static void SendSoundEffectAll(RoomSystem.SoundEffect sound)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action.Invoke(sound, NetworkSystem.Instance.LocalPlayer);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.soundSendData[0] = sound.id;
		RoomSystem.soundSendData[1] = sound.volume;
		RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
		byte b = 3;
		object obj = RoomSystem.soundSendData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004BB7 RID: 19383 RVA: 0x0018BAAD File Offset: 0x00189CAD
	internal static void SendSoundEffectToPlayer(int soundIndex, float soundVolume, NetPlayer player, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectToPlayer(new RoomSystem.SoundEffect(soundIndex, soundVolume, stopCurrentAudio), player);
	}

	// Token: 0x06004BB8 RID: 19384 RVA: 0x0018BAC0 File Offset: 0x00189CC0
	internal static void SendSoundEffectToPlayer(RoomSystem.SoundEffect sound, NetPlayer player)
	{
		if (player.IsLocal)
		{
			Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
			if (action == null)
			{
				return;
			}
			action.Invoke(sound, player);
			return;
		}
		else
		{
			if (!RoomSystem.joinedRoom)
			{
				return;
			}
			RoomSystem.soundSendData[0] = sound.id;
			RoomSystem.soundSendData[1] = sound.volume;
			RoomSystem.soundSendData[2] = sound.stopCurrentAudio;
			byte b = 3;
			object obj = RoomSystem.soundSendData;
			RoomSystem.SendEvent(b, obj, player, false);
			return;
		}
	}

	// Token: 0x06004BB9 RID: 19385 RVA: 0x0018BB39 File Offset: 0x00189D39
	internal static void SendSoundEffectOnOther(int soundIndex, float soundvolume, NetPlayer target, bool stopCurrentAudio = false)
	{
		RoomSystem.SendSoundEffectOnOther(new RoomSystem.SoundEffect(soundIndex, soundvolume, stopCurrentAudio), target);
	}

	// Token: 0x06004BBA RID: 19386 RVA: 0x0018BB4C File Offset: 0x00189D4C
	internal static void SendSoundEffectOnOther(RoomSystem.SoundEffect sound, NetPlayer target)
	{
		Action<RoomSystem.SoundEffect, NetPlayer> action = RoomSystem.soundEffectCallback;
		if (action != null)
		{
			action.Invoke(sound, target);
		}
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.sendSoundDataOther[0] = sound.id;
		RoomSystem.sendSoundDataOther[1] = sound.volume;
		RoomSystem.sendSoundDataOther[2] = sound.stopCurrentAudio;
		RoomSystem.sendSoundDataOther[3] = target.ActorNumber;
		byte b = 3;
		object obj = RoomSystem.sendSoundDataOther;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x06004BBB RID: 19387 RVA: 0x0018BBD4 File Offset: 0x00189DD4
	internal static void OnPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		RoomSystem.PlayerEffectConfig playerEffectConfig;
		RigContainer rigContainer;
		if (RoomSystem.playerEffectDictionary.TryGetValue(effect, ref playerEffectConfig) && VRRigCache.Instance.TryGetVrrig(target, out rigContainer) && rigContainer != null && rigContainer.Rig != null && playerEffectConfig.tagEffectPack != null)
		{
			TagEffectsLibrary.PlayEffect(rigContainer.Rig.transform, false, rigContainer.Rig.scaleFactor, target.IsLocal ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON, playerEffectConfig.tagEffectPack, playerEffectConfig.tagEffectPack, rigContainer.Rig.transform.rotation);
		}
	}

	// Token: 0x06004BBC RID: 19388 RVA: 0x0018BC6C File Offset: 0x00189E6C
	private static void DeserializePlayerEffect(object[] data, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "DeserializePlayerEffect");
		if (!RoomSystem.callbackInstance.roomSettings.PlayerEffectLimiter.CheckCallServerTime(info.SentServerTime))
		{
			return;
		}
		int playerID = Convert.ToInt32(data[0]);
		PlayerEffect effect = (PlayerEffect)Convert.ToInt32(data[1]);
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerID);
		RoomSystem.OnPlayerEffect(effect, player);
	}

	// Token: 0x06004BBD RID: 19389 RVA: 0x0018BCC8 File Offset: 0x00189EC8
	internal static void SendPlayerEffect(PlayerEffect effect, NetPlayer target)
	{
		RoomSystem.OnPlayerEffect(effect, target);
		if (!RoomSystem.joinedRoom)
		{
			return;
		}
		RoomSystem.playerEffectData[0] = target.ActorNumber;
		RoomSystem.playerEffectData[1] = effect;
		byte b = 6;
		object obj = RoomSystem.playerEffectData;
		RoomSystem.SendEvent(b, obj, NetworkSystemRaiseEvent.neoOthers, false);
	}

	// Token: 0x04005B83 RID: 23427
	private static RoomSystem.ImpactFxContainer impactEffect = new RoomSystem.ImpactFxContainer();

	// Token: 0x04005B84 RID: 23428
	private static RoomSystem.LaunchProjectileContainer launchProjectile = new RoomSystem.LaunchProjectileContainer();

	// Token: 0x04005B85 RID: 23429
	public static GameObject playerImpactEffectPrefab = null;

	// Token: 0x04005B86 RID: 23430
	private static readonly object[] projectileSendData = new object[9];

	// Token: 0x04005B87 RID: 23431
	private static readonly object[] impactSendData = new object[6];

	// Token: 0x04005B88 RID: 23432
	private static readonly List<int> hashValues = new List<int>(2);

	// Token: 0x04005B89 RID: 23433
	[SerializeField]
	private RoomSystemSettings roomSettings;

	// Token: 0x04005B8A RID: 23434
	[SerializeField]
	private string[] prefabsToInstantiateByPath;

	// Token: 0x04005B8B RID: 23435
	[SerializeField]
	private GameObject[] prefabsToInstantiate;

	// Token: 0x04005B8C RID: 23436
	private List<GameObject> prefabsInstantiated = new List<GameObject>();

	// Token: 0x04005B8D RID: 23437
	public static Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig> playerEffectDictionary = new Dictionary<PlayerEffect, RoomSystem.PlayerEffectConfig>();

	// Token: 0x04005B8E RID: 23438
	[OnEnterPlay_SetNull]
	private static RoomSystem callbackInstance;

	// Token: 0x04005B91 RID: 23441
	private static byte m_roomSizeOnJoin;

	// Token: 0x04005B92 RID: 23442
	[OnEnterPlay_Clear]
	private static List<NetPlayer> netPlayersInRoom = new List<NetPlayer>(10);

	// Token: 0x04005B93 RID: 23443
	[OnEnterPlay_Set("")]
	private static string roomGameMode = "";

	// Token: 0x04005B94 RID: 23444
	[OnEnterPlay_Set(false)]
	private static bool joinedRoom = false;

	// Token: 0x04005B97 RID: 23447
	[OnEnterPlay_SetNull]
	private static PhotonView[] sceneViews;

	// Token: 0x04005B98 RID: 23448
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor LeftRoomEvent = new DelegateListProcessor();

	// Token: 0x04005B99 RID: 23449
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor JoinedRoomEvent = new DelegateListProcessor();

	// Token: 0x04005B9A RID: 23450
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerJoinedEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04005B9B RID: 23451
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor<NetPlayer> PlayerLeftEvent = new DelegateListProcessor<NetPlayer>();

	// Token: 0x04005B9C RID: 23452
	[OnEnterPlay_SetNew]
	public static DelegateListProcessor PlayersChangedEvent = new DelegateListProcessor();

	// Token: 0x04005B9D RID: 23453
	private static Timer disconnectTimer = new Timer();

	// Token: 0x04005B9E RID: 23454
	[OnExitPlay_Clear]
	internal static readonly Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>> netEventCallbacks = new Dictionary<byte, Action<object[], PhotonMessageInfoWrapped>>(10);

	// Token: 0x04005B9F RID: 23455
	private static readonly object[] sendEventData = new object[3];

	// Token: 0x04005BA0 RID: 23456
	private static readonly object[] groupJoinSendData = new object[2];

	// Token: 0x04005BA1 RID: 23457
	private static readonly object[] reportTouchSendData = new object[1];

	// Token: 0x04005BA2 RID: 23458
	private static readonly object[] reportHitSendData = new object[3];

	// Token: 0x04005BA3 RID: 23459
	[OnExitPlay_SetNull]
	public static Action<NetPlayer, NetPlayer> playerTouchedCallback;

	// Token: 0x04005BA4 RID: 23460
	private static CallLimiter playerLaunchedCallLimiter = new CallLimiter(3, 15f, 0.5f);

	// Token: 0x04005BA5 RID: 23461
	private static CallLimiter hitPlayerCallLimiter = new CallLimiter(10, 2f, 0.5f);

	// Token: 0x04005BA6 RID: 23462
	private static object[] statusSendData = new object[1];

	// Token: 0x04005BA7 RID: 23463
	public static Action<RoomSystem.StatusEffects> statusEffectCallback;

	// Token: 0x04005BA8 RID: 23464
	private static object[] soundSendData = new object[3];

	// Token: 0x04005BA9 RID: 23465
	private static object[] sendSoundDataOther = new object[4];

	// Token: 0x04005BAA RID: 23466
	public static Action<RoomSystem.SoundEffect, NetPlayer> soundEffectCallback;

	// Token: 0x04005BAB RID: 23467
	private static object[] playerEffectData = new object[2];

	// Token: 0x02000BF1 RID: 3057
	private class ImpactFxContainer : IFXContext
	{
		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06004BBF RID: 19391 RVA: 0x0018BD2D File Offset: 0x00189F2D
		public FXSystemSettings settings
		{
			get
			{
				return this.targetRig.fxSettings;
			}
		}

		// Token: 0x06004BC0 RID: 19392 RVA: 0x0018BD3C File Offset: 0x00189F3C
		public virtual void OnPlayFX()
		{
			NetPlayer creator = this.targetRig.creator;
			ProjectileTracker.ProjectileInfo projectileInfo;
			if (this.targetRig.isOfflineVRRig)
			{
				projectileInfo = ProjectileTracker.GetLocalProjectile(this.projectileIndex);
			}
			else
			{
				ValueTuple<bool, ProjectileTracker.ProjectileInfo> andRemoveRemotePlayerProjectile = ProjectileTracker.GetAndRemoveRemotePlayerProjectile(creator, this.projectileIndex);
				if (!andRemoveRemotePlayerProjectile.Item1)
				{
					return;
				}
				projectileInfo = andRemoveRemotePlayerProjectile.Item2;
			}
			SlingshotProjectile projectileInstance = projectileInfo.projectileInstance;
			GameObject obj = projectileInfo.hasImpactOverride ? projectileInstance.playerImpactEffectPrefab : RoomSystem.playerImpactEffectPrefab;
			GameObject gameObject = ObjectPools.instance.Instantiate(obj, this.position, true);
			gameObject.transform.localScale = Vector3.one * this.targetRig.scaleFactor;
			GorillaColorizableBase gorillaColorizableBase;
			if (gameObject.TryGetComponent<GorillaColorizableBase>(ref gorillaColorizableBase))
			{
				gorillaColorizableBase.SetColor(this.colour);
			}
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(projectileInstance.transform.localScale.x * projectileInstance.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(projectileInstance.impactSoundVolumeOverride, projectileInstance.impactSoundPitchOverride);
			}
			if (projectileInstance.gameObject.activeSelf && projectileInstance.projectileOwner == creator)
			{
				projectileInstance.Deactivate();
			}
		}

		// Token: 0x04005BAC RID: 23468
		public VRRig targetRig;

		// Token: 0x04005BAD RID: 23469
		public Vector3 position;

		// Token: 0x04005BAE RID: 23470
		public Color colour;

		// Token: 0x04005BAF RID: 23471
		public int projectileIndex;
	}

	// Token: 0x02000BF2 RID: 3058
	private class LaunchProjectileContainer : RoomSystem.ImpactFxContainer
	{
		// Token: 0x06004BC2 RID: 19394 RVA: 0x0018BE70 File Offset: 0x0018A070
		public override void OnPlayFX()
		{
			GameObject gameObject = null;
			SlingshotProjectile slingshotProjectile = null;
			try
			{
				switch (this.projectileSource)
				{
				case RoomSystem.ProjectileSource.ProjectileWeapon:
					if (this.targetRig.projectileWeapon.IsNotNull() && this.targetRig.projectileWeapon.IsNotNull())
					{
						this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 70f, 100f);
						SlingshotProjectile slingshotProjectile2 = this.targetRig.projectileWeapon.LaunchNetworkedProjectile(this.position, this.velocity, this.projectileSource, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour, this.messageInfo);
						if (slingshotProjectile2.IsNotNull())
						{
							ProjectileTracker.AddRemotePlayerProjectile(this.messageInfo.Sender, slingshotProjectile2, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
						}
					}
					return;
				case RoomSystem.ProjectileSource.LeftHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetLeftHandThrowable();
					break;
				case RoomSystem.ProjectileSource.RightHand:
					this.tempThrowableGO = this.targetRig.myBodyDockPositions.GetRightHandThrowable();
					break;
				default:
					return;
				}
				if (!this.tempThrowableGO.IsNull() && this.tempThrowableGO.TryGetComponent<SnowballThrowable>(ref this.tempThrowableRef) && !(this.tempThrowableRef is GrowingSnowballThrowable))
				{
					this.velocity = this.targetRig.ClampVelocityRelativeToPlayerSafe(this.velocity, 50f, 100f);
					int projectileHash = this.tempThrowableRef.ProjectileHash;
					gameObject = ObjectPools.instance.Instantiate(projectileHash, true);
					slingshotProjectile = gameObject.GetComponent<SlingshotProjectile>();
					ProjectileTracker.AddRemotePlayerProjectile(this.targetRig.creator, slingshotProjectile, this.projectileIndex, this.messageInfo.SentServerTime, this.velocity, this.position, this.targetRig.scaleFactor);
					slingshotProjectile.Launch(this.position, this.velocity, this.messageInfo.Sender, false, false, this.projectileIndex, this.targetRig.scaleFactor, this.overridecolour, this.colour);
				}
			}
			catch
			{
				GorillaNot.instance.SendReport("throwable error", this.messageInfo.Sender.UserId, this.messageInfo.Sender.NickName);
				if (slingshotProjectile != null && slingshotProjectile)
				{
					slingshotProjectile.transform.position = Vector3.zero;
					slingshotProjectile.Deactivate();
				}
				else if (gameObject.IsNotNull())
				{
					ObjectPools.instance.Destroy(gameObject);
				}
			}
		}

		// Token: 0x04005BB0 RID: 23472
		public Vector3 velocity;

		// Token: 0x04005BB1 RID: 23473
		public RoomSystem.ProjectileSource projectileSource;

		// Token: 0x04005BB2 RID: 23474
		public bool overridecolour;

		// Token: 0x04005BB3 RID: 23475
		public PhotonMessageInfoWrapped messageInfo;

		// Token: 0x04005BB4 RID: 23476
		private GameObject tempThrowableGO;

		// Token: 0x04005BB5 RID: 23477
		private SnowballThrowable tempThrowableRef;
	}

	// Token: 0x02000BF3 RID: 3059
	internal enum ProjectileSource
	{
		// Token: 0x04005BB7 RID: 23479
		ProjectileWeapon,
		// Token: 0x04005BB8 RID: 23480
		LeftHand,
		// Token: 0x04005BB9 RID: 23481
		RightHand
	}

	// Token: 0x02000BF4 RID: 3060
	private struct Events
	{
		// Token: 0x04005BBA RID: 23482
		public const byte PROJECTILE = 0;

		// Token: 0x04005BBB RID: 23483
		public const byte IMPACT = 1;

		// Token: 0x04005BBC RID: 23484
		public const byte STATUS_EFFECT = 2;

		// Token: 0x04005BBD RID: 23485
		public const byte SOUND_EFFECT = 3;

		// Token: 0x04005BBE RID: 23486
		public const byte NEARBY_JOIN = 4;

		// Token: 0x04005BBF RID: 23487
		public const byte PLAYER_TOUCHED = 5;

		// Token: 0x04005BC0 RID: 23488
		public const byte PLAYER_EFFECT = 6;

		// Token: 0x04005BC1 RID: 23489
		public const byte PARTY_JOIN = 7;

		// Token: 0x04005BC2 RID: 23490
		public const byte PLAYER_LAUNCHED = 8;

		// Token: 0x04005BC3 RID: 23491
		public const byte PLAYER_HIT = 9;

		// Token: 0x04005BC4 RID: 23492
		public const byte ELEVATOR_JOIN = 10;

		// Token: 0x04005BC5 RID: 23493
		public const byte SHUTTLE_JOIN = 11;
	}

	// Token: 0x02000BF5 RID: 3061
	public enum StatusEffects
	{
		// Token: 0x04005BC7 RID: 23495
		TaggedTime,
		// Token: 0x04005BC8 RID: 23496
		JoinedTaggedTime,
		// Token: 0x04005BC9 RID: 23497
		SetSlowedTime,
		// Token: 0x04005BCA RID: 23498
		UnTagged,
		// Token: 0x04005BCB RID: 23499
		FrozenTime
	}

	// Token: 0x02000BF6 RID: 3062
	public struct SoundEffect
	{
		// Token: 0x06004BC4 RID: 19396 RVA: 0x0018C128 File Offset: 0x0018A328
		public SoundEffect(int soundID, float soundVolume, bool _stopCurrentAudio)
		{
			this.id = soundID;
			this.volume = soundVolume;
			this.volume = soundVolume;
			this.stopCurrentAudio = _stopCurrentAudio;
		}

		// Token: 0x04005BCC RID: 23500
		public int id;

		// Token: 0x04005BCD RID: 23501
		public float volume;

		// Token: 0x04005BCE RID: 23502
		public bool stopCurrentAudio;
	}

	// Token: 0x02000BF7 RID: 3063
	[Serializable]
	public struct PlayerEffectConfig
	{
		// Token: 0x04005BCF RID: 23503
		public PlayerEffect type;

		// Token: 0x04005BD0 RID: 23504
		public TagEffectPack tagEffectPack;
	}
}
