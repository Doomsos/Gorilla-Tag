using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011EA RID: 4586
	[WeaverGenerated]
	[Serializable]
	internal class UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString : UnityDictionarySurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString, NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x06007301 RID: 29441 RVA: 0x0025A9A2 File Offset: 0x00258BA2
		// (set) Token: 0x06007302 RID: 29442 RVA: 0x0025A9AA File Offset: 0x00258BAA
		[WeaverGenerated]
		public override SerializableDictionary<NetworkString<_32>, NetworkString<_32>> DataProperty
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

		// Token: 0x06007303 RID: 29443 RVA: 0x0025A9B3 File Offset: 0x00258BB3
		[WeaverGenerated]
		public UnityDictionarySurrogate@ReaderWriter@Fusion_NetworkString`1<Fusion__32>@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04008429 RID: 33833
		[WeaverGenerated]
		public SerializableDictionary<NetworkString<_32>, NetworkString<_32>> Data = SerializableDictionary.Create<NetworkString<_32>, NetworkString<_32>>();
	}
}
