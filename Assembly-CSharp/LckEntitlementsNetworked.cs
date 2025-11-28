using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class LckEntitlementsNetworked : MonoBehaviour
{
	// Token: 0x0600151D RID: 5405 RVA: 0x00077B44 File Offset: 0x00075D44
	public void Awake()
	{
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			Debug.LogError("LCK: Unable to find VRRigSerializer for LckEntitlementsNetworked.");
			return;
		}
		InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		if (succesfullSpawnEvent == null)
		{
			return;
		}
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
		succesfullSpawnEvent.Add(inAction);
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00077BA8 File Offset: 0x00075DA8
	private void OnSuccessfulSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		if (LckEntitlementsManager.Instance == null)
		{
			Debug.LogError("LCK: LckEntitlementsManager.Instance is not available in the scene!");
			return;
		}
		string userId = this.m_rigNetworkController.VRRig.OwningNetPlayer.UserId;
		if (userId.IsNullOrEmpty())
		{
			Debug.LogError("LCK: owningUserId is null on spawn. Cannot process entitlements.");
			return;
		}
		if (rig.Rig.isLocal)
		{
			LckEntitlementsManager.Instance.OnLocalPlayerSpawned(userId);
			return;
		}
		LckEntitlementsManager.Instance.OnRemotePlayerSpawned(userId);
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00077C1C File Offset: 0x00075E1C
	private void OnDestroy()
	{
		if (this.m_rigNetworkController != null && this.m_rigNetworkController.SuccesfullSpawnEvent != null)
		{
			ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
			InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
			succesfullSpawnEvent.Remove(inAction);
		}
	}

	// Token: 0x04001FA6 RID: 8102
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;
}
