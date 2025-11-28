using System;

namespace Viveport
{
	// Token: 0x02000D12 RID: 3346
	public class Leaderboard
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x0600512E RID: 20782 RVA: 0x001A22B3 File Offset: 0x001A04B3
		// (set) Token: 0x0600512F RID: 20783 RVA: 0x001A22BB File Offset: 0x001A04BB
		public int Rank { get; set; }

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06005130 RID: 20784 RVA: 0x001A22C4 File Offset: 0x001A04C4
		// (set) Token: 0x06005131 RID: 20785 RVA: 0x001A22CC File Offset: 0x001A04CC
		public int Score { get; set; }

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06005132 RID: 20786 RVA: 0x001A22D5 File Offset: 0x001A04D5
		// (set) Token: 0x06005133 RID: 20787 RVA: 0x001A22DD File Offset: 0x001A04DD
		public string UserName { get; set; }
	}
}
