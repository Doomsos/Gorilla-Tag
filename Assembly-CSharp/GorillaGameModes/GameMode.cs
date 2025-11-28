using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000D89 RID: 3465
	public class GameMode : MonoBehaviour
	{
		// Token: 0x060054E7 RID: 21735 RVA: 0x001ABD34 File Offset: 0x001A9F34
		private void Awake()
		{
			if (GameMode.instance.IsNull())
			{
				GameMode.instance = this;
				foreach (GorillaGameManager gorillaGameManager in base.gameObject.GetComponentsInChildren<GorillaGameManager>(true))
				{
					int num = (int)gorillaGameManager.GameType();
					string text = gorillaGameManager.GameTypeName();
					if (GameMode.gameModeTable.ContainsKey(num))
					{
						Debug.LogWarning("Duplicate gamemode type, skipping this instance", gorillaGameManager);
					}
					else
					{
						GameMode.gameModeTable.Add((int)gorillaGameManager.GameType(), gorillaGameManager);
						GameMode.gameModeKeyByName.Add(text, num);
						GameMode.gameModes.Add(gorillaGameManager);
						GameMode.gameModeNames.Add(text);
					}
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x060054E8 RID: 21736 RVA: 0x001ABDD9 File Offset: 0x001A9FD9
		private void OnDestroy()
		{
			if (GameMode.instance == this)
			{
				GameMode.instance = null;
			}
		}

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x060054E9 RID: 21737 RVA: 0x001ABDF0 File Offset: 0x001A9FF0
		// (remove) Token: 0x060054EA RID: 21738 RVA: 0x001ABE24 File Offset: 0x001AA024
		public static event GameMode.OnStartGameModeAction OnStartGameMode;

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060054EB RID: 21739 RVA: 0x001ABE57 File Offset: 0x001AA057
		public static GorillaGameManager ActiveGameMode
		{
			get
			{
				return GameMode.activeGameMode;
			}
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060054EC RID: 21740 RVA: 0x001ABE5E File Offset: 0x001AA05E
		internal static GameModeSerializer ActiveNetworkHandler
		{
			get
			{
				return GameMode.activeNetworkHandler;
			}
		}

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060054ED RID: 21741 RVA: 0x001ABE65 File Offset: 0x001AA065
		public static GameModeZoneMapping GameModeZoneMapping
		{
			get
			{
				return GameMode.instance.gameModeZoneMapping;
			}
		}

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060054EE RID: 21742 RVA: 0x001ABE71 File Offset: 0x001AA071
		// (set) Token: 0x060054EF RID: 21743 RVA: 0x001ABE78 File Offset: 0x001AA078
		public static GameModeType CurrentGameModeType { get; private set; } = GameModeType.None;

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x060054F0 RID: 21744 RVA: 0x001ABE80 File Offset: 0x001AA080
		// (remove) Token: 0x060054F1 RID: 21745 RVA: 0x001ABEB4 File Offset: 0x001AA0B4
		public static event Action<List<NetPlayer>, List<NetPlayer>> ParticipatingPlayersChanged;

		// Token: 0x060054F2 RID: 21746 RVA: 0x001ABEE8 File Offset: 0x001AA0E8
		static GameMode()
		{
			GameMode.StaticLoad();
		}

		// Token: 0x060054F3 RID: 21747 RVA: 0x001ABF8C File Offset: 0x001AA18C
		[OnEnterPlay_Run]
		private static void StaticLoad()
		{
			RoomSystem.LeftRoomEvent += new Action(GameMode.ResetGameModes);
			RoomSystem.JoinedRoomEvent += new Action(GameMode.RefreshPlayers);
			RoomSystem.PlayersChangedEvent += new Action(GameMode.RefreshPlayers);
		}

		// Token: 0x060054F4 RID: 21748 RVA: 0x001ABFEA File Offset: 0x001AA1EA
		public static bool IsPlaying(GameModeType type)
		{
			return type == GameMode.CurrentGameModeType;
		}

		// Token: 0x060054F5 RID: 21749 RVA: 0x001ABFF4 File Offset: 0x001AA1F4
		internal static bool LoadGameModeFromProperty()
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x060054F6 RID: 21750 RVA: 0x001AC000 File Offset: 0x001AA200
		internal static bool ChangeGameFromProperty()
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeFromRoomProperty());
		}

		// Token: 0x060054F7 RID: 21751 RVA: 0x001AC00C File Offset: 0x001AA20C
		internal static bool LoadGameModeFromProperty(string prop)
		{
			return GameMode.LoadGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x060054F8 RID: 21752 RVA: 0x001AC019 File Offset: 0x001AA219
		internal static bool ChangeGameFromProperty(string prop)
		{
			return GameMode.ChangeGameMode(GameMode.FindGameModeInString(prop));
		}

		// Token: 0x060054F9 RID: 21753 RVA: 0x001AC028 File Offset: 0x001AA228
		public static int GetGameModeKeyFromRoomProp()
		{
			string text = GameMode.FindGameModeFromRoomProperty();
			int result;
			if (string.IsNullOrEmpty(text) || !GameMode.gameModeKeyByName.TryGetValue(text, ref result))
			{
				GTDev.LogWarning<string>("Unable to find game mode key for " + text, null);
				return -1;
			}
			return result;
		}

		// Token: 0x060054FA RID: 21754 RVA: 0x001AC066 File Offset: 0x001AA266
		private static string FindGameModeFromRoomProperty()
		{
			if (!NetworkSystem.Instance.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.GameModeString))
			{
				return null;
			}
			return GameMode.FindGameModeInString(NetworkSystem.Instance.GameModeString);
		}

		// Token: 0x060054FB RID: 21755 RVA: 0x001AC096 File Offset: 0x001AA296
		public static bool IsValidGameMode(string gameMode)
		{
			return !string.IsNullOrEmpty(gameMode) && GameMode.gameModeKeyByName.ContainsKey(gameMode);
		}

		// Token: 0x060054FC RID: 21756 RVA: 0x001AC0B0 File Offset: 0x001AA2B0
		private static string FindGameModeInString(string gmString)
		{
			for (int i = 0; i < GameMode.gameModes.Count; i++)
			{
				string text = GameMode.gameModes[i].GameTypeName();
				if (gmString.EndsWith(text))
				{
					return text;
				}
			}
			return null;
		}

		// Token: 0x060054FD RID: 21757 RVA: 0x001AC0F0 File Offset: 0x001AA2F0
		public static bool LoadGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				Debug.LogError("GAME MODE NULL");
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, ref key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x060054FE RID: 21758 RVA: 0x001AC134 File Offset: 0x001AA334
		public static bool LoadGameMode(int key)
		{
			foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
			{
			}
			if (!GameMode.gameModeTable.ContainsKey(key))
			{
				Debug.LogWarning("Missing game mode for key " + key.ToString());
				return false;
			}
			PrefabType prefabType;
			VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("GameMode", ref prefabType);
			GameObject prefab = prefabType.prefab;
			if (prefab == null)
			{
				GTDev.LogError<string>("Unable to find game mode prefab to spawn", null);
				return false;
			}
			if (NetworkSystem.Instance.NetInstantiate(prefab, Vector3.zero, Quaternion.identity, true, 0, new object[]
			{
				key
			}, delegate(NetworkRunner runner, NetworkObject no)
			{
				no.GetComponent<GameModeSerializer>().Init(key);
			}).IsNull())
			{
				GTDev.LogWarning<string>("Unable to create GameManager with key " + key.ToString(), null);
				return false;
			}
			return true;
		}

		// Token: 0x060054FF RID: 21759 RVA: 0x001AC250 File Offset: 0x001AA450
		internal static bool ChangeGameMode(string gameMode)
		{
			if (gameMode == null)
			{
				return false;
			}
			int key;
			if (!GameMode.gameModeKeyByName.TryGetValue(gameMode, ref key))
			{
				Debug.LogWarning("Unable to find game mode key for " + gameMode);
				return false;
			}
			return GameMode.ChangeGameMode(key);
		}

		// Token: 0x06005500 RID: 21760 RVA: 0x001AC28C File Offset: 0x001AA48C
		internal static bool ChangeGameMode(int key)
		{
			GorillaGameManager gorillaGameManager;
			if (!NetworkSystem.Instance.IsMasterClient || !GameMode.gameModeTable.TryGetValue(key, ref gorillaGameManager) || gorillaGameManager == GameMode.activeGameMode)
			{
				return false;
			}
			if (GameMode.activeNetworkHandler.IsNotNull())
			{
				NetworkSystem.Instance.NetDestroy(GameMode.activeNetworkHandler.gameObject);
			}
			GameMode.StopGameModeSafe(GameMode.activeGameMode);
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			return GameMode.LoadGameMode(key);
		}

		// Token: 0x06005501 RID: 21761 RVA: 0x001AC300 File Offset: 0x001AA500
		internal static void SetupGameModeRemote(GameModeSerializer networkSerializer)
		{
			GorillaGameManager gameModeInstance = networkSerializer.GameModeInstance;
			bool flag = gameModeInstance != GameMode.activeGameMode;
			if (GameMode.activeGameMode.IsNotNull() && gameModeInstance.IsNotNull() && flag)
			{
				GameMode.StopGameModeSafe(GameMode.activeGameMode);
			}
			GameMode.activeNetworkHandler = networkSerializer;
			GameMode.activeGameMode = gameModeInstance;
			GameMode.activeGameMode.NetworkLinkSetup(networkSerializer);
			GameMode.CurrentGameModeType = GameMode.activeGameMode.GameType();
			if (!GameMode.activatedGameModes.Contains(GameMode.activeGameMode))
			{
				GameMode.activatedGameModes.Add(GameMode.activeGameMode);
			}
			if (flag)
			{
				GameMode.StartGameModeSafe(GameMode.activeGameMode);
				if (GameMode.OnStartGameMode != null)
				{
					GameMode.OnStartGameMode(GameMode.activeGameMode.GameType());
				}
			}
		}

		// Token: 0x06005502 RID: 21762 RVA: 0x001AC3B1 File Offset: 0x001AA5B1
		internal static void RemoveNetworkLink(GameModeSerializer networkSerializer)
		{
			if (GameMode.activeGameMode.IsNotNull() && networkSerializer == GameMode.activeNetworkHandler)
			{
				GameMode.activeGameMode.NetworkLinkDestroyed(networkSerializer);
				GameMode.activeNetworkHandler = null;
				return;
			}
		}

		// Token: 0x06005503 RID: 21763 RVA: 0x001AC3DE File Offset: 0x001AA5DE
		public static GorillaGameManager GetGameModeInstance(GameModeType type)
		{
			return GameMode.GetGameModeInstance((int)type);
		}

		// Token: 0x06005504 RID: 21764 RVA: 0x001AC3E8 File Offset: 0x001AA5E8
		public static GorillaGameManager GetGameModeInstance(int type)
		{
			GorillaGameManager gorillaGameManager;
			if (GameMode.gameModeTable.TryGetValue(type, ref gorillaGameManager))
			{
				if (gorillaGameManager == null)
				{
					Debug.LogError("Couldnt get mode from table");
					foreach (KeyValuePair<int, GorillaGameManager> keyValuePair in GameMode.gameModeTable)
					{
					}
				}
				return gorillaGameManager;
			}
			return null;
		}

		// Token: 0x06005505 RID: 21765 RVA: 0x001AC458 File Offset: 0x001AA658
		public static T GetGameModeInstance<T>(GameModeType type) where T : GorillaGameManager
		{
			return GameMode.GetGameModeInstance<T>((int)type);
		}

		// Token: 0x06005506 RID: 21766 RVA: 0x001AC460 File Offset: 0x001AA660
		public static T GetGameModeInstance<T>(int type) where T : GorillaGameManager
		{
			T t = GameMode.GetGameModeInstance(type) as T;
			if (t != null)
			{
				return t;
			}
			return default(T);
		}

		// Token: 0x06005507 RID: 21767 RVA: 0x001AC494 File Offset: 0x001AA694
		public static void ResetGameModes()
		{
			GameMode.CurrentGameModeType = GameModeType.None;
			GameMode.activeGameMode = null;
			GameMode.activeNetworkHandler = null;
			GameMode.optOutPlayers.Clear();
			GameMode.ParticipatingPlayers.Clear();
			for (int i = 0; i < GameMode.activatedGameModes.Count; i++)
			{
				GorillaGameManager gameMode = GameMode.activatedGameModes[i];
				GameMode.StopGameModeSafe(gameMode);
				GameMode.ResetGameModeSafe(gameMode);
			}
			GameMode.activatedGameModes.Clear();
		}

		// Token: 0x06005508 RID: 21768 RVA: 0x001AC4FC File Offset: 0x001AA6FC
		private static void StartGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StartPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06005509 RID: 21769 RVA: 0x001AC524 File Offset: 0x001AA724
		private static void StopGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.StopPlaying();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600550A RID: 21770 RVA: 0x001AC54C File Offset: 0x001AA74C
		private static void ResetGameModeSafe(GorillaGameManager gameMode)
		{
			try
			{
				gameMode.ResetGame();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600550B RID: 21771 RVA: 0x001AC574 File Offset: 0x001AA774
		public static void ReportTag(NetPlayer player)
		{
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportTag", false, new object[]
				{
					player.ActorNumber
				});
			}
		}

		// Token: 0x0600550C RID: 21772 RVA: 0x001AC5B4 File Offset: 0x001AA7B4
		public static void ReportHit()
		{
			if (GorillaGameManager.instance.GameType() == GameModeType.Custom)
			{
				CustomGameMode.TaggedByEnvironment();
			}
			if (NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_ReportHit", false, Array.Empty<object>());
			}
		}

		// Token: 0x0600550D RID: 21773 RVA: 0x001AC600 File Offset: 0x001AA800
		public static bool LocalIsTagged(NetPlayer player)
		{
			return !GameMode.ActiveGameMode.IsNull() && GameMode.ActiveGameMode.LocalIsTagged(player);
		}

		// Token: 0x0600550E RID: 21774 RVA: 0x001AC61B File Offset: 0x001AA81B
		public static void BroadcastRoundComplete()
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastRoundComplete", true, Array.Empty<object>());
			}
		}

		// Token: 0x0600550F RID: 21775 RVA: 0x001AC658 File Offset: 0x001AA858
		public static void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && NetworkSystem.Instance.InRoom && GameMode.activeNetworkHandler.IsNotNull())
			{
				GameMode.activeNetworkHandler.SendRPC("RPC_BroadcastTag", true, new object[]
				{
					taggedPlayer.ActorNumber,
					taggingPlayer.ActorNumber
				});
			}
		}

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x06005510 RID: 21776 RVA: 0x001AC6BB File Offset: 0x001AA8BB
		public static List<NetPlayer> ParticipatingPlayers
		{
			get
			{
				return GameMode._participatingPlayers;
			}
		}

		// Token: 0x06005511 RID: 21777 RVA: 0x001AC6C4 File Offset: 0x001AA8C4
		public static void RefreshPlayers()
		{
			GameMode._oldPlayersCount = GameMode._participatingPlayers.Count;
			for (int i = 0; i < GameMode._oldPlayersCount; i++)
			{
				GameMode._oldPlayersBuffer[i] = GameMode._participatingPlayers[i];
			}
			GameMode._participatingPlayers.Clear();
			List<NetPlayer> playersInRoom = RoomSystem.PlayersInRoom;
			int num = Mathf.Min(playersInRoom.Count, 10);
			for (int j = 0; j < num; j++)
			{
				if (GameMode.CanParticipate(playersInRoom[j]))
				{
					GameMode.ParticipatingPlayers.Add(playersInRoom[j]);
				}
			}
			GameMode._tempRemovedPlayers.Clear();
			for (int k = 0; k < GameMode._oldPlayersCount; k++)
			{
				NetPlayer netPlayer = GameMode._oldPlayersBuffer[k];
				if (!GameMode.ContainsNetPlayer(GameMode._participatingPlayers, netPlayer))
				{
					GameMode._tempRemovedPlayers.Add(netPlayer);
				}
			}
			GameMode._tempAddedPlayers.Clear();
			int count = GameMode._participatingPlayers.Count;
			for (int l = 0; l < count; l++)
			{
				NetPlayer netPlayer2 = GameMode._participatingPlayers[l];
				if (!GameMode.ContainsNetPlayer(GameMode._oldPlayersBuffer, netPlayer2, GameMode._oldPlayersCount))
				{
					GameMode._tempAddedPlayers.Add(netPlayer2);
				}
			}
			if ((GameMode._tempAddedPlayers.Count > 0 || GameMode._tempRemovedPlayers.Count > 0) && GameMode.ParticipatingPlayersChanged != null)
			{
				GameMode.ParticipatingPlayersChanged.Invoke(GameMode._tempAddedPlayers, GameMode._tempRemovedPlayers);
			}
		}

		// Token: 0x06005512 RID: 21778 RVA: 0x001AC81C File Offset: 0x001AAA1C
		private static bool ContainsNetPlayer(List<NetPlayer> list, NetPlayer candidate)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005513 RID: 21779 RVA: 0x001AC84C File Offset: 0x001AAA4C
		private static bool ContainsNetPlayer(NetPlayer[] array, NetPlayer candidate, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (array[i] == candidate)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005514 RID: 21780 RVA: 0x001AC86E File Offset: 0x001AAA6E
		public static void OptOut(VRRig rig)
		{
			GameMode.OptOut(rig.creator.ActorNumber);
		}

		// Token: 0x06005515 RID: 21781 RVA: 0x001AC880 File Offset: 0x001AAA80
		public static void OptOut(NetPlayer player)
		{
			GameMode.OptOut(player.ActorNumber);
		}

		// Token: 0x06005516 RID: 21782 RVA: 0x001AC88D File Offset: 0x001AAA8D
		public static void OptOut(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Add(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x06005517 RID: 21783 RVA: 0x001AC8A1 File Offset: 0x001AAAA1
		public static void OptIn(VRRig rig)
		{
			GameMode.OptIn(rig.creator.ActorNumber);
		}

		// Token: 0x06005518 RID: 21784 RVA: 0x001AC8B3 File Offset: 0x001AAAB3
		public static void OptIn(NetPlayer player)
		{
			GameMode.OptIn(player.ActorNumber);
		}

		// Token: 0x06005519 RID: 21785 RVA: 0x001AC8C0 File Offset: 0x001AAAC0
		public static void OptIn(int playerActorNumber)
		{
			if (GameMode.optOutPlayers.Remove(playerActorNumber))
			{
				GameMode.RefreshPlayers();
			}
		}

		// Token: 0x0600551A RID: 21786 RVA: 0x001AC8D4 File Offset: 0x001AAAD4
		private static bool CanParticipate(NetPlayer player)
		{
			return player.InRoom() && !GameMode.optOutPlayers.Contains(player.ActorNumber) && NetworkSystem.Instance.GetPlayerTutorialCompletion(player.ActorNumber) && (!(GorillaGameManager.instance != null) || GorillaGameManager.instance.CanPlayerParticipate(player));
		}

		// Token: 0x040061F7 RID: 25079
		[SerializeField]
		private GameModeZoneMapping gameModeZoneMapping;

		// Token: 0x040061F9 RID: 25081
		[OnEnterPlay_SetNull]
		private static GameMode instance;

		// Token: 0x040061FA RID: 25082
		[OnEnterPlay_Clear]
		private static Dictionary<int, GorillaGameManager> gameModeTable = new Dictionary<int, GorillaGameManager>();

		// Token: 0x040061FB RID: 25083
		[OnEnterPlay_Clear]
		private static Dictionary<string, int> gameModeKeyByName = new Dictionary<string, int>();

		// Token: 0x040061FC RID: 25084
		[OnEnterPlay_Clear]
		private static Dictionary<int, FusionGameModeData> fusionTypeTable = new Dictionary<int, FusionGameModeData>();

		// Token: 0x040061FD RID: 25085
		[OnEnterPlay_Clear]
		private static List<GorillaGameManager> gameModes = new List<GorillaGameManager>(10);

		// Token: 0x040061FE RID: 25086
		[OnEnterPlay_Clear]
		public static readonly List<string> gameModeNames = new List<string>(10);

		// Token: 0x040061FF RID: 25087
		[OnEnterPlay_Clear]
		private static readonly List<GorillaGameManager> activatedGameModes = new List<GorillaGameManager>(12);

		// Token: 0x04006200 RID: 25088
		[OnEnterPlay_SetNull]
		private static GorillaGameManager activeGameMode = null;

		// Token: 0x04006201 RID: 25089
		[OnEnterPlay_SetNull]
		private static GameModeSerializer activeNetworkHandler = null;

		// Token: 0x04006204 RID: 25092
		[OnEnterPlay_Clear]
		private static readonly HashSet<int> optOutPlayers = new HashSet<int>(10);

		// Token: 0x04006205 RID: 25093
		[OnEnterPlay_Clear]
		private static readonly List<NetPlayer> _participatingPlayers = new List<NetPlayer>(10);

		// Token: 0x04006206 RID: 25094
		private static readonly NetPlayer[] _oldPlayersBuffer = new NetPlayer[10];

		// Token: 0x04006207 RID: 25095
		private static int _oldPlayersCount;

		// Token: 0x04006208 RID: 25096
		private static readonly List<NetPlayer> _tempAddedPlayers = new List<NetPlayer>(10);

		// Token: 0x04006209 RID: 25097
		private static readonly List<NetPlayer> _tempRemovedPlayers = new List<NetPlayer>(10);

		// Token: 0x02000D8A RID: 3466
		// (Invoke) Token: 0x0600551D RID: 21789
		public delegate void OnStartGameModeAction(GameModeType newGameModeType);
	}
}
