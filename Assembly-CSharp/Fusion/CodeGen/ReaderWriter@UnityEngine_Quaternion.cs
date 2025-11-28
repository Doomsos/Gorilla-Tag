using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020011E5 RID: 4581
	[WeaverGenerated]
	internal struct ReaderWriter@UnityEngine_Quaternion : IElementReaderWriter<Quaternion>
	{
		// Token: 0x060072F5 RID: 29429 RVA: 0x0025A8FB File Offset: 0x00258AFB
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe Quaternion Read(byte* data, int index)
		{
			return *(Quaternion*)(data + index * 16);
		}

		// Token: 0x060072F6 RID: 29430 RVA: 0x0025A90B File Offset: 0x00258B0B
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe ref Quaternion ReadRef(byte* data, int index)
		{
			return ref *(Quaternion*)(data + index * 16);
		}

		// Token: 0x060072F7 RID: 29431 RVA: 0x0025A916 File Offset: 0x00258B16
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, Quaternion val)
		{
			*(Quaternion*)(data + index * 16) = val;
		}

		// Token: 0x060072F8 RID: 29432 RVA: 0x00186603 File Offset: 0x00184803
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 4;
		}

		// Token: 0x060072F9 RID: 29433 RVA: 0x0025A928 File Offset: 0x00258B28
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementHashCode(Quaternion val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060072FA RID: 29434 RVA: 0x0025A944 File Offset: 0x00258B44
		[MethodImpl(256)]
		[WeaverGenerated]
		public static IElementReaderWriter<Quaternion> GetInstance()
		{
			if (ReaderWriter@UnityEngine_Quaternion.Instance == null)
			{
				ReaderWriter@UnityEngine_Quaternion.Instance = default(ReaderWriter@UnityEngine_Quaternion);
			}
			return ReaderWriter@UnityEngine_Quaternion.Instance;
		}

		// Token: 0x04008356 RID: 33622
		[WeaverGenerated]
		public static IElementReaderWriter<Quaternion> Instance;
	}
}
