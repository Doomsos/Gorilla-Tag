using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011DB RID: 4571
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@Fusion_NetworkString : UnityValueSurrogate<NetworkString<_32>, ReaderWriter@Fusion_NetworkString>
	{
		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x060072E9 RID: 29417 RVA: 0x0025A8AC File Offset: 0x00258AAC
		// (set) Token: 0x060072EA RID: 29418 RVA: 0x0025A8B4 File Offset: 0x00258AB4
		[WeaverGenerated]
		public override NetworkString<_32> DataProperty
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

		// Token: 0x060072EB RID: 29419 RVA: 0x0025A8BD File Offset: 0x00258ABD
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@Fusion_NetworkString()
		{
		}

		// Token: 0x0400833E RID: 33598
		[WeaverGenerated]
		public NetworkString<_32> Data;
	}
}
