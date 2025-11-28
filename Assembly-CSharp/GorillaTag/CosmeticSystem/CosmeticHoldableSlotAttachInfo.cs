using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001057 RID: 4183
	[Serializable]
	public struct CosmeticHoldableSlotAttachInfo
	{
		// Token: 0x0400780E RID: 30734
		[Tooltip("The anchor that this holdable cosmetic can attach to.")]
		public GTSturdyEnum<GTHardCodedBones.EHandAndStowSlots> stowSlot;

		// Token: 0x0400780F RID: 30735
		public XformOffset offset;
	}
}
