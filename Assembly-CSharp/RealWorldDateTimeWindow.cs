using System;
using UniLabs.Time;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class RealWorldDateTimeWindow : ScriptableObject
{
	// Token: 0x0600035D RID: 861 RVA: 0x00014052 File Offset: 0x00012252
	public bool MatchesDate(DateTime utcDate)
	{
		return this.startTime <= utcDate && this.endTime >= utcDate;
	}

	// Token: 0x040003EB RID: 1003
	[SerializeField]
	private UDateTime startTime;

	// Token: 0x040003EC RID: 1004
	[SerializeField]
	private UDateTime endTime;
}
