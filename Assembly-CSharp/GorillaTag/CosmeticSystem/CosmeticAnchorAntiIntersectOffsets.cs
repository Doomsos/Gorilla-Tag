using System;
using UnityEngine.Serialization;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001053 RID: 4179
	[Serializable]
	public struct CosmeticAnchorAntiIntersectOffsets
	{
		// Token: 0x040077F4 RID: 30708
		public CosmeticAnchorAntiClipEntry nameTag;

		// Token: 0x040077F5 RID: 30709
		public CosmeticAnchorAntiClipEntry leftArm;

		// Token: 0x040077F6 RID: 30710
		public CosmeticAnchorAntiClipEntry rightArm;

		// Token: 0x040077F7 RID: 30711
		public CosmeticAnchorAntiClipEntry chest;

		// Token: 0x040077F8 RID: 30712
		public CosmeticAnchorAntiClipEntry huntComputer;

		// Token: 0x040077F9 RID: 30713
		public CosmeticAnchorAntiClipEntry badge;

		// Token: 0x040077FA RID: 30714
		public CosmeticAnchorAntiClipEntry builderWatch;

		// Token: 0x040077FB RID: 30715
		public CosmeticAnchorAntiClipEntry friendshipBraceletLeft;

		// Token: 0x040077FC RID: 30716
		[FormerlySerializedAs("friendshipBradceletRight")]
		public CosmeticAnchorAntiClipEntry friendshipBraceletRight;

		// Token: 0x040077FD RID: 30717
		public static readonly CosmeticAnchorAntiIntersectOffsets Identity = new CosmeticAnchorAntiIntersectOffsets
		{
			nameTag = CosmeticAnchorAntiClipEntry.Identity,
			leftArm = CosmeticAnchorAntiClipEntry.Identity,
			rightArm = CosmeticAnchorAntiClipEntry.Identity,
			chest = CosmeticAnchorAntiClipEntry.Identity,
			huntComputer = CosmeticAnchorAntiClipEntry.Identity,
			badge = CosmeticAnchorAntiClipEntry.Identity,
			builderWatch = CosmeticAnchorAntiClipEntry.Identity
		};
	}
}
