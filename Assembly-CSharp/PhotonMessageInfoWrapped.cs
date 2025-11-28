using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020003C4 RID: 964
public struct PhotonMessageInfoWrapped
{
	// Token: 0x1700024D RID: 589
	// (get) Token: 0x06001723 RID: 5923 RVA: 0x000807F1 File Offset: 0x0007E9F1
	public double SentServerTime
	{
		get
		{
			return this.sentTick / 1000.0;
		}
	}

	// Token: 0x06001724 RID: 5924 RVA: 0x00080805 File Offset: 0x0007EA05
	public PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		Player sender = info.Sender;
		this.senderID = ((sender != null) ? sender.ActorNumber : -1);
		this.Sender = NetPlayer.Get(info.Sender);
		this.sentTick = info.SentServerTimestamp;
		this.punInfo = info;
	}

	// Token: 0x06001725 RID: 5925 RVA: 0x00080844 File Offset: 0x0007EA44
	public PhotonMessageInfoWrapped(RpcInfo info)
	{
		this.senderID = info.Source.PlayerId;
		this.Sender = NetPlayer.Get(info.Source);
		this.sentTick = info.Tick.Raw;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001726 RID: 5926 RVA: 0x00080891 File Offset: 0x0007EA91
	public PhotonMessageInfoWrapped(int playerID, int tick)
	{
		this.senderID = playerID;
		this.Sender = NetworkSystem.Instance.GetPlayer(this.senderID);
		this.sentTick = tick;
		this.punInfo = default(PhotonMessageInfo);
	}

	// Token: 0x06001727 RID: 5927 RVA: 0x000808C3 File Offset: 0x0007EAC3
	public static implicit operator PhotonMessageInfoWrapped(PhotonMessageInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x06001728 RID: 5928 RVA: 0x000808CB File Offset: 0x0007EACB
	public static implicit operator PhotonMessageInfoWrapped(RpcInfo info)
	{
		return new PhotonMessageInfoWrapped(info);
	}

	// Token: 0x04002131 RID: 8497
	public readonly int senderID;

	// Token: 0x04002132 RID: 8498
	public readonly int sentTick;

	// Token: 0x04002133 RID: 8499
	public readonly PhotonMessageInfo punInfo;

	// Token: 0x04002134 RID: 8500
	public readonly NetPlayer Sender;
}
