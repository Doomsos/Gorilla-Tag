using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011DA RID: 4570
	[WeaverGenerated]
	internal struct ReaderWriter@Fusion_NetworkString : IElementReaderWriter<NetworkString<_32>>
	{
		// Token: 0x060072E3 RID: 29411 RVA: 0x0025A80E File Offset: 0x00258A0E
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe NetworkString<_32> Read(byte* data, int index)
		{
			return *(NetworkString<_32>*)(data + index * 132);
		}

		// Token: 0x060072E4 RID: 29412 RVA: 0x0025A81E File Offset: 0x00258A1E
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe ref NetworkString<_32> ReadRef(byte* data, int index)
		{
			return ref *(NetworkString<_32>*)(data + index * 132);
		}

		// Token: 0x060072E5 RID: 29413 RVA: 0x0025A829 File Offset: 0x00258A29
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkString<_32> val)
		{
			*(NetworkString<_32>*)(data + index * 132) = val;
		}

		// Token: 0x060072E6 RID: 29414 RVA: 0x0025A83A File Offset: 0x00258A3A
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 33;
		}

		// Token: 0x060072E7 RID: 29415 RVA: 0x0025A844 File Offset: 0x00258A44
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkString<_32> val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060072E8 RID: 29416 RVA: 0x0025A860 File Offset: 0x00258A60
		[MethodImpl(256)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_32>> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkString.Instance == null)
			{
				ReaderWriter@Fusion_NetworkString.Instance = default(ReaderWriter@Fusion_NetworkString);
			}
			return ReaderWriter@Fusion_NetworkString.Instance;
		}

		// Token: 0x0400833D RID: 33597
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_32>> Instance;
	}
}
