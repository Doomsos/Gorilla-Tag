using System;
using UnityEngine;

// Token: 0x020007B9 RID: 1977
public class GorillaTagCompetitivePrivateRoomBlocker : MonoBehaviour
{
	// Token: 0x06003416 RID: 13334 RVA: 0x001181AB File Offset: 0x001163AB
	private void Update()
	{
		this.blocker.SetActive(NetworkSystem.Instance.SessionIsPrivate);
	}

	// Token: 0x04004266 RID: 16998
	[SerializeField]
	private GameObject blocker;
}
