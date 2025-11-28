using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001204 RID: 4612
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterSingle : UnityLinkedListSurrogate<float, ElementReaderWriterSingle>
	{
		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x06007331 RID: 29489 RVA: 0x0025AC2D File Offset: 0x00258E2D
		// (set) Token: 0x06007332 RID: 29490 RVA: 0x0025AC35 File Offset: 0x00258E35
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

		// Token: 0x06007333 RID: 29491 RVA: 0x0025AC3E File Offset: 0x00258E3E
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x0400866D RID: 34413
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
