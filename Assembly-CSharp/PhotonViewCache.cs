using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000760 RID: 1888
public class PhotonViewCache : MonoBehaviour, IPunInstantiateMagicCallback
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x060030D6 RID: 12502 RVA: 0x0010A77C File Offset: 0x0010897C
	// (set) Token: 0x060030D7 RID: 12503 RVA: 0x0010A784 File Offset: 0x00108984
	public bool Initialized { get; private set; }

	// Token: 0x060030D8 RID: 12504 RVA: 0x00002789 File Offset: 0x00000989
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x04003FBD RID: 16317
	private PhotonView[] m_photonViews;

	// Token: 0x04003FBE RID: 16318
	[SerializeField]
	private bool m_isRoomObject;
}
