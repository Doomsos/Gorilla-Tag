using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020005FB RID: 1531
[NetworkBehaviourWeaved(0)]
public class GameAgentManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002690 RID: 9872 RVA: 0x000CD3D3 File Offset: 0x000CB5D3
	// (set) Token: 0x06002691 RID: 9873 RVA: 0x000CD3DB File Offset: 0x000CB5DB
	public bool TickRunning { get; set; }

	// Token: 0x06002692 RID: 9874 RVA: 0x000CD3E4 File Offset: 0x000CB5E4
	protected override void Awake()
	{
		this.agents = new List<GameAgent>(128);
		this.netIdsForDestination = new List<int>();
		this.destinationsForDestination = new List<Vector3>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<byte>();
		this.netIdsForBehavior = new List<int>();
		this.behaviorsForBehavior = new List<byte>();
		this.nextAgentIndexUpdate = 0;
		this.nextAgentIndexThink = 0;
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000CD451 File Offset: 0x000CB651
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000CD45F File Offset: 0x000CB65F
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000CD46D File Offset: 0x000CB66D
	public static GameAgentManager Get(GameEntity gameEntity)
	{
		if (!(gameEntity == null) && !(gameEntity.manager == null))
		{
			return gameEntity.manager.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000CD493 File Offset: 0x000CB693
	public List<GameAgent> GetAgents()
	{
		return this.agents;
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000CD49B File Offset: 0x000CB69B
	public int GetGameAgentCount()
	{
		return this.agents.Count;
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000CD4A8 File Offset: 0x000CB6A8
	public void AddGameAgent(GameAgent gameAgent)
	{
		this.agents.Add(gameAgent);
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000CD4B6 File Offset: 0x000CB6B6
	public void RemoveGameAgent(GameAgent gameAgent)
	{
		this.agents.Remove(gameAgent);
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000CD4C5 File Offset: 0x000CB6C5
	public GameAgent GetGameAgent(GameEntityId id)
	{
		return this.entityManager.GetGameEntity(id).GetComponent<GameAgent>();
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000CD4D8 File Offset: 0x000CB6D8
	public void Tick()
	{
		if (this.IsAuthority())
		{
			int num = Mathf.Min(1, this.agents.Count);
			for (int i = 0; i < num; i++)
			{
				if (this.nextAgentIndexThink >= this.agents.Count)
				{
					this.nextAgentIndexThink = 0;
				}
				this.agents[this.nextAgentIndexThink].OnThink(Time.deltaTime);
				this.nextAgentIndexThink++;
			}
		}
		for (int j = 0; j < this.agents.Count; j++)
		{
			if (this.agents[j] != null)
			{
				this.agents[j].OnUpdate();
			}
		}
		if (this.IsAuthority())
		{
			if (this.netIdsForDestination.Count > 0 && Time.time > this.lastDestinationSentTime + this.destinationCooldown)
			{
				this.lastDestinationSentTime = Time.time;
				base.SendRPC("ApplyDestinationRPC", 0, new object[]
				{
					this.netIdsForDestination.ToArray(),
					this.destinationsForDestination.ToArray()
				});
				this.netIdsForDestination.Clear();
				this.destinationsForDestination.Clear();
			}
			if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSentTime + this.stateCooldown)
			{
				this.lastStateSentTime = Time.time;
				base.SendRPC("ApplyStateRPC", 0, new object[]
				{
					this.netIdsForState.ToArray(),
					this.statesForState.ToArray()
				});
				this.netIdsForState.Clear();
				this.statesForState.Clear();
			}
			if (this.netIdsForBehavior.Count > 0 && Time.time > this.lastBehaviorSentTime + this.behaviorCooldown)
			{
				this.lastBehaviorSentTime = Time.time;
				base.SendRPC("ApplyBehaviorRPC", 0, new object[]
				{
					this.netIdsForBehavior.ToArray(),
					this.behaviorsForBehavior.ToArray()
				});
				this.netIdsForBehavior.Clear();
				this.behaviorsForBehavior.Clear();
			}
		}
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000CD6E7 File Offset: 0x000CB8E7
	public bool IsAuthority()
	{
		return this.entityManager.IsAuthority();
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000CD6F4 File Offset: 0x000CB8F4
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000CD702 File Offset: 0x000CB902
	public bool IsAuthorityPlayer(Player player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000CD710 File Offset: 0x000CB910
	public Player GetAuthorityPlayer()
	{
		return this.entityManager.GetAuthorityPlayer();
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x000CD71D File Offset: 0x000CB91D
	public bool IsZoneActive()
	{
		return this.entityManager.IsZoneActive();
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000CD72A File Offset: 0x000CB92A
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.entityManager.IsPositionInZone(pos);
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000CD738 File Offset: 0x000CB938
	public bool IsValidClientRPC(Player sender)
	{
		return this.entityManager.IsValidClientRPC(sender);
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000CD746 File Offset: 0x000CB946
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000CD755 File Offset: 0x000CB955
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000CD765 File Offset: 0x000CB965
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000CD774 File Offset: 0x000CB974
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.entityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x060026A7 RID: 9895 RVA: 0x000CD782 File Offset: 0x000CB982
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000CD791 File Offset: 0x000CB991
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000CD7A1 File Offset: 0x000CB9A1
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000CD7B0 File Offset: 0x000CB9B0
	public void RequestDestination(GameAgent agent, Vector3 dest)
	{
		if (!this.IsAuthority())
		{
			Debug.LogError("RequestDestination should only be called from the master client");
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForDestination.Contains(netIdFromEntityId))
		{
			this.destinationsForDestination[this.netIdsForDestination.IndexOf(netIdFromEntityId)] = dest;
			return;
		}
		this.netIdsForDestination.Add(netIdFromEntityId);
		this.destinationsForDestination.Add(dest);
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000CD828 File Offset: 0x000CBA28
	[PunRPC]
	public void ApplyDestinationRPC(int[] netEntityId, Vector3[] dest, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyDestination))
		{
			return;
		}
		if (netEntityId == null || dest == null || netEntityId.Length != dest.Length)
		{
			return;
		}
		int i = 0;
		while (i < netEntityId.Length)
		{
			if (this.IsValidClientRPC(info.Sender, netEntityId[i], dest[i]))
			{
				int num = i;
				float num2 = 10000f;
				if (dest[num].IsValid(num2))
				{
					i++;
					continue;
				}
			}
			return;
		}
		for (int j = 0; j < netEntityId.Length; j++)
		{
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[j]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.ApplyDestination(dest[j]);
		}
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000CD8EC File Offset: 0x000CBAEC
	public void RequestState(GameAgent agent, byte state)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = state;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(state);
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000CD958 File Offset: 0x000CBB58
	[PunRPC]
	public void ApplyStateRPC(int[] netEntityId, byte[] state, PhotonMessageInfo info)
	{
		if (netEntityId == null || state == null || netEntityId.Length != state.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.OnBodyStateChanged(state[i]);
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000CD9E0 File Offset: 0x000CBBE0
	public void RequestBehavior(GameAgent agent, byte behavior)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForBehavior.Contains(netIdFromEntityId))
		{
			this.behaviorsForBehavior[this.netIdsForBehavior.IndexOf(netIdFromEntityId)] = behavior;
			return;
		}
		this.netIdsForBehavior.Add(netIdFromEntityId);
		this.behaviorsForBehavior.Add(behavior);
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000CDA4C File Offset: 0x000CBC4C
	[PunRPC]
	public void ApplyBehaviorRPC(int[] netEntityId, byte[] behavior, PhotonMessageInfo info)
	{
		if (netEntityId == null || behavior == null || netEntityId.Length != behavior.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyBehaviour))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component != null)
			{
				component.OnBehaviorStateChanged(behavior[i]);
			}
		}
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000CDAD4 File Offset: 0x000CBCD4
	public void RequestTarget(GameAgent agent, NetPlayer player)
	{
		if (player == agent.targetPlayer)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.targetPlayer = player;
		base.SendRPC("ApplyTargetRPC", 1, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			(player == null) ? null : player.GetPlayerRef()
		});
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000CDB44 File Offset: 0x000CBD44
	[PunRPC]
	public void ApplyTargetRPC(int agentNetId, Player player, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, agentNetId) || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget) || player == null)
		{
			return;
		}
		GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
		if (gameEntity == null)
		{
			return;
		}
		GameAgent component = gameEntity.GetComponent<GameAgent>();
		if (component == null)
		{
			return;
		}
		component.targetPlayer = NetPlayer.Get(player);
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000CDBB4 File Offset: 0x000CBDB4
	public void RequestJump(GameAgent agent, Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.OnJumpRequested(start, end, heightScale, speedScale);
		base.SendRPC("ApplyJumpRPC", 1, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			start,
			end,
			heightScale,
			speedScale
		});
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000CDC38 File Offset: 0x000CBE38
	[PunRPC]
	public void ApplyJumpRPC(int agentNetId, Vector3 start, Vector3 end, float heightScale, float speedScale, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, agentNetId) && !this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget))
		{
			float num = 10000f;
			if (start.IsValid(num))
			{
				float num2 = 10000f;
				if (end.IsValid(num2) && this.entityManager.IsPositionInZone(start) && this.entityManager.IsPositionInZone(end) && this.entityManager.IsEntityNearPosition(agentNetId, start, 16f) && heightScale <= 5f && speedScale <= 5f)
				{
					if ((end - start).sqrMagnitude > 625f)
					{
						return;
					}
					GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
					if (gameEntity == null)
					{
						return;
					}
					GameAgent component = gameEntity.GetComponent<GameAgent>();
					if (component == null)
					{
						return;
					}
					component.OnJumpRequested(start, end, heightScale, speedScale);
					return;
				}
			}
		}
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000CDD20 File Offset: 0x000CBF20
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		int num = Mathf.Min(4, this.agents.Count);
		stream.SendNext(num);
		for (int i = 0; i < num; i++)
		{
			if (this.nextAgentIndexUpdate >= this.agents.Count)
			{
				this.nextAgentIndexUpdate = 0;
			}
			stream.SendNext(this.entityManager.GetNetIdFromEntityId(this.agents[this.nextAgentIndexUpdate].entity.id));
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.agents[this.nextAgentIndexUpdate].transform.position);
			stream.SendNext(num2);
			int num3 = BitPackUtils.PackQuaternionForNetwork(this.agents[this.nextAgentIndexUpdate].transform.rotation);
			stream.SendNext(num3);
			this.nextAgentIndexUpdate++;
		}
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x000CDE10 File Offset: 0x000CC010
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int netId = (int)stream.ReceiveNext();
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork((int)stream.ReceiveNext());
			if (this.IsPositionInZone(vector) && this.entityManager.IsValidNetId(netId))
			{
				GameEntityId entityIdFromNetId = this.entityManager.GetEntityIdFromNetId(netId);
				GameAgent gameAgent = this.GetGameAgent(entityIdFromNetId);
				if (gameAgent != null)
				{
					gameAgent.ApplyNetworkUpdate(vector, rotation);
				}
			}
		}
	}

	// Token: 0x060026B9 RID: 9913 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x0400326D RID: 12909
	public const float MAX_JUMP_DISTANCE = 25f;

	// Token: 0x0400326E RID: 12910
	public GameEntityManager entityManager;

	// Token: 0x0400326F RID: 12911
	public PhotonView photonView;

	// Token: 0x04003270 RID: 12912
	private List<GameAgent> agents;

	// Token: 0x04003271 RID: 12913
	private float lastDestinationSentTime;

	// Token: 0x04003272 RID: 12914
	private float destinationCooldown;

	// Token: 0x04003273 RID: 12915
	private List<int> netIdsForDestination;

	// Token: 0x04003274 RID: 12916
	private List<Vector3> destinationsForDestination;

	// Token: 0x04003275 RID: 12917
	private List<int> netIdsForState;

	// Token: 0x04003276 RID: 12918
	private List<byte> statesForState;

	// Token: 0x04003277 RID: 12919
	private float lastStateSentTime;

	// Token: 0x04003278 RID: 12920
	private float stateCooldown;

	// Token: 0x04003279 RID: 12921
	private List<int> netIdsForBehavior;

	// Token: 0x0400327A RID: 12922
	private List<byte> behaviorsForBehavior;

	// Token: 0x0400327B RID: 12923
	private float lastBehaviorSentTime;

	// Token: 0x0400327C RID: 12924
	private float behaviorCooldown = 0.25f;

	// Token: 0x0400327D RID: 12925
	private const int MAX_UPDATES_PER_FRAME = 4;

	// Token: 0x0400327E RID: 12926
	private int nextAgentIndexUpdate;

	// Token: 0x0400327F RID: 12927
	private const int MAX_THINK_PER_FRAME = 1;

	// Token: 0x04003280 RID: 12928
	private int nextAgentIndexThink;

	// Token: 0x04003282 RID: 12930
	public CallLimitersList<CallLimiter, GameAgentManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameAgentManager.RPC>();

	// Token: 0x020005FC RID: 1532
	public enum RPC
	{
		// Token: 0x04003284 RID: 12932
		ApplyDestination,
		// Token: 0x04003285 RID: 12933
		ApplyState,
		// Token: 0x04003286 RID: 12934
		ApplyBehaviour,
		// Token: 0x04003287 RID: 12935
		ApplyImpact,
		// Token: 0x04003288 RID: 12936
		ApplyTarget
	}
}
