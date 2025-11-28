using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000EDE RID: 3806
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06005F23 RID: 24355 RVA: 0x001E9497 File Offset: 0x001E7697
		// (set) Token: 0x06005F24 RID: 24356 RVA: 0x001E949F File Offset: 0x001E769F
		public string Title { get; set; }

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06005F25 RID: 24357 RVA: 0x001E94A8 File Offset: 0x001E76A8
		// (set) Token: 0x06005F26 RID: 24358 RVA: 0x001E94B0 File Offset: 0x001E76B0
		public List<string> Entries { get; set; }
	}
}
