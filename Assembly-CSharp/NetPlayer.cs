using System;
using System.Collections.Generic;
using Fusion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003C5 RID: 965
[Serializable]
public abstract class NetPlayer : ObjectPoolEvents
{
	// Token: 0x1700024E RID: 590
	// (get) Token: 0x06001729 RID: 5929
	public abstract bool IsValid { get; }

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x0600172A RID: 5930
	public abstract int ActorNumber { get; }

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x0600172B RID: 5931
	public abstract string UserId { get; }

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x0600172C RID: 5932
	public abstract bool IsMasterClient { get; }

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x0600172D RID: 5933
	public abstract bool IsLocal { get; }

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x0600172E RID: 5934
	public abstract bool IsNull { get; }

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x0600172F RID: 5935
	public abstract string NickName { get; }

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001730 RID: 5936 RVA: 0x000808D3 File Offset: 0x0007EAD3
	// (set) Token: 0x06001731 RID: 5937 RVA: 0x000808DB File Offset: 0x0007EADB
	public virtual string SanitizedNickName { get; set; } = string.Empty;

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x06001732 RID: 5938
	public abstract string DefaultName { get; }

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06001733 RID: 5939
	public abstract bool InRoom { get; }

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06001734 RID: 5940 RVA: 0x000808E4 File Offset: 0x0007EAE4
	// (set) Token: 0x06001735 RID: 5941 RVA: 0x000808EC File Offset: 0x0007EAEC
	public virtual float JoinedTime { get; private set; }

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06001736 RID: 5942 RVA: 0x000808F5 File Offset: 0x0007EAF5
	// (set) Token: 0x06001737 RID: 5943 RVA: 0x000808FD File Offset: 0x0007EAFD
	public virtual float LeftTime { get; private set; }

	// Token: 0x06001738 RID: 5944
	public abstract bool Equals(NetPlayer myPlayer, NetPlayer other);

	// Token: 0x06001739 RID: 5945 RVA: 0x00080906 File Offset: 0x0007EB06
	public virtual void OnReturned()
	{
		this.LeftTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus != null)
		{
			singleCallRPCStatus.Clear();
		}
		this.SanitizedNickName = string.Empty;
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x0008092F File Offset: 0x0007EB2F
	public virtual void OnTaken()
	{
		this.JoinedTime = Time.time;
		HashSet<int> singleCallRPCStatus = this.SingleCallRPCStatus;
		if (singleCallRPCStatus == null)
		{
			return;
		}
		singleCallRPCStatus.Clear();
	}

	// Token: 0x0600173B RID: 5947 RVA: 0x0008094C File Offset: 0x0007EB4C
	public virtual bool CheckSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		return this.SingleCallRPCStatus.Contains((int)RPCType);
	}

	// Token: 0x0600173C RID: 5948 RVA: 0x0008095A File Offset: 0x0007EB5A
	public virtual void ReceivedSingleCallRPC(NetPlayer.SingleCallRPC RPCType)
	{
		this.SingleCallRPCStatus.Add((int)RPCType);
	}

	// Token: 0x0600173D RID: 5949 RVA: 0x00080969 File Offset: 0x0007EB69
	public Player GetPlayerRef()
	{
		return (this as PunNetPlayer).PlayerRef;
	}

	// Token: 0x0600173E RID: 5950 RVA: 0x00080976 File Offset: 0x0007EB76
	public string ToStringFull()
	{
		return string.Format("#{0: 0:00} '{1}', Not sure what to do with inactive yet, Or custom props?", this.ActorNumber, this.NickName);
	}

	// Token: 0x0600173F RID: 5951 RVA: 0x00080993 File Offset: 0x0007EB93
	public static implicit operator NetPlayer(Player player)
	{
		Utils.Log("Using an implicit cast from Player to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x000809B6 File Offset: 0x0007EBB6
	public static implicit operator NetPlayer(PlayerRef player)
	{
		Utils.Log("Using an implicit cast from PlayerRef to NetPlayer. Please make sure this was intended as this has potential to cause errors when switching between network backends");
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x000809D9 File Offset: 0x0007EBD9
	public static NetPlayer Get(Player player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x000809F2 File Offset: 0x0007EBF2
	public static NetPlayer Get(PlayerRef player)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(player) : null) ?? null;
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00080A0B File Offset: 0x0007EC0B
	public static NetPlayer Get(int actorNr)
	{
		NetworkSystem instance = NetworkSystem.Instance;
		return ((instance != null) ? instance.GetPlayer(actorNr) : null) ?? null;
	}

	// Token: 0x04002138 RID: 8504
	private HashSet<int> SingleCallRPCStatus = new HashSet<int>(5);

	// Token: 0x020003C6 RID: 966
	public enum SingleCallRPC
	{
		// Token: 0x0400213A RID: 8506
		CMS_RequestRoomInitialization,
		// Token: 0x0400213B RID: 8507
		CMS_RequestTriggerHistory,
		// Token: 0x0400213C RID: 8508
		CMS_SyncTriggerHistory,
		// Token: 0x0400213D RID: 8509
		CMS_SyncTriggerCounts,
		// Token: 0x0400213E RID: 8510
		RankedSendScoreToLateJoiner,
		// Token: 0x0400213F RID: 8511
		Count
	}
}
