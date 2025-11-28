using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011E7 RID: 4583
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterInt32 : UnityValueSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x060072FE RID: 29438 RVA: 0x0025A989 File Offset: 0x00258B89
		// (set) Token: 0x060072FF RID: 29439 RVA: 0x0025A991 File Offset: 0x00258B91
		[WeaverGenerated]
		public override int DataProperty
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

		// Token: 0x06007300 RID: 29440 RVA: 0x0025A99A File Offset: 0x00258B9A
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x04008358 RID: 33624
		[WeaverGenerated]
		public int Data;
	}
}
