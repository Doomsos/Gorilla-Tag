using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011FC RID: 4604
	[WeaverGenerated]
	[NetworkStructWeaved(6)]
	[Serializable]
	[StructLayout(2)]
	internal struct FixedStorage@6 : INetworkStruct
	{
		// Token: 0x0400864F RID: 34383
		[FixedBuffer(typeof(int), 6)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@6.<Data>e__FixedBuffer Data;

		// Token: 0x04008650 RID: 34384
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x04008651 RID: 34385
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x04008652 RID: 34386
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x04008653 RID: 34387
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(16)]
		private int _4;

		// Token: 0x04008654 RID: 34388
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(20)]
		private int _5;

		// Token: 0x020011FD RID: 4605
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(0, Size = 24)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x04008655 RID: 34389
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
