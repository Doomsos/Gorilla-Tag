using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011EC RID: 4588
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt32 : UnityArraySurrogate<int, ElementReaderWriterInt32>
	{
		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x06007307 RID: 29447 RVA: 0x0025AA0A File Offset: 0x00258C0A
		// (set) Token: 0x06007308 RID: 29448 RVA: 0x0025AA12 File Offset: 0x00258C12
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

		// Token: 0x06007309 RID: 29449 RVA: 0x0025AA1B File Offset: 0x00258C1B
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt32()
		{
		}

		// Token: 0x0400842B RID: 33835
		[WeaverGenerated]
		public int[] Data = Array.Empty<int>();
	}
}
