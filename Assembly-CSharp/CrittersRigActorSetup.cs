using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class CrittersRigActorSetup : MonoBehaviour
{
	// Token: 0x060002B9 RID: 697 RVA: 0x00010BCB File Offset: 0x0000EDCB
	public void OnEnable()
	{
		CrittersManager.RegisterRigActorSetup(this);
	}

	// Token: 0x060002BA RID: 698 RVA: 0x00010BD4 File Offset: 0x0000EDD4
	public void OnDisable()
	{
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			this.rigActors[i].actorSet = null;
		}
	}

	// Token: 0x060002BB RID: 699 RVA: 0x00010C08 File Offset: 0x0000EE08
	private CrittersActor RefreshActorForIndex(int index)
	{
		CrittersRigActorSetup.RigActor rigActor = this.rigActors[index];
		if (rigActor.actorSet.IsNotNull())
		{
			rigActor.actorSet.gameObject.SetActive(false);
		}
		CrittersActor crittersActor = CrittersManager.instance.SpawnActor(rigActor.type, rigActor.subIndex);
		if (crittersActor.IsNull())
		{
			return null;
		}
		crittersActor.isOnPlayer = true;
		crittersActor.rigIndex = index;
		crittersActor.rigPlayerId = this.myRig.Creator.ActorNumber;
		if (crittersActor.rigPlayerId == -1 && PhotonNetwork.InRoom)
		{
			crittersActor.rigPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
		}
		crittersActor.PlacePlayerCrittersActor();
		return crittersActor;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00010CB0 File Offset: 0x0000EEB0
	public void CheckUpdate(ref List<object> refActorData, bool forceCheck = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			CrittersRigActorSetup.RigActor rigActor = this.rigActors[i];
			RigContainer rigContainer;
			if (forceCheck || rigActor.actorSet == null || (rigActor.actorSet.rigPlayerId != this.myRig.Creator.ActorNumber && VRRigCache.Instance.TryGetVrrig(this.myRig.Creator, out rigContainer) && CrittersManager.instance.rigSetupByRig.ContainsKey(this.myRig)))
			{
				CrittersActor crittersActor = this.RefreshActorForIndex(i);
				if (crittersActor != null)
				{
					crittersActor.AddPlayerCrittersActorDataToList(ref refActorData);
				}
			}
		}
	}

	// Token: 0x0400032A RID: 810
	public CrittersRigActorSetup.RigActor[] rigActors;

	// Token: 0x0400032B RID: 811
	public List<object> rigActorData = new List<object>();

	// Token: 0x0400032C RID: 812
	public VRRig myRig;

	// Token: 0x02000070 RID: 112
	[Serializable]
	public struct RigActor
	{
		// Token: 0x0400032D RID: 813
		public Transform location;

		// Token: 0x0400032E RID: 814
		public CrittersActor.CrittersActorType type;

		// Token: 0x0400032F RID: 815
		public int subIndex;

		// Token: 0x04000330 RID: 816
		public CrittersActor actorSet;
	}
}
