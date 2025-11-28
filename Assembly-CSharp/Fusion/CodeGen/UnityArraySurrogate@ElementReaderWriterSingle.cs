using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001201 RID: 4609
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterSingle : UnityArraySurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x0600732E RID: 29486 RVA: 0x0025AC29 File Offset: 0x00258E29
		// (set) Token: 0x0600732F RID: 29487 RVA: 0x0025AC31 File Offset: 0x00258E31
		[WeaverGenerated]
		public override float[] DataProperty
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

		// Token: 0x06007330 RID: 29488 RVA: 0x0025AC3A File Offset: 0x00258E3A
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x04008659 RID: 34393
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
