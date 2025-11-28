using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000BE5 RID: 3045
internal class PaintbrawlRPCs : RPCNetworkBase
{
	// Token: 0x06004B2D RID: 19245 RVA: 0x00188C4D File Offset: 0x00186E4D
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.paintbrawlManager = (GorillaPaintbrawlManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06004B2E RID: 19246 RVA: 0x00188C68 File Offset: 0x00186E68
	[PunRPC]
	public void RPC_ReportSlingshotHit(Player taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RPC_ReportSlingshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayer);
		PhotonMessageInfoWrapped info2 = new PhotonMessageInfoWrapped(info);
		this.paintbrawlManager.ReportSlingshotHit(player, hitLocation, projectileCount, info2);
	}

	// Token: 0x04005B55 RID: 23381
	private GameModeSerializer serializer;

	// Token: 0x04005B56 RID: 23382
	private GorillaPaintbrawlManager paintbrawlManager;
}
