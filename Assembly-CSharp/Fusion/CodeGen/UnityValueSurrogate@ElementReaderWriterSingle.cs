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
		// (get) Token: 0x060072EF RID: 29423 RVA: 0x0025A8DE File Offset: 0x00258ADE
		// (set) Token: 0x060072F0 RID: 29424 RVA: 0x0025A8E6 File Offset: 0x00258AE6
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

		// Token: 0x060072F1 RID: 29425 RVA: 0x0025A8EF File Offset: 0x00258AEF
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04008344 RID: 33604
		[WeaverGenerated]
		public float Data;
	}
}
