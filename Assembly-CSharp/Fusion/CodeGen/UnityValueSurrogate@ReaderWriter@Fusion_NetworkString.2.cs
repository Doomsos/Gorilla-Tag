using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011F8 RID: 4600
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_128>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x0600731F RID: 29471 RVA: 0x0025AB60 File Offset: 0x00258D60
		// (set) Token: 0x06007320 RID: 29472 RVA: 0x0025AB68 File Offset: 0x00258D68
		[WeaverGenerated]
		public override NetworkString<_128> DataProperty
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

		// Token: 0x06007321 RID: 29473 RVA: 0x0025AB71 File Offset: 0x00258D71
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x04008605 RID: 34309
		[WeaverGenerated]
		public NetworkString<_128> Data;
	}
}
