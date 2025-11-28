using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011D3 RID: 4563
	[WeaverGenerated]
	[NetworkStructWeaved(1)]
	[Serializable]
	[StructLayout(2)]
	internal struct FixedStorage@1 : INetworkStruct
	{
		// Token: 0x04008316 RID: 33558
		[FixedBuffer(typeof(int), 1)]
		[WeaverGenerated]
		[FieldOffset(0)]
		public FixedStorage@1.<Data>e__FixedBuffer Data;

		// Token: 0x020011D4 RID: 4564
		[CompilerGenerated]
		[UnsafeValueType]
		[WeaverGenerated]
		[StructLayout(0, Size = 4)]
		public struct <Data>e__FixedBuffer
		{
			// Token: 0x04008317 RID: 33559
			[WeaverGenerated]
			public int FixedElementField;
		}
	}
}
