using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000159 RID: 345
[DefaultExecutionOrder(0)]
public class SuperInfectionManager : MonoBehaviour, IGameEntityZoneComponent, IFactoryItemProvider
{
	// Token: 0x06000943 RID: 2371 RVA: 0x00031C9E File Offset: 0x0002FE9E
	private void Awake()
	{
		GameEntityManager gameEntityManager = this.gameEntityManager;
		gameEntityManager.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(gameEntityManager.OnEntityRemoved, new Action<GameEntity>(this.OnEntityRemoved));
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00031CC7 File Offset: 0x0002FEC7
	public void OnEnableZoneSuperInfection(SuperInfection zone)
	{
		this.zoneSuperInfection = zone;
		if (this.PendingZoneInit)
		{
			this.PendingZoneInit = false;
			this.OnZoneInit();
		}
		if (this.gameEntityManager.PendingTableData)
		{
			this.gameEntityManager.ResolveTableData();
		}
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00031CFD File Offset: 0x0002FEFD
	private void OnEnable()
	{
		SuperInfectionManager.siManagerByZone.TryAdd(this.gameEntityManager.zone, this);
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00031D16 File Offset: 0x0002FF16
	private void OnDisable()
	{
		SuperInfectionManager.siManagerByZone.Remove(this.gameEntityManager.zone);
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00031D30 File Offset: 0x0002FF30
	public static SuperInfectionManager GetSIManagerForZone(GTZone targetZone)
	{
		SuperInfectionManager result;
		if (SuperInfectionManager.siManagerByZone.TryGetValue(targetZone, ref result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00002789 File Offset: 0x00000989
	public void OnZoneCreate()
	{
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00031D50 File Offset: 0x0002FF50
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].WriteDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].WriteDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.WriteDataPUN(stream, info);
		SuperInfectionManager.tempRigs.Clear();
		VRRigCache.Instance.GetActiveRigs(SuperInfectionManager.tempRigs);
		SuperInfectionManager.tempRigs2.Clear();
		for (int k = 0; k < SuperInfectionManager.tempRigs.Count; k++)
		{
			if (SuperInfectionManager.tempRigs[k].OwningNetPlayer != null)
			{
				SuperInfectionManager.tempRigs2.Add(SuperInfectionManager.tempRigs[k]);
			}
		}
		int count = SuperInfectionManager.tempRigs2.Count;
		stream.SendNext(count);
		for (int l = 0; l < count; l++)
		{
			SIPlayer siplayer = SIPlayer.Get(SuperInfectionManager.tempRigs2[l].OwningNetPlayer.ActorNumber);
			stream.SendNext(siplayer.ActorNr);
			siplayer.WriteDataPUN(stream, info);
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00031EA4 File Offset: 0x000300A4
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].ReadDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].ReadDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.ReadDataPUN(stream, info);
		int num = (int)stream.ReceiveNext();
		if (num < 0 || num > 10)
		{
			return;
		}
		for (int k = 0; k < num; k++)
		{
			SIPlayer siplayer = SIPlayer.Get((int)stream.ReceiveNext());
			if (siplayer == null)
			{
				return;
			}
			if (!siplayer.ReadDataPUN(stream, info))
			{
				return;
			}
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00031F8C File Offset: 0x0003018C
	void IGameEntityZoneComponent.SerializeZoneData(BinaryWriter writer)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].SerializeZoneData(writer);
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00031FD4 File Offset: 0x000301D4
	void IGameEntityZoneComponent.DeserializeZoneData(BinaryReader reader)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].DeserializeZoneData(reader);
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00002789 File Offset: 0x00000989
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00002789 File Offset: 0x00000989
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0003201C File Offset: 0x0003021C
	void IGameEntityZoneComponent.SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		siplayer.SerializeNetworkState(writer, siplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x00032048 File Offset: 0x00030248
	void IGameEntityZoneComponent.DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		SIPlayer player = SIPlayer.Get(actorNumber);
		SIPlayer.DeserializeNetworkStateAndBurn(reader, player, this);
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x00032064 File Offset: 0x00030264
	public bool IsZoneReady()
	{
		return NetworkSystem.Instance.InRoom && this.IsInSuperInfectionMode() && this.zoneSuperInfection.IsNotNull() && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone;
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x000320B0 File Offset: 0x000302B0
	public bool ShouldClearZone()
	{
		return GameMode.ActiveGameMode != null && GameMode.ActiveGameMode.GameType() != GameModeType.SuperInfect;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x000320D2 File Offset: 0x000302D2
	public bool IsInSuperInfectionMode()
	{
		return GameMode.ActiveGameMode != null && GameMode.ActiveGameMode.GameType() == GameModeType.SuperInfect;
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x000320F4 File Offset: 0x000302F4
	public void OnCreateGameEntity(GameEntity entity)
	{
		SIGadget component = entity.GetComponent<SIGadget>();
		if (component != null)
		{
			SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
			if (siplayer != null && !siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
			{
				siplayer.activePlayerGadgets.Add(entity.GetNetId());
			}
			SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)(entity.createData >> 32));
			siupgradeSet = component.FilterUpgradeNodes(siupgradeSet);
			component.ApplyUpgradeNodes(siupgradeSet);
			component.RefreshUpgradeVisuals(siupgradeSet);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.AddGadget(component);
			}
		}
		foreach (SuperInfectionSnapPoint snapPoint in entity.GetComponentsInChildren<SuperInfectionSnapPoint>(true))
		{
			this.RegisterSnapPoint(snapPoint);
		}
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x000321B7 File Offset: 0x000303B7
	public void OnZoneClear(ZoneClearReason reason)
	{
		SuperInfection superInfection = this.zoneSuperInfection;
		if (superInfection != null)
		{
			superInfection.OnZoneClear(reason);
		}
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			localPlayer.Reset();
		}
		SIPlayer.ClearPlayerCache();
		this.allSnapPoints.Clear();
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x000321EC File Offset: 0x000303EC
	public void OnZoneInit()
	{
		if (this.zoneSuperInfection == null)
		{
			this.PendingZoneInit = true;
			return;
		}
		SuperInfectionManager.activeSuperInfectionManager = this;
		if (this.gameEntityManager.IsAuthority())
		{
			this.TestSpawnGadget();
		}
		this.zoneSuperInfection.OnZoneInit();
		if (SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.progression.Init();
			if (this.progression.ClientReady)
			{
				SIPlayer.SetAndBroadcastProgression();
			}
			else
			{
				this.progression.OnClientReady += new Action(this.<OnZoneInit>g__WhenReady|39_0);
			}
		}
		this.allSnapPoints.Clear();
		foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
		{
			if (!(gameEntity == null))
			{
				foreach (SuperInfectionSnapPoint snapPoint in gameEntity.GetComponentsInChildren<SuperInfectionSnapPoint>(true))
				{
					this.RegisterSnapPoint(snapPoint);
				}
			}
		}
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x000322FC File Offset: 0x000304FC
	public void RegisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		List<SuperInfectionSnapPoint> list;
		if (!this.allSnapPoints.TryGetValue(snapPoint.jointType, ref list))
		{
			list = (this.allSnapPoints[snapPoint.jointType] = new List<SuperInfectionSnapPoint>());
		}
		list.Add(snapPoint);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00032340 File Offset: 0x00030540
	public void UnregisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		if (this.allSnapPoints.ContainsKey(snapPoint.jointType))
		{
			this.allSnapPoints[snapPoint.jointType].Remove(snapPoint);
			if (this.allSnapPoints[snapPoint.jointType].Count == 0)
			{
				this.allSnapPoints.Remove(snapPoint.jointType);
			}
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x000323A2 File Offset: 0x000305A2
	public IEnumerable<SuperInfectionSnapPoint> GetPoints(SnapJointType jointType)
	{
		foreach (KeyValuePair<SnapJointType, List<SuperInfectionSnapPoint>> keyValuePair in this.allSnapPoints)
		{
			if ((keyValuePair.Key & jointType) != SnapJointType.None)
			{
				foreach (SuperInfectionSnapPoint superInfectionSnapPoint in keyValuePair.Value)
				{
					yield return superInfectionSnapPoint;
				}
				List<SuperInfectionSnapPoint>.Enumerator enumerator2 = default(List<SuperInfectionSnapPoint>.Enumerator);
			}
		}
		Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator enumerator = default(Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x000323BC File Offset: 0x000305BC
	public SuperInfectionSnapPoint FindNearestSnapPoint(SnapJointType jointType, Vector3 origin, float maxDist, bool includeOccupied = false)
	{
		SuperInfectionSnapPoint result = null;
		float num = maxDist * maxDist;
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.GetPoints(jointType))
		{
			if (!(superInfectionSnapPoint == null) && superInfectionSnapPoint.isActiveAndEnabled && (includeOccupied || !superInfectionSnapPoint.HasSnapped()))
			{
				float sqrMagnitude = (superInfectionSnapPoint.transform.position - origin).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = superInfectionSnapPoint;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00032450 File Offset: 0x00030650
	public void CallRPC(SuperInfectionManager.ClientToAuthorityRPC clientToAuthorityRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToAuthorityRPC", this.gameEntityManager.GetAuthorityPlayer(), new object[]
			{
				(int)clientToAuthorityRPC,
				data
			});
		}
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x0003248C File Offset: 0x0003068C
	public void CallRPC(SuperInfectionManager.ClientToClientRPC clientToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToClientRPC", 1, new object[]
			{
				(int)clientToClientRPC,
				data
			});
		}
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x000324BE File Offset: 0x000306BE
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", 1, new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x000324F0 File Offset: 0x000306F0
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, int actorNr, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", NetworkSystem.Instance.GetNetPlayerByID(actorNr).GetPlayerRef(), new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0003253C File Offset: 0x0003073C
	[PunRPC]
	public void SIClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToAuthorityRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToAuthorityRPC(clientToAuthorityRPCEnum, data, info);
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00032598 File Offset: 0x00030798
	public void ProcessClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToAuthorityRPCEnum)
		{
		case 0:
		{
			if (data.Length != 4)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			int data2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out data2))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num2))
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out num3))
			{
				return;
			}
			if (num3 < 0 || num3 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SITouchscreenButton.SITouchscreenButtonType), (SITouchscreenButton.SITouchscreenButtonType)num))
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SICombinedTerminal.TerminalSubFunction), (SICombinedTerminal.TerminalSubFunction)num2))
			{
				return;
			}
			this.zoneSuperInfection.siTerminals[num3].TouchscreenButtonPressed((SITouchscreenButton.SITouchscreenButtonType)num, data2, info.Sender.ActorNumber, (SICombinedTerminal.TerminalSubFunction)num2);
			return;
		}
		case 1:
		{
			if (data.Length != 1)
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num4))
			{
				return;
			}
			if (num4 < 0 || num4 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			SICombinedTerminal sicombinedTerminal = this.zoneSuperInfection.siTerminals[num4];
			if (!siplayer.gamePlayer.rig.IsPositionInRange(sicombinedTerminal.transform.position, 3f))
			{
				return;
			}
			sicombinedTerminal.PlayerHandScanned(info.Sender.ActorNumber);
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int num5;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num5))
			{
				return;
			}
			if (num5 < 0 || num5 >= this.zoneSuperInfection.siDeposits.Length)
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (gameEntityFromNetId == null)
			{
				return;
			}
			SIResourceDeposit siresourceDeposit = this.zoneSuperInfection.siDeposits[num5];
			if ((gameEntityFromNetId.transform.position - siresourceDeposit.transform.position).IsLongerThan(3f))
			{
				return;
			}
			SIResource component = gameEntityFromNetId.GetComponent<SIResource>();
			if (component != null)
			{
				siresourceDeposit.ResourceDeposited(component);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToAuthorityRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data3;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data3))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToAuthorityRPC(info, rpcID2, data3);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00032860 File Offset: 0x00030A60
	[PunRPC]
	public void SIAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.authorityToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessAuthorityToClientRPC(authorityToClientRPCEnum, data, info);
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x000328BC File Offset: 0x00030ABC
	public void ProcessAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (authorityToClientRPCEnum)
		{
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (!gameEntityFromNetId)
			{
				return;
			}
			SIGadget component = gameEntityFromNetId.GetComponent<SIGadget>();
			if (component)
			{
				component.ProcessAuthorityToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessAuthorityToClientRPC(info, rpcID2, data2);
				return;
			}
			break;
		}
		case 5:
		{
			if (data.Length != 1)
			{
				return;
			}
			Vector3 position;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out position))
			{
				return;
			}
			if (SIPlayer.LocalPlayer)
			{
				SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(position);
				return;
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x000329CC File Offset: 0x00030BCC
	[PunRPC]
	public void SIClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToClientRPC(clientToClientRPCEnum, data, info);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00032A14 File Offset: 0x00030C14
	public void ProcessClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToClientRPCEnum)
		{
		case 0:
		{
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			if (data.Length != 8)
			{
				return;
			}
			int[] resourceArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[0], out resourceArray))
			{
				return;
			}
			int[] limitedDepositTimeArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[1], out limitedDepositTimeArray))
			{
				return;
			}
			bool[][] techTreeData;
			if (!GameEntityManager.ValidateDataType<bool[][]>(data[2], out techTreeData))
			{
				return;
			}
			int stashedQuests;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out stashedQuests))
			{
				return;
			}
			int stashedBonusPoints;
			if (!GameEntityManager.ValidateDataType<int>(data[4], out stashedBonusPoints))
			{
				return;
			}
			int bonusProgress;
			if (!GameEntityManager.ValidateDataType<int>(data[5], out bonusProgress))
			{
				return;
			}
			int[] currentQuestIds;
			if (!GameEntityManager.ValidateDataType<int[]>(data[6], out currentQuestIds))
			{
				return;
			}
			int[] currentQuestProgresses;
			if (!GameEntityManager.ValidateDataType<int[]>(data[7], out currentQuestProgresses))
			{
				return;
			}
			siplayer.UpdateProgression(resourceArray, limitedDepositTimeArray, techTreeData, stashedQuests, stashedBonusPoints, bonusProgress, currentQuestIds, currentQuestProgresses);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.RefreshStations(info.Sender.ActorNumber);
				return;
			}
			break;
		}
		case 1:
		{
			if (data.Length != 5)
			{
				return;
			}
			if (SIPlayer.Get(info.Sender.ActorNumber) == null)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			Vector3 velocity;
			if (GameEntityManager.ValidateDataType<Vector3>(data[1], out velocity))
			{
				float num = 10000f;
				if (velocity.IsValid(num))
				{
					Vector3 angVelocity;
					if (GameEntityManager.ValidateDataType<Vector3>(data[2], out angVelocity))
					{
						num = 10000f;
						if (angVelocity.IsValid(num))
						{
							Vector3 targetPosition;
							if (GameEntityManager.ValidateDataType<Vector3>(data[3], out targetPosition))
							{
								num = 10000f;
								if (targetPosition.IsValid(num))
								{
									Quaternion targetRotation;
									if (!GameEntityManager.ValidateDataType<Quaternion>(data[4], out targetRotation) || !targetRotation.IsValid())
									{
										return;
									}
									GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
									if (gameEntityFromNetId == null)
									{
										return;
									}
									if (gameEntityFromNetId.heldByActorNumber != info.Sender.ActorNumber && gameEntityFromNetId.snappedByActorNumber != info.Sender.ActorNumber)
									{
										return;
									}
									SIGadgetDashYoyo component = gameEntityFromNetId.GetComponent<SIGadgetDashYoyo>();
									if (component == null)
									{
										return;
									}
									component.RemoteThrowYoYoTarget(velocity, angVelocity, targetPosition, targetRotation);
									return;
								}
							}
							return;
						}
					}
					return;
				}
			}
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToClientRPC(info, rpcID2, data2);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00032CC4 File Offset: 0x00030EC4
	[ContextMenu("Spawn Debug Object")]
	private void TestSpawnGadget()
	{
		this.testSpawner.Spawn(this.gameEntityManager);
	}

	// Token: 0x06000966 RID: 2406 RVA: 0x00032CD7 File Offset: 0x00030ED7
	public IEnumerable<GameEntity> GetFactoryItems()
	{
		return this.techTreeSO.SpawnableEntities;
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x00032CE4 File Offset: 0x00030EE4
	private void OnEntityRemoved(GameEntity entity)
	{
		SIGadget sigadget;
		entity.TryGetComponent<SIGadget>(ref sigadget);
		if (this.zoneSuperInfection != null && sigadget != null)
		{
			this.zoneSuperInfection.RemoveGadget(sigadget);
		}
		if (sigadget == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
		if (siplayer != null && siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
		{
			Debug.Log(string.Format("GadgetDebug: removing gadget grom list {0} {1}", siplayer.gameObject.name, entity.GetNetId()));
			siplayer.activePlayerGadgets.Remove(entity.GetNetId());
		}
	}

	// Token: 0x06000968 RID: 2408 RVA: 0x00032D8A File Offset: 0x00030F8A
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		if (entity.GetComponent<SIGadget>() == null)
		{
			return createData;
		}
		return createData >> 32 << 32 | (long)SIPlayer.LocalPlayer.ActorNr;
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x00032DB0 File Offset: 0x00030FB0
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		GameObject gameObject = this.gameEntityManager.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		if (gameObject.GetComponent<SIGadget>() == null)
		{
			return false;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (siplayer == null)
		{
			return false;
		}
		SIPlayer siplayer2 = SIPlayer.Get((int)(createData & (long)((ulong)-1)));
		if (siplayer != siplayer2)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < siplayer.activePlayerGadgets.Count; i++)
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
			if (((gameEntityFromNetId != null) ? gameEntityFromNetId.GetComponent<SIGadget>() : null) != null)
			{
				num++;
			}
		}
		if (num > siplayer.totalGadgetLimit)
		{
			return false;
		}
		bool flag = false;
		int num2 = 0;
		while (num2 < siplayer.CurrentProgression.techTreeData.Length && !flag)
		{
			for (int j = 0; j < siplayer.CurrentProgression.techTreeData[num2].Length; j++)
			{
				if (siplayer.CurrentProgression.techTreeData[num2][j] && siplayer.progressionSORef.IsValidNode(num2, j))
				{
					SITechTreeNode treeNode = siplayer.progressionSORef.GetTreeNode(num2, j);
					if (treeNode != null && treeNode.IsDispensableGadget && treeNode.unlockedGadgetPrefab.gameObject.name.GetStaticHash() == entityTypeId)
					{
						flag = true;
						break;
					}
				}
			}
			num2++;
		}
		return flag;
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x00032F18 File Offset: 0x00031118
	public void ClearPlayerGadgets(SIPlayer siPlayer)
	{
		for (int i = siPlayer.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			if (i < siPlayer.activePlayerGadgets.Count && siPlayer.activePlayerGadgets[i] >= 0)
			{
				GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siPlayer.activePlayerGadgets[i]);
				if (!(gameEntityFromNetId == null) && !(gameEntityFromNetId.id == GameEntityId.Invalid))
				{
					this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
				}
			}
		}
		siPlayer.activePlayerGadgets.Clear();
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00032FE0 File Offset: 0x000311E0
	[CompilerGenerated]
	private void <OnZoneInit>g__WhenReady|39_0()
	{
		this.progression.OnClientReady -= new Action(this.<OnZoneInit>g__WhenReady|39_0);
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x04000B3E RID: 2878
	private const string preLog = "[SuperInfectionManager]  ";

	// Token: 0x04000B3F RID: 2879
	private const string preErr = "[SuperInfectionManager]  ERROR!!!  ";

	// Token: 0x04000B40 RID: 2880
	public GameEntityManager gameEntityManager;

	// Token: 0x04000B41 RID: 2881
	public TestSpawnGadget testSpawner;

	// Token: 0x04000B42 RID: 2882
	public PhotonView photonView;

	// Token: 0x04000B43 RID: 2883
	public XSceneRef zoneSuperInfectionRef;

	// Token: 0x04000B44 RID: 2884
	[NonSerialized]
	public SuperInfection zoneSuperInfection;

	// Token: 0x04000B45 RID: 2885
	[SerializeField]
	private SITechTreeSO techTreeSO;

	// Token: 0x04000B46 RID: 2886
	[SerializeField]
	private SIProgression progression;

	// Token: 0x04000B47 RID: 2887
	public static SuperInfectionManager activeSuperInfectionManager;

	// Token: 0x04000B48 RID: 2888
	public static Dictionary<GTZone, SuperInfectionManager> siManagerByZone = new Dictionary<GTZone, SuperInfectionManager>();

	// Token: 0x04000B49 RID: 2889
	private static List<VRRig> tempRigs = new List<VRRig>(10);

	// Token: 0x04000B4A RID: 2890
	private static List<VRRig> tempRigs2 = new List<VRRig>(10);

	// Token: 0x04000B4B RID: 2891
	private readonly Dictionary<SnapJointType, List<SuperInfectionSnapPoint>> allSnapPoints = new Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>();

	// Token: 0x04000B4C RID: 2892
	private const float rpcProximityCheckRange = 3f;

	// Token: 0x04000B4D RID: 2893
	private bool PendingZoneInit;

	// Token: 0x04000B4E RID: 2894
	private bool PendingTableData;

	// Token: 0x0200015A RID: 346
	public enum ClientToAuthorityRPC
	{
		// Token: 0x04000B50 RID: 2896
		CombinedTerminalButtonPress,
		// Token: 0x04000B51 RID: 2897
		CombinedTerminalHandScan,
		// Token: 0x04000B52 RID: 2898
		ResourceDepositDeposited,
		// Token: 0x04000B53 RID: 2899
		CallEntityRPC,
		// Token: 0x04000B54 RID: 2900
		CallEntityRPCData
	}

	// Token: 0x0200015B RID: 347
	public enum AuthorityToClientRPC
	{
		// Token: 0x04000B56 RID: 2902
		TechPointGranted,
		// Token: 0x04000B57 RID: 2903
		ResourceDepositTechPointGranted,
		// Token: 0x04000B58 RID: 2904
		ResourceDepositTechPointRejected,
		// Token: 0x04000B59 RID: 2905
		CallEntityRPC,
		// Token: 0x04000B5A RID: 2906
		CallEntityRPCData,
		// Token: 0x04000B5B RID: 2907
		TriggerMonkeIdolDepositCelebration
	}

	// Token: 0x0200015C RID: 348
	public enum ClientToClientRPC
	{
		// Token: 0x04000B5D RID: 2909
		BroadcastProgression,
		// Token: 0x04000B5E RID: 2910
		LaunchDashYoyo,
		// Token: 0x04000B5F RID: 2911
		CallEntityRPC,
		// Token: 0x04000B60 RID: 2912
		CallEntityRPCData
	}
}
