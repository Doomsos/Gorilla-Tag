using System;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class MonkeBallPlayer : MonoBehaviour
{
	// Token: 0x06002278 RID: 8824 RVA: 0x000B491B File Offset: 0x000B2B1B
	private void Awake()
	{
		if (this.gamePlayer == null)
		{
			this.gamePlayer = base.GetComponent<GameBallPlayer>();
		}
	}

	// Token: 0x04002D07 RID: 11527
	public GameBallPlayer gamePlayer;

	// Token: 0x04002D08 RID: 11528
	public MonkeBallGoalZone currGoalZone;
}
