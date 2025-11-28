using System;

// Token: 0x02000BFE RID: 3070
internal interface ITickSystemPost
{
	// Token: 0x1700071C RID: 1820
	// (get) Token: 0x06004BDE RID: 19422
	// (set) Token: 0x06004BDF RID: 19423
	bool PostTickRunning { get; set; }

	// Token: 0x06004BE0 RID: 19424
	void PostTick();
}
