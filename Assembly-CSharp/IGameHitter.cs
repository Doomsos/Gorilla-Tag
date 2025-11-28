using System;
using UnityEngine;

// Token: 0x0200061E RID: 1566
public interface IGameHitter
{
	// Token: 0x060027D4 RID: 10196
	void OnSuccessfulHit(GameHitData hit);

	// Token: 0x060027D5 RID: 10197 RVA: 0x00002789 File Offset: 0x00000989
	void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
	}
}
