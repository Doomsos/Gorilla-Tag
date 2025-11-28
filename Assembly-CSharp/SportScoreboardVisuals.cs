using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020008E7 RID: 2279
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x06003A5B RID: 14939 RVA: 0x0013411C File Offset: 0x0013231C
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x040049A4 RID: 18852
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x040049A5 RID: 18853
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x040049A6 RID: 18854
	[SerializeField]
	private int TeamIndex;
}
