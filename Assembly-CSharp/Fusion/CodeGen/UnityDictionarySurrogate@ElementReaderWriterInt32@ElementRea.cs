using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011FB RID: 4603
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32 : UnityDictionarySurrogate<int, ElementReaderWriterInt32, int, ElementReaderWriterInt32>
	{
		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06007322 RID: 29474 RVA: 0x0025AB99 File Offset: 0x00258D99
		// (set) Token: 0x06007323 RID: 29475 RVA: 0x0025ABA1 File Offset: 0x00258DA1
		[WeaverGenerated]
		public override SerializableDictionary<int, int> DataProperty
		{
			[WeaverGenerated]
			get
			{
				return this.Data;
			}
			[WeaverGenerated]
			set
			{
				this.Data = value;
			}
		}

		// Token: 0x06007324 RID: 29476 RVA: 0x0025ABAA File Offset: 0x00258DAA
		[WeaverGenerated]
		public UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32()
		{
		}

		// Token: 0x0400864E RID: 34382
		[WeaverGenerated]
		public SerializableDictionary<int, int> Data = SerializableDictionary.Create<int, int>();
	}
}
