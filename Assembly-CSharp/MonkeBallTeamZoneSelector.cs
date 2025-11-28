using System;
using UnityEngine;

// Token: 0x02000558 RID: 1368
public class MonkeBallTeamZoneSelector : MonoBehaviour
{
	// Token: 0x06002295 RID: 8853 RVA: 0x000B4D34 File Offset: 0x000B2F34
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.IsLocalPlayer() && gamePlayer.teamId != this.teamId)
		{
			MonkeBallGame.Instance.RequestSetTeam(this.teamId);
		}
	}

	// Token: 0x04002D2A RID: 11562
	public int teamId;
}
