using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020003D8 RID: 984
public static class NetworkSystemRaiseEvent
{
	// Token: 0x06001813 RID: 6163 RVA: 0x000818EE File Offset: 0x0007FAEE
	public static void RaiseEvent(byte code, object data)
	{
		PhotonNetwork.RaiseEvent(code, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x00081904 File Offset: 0x0007FB04
	public static void RaiseEvent(byte code, object data, NetEventOptions options, bool reliable)
	{
		PhotonNetwork.RaiseEvent(code, data, new RaiseEventOptions
		{
			TargetActors = options.TargetActors,
			Receivers = (byte)options.Reciever,
			Flags = options.Flags
		}, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
	}

	// Token: 0x04002186 RID: 8582
	public static readonly NetEventOptions neoOthers = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.others
	};

	// Token: 0x04002187 RID: 8583
	public static readonly NetEventOptions neoMaster = new NetEventOptions
	{
		Reciever = NetEventOptions.RecieverTarget.master
	};

	// Token: 0x04002188 RID: 8584
	public static readonly NetEventOptions neoTarget = new NetEventOptions
	{
		TargetActors = new int[1]
	};

	// Token: 0x04002189 RID: 8585
	public static readonly NetEventOptions newWeb = new NetEventOptions
	{
		Flags = new WebFlags(3)
	};
}
