using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011F7 RID: 4599
	[WeaverGenerated]
	internal struct ReaderWriter@Fusion_NetworkString : IElementReaderWriter<NetworkString<_128>>
	{
		// Token: 0x06007319 RID: 29465 RVA: 0x0025AAE4 File Offset: 0x00258CE4
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe NetworkString<_128> Read(byte* data, int index)
		{
			return *(NetworkString<_128>*)(data + index * 516);
		}

		// Token: 0x0600731A RID: 29466 RVA: 0x0025AAF4 File Offset: 0x00258CF4
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe ref NetworkString<_128> ReadRef(byte* data, int index)
		{
			return ref *(NetworkString<_128>*)(data + index * 516);
		}

		// Token: 0x0600731B RID: 29467 RVA: 0x0025AAFF File Offset: 0x00258CFF
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, NetworkString<_128> val)
		{
			*(NetworkString<_128>*)(data + index * 516) = val;
		}

		// Token: 0x0600731C RID: 29468 RVA: 0x0025AB10 File Offset: 0x00258D10
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 129;
		}

		// Token: 0x0600731D RID: 29469 RVA: 0x0025AB18 File Offset: 0x00258D18
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementHashCode(NetworkString<_128> val)
		{
			return val.GetHashCode();
		}

		// Token: 0x0600731E RID: 29470 RVA: 0x0025AB34 File Offset: 0x00258D34
		[MethodImpl(256)]
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_128>> GetInstance()
		{
			if (ReaderWriter@Fusion_NetworkString.Instance == null)
			{
				ReaderWriter@Fusion_NetworkString.Instance = default(ReaderWriter@Fusion_NetworkString);
			}
			return ReaderWriter@Fusion_NetworkString.Instance;
		}

		// Token: 0x04008604 RID: 34308
		[WeaverGenerated]
		public static IElementReaderWriter<NetworkString<_128>> Instance;
	}
}
