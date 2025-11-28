using System;

// Token: 0x02000904 RID: 2308
public interface ITimeOfDaySystem
{
	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x06003B0C RID: 15116
	double currentTimeInSeconds { get; }

	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x06003B0D RID: 15117
	double totalTimeInSeconds { get; }
}
