using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DDD RID: 3549
	public class EnvItem : MonoBehaviour, IPunInstantiateMagicCallback
	{
		// Token: 0x0600582C RID: 22572 RVA: 0x00002789 File Offset: 0x00000989
		public void OnEnable()
		{
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDisable()
		{
		}

		// Token: 0x0600582E RID: 22574 RVA: 0x001C2E2C File Offset: 0x001C102C
		public void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			object[] instantiationData = info.photonView.InstantiationData;
			this.spawnedByPhotonViewId = (int)instantiationData[0];
		}

		// Token: 0x04006577 RID: 25975
		public int spawnedByPhotonViewId;
	}
}
