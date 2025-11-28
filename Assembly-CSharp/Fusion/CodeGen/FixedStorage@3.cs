using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011DC RID: 4572
	[WeaverGenerated]
	[NetworkStructWeaved(3)]
	[Serializable]
	[StructLayout(2)]
	internal struct FixedStorage@3 : INetworkStruct
	{
		// Token: 0x0400833F RID: 33599
		[FixedBuffer(typeof(int), 3)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@3.<Data>e__FixedBuffer Data;

		// Token: 0x04008340 RID: 33600
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(4)]
		private int _1;

		// Token: 0x04008341 RID: 33601
		[WeaverGenerated]
		[NonSerialized]
		[FieldOffset(8)]
		private int _2;

		// Token: 0x020011DD RID: 4573
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(0, Size = 12)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x04008342 RID: 33602
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
