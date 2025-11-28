using System;
using UnityEngine;

// Token: 0x020006D7 RID: 1751
public struct GameNoiseEvent
{
	// Token: 0x06002CD6 RID: 11478 RVA: 0x000F2EE2 File Offset: 0x000F10E2
	public bool IsValid()
	{
		return (float)(Time.timeAsDouble - this.eventTime) <= this.duration;
	}

	// Token: 0x04003A3D RID: 14909
	public Vector3 position;

	// Token: 0x04003A3E RID: 14910
	public double eventTime;

	// Token: 0x04003A3F RID: 14911
	public float duration;

	// Token: 0x04003A40 RID: 14912
	public float magnitude;
}
