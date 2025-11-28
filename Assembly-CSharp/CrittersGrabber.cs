using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class CrittersGrabber : CrittersActor
{
	// Token: 0x060001B6 RID: 438 RVA: 0x0000A9F1 File Offset: 0x00008BF1
	public override void ProcessRemote()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000AA0B File Offset: 0x00008C0B
	public override bool ProcessLocal()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
		return base.ProcessLocal();
	}

	// Token: 0x04000201 RID: 513
	public Transform grabPosition;

	// Token: 0x04000202 RID: 514
	public bool grabbing;

	// Token: 0x04000203 RID: 515
	public float grabDistance;

	// Token: 0x04000204 RID: 516
	public List<CrittersActor> grabbedActors = new List<CrittersActor>();

	// Token: 0x04000205 RID: 517
	public bool isLeft;
}
