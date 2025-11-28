using System;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001055 RID: 4181
	[Serializable]
	public struct CosmeticAnchorAntiClipEntry
	{
		// Token: 0x04007808 RID: 30728
		public bool enabled;

		// Token: 0x04007809 RID: 30729
		public XformOffset offset;

		// Token: 0x0400780A RID: 30730
		public static readonly CosmeticAnchorAntiClipEntry Identity = new CosmeticAnchorAntiClipEntry
		{
			offset = XformOffset.Identity
		};
	}
}
