using System;

// Token: 0x02000BEA RID: 3050
internal interface IUserCosmeticsCallback
{
	// Token: 0x06004B40 RID: 19264
	bool OnGetUserCosmetics(string cosmetics);

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x06004B41 RID: 19265
	// (set) Token: 0x06004B42 RID: 19266
	bool PendingUpdate { get; set; }
}
