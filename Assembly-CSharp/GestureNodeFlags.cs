using System;

// Token: 0x02000258 RID: 600
[Flags]
public enum GestureNodeFlags : uint
{
	// Token: 0x04001338 RID: 4920
	None = 0U,
	// Token: 0x04001339 RID: 4921
	HandLeft = 1U,
	// Token: 0x0400133A RID: 4922
	HandRight = 2U,
	// Token: 0x0400133B RID: 4923
	HandOpen = 4U,
	// Token: 0x0400133C RID: 4924
	HandClosed = 8U,
	// Token: 0x0400133D RID: 4925
	DigitOpen = 16U,
	// Token: 0x0400133E RID: 4926
	DigitClosed = 32U,
	// Token: 0x0400133F RID: 4927
	DigitBent = 64U,
	// Token: 0x04001340 RID: 4928
	TowardFace = 128U,
	// Token: 0x04001341 RID: 4929
	AwayFromFace = 256U,
	// Token: 0x04001342 RID: 4930
	AxisWorldUp = 512U,
	// Token: 0x04001343 RID: 4931
	AxisWorldDown = 1024U
}
