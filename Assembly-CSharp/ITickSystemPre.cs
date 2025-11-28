using System;

// Token: 0x02000BFC RID: 3068
internal interface ITickSystemPre
{
	// Token: 0x1700071A RID: 1818
	// (get) Token: 0x06004BD8 RID: 19416
	// (set) Token: 0x06004BD9 RID: 19417
	bool PreTickRunning { get; set; }

	// Token: 0x06004BDA RID: 19418
	void PreTick();
}
