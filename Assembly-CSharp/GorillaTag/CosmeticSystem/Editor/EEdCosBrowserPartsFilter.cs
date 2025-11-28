using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x02001068 RID: 4200
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x040078C2 RID: 30914
		None = 0,
		// Token: 0x040078C3 RID: 30915
		NoParts = 1,
		// Token: 0x040078C4 RID: 30916
		Holdable = 2,
		// Token: 0x040078C5 RID: 30917
		Functional = 4,
		// Token: 0x040078C6 RID: 30918
		Wardrobe = 8,
		// Token: 0x040078C7 RID: 30919
		Store = 16,
		// Token: 0x040078C8 RID: 30920
		FirstPerson = 32,
		// Token: 0x040078C9 RID: 30921
		LocalRig = 64,
		// Token: 0x040078CA RID: 30922
		All = 127
	}
}
