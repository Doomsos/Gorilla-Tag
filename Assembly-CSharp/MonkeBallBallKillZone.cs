using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200054B RID: 1355
public class MonkeBallBallKillZone : MonoBehaviour
{
	// Token: 0x0600222C RID: 8748 RVA: 0x000B2C3C File Offset: 0x000B0E3C
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.RequestResetBall(component.id, -1);
				return;
			}
			GameBallManager.Instance.RequestSetBallPosition(component.id);
		}
	}
}
