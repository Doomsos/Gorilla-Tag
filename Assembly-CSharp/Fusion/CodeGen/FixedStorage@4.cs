using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011E3 RID: 4579
	[WeaverGenerated]
	[NetworkStructWeaved(4)]
	[Serializable]
	[StructLayout(2)]
	internal struct FixedStorage@4 : INetworkStruct
	{
		// Token: 0x04008351 RID: 33617
		[FixedBuffer(typeof(int), 4)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@4.<Data>e__FixedBuffer Data;

		// Token: 0x04008352 RID: 33618
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x04008353 RID: 33619
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x04008354 RID: 33620
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(12)]
		private int _3;

		// Token: 0x020011E4 RID: 4580
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(0, Size = 16)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x04008355 RID: 33621
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
