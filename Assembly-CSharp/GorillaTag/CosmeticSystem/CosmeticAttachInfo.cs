using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001056 RID: 4182
	[Serializable]
	public struct CosmeticAttachInfo
	{
		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06006944 RID: 26948 RVA: 0x00224080 File Offset: 0x00222280
		public static CosmeticAttachInfo Identity
		{
			get
			{
				return new CosmeticAttachInfo
				{
					selectSide = ECosmeticSelectSide.Both,
					parentBone = GTHardCodedBones.EBone.None,
					offset = XformOffset.Identity
				};
			}
		}

		// Token: 0x06006945 RID: 26949 RVA: 0x002240BC File Offset: 0x002222BC
		public CosmeticAttachInfo(ECosmeticSelectSide selectSide, GTHardCodedBones.EBone parentBone, XformOffset offset)
		{
			this.selectSide = selectSide;
			this.parentBone = parentBone;
			this.offset = offset;
		}

		// Token: 0x0400780B RID: 30731
		[Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
		public StringEnum<ECosmeticSelectSide> selectSide;

		// Token: 0x0400780C RID: 30732
		public GTHardCodedBones.SturdyEBone parentBone;

		// Token: 0x0400780D RID: 30733
		public XformOffset offset;
	}
}
