using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200073D RID: 1853
public class GRToolUpgradeStationDepositBox : MonoBehaviour
{
	// Token: 0x06002FDF RID: 12255 RVA: 0x00105E48 File Offset: 0x00104048
	public void OnTriggerEnter(Collider other)
	{
		GRTool component = other.attachedRigidbody.GetComponent<GRTool>();
		if (component.IsNotNull() && component.gameEntity.IsNotNull() && component.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber && component.gameEntity.IsHeldByLocalPlayer())
		{
			Debug.LogError("Tool Deposited");
			this.upgradeStation.ToolInserted(component);
		}
	}

	// Token: 0x04003ED0 RID: 16080
	public GRToolUpgradeStation upgradeStation;
}
