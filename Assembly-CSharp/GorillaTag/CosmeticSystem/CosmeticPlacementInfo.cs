using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200105B RID: 4187
	[Serializable]
	public struct CosmeticPlacementInfo
	{
		// Token: 0x04007834 RID: 30772
		[Tooltip("The bone to attach the cosmetic to.")]
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x04007835 RID: 30773
		public XformOffset offset;
	}
}
