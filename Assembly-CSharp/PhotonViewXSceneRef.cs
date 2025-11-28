using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200033A RID: 826
public class PhotonViewXSceneRef : MonoBehaviour
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060013E9 RID: 5097 RVA: 0x0007358C File Offset: 0x0007178C
	public PhotonView photonView
	{
		get
		{
			PhotonView result;
			if (this.reference.TryResolve<PhotonView>(out result))
			{
				return result;
			}
			return null;
		}
	}

	// Token: 0x04001E79 RID: 7801
	[SerializeField]
	private XSceneRef reference;
}
