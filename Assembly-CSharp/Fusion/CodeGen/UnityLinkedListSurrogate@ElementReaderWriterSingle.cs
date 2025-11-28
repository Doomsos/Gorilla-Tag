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
		// (get) Token: 0x06007331 RID: 29489 RVA: 0x0025AC4D File Offset: 0x00258E4D
		// (set) Token: 0x06007332 RID: 29490 RVA: 0x0025AC55 File Offset: 0x00258E55
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

		// Token: 0x06007333 RID: 29491 RVA: 0x0025AC5E File Offset: 0x00258E5E
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterSingle()
		{
		}

		// Token: 0x0400866D RID: 34413
		[WeaverGenerated]
		public float[] Data = Array.Empty<float>();
	}
}
