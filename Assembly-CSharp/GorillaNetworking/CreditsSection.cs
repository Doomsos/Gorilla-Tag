using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000EDE RID: 3806
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06005F23 RID: 24355 RVA: 0x001E94B7 File Offset: 0x001E76B7
		// (set) Token: 0x06005F24 RID: 24356 RVA: 0x001E94BF File Offset: 0x001E76BF
		public string Title { get; set; }

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06005F25 RID: 24357 RVA: 0x001E94C8 File Offset: 0x001E76C8
		// (set) Token: 0x06005F26 RID: 24358 RVA: 0x001E94D0 File Offset: 0x001E76D0
		public List<string> Entries { get; set; }
	}
}
