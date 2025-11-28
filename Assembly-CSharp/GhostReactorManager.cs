using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200064D RID: 1613
[NetworkBehaviourWeaved(0)]
public class GhostReactorManager : NetworkComponent, IGameEntityZoneComponent
{
	// Token: 0x060028FC RID: 10492 RVA: 0x000DBAE4 File Offset: 0x000D9CE4
	protected override void Awake()
	{
		base.Awake();
		this.noiseEventManager = base.GetComponent<GRNoiseEventManager>();
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000DBAF8 File Offset: 0x000D9CF8
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x0007A757 File Offset: 0x00078957
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x060028FF RID: 10495 RVA: 0x000DBB06 File Offset: 0x000D9D06
	public bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x06002900 RID: 10496 RVA: 0x000DBB13 File Offset: 0x000D9D13
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002901 RID: 10497 RVA: 0x000DBB21 File Offset: 0x000D9D21
	private bool IsAuthorityPlayer(Player player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002902 RID: 10498 RVA: 0x000DBB2F File Offset: 0x000D9D2F
	private Player GetAuthorityPlayer()
	{
		return this.gameEntityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002903 RID: 10499 RVA: 0x000DBB3C File Offset: 0x000D9D3C
	public bool IsZoneActive()
	{
		return this.gameEntityManager.IsZoneActive();
	}

	// Token: 0x06002904 RID: 10500 RVA: 0x000DBB49 File Offset: 0x000D9D49
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.gameEntityManager.IsPositionInZone(pos);
	}

	// Token: 0x06002905 RID: 10501 RVA: 0x000DBB57 File Offset: 0x000D9D57
	public bool IsValidClientRPC(Player sender)
	{
		return this.gameEntityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002906 RID: 10502 RVA: 0x000DBB65 File Offset: 0x000D9D65
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002907 RID: 10503 RVA: 0x000DBB74 File Offset: 0x000D9D74
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002908 RID: 10504 RVA: 0x000DBB84 File Offset: 0x000D9D84
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002909 RID: 10505 RVA: 0x000DBB93 File Offset: 0x000D9D93
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x0600290A RID: 10506 RVA: 0x000DBBA1 File Offset: 0x000D9DA1
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x0600290B RID: 10507 RVA: 0x000DBBB0 File Offset: 0x000D9DB0
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000DBBC0 File Offset: 0x000D9DC0
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x0600290D RID: 10509 RVA: 0x000DBBCF File Offset: 0x000D9DCF
	public static GhostReactorManager Get(GameEntity gameEntity)
	{
		if (gameEntity == null || gameEntity.manager == null)
		{
			return null;
		}
		return gameEntity.manager.ghostReactorManager;
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x00002789 File Offset: 0x00000989
	public void RefreshShiftCredit()
	{
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x000DBBF8 File Offset: 0x000D9DF8
	[PunRPC]
	public void RefreshShiftCreditRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RefreshShiftCredit))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x000DBC5C File Offset: 0x000D9E5C
	public void SendMothershipId()
	{
		string mothershipId = MothershipClientContext.MothershipId;
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x000DBC64 File Offset: 0x000D9E64
	[PunRPC]
	public void SendMothershipIdRPC(string mothershipId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SendMothershipId))
		{
			return;
		}
		if (mothershipId.Length > 40)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (!grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			grplayer.mothershipId = mothershipId.Trim();
			ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
		}
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x000DBCEC File Offset: 0x000D9EEC
	public void RequestCollectItem(GameEntityId collectibleEntityId, GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestCollectItemRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x000DBD40 File Offset: 0x000D9F40
	public void RequestDepositCollectible(GameEntityId collectibleEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("ApplyCollectItemRPC", 0, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				-1,
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x000DBDB0 File Offset: 0x000D9FB0
	[PunRPC]
	public void RequestCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectibleEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestCollectItemLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsEntityNearEntity(collectibleEntityNetId, collectorEntityNetId, 16f))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyCollectItemRPC", 0, new object[]
			{
				collectibleEntityNetId,
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000DBE5C File Offset: 0x000DA05C
	[PunRPC]
	public void ApplyCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyCollectItem))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(collectingPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity2 != null)
			{
				GRToolCollector component2 = gameEntity2.GetComponent<GRToolCollector>();
				if (component2 != null && component2.tool != null)
				{
					component2.PerformCollection(component);
				}
			}
			else
			{
				ProgressionManager.Instance.DepositCore(component.type);
				this.ReportCoreCollection(grplayer, component.type);
				int count = this.reactor.vrRigs.Count;
				int coreValue = component.energyValue / 4;
				for (int i = 0; i < count; i++)
				{
					GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(coreValue);
				}
				grplayer.IncrementCoresCollectedPlayer(coreValue);
			}
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000DBFCC File Offset: 0x000DA1CC
	public void RequestApplySeedExtractorState(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		this.photonView.RPC("RequestApplySeedExtractorStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			coreCount,
			coresProcessedByOverdrive,
			researchPoints,
			coreProcessingPercentage,
			overdriveSupply
		});
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x000DC024 File Offset: 0x000DA224
	[PunRPC]
	public void RequestApplySeedExtractorStateRPC(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (info.Sender.ActorNumber != this.reactor.seedExtractor.CurrentPlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ApplySeedExtractorStateRPC", 0, new object[]
		{
			info.Sender.ActorNumber,
			coreCount,
			coresProcessedByOverdrive,
			researchPoints,
			coreProcessingPercentage,
			overdriveSupply
		});
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x000DC0E8 File Offset: 0x000DA2E8
	[PunRPC]
	public void ApplySeedExtractorStateRPC(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (this.reactor != null && this.reactor.seedExtractor != null)
		{
			this.reactor.seedExtractor.ApplyState(playerActorNumber, coreCount, coresProcessedByOverdrive, researchPoints, coreProcessingPercentage, overdriveSupply);
		}
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x000DC170 File Offset: 0x000DA370
	public void RequestDistillCollectible(GameEntityId collectibleEntityId, Player sender)
	{
		if (!this.IsValidAuthorityRPC(sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("DistillItemRPC", 0, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x000DC1D8 File Offset: 0x000DA3D8
	[PunRPC]
	public void DistillItemRPC(int collectibleEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.DistillItem))
		{
			return;
		}
		if (GRPlayer.Get(collectingPlayerActorNumber) == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			Debug.LogWarning("Warning - NOT IMPLEMENTED - Return validating inserting core for distillery.");
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000DC288 File Offset: 0x000DA488
	public void RequestChargeTool(GameEntityId collectorEntityId, GameEntityId targetToolId, int targetEnergyDelta = 0, bool useCollectorEnergy = true)
	{
		this.photonView.RPC("RequestChargeToolRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(targetToolId),
			targetEnergyDelta,
			useCollectorEnergy
		});
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000DC2EC File Offset: 0x000DA4EC
	[PunRPC]
	public void RequestChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, PhotonMessageInfo info)
	{
		GamePlayer player;
		if (!this.IsValidAuthorityRPC(info.Sender) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId) || !this.gameEntityManager.IsEntityNearEntity(collectorEntityNetId, targetToolNetId, 16f) || !GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out player) || !this.gameEntityManager.IsPlayerHandNearEntity(player, collectorEntityNetId, false, true, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(player, targetToolNetId, false, true, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestChargeToolLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyChargeToolRPC", 0, new object[]
			{
				collectorEntityNetId,
				targetToolNetId,
				targetEnergyDelta,
				useCollectorEnergy,
				info.Sender
			});
		}
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x000DC3F0 File Offset: 0x000DA5F0
	[PunRPC]
	public void ApplyChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, Player collectingPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyChargeTool) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId))
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(targetToolNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity != null && gameEntity2 != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				if (component != null && component.tool != null && component2 != null)
				{
					int num = (targetEnergyDelta != 0) ? targetEnergyDelta : 100;
					int num2 = Mathf.Max(component2.GetEnergyMax() - component2.energy, 0);
					int num3;
					if (!useCollectorEnergy)
					{
						num3 = Mathf.Min(num, num2);
						Debug.Log(string.Format("Apply SelfCharge {0}", num3));
					}
					else
					{
						num3 = Mathf.Min(Mathf.Min(component.tool.energy, num), num2);
					}
					if (num3 > 0)
					{
						if (useCollectorEnergy)
						{
							component.tool.SetEnergy(component.tool.energy - num3);
						}
						component2.RefillEnergy(num3, entityIdFromNetId);
						component.PlayChargeEffect(component2);
					}
				}
			}
		}
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x000DC563 File Offset: 0x000DA763
	public void RequestDepositCurrency(GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestDepositCurrencyRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000DC598 File Offset: 0x000DA798
	[PunRPC]
	public void RequestDepositCurrencyRPC(int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectorEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestDepositCurrencyLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
		this.gameEntityManager.GetGameEntity(entityIdFromNetId);
		GamePlayer player;
		if (GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out player) && this.gameEntityManager.IsPlayerHandNearEntity(player, collectorEntityNetId, false, true, 16f) && (grplayer.transform.position - this.reactor.currencyDepositor.transform.position).magnitude < 16f)
		{
			this.photonView.RPC("ApplyDepositCurrencyRPC", 0, new object[]
			{
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000DC694 File Offset: 0x000DA894
	[PunRPC]
	public void ApplyDepositCurrencyRPC(int collectorEntityNetId, int targetPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectorEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyDepositCurrency))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(targetPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				if (component != null && component.tool != null)
				{
					int energy = component.tool.energy;
					int energyDepositPerUse = component.energyDepositPerUse;
					if (energy >= energyDepositPerUse)
					{
						this.ReportCoreCollection(grplayer, ProgressionManager.CoreType.Core);
						int count = this.reactor.vrRigs.Count;
						int coreValue = energyDepositPerUse / 4;
						for (int i = 0; i < count; i++)
						{
							GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(coreValue);
						}
						grplayer.IncrementCoresCollectedPlayer(coreValue);
						int energy2 = energy - energyDepositPerUse;
						component.tool.SetEnergy(energy2);
						this.reactor.RefreshScoreboards();
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.Core);
						component.PlayChargeEffect(this.reactor.currencyDepositor);
					}
				}
			}
		}
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000DC7DE File Offset: 0x000DA9DE
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", 0, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition
		});
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000DC820 File Offset: 0x000DAA20
	[PunRPC]
	private void ApplyEnemyHitPlayerRPC(GhostReactor.EnemyType type, int entityNetId, Vector3 hitPosition, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidNetId(entityNetId))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.applyEnemyHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.OnEnemyHitPlayerInternal(type, entityIdFromNetId, grplayer, hitPosition);
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000DC884 File Offset: 0x000DAA84
	private void OnEnemyHitPlayerInternal(GhostReactor.EnemyType type, GameEntityId entityId, GRPlayer player, Vector3 hitPosition)
	{
		if (type == GhostReactor.EnemyType.Chaser || type == GhostReactor.EnemyType.Phantom || type == GhostReactor.EnemyType.Ranged || type == GhostReactor.EnemyType.CustomMapsEnemy)
		{
			player.OnPlayerHit(hitPosition, this, entityId);
			GameHitter component = this.gameEntityManager.GetGameEntity(entityId).GetComponent<GameHitter>();
			if (component != null)
			{
				component.ApplyHitToPlayer(player, hitPosition);
			}
		}
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000DC8CE File Offset: 0x000DAACE
	public void ReportLocalPlayerHit()
	{
		base.GetView.RPC("ReportLocalPlayerHitRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000DC8E8 File Offset: 0x000DAAE8
	[PunRPC]
	private void ReportLocalPlayerHitRPC(PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.reportLocalHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this);
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000DC92C File Offset: 0x000DAB2C
	public void RequestPlayerRevive(GRReviveStation reviveStation, GRPlayer player)
	{
		if ((NetworkSystem.Instance.InRoom && this.IsAuthority()) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("ApplyPlayerRevivedRPC", 0, new object[]
			{
				reviveStation.Index,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber
			});
		}
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000DC99C File Offset: 0x000DAB9C
	[PunRPC]
	private void ApplyPlayerRevivedRPC(int reviveStationIndex, int playerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyPlayerRevived))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (reviveStationIndex < 0 || reviveStationIndex >= this.reactor.reviveStations.Count)
		{
			return;
		}
		GRReviveStation grreviveStation = this.reactor.reviveStations[reviveStationIndex];
		if (grreviveStation == null)
		{
			return;
		}
		grreviveStation.RevivePlayer(grplayer);
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000DCA14 File Offset: 0x000DAC14
	public void RequestPlayerStateChange(GRPlayer player, GRPlayer.GRPlayerState newState)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("PlayerStateChangeRPC", 0, new object[]
			{
				PhotonNetwork.LocalPlayer.ActorNumber,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
				(int)newState
			});
			return;
		}
		player.ChangePlayerState(newState, this);
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000DCA88 File Offset: 0x000DAC88
	[PunRPC]
	private void PlayerStateChangeRPC(int playerResponsibleNumber, int playerActorNumber, int newState, PhotonMessageInfo info)
	{
		bool flag = this.IsValidClientRPC(info.Sender);
		bool flag2 = newState == 1 && info.Sender.ActorNumber == playerActorNumber;
		bool flag3 = newState == 0 && flag;
		if (!flag2 && !flag3)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || grplayer2.IsNull() || !grplayer2.playerStateChangeLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (newState == 0 && playerResponsibleNumber != playerActorNumber)
		{
			GRPlayer grplayer3 = GRPlayer.Get(playerResponsibleNumber);
			if (grplayer3 != null && grplayer3 != grplayer)
			{
				grplayer3.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
		grplayer.ChangePlayerState((GRPlayer.GRPlayerState)newState, this);
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000DCB40 File Offset: 0x000DAD40
	public void RequestGrantPlayerShield(GRPlayer player, int shieldHp, int shieldFlags)
	{
		base.GetView.RPC("RequestGrantPlayerShieldRPC", this.GetAuthorityPlayer(), new object[]
		{
			PhotonNetwork.LocalPlayer.ActorNumber,
			player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
			shieldHp,
			shieldFlags
		});
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x000DCBAC File Offset: 0x000DADAC
	[PunRPC]
	private void RequestGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (!this.IsValidAuthorityRPC(info.Sender) || grplayer.IsNull() || !grplayer.fireShieldLimiter.CheckCallTime(Time.unscaledTime) || grplayer2.IsNull() || !grplayer2.CanActivateShield(shieldHp))
		{
			return;
		}
		base.GetView.RPC("ApplyGrantPlayerShieldRPC", 0, new object[]
		{
			shieldingPlayer,
			playerToGrantShieldActorNumber,
			shieldHp,
			shieldFlags
		});
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x000DCC4C File Offset: 0x000DAE4C
	[PunRPC]
	private void ApplyGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.GrantPlayerShield))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (grplayer.TryActivateShield(shieldHp, shieldFlags))
		{
			GRPlayer grplayer2 = GRPlayer.Get(shieldingPlayer);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000DCCB0 File Offset: 0x000DAEB0
	public void RequestFireProjectile(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if ((NetworkSystem.Instance.InRoom && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("RequestFireProjectileRPC", 0, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(entityId),
				firingPosition,
				targetPosition,
				networkTime
			});
		}
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000DCD30 File Offset: 0x000DAF30
	[PunRPC]
	private void RequestFireProjectileRPC(int entityNetId, Vector3 firingPosition, Vector3 targetPosition, double networkTime, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId, targetPosition) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RequestFireProjectile) || !this.gameEntityManager.IsEntityNearPosition(entityNetId, firingPosition, 16f))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		this.OnRequestFireProjectileInternal(entityIdFromNetId, firingPosition, targetPosition, networkTime);
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x000DCD8C File Offset: 0x000DAF8C
	private void OnRequestFireProjectileInternal(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		GREnemyRanged gameComponent = this.gameEntityManager.GetGameComponent<GREnemyRanged>(entityId);
		if (gameComponent != null)
		{
			gameComponent.RequestRangedAttack(firingPosition, targetPosition, networkTime);
		}
		GRHazardTower gameComponent2 = this.gameEntityManager.GetGameComponent<GRHazardTower>(entityId);
		if (gameComponent2 != null)
		{
			gameComponent2.OnFire(firingPosition, targetPosition, networkTime);
		}
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x000DCDDC File Offset: 0x000DAFDC
	[PunRPC]
	public void BroadcastHandprint(Vector3 pos, Quaternion orient, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		float num = 10000f;
		if (!pos.IsValid(num) || !orient.IsValid())
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender);
		if (grplayer == null)
		{
			return;
		}
		if (!GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, pos, false, true, 3f))
		{
			return;
		}
		if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && Time.time - this.LastHandprintTime <= 0.25f)
		{
			return;
		}
		this.LastHandprintTime = Time.time;
		this.reactor.AddHandprint(pos, orient);
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x000DCE87 File Offset: 0x000DB087
	public void OnAbilityDie(GameEntity entity)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.OnAbilityDie(entity);
	}

	// Token: 0x06002932 RID: 10546 RVA: 0x000DCEA4 File Offset: 0x000DB0A4
	public void RequestShiftStartAuthority(bool isFirstShift)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			double time = PhotonNetwork.Time;
			SRand srand = new SRand(Mathf.FloorToInt(Time.time * 100f));
			int num = srand.NextInt(0, int.MaxValue);
			string text = Guid.NewGuid().ToString();
			this.photonView.RPC("ApplyShiftStartRPC", 0, new object[]
			{
				time,
				num,
				text,
				isFirstShift
			});
			shiftManager.RequestState(GhostReactorShiftManager.State.ShiftActive);
			ProgressionManager.Instance.StartOfShift(text, shiftManager.shiftRewardCoresForMothership, this.reactor.vrRigs.Count, this.reactor.GetDepthLevel());
		}
	}

	// Token: 0x06002933 RID: 10547 RVA: 0x000DCF8C File Offset: 0x000DB18C
	[PunRPC]
	public void ApplyShiftStartRPC(double shiftStartTime, int randomSeed, string gameIdGuid, bool isFirstShift, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftStartTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		int num = Math.Clamp(this.reactor.NumActivePlayers, 0, this.reactor.difficultyScalingPerPlayer.Count - 1);
		this.reactor.difficultyScalingForCurrentFloor = 1f;
		if (this.reactor.difficultyScalingPerPlayer.Count > 0)
		{
			this.reactor.difficultyScalingForCurrentFloor = this.reactor.difficultyScalingPerPlayer[num];
		}
		double num2 = PhotonNetwork.Time - shiftStartTime;
		if (num2 < 0.0 || num2 > 10.0)
		{
			return;
		}
		levelGenerator.Generate(randomSeed);
		if (this.gameEntityManager.IsAuthority())
		{
			if (this.activeSpawnSectionEntitiesCoroutine != null)
			{
				base.StopCoroutine(this.activeSpawnSectionEntitiesCoroutine);
			}
			this.activeSpawnSectionEntitiesCoroutine = base.StartCoroutine(this.SpawnSectionEntitiesCoroutine(this.reactor.difficultyScalingForCurrentFloor));
		}
		shiftManager.shiftStats.ResetShiftStats();
		shiftManager.ResetJudgment();
		shiftManager.RefreshShiftStatsDisplay();
		shiftManager.OnShiftStarted(gameIdGuid, shiftStartTime, true, isFirstShift);
		this.reactor.ClearAllHandprints();
		this.reactor.ClearAllRespawns();
	}

	// Token: 0x06002934 RID: 10548 RVA: 0x000DD0E7 File Offset: 0x000DB2E7
	private IEnumerator SpawnSectionEntitiesCoroutine(float respawnCount)
	{
		int initialFrameCount = Time.frameCount;
		while (initialFrameCount == Time.frameCount)
		{
			yield return this.spawnSectionEntitiesWait;
		}
		if (this.gameEntityManager.IsAuthority())
		{
			this.reactor.levelGenerator.SpawnEntitiesInEachSection(respawnCount);
		}
		yield break;
	}

	// Token: 0x06002935 RID: 10549 RVA: 0x000DD100 File Offset: 0x000DB300
	public void RequestShiftEnd()
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		GhostReactorManager.tempEntitiesToDestroy.Clear();
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			GameEntity gameEntity = gameEntities[i];
			if (gameEntity != null && !this.ShouldEntitySurviveShift(gameEntity))
			{
				GhostReactorManager.tempEntitiesToDestroy.Add(gameEntity.id);
			}
		}
		this.gameEntityManager.RequestDestroyItems(GhostReactorManager.tempEntitiesToDestroy);
		this.photonView.RPC("ApplyShiftEndRPC", 1, new object[]
		{
			PhotonNetwork.Time
		});
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(PhotonNetwork.Time, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
		shiftManager.RequestState(GhostReactorShiftManager.State.PostShift);
	}

	// Token: 0x06002936 RID: 10550 RVA: 0x000DD210 File Offset: 0x000DB410
	[PunRPC]
	public void ApplyShiftEndRPC(double networkedTime, PhotonMessageInfo info)
	{
		if (!double.IsFinite(networkedTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftEnd))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		this.reactor.ClearAllRespawns();
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(networkedTime, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
	}

	// Token: 0x06002937 RID: 10551 RVA: 0x000DD2B0 File Offset: 0x000DB4B0
	private bool ShouldEntitySurviveShift(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return true;
		}
		if (this.reactor == null)
		{
			return false;
		}
		if (gameEntity.GetComponent<GREnemyChaser>() != null || gameEntity.GetComponent<GREnemyRanged>() != null || gameEntity.GetComponent<GREnemyPhantom>() != null || gameEntity.GetComponent<GREnemyPest>() != null)
		{
			return false;
		}
		if (gameEntity.GetComponent<GRBreakable>() != null || gameEntity.GetComponent<GRCollectibleDispenser>() != null || gameEntity.GetComponent<GRMetalEnergyGate>() != null || gameEntity.GetComponent<GRBarrierSpectral>() != null || gameEntity.GetComponent<GRSconce>() != null)
		{
			return false;
		}
		Collider safeZoneLimit = this.reactor.safeZoneLimit;
		Vector3 position = gameEntity.gameObject.transform.position;
		return safeZoneLimit.bounds.Contains(position) || gameEntity.GetComponent<GRBadge>() != null;
	}

	// Token: 0x06002938 RID: 10552 RVA: 0x000DD39F File Offset: 0x000DB59F
	public void ReportEnemyDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.EnemyDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRKillEnemy", 1);
	}

	// Token: 0x06002939 RID: 10553 RVA: 0x000DD3D8 File Offset: 0x000DB5D8
	public void ReportCoreCollection(GRPlayer player, ProgressionManager.CoreType type)
	{
		Debug.Log("GhostReactorManager ReportCoreCollection");
		if (player == null)
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		if (type == ProgressionManager.CoreType.ChaosSeed)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.SentientCoresCollected);
		}
		else if (type == ProgressionManager.CoreType.SuperCore)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 3f);
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 15f);
				}
			}
		}
		else
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 1f);
			int count2 = this.reactor.vrRigs.Count;
			for (int j = 0; j < count2; j++)
			{
				GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
				if (grplayer2 != null)
				{
					grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 5f);
				}
			}
		}
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRCollectCore", 1);
	}

	// Token: 0x0600293A RID: 10554 RVA: 0x000DD524 File Offset: 0x000DB724
	public void ReportPlayerDeath(GRPlayer player)
	{
		if (this.reactor == null || player == null || this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.PlayerDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Deaths, 1f);
	}

	// Token: 0x0600293B RID: 10555 RVA: 0x000DD580 File Offset: 0x000DB780
	public void PromotionBotActivePlayerRequest(int state)
	{
		this.photonView.RPC("PromotionBotActivePlayerRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			state
		});
	}

	// Token: 0x0600293C RID: 10556 RVA: 0x000DD5A8 File Offset: 0x000DB7A8
	[PunRPC]
	public void PromotionBotActivePlayerRequestRPC(int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.promotionBotLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (promotionBot == null)
		{
			return;
		}
		if (state == 6)
		{
			if (promotionBot.currentPlayerActorNumber != -1)
			{
				return;
			}
			state = 1;
		}
		int actorNumber = info.Sender.ActorNumber;
		this.photonView.RPC("PromotionBotActivePlayerResponseRPC", 1, new object[]
		{
			actorNumber,
			state
		});
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x0600293D RID: 10557 RVA: 0x000DD664 File Offset: 0x000DB864
	[PunRPC]
	public void PromotionBotActivePlayerResponseRPC(int actorNumber, int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (GRPlayer.Get(info.Sender.ActorNumber) == null || promotionBot == null || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.PromotionBotResponse))
		{
			return;
		}
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x0600293E RID: 10558 RVA: 0x000DD6D4 File Offset: 0x000DB8D4
	[PunRPC]
	public void BroadcastScoreboardPage(int scoreboardPage, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.scoreboardPageLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (GRUIScoreboard.ValidPage((GRUIScoreboard.ScoreboardScreen)scoreboardPage))
		{
			GhostReactor.instance.UpdateScoreboardScreen((GRUIScoreboard.ScoreboardScreen)scoreboardPage);
		}
	}

	// Token: 0x0600293F RID: 10559 RVA: 0x000DD730 File Offset: 0x000DB930
	[PunRPC]
	public void BroadcastStartingProgression(int points, int redeemedPoints, double shiftJoinedTime, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftJoinedTime) || double.IsInfinity(shiftJoinedTime))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.progressionBroadcastLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.SetProgressionData(points, redeemedPoints, false);
		grplayer.shiftJoinTime = Math.Clamp(shiftJoinedTime, PhotonNetwork.Time - 10.0, PhotonNetwork.Time);
	}

	// Token: 0x06002940 RID: 10560 RVA: 0x000DD7B4 File Offset: 0x000DB9B4
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			0,
			0
		});
	}

	// Token: 0x06002941 RID: 10561 RVA: 0x000DD7ED File Offset: 0x000DB9ED
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			0
		});
	}

	// Token: 0x06002942 RID: 10562 RVA: 0x000DD826 File Offset: 0x000DBA26
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0, int param1)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			param1
		});
	}

	// Token: 0x06002943 RID: 10563 RVA: 0x000DD860 File Offset: 0x000DBA60
	public bool VerifyShuttleInteractability(GRPlayer player, int shuttleIdx, bool ignoreOwnership = false)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(shuttleIdx);
		return !(shuttleById == null) && shuttleById.IsShuttleInteractableByPlayer(player, ignoreOwnership);
	}

	// Token: 0x06002944 RID: 10564 RVA: 0x000DD89C File Offset: 0x000DBA9C
	[PunRPC]
	public void RequestPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		bool flag = false;
		switch (playerAction)
		{
		case 1:
			flag = (!shiftManager.ShiftActive && shiftManager.authorizedToDelveDeeper);
			if (flag)
			{
				int num = this.reactor.GetDepthLevel() + 1;
				this.reactor.depthConfigIndex = this.reactor.PickLevelConfigForDepth(num);
				param0 = num;
				param1 = this.reactor.depthConfigIndex;
			}
			break;
		case 2:
			flag = true;
			break;
		case 3:
			flag = this.VerifyShuttleInteractability(grplayer, param0, true);
			param1 = info.Sender.ActorNumber;
			break;
		case 4:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 5:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 6:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 7:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 8:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 9:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 1);
			param1 = info.Sender.ActorNumber;
			break;
		case 10:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 3);
			param1 = info.Sender.ActorNumber;
			break;
		case 11:
			flag = (param0 == info.Sender.ActorNumber || this.IsAuthorityPlayer(info.Sender));
			if (this.reactor.seedExtractor.StationOpen && this.reactor.seedExtractor.CurrentPlayerActorNumber != info.Sender.ActorNumber)
			{
				playerAction = 13;
			}
			break;
		case 12:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 13:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 14:
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(param1);
			if (this.IsAuthorityPlayer(info.Sender) && gameEntityFromNetId != null && gameEntityFromNetId.lastHeldByActorNumber == param0)
			{
				flag = true;
			}
			break;
		}
		case 15:
		{
			int netId = param1;
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (gameEntityFromNetId2 != null && this.reactor.seedExtractor.ValidateSeedDepositSucceeded(param0, param1))
			{
				this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId2.id);
				flag = true;
			}
			break;
		}
		case 16:
			flag = (info.Sender.ActorNumber == param0);
			break;
		}
		if (flag)
		{
			this.photonView.RPC("ApplyPlayerActionRPC", 0, new object[]
			{
				playerAction,
				param0,
				param1
			});
		}
	}

	// Token: 0x06002945 RID: 10565 RVA: 0x000DDBEC File Offset: 0x000DBDEC
	[PunRPC]
	public void ApplyPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		this.gameEntityManager.IsAuthorityPlayer(info.Sender);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		switch (playerAction)
		{
		case 1:
			this.reactor.SetNextDelveDepth(param0, param1);
			return;
		case 2:
			this.reactor.shiftManager.SetState((GhostReactorShiftManager.State)param0, false);
			return;
		case 3:
		{
			GRPlayer grplayer2 = GRPlayer.Get(param1);
			if (grplayer2 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer2, param0, true))
			{
				return;
			}
			GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById != null)
			{
				shuttleById.OnOpenDoor();
				return;
			}
			break;
		}
		case 4:
		{
			GRPlayer grplayer3 = GRPlayer.Get(param1);
			if (grplayer3 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer3, param0, false))
			{
				return;
			}
			GRShuttle shuttleById2 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById2 != null)
			{
				shuttleById2.OnCloseDoor();
				return;
			}
			break;
		}
		case 5:
		{
			GRPlayer grplayer4 = GRPlayer.Get(param1);
			if (grplayer4 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer4, param0, false))
			{
				return;
			}
			GRShuttle shuttleById3 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById3 != null)
			{
				shuttleById3.OnLaunch();
				return;
			}
			break;
		}
		case 6:
		{
			GRPlayer grplayer5 = GRPlayer.Get(param1);
			if (grplayer5 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer5, param0, false))
			{
				return;
			}
			GRShuttle shuttleById4 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById4 != null)
			{
				shuttleById4.OnArrive();
				return;
			}
			break;
		}
		case 7:
		{
			GRPlayer grplayer6 = GRPlayer.Get(param1);
			if (grplayer6 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer6, param0, false))
			{
				return;
			}
			GRShuttle shuttleById5 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById5 != null)
			{
				shuttleById5.OnTargetLevelUp();
				return;
			}
			break;
		}
		case 8:
		{
			GRPlayer grplayer7 = GRPlayer.Get(param1);
			if (grplayer7 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer7, param0, false))
			{
				return;
			}
			GRShuttle shuttleById6 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById6 != null)
			{
				shuttleById6.OnTargetLevelDown();
				return;
			}
			break;
		}
		case 9:
		{
			GRPlayer grplayer8 = GRPlayer.Get(param1);
			if (grplayer8 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 1);
				grplayer8.dropPodLevel = param0;
				this.reactor.RefreshBays();
				grplayer8.RefreshShuttles();
				return;
			}
			break;
		}
		case 10:
		{
			GRPlayer grplayer9 = GRPlayer.Get(param1);
			if (grplayer9 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 3);
				grplayer9.dropPodChasisLevel = param0;
				this.reactor.RefreshBays();
				grplayer9.RefreshShuttles();
				return;
			}
			break;
		}
		case 11:
			this.reactor.seedExtractor.CardSwipeSuccess();
			this.reactor.seedExtractor.OpenStation(param0);
			return;
		case 12:
			this.reactor.seedExtractor.CloseStation();
			return;
		case 13:
			this.reactor.seedExtractor.CardSwipeFail();
			return;
		case 14:
			this.reactor.seedExtractor.TryDepositSeed(param0, param1);
			return;
		case 15:
			this.reactor.seedExtractor.SeedDepositSucceeded(param0, param1);
			return;
		case 16:
			this.reactor.seedExtractor.SeedDepositFailed(param0, param1);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000DDF60 File Offset: 0x000DC160
	public GRToolUpgradePurchaseStationFull GetToolUpgradeStationFullForIndex(int idx)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null || idx < 0 || idx >= this.reactor.toolUpgradePurchaseStationsFull.Count)
		{
			return null;
		}
		return this.reactor.toolUpgradePurchaseStationsFull[idx];
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000DDFB2 File Offset: 0x000DC1B2
	public int GetIndexForToolUpgradeStationFull(GRToolUpgradePurchaseStationFull station)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null)
		{
			return -1;
		}
		return this.reactor.toolUpgradePurchaseStationsFull.IndexOf(station);
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000DDFE4 File Offset: 0x000DC1E4
	public void RequestNetworkShelfAndItemChange(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", 1, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000DE04C File Offset: 0x000DC24C
	private void SelectToolShelfAndItemRPCRouted(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber == info.Sender.ActorNumber)
		{
			toolUpgradeStationFullForIndex.SetSelectedShelfAndItem(shelf, item, true);
		}
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000DE088 File Offset: 0x000DC288
	public void RequestPurchaseToolOrUpgrade(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000DE0F4 File Offset: 0x000DC2F4
	public void RequestPurchaseRPCRoutedAuthority(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != info.Sender.ActorNumber)
		{
			return;
		}
		ValueTuple<bool, bool> valueTuple = toolUpgradeStationFullForIndex.TryPurchaseAuthority(grplayer, shelf, item);
		bool item2 = valueTuple.Item1;
		if (!valueTuple.Item2)
		{
			return;
		}
		if (item2)
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", 1, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		else
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", 1, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, item2);
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000DE218 File Offset: 0x000DC418
	public void NotifyPurchaseToolOrUpgradeRPCRouted(int actorNumber, int stationIndex, int shelf, int item, bool success, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		if (grplayer != null)
		{
			toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, success);
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000DE264 File Offset: 0x000DC464
	public void RequestStationExclusivity(GRToolUpgradePurchaseStationFull station)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			0,
			0
		});
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000DE2D0 File Offset: 0x000DC4D0
	public void SetActivePlayerAuthority(GRToolUpgradePurchaseStationFull station, int actorNumber)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		station.SetActivePlayer(actorNumber);
		this.photonView.RPC("ToolPurchaseV2_RPC", 1, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			station.currentActivePlayerActorNumber,
			0
		});
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000DE34C File Offset: 0x000DC54C
	public void RequestStationExclusivityRPCRoutedAuthority(int stationIndex, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != -1)
		{
			return;
		}
		this.SetActivePlayerAuthority(toolUpgradeStationFullForIndex, info.Sender.ActorNumber);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000DE398 File Offset: 0x000DC598
	public void SetToolStationActivePlayerRPCRouted(int stationIndex, int activeOwner, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetActivePlayer(activeOwner);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000DE3D0 File Offset: 0x000DC5D0
	public void BroadcastHandleAndSelectionWheelPosition(GRToolUpgradePurchaseStationFull station, int handlePos, int wheelPos)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		if (NetworkSystem.Instance.LocalPlayer.ActorNumber != station.currentActivePlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", 1, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			handlePos,
			wheelPos
		});
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000DE450 File Offset: 0x000DC650
	public void SetHandleAndSelectionWheelPositionRPCRouted(int stationIndex, int handlePos, int wheelPos, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != toolUpgradeStationFullForIndex.currentActivePlayerActorNumber)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetHandleAndSelectionWheelPositionRemote(handlePos, wheelPos);
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x00002789 File Offset: 0x00000989
	public void RequestHackToolStation()
	{
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000DE48C File Offset: 0x000DC68C
	[PunRPC]
	public void ToolPurchaseV2_RPC(GhostReactorManager.ToolPurchaseActionV2 command, int initiatorID, int stationIndex, int param1, int param2, PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		switch (command)
		{
		case GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority:
			this.RequestPurchaseRPCRoutedAuthority(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem:
			this.SelectToolShelfAndItemRPCRouted(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, false, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, true, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority:
			this.RequestStationExclusivityRPCRoutedAuthority(stationIndex, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer:
			this.SetToolStationActivePlayerRPCRouted(stationIndex, param1, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition:
			this.SetHandleAndSelectionWheelPositionRPCRouted(stationIndex, param1, param2, info);
			break;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationHackedDebug:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000DE52F File Offset: 0x000DC72F
	public void ToolPurchaseStationRequest(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action)
	{
		this.photonView.RPC("ToolPurchaseStationRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			stationIndex,
			action
		});
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000DE560 File Offset: 0x000DC760
	[PunRPC]
	public void ToolPurchaseStationRequestRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidAuthorityRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestToolPurchaseStationLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (action)
		{
		case GhostReactorManager.ToolPurchaseStationAction.ShiftLeft:
			grtoolPurchaseStation.ShiftLeftAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", 1, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.ShiftRight:
			grtoolPurchaseStation.ShiftRightAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", 1, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.TryPurchase:
		{
			bool flag = false;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetNetPlayerByID(info.Sender.ActorNumber), out rigContainer))
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				int num;
				if (component != null && grtoolPurchaseStation.TryPurchaseAuthority(component, out num))
				{
					this.photonView.RPC("ToolPurchaseStationResponseRPC", 1, new object[]
					{
						stationIndex,
						GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded,
						info.Sender.ActorNumber,
						num
					});
					this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded, info.Sender.ActorNumber, num);
					flag = true;
				}
			}
			if (!flag)
			{
				this.photonView.RPC("ToolPurchaseStationResponseRPC", 1, new object[]
				{
					stationIndex,
					GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed,
					info.Sender.ActorNumber,
					0
				});
				this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed, info.Sender.ActorNumber, 0);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000DE7A8 File Offset: 0x000DC9A8
	[PunRPC]
	public void ToolPurchaseStationResponseRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidClientRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolPurchaseResponse))
		{
			return;
		}
		this.ToolPurchaseResponseLocal(stationIndex, responseType, dataA, dataB);
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000DE808 File Offset: 0x000DCA08
	private void ToolPurchaseResponseLocal(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (responseType)
		{
		case GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate:
			grtoolPurchaseStation.OnSelectionUpdate(dataA);
			return;
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded:
		{
			grtoolPurchaseStation.OnPurchaseSucceeded();
			GRPlayer grplayer = GRPlayer.Get(dataA);
			if (grplayer != null)
			{
				grplayer.IncrementCoresSpentPlayer(dataB);
				grplayer.AddItemPurchased(grtoolPurchaseStation.GetCurrentToolName());
				grplayer.SubtractShiftCredit(dataB);
				return;
			}
			break;
		}
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed:
			grtoolPurchaseStation.OnPurchaseFailed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000DE8A4 File Offset: 0x000DCAA4
	public void ToolUpgradeStationRequestUpgrade(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolUpgradeStationRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[]
		{
			UpgradeID,
			entityNetId
		});
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000DE8D4 File Offset: 0x000DCAD4
	public void ToolSnapRequestUpgrade(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolSnapRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[]
		{
			upgradeNetID,
			UpgradeID,
			entityNetId
		});
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000DE910 File Offset: 0x000DCB10
	[PunRPC]
	public void ToolSnapRequestUpgradeRPC(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			Object component = gameEntity.GetComponent<GRTool>();
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(upgradeNetID));
			if (component != null && gameEntity2 != null && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f) && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f))
			{
				this.photonView.RPC("UpgradeToolRemoteRPC", 0, new object[]
				{
					UpgradeID,
					entityNetId,
					false,
					info.Sender.ActorNumber
				});
				this.gameEntityManager.RequestDestroyItem(gameEntity2.id);
			}
		}
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000DEA58 File Offset: 0x000DCC58
	public void ToolUpgradeStationRequestUpgradeRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000DEA68 File Offset: 0x000DCC68
	[PunRPC]
	public void UpgradeToolRemoteRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, bool applyCost, int playerNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (applyCost)
		{
			GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
			int shiftCreditDelta;
			if (grplayer != null && this.reactor.toolProgression.GetShiftCreditCost(UpgradeID, out shiftCreditDelta))
			{
				grplayer.SubtractShiftCredit(shiftCreditDelta);
			}
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			GRTool component = gameEntity.GetComponent<GRTool>();
			if (component != null)
			{
				component.UpgradeTool(UpgradeID);
			}
		}
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x00027DED File Offset: 0x00025FED
	private bool DoesUserHaveResearchUnlocked(int UserID, string ResearchID)
	{
		return true;
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000DEAF7 File Offset: 0x000DCCF7
	public void ToolPlacedInUpgradeStation(GameEntity entity)
	{
		this.photonView.RPC("PlacedToolInUpgradeStationRPC", 0, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id)
		});
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x00002789 File Offset: 0x00000989
	public void PlacedToolInUpgradeStationRPC(int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000DEB29 File Offset: 0x000DCD29
	public void UpgradeToolAtToolStation()
	{
		this.photonView.RPC("UpgradeToolAtToolStationRPC", 0, Array.Empty<object>());
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x00002789 File Offset: 0x00000989
	public void UpgradeToolAtToolStationRPC(PhotonMessageInfo info)
	{
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x00002789 File Offset: 0x00000989
	public void LocalEjectToolInUpgradeStation()
	{
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000DEB44 File Offset: 0x000DCD44
	public void EntityEnteredDropZone(GameEntity entity)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRUIStationEmployeeBadges employeeBadges = this.reactor.employeeBadges;
		long num = BitPackUtils.PackWorldPosForNetwork(entity.transform.position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(entity.transform.rotation);
		if (entity.gameObject.GetComponent<GRBadge>() != null)
		{
			GRUIEmployeeBadgeDispenser gruiemployeeBadgeDispenser = employeeBadges.badgeDispensers[entity.gameObject.GetComponent<GRBadge>().dispenserIndex];
			if (gruiemployeeBadgeDispenser != null)
			{
				num = BitPackUtils.PackWorldPosForNetwork(gruiemployeeBadgeDispenser.GetSpawnPosition());
				num2 = BitPackUtils.PackQuaternionForNetwork(gruiemployeeBadgeDispenser.GetSpawnRotation());
			}
		}
		this.photonView.RPC("EntityEnteredDropZoneRPC", 0, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id),
			num,
			num2
		});
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000DEC2C File Offset: 0x000DCE2C
	[PunRPC]
	public void EntityEnteredDropZoneRPC(int entityNetId, long position, int rotation, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.EntityEnteredDropZone))
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "EntityEnteredDropZoneRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
		float num = 10000f;
		if (!vector.IsValid(num))
		{
			return;
		}
		Quaternion rotation2 = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
		if (!rotation2.IsValid())
		{
			return;
		}
		if (!this.IsPositionInZone(vector))
		{
			return;
		}
		if ((vector - this.reactor.dropZone.transform.position).magnitude > 5f)
		{
			return;
		}
		this.LocalEntityEnteredDropZone(this.gameEntityManager.GetEntityIdFromNetId(entityNetId), vector, rotation2);
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000DECD8 File Offset: 0x000DCED8
	private void LocalEntityEnteredDropZone(GameEntityId entityId, Vector3 position, Quaternion rotation)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRDropZone dropZone = this.reactor.dropZone;
		Vector3 linearVelocity = dropZone.GetRepelDirectionWorld() * GhostReactor.DROP_ZONE_REPEL;
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityId);
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber >= 0 && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			int handIndex = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
				GamePlayerLocal.instance.ClearGrabbed(handIndex);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased.Invoke();
			}
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		if (!(gameEntity.gameObject.GetComponent<GRBadge>() != null))
		{
			Rigidbody component = gameEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = linearVelocity;
				component.angularVelocity = Vector3.zero;
			}
		}
		dropZone.PlayEffect();
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000DEE04 File Offset: 0x000DD004
	public void RequestRecycleScanItem(GameEntityId gameEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(gameEntityId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleScanItemRPC", 0, new object[]
		{
			netIdFromEntityId
		});
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000DEE48 File Offset: 0x000DD048
	[PunRPC]
	public void ApplyRecycleScanItemRPC(int netId, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplRecycleScanItem))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(netId);
		this.reactor.recycler.ScanItem(entityIdFromNetId);
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000DEE9C File Offset: 0x000DD09C
	public void RequestRecycleItem(int lastHeldActorNumber, GameEntityId toolId, GRTool.GRToolType toolType)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.gameEntityManager == null)
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(toolId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleItemRPC", 0, new object[]
		{
			lastHeldActorNumber,
			netIdFromEntityId,
			toolType
		});
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x000DEF00 File Offset: 0x000DD100
	[PunRPC]
	public void ApplyRecycleItemRPC(int lastHeldActorNumber, int toolNetId, GRTool.GRToolType toolType, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyRecycleItem) || !this.gameEntityManager.IsEntityNearPosition(toolNetId, this.reactor.recycler.transform.position, 16f))
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		Mathf.FloorToInt((float)this.reactor.recycler.GetRecycleValue(toolType) / (float)count);
		ProgressionManager.Instance.RecycleTool(toolType, this.reactor.vrRigs.Count);
		this.reactor.RefreshScoreboards();
		this.reactor.recycler.RecycleItem();
		this.gameEntityManager.DestroyItemLocal(this.gameEntityManager.GetEntityIdFromNetId(toolNetId));
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x000DEFD8 File Offset: 0x000DD1D8
	public void RequestSentientCorePerformJump(GameEntity entity, Vector3 startPos, Vector3 normal, Vector3 direction, float waitTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(entity.id);
		double num = PhotonNetwork.Time + (double)waitTime;
		base.SendRPC("SentientCorePerformJumpRPC", 0, new object[]
		{
			netIdFromEntityId,
			startPos,
			normal,
			direction,
			num
		});
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x000DF04C File Offset: 0x000DD24C
	[PunRPC]
	public void SentientCorePerformJumpRPC(int entityNetId, Vector3 startPosition, Vector3 surfaceNormal, Vector3 jumpDirection, double jumpStartTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, startPosition) && !this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplySentientCoreDestination))
		{
			float num = 10000f;
			if (startPosition.IsValid(num))
			{
				float num2 = 10000f;
				if (surfaceNormal.IsValid(num2))
				{
					float num3 = 10000f;
					if (jumpDirection.IsValid(num3) && double.IsFinite(jumpStartTime) && PhotonNetwork.Time - jumpStartTime <= 5.0 && this.gameEntityManager.IsEntityNearPosition(entityNetId, startPosition, 16f))
					{
						GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
						if (gameEntity == null)
						{
							return;
						}
						GRSentientCore component = gameEntity.GetComponent<GRSentientCore>();
						if (component == null)
						{
							return;
						}
						component.PerformJump(startPosition, surfaceNormal, jumpDirection, jumpStartTime);
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000DF11D File Offset: 0x000DD31D
	protected void OnNewPlayerEnteredGhostReactor()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityZoneClear(GTZone zoneId)
	{
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000DF13C File Offset: 0x000DD33C
	public void OnZoneCreate()
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		int newDepthConfigIndex = this.reactor.PickLevelConfigForDepth(grplayer.shuttleData.targetLevel);
		this.reactor.SetNextDelveDepth(grplayer.shuttleData.targetLevel, newDepthConfigIndex);
		this.reactor.DelveToNextDepth();
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		}
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000DF1D4 File Offset: 0x000DD3D4
	public void OnZoneInit()
	{
		if (this.reactor == null)
		{
			return;
		}
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		this.reactor.VRRigRefresh();
		if (this.reactor.employeeTerminal != null)
		{
			this.reactor.employeeTerminal.Setup();
		}
		if (GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, this.reactor.toolProgression.GetDropPodLevel());
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, this.reactor.toolProgression.GetDropPodChasisLevel());
		}
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x000DF27C File Offset: 0x000DD47C
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer component = GamePlayerLocal.instance.gamePlayer.GetComponent<GRPlayer>();
		if (component != null)
		{
			GRBadge badge = component.badge;
			if (badge != null && badge.IsAttachedToPlayer())
			{
				component.lastLeftWithBadgeAttachedTime = Time.timeAsDouble;
			}
			component.SendGameEndedTelemetry(false, reason);
		}
		if (this.reactor.levelGenerator != null)
		{
			this.reactor.levelGenerator.ClearLevelSections();
		}
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.OnShiftEnded(0.0, false, reason);
		}
		GRPlayer grplayer = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			grplayer.SetGooParticleSystemEnabled(false, false);
			grplayer.SetGooParticleSystemEnabled(true, false);
		}
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000DF35D File Offset: 0x000DD55D
	public bool IsZoneReady()
	{
		return this.reactor != null;
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x00027DED File Offset: 0x00025FED
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x00002789 File Offset: 0x00000989
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000DF36C File Offset: 0x000DD56C
	public void SerializeZoneData(BinaryWriter writer)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		writer.Write(this.reactor.depthLevel);
		writer.Write(this.reactor.depthConfigIndex);
		writer.Write(this.reactor.difficultyScalingForCurrentFloor);
		if (shiftManager != null)
		{
			writer.Write(shiftManager.ShiftActive);
			writer.Write(shiftManager.ShiftStartNetworkTime);
			shiftManager.shiftStats.Serialize(writer);
			writer.Write(shiftManager.ShiftId);
			writer.Write(shiftManager.stateStartTime);
			writer.Write((byte)shiftManager.GetState());
			writer.Write(levelGenerator.seed);
		}
		if (promotionBot != null)
		{
			writer.Write(promotionBot.GetCurrentPlayerActorNumber());
			writer.Write((int)promotionBot.currentState);
		}
		for (int i = 0; i < array.Length; i++)
		{
			writer.Write((int)array[i].currentScreen);
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		writer.Write(toolPurchasingStations.Count);
		for (int j = 0; j < toolPurchasingStations.Count; j++)
		{
			writer.Write(toolPurchasingStations[j].ActiveEntryIndex);
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		writer.Write(toolUpgradePurchaseStationsFull.Count);
		for (int k = 0; k < toolUpgradePurchaseStationsFull.Count; k++)
		{
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedShelf);
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedItem);
			writer.Write(toolUpgradePurchaseStationsFull[k].currentActivePlayerActorNumber);
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		writer.Write(this.reactor.respawnQueue.Count);
		for (int l = 0; l < respawnQueue.Count; l++)
		{
			writer.Write(respawnQueue[l].entityTypeID);
			writer.Write(respawnQueue[l].entityCreateData);
			writer.Write(respawnQueue[l].entityNextRespawnTime);
		}
		bool flag = false;
		writer.Write(flag);
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000DF5B4 File Offset: 0x000DD7B4
	public void DeserializeZoneData(BinaryReader reader)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		int depthLevel = reader.ReadInt32();
		this.reactor.depthLevel = depthLevel;
		int depthConfigIndex = reader.ReadInt32();
		this.reactor.depthConfigIndex = depthConfigIndex;
		float difficultyScalingForCurrentFloor = reader.ReadSingle();
		this.reactor.difficultyScalingForCurrentFloor = difficultyScalingForCurrentFloor;
		if (shiftManager != null)
		{
			bool flag = reader.ReadBoolean();
			double shiftStartTime = reader.ReadDouble();
			shiftManager.shiftStats.Deserialize(reader);
			shiftManager.RefreshShiftStatsDisplay();
			string text = reader.ReadString();
			shiftManager.SetShiftId(text);
			shiftManager.stateStartTime = reader.ReadDouble();
			GhostReactorShiftManager.State newState = (GhostReactorShiftManager.State)reader.ReadByte();
			shiftManager.SetState(newState, true);
			int inputSeed = reader.ReadInt32();
			if (flag)
			{
				levelGenerator.Generate(inputSeed);
				shiftManager.OnShiftStarted(text, shiftStartTime, false, true);
				this.reactor.ClearAllHandprints();
			}
		}
		if (promotionBot != null)
		{
			int actorNumber = reader.ReadInt32();
			int state = reader.ReadInt32();
			promotionBot.SetActivePlayerStateChange(actorNumber, state);
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i].currentScreen = (GRUIScoreboard.ScoreboardScreen)reader.ReadInt32();
		}
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		int num = reader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			int newSelectedIndex = reader.ReadInt32();
			if (j < toolPurchasingStations.Count && toolPurchasingStations[j] != null)
			{
				toolPurchasingStations[j].OnSelectionUpdate(newSelectedIndex);
			}
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		int num2 = reader.ReadInt32();
		for (int k = 0; k < num2; k++)
		{
			int shelf = reader.ReadInt32();
			int item = reader.ReadInt32();
			int activePlayer = reader.ReadInt32();
			if (k < toolUpgradePurchaseStationsFull.Count && toolUpgradePurchaseStationsFull[k] != null)
			{
				toolUpgradePurchaseStationsFull[k].SetSelectedShelfAndItem(shelf, item, true);
				toolUpgradePurchaseStationsFull[k].SetActivePlayer(activePlayer);
			}
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		respawnQueue.Clear();
		int num3 = reader.ReadInt32();
		for (int l = 0; l < num3; l++)
		{
			respawnQueue.Add(new GhostReactor.EntityTypeRespawnTracker
			{
				entityTypeID = reader.ReadInt32(),
				entityCreateData = reader.ReadInt64(),
				entityNextRespawnTime = reader.ReadSingle()
			});
		}
		reader.ReadBoolean();
		this.reactor.VRRigRefresh();
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000DF85B File Offset: 0x000DDA5B
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x00002789 File Offset: 0x00000989
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x00002789 File Offset: 0x00000989
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000DF860 File Offset: 0x000DDA60
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion rot, GorillaSurfaceOverride surfaceOverride, Vector3 handVelocity)
	{
		if (this.reactor != null)
		{
			this.reactor.OnTapLocal(isLeftHand, pos, rot, surfaceOverride);
		}
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handVelocity.magnitude / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(pos, num, 1f);
			}
		}
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000DF8CC File Offset: 0x000DDACC
	public void OnSharedTap(VRRig rig, Vector3 tapPos, float handTapSpeed)
	{
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handTapSpeed / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(tapPos, num, 1f);
			}
		}
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000DF914 File Offset: 0x000DDB14
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		grplayer.SerializeNetworkState(writer, grplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000DF940 File Offset: 0x000DDB40
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		GRPlayer player = GRPlayer.Get(actorNumber);
		GRPlayer.DeserializeNetworkStateAndBurn(reader, player, this);
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x00002076 File Offset: 0x00000276
	public bool DebugIsToolStationHacked()
	{
		return false;
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x06002984 RID: 10628 RVA: 0x00002076 File Offset: 0x00000276
	public static bool AggroDisabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040034D2 RID: 13522
	private const string EVENT_CORE_COLLECTED = "GRCollectCore";

	// Token: 0x040034D3 RID: 13523
	private const string EVENT_ENEMY_KILLED = "GRKillEnemy";

	// Token: 0x040034D4 RID: 13524
	public const string EVENT_BREAKABLE_BROKEN = "GRSmashBreakable";

	// Token: 0x040034D5 RID: 13525
	public const string EVENT_ENEMY_ARMOR_BREAK = "GRArmorBreak";

	// Token: 0x040034D6 RID: 13526
	public const string NETWORK_ROOM_GR_DEPTH = "ghostReactorDepth";

	// Token: 0x040034D7 RID: 13527
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x040034D8 RID: 13528
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x040034D9 RID: 13529
	public GameEntityManager gameEntityManager;

	// Token: 0x040034DA RID: 13530
	public GameAgentManager gameAgentManager;

	// Token: 0x040034DB RID: 13531
	public GRNoiseEventManager noiseEventManager;

	// Token: 0x040034DC RID: 13532
	public PhotonView photonView;

	// Token: 0x040034DD RID: 13533
	public GhostReactor reactor;

	// Token: 0x040034DE RID: 13534
	public CallLimitersList<CallLimiter, GhostReactorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GhostReactorManager.RPC>();

	// Token: 0x040034DF RID: 13535
	private const float HandprintThrottleTime = 0.25f;

	// Token: 0x040034E0 RID: 13536
	private float LastHandprintTime;

	// Token: 0x040034E1 RID: 13537
	private Coroutine activeSpawnSectionEntitiesCoroutine;

	// Token: 0x040034E2 RID: 13538
	private WaitForSeconds spawnSectionEntitiesWait = new WaitForSeconds(0.1f);

	// Token: 0x040034E3 RID: 13539
	private static List<GameEntityId> tempEntitiesToDestroy = new List<GameEntityId>();

	// Token: 0x040034E4 RID: 13540
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x040034E5 RID: 13541
	public static bool entityDebugEnabled = false;

	// Token: 0x040034E6 RID: 13542
	public static bool noiseDebugEnabled = false;

	// Token: 0x040034E7 RID: 13543
	public static bool bayUnlockEnabled = false;

	// Token: 0x0200064E RID: 1614
	public enum RPC
	{
		// Token: 0x040034E9 RID: 13545
		ApplyCollectItem,
		// Token: 0x040034EA RID: 13546
		ApplyChargeTool,
		// Token: 0x040034EB RID: 13547
		ApplyDepositCurrency,
		// Token: 0x040034EC RID: 13548
		ApplyPlayerRevived,
		// Token: 0x040034ED RID: 13549
		GrantPlayerShield,
		// Token: 0x040034EE RID: 13550
		RequestFireProjectile,
		// Token: 0x040034EF RID: 13551
		ApplyShiftStart,
		// Token: 0x040034F0 RID: 13552
		ApplyShiftEnd,
		// Token: 0x040034F1 RID: 13553
		ToolPurchaseResponse,
		// Token: 0x040034F2 RID: 13554
		ApplyBreakableBroken,
		// Token: 0x040034F3 RID: 13555
		EntityEnteredDropZone,
		// Token: 0x040034F4 RID: 13556
		PromotionBotResponse,
		// Token: 0x040034F5 RID: 13557
		DistillItem,
		// Token: 0x040034F6 RID: 13558
		ApplySentientCoreDestination,
		// Token: 0x040034F7 RID: 13559
		Handprint,
		// Token: 0x040034F8 RID: 13560
		ApplyRecycleItem,
		// Token: 0x040034F9 RID: 13561
		ApplRecycleScanItem,
		// Token: 0x040034FA RID: 13562
		SeedExtractorAction,
		// Token: 0x040034FB RID: 13563
		ToolUpgradeStationAction,
		// Token: 0x040034FC RID: 13564
		SendMothershipId,
		// Token: 0x040034FD RID: 13565
		RefreshShiftCredit
	}

	// Token: 0x0200064F RID: 1615
	public enum GRPlayerAction
	{
		// Token: 0x040034FF RID: 13567
		ButtonShiftStart,
		// Token: 0x04003500 RID: 13568
		DelveDeeper,
		// Token: 0x04003501 RID: 13569
		DelveState,
		// Token: 0x04003502 RID: 13570
		ShuttleOpen,
		// Token: 0x04003503 RID: 13571
		ShuttleClose,
		// Token: 0x04003504 RID: 13572
		ShuttleLaunch,
		// Token: 0x04003505 RID: 13573
		ShuttleArrive,
		// Token: 0x04003506 RID: 13574
		ShuttleTargetLevelUp,
		// Token: 0x04003507 RID: 13575
		ShuttleTargetLevelDown,
		// Token: 0x04003508 RID: 13576
		SetPodLevel,
		// Token: 0x04003509 RID: 13577
		SetPodChassisLevel,
		// Token: 0x0400350A RID: 13578
		SeedExtractorOpenStation,
		// Token: 0x0400350B RID: 13579
		SeedExtractorCloseStation,
		// Token: 0x0400350C RID: 13580
		SeedExtractorCardSwipeFail,
		// Token: 0x0400350D RID: 13581
		SeedExtractorTryDepositSeed,
		// Token: 0x0400350E RID: 13582
		SeedExtractorDepositSeedSucceeded,
		// Token: 0x0400350F RID: 13583
		SeedExtractorDepositSeedFailed,
		// Token: 0x04003510 RID: 13584
		DEBUG_ResetDepth,
		// Token: 0x04003511 RID: 13585
		DEBUG_DelveDeeper,
		// Token: 0x04003512 RID: 13586
		DEBUG_DelveShallower
	}

	// Token: 0x02000650 RID: 1616
	public enum ToolPurchaseActionV2
	{
		// Token: 0x04003514 RID: 13588
		RequestPurchaseAuthority,
		// Token: 0x04003515 RID: 13589
		SelectShelfAndItem,
		// Token: 0x04003516 RID: 13590
		NotifyPurchaseFail,
		// Token: 0x04003517 RID: 13591
		NotifyPurchaseSuccess,
		// Token: 0x04003518 RID: 13592
		RequestStationExclusivityAuthority,
		// Token: 0x04003519 RID: 13593
		SetToolStationActivePlayer,
		// Token: 0x0400351A RID: 13594
		SetHandleAndSelectionWheelPosition,
		// Token: 0x0400351B RID: 13595
		SetToolStationHackedDebug
	}

	// Token: 0x02000651 RID: 1617
	public enum ToolPurchaseStationAction
	{
		// Token: 0x0400351D RID: 13597
		ShiftLeft,
		// Token: 0x0400351E RID: 13598
		ShiftRight,
		// Token: 0x0400351F RID: 13599
		TryPurchase
	}

	// Token: 0x02000652 RID: 1618
	public enum ToolPurchaseStationResponse
	{
		// Token: 0x04003521 RID: 13601
		SelectionUpdate,
		// Token: 0x04003522 RID: 13602
		PurchaseSucceeded,
		// Token: 0x04003523 RID: 13603
		PurchaseFailed
	}
}
