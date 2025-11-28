using System;

// Token: 0x02000BFD RID: 3069
internal interface ITickSystemTick
{
	// Token: 0x1700071B RID: 1819
	// (get) Token: 0x06004BDB RID: 19419
	// (set) Token: 0x06004BDC RID: 19420
	bool TickRunning { get; set; }

	// Token: 0x06004BDD RID: 19421
	void Tick();
}
