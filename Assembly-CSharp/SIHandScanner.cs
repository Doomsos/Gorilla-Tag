using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200011C RID: 284
public class SIHandScanner : MonoBehaviour
{
	// Token: 0x0600073A RID: 1850 RVA: 0x00027C22 File Offset: 0x00025E22
	public void HandScanned(SIPlayer scannedPlayer)
	{
		if (!scannedPlayer.gamePlayer.IsLocal())
		{
			return;
		}
		this.onHandScanned.Invoke(NetworkSystem.Instance.LocalPlayerID);
	}

	// Token: 0x0400091B RID: 2331
	public UnityEvent<int> onHandScanned;
}
