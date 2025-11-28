using System;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E11 RID: 3601
	[Serializable]
	public struct CustomMapCosmeticItem
	{
		// Token: 0x040066F2 RID: 26354
		public GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot;

		// Token: 0x040066F3 RID: 26355
		public HeadModel_CosmeticStand.BustType bustType;

		// Token: 0x040066F4 RID: 26356
		public string playFabID;
	}
}
