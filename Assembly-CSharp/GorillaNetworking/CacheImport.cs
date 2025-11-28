using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000F2A RID: 3882
	public class CacheImport
	{
		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06006146 RID: 24902 RVA: 0x001F5727 File Offset: 0x001F3927
		// (set) Token: 0x06006147 RID: 24903 RVA: 0x001F572F File Offset: 0x001F392F
		public string DeploymentId { get; set; }

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06006148 RID: 24904 RVA: 0x001F5738 File Offset: 0x001F3938
		// (set) Token: 0x06006149 RID: 24905 RVA: 0x001F5740 File Offset: 0x001F3940
		public Dictionary<string, Dictionary<string, string>> TitleData { get; set; }
	}
}
