using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EF8 RID: 3832
	public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
	{
		// Token: 0x06006037 RID: 24631 RVA: 0x001F0900 File Offset: 0x001EEB00
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.makeSureThisIsEnabled != null)
			{
				this.makeSureThisIsEnabled.SetActive(true);
			}
			GameObject[] array = this.makeSureTheseAreEnabled;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.componentTypeToRemove != "" && this.componentTarget.GetComponent(this.componentTypeToRemove) != null)
				{
					Object.Destroy(this.componentTarget.GetComponent(this.componentTypeToRemove));
				}
				PhotonNetwork.Disconnect();
				SkinnedMeshRenderer[] array2 = this.photonNetworkController.offlineVRRig;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].enabled = true;
				}
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		// Token: 0x04006EE0 RID: 28384
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04006EE1 RID: 28385
		public GameObject offlineVRRig;

		// Token: 0x04006EE2 RID: 28386
		public GameObject makeSureThisIsEnabled;

		// Token: 0x04006EE3 RID: 28387
		public GameObject[] makeSureTheseAreEnabled;

		// Token: 0x04006EE4 RID: 28388
		public string componentTypeToRemove;

		// Token: 0x04006EE5 RID: 28389
		public GameObject componentTarget;
	}
}
