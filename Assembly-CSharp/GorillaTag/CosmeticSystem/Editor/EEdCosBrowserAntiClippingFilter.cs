using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02001066 RID: 4198
	[Flags]
	public enum EEdCosBrowserAntiClippingFilter
	{
		// Token: 0x040078A7 RID: 30887
		None = 0,
		// Token: 0x040078A8 RID: 30888
		NameTag = 1,
		// Token: 0x040078A9 RID: 30889
		LeftArm = 2,
		// Token: 0x040078AA RID: 30890
		RightArm = 4,
		// Token: 0x040078AB RID: 30891
		Chest = 8,
		// Token: 0x040078AC RID: 30892
		HuntComputer = 16,
		// Token: 0x040078AD RID: 30893
		Badge = 32,
		// Token: 0x040078AE RID: 30894
		BuilderWatch = 64,
		// Token: 0x040078AF RID: 30895
		FriendshipBraceletLeft = 128,
		// Token: 0x040078B0 RID: 30896
		FriendshipBraceletRight = 256,
		// Token: 0x040078B1 RID: 30897
		All = 511
	}
}
