using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011FF RID: 4607
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterInt32 : UnityLinkedListSurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06007328 RID: 29480 RVA: 0x0025ABE1 File Offset: 0x00258DE1
		// (set) Token: 0x06007329 RID: 29481 RVA: 0x0025ABE9 File Offset: 0x00258DE9
		[WeaverGenerated]
		public override int[] DataProperty
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

		// Token: 0x0600732A RID: 29482 RVA: 0x0025ABF2 File Offset: 0x00258DF2
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x04008657 RID: 34391
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
