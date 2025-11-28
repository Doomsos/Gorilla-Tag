using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000627 RID: 1575
public class GamePlayer : MonoBehaviour
{
	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x0600280B RID: 10251 RVA: 0x000D5269 File Offset: 0x000D3469
	// (set) Token: 0x0600280C RID: 10252 RVA: 0x000D5271 File Offset: 0x000D3471
	public bool DidJoinWithItems { get; set; }

	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x0600280D RID: 10253 RVA: 0x000D527A File Offset: 0x000D347A
	// (set) Token: 0x0600280E RID: 10254 RVA: 0x000D5282 File Offset: 0x000D3482
	public bool AdditionalDataInitialized { get; set; }

	// Token: 0x0600280F RID: 10255 RVA: 0x000D528C File Offset: 0x000D348C
	private void Awake()
	{
		this.handTransforms = new Transform[2];
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		this.hands = new GamePlayer.HandData[2];
		this.ResetData();
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netSnapLimiter = new CallLimiter(25, 1f, 0.5f);
		if (this.snapPointManager == null)
		{
			this.snapPointManager = base.GetComponentInChildren<SuperInfectionSnapPointManager>(true);
			if (this.snapPointManager == null)
			{
				Debug.LogError("[GamePlayer]  ERROR!!!  Snappoints cannot function because the required `SuperInfectionSnapPointManager` could found in children.", this);
			}
		}
	}

	// Token: 0x06002810 RID: 10256 RVA: 0x000D5394 File Offset: 0x000D3594
	public void Clear()
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityId != GameEntityId.Invalid && this.hands[i].grabbedEntityManager != null)
			{
				this.hands[i].grabbedEntityManager.RequestThrowEntity(this.hands[i].grabbedEntityId, GamePlayer.IsLeftHand(i), GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearGrabbed(i);
		}
		for (int j = 0; j < 2; j++)
		{
			if (this.hands[j].snappedEntityId != GameEntityId.Invalid && this.hands[j].snappedEntityManager != null)
			{
				GameEntityId snappedEntityId = this.hands[j].snappedEntityId;
				GameEntityManager snappedEntityManager = this.hands[j].snappedEntityManager;
				snappedEntityManager.RequestGrabEntity(snappedEntityId, !GamePlayer.IsLeftHand(j), Vector3.zero, Quaternion.identity);
				snappedEntityManager.RequestThrowEntity(snappedEntityId, !GamePlayer.IsLeftHand(j), GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearSnapped(j);
		}
	}

	// Token: 0x06002811 RID: 10257 RVA: 0x000D54E0 File Offset: 0x000D36E0
	public void ResetData()
	{
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
			this.ClearSnapped(i);
		}
		this.DidJoinWithItems = false;
		this.AdditionalDataInitialized = false;
		this.SetInitializePlayer(false);
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000D551C File Offset: 0x000D371C
	private void Start()
	{
		GamePlayer.InitializeStaticLookupCaches();
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x000D5524 File Offset: 0x000D3724
	public void MigrateHeldActorNumbers()
	{
		int actorNumber = this.rig.OwningNetPlayer.ActorNumber;
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityManager != null)
			{
				GameEntity gameEntity = this.hands[i].grabbedEntityManager.GetGameEntity(this.hands[i].grabbedEntityId);
				if (gameEntity != null)
				{
					gameEntity.MigrateHeldBy(actorNumber);
				}
			}
			if (this.hands[i].snappedEntityManager != null)
			{
				GameEntity gameEntity2 = this.hands[i].snappedEntityManager.GetGameEntity(this.hands[i].snappedEntityId);
				if (gameEntity2 != null)
				{
					gameEntity2.MigrateSnappedBy(actorNumber);
				}
			}
		}
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000D55F8 File Offset: 0x000D37F8
	public void SetGrabbed(GameEntityId gameBallId, int handIndex, GameEntityManager gameEntityManager)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GamePlayer.HandData handData = this.hands[handIndex];
		handData.grabbedEntityId = gameBallId;
		handData.grabbedEntityManager = gameEntityManager;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000D5640 File Offset: 0x000D3840
	public void SetSnapped(GameEntityId gameBallId, int handIndex, GameEntityManager gameEntityManager)
	{
		if (gameBallId.IsValid())
		{
			this.ClearSnappedIfSnapped(gameBallId);
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GamePlayer.HandData handData = this.hands[handIndex];
		handData.snappedEntityId = gameBallId;
		handData.snappedEntityManager = gameEntityManager;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06002817 RID: 10263 RVA: 0x000D5690 File Offset: 0x000D3890
	public void ClearZone(GameEntityManager manager)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityId != GameEntityId.Invalid && this.hands[i].grabbedEntityManager == manager)
			{
				GameEntity gameEntity = this.hands[i].grabbedEntityManager.GetGameEntity(this.hands[i].grabbedEntityId);
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased.Invoke();
					}
				}
				this.ClearGrabbed(i);
			}
			if (this.hands[i].snappedEntityId != GameEntityId.Invalid && this.hands[i].snappedEntityManager == manager)
			{
				GameEntity gameEntity2 = this.hands[i].snappedEntityManager.GetGameEntity(this.hands[i].snappedEntityId);
				if (gameEntity2 != null)
				{
					Action onReleased2 = gameEntity2.OnReleased;
					if (onReleased2 != null)
					{
						onReleased2.Invoke();
					}
				}
				this.ClearSnapped(i);
			}
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			this.DidJoinWithItems = false;
		}
	}

	// Token: 0x06002818 RID: 10264 RVA: 0x000D57B8 File Offset: 0x000D39B8
	public void ClearGrabbedIfHeld(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002819 RID: 10265 RVA: 0x000D57F4 File Offset: 0x000D39F4
	public void ClearSnappedIfSnapped(GameEntityId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].snappedEntityId == gameBallId)
			{
				this.ClearSnapped(i);
			}
		}
	}

	// Token: 0x0600281A RID: 10266 RVA: 0x000D582D File Offset: 0x000D3A2D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex, null);
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x000D583C File Offset: 0x000D3A3C
	public void ClearSnapped(int handIndex)
	{
		this.SetSnapped(GameEntityId.Invalid, handIndex, null);
	}

	// Token: 0x0600281C RID: 10268 RVA: 0x000D584B File Offset: 0x000D3A4B
	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000D5853 File Offset: 0x000D3A53
	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000D585C File Offset: 0x000D3A5C
	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x000D5870 File Offset: 0x000D3A70
	public bool IsHoldingEntity(GameEntityManager gameEntityManager, bool isLeftHand)
	{
		return gameEntityManager.GetGameEntity(this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x000D588A File Offset: 0x000D3A8A
	public bool IsHoldingEntity(GameEntityId gameEntityId)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true)) == gameEntityId || this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(false)) == gameEntityId;
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000D58B4 File Offset: 0x000D3AB4
	public void RequestDropAllSnapped()
	{
		this.Clear();
		this.snapPointManager.DropAllSnappedAuthority();
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000D58C7 File Offset: 0x000D3AC7
	public List<GameEntityId> HeldAndSnappedItems(GameEntityManager manager)
	{
		return Enumerable.ToList<GameEntityId>(this.IterateHeldAndSnappedItems(manager));
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000D58D5 File Offset: 0x000D3AD5
	public IEnumerable<GameEntityId> IterateHeldAndSnappedItems(GameEntityManager manager)
	{
		int num;
		for (int i = 0; i < 2; i = num)
		{
			if (this.hands[i].grabbedEntityId != GameEntityId.Invalid && this.hands[i].grabbedEntityManager == manager)
			{
				yield return this.hands[i].grabbedEntityId;
			}
			if (this.hands[i].snappedEntityId != GameEntityId.Invalid && this.hands[i].snappedEntityManager == manager)
			{
				yield return this.hands[i].snappedEntityId;
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000D58EC File Offset: 0x000D3AEC
	public List<GameEntity> HeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		return Enumerable.ToList<GameEntity>(this.IterateHeldAndSnappedEntities(ignoreEntitiesInManager));
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000D58FA File Offset: 0x000D3AFA
	public IEnumerable<GameEntity> IterateHeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		int num;
		for (int i = 0; i < 2; i = num)
		{
			if (this.hands[i].grabbedEntityId != GameEntityId.Invalid && this.hands[i].grabbedEntityManager != null && this.hands[i].grabbedEntityManager != ignoreEntitiesInManager)
			{
				GameEntity gameEntity = this.hands[i].grabbedEntityManager.GetGameEntity(this.hands[i].grabbedEntityId);
				yield return gameEntity;
			}
			if (this.hands[i].snappedEntityId != GameEntityId.Invalid && this.hands[i].snappedEntityManager != null && this.hands[i].snappedEntityManager != ignoreEntitiesInManager)
			{
				GameEntity gameEntity2 = this.hands[i].snappedEntityManager.GetGameEntity(this.hands[i].snappedEntityId);
				yield return gameEntity2;
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000D5914 File Offset: 0x000D3B14
	public void DeleteGrabbedEntityLocal(int handIndex)
	{
		if (this.hands[handIndex].grabbedEntityId != GameEntityId.Invalid && this.hands[handIndex].grabbedEntityManager != null)
		{
			GameEntity gameEntity = this.hands[handIndex].grabbedEntityManager.GetGameEntity(this.hands[handIndex].grabbedEntityId);
			if (gameEntity != null)
			{
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased.Invoke();
					}
				}
				this.hands[handIndex].grabbedEntityManager.DestroyItemLocal(this.hands[handIndex].grabbedEntityId);
			}
		}
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000D59C8 File Offset: 0x000D3BC8
	public int MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		int num = 0;
		for (int i = 0; i < this.hands.Length; i++)
		{
			GameEntityId grabbedEntityId = this.hands[i].grabbedEntityId;
			if (grabbedEntityId != GameEntityId.Invalid && this.hands[i].grabbedEntityManager != newEntityManager)
			{
				GameEntity gameEntity = this.hands[i].grabbedEntityManager.GetGameEntity(grabbedEntityId);
				if (gameEntity != null && gameEntity.IsValidToMigrate())
				{
					GameEntityId grabbedEntityId2 = gameEntity.MigrateToEntityManager(newEntityManager);
					this.hands[i].grabbedEntityManager = newEntityManager;
					this.hands[i].grabbedEntityId = grabbedEntityId2;
					num++;
				}
			}
			GameEntityId snappedEntityId = this.hands[i].snappedEntityId;
			if (snappedEntityId != GameEntityId.Invalid && this.hands[i].snappedEntityManager != newEntityManager)
			{
				GameEntity gameEntity2 = this.hands[i].snappedEntityManager.GetGameEntity(snappedEntityId);
				if (gameEntity2 != null && gameEntity2.IsValidToMigrate())
				{
					GameEntityId snappedEntityId2 = gameEntity2.MigrateToEntityManager(newEntityManager);
					this.hands[i].snappedEntityManager = newEntityManager;
					this.hands[i].snappedEntityId = snappedEntityId2;
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000D5B1F File Offset: 0x000D3D1F
	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000D5B2D File Offset: 0x000D3D2D
	public GameEntityId GetGrabbedGameEntityId(int handIndex)
	{
		if (handIndex < 0 || handIndex >= this.hands.Length)
		{
			return GameEntityId.Invalid;
		}
		return this.hands[handIndex].grabbedEntityId;
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x000D5B55 File Offset: 0x000D3D55
	public GameEntityId GetGrabbedGameEntityIdAndManager(int handIndex, out GameEntityManager manager)
	{
		if (handIndex < 0 || handIndex >= this.hands.Length)
		{
			manager = null;
			return GameEntityId.Invalid;
		}
		manager = this.hands[handIndex].grabbedEntityManager;
		return this.hands[handIndex].grabbedEntityId;
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x000D5B94 File Offset: 0x000D3D94
	public GameEntity GetGrabbedGameEntity(int handIndex)
	{
		if (handIndex < 0 || handIndex >= this.hands.Length || this.hands[handIndex].grabbedEntityManager == null)
		{
			return null;
		}
		return this.hands[handIndex].grabbedEntityManager.GetGameEntity(this.GetGrabbedGameEntityId(handIndex));
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000D5BE8 File Offset: 0x000D3DE8
	public int FindHandIndex(GameEntityId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000D5C24 File Offset: 0x000D3E24
	public int FindSnapIndex(GameEntityId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].snappedEntityId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000D5C60 File Offset: 0x000D3E60
	public GameEntityId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedEntityId.IsValid())
			{
				return this.hands[i].grabbedEntityId;
			}
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000B1CAB File Offset: 0x000AFEAB
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x000B1CB1 File Offset: 0x000AFEB1
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000D5CB0 File Offset: 0x000D3EB0
	[Obsolete("Method `GamePlayer.TryGetGamePlayer(Player)` is obsolete, use `TryGetGamePlayer(Player, out GamePlayer)` instead.")]
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null && currentRoom.GetPlayer(actorNumber, false) == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000D5CFC File Offset: 0x000D3EFC
	public static GamePlayer GetGamePlayer(Player player)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(player, out result);
		return result;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000D5D13 File Offset: 0x000D3F13
	public static bool TryGetGamePlayer(Player player, out GamePlayer gamePlayer)
	{
		if (player == null)
		{
			gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(player.ActorNumber, out gamePlayer);
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x000D5D2C File Offset: 0x000D3F2C
	[Obsolete("Method `GamePlayer.GetGamePlayer(actorNum)` is obsolete, use `TryGetGamePlayer(actorNum, out GamePlayer)` instead.")]
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(actorNumber, out result);
		return result;
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x000D5D44 File Offset: 0x000D3F44
	public static bool TryGetGamePlayer(int actorNumber, out GamePlayer out_gamePlayer)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			out_gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(rigContainer.Rig, out out_gamePlayer);
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x000D5D80 File Offset: 0x000D3F80
	[Obsolete("Method `GamePlayer.GetGamePlayer(VRRig)` is obsolete, use `TryGetGamePlayer(VRRig, out GamePlayer)` instead.")]
	public static GamePlayer GetGamePlayer(VRRig rig)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(rig, out result);
		return result;
	}

	// Token: 0x06002837 RID: 10295 RVA: 0x000D5D98 File Offset: 0x000D3F98
	public static bool TryGetGamePlayer(VRRig rig, out GamePlayer out_gamePlayer)
	{
		if (rig == null)
		{
			out_gamePlayer = null;
			return false;
		}
		out_gamePlayer = rig.GetComponent<GamePlayer>();
		return out_gamePlayer != null;
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000D5DC4 File Offset: 0x000D3FC4
	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000D5E00 File Offset: 0x000D4000
	public static Transform GetHandTransform(VRRig rig, int handIndex)
	{
		GamePlayer gamePlayer;
		if (handIndex >= 0 && handIndex < 2 && GamePlayer.TryGetGamePlayer(rig, out gamePlayer))
		{
			return gamePlayer.handTransforms[handIndex];
		}
		return null;
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000D5E29 File Offset: 0x000D4029
	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000D5E50 File Offset: 0x000D4050
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player, GameEntityManager manager)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedEntityManager == manager)
			{
				int netIdFromEntityId = manager.GetNetIdFromEntityId(this.hands[i].grabbedEntityId);
				writer.Write(netIdFromEntityId);
				long num = 0L;
				if (netIdFromEntityId != -1)
				{
					GameEntity gameEntity = manager.GetGameEntity(this.hands[i].grabbedEntityId);
					if (gameEntity != null)
					{
						num = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
					}
				}
				writer.Write(num);
			}
			else
			{
				writer.Write(-1);
				writer.Write(0L);
			}
			if (this.hands[i].snappedEntityManager == manager)
			{
				int netIdFromEntityId2 = manager.GetNetIdFromEntityId(this.hands[i].snappedEntityId);
				writer.Write(netIdFromEntityId2);
				long num2 = 0L;
				if (netIdFromEntityId2 != -1)
				{
					GameEntity gameEntity2 = manager.GetGameEntity(this.hands[i].snappedEntityId);
					if (gameEntity2 != null)
					{
						num2 = BitPackUtils.PackHandPosRotForNetwork(gameEntity2.transform.localPosition, gameEntity2.transform.localRotation);
					}
				}
				writer.Write(num2);
			}
			else
			{
				writer.Write(-1);
				writer.Write(0L);
			}
		}
		writer.Write(this.AdditionalDataInitialized);
	}

	// Token: 0x0600283C RID: 10300 RVA: 0x000D5FB0 File Offset: 0x000D41B0
	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer, GameEntityManager manager)
	{
		for (int i = 0; i < 2; i++)
		{
			int num = reader.ReadInt32();
			long num2 = reader.ReadInt64();
			int num3 = reader.ReadInt32();
			long num4 = reader.ReadInt64();
			if (num != -1)
			{
				GameEntityId entityIdFromNetId = manager.GetEntityIdFromNetId(num);
				if (entityIdFromNetId.IsValid())
				{
					GameEntity gameEntity = manager.GetGameEntity(entityIdFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out localPosition, out localRotation);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							manager.GrabEntityOnCreate(entityIdFromNetId, GamePlayer.IsLeftHand(i), localPosition, localRotation, gamePlayer.rig.OwningNetPlayer);
						}
					}
				}
			}
			if (num3 != -1)
			{
				GameEntityId entityIdFromNetId2 = manager.GetEntityIdFromNetId(num3);
				if (entityIdFromNetId2.IsValid())
				{
					GameEntity gameEntity2 = manager.GetGameEntity(entityIdFromNetId2);
					if (num4 != 0L && !(gameEntity2 == null))
					{
						Vector3 localPosition2;
						Quaternion localRotation2;
						BitPackUtils.UnpackHandPosRotFromNetwork(num4, out localPosition2, out localRotation2);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							SnapJointType jointType = GamePlayer.IsLeftHand(i) ? SnapJointType.HandL : SnapJointType.HandR;
							manager.SnapEntityOnCreate(entityIdFromNetId2, GamePlayer.IsLeftHand(i), localPosition2, localRotation2, (int)jointType, gamePlayer.rig.OwningNetPlayer);
						}
					}
				}
			}
		}
		bool initializePlayer = reader.ReadBoolean();
		if (gamePlayer != null)
		{
			gamePlayer.SetInitializePlayer(initializePlayer);
		}
	}

	// Token: 0x0600283D RID: 10301 RVA: 0x000D60EC File Offset: 0x000D42EC
	internal static void InitializeStaticLookupCaches()
	{
		GamePlayer.lookupCache_actorNum_to_gamePlayer = new ValueTuple<int, GamePlayer>[10];
		GamePlayer.lookupCache_rigInstanceId_to_gamePlayer = new ValueTuple<int, GamePlayer>[10];
		if (VRRigCache.isInitialized)
		{
			GamePlayer.UpdateStaticLookupCaches();
		}
	}

	// Token: 0x0600283E RID: 10302 RVA: 0x000D6114 File Offset: 0x000D4314
	internal static void UpdateStaticLookupCaches()
	{
		if (GamePlayer.lookupCache_actorNum_to_gamePlayer == null)
		{
			return;
		}
		List<VRRig> list;
		using (ListPool<VRRig>.Get(ref list))
		{
			if (list.Capacity < 10)
			{
				list.Capacity = 10;
			}
			VRRigCache.Instance.GetActiveRigs(list);
			if (list.Count > GamePlayer.lookupCache_actorNum_to_gamePlayer.Length)
			{
				int num = list.Count * 2;
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_actorNum_to_gamePlayer, num);
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_rigInstanceId_to_gamePlayer, num);
			}
			GamePlayer.staticLookupCachesCount = list.Count;
			if (GamePlayer.staticLookupCachesCount >= 1)
			{
				VRRig vrrig = list[0];
				if (vrrig == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) The VRRig at index 0 is expected to be the local rig but is null.");
				}
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(actorNumber, gamePlayer);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(vrrig.GetInstanceID(), gamePlayer);
			}
			for (int i = 1; i < GamePlayer.staticLookupCachesCount; i++)
			{
				VRRig vrrig2 = list[i];
				if (vrrig2 == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) An entry from `VRRigCache.Instance.GetActiveRigs(activeRigs)` is null but is expected to be ready and all entries not null at this stage.");
				}
				GamePlayer component = vrrig2.GetComponent<GamePlayer>();
				if (component == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) Could not get GamePlayer from rig which is expected to be ready at this stage.");
				}
				NetPlayer owningNetPlayer = vrrig2.OwningNetPlayer;
				int num2 = (owningNetPlayer != null) ? owningNetPlayer.ActorNumber : int.MinValue;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(num2, component);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(vrrig2.GetInstanceID(), component);
			}
			for (int j = GamePlayer.staticLookupCachesCount; j < GamePlayer.lookupCache_actorNum_to_gamePlayer.Length; j++)
			{
				GamePlayer.lookupCache_actorNum_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
			}
		}
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000D62FC File Offset: 0x000D44FC
	public void SetInitializePlayer(bool initialized)
	{
		bool additionalDataInitialized = this.AdditionalDataInitialized;
		this.AdditionalDataInitialized = initialized;
		if (!additionalDataInitialized && this.AdditionalDataInitialized)
		{
			Action onPlayerInitialized = this.OnPlayerInitialized;
			if (onPlayerInitialized == null)
			{
				return;
			}
			onPlayerInitialized.Invoke();
		}
	}

	// Token: 0x0400338B RID: 13195
	private const string preLog = "[GamePlayer]  ";

	// Token: 0x0400338C RID: 13196
	private const string preErr = "[GamePlayer]  ERROR!!!  ";

	// Token: 0x0400338D RID: 13197
	public VRRig rig;

	// Token: 0x0400338E RID: 13198
	public Transform leftHand;

	// Token: 0x0400338F RID: 13199
	public Transform rightHand;

	// Token: 0x04003390 RID: 13200
	public SuperInfectionSnapPointManager snapPointManager;

	// Token: 0x04003391 RID: 13201
	private Transform[] handTransforms;

	// Token: 0x04003392 RID: 13202
	private GamePlayer.HandData[] hands;

	// Token: 0x04003393 RID: 13203
	public const int MAX_HANDS = 2;

	// Token: 0x04003394 RID: 13204
	public const int LEFT_HAND = 0;

	// Token: 0x04003395 RID: 13205
	public const int RIGHT_HAND = 1;

	// Token: 0x04003396 RID: 13206
	public CallLimiter newJoinZoneLimiter;

	// Token: 0x04003397 RID: 13207
	public CallLimiter netImpulseLimiter;

	// Token: 0x04003398 RID: 13208
	public CallLimiter netGrabLimiter;

	// Token: 0x04003399 RID: 13209
	public CallLimiter netThrowLimiter;

	// Token: 0x0400339A RID: 13210
	public CallLimiter netStateLimiter;

	// Token: 0x0400339B RID: 13211
	public CallLimiter netSnapLimiter;

	// Token: 0x0400339E RID: 13214
	public Action OnPlayerInitialized;

	// Token: 0x0400339F RID: 13215
	public Action OnPlayerLeftZone;

	// Token: 0x040033A0 RID: 13216
	private bool grabbingDisabled;

	// Token: 0x040033A1 RID: 13217
	private const bool _k_MATTO__USE_STATIC_CACHE = false;

	// Token: 0x040033A2 RID: 13218
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_actorNum_to_gamePlayer;

	// Token: 0x040033A3 RID: 13219
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_rigInstanceId_to_gamePlayer;

	// Token: 0x040033A4 RID: 13220
	[OnEnterPlay_Set(0)]
	private static int staticLookupCachesCount;

	// Token: 0x040033A5 RID: 13221
	public const int INVALID_ACTOR_NUMBER = -2147483648;

	// Token: 0x02000628 RID: 1576
	private struct HandData
	{
		// Token: 0x040033A6 RID: 13222
		public GameEntityId grabbedEntityId;

		// Token: 0x040033A7 RID: 13223
		public GameEntityManager grabbedEntityManager;

		// Token: 0x040033A8 RID: 13224
		public GameEntityId snappedEntityId;

		// Token: 0x040033A9 RID: 13225
		public GameEntityManager snappedEntityManager;
	}
}
