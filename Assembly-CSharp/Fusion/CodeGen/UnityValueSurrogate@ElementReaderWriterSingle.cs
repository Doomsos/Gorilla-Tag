using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011DF RID: 4575
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterSingle : UnityValueSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x060072EF RID: 29423 RVA: 0x0025A8BE File Offset: 0x00258ABE
		// (set) Token: 0x060072F0 RID: 29424 RVA: 0x0025A8C6 File Offset: 0x00258AC6
		[WeaverGenerated]
		public override float DataProperty
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

		// Token: 0x060072F1 RID: 29425 RVA: 0x0025A8CF File Offset: 0x00258ACF
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04008344 RID: 33604
		[WeaverGenerated]
		public float Data;
	}
}
