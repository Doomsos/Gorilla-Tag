using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200077C RID: 1916
public abstract class GorillaGameManager : MonoBehaviourPunCallbacks, ITickSystemTick, IWrappedSerializable, INetworkStruct
{
	// Token: 0x060031E2 RID: 12770 RVA: 0x0010E4A6 File Offset: 0x0010C6A6
	public static string GameModeEnumToName(GameModeType gameMode)
	{
		return gameMode.ToString();
	}

	// Token: 0x14000057 RID: 87
	// (add) Token: 0x060031E3 RID: 12771 RVA: 0x0010E4B8 File Offset: 0x0010C6B8
	// (remove) Token: 0x060031E4 RID: 12772 RVA: 0x0010E4EC File Offset: 0x0010C6EC
	public static event GorillaGameManager.OnTouchDelegate OnTouch;

	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x060031E5 RID: 12773 RVA: 0x0010E51F File Offset: 0x0010C71F
	public static GorillaGameManager instance
	{
		get
		{
			return GameMode.ActiveGameMode;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x060031E6 RID: 12774 RVA: 0x0010E526 File Offset: 0x0010C726
	// (set) Token: 0x060031E7 RID: 12775 RVA: 0x0010E52E File Offset: 0x0010C72E
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060031E8 RID: 12776 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Awake()
	{
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDisable()
	{
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x0010E538 File Offset: 0x0010C738
	public virtual void Tick()
	{
		if (this.lastCheck + this.checkCooldown < Time.time)
		{
			this.lastCheck = Time.time;
			if (NetworkSystem.Instance.IsMasterClient && !this.ValidGameMode())
			{
				GameMode.ChangeGameFromProperty();
				return;
			}
			this.InfrequentUpdate();
		}
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x0010E585 File Offset: 0x0010C785
	public virtual void InfrequentUpdate()
	{
		GameMode.RefreshPlayers();
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x0010E59C File Offset: 0x0010C79C
	public virtual string GameModeName()
	{
		if (this._gameModeName == null)
		{
			this._gameModeName = this.GameType().ToString().ToUpper();
		}
		return this._gameModeName;
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x0010E5D8 File Offset: 0x0010C7D8
	public virtual string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_NONE_ROOM_LABEL", out result, "(NONE GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_NONE_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HitPlayer(NetPlayer player)
	{
	}

	// Token: 0x060031F2 RID: 12786 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return false;
	}

	// Token: 0x060031F3 RID: 12787 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HandleHandTap(NetPlayer tappingPlayer, Tappable hitTappable, bool leftHand, Vector3 handVelocity, Vector3 tapSurfaceNormal)
	{
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanJoinFrienship(NetPlayer player)
	{
		return true;
	}

	// Token: 0x060031F5 RID: 12789 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanPlayerParticipate(NetPlayer player)
	{
		return true;
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x0010E603 File Offset: 0x0010C803
	public virtual void HandleRoundComplete()
	{
		PlayerGameEvents.GameModeCompleteRound();
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer, double tagTime)
	{
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool LocalIsTagged(NetPlayer player)
	{
		return false;
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x0010E60C File Offset: 0x0010C80C
	public virtual VRRig FindPlayerVRRig(NetPlayer player)
	{
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x0010E634 File Offset: 0x0010C834
	public static VRRig StaticFindRigForPlayer(NetPlayer player)
	{
		VRRig result = null;
		RigContainer rigContainer;
		if (GorillaGameManager.instance != null)
		{
			result = GorillaGameManager.instance.FindPlayerVRRig(player);
		}
		else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			result = rigContainer.Rig;
		}
		return result;
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x0010E675 File Offset: 0x0010C875
	public virtual float[] LocalPlayerSpeed()
	{
		this.playerSpeed[0] = this.slowJumpLimit;
		this.playerSpeed[1] = this.slowJumpMultiplier;
		return this.playerSpeed;
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x0010E69C File Offset: 0x0010C89C
	public virtual void UpdatePlayerAppearance(VRRig rig)
	{
		ScienceExperimentManager instance = ScienceExperimentManager.instance;
		int materialIndex;
		if (instance != null && instance.GetMaterialIfPlayerInGame(rig.creator.ActorNumber, out materialIndex))
		{
			rig.ChangeMaterialLocal(materialIndex);
			return;
		}
		int materialIndex2 = this.MyMatIndex(rig.creator);
		rig.ChangeMaterialLocal(materialIndex2);
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x00002076 File Offset: 0x00000276
	public virtual int MyMatIndex(NetPlayer forPlayer)
	{
		return 0;
	}

	// Token: 0x06003201 RID: 12801 RVA: 0x000F2F19 File Offset: 0x000F1119
	public virtual int SpecialHandFX(NetPlayer player, RigContainer rigContainer)
	{
		return -1;
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x0010E6EB File Offset: 0x0010C8EB
	public virtual bool ValidGameMode()
	{
		return NetworkSystem.Instance.InRoom && ((NetworkSystem.Instance.SessionIsPrivate && RoomSystem.IsVStumpRoom) || NetworkSystem.Instance.GameModeString.Contains(this.GameTypeName()));
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x0010E728 File Offset: 0x0010C928
	public static void OnInstanceReady(Action action)
	{
		GorillaParent.OnReplicatedClientReady(delegate
		{
			if (GorillaGameManager.instance)
			{
				action.Invoke();
				return;
			}
			GorillaGameManager.onInstanceReady = (Action)Delegate.Combine(GorillaGameManager.onInstanceReady, action);
		});
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x0010E746 File Offset: 0x0010C946
	public static void ReplicatedClientReady()
	{
		GorillaGameManager.replicatedClientReady = true;
	}

	// Token: 0x06003205 RID: 12805 RVA: 0x0010E74E File Offset: 0x0010C94E
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaGameManager.replicatedClientReady)
		{
			action.Invoke();
			return;
		}
		GorillaGameManager.onReplicatedClientReady = (Action)Delegate.Combine(GorillaGameManager.onReplicatedClientReady, action);
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06003206 RID: 12806 RVA: 0x0010E773 File Offset: 0x0010C973
	internal GameModeSerializer Serializer
	{
		get
		{
			return this.serializer;
		}
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x0010E77B File Offset: 0x0010C97B
	internal virtual void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		this.serializer = netSerializer;
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x0010E784 File Offset: 0x0010C984
	internal virtual void NetworkLinkDestroyed(GameModeSerializer netSerializer)
	{
		if (this.serializer == netSerializer)
		{
			this.serializer = null;
		}
	}

	// Token: 0x06003209 RID: 12809
	public abstract GameModeType GameType();

	// Token: 0x0600320A RID: 12810 RVA: 0x0010E79C File Offset: 0x0010C99C
	public string GameTypeName()
	{
		return this.GameType().ToString();
	}

	// Token: 0x0600320B RID: 12811
	public abstract void AddFusionDataBehaviour(NetworkObject behaviour);

	// Token: 0x0600320C RID: 12812
	public abstract void OnSerializeRead(object newData);

	// Token: 0x0600320D RID: 12813
	public abstract object OnSerializeWrite();

	// Token: 0x0600320E RID: 12814
	public abstract void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x0600320F RID: 12815
	public abstract void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003210 RID: 12816 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ResetGame()
	{
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x0010E7C0 File Offset: 0x0010C9C0
	public virtual void StartPlaying()
	{
		TickSystem<object>.AddTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += new Action<NetPlayer>(this.OnMasterClientSwitched);
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		GorillaTelemetry.PostGameModeEvent(GTGameModeEventType.game_mode_start, this.GameType());
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x0010E858 File Offset: 0x0010CA58
	public virtual void StopPlaying()
	{
		TickSystem<object>.RemoveTickCallback(this);
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= new Action<NetPlayer>(this.OnMasterClientSwitched);
		this.lastCheck = 0f;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnMasterClientSwitched(Player newMaster)
	{
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x0010E8DC File Offset: 0x0010CADC
	public virtual void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
		if (this.lastTaggedActorNr.ContainsKey(otherPlayer.ActorNumber))
		{
			this.lastTaggedActorNr.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x0010E913 File Offset: 0x0010CB13
	public virtual void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		this.currentNetPlayerArray = NetworkSystem.Instance.AllNetPlayers;
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnMasterClientSwitched(NetPlayer newMaster)
	{
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x0010E928 File Offset: 0x0010CB28
	internal static void ForceStopGame_DisconnectAndDestroy()
	{
		Application.Quit();
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance != null)
		{
			instance.ReturnToSinglePlayer();
		}
		Object.DestroyImmediate(PhotonNetworkController.Instance);
		Object.DestroyImmediate(GTPlayer.Instance);
		GameObject[] array = Object.FindObjectsByType<GameObject>(0);
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i]);
		}
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x0010E980 File Offset: 0x0010CB80
	public void AddLastTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (this.lastTaggedActorNr.ContainsKey(taggedPlayer.ActorNumber))
		{
			this.lastTaggedActorNr[taggedPlayer.ActorNumber] = taggingPlayer.ActorNumber;
			return;
		}
		this.lastTaggedActorNr.Add(taggedPlayer.ActorNumber, taggingPlayer.ActorNumber);
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x0010E9D0 File Offset: 0x0010CBD0
	public void WriteLastTagged(PhotonStream stream)
	{
		stream.SendNext(this.lastTaggedActorNr.Count);
		foreach (KeyValuePair<int, int> keyValuePair in this.lastTaggedActorNr)
		{
			stream.SendNext(keyValuePair.Key);
			stream.SendNext(keyValuePair.Value);
		}
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x0010EA58 File Offset: 0x0010CC58
	public void ReadLastTagged(PhotonStream stream)
	{
		this.lastTaggedActorNr.Clear();
		int num = Mathf.Min((int)stream.ReceiveNext(), 10);
		for (int i = 0; i < num; i++)
		{
			this.lastTaggedActorNr.Add((int)stream.ReceiveNext(), (int)stream.ReceiveNext());
		}
	}

	// Token: 0x04004077 RID: 16503
	protected const string GAME_MODE_NONE_KEY = "GAME_MODE_NONE";

	// Token: 0x04004078 RID: 16504
	protected const string GAME_MODE_CASUAL_ROOM_LABEL_KEY = "GAME_MODE_CASUAL_ROOM_LABEL";

	// Token: 0x04004079 RID: 16505
	protected const string GAME_MODE_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_INFECTION_ROOM_LABEL";

	// Token: 0x0400407A RID: 16506
	protected const string GAME_MODE_HUNT_ROOM_LABEL_KEY = "GAME_MODE_HUNT_ROOM_LABEL";

	// Token: 0x0400407B RID: 16507
	protected const string GAME_MODE_PAINTBRAWL_ROOM_LABEL_KEY = "GAME_MODE_PAINTBRAWL_ROOM_LABEL";

	// Token: 0x0400407C RID: 16508
	protected const string GAME_MODE_SUPER_INFECTION_ROOM_LABEL_KEY = "GAME_MODE_SUPER_INFECTION_ROOM_LABEL";

	// Token: 0x0400407D RID: 16509
	protected const string GAME_MODE_NONE_ROOM_LABEL_KEY = "GAME_MODE_NONE_ROOM_LABEL";

	// Token: 0x0400407E RID: 16510
	protected const string GAME_MODE_CUSTOM_ROOM_LABEL_KEY = "GAME_MODE_CUSTOM_ROOM_LABEL";

	// Token: 0x0400407F RID: 16511
	protected const string GAME_MODE_GHOST_ROOM_LABEL_KEY = "GAME_MODE_GHOST_ROOM_LABEL";

	// Token: 0x04004080 RID: 16512
	protected const string GAME_MODE_AMBUSH_ROOM_LABEL_KEY = "GAME_MODE_AMBUSH_ROOM_LABEL";

	// Token: 0x04004081 RID: 16513
	protected const string GAME_MODE_FREEZE_TAG_ROOM_LABEL_KEY = "GAME_MODE_FREEZE_TAG_ROOM_LABEL";

	// Token: 0x04004082 RID: 16514
	protected const string GAME_MODE_GUARDIAN_ROOM_LABEL_KEY = "GAME_MODE_GUARDIAN_ROOM_LABEL";

	// Token: 0x04004083 RID: 16515
	protected const string GAME_MODE_PROP_HUNT_ROOM_LABEL_KEY = "GAME_MODE_PROP_HUNT_ROOM_LABEL";

	// Token: 0x04004084 RID: 16516
	protected const string GAME_MODE_COMP_INF_ROOM_LABEL_KEY = "GAME_MODE_COMP_INF_ROOM_LABEL";

	// Token: 0x04004085 RID: 16517
	public const int k_defaultMatIndex = 0;

	// Token: 0x04004087 RID: 16519
	public float fastJumpLimit;

	// Token: 0x04004088 RID: 16520
	public float fastJumpMultiplier;

	// Token: 0x04004089 RID: 16521
	public float slowJumpLimit;

	// Token: 0x0400408A RID: 16522
	public float slowJumpMultiplier;

	// Token: 0x0400408B RID: 16523
	public float lastCheck;

	// Token: 0x0400408C RID: 16524
	public float checkCooldown = 3f;

	// Token: 0x0400408D RID: 16525
	public float tagDistanceThreshold = 4f;

	// Token: 0x0400408E RID: 16526
	private NetPlayer outPlayer;

	// Token: 0x0400408F RID: 16527
	private int outInt;

	// Token: 0x04004090 RID: 16528
	private VRRig tempRig;

	// Token: 0x04004091 RID: 16529
	public NetPlayer[] currentNetPlayerArray;

	// Token: 0x04004092 RID: 16530
	public float[] playerSpeed = new float[2];

	// Token: 0x04004093 RID: 16531
	public Dictionary<int, int> lastTaggedActorNr = new Dictionary<int, int>();

	// Token: 0x04004095 RID: 16533
	private string _gameModeName;

	// Token: 0x04004096 RID: 16534
	private static Action onInstanceReady;

	// Token: 0x04004097 RID: 16535
	private static bool replicatedClientReady;

	// Token: 0x04004098 RID: 16536
	private static Action onReplicatedClientReady;

	// Token: 0x04004099 RID: 16537
	private GameModeSerializer serializer;

	// Token: 0x0200077D RID: 1917
	// (Invoke) Token: 0x0600321F RID: 12831
	public delegate void OnTouchDelegate(NetPlayer taggedPlayer, NetPlayer taggingPlayer);
}
