using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B99 RID: 2969
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x0600495D RID: 18781 RVA: 0x00181437 File Offset: 0x0017F637
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.NetViews = base.GetComponents<PhotonView>();
		}
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RegisterViews(this.NetViews);
	}

	// Token: 0x0600495E RID: 18782 RVA: 0x00181461 File Offset: 0x0017F661
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGaurdHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x040059E4 RID: 23012
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x040059E5 RID: 23013
	[SerializeField]
	private bool autoRegisterAll = true;
}
