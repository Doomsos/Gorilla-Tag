using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x020011D5 RID: 4565
	[WeaverGenerated]
	internal struct ReaderWriter@BarrelCannon__BarrelCannonState : IElementReaderWriter<BarrelCannon.BarrelCannonState>
	{
		// Token: 0x060072D7 RID: 29399 RVA: 0x0025A768 File Offset: 0x00258968
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe BarrelCannon.BarrelCannonState Read(byte* data, int index)
		{
			return *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x060072D8 RID: 29400 RVA: 0x0025A778 File Offset: 0x00258978
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe ref BarrelCannon.BarrelCannonState ReadRef(byte* data, int index)
		{
			return ref *(BarrelCannon.BarrelCannonState*)(data + index * 4);
		}

		// Token: 0x060072D9 RID: 29401 RVA: 0x0025A783 File Offset: 0x00258983
		[MethodImpl(256)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, BarrelCannon.BarrelCannonState val)
		{
			*(BarrelCannon.BarrelCannonState*)(data + index * 4) = val;
		}

		// Token: 0x060072DA RID: 29402 RVA: 0x00027DED File Offset: 0x00025FED
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060072DB RID: 29403 RVA: 0x0025A794 File Offset: 0x00258994
		[MethodImpl(256)]
		[WeaverGenerated]
		public int GetElementHashCode(BarrelCannon.BarrelCannonState val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060072DC RID: 29404 RVA: 0x0025A7B0 File Offset: 0x002589B0
		[MethodImpl(256)]
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> GetInstance()
		{
			if (ReaderWriter@BarrelCannon__BarrelCannonState.Instance == null)
			{
				ReaderWriter@BarrelCannon__BarrelCannonState.Instance = default(ReaderWriter@BarrelCannon__BarrelCannonState);
			}
			return ReaderWriter@BarrelCannon__BarrelCannonState.Instance;
		}

		// Token: 0x04008318 RID: 33560
		[WeaverGenerated]
		public static IElementReaderWriter<BarrelCannon.BarrelCannonState> Instance;
	}
}
