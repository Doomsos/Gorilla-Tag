using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200079A RID: 1946
public class GorillaJoinTeamBox : GorillaTriggerBox
{
	// Token: 0x060032F1 RID: 13041 RVA: 0x001133D1 File Offset: 0x001115D1
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("GorillaGameManager").GetComponent<GorillaGameManager>() != null)
		{
			bool inRoom = PhotonNetwork.InRoom;
		}
	}

	// Token: 0x04004160 RID: 16736
	public bool joinRedTeam;
}
