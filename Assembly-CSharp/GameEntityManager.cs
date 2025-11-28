using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using ExitGames.Client.Photon;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;

// Token: 0x0200060F RID: 1551
[NetworkBehaviourWeaved(0)]
public class GameEntityManager : NetworkComponent, IMatchmakingCallbacks, IInRoomCallbacks, IRequestableOwnershipGuardCallbacks, ITickSystemTick, IGorillaSliceableSimple
{
	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06002725 RID: 10021 RVA: 0x000CEB20 File Offset: 0x000CCD20
	// (remove) Token: 0x06002726 RID: 10022 RVA: 0x000CEB58 File Offset: 0x000CCD58
	public event GameEntityManager.ZoneStartEvent onZoneStart;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x06002727 RID: 10023 RVA: 0x000CEB90 File Offset: 0x000CCD90
	// (remove) Token: 0x06002728 RID: 10024 RVA: 0x000CEBC8 File Offset: 0x000CCDC8
	public event GameEntityManager.ZoneClearEvent onZoneClear;

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x06002729 RID: 10025 RVA: 0x000CEBFD File Offset: 0x000CCDFD
	// (set) Token: 0x0600272A RID: 10026 RVA: 0x000CEC04 File Offset: 0x000CCE04
	public static GameEntityManager activeManager { get; private set; }

	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x0600272B RID: 10027 RVA: 0x000CEC0C File Offset: 0x000CCE0C
	// (set) Token: 0x0600272C RID: 10028 RVA: 0x000CEC14 File Offset: 0x000CCE14
	public bool TickRunning { get; set; }

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x0600272D RID: 10029 RVA: 0x000CEC1D File Offset: 0x000CCE1D
	// (set) Token: 0x0600272E RID: 10030 RVA: 0x000CEC25 File Offset: 0x000CCE25
	public bool PendingTableData { get; private set; }

	// Token: 0x0600272F RID: 10031 RVA: 0x000CEC30 File Offset: 0x000CCE30
	protected override void Awake()
	{
		base.Awake();
		this.entities = new List<GameEntity>(64);
		this.gameEntityData = new List<GameEntityData>(64);
		this.netIdToIndex = new Dictionary<int, int>(16384);
		this.netIds = new NativeArray<int>(16384, 4, 1);
		this.createdItemTypeCount = new Dictionary<int, int>();
		this.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(this.OnEntityRemoved, new Action<GameEntity>(CustomGameMode.OnGameEntityRemoved));
		this.zoneStateData = new GameEntityManager.ZoneStateData
		{
			zoneStateRequests = new List<GameEntityManager.ZoneStateRequest>(),
			zonePlayers = new List<Player>(),
			recievedStateBytes = new byte[15360],
			numRecievedStateBytes = 0
		};
		this.guard.AddCallbackTarget(this);
		this.netIdsForCreate = new List<int>();
		this.entityTypeIdsForCreate = new List<int>();
		this.packedPositionsForCreate = new List<long>();
		this.packedRotationsForCreate = new List<int>();
		this.createDataForCreate = new List<long>();
		this.netIdsForDelete = new List<int>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<long>();
		this.zoneComponents = new List<IGameEntityZoneComponent>(8);
		if (this.ghostReactorManager != null)
		{
			this.zoneComponents.Add(this.ghostReactorManager);
		}
		if (this.customMapsManager != null)
		{
			this.zoneComponents.Add(this.customMapsManager);
		}
		if (this.superInfectionManager != null)
		{
			this.zoneComponents.Add(this.superInfectionManager);
		}
		this.BuildFactory();
		GameEntityManager.allManagers.Add(this);
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000CEDC4 File Offset: 0x000CCFC4
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
		VRRigCache.OnRigDeactivated += new Action<RigContainer>(this.OnRigDeactivated);
		VRRigCache.OnActiveRigsChanged += new Action(this.RefreshRigList);
		this.RefreshRigList();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000CEE12 File Offset: 0x000CD012
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
		VRRigCache.OnRigDeactivated -= new Action<RigContainer>(this.OnRigDeactivated);
		VRRigCache.OnActiveRigsChanged -= new Action(this.RefreshRigList);
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000CEE4F File Offset: 0x000CD04F
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.netIds.Dispose();
		GameEntityManager.allManagers.Remove(this);
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000CEE70 File Offset: 0x000CD070
	public static GameEntityManager GetManagerForZone(GTZone zone)
	{
		for (int i = 0; i < GameEntityManager.allManagers.Count; i++)
		{
			if (GameEntityManager.allManagers[i].zone == zone)
			{
				return GameEntityManager.allManagers[i];
			}
		}
		return null;
	}

	// Token: 0x06002734 RID: 10036 RVA: 0x000CEEB2 File Offset: 0x000CD0B2
	public void SliceUpdate()
	{
		this.UpdateZoneState();
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000CEEBC File Offset: 0x000CD0BC
	public void Tick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		float time = Time.time;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (gameEntity != null && gameEntity.LastTickTime + gameEntity.MinTimeBetweenTicks < time && gameEntity.isActiveAndEnabled)
			{
				Action onTick = gameEntity.OnTick;
				if (onTick != null)
				{
					onTick.Invoke();
				}
				gameEntity.LastTickTime = time;
			}
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.netIdsForCreate.Count > 0 && Time.time > this.lastCreateSent + this.createCooldown)
		{
			this.lastCreateSent = Time.time;
			this.photonView.RPC("CreateItemRPC", 1, new object[]
			{
				this.netIdsForCreate.ToArray(),
				this.entityTypeIdsForCreate.ToArray(),
				this.packedPositionsForCreate.ToArray(),
				this.packedRotationsForCreate.ToArray(),
				this.createDataForCreate.ToArray()
			});
			this.netIdsForCreate.Clear();
			this.entityTypeIdsForCreate.Clear();
			this.packedPositionsForCreate.Clear();
			this.packedRotationsForCreate.Clear();
			this.createDataForCreate.Clear();
		}
		if (this.netIdsForDelete.Count > 0 && Time.time > this.lastDestroySent + this.destroyCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("DestroyItemRPC", 1, new object[]
			{
				this.netIdsForDelete.ToArray()
			});
			this.netIdsForDelete.Clear();
		}
		if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSent + this.stateCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("ApplyStateRPC", 0, new object[]
			{
				this.netIdsForState.ToArray(),
				this.statesForState.ToArray()
			});
			this.netIdsForState.Clear();
			this.statesForState.Clear();
		}
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000CF0D6 File Offset: 0x000CD2D6
	public GameEntityId AddGameEntity(GameEntity gameEntity)
	{
		return this.AddGameEntity(this.CreateNetId(), gameEntity);
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000CF0E8 File Offset: 0x000CD2E8
	public GameEntityId AddGameEntity(int netId, GameEntity gameEntity)
	{
		int num = this.FindNewEntityIndex();
		this.entities[num] = gameEntity;
		GameEntityData gameEntityData = default(GameEntityData);
		this.gameEntityData.Add(gameEntityData);
		gameEntity.id = new GameEntityId
		{
			index = num
		};
		this.netIdToIndex[netId] = num;
		this.netIds[num] = netId;
		Action<GameEntity> onEntityAdded = this.OnEntityAdded;
		if (onEntityAdded != null)
		{
			onEntityAdded.Invoke(gameEntity);
		}
		return gameEntity.id;
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000CF168 File Offset: 0x000CD368
	private int FindNewEntityIndex()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] == null)
			{
				return i;
			}
		}
		this.entities.Add(null);
		return this.entities.Count - 1;
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000CF1BC File Offset: 0x000CD3BC
	public void RemoveGameEntity(GameEntity entity)
	{
		int index = entity.id.index;
		if (index < 0 || index >= this.entities.Count)
		{
			return;
		}
		if (this.entities[index] == entity)
		{
			this.entities[index] = null;
		}
		else
		{
			for (int i = 0; i < this.entities.Count; i++)
			{
				if (this.entities[i] == entity)
				{
					this.entities[i] = null;
					break;
				}
			}
		}
		Action<GameEntity> onEntityRemoved = this.OnEntityRemoved;
		if (onEntityRemoved == null)
		{
			return;
		}
		onEntityRemoved.Invoke(entity);
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000CF255 File Offset: 0x000CD455
	public List<GameEntity> GetGameEntities()
	{
		return this.entities;
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000CF260 File Offset: 0x000CD460
	public bool IsValidNetId(int netId)
	{
		int num;
		return this.netIdToIndex.TryGetValue(netId, ref num) && num >= 0 && num < this.entities.Count;
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000CF294 File Offset: 0x000CD494
	public int FindOpenIndex()
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (this.netIds[i] != -1)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000CF2CC File Offset: 0x000CD4CC
	public GameEntityId GetEntityIdFromNetId(int netId)
	{
		int index;
		if (this.netIdToIndex.TryGetValue(netId, ref index))
		{
			return new GameEntityId
			{
				index = index
			};
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000CF300 File Offset: 0x000CD500
	public int GetNetIdFromEntityId(GameEntityId id)
	{
		if (id.index < 0 || id.index >= this.netIds.Length)
		{
			return -1;
		}
		return this.netIds[id.index];
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000CF331 File Offset: 0x000CD531
	public virtual bool IsAuthority()
	{
		return !NetworkSystem.Instance.InRoom || this.guard.isTrulyMine;
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x000CF34C File Offset: 0x000CD54C
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return player != null && this.IsAuthorityPlayer(player.GetPlayerRef());
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000CF35F File Offset: 0x000CD55F
	public bool IsAuthorityPlayer(Player player)
	{
		return player != null && this.guard.actualOwner != null && player == this.guard.actualOwner.GetPlayerRef();
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000CF386 File Offset: 0x000CD586
	public bool IsZoneAuthority()
	{
		return this.IsAuthority();
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000CF38E File Offset: 0x000CD58E
	public bool HasAuthority()
	{
		return this.GetAuthorityPlayer() != null;
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000CF399 File Offset: 0x000CD599
	public Player GetAuthorityPlayer()
	{
		if (this.guard.actualOwner != null)
		{
			return this.guard.actualOwner.GetPlayerRef();
		}
		return null;
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000CF3BA File Offset: 0x000CD5BA
	public virtual bool IsZoneActive()
	{
		return this.zoneStateData.state == GameEntityManager.ZoneState.Active;
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000CF3CC File Offset: 0x000CD5CC
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.zoneLimit == null || this.zoneLimit.bounds.Contains(pos);
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x000CF3FD File Offset: 0x000CD5FD
	public virtual bool IsValidClientRPC(Player sender)
	{
		return this.IsAuthorityPlayer(sender) && (this.IsZoneActive() || sender == PhotonNetwork.LocalPlayer);
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000CF41C File Offset: 0x000CD61C
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.IsValidClientRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x000CF430 File Offset: 0x000CD630
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidClientRPC(sender, entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000CF445 File Offset: 0x000CD645
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.IsValidClientRPC(sender) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000CF459 File Offset: 0x000CD659
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.IsAuthority() && (this.IsZoneActive() || sender == PhotonNetwork.LocalPlayer);
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000CF477 File Offset: 0x000CD677
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000CF48B File Offset: 0x000CD68B
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender, entityNetId) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000CF4A0 File Offset: 0x000CD6A0
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsPositionInZone(pos);
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000CF4B4 File Offset: 0x000CD6B4
	public bool IsValidEntity(GameEntityId id)
	{
		return this.GetGameEntity(id) != null;
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000CF4C3 File Offset: 0x000CD6C3
	public GameEntity GetGameEntity(GameEntityId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		return this.GetGameEntity(id.index);
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000CF4DC File Offset: 0x000CD6DC
	public GameEntity GetGameEntityFromNetId(int netId)
	{
		int index;
		if (this.netIdToIndex.TryGetValue(netId, ref index))
		{
			return this.GetGameEntity(index);
		}
		return null;
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000CF502 File Offset: 0x000CD702
	private GameEntity GetGameEntity(int index)
	{
		if (index == -1)
		{
			return null;
		}
		if (index < 0 || index >= this.entities.Count)
		{
			return null;
		}
		return this.entities[index];
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000CF52C File Offset: 0x000CD72C
	public T GetGameComponent<T>(GameEntityId id) where T : Component
	{
		GameEntity gameEntity = this.GetGameEntity(id);
		if (gameEntity == null)
		{
			return default(T);
		}
		return gameEntity.GetComponent<T>();
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000CF55C File Offset: 0x000CD75C
	public bool IsEntityValidToMigrate(GameEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		Vector3 position = VRRig.LocalRig.transform.position;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag = true;
		int num = 0;
		while (num < this.zoneComponents.Count && flag)
		{
			flag &= this.zoneComponents[num].ValidateMigratedGameEntity(this.GetNetIdFromEntityId(entity.id), entity.typeId, position, Quaternion.identity, entity.createData, actorNumber);
			num++;
		}
		return flag;
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000CF5E0 File Offset: 0x000CD7E0
	private void BuildFactory()
	{
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			string text = "[GameEntityManager]  BuildFactory: Entity names and typeIds for manager \"" + base.name + "\":";
			utf16ValueStringBuilder.AppendLine(text);
			foreach (IGameEntityZoneComponent gameEntityZoneComponent in this.zoneComponents)
			{
				IFactoryItemProvider factoryItemProvider = gameEntityZoneComponent as IFactoryItemProvider;
				if (factoryItemProvider != null)
				{
					foreach (GameEntity gameEntity in factoryItemProvider.GetFactoryItems())
					{
						if (!this.tempFactoryItems.Contains(gameEntity))
						{
							this.tempFactoryItems.Add(gameEntity);
						}
					}
				}
			}
			this.itemPrefabFactory = new Dictionary<int, GameObject>(1024);
			this.priceLookupByEntityId = new Dictionary<int, int>();
			for (int i = 0; i < this.tempFactoryItems.Count; i++)
			{
				GameObject gameObject = this.tempFactoryItems[i].gameObject;
				int staticHash = gameObject.name.GetStaticHash();
				if (gameObject.GetComponent<GRToolLantern>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				else if (gameObject.GetComponent<GRToolCollector>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				this.itemPrefabFactory.Add(staticHash, gameObject);
				utf16ValueStringBuilder.AppendFormat<string, int>("    - name=\"{0}\", typeId={1}\n", gameObject.name, staticHash);
				if (utf16ValueStringBuilder.Length > 5000)
				{
					utf16ValueStringBuilder.Append("... (continued in next log message) ...");
					utf16ValueStringBuilder.Clear();
					if (i + 1 < this.tempFactoryItems.Count)
					{
						utf16ValueStringBuilder.Append(text);
						utf16ValueStringBuilder.Append(" ... CONTINUED FROM PREVIOUS ...\n");
					}
				}
			}
		}
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000CF7F4 File Offset: 0x000CD9F4
	private int CreateNetId()
	{
		int result = this.nextNetId;
		this.nextNetId++;
		return result;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000CF80C File Offset: 0x000CDA0C
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive() || !this.IsPositionInZone(position))
		{
			return GameEntityId.Invalid;
		}
		long num = BitPackUtils.PackWorldPosForNetwork(position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(rotation);
		int num3 = this.CreateNetId();
		this.netIdsForCreate.Add(num3);
		this.entityTypeIdsForCreate.Add(entityTypeId);
		this.packedPositionsForCreate.Add(num);
		this.packedRotationsForCreate.Add(num2);
		this.createDataForCreate.Add(createData);
		return this.CreateAndInitItemLocal(num3, entityTypeId, position, rotation, createData);
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000CF898 File Offset: 0x000CDA98
	[PunRPC]
	public void CreateItemRPC(int[] netId, int[] entityTypeId, long[] packedPos, int[] packedRot, long[] createData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItem))
		{
			return;
		}
		if (netId == null || entityTypeId == null || packedPos == null || createData == null || netId.Length != entityTypeId.Length || netId.Length != packedPos.Length || netId.Length != packedRot.Length || netId.Length != createData.Length)
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos[i]);
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRot[i]);
			float num = 10000f;
			if (!vector.IsValid(num) || !rotation.IsValid() || !this.FactoryHasEntity(entityTypeId[i]) || !this.IsPositionInZone(vector))
			{
				return;
			}
			this.CreateAndInitItemLocal(netId[i], entityTypeId[i], vector, rotation, createData[i]);
		}
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000CF958 File Offset: 0x000CDB58
	public void RequestCreateItems(List<GameEntityCreateData> entityData)
	{
		Debug.Log(string.Format("GameEntityManager RequestCreateItems List.Count: {0}", entityData.Count));
		if (!this.IsZoneAuthority() || !this.IsZoneActive())
		{
			GTDev.LogError<string>(string.Format("[GameEntityManager::RequestCreateItems] Cannot create items. Zone Auth: {0} ", this.IsZoneAuthority()) + string.Format("| Zone Active: {0}", this.IsZoneActive()), null);
			return;
		}
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(GameEntityManager.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(entityData.Count);
		for (int i = 0; i < entityData.Count; i++)
		{
			GameEntityCreateData gameEntityCreateData = entityData[i];
			int num = this.CreateNetId();
			long num2 = BitPackUtils.PackWorldPosForNetwork(gameEntityCreateData.position);
			int num3 = BitPackUtils.PackQuaternionForNetwork(gameEntityCreateData.rotation);
			binaryWriter.Write(num);
			binaryWriter.Write(gameEntityCreateData.entityTypeId);
			binaryWriter.Write(num2);
			binaryWriter.Write(num3);
			binaryWriter.Write(gameEntityCreateData.createData);
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		this.photonView.RPC("CreateItemsRPC", 0, new object[]
		{
			(int)this.zone,
			array
		});
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000CFA98 File Offset: 0x000CDC98
	[PunRPC]
	public void CreateItemsRPC(int zoneId, byte[] stateData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || stateData == null || stateData.Length >= 15360 || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItems))
		{
			return;
		}
		try
		{
			byte[] array = GZipStream.UncompressBuffer(stateData);
			int num = array.Length;
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num2 = binaryReader.ReadInt32();
					for (int i = 0; i < num2; i++)
					{
						int netId = binaryReader.ReadInt32();
						int entityTypeId = binaryReader.ReadInt32();
						long data = binaryReader.ReadInt64();
						int data2 = binaryReader.ReadInt32();
						long createData = binaryReader.ReadInt64();
						Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
						Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
						float num3 = 10000f;
						if (vector.IsValid(num3) && rotation.IsValid() && this.FactoryHasEntity(entityTypeId) && this.IsPositionInZone(vector))
						{
							this.CreateAndInitItemLocal(netId, entityTypeId, vector, rotation, createData);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000CFBB8 File Offset: 0x000CDDB8
	public void JoinWithItems(List<GameEntity> entities)
	{
		if (entities.Count == 0)
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(GameEntityManager.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		int num = 0;
		for (int i = 0; i < entities.Count; i++)
		{
			if (entities[i] != null)
			{
				num++;
			}
		}
		binaryWriter.Write(num);
		for (int j = 0; j < entities.Count; j++)
		{
			GameEntity gameEntity = entities[j];
			if (!(gameEntity == null))
			{
				int typeId = gameEntity.typeId;
				long num2 = BitPackUtils.PackWorldPosForNetwork(gameEntity.transform.localPosition);
				int num3 = BitPackUtils.PackQuaternionForNetwork(gameEntity.transform.localRotation);
				long num4 = gameEntity.createData;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					num4 = this.zoneComponents[k].ProcessMigratedGameEntityCreateData(gameEntity, num4);
				}
				byte b;
				switch (gameEntity.snappedJoint)
				{
				default:
					b = ((gameEntity.heldByHandIndex == 0) ? 1 : 0);
					break;
				case SnapJointType.HandL:
					b = 3;
					break;
				case SnapJointType.HandR:
					b = 2;
					break;
				}
				binaryWriter.Write(typeId);
				binaryWriter.Write(num2);
				binaryWriter.Write(num3);
				binaryWriter.Write(num4);
				binaryWriter.Write(b);
			}
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		this.photonView.RPC("JoinWithItemsRPC", this.GetAuthorityPlayer(), new object[]
		{
			array,
			new int[0],
			PhotonNetwork.LocalPlayer.ActorNumber
		});
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000CFD70 File Offset: 0x000CDF70
	[PunRPC]
	public void PlayerLeftZoneRPC(PhotonMessageInfo info)
	{
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			gamePlayer.DidJoinWithItems = false;
		}
		foreach (GameEntityId gameEntityId in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId)))
			{
				this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId));
			}
			this.DestroyItemLocal(gameEntityId);
		}
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone.Invoke();
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000CFE14 File Offset: 0x000CE014
	[PunRPC]
	public void JoinWithItemsRPC(byte[] stateData, int[] netIds, int joiningActorNum, PhotonMessageInfo info)
	{
		GamePlayer joiningPlayer = GamePlayer.GetGamePlayer(joiningActorNum);
		bool isAuthority = this.IsAuthority();
		if (isAuthority)
		{
			if (!this.IsValidAuthorityRPC(info.Sender))
			{
				return;
			}
		}
		else if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (joiningPlayer == null || (!isAuthority && this.GetAuthorityPlayer() != info.Sender) || (isAuthority && info.Sender.ActorNumber != joiningActorNum) || stateData == null || stateData.Length >= 15360 || joiningPlayer.DidJoinWithItems)
		{
			return;
		}
		if (!this.IsInZone())
		{
			return;
		}
		if (isAuthority)
		{
			joiningPlayer.DidJoinWithItems = true;
		}
		Action createItemsCallback = null;
		createItemsCallback = delegate()
		{
			try
			{
				GamePlayer joiningPlayer2 = joiningPlayer;
				joiningPlayer2.OnPlayerInitialized = (Action)Delegate.Remove(joiningPlayer2.OnPlayerInitialized, createItemsCallback);
				byte[] array = GZipStream.UncompressBuffer(stateData);
				int num = array.Length;
				using (MemoryStream memoryStream = new MemoryStream(array))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num2 = binaryReader.ReadInt32();
						if (num2 <= 4)
						{
							if (isAuthority || netIds.Length == num2)
							{
								if (isAuthority)
								{
									netIds = new int[num2];
									for (int i = 0; i < num2; i++)
									{
										netIds[i] = this.CreateNetId();
									}
								}
								for (int j = 0; j < num2; j++)
								{
									int netId = netIds[j];
									int entityTypeId = binaryReader.ReadInt32();
									long data = binaryReader.ReadInt64();
									int data2 = binaryReader.ReadInt32();
									long createData = binaryReader.ReadInt64();
									byte b = binaryReader.ReadByte();
									Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
									Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data2);
									float num3 = 10000f;
									if (vector.IsValid(num3) && quaternion.IsValid() && this.FactoryHasEntity(entityTypeId) && this.IsPositionInZone(vector))
									{
										bool flag = true;
										int num4 = 0;
										while (num4 < this.zoneComponents.Count && flag)
										{
											flag &= this.zoneComponents[num4].ValidateMigratedGameEntity(netId, entityTypeId, joiningPlayer.rig.transform.position, Quaternion.identity, createData, joiningActorNum);
											num4++;
										}
										if (flag)
										{
											GameEntityId gameEntityId = this.CreateAndInitItemLocal(netId, entityTypeId, joiningPlayer.rig.transform.position, Quaternion.identity, createData);
											bool isLeftHand = false;
											SnapJointType snapJointType;
											switch (b)
											{
											default:
												snapJointType = SnapJointType.None;
												break;
											case 1:
												snapJointType = SnapJointType.None;
												isLeftHand = true;
												break;
											case 2:
												snapJointType = SnapJointType.HandR;
												break;
											case 3:
												snapJointType = SnapJointType.HandL;
												isLeftHand = true;
												break;
											}
											if (snapJointType != SnapJointType.None)
											{
												this.SnapEntityLocal(gameEntityId, isLeftHand, vector, quaternion, (int)snapJointType, joiningPlayer.rig.OwningNetPlayer);
											}
											else
											{
												this.GrabEntityOnCreate(gameEntityId, isLeftHand, vector, quaternion, joiningPlayer.rig.OwningNetPlayer);
											}
										}
									}
								}
								if (isAuthority)
								{
									this.photonView.RPC("JoinWithItemsRPC", 1, new object[]
									{
										stateData,
										netIds,
										joiningActorNum
									});
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		};
		if (joiningPlayer.AdditionalDataInitialized)
		{
			createItemsCallback.Invoke();
			return;
		}
		GamePlayer joiningPlayer3 = joiningPlayer;
		joiningPlayer3.OnPlayerInitialized = (Action)Delegate.Combine(joiningPlayer3.OnPlayerInitialized, createItemsCallback);
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000CFF60 File Offset: 0x000CE160
	public bool FactoryHasEntity(int entityTypeId)
	{
		GameObject gameObject;
		return this.itemPrefabFactory.TryGetValue(entityTypeId, ref gameObject);
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000CFF7C File Offset: 0x000CE17C
	public GameObject FactoryPrefabById(int entityTypeId)
	{
		GameObject result;
		if (this.itemPrefabFactory.TryGetValue(entityTypeId, ref result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000CFF9C File Offset: 0x000CE19C
	public bool PriceLookup(int entityTypeId, out int price)
	{
		if (this.priceLookupByEntityId.TryGetValue(entityTypeId, ref price))
		{
			return true;
		}
		price = -1;
		return false;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000CFFB4 File Offset: 0x000CE1B4
	private void ValidateThatNetIdIsNotAlreadyUsed(int netId, int newTypeId)
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (i < this.entities.Count && this.netIds[i] == netId)
			{
				this.entities[i] == null;
			}
		}
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000D0008 File Offset: 0x000CE208
	public GameEntityId CreateAndInitItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		GameEntity gameEntity = this.CreateItemLocal(netId, entityTypeId, position, rotation);
		if (gameEntity == null)
		{
			return GameEntityId.Invalid;
		}
		this.InitItemLocal(gameEntity, createData);
		return gameEntity.id;
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000D0040 File Offset: 0x000CE240
	public GameEntity CreateItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation)
	{
		this.nextNetId = Mathf.Max(netId + 1, this.nextNetId);
		GameObject gameObject;
		if (!this.itemPrefabFactory.TryGetValue(entityTypeId, ref gameObject))
		{
			return null;
		}
		if (!this.createdItemTypeCount.ContainsKey(entityTypeId))
		{
			this.createdItemTypeCount[entityTypeId] = 0;
		}
		if (this.createdItemTypeCount[entityTypeId] > 100)
		{
			return null;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int num = dictionary[entityTypeId];
		dictionary[entityTypeId] = num + 1;
		GameEntity componentInChildren = Object.Instantiate<GameObject>(gameObject, position, rotation).GetComponentInChildren<GameEntity>();
		this.AddGameEntity(netId, componentInChildren);
		componentInChildren.Create(this, entityTypeId);
		return componentInChildren;
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000D00DC File Offset: 0x000CE2DC
	public void InitItemLocal(GameEntity entity, long createData)
	{
		entity.Init(createData);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].OnCreateGameEntity(entity);
		}
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000D0118 File Offset: 0x000CE318
	public void RequestDestroyItem(GameEntityId entityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(entityId)))
		{
			this.netIdsForDelete.Add(this.GetNetIdFromEntityId(entityId));
		}
		this.DestroyItemLocal(entityId);
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000D0150 File Offset: 0x000CE350
	public void RequestDestroyItems(List<GameEntityId> entityIds)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < entityIds.Count; i++)
		{
			list.Add(this.GetNetIdFromEntityId(entityIds[i]));
		}
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("DestroyItemRPC", 0, new object[]
			{
				list.ToArray()
			});
		}
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000D01B8 File Offset: 0x000CE3B8
	[PunRPC]
	public void DestroyItemRPC(int[] entityNetId, PhotonMessageInfo info)
	{
		if (entityNetId == null || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.DestroyItem))
		{
			return;
		}
		for (int i = 0; i < entityNetId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, entityNetId[i]))
			{
				return;
			}
			this.DestroyItemLocal(this.GetEntityIdFromNetId(entityNetId[i]));
		}
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000D0208 File Offset: 0x000CE408
	public void DestroyItemLocal(GameEntityId entityId)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		if (!this.createdItemTypeCount.ContainsKey(gameEntity.typeId))
		{
			this.createdItemTypeCount[gameEntity.typeId] = 1;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int typeId = gameEntity.typeId;
		int num = dictionary[typeId];
		dictionary[typeId] = num - 1;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			gamePlayer.ClearGrabbedIfHeld(gameEntity.id);
			if (gamePlayer.IsLocal())
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id);
			}
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			gamePlayer2.ClearSnappedIfSnapped(gameEntity.id);
		}
		this.RemoveGameEntity(gameEntity);
		Object.Destroy(gameEntity.gameObject);
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000D02D0 File Offset: 0x000CE4D0
	public void RequestState(GameEntityId entityId, long newState)
	{
		if (this.IsAuthority())
		{
			this.RequestStateAuthority(entityId, newState);
			return;
		}
		this.photonView.RPC("RequestStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			newState
		});
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000D0324 File Offset: 0x000CE524
	private void RequestStateAuthority(GameEntityId entityId, long newState)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(entityId);
		if (!this.IsValidNetId(netIdFromEntityId))
		{
			return;
		}
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = newState;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(newState);
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000D038C File Offset: 0x000CE58C
	[PunRPC]
	public void RequestStateRPC(int entityNetId, long newState, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netStateLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		if (gameEntity == null || gameEntity.IsNull())
		{
			return;
		}
		bool flag = false;
		GRToolClub component = gameEntity.GetComponent<GRToolClub>();
		GRToolCollector component2 = gameEntity.GetComponent<GRToolCollector>();
		GRToolRevive component3 = gameEntity.GetComponent<GRToolRevive>();
		GRToolLantern component4 = gameEntity.GetComponent<GRToolLantern>();
		GRToolFlash component5 = gameEntity.GetComponent<GRToolFlash>();
		GRToolDirectionalShield component6 = gameEntity.GetComponent<GRToolDirectionalShield>();
		GRToolShieldGun component7 = gameEntity.GetComponent<GRToolShieldGun>();
		if (component == null && component2 == null && component3 == null && component4 == null && component5 == null && component6 == null && component7 == null)
		{
			flag = this.IsAuthorityPlayer(info.Sender);
		}
		bool flag2 = gamePlayer.IsHoldingEntity(entityIdFromNetId, false) || gamePlayer.IsHoldingEntity(entityIdFromNetId, true);
		bool flag3 = gameEntity.lastHeldByActorNumber == info.Sender.ActorNumber;
		if (!flag && (flag2 || flag3))
		{
			if (component4 != null)
			{
				flag = component4.CanChangeState(newState);
			}
			if (component5 != null)
			{
				flag = component5.CanChangeState(newState);
			}
			if (component != null || component2 != null || component3 != null || component6 != null || component7 != null)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			bool flag4 = gameEntity.snappedByActorNumber == gamePlayer.rig.OwningNetPlayer.ActorNumber;
			if (gameEntity.canHoldingPlayerUpdateState && flag2)
			{
				flag = true;
			}
			else if (gameEntity.canLastHoldingPlayerUpdateState && flag3)
			{
				flag = true;
			}
			else if (gameEntity.canSnapPlayerUpdateState && flag4)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.netIdsForState.Contains(entityNetId))
			{
				this.statesForState[this.netIdsForState.IndexOf(entityNetId)] = newState;
				return;
			}
			this.netIdsForState.Add(entityNetId);
			this.statesForState.Add(newState);
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000D0594 File Offset: 0x000CE794
	[PunRPC]
	public void ApplyStateRPC(int[] netId, long[] newState, PhotonMessageInfo info)
	{
		if (netId == null || newState == null || netId.Length != newState.Length || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netId[i]))
			{
				return;
			}
			GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(netId[i]);
			GameEntity gameEntity = this.entities[entityIdFromNetId.index];
			if (gameEntity != null)
			{
				gameEntity.SetState(newState[i]);
			}
		}
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000D060C File Offset: 0x000CE80C
	public void RequestGrabEntity(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
			this.photonView.RPC("RequestGrabEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(gameEntityId),
				isLeftHand,
				num
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000D0690 File Offset: 0x000CE890
	[PunRPC]
	public void RequestGrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!vector.IsValid(num) || !quaternion.IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !this.IsPlayerHandNearEntity(gamePlayer, entityNetId, isLeftHand, false, 16f) || this.IsValidEntity(gamePlayer.GetGameEntityId(isLeftHand)) || !gamePlayer.netGrabLimiter.CheckCallTime(Time.time) || gamePlayer.IsHoldingEntity(this, isLeftHand))
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(entityNetId));
		if (gameEntity == null)
		{
			return;
		}
		if (!this.ValidateGrab(gameEntity, info.Sender.ActorNumber, isLeftHand))
		{
			return;
		}
		this.photonView.RPC("GrabEntityRPC", 0, new object[]
		{
			entityNetId,
			isLeftHand,
			packedPosRot,
			info.Sender
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000D07A0 File Offset: 0x000CE9A0
	[PunRPC]
	public void GrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.GrabEntity))
		{
			return;
		}
		Vector3 localPosition;
		Quaternion localRotation;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
		float num = 10000f;
		if (!localPosition.IsValid(num) || !localRotation.IsValid() || localPosition.sqrMagnitude > 6400f)
		{
			return;
		}
		this.GrabEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer));
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000D0818 File Offset: 0x000CEA18
	private void GrabEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(grabbedByPlayer.ActorNumber), out rigContainer))
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		if (gameEntity == null)
		{
			return;
		}
		if (grabbedByPlayer == null)
		{
			return;
		}
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		if (grabbedByPlayer.IsLocal && gameEntity.heldByActorNumber == grabbedByPlayer.ActorNumber && gameEntity.heldByHandIndex == handIndex)
		{
			return;
		}
		this.TryDetachCompletely(gameEntity);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(grabbedByPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer2))
		{
			int num = gamePlayer2.FindHandIndex(gameEntityId);
			bool flag = gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
			gamePlayer2.ClearGrabbedIfHeld(gameEntityId);
			if (num != -1 && flag)
			{
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
		}
		Transform handTransform = GamePlayer.GetHandTransform(rigContainer.Rig, handIndex);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (grabbedByPlayer.IsLocal)
			{
				component.constraints = 126;
				component.isKinematic = false;
			}
			else
			{
				component.constraints = 0;
				component.isKinematic = true;
			}
		}
		gameEntity.transform.SetParent(handTransform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameEntity.heldByHandIndex = handIndex;
		gameEntity.lastHeldByActorNumber = gameEntity.heldByActorNumber;
		gamePlayer.SetGrabbed(gameEntityId, handIndex, this);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.SetGrabbed(gameEntityId, GamePlayer.GetHandIndex(isLeftHand));
			GamePlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		GameSnappable component2 = gameEntity.GetComponent<GameSnappable>();
		if (component2 != null && component2.snappedToJoint != null && component2.snappedToJoint.jointType != SnapJointType.None)
		{
			SuperInfectionSnapPoint superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, component2.snappedToJoint.jointType);
			if (superInfectionSnapPoint == null)
			{
				superInfectionSnapPoint = component2.snappedToJoint;
			}
			superInfectionSnapPoint.Unsnapped();
			component2.OnUnsnap();
			Action onUnsnapped = gameEntity.OnUnsnapped;
			if (onUnsnapped != null)
			{
				onUnsnapped.Invoke();
			}
			gameEntity.snappedByActorNumber = -1;
			gameEntity.snappedJoint = SnapJointType.None;
		}
		gameEntity.PlayCatchFx();
		Action onGrabbed = gameEntity.OnGrabbed;
		if (onGrabbed != null)
		{
			onGrabbed.Invoke();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, true);
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000D0A7A File Offset: 0x000CEC7A
	public void GrabEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.gamePlayer.DeleteGrabbedEntityLocal(GamePlayer.GetHandIndex(isLeftHand));
		}
		this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, grabbedByPlayer);
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000D0AB4 File Offset: 0x000CECB4
	public GameEntityId TryGrabLocal(Vector3 handPosition, bool isLeftHand, out Vector3 closestPointOnBoundingBox)
	{
		float num = 0.03f;
		float maxAdjustedGrabDistance = 0f;
		float num2 = 0.1f;
		float num3 = 0.25f;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		Vector3 rigidbodyVelocity = GTPlayer.Instance.RigidbodyVelocity;
		GameEntity gameEntity = null;
		float num4 = float.MaxValue;
		Vector3 vector = handPosition;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity2 = this.entities[i];
			if (this.ValidateGrab(gameEntity2, actorNumber, isLeftHand))
			{
				float num5 = 0.75f;
				float sqrMagnitude = (handPosition - gameEntity2.transform.position).sqrMagnitude;
				if (sqrMagnitude <= num5 * num5)
				{
					Vector3 vector2 = gameEntity2.GetVelocity() - rigidbodyVelocity;
					float magnitude = vector2.magnitude;
					float num6 = Mathf.Clamp(magnitude * num2, 0f, num3);
					Vector3 slopProjection = (magnitude > 0.2f) ? (vector2.normalized * num6) : Vector3.zero;
					maxAdjustedGrabDistance = Mathf.Max(num, gameEntity2.pickupRangeFromSurface);
					this.renderSearchList.Clear();
					gameEntity2.GetComponentsInChildren<MeshFilter>(false, this.renderSearchList);
					foreach (MeshFilter meshFilter in this.renderSearchList)
					{
						if (!(this.GetParentEntity<GameEntity>(meshFilter.transform) != gameEntity2))
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, meshFilter.transform, slopProjection, meshFilter.sharedMesh.bounds, num6, maxAdjustedGrabDistance, gameEntity2, ref num4, ref gameEntity, ref vector);
						}
					}
					this.renderSearchListSkinned.Clear();
					gameEntity2.GetComponentsInChildren<SkinnedMeshRenderer>(false, this.renderSearchListSkinned);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.renderSearchListSkinned)
					{
						if (!(this.GetParentEntity<GameEntity>(skinnedMeshRenderer.transform) != gameEntity2))
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, skinnedMeshRenderer.transform, slopProjection, skinnedMeshRenderer.localBounds, num6, maxAdjustedGrabDistance, gameEntity2, ref num4, ref gameEntity, ref vector);
						}
					}
					if (this.renderSearchList.Count == 0 && this.renderSearchListSkinned.Count == 0)
					{
						float num7 = Mathf.Sqrt(sqrMagnitude);
						if (num7 < num4)
						{
							num4 = num7;
							gameEntity = gameEntity2;
							vector = gameEntity2.transform.position;
						}
					}
				}
			}
		}
		closestPointOnBoundingBox = vector;
		if (!(gameEntity != null))
		{
			return GameEntityId.Invalid;
		}
		if (num4 > Mathf.Max(num, gameEntity.pickupRangeFromSurface))
		{
			return GameEntityId.Invalid;
		}
		return gameEntity.id;
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000D0D58 File Offset: 0x000CEF58
	[MethodImpl(256)]
	private static void _TryGrabLocal_TestBounds(Vector3 handPosition, Transform t, Vector3 slopProjection, Bounds bounds, float slopForSpeed, float maxAdjustedGrabDistance, GameEntity entity, ref float bestDist, ref GameEntity bestEntity, ref Vector3 closestPoint)
	{
		Vector3 vector = t.InverseTransformPoint(handPosition);
		Vector3 b = t.InverseTransformPoint(handPosition + slopProjection);
		bounds.extents != bounds.extents;
		Vector3 vector2;
		float num;
		Vector3 vector3;
		float num2;
		if (GameEntityManager.SegmentHitsBounds(bounds, vector, b, out vector2, out num))
		{
			vector3 = ((num <= 0f) ? Vector3.zero : t.TransformVector(vector - vector2));
			num2 = vector3.magnitude - slopForSpeed;
		}
		else
		{
			vector3 = t.TransformVector(vector - bounds.ClosestPoint(vector));
			num2 = vector3.magnitude;
		}
		num2 = Mathf.Max(0f, num2 - maxAdjustedGrabDistance);
		vector3 = vector3.normalized * num2;
		if (num2 < bestDist)
		{
			bestDist = num2;
			bestEntity = entity;
			closestPoint = handPosition - vector3;
		}
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000D0E20 File Offset: 0x000CF020
	private void DrawDebugStar(Vector3 position, float radius)
	{
		for (int i = 0; i < 20; i++)
		{
			Debug.DrawLine(position, position + Random.onUnitSphere * radius, Color.red, 10f);
		}
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000D0E5C File Offset: 0x000CF05C
	private static bool SegmentHitsBounds(Bounds bounds, Vector3 a, Vector3 b, out Vector3 hitPoint, out float distance)
	{
		hitPoint = default(Vector3);
		distance = float.MaxValue;
		Vector3 vector = b - a;
		float magnitude = vector.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			if (bounds.Contains(a))
			{
				distance = 0f;
				hitPoint = a;
				return true;
			}
			return false;
		}
		else
		{
			Ray ray;
			ray..ctor(a, vector / magnitude);
			if (bounds.IntersectRay(ray, ref distance) && distance <= magnitude)
			{
				hitPoint = a + ray.direction * distance;
				return true;
			}
			return false;
		}
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000D0EEC File Offset: 0x000CF0EC
	public bool GetEntitiesWithComponentInRadius<T>(Vector3 center, float radius, bool checkRootOnly, List<T> nearbyEntities)
	{
		float num = radius * radius;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null))
			{
				T t;
				if (checkRootOnly)
				{
					t = gameEntity.GetComponent<T>();
				}
				else
				{
					t = gameEntity.GetComponentInChildren<T>();
				}
				if (t != null && (this.entities[i].transform.position - center).sqrMagnitude < num)
				{
					nearbyEntities.Add(t);
				}
			}
		}
		return nearbyEntities.Count > 0;
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000D0F80 File Offset: 0x000CF180
	private bool ValidateGrab(GameEntity gameEntity, int playerActorNumber, bool isLeftHand)
	{
		if (gameEntity == null || !gameEntity.pickupable)
		{
			return false;
		}
		if (gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != playerActorNumber)
		{
			return false;
		}
		if (gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != playerActorNumber && GamePlayer.GetGamePlayer(gameEntity.heldByActorNumber) != null)
		{
			return false;
		}
		if (gameEntity.snappedByActorNumber != -1 && gameEntity.snappedByActorNumber != playerActorNumber && GamePlayer.GetGamePlayer(gameEntity.snappedByActorNumber) != null)
		{
			return false;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && !component.CanGrabWithHand(isLeftHand))
		{
			return false;
		}
		if (this.IsValidEntity(gameEntity.attachedToEntityId))
		{
			GameEntity gameEntity2 = this.GetGameEntity(gameEntity.attachedToEntityId);
			if (gameEntity2 != null)
			{
				if (gameEntity2.snappedByActorNumber != -1 && gameEntity2.snappedByActorNumber != playerActorNumber && GamePlayer.GetGamePlayer(gameEntity2.snappedByActorNumber) != null)
				{
					return false;
				}
				GameSnappable component2 = gameEntity2.GetComponent<GameSnappable>();
				if (component2 != null && !component2.CanGrabWithHand(isLeftHand))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000D109C File Offset: 0x000CF29C
	private T GetParentEntity<T>(Transform transform) where T : MonoBehaviour
	{
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000D10DC File Offset: 0x000CF2DC
	public void RequestThrowEntity(GameEntityId entityId, bool isLeftHand, Vector3 headPosition, Vector3 velocity, Vector3 angVelocity)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 vector = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			Vector3 vector2 = gameEntity.transform.TransformPoint(component.centerOfMass);
			Vector3 vector3 = vector2 - headPosition;
			float magnitude = vector3.magnitude;
			if (magnitude > 0f)
			{
				vector3 /= magnitude;
				RaycastHit raycastHit;
				if (Physics.SphereCast(headPosition, 0.05f, vector3, ref raycastHit, magnitude + 0.1f, 513, 1))
				{
					component.GetComponentsInChildren<Collider>(this._collidersList);
					Vector3 vector4 = component.position + -raycastHit.normal * 1000f;
					float num = float.MaxValue;
					bool flag = false;
					Plane plane;
					plane..ctor(raycastHit.normal, raycastHit.point);
					foreach (Collider collider in this._collidersList)
					{
						if (collider.enabled && !collider.isTrigger)
						{
							Vector3 vector5 = collider.ClosestPoint(vector4);
							float num2 = Mathf.Abs(plane.GetDistanceToPoint(vector5));
							if (num2 < num)
							{
								num = num2;
								flag = true;
							}
						}
					}
					if (flag)
					{
						vector += raycastHit.normal * num;
					}
					else
					{
						float num3 = Mathf.Max(raycastHit.distance - 0.2f, 0f);
						Vector3 vector6 = headPosition + vector3 * num3;
						vector += vector6 - vector2;
					}
				}
			}
		}
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.ThrowEntityLocal(entityId, isLeftHand, vector, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			this.photonView.RPC("RequestThrowEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(entityId),
				isLeftHand,
				vector,
				rotation,
				velocity,
				angVelocity
			});
			PhotonNetwork.SendAllOutgoingCommands();
		}
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000D1334 File Offset: 0x000CF534
	[PunRPC]
	public void RequestThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f && this.IsPositionInZone(position))
					{
						GamePlayer gamePlayer;
						if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netThrowLimiter.CheckCallTime(Time.time))
						{
							return;
						}
						this.photonView.RPC("ThrowEntityRPC", 0, new object[]
						{
							entityNetId,
							isLeftHand,
							position,
							rotation,
							velocity,
							angVelocity,
							info.Sender,
							info.SentServerTime
						});
						PhotonNetwork.SendAllOutgoingCommands();
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000D1458 File Offset: 0x000CF658
	[PunRPC]
	public void ThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownByPlayer, double throwTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f)
					{
						NetPlayer netPlayer = NetPlayer.Get(thrownByPlayer);
						if (netPlayer.IsLocal && !this.IsAuthority())
						{
							return;
						}
						this.ThrowEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, velocity, angVelocity, netPlayer);
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000D1500 File Offset: 0x000CF700
	private void ThrowEntityLocal(GameEntityId entityId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		if (entityId.index < 0 || entityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[entityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (thrownByPlayer == null)
		{
			return;
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.constraints = 0;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GamePlayer gamePlayerRef = rigContainer.Rig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearGrabbedIfHeld(entityId);
				gamePlayerRef.ClearSnappedIfSnapped(entityId);
			}
		}
		gameEntity.PlayThrowFx();
		Action onReleased = gameEntity.OnReleased;
		if (onReleased != null)
		{
			onReleased.Invoke();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, false);
		GRBadge component2 = gameEntity.GetComponent<GRBadge>();
		if (component2 != null)
		{
			GRPlayer grplayer = GRPlayer.Get(thrownByPlayer.ActorNumber);
			if (grplayer != null)
			{
				grplayer.AttachBadge(component2);
			}
		}
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000D169C File Offset: 0x000CF89C
	public void RequestSnapEntity(GameEntityId entityId, bool isLeftHand, SnapJointType jointType)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 position = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		if (!this.IsAuthority())
		{
			this.SnapEntityLocal(entityId, isLeftHand, position, rotation, (int)jointType, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		this.photonView.RPC("RequestSnapEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			isLeftHand,
			position,
			rotation,
			(int)jointType
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000D1744 File Offset: 0x000CF944
	[PunRPC]
	public void RequestSnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid() && this.IsPositionInZone(position))
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("SnapEntityRPC", 0, new object[]
				{
					entityNetId,
					isLeftHand,
					position,
					rotation,
					jointType,
					info.Sender,
					info.SentServerTime
				});
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000D1834 File Offset: 0x000CFA34
	[PunRPC]
	public void SnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, Player thrownByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				if (!this.IsAuthority() && thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.SnapEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, jointType, NetPlayer.Get(thrownByPlayer));
				return;
			}
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000D18B4 File Offset: 0x000CFAB4
	private void SnapEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, NetPlayer snappedByPlayer)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (snappedByPlayer == null)
		{
			return;
		}
		if (snappedByPlayer.IsLocal && gameEntity.heldByActorNumber != snappedByPlayer.ActorNumber && gameEntity.lastHeldByActorNumber == snappedByPlayer.ActorNumber)
		{
			return;
		}
		GamePlayer gamePlayer = null;
		this.TryDetachCompletely(gameEntity);
		SuperInfectionSnapPoint superInfectionSnapPoint;
		if (jointType == 64)
		{
			gameEntity.GetComponent<GameSnappable>();
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
		}
		else
		{
			gamePlayer = GamePlayer.GetGamePlayer(snappedByPlayer.ActorNumber);
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
			int num = -1;
			if (jointType == 1)
			{
				num = GamePlayer.GetHandIndex(true);
			}
			if (jointType == 4)
			{
				num = GamePlayer.GetHandIndex(false);
			}
			if (jointType == 128)
			{
				num = GamePlayer.GetHandIndex(true);
			}
			if (jointType == 256)
			{
				num = GamePlayer.GetHandIndex(false);
			}
			if (num != -1)
			{
				gamePlayer.SetSnapped(gameEntityId, num, this);
			}
		}
		if (superInfectionSnapPoint == null)
		{
			return;
		}
		if (superInfectionSnapPoint.HasSnapped())
		{
			GameEntity snappedEntity = superInfectionSnapPoint.GetSnappedEntity();
			snappedEntity.transform.SetParent(null);
			snappedEntity.transform.SetLocalPositionAndRotation(position, rotation);
			Rigidbody component = snappedEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.constraints = 0;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = Vector3.up * 5f;
			}
			snappedEntity.heldByActorNumber = -1;
			snappedEntity.heldByHandIndex = -1;
			snappedEntity.snappedByActorNumber = -1;
			snappedEntity.snappedJoint = SnapJointType.None;
			snappedEntity.PlayThrowFx();
			Action onReleased = snappedEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased.Invoke();
			}
		}
		superInfectionSnapPoint.Snapped(gameEntity);
		gameEntity.transform.SetParent(superInfectionSnapPoint.transform);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component2 = gameEntity.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.isKinematic = true;
		}
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		GameSnappable component3 = gameEntity.GetComponent<GameSnappable>();
		if (component3 != null)
		{
			component3.GetSnapOffset((SnapJointType)jointType, out zero, out identity);
		}
		gameEntity.transform.localPosition = zero;
		gameEntity.transform.localRotation = identity;
		gameEntity.snappedByActorNumber = snappedByPlayer.ActorNumber;
		gameEntity.snappedJoint = (SnapJointType)jointType;
		if (component3 != null)
		{
			component3.OnSnap();
		}
		Action onSnapped = gameEntity.OnSnapped;
		if (onSnapped != null)
		{
			onSnapped.Invoke();
		}
		gameEntity.PlaySnapFx();
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000D1B1C File Offset: 0x000CFD1C
	public void SnapEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000D1B30 File Offset: 0x000CFD30
	private void TryUnsnapLocal(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(gameEntity.snappedByActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.ClearSnappedIfSnapped(gameEntity.id);
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && component.snappedToJoint != null && component.snappedToJoint.jointType != SnapJointType.None)
		{
			SuperInfectionSnapPoint superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, component.snappedToJoint.jointType);
			if (superInfectionSnapPoint == null)
			{
				superInfectionSnapPoint = component.snappedToJoint;
			}
			component.OnUnsnap();
			superInfectionSnapPoint.Unsnapped();
			Action onUnsnapped = gameEntity.OnUnsnapped;
			if (onUnsnapped != null)
			{
				onUnsnapped.Invoke();
			}
		}
		gameEntity.snappedByActorNumber = -1;
		gameEntity.snappedJoint = SnapJointType.None;
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000D1BE0 File Offset: 0x000CFDE0
	public void RequestAttachEntity(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			this.AttachEntityLocal(entityId, attachToEntityId, slotId, localPosition, localRotation);
		}
		this.photonView.RPC("RequestAttachEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000D1C70 File Offset: 0x000CFE70
	public void RequestAttachEntityAuthority(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		this.photonView.RPC("AttachEntityRPC", 0, new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation,
			default(object),
			PhotonNetwork.Time
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000D1CFC File Offset: 0x000CFEFC
	[PunRPC]
	public void RequestAttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, PhotonMessageInfo info)
	{
		bool flag = !this.IsValidNetId(attachToEntityNetId);
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (localPosition.IsValid(num) && localRotation.IsValid())
			{
				if (!flag)
				{
					if (localPosition.sqrMagnitude > 4f || !this.IsEntityNearEntity(entityNetId, attachToEntityNetId, 16f))
					{
						return;
					}
				}
				else if (!this.IsPositionInZone(localPosition))
				{
					return;
				}
				GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(entityNetId);
				if (gameEntityFromNetId == null)
				{
					return;
				}
				GameDockable component = gameEntityFromNetId.GetComponent<GameDockable>();
				if (component == null)
				{
					return;
				}
				GameEntity gameEntityFromNetId2 = this.GetGameEntityFromNetId(attachToEntityNetId);
				if (gameEntityFromNetId2 != null)
				{
					GameDock component2 = gameEntityFromNetId2.GetComponent<GameDock>();
					if (component2 == null)
					{
						return;
					}
					if (!component2.CanDock(component))
					{
						return;
					}
				}
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId)) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("AttachEntityRPC", 0, new object[]
				{
					entityNetId,
					attachToEntityNetId,
					slotId,
					localPosition,
					localRotation,
					info.Sender,
					info.SentServerTime
				});
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000D1E60 File Offset: 0x000D0060
	[PunRPC]
	public void AttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, Player attachedByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId) && this.IsValidNetId(attachToEntityNetId) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (localPosition.IsValid(num) && localRotation.IsValid())
			{
				if (!this.IsAuthority() && attachedByPlayer != null && attachedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.AttachEntityLocal(this.GetEntityIdFromNetId(entityNetId), this.GetEntityIdFromNetId(attachToEntityNetId), slotId, localPosition, localRotation);
				return;
			}
		}
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000D1EE8 File Offset: 0x000D00E8
	private void AttachEntityLocal(GameEntityId gameEntityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		GameEntity gameEntity2 = this.entities[attachToEntityId.index];
		this.TryDetachCompletely(gameEntity);
		bool flag = gameEntity2 == null;
		Transform parent = (gameEntity2 == null) ? null : gameEntity2.transform;
		gameEntity.transform.SetParent(parent);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.attachedToEntityId = (flag ? GameEntityId.Invalid : gameEntity2.id);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = !flag;
			component.constraints = 0;
		}
		GameDockable component2 = gameEntity.GetComponent<GameDockable>();
		if (gameEntity2 != null)
		{
			Action onAttached = gameEntity.OnAttached;
			if (onAttached != null)
			{
				onAttached.Invoke();
			}
			GameDock component3 = gameEntity2.GetComponent<GameDock>();
			if (component3 != null)
			{
				component3.OnDock(gameEntity, gameEntity2);
				if (component2 != null)
				{
					component2.OnDock(gameEntity, gameEntity2);
				}
			}
		}
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000D2008 File Offset: 0x000D0208
	private void TryDetachLocal(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return;
		}
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			GameEntity gameEntity2 = this.entities[gameEntity.attachedToEntityId.index];
			if (gameEntity2 != null)
			{
				GameDock component = gameEntity2.GetComponent<GameDock>();
				if (component != null)
				{
					component.OnUndock(gameEntity, gameEntity2);
					GameDockable component2 = gameEntity.GetComponent<GameDockable>();
					if (component2 != null)
					{
						component2.OnUndock(gameEntity, gameEntity2);
					}
				}
			}
		}
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			Action onDetached = gameEntity.OnDetached;
			if (onDetached != null)
			{
				onDetached.Invoke();
			}
		}
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000D20AF File Offset: 0x000D02AF
	public void TryDetachCompletely(GameEntity gameEntity)
	{
		this.TryRemoveFromHandLocal(gameEntity);
		this.TryUnsnapLocal(gameEntity);
		this.TryDetachLocal(gameEntity);
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000D20C8 File Offset: 0x000D02C8
	private void TryRemoveFromHandLocal(GameEntity gameEntity)
	{
		GameEntityId id = gameEntity.id;
		int heldByActorNumber = gameEntity.heldByActorNumber;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(heldByActorNumber);
		if (gamePlayer != null)
		{
			bool flag = heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
			gamePlayer.ClearGrabbedIfHeld(id);
			if (flag)
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(id);
			}
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased.Invoke();
			}
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000D1B1C File Offset: 0x000CFD1C
	public void AttachEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000D2138 File Offset: 0x000D0338
	public void RequestHit(GameHitData hit)
	{
		GameHittable gameComponent = this.GetGameComponent<GameHittable>(hit.hitEntityId);
		if (gameComponent == null)
		{
			return;
		}
		gameComponent.ApplyHit(hit);
		base.SendRPC("RequestHitRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(hit.hitEntityId),
			this.GetNetIdFromEntityId(hit.hitByEntityId),
			hit.hitTypeId,
			hit.hitEntityPosition,
			hit.hitPosition,
			hit.hitImpulse
		});
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000D21E0 File Offset: 0x000D03E0
	[PunRPC]
	public void RequestHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (entityPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitPosition.IsValid(num2))
			{
				float num3 = 10000f;
				if (hitImpulse.IsValid(num3) && this.IsValidAuthorityRPC(info.Sender, hittableNetId, entityPosition) && this.IsPositionInZone(hitPosition))
				{
					GamePlayer gamePlayer;
					if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netImpulseLimiter.CheckCallTime(Time.time))
					{
						return;
					}
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
					GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
					if (gameComponent == null)
					{
						return;
					}
					GameHitData hitData = new GameHitData
					{
						hitTypeId = hitTypeId,
						hitEntityId = entityIdFromNetId,
						hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
						hitEntityPosition = entityPosition,
						hitPosition = hitPosition,
						hitImpulse = hitImpulse
					};
					if (!gameComponent.IsHitValid(hitData))
					{
						return;
					}
					base.SendRPC("ApplyHitRPC", 0, new object[]
					{
						hittableNetId,
						hitByNetId,
						hitTypeId,
						entityPosition,
						hitPosition,
						hitImpulse,
						info.Sender
					});
					return;
				}
			}
		}
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000D2320 File Offset: 0x000D0520
	[PunRPC]
	public void ApplyHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, Player player, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (hitPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitImpulse.IsValid(num2) && this.IsValidClientRPC(info.Sender, hittableNetId, entityPosition) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.HitEntity) && player != null)
			{
				if (player.IsLocal)
				{
					return;
				}
				if (this.GetGameEntity(this.GetEntityIdFromNetId(hittableNetId)) == null)
				{
					return;
				}
				hitImpulse = Vector3.ClampMagnitude(hitImpulse, 100f);
				GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
				GameHitData hitData = new GameHitData
				{
					hitTypeId = hitTypeId,
					hitEntityId = entityIdFromNetId,
					hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
					hitEntityPosition = entityPosition,
					hitPosition = hitPosition,
					hitImpulse = hitImpulse,
					hitAmount = 0
				};
				GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(hitByNetId));
				GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
				if (gameEntity != null)
				{
					GameHitter component = gameEntity.GetComponent<GameHitter>();
					if (component != null)
					{
						hitData.hitAmount = component.CalcHitAmount((GameHitType)hitTypeId, gameComponent, gameEntity);
					}
				}
				if (gameComponent != null)
				{
					gameComponent.ApplyHit(hitData);
				}
				return;
			}
		}
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000D244C File Offset: 0x000D064C
	public bool IsPlayerHandNearEntity(GamePlayer player, int entityNetId, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && GameEntityManager.IsPlayerHandNearPosition(player, gameEntity.transform.position, isLeftHand, checkBothHands, acceptableRadius);
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x000D248C File Offset: 0x000D068C
	public static bool IsPlayerHandNearPosition(GamePlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		bool flag = true;
		if (player != null && player.rig != null)
		{
			if (isLeftHand || checkBothHands)
			{
				flag = ((worldPosition - player.rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
			}
			if (!isLeftHand || checkBothHands)
			{
				float sqrMagnitude = (worldPosition - player.rig.rightHandTransform.position).sqrMagnitude;
				flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
			}
		}
		return flag;
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x000D2518 File Offset: 0x000D0718
	public bool IsEntityNearEntity(int entityNetId, int otherEntityNetId, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(otherEntityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && this.IsEntityNearPosition(entityNetId, gameEntity.transform.position, acceptableRadius);
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000D2554 File Offset: 0x000D0754
	public bool IsEntityNearPosition(int entityNetId, Vector3 position, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && Vector3.SqrMagnitude(gameEntity.transform.position - position) < acceptableRadius * acceptableRadius;
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x0000D11E File Offset: 0x0000B31E
	public static bool ValidateDataType<T>(object obj, out T dataAsType)
	{
		if (obj is T)
		{
			dataAsType = (T)((object)obj);
			return true;
		}
		dataAsType = default(T);
		return false;
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000D2598 File Offset: 0x000D0798
	private void ClearZone(bool ignoreHeldGadgets = false)
	{
		if (ignoreHeldGadgets)
		{
			List<GameEntity> list = GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null);
			Vector3 position = VRRig.LocalRig.transform.position;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null || list[i].manager != this)
				{
					list.RemoveAt(i);
				}
				else
				{
					GameEntity gameEntity = list[i];
					bool flag = true;
					int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity.id);
					int num = 0;
					while (num < this.zoneComponents.Count && flag)
					{
						flag &= this.zoneComponents[num].ValidateMigratedGameEntity(netIdFromEntityId, gameEntity.typeId, position, Quaternion.identity, gameEntity.createData, actorNumber);
						num++;
					}
					if (!flag)
					{
						list.RemoveAt(i);
					}
				}
			}
			for (int j = this.entities.Count - 1; j >= 0; j--)
			{
				if (!(this.entities[j] == null) && !list.Contains(this.entities[j]))
				{
					this.DestroyItemLocal(this.entities[j].id);
				}
			}
			GamePlayerLocal.instance.gamePlayer.DidJoinWithItems = false;
		}
		else
		{
			for (int k = 0; k < this.entities.Count; k++)
			{
				if (this.entities[k] != null && this.entities[k].manager == this)
				{
					this.DestroyItemLocal(this.entities[k].id);
				}
			}
			GamePlayer gamePlayerRef = VRRig.LocalRig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearZone(this);
			}
		}
		for (int l = 0; l < this.entities.Count; l++)
		{
			if (this.entities[l] != null && this.entities[l].manager != this)
			{
				this.entities[l] = null;
			}
		}
		foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
		{
			GamePlayer component = vrrig.GetComponent<GamePlayer>();
			if (!component.IsLocal())
			{
				component.ClearZone(this);
			}
		}
		this.gameEntityData.Clear();
		for (int m = 0; m < this.zoneComponents.Count; m++)
		{
			this.zoneComponents[m].OnZoneClear(this.zoneClearReason);
		}
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000D2874 File Offset: 0x000D0A74
	public int SerializeGameState(int zoneId, byte[] bytes, int maxBytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].SerializeZoneData(binaryWriter);
		}
		GameEntityManager.tempEntitiesToSerialize.Clear();
		for (int j = 0; j < this.entities.Count; j++)
		{
			GameEntity gameEntity = this.entities[j];
			if (!(gameEntity == null))
			{
				GameEntityManager.tempEntitiesToSerialize.Add(gameEntity);
			}
		}
		binaryWriter.Write(GameEntityManager.tempEntitiesToSerialize.Count);
		for (int k = 0; k < GameEntityManager.tempEntitiesToSerialize.Count; k++)
		{
			GameEntity gameEntity2 = GameEntityManager.tempEntitiesToSerialize[k];
			if (!(gameEntity2 == null))
			{
				int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity2.id);
				binaryWriter.Write(netIdFromEntityId);
				binaryWriter.Write(gameEntity2.typeId);
				long num = BitPackUtils.PackWorldPosForNetwork(gameEntity2.transform.position);
				int num2 = BitPackUtils.PackQuaternionForNetwork(gameEntity2.transform.rotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
			}
		}
		for (int l = 0; l < GameEntityManager.tempEntitiesToSerialize.Count; l++)
		{
			GameEntity gameEntity3 = GameEntityManager.tempEntitiesToSerialize[l];
			if (!(gameEntity3 == null))
			{
				int netIdFromEntityId2 = this.GetNetIdFromEntityId(gameEntity3.id);
				binaryWriter.Write(netIdFromEntityId2);
				binaryWriter.Write(gameEntity3.createData);
				binaryWriter.Write(gameEntity3.GetState());
				int num3 = -1;
				GameEntity gameEntity4 = this.GetGameEntity(gameEntity3.attachedToEntityId);
				if (gameEntity4 != null)
				{
					num3 = this.GetNetIdFromEntityId(gameEntity4.id);
				}
				binaryWriter.Write(num3);
				if (num3 != -1)
				{
					long num4 = BitPackUtils.PackHandPosRotForNetwork(gameEntity3.transform.localPosition, gameEntity3.transform.localRotation);
					binaryWriter.Write(num4);
				}
				GameAgent component = gameEntity3.GetComponent<GameAgent>();
				bool flag = component != null;
				binaryWriter.Write(flag);
				if (flag)
				{
					long num5 = BitPackUtils.PackWorldPosForNetwork(component.navAgent.destination);
					binaryWriter.Write(num5);
					NetPlayer targetPlayer = component.targetPlayer;
					int num6 = (targetPlayer != null) ? targetPlayer.ActorNumber : -1;
					binaryWriter.Write(num6);
				}
				byte b = (byte)gameEntity3.entitySerialize.Count;
				binaryWriter.Write(b);
				for (int m = 0; m < (int)b; m++)
				{
					gameEntity3.entitySerialize[m].OnGameEntitySerialize(binaryWriter);
				}
				for (int n = 0; n < this.zoneComponents.Count; n++)
				{
					this.zoneComponents[n].SerializeZoneEntityData(binaryWriter, gameEntity3);
				}
			}
		}
		int count = GameEntityManager.tempRigs.Count;
		binaryWriter.Write(count);
		for (int num7 = 0; num7 < GameEntityManager.tempRigs.Count; num7++)
		{
			VRRig vrrig = GameEntityManager.tempRigs[num7];
			NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
			binaryWriter.Write(owningNetPlayer.ActorNumber);
			GamePlayer gamePlayerRef = vrrig.GamePlayerRef;
			bool flag2 = gamePlayerRef != null;
			binaryWriter.Write(flag2);
			if (flag2)
			{
				gamePlayerRef.SerializeNetworkState(binaryWriter, owningNetPlayer, this);
				for (int num8 = 0; num8 < this.zoneComponents.Count; num8++)
				{
					this.zoneComponents[num8].SerializeZonePlayerData(binaryWriter, owningNetPlayer.ActorNumber);
				}
			}
		}
		return (int)memoryStream.Position;
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000D2BE0 File Offset: 0x000D0DE0
	public void DeserializeTableState(byte[] bytes, int numBytes)
	{
		if (numBytes <= 0)
		{
			return;
		}
		GameEntityManager.tempAttachments.Clear();
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				for (int i = 0; i < this.zoneComponents.Count; i++)
				{
					this.zoneComponents[i].DeserializeZoneData(binaryReader);
				}
				int num = binaryReader.ReadInt32();
				for (int j = 0; j < num; j++)
				{
					int netId = binaryReader.ReadInt32();
					int entityTypeId = binaryReader.ReadInt32();
					long data = binaryReader.ReadInt64();
					int data2 = binaryReader.ReadInt32();
					Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(data);
					Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
					this.CreateItemLocal(netId, entityTypeId, position, rotation);
				}
				int k = 0;
				while (k < num)
				{
					int num2 = binaryReader.ReadInt32();
					long createData = binaryReader.ReadInt64();
					long state = binaryReader.ReadInt64();
					GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(num2);
					if (gameEntityFromNetId != null)
					{
						this.InitItemLocal(gameEntityFromNetId, createData);
						gameEntityFromNetId.SetState(state);
					}
					int num3 = binaryReader.ReadInt32();
					if (num3 == -1)
					{
						goto IL_14A;
					}
					long data3 = binaryReader.ReadInt64();
					if (!(gameEntityFromNetId == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(data3, out localPosition, out localRotation);
						GameEntityManager.tempAttachments.Add(new GameEntityManager.AttachmentData
						{
							entityNetId = num2,
							attachToEntityNetId = num3,
							localPosition = localPosition,
							localRotation = localRotation
						});
						goto IL_14A;
					}
					IL_200:
					k++;
					continue;
					IL_14A:
					if (binaryReader.ReadBoolean())
					{
						long data4 = binaryReader.ReadInt64();
						int playerID = binaryReader.ReadInt32();
						Vector3 destination = BitPackUtils.UnpackWorldPosFromNetwork(data4);
						GameAgent component = gameEntityFromNetId.GetComponent<GameAgent>();
						if (component != null)
						{
							if (component.IsOnNavMesh())
							{
								component.navAgent.destination = destination;
							}
							component.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
						}
					}
					byte b = binaryReader.ReadByte();
					for (int l = 0; l < (int)b; l++)
					{
						gameEntityFromNetId.entitySerialize[l].OnGameEntityDeserialize(binaryReader);
					}
					for (int m = 0; m < this.zoneComponents.Count; m++)
					{
						this.zoneComponents[m].DeserializeZoneEntityData(binaryReader, gameEntityFromNetId);
					}
					goto IL_200;
				}
				int num4 = binaryReader.ReadInt32();
				for (int n = 0; n < num4; n++)
				{
					int actorNumber = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						GamePlayer gamePlayer;
						GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer);
						GamePlayer.DeserializeNetworkState(binaryReader, gamePlayer, this);
						for (int num5 = 0; num5 < this.zoneComponents.Count; num5++)
						{
							this.zoneComponents[num5].DeserializeZonePlayerData(binaryReader, actorNumber);
						}
					}
				}
				for (int num6 = 0; num6 < GameEntityManager.tempAttachments.Count; num6++)
				{
					GameEntityManager.AttachmentData attachmentData = GameEntityManager.tempAttachments[num6];
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(attachmentData.entityNetId);
					GameEntityId entityIdFromNetId2 = this.GetEntityIdFromNetId(attachmentData.attachToEntityNetId);
					if (!(entityIdFromNetId == entityIdFromNetId2))
					{
						this.AttachEntityLocal(entityIdFromNetId, entityIdFromNetId2, 0, attachmentData.localPosition, attachmentData.localRotation);
					}
				}
			}
		}
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000D2F18 File Offset: 0x000D1118
	private void UpdateZoneState()
	{
		this.UpdateAuthority(GameEntityManager.tempRigs);
		if (this.IsAuthority())
		{
			this.UpdateClientsFromAuthority(GameEntityManager.tempRigs);
			this.UpdateZoneStateAuthority();
		}
		else
		{
			this.UpdateZoneStateClient();
		}
		for (int i = this.zoneStateData.zonePlayers.Count - 1; i >= 0; i--)
		{
			if (this.zoneStateData.zonePlayers[i] == null)
			{
				this.zoneStateData.zonePlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000D2F94 File Offset: 0x000D1194
	private void UpdateAuthority(List<VRRig> allRigs)
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (!this.IsAuthority())
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
		}
		else if (this.IsAuthority() && !this.IsInZone())
		{
			Player player = null;
			if (this.useRandomCheckForAuthority)
			{
				int num = 0;
				while (player == null)
				{
					if (num >= 10)
					{
						break;
					}
					num++;
					int num2 = Random.Range(0, allRigs.Count);
					VRRig vrrig = allRigs[num2];
					GamePlayer gamePlayer;
					if (GamePlayer.TryGetGamePlayer(vrrig, out gamePlayer) && !(gamePlayer.rig == null) && gamePlayer.rig.OwningNetPlayer != null && !gamePlayer.rig.isLocal && vrrig.zoneEntity.currentZone == this.zone)
					{
						player = gamePlayer.rig.OwningNetPlayer.GetPlayerRef();
					}
				}
			}
			else
			{
				for (int i = 0; i < allRigs.Count; i++)
				{
					VRRig vrrig2 = allRigs[i];
					GamePlayer gamePlayer2;
					if (GamePlayer.TryGetGamePlayer(vrrig2, out gamePlayer2) && !(gamePlayer2.rig == null) && gamePlayer2.rig.OwningNetPlayer != null && !gamePlayer2.rig.isLocal && vrrig2.zoneEntity.currentZone == this.zone)
					{
						player = gamePlayer2.rig.OwningNetPlayer.GetPlayerRef();
					}
				}
			}
			if (player != null && player != null)
			{
				this.guard.TransferOwnership(player, "");
			}
		}
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000D3114 File Offset: 0x000D1314
	private void UpdateClientsFromAuthority(List<VRRig> allRigs)
	{
		if (!this.IsInZone())
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			GameEntityManager.ZoneStateRequest zoneStateRequest = this.zoneStateData.zoneStateRequests[i];
			if (zoneStateRequest.player != null && zoneStateRequest.zone == this.zone)
			{
				this.SendZoneStateToPlayerOrTarget(zoneStateRequest.zone, zoneStateRequest.player, 2);
				zoneStateRequest.completed = true;
				this.zoneStateData.zoneStateRequests[i] = zoneStateRequest;
				this.zoneStateData.zoneStateRequests.RemoveAt(i);
				return;
			}
			this.zoneStateData.zoneStateRequests.RemoveAt(i);
			i--;
		}
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000D31CC File Offset: 0x000D13CC
	public void TestSerializeTableState()
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		int num = this.SerializeGameState((int)this.zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		Debug.LogFormat("Test Serialize Game State Buffer Size Uncompressed {0}", new object[]
		{
			num
		});
		Debug.LogFormat("Test Serialize Game State Buffer Size Compressed {0}", new object[]
		{
			array.Length
		});
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000D323C File Offset: 0x000D143C
	public static void ClearByteBuffer(byte[] buffer)
	{
		int num = buffer.Length;
		for (int i = 0; i < num; i++)
		{
			buffer[i] = 0;
		}
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000D3260 File Offset: 0x000D1460
	private void SendZoneStateToPlayerOrTarget(GTZone zone, Player player, RpcTarget target)
	{
		GameEntityManager.ClearByteBuffer(GameEntityManager.tempSerializeGameState);
		this.SerializeGameState((int)zone, GameEntityManager.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(GameEntityManager.tempSerializeGameState);
		byte[] array2 = new byte[512];
		int i = 0;
		int num = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			int num3 = Mathf.Min(512, num2 - i);
			Array.Copy(array, i, array2, 0, num3);
			if (player != null)
			{
				this.photonView.RPC("SendTableDataRPC", player, new object[]
				{
					num,
					num2,
					array2
				});
			}
			else
			{
				this.photonView.RPC("SendTableDataRPC", target, new object[]
				{
					num,
					num2,
					array2
				});
			}
			i += num3;
			num++;
		}
	}

	// Token: 0x0600279D RID: 10141 RVA: 0x000D333C File Offset: 0x000D153C
	[PunRPC]
	public void SendTableDataRPC(int packetNum, int totalBytes, byte[] bytes, PhotonMessageInfo info)
	{
		if (!this.IsAuthorityPlayer(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.SendTableData) || bytes == null || bytes.Length >= 15360)
		{
			return;
		}
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (packetNum == 0)
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
		}
		Array.Copy(bytes, 0, this.zoneStateData.recievedStateBytes, this.zoneStateData.numRecievedStateBytes, bytes.Length);
		this.zoneStateData.numRecievedStateBytes += bytes.Length;
		if (this.zoneStateData.numRecievedStateBytes >= totalBytes)
		{
			if (this.superInfectionManager != null && this.superInfectionManager.zoneSuperInfection == null)
			{
				this.PendingTableData = true;
				return;
			}
			this.ResolveTableData();
		}
	}

	// Token: 0x0600279E RID: 10142 RVA: 0x000D342C File Offset: 0x000D162C
	public void ResolveTableData()
	{
		this.PendingTableData = false;
		if (GameEntityManager.activeManager.IsNotNull() && GameEntityManager.activeManager != this)
		{
			GameEntityManager.activeManager.zoneClearReason = ZoneClearReason.LeaveZone;
			GameEntityManager.activeManager.ClearZone(false);
		}
		this.ClearZone(false);
		try
		{
			byte[] array = GZipStream.UncompressBuffer(this.zoneStateData.recievedStateBytes);
			int numBytes = array.Length;
			this.DeserializeTableState(array, numBytes);
			this.SetZoneState(GameEntityManager.ZoneState.Active);
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				this.zoneComponents[i].OnZoneInit();
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x0600279F RID: 10143 RVA: 0x000D34D8 File Offset: 0x000D16D8
	private void UpdateZoneStateAuthority()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone())
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entities.Count > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(false);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state > GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (this.IsInZone() && PhotonNetwork.InRoom)
		{
			this.SetZoneState(GameEntityManager.ZoneState.Active);
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				this.zoneComponents[i].OnZoneCreate();
			}
			for (int j = 0; j < this.zoneComponents.Count; j++)
			{
				this.zoneComponents[j].OnZoneInit();
			}
		}
	}

	// Token: 0x060027A0 RID: 10144 RVA: 0x000D35DC File Offset: 0x000D17DC
	private void UpdateZoneStateClient()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone())
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entities.Count > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(false);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - this.zoneStateData.stateStartTime > 1.0)
			{
				this.nextNetId = 1;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[]
				{
					(int)this.zone
				});
				this.JoinWithItems(GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null));
			}
		}
		else if (this.HasAuthority() && this.IsInZone() && !this.IsAuthority())
		{
			this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
			return;
		}
	}

	// Token: 0x060027A1 RID: 10145 RVA: 0x000D3708 File Offset: 0x000D1908
	private bool IsInZone()
	{
		bool flag = true;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag &= this.zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	// Token: 0x060027A2 RID: 10146 RVA: 0x000D3744 File Offset: 0x000D1944
	private bool ShouldClearZone()
	{
		bool flag = false;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag |= this.zoneComponents[i].ShouldClearZone();
		}
		return flag;
	}

	// Token: 0x060027A3 RID: 10147 RVA: 0x000D3780 File Offset: 0x000D1980
	private void SetZoneState(GameEntityManager.ZoneState newState)
	{
		if (newState == this.zoneStateData.state)
		{
			return;
		}
		this.zoneStateData.state = newState;
		this.zoneStateData.stateStartTime = Time.timeAsDouble;
		switch (this.zoneStateData.state)
		{
		case GameEntityManager.ZoneState.WaitingToEnterZone:
			if (!this.IsAuthority())
			{
				this.photonView.RPC("PlayerLeftZoneRPC", this.GetAuthorityPlayer(), Array.Empty<object>());
			}
			this.ClearZone(!this.ShouldClearZone() && this.zoneClearReason != ZoneClearReason.MigrateGameEntityZone);
			return;
		case GameEntityManager.ZoneState.WaitingToRequestState:
			break;
		case GameEntityManager.ZoneState.WaitingForState:
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
			return;
		case GameEntityManager.ZoneState.Active:
			if (!(GameEntityManager.activeManager == this))
			{
				GameEntityManager activeManager = GameEntityManager.activeManager;
				GameEntityManager.activeManager = this;
				GamePlayerLocal.instance.MigrateToEntityManager(this);
				if (activeManager.IsNotNull())
				{
					activeManager.zoneClearReason = ZoneClearReason.MigrateGameEntityZone;
					activeManager.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060027A4 RID: 10148 RVA: 0x000D388B File Offset: 0x000D1A8B
	public void DebugSendState()
	{
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
	}

	// Token: 0x060027A5 RID: 10149 RVA: 0x000D3894 File Offset: 0x000D1A94
	[PunRPC]
	public void RequestZoneStateRPC(int zoneId, PhotonMessageInfo info)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (zoneId != (int)this.zone || this.zoneStateData.zoneStateRequests == null)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer))
		{
			return;
		}
		if (!gamePlayer.newJoinZoneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			if (this.zoneStateData.zoneStateRequests[i].player == info.Sender)
			{
				return;
			}
		}
		this.zoneStateData.zoneStateRequests.Add(new GameEntityManager.ZoneStateRequest
		{
			player = info.Sender,
			zone = this.zone,
			completed = false
		});
	}

	// Token: 0x060027A6 RID: 10150 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060027A7 RID: 10151 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060027A8 RID: 10152 RVA: 0x000D3959 File Offset: 0x000D1B59
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.WriteDataPUN(stream, info);
		}
	}

	// Token: 0x060027A9 RID: 10153 RVA: 0x000D3976 File Offset: 0x000D1B76
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x060027AA RID: 10154 RVA: 0x000D3993 File Offset: 0x000D1B93
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
		this.zoneClearReason = ZoneClearReason.JoinZone;
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x060027AB RID: 10155 RVA: 0x000D39A3 File Offset: 0x000D1BA3
	void IMatchmakingCallbacks.OnLeftRoom()
	{
		this.zoneClearReason = ZoneClearReason.Disconnect;
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x060027AC RID: 10156 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060027AD RID: 10157 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060027AE RID: 10158 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x060027AF RID: 10159 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x060027B0 RID: 10160 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x060027B1 RID: 10161 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x060027B2 RID: 10162 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}

	// Token: 0x060027B3 RID: 10163 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060027B4 RID: 10164 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerLeftRoom(Player leavingPlayer)
	{
	}

	// Token: 0x060027B5 RID: 10165 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060027B6 RID: 10166 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060027B7 RID: 10167 RVA: 0x000D39B4 File Offset: 0x000D1BB4
	public void OnRigDeactivated(RigContainer container)
	{
		if (this != GameEntityManager.activeManager)
		{
			return;
		}
		GamePlayer component = container.GetComponent<GamePlayer>();
		if (this.IsAuthority())
		{
			this.RequestDestroyItems(component.HeldAndSnappedItems(this));
		}
		component.ResetData();
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x000D39F4 File Offset: 0x000D1BF4
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null || !toPlayer.IsLocal)
		{
			return;
		}
		if (fromPlayer == null || fromPlayer.InRoom)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(fromPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		foreach (GameEntityId gameEntityId in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId)))
			{
				this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId));
			}
			this.DestroyItemLocal(gameEntityId);
		}
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone.Invoke();
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x00002789 File Offset: 0x00000989
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x000D3AA0 File Offset: 0x000D1CA0
	public void RefreshRigList()
	{
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040032D8 RID: 13016
	private const string preLog = "[GameEntityManager]  ";

	// Token: 0x040032D9 RID: 13017
	private const string preErr = "[GameEntityManager]  ERROR!!!  ";

	// Token: 0x040032DA RID: 13018
	private const string preErrBeta = "[GameEntityManager]  ERROR!!!  (beta only log) ";

	// Token: 0x040032DB RID: 13019
	private const int MAX_STATE_BYTES = 15360;

	// Token: 0x040032DC RID: 13020
	private const int MAX_CHUNK_BYTES = 512;

	// Token: 0x040032DD RID: 13021
	public const float MAX_LOCAL_MAGNITUDE_SQ = 6400f;

	// Token: 0x040032DE RID: 13022
	public const float MAX_DISTANCE_FROM_HAND = 16f;

	// Token: 0x040032DF RID: 13023
	public const float MAX_ENTITY_DIST = 16f;

	// Token: 0x040032E0 RID: 13024
	public const float MAX_THROW_SPEED_SQ = 1600f;

	// Token: 0x040032E1 RID: 13025
	public const int MAX_ENTITY_COUNT_PER_TYPE = 100;

	// Token: 0x040032E2 RID: 13026
	public const int INVALID_ID = -1;

	// Token: 0x040032E3 RID: 13027
	public const int INVALID_INDEX = -1;

	// Token: 0x040032E4 RID: 13028
	private static List<GameEntityManager> allManagers = new List<GameEntityManager>(8);

	// Token: 0x040032E5 RID: 13029
	public GTZone zone;

	// Token: 0x040032E6 RID: 13030
	public PhotonView photonView;

	// Token: 0x040032E7 RID: 13031
	public RequestableOwnershipGuard guard;

	// Token: 0x040032E8 RID: 13032
	public Player prevAuthorityPlayer;

	// Token: 0x040032E9 RID: 13033
	public BoxCollider zoneLimit;

	// Token: 0x040032EA RID: 13034
	public bool useRandomCheckForAuthority;

	// Token: 0x040032EB RID: 13035
	public GameAgentManager gameAgentManager;

	// Token: 0x040032EC RID: 13036
	public GhostReactorManager ghostReactorManager;

	// Token: 0x040032ED RID: 13037
	public CustomMapsGameManager customMapsManager;

	// Token: 0x040032EE RID: 13038
	public SuperInfectionManager superInfectionManager;

	// Token: 0x040032EF RID: 13039
	private List<IGameEntityZoneComponent> zoneComponents;

	// Token: 0x040032F0 RID: 13040
	private List<GameEntity> entities;

	// Token: 0x040032F1 RID: 13041
	private List<GameEntityData> gameEntityData;

	// Token: 0x040032F2 RID: 13042
	public List<GameEntity> tempFactoryItems;

	// Token: 0x040032F5 RID: 13045
	private Dictionary<int, GameObject> itemPrefabFactory;

	// Token: 0x040032F6 RID: 13046
	private Dictionary<int, int> priceLookupByEntityId;

	// Token: 0x040032F7 RID: 13047
	private List<GameEntity> tempEntities = new List<GameEntity>();

	// Token: 0x040032F8 RID: 13048
	private List<int> netIdsForCreate;

	// Token: 0x040032F9 RID: 13049
	private List<int> entityTypeIdsForCreate;

	// Token: 0x040032FA RID: 13050
	private List<int> packedRotationsForCreate;

	// Token: 0x040032FB RID: 13051
	private List<long> packedPositionsForCreate;

	// Token: 0x040032FC RID: 13052
	private List<long> createDataForCreate;

	// Token: 0x040032FD RID: 13053
	private float createCooldown = 0.24f;

	// Token: 0x040032FE RID: 13054
	private float lastCreateSent;

	// Token: 0x040032FF RID: 13055
	private List<int> netIdsForDelete;

	// Token: 0x04003300 RID: 13056
	private float destroyCooldown = 0.25f;

	// Token: 0x04003301 RID: 13057
	private float lastDestroySent;

	// Token: 0x04003302 RID: 13058
	private List<int> netIdsForState;

	// Token: 0x04003303 RID: 13059
	private List<long> statesForState;

	// Token: 0x04003304 RID: 13060
	private float lastStateSent;

	// Token: 0x04003305 RID: 13061
	private float stateCooldown;

	// Token: 0x04003306 RID: 13062
	private Dictionary<int, int> netIdToIndex;

	// Token: 0x04003307 RID: 13063
	private NativeArray<int> netIds;

	// Token: 0x04003308 RID: 13064
	private Dictionary<int, int> createdItemTypeCount;

	// Token: 0x0400330A RID: 13066
	private ZoneClearReason zoneClearReason;

	// Token: 0x0400330B RID: 13067
	[NonSerialized]
	public Action<GameEntity> OnEntityRemoved;

	// Token: 0x0400330C RID: 13068
	[NonSerialized]
	public Action<GameEntity> OnEntityAdded;

	// Token: 0x0400330F RID: 13071
	private GameEntityManager.ZoneStateData zoneStateData;

	// Token: 0x04003310 RID: 13072
	private int nextNetId = 1;

	// Token: 0x04003311 RID: 13073
	public CallLimitersList<CallLimiter, GameEntityManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameEntityManager.RPC>();

	// Token: 0x04003312 RID: 13074
	private List<MeshFilter> renderSearchList = new List<MeshFilter>(32);

	// Token: 0x04003313 RID: 13075
	private List<SkinnedMeshRenderer> renderSearchListSkinned = new List<SkinnedMeshRenderer>(32);

	// Token: 0x04003314 RID: 13076
	private List<Collider> _collidersList = new List<Collider>(16);

	// Token: 0x04003315 RID: 13077
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x04003316 RID: 13078
	private static List<GameEntity> tempEntitiesToSerialize = new List<GameEntity>(512);

	// Token: 0x04003317 RID: 13079
	private static List<GameEntityManager.AttachmentData> tempAttachments = new List<GameEntityManager.AttachmentData>(512);

	// Token: 0x04003318 RID: 13080
	private static byte[] tempSerializeGameState = new byte[15360];

	// Token: 0x02000610 RID: 1552
	// (Invoke) Token: 0x060027C3 RID: 10179
	public delegate void ZoneStartEvent(GTZone zoneId);

	// Token: 0x02000611 RID: 1553
	// (Invoke) Token: 0x060027C7 RID: 10183
	public delegate void ZoneClearEvent(GTZone zoneId);

	// Token: 0x02000612 RID: 1554
	private enum ZoneState
	{
		// Token: 0x0400331A RID: 13082
		WaitingToEnterZone,
		// Token: 0x0400331B RID: 13083
		WaitingToRequestState,
		// Token: 0x0400331C RID: 13084
		WaitingForState,
		// Token: 0x0400331D RID: 13085
		Active
	}

	// Token: 0x02000613 RID: 1555
	private struct ZoneStateRequest
	{
		// Token: 0x0400331E RID: 13086
		public Player player;

		// Token: 0x0400331F RID: 13087
		public GTZone zone;

		// Token: 0x04003320 RID: 13088
		public bool completed;
	}

	// Token: 0x02000614 RID: 1556
	private class ZoneStateData
	{
		// Token: 0x04003321 RID: 13089
		public GameEntityManager.ZoneState state;

		// Token: 0x04003322 RID: 13090
		public double stateStartTime;

		// Token: 0x04003323 RID: 13091
		public List<GameEntityManager.ZoneStateRequest> zoneStateRequests;

		// Token: 0x04003324 RID: 13092
		public List<Player> zonePlayers;

		// Token: 0x04003325 RID: 13093
		public byte[] recievedStateBytes;

		// Token: 0x04003326 RID: 13094
		public int numRecievedStateBytes;
	}

	// Token: 0x02000615 RID: 1557
	public enum RPC
	{
		// Token: 0x04003328 RID: 13096
		CreateItem,
		// Token: 0x04003329 RID: 13097
		CreateItems,
		// Token: 0x0400332A RID: 13098
		DestroyItem,
		// Token: 0x0400332B RID: 13099
		ApplyState,
		// Token: 0x0400332C RID: 13100
		GrabEntity,
		// Token: 0x0400332D RID: 13101
		ThrowEntity,
		// Token: 0x0400332E RID: 13102
		SendTableData,
		// Token: 0x0400332F RID: 13103
		HitEntity
	}

	// Token: 0x02000616 RID: 1558
	private struct AttachmentData
	{
		// Token: 0x04003330 RID: 13104
		public int entityNetId;

		// Token: 0x04003331 RID: 13105
		public int attachToEntityNetId;

		// Token: 0x04003332 RID: 13106
		public Vector3 localPosition;

		// Token: 0x04003333 RID: 13107
		public Quaternion localRotation;
	}
}
