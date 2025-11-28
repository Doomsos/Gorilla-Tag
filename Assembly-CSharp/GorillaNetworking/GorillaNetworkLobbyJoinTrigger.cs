using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EFD RID: 3837
	public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x04006EFC RID: 28412
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04006EFD RID: 28413
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04006EFE RID: 28414
		public string gameModeName;

		// Token: 0x04006EFF RID: 28415
		public PhotonNetworkController photonNetworkController;

		// Token: 0x04006F00 RID: 28416
		public string componentTypeToRemove;

		// Token: 0x04006F01 RID: 28417
		public GameObject componentRemoveTarget;

		// Token: 0x04006F02 RID: 28418
		public string componentTypeToAdd;

		// Token: 0x04006F03 RID: 28419
		public GameObject componentAddTarget;

		// Token: 0x04006F04 RID: 28420
		public GameObject gorillaParent;

		// Token: 0x04006F05 RID: 28421
		public GameObject joinFailedBlock;
	}
}
