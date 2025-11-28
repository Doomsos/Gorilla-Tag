using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011EB RID: 4587
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterInt64 : UnityArraySurrogate<long, ElementReaderWriterInt64>
	{
		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x06007304 RID: 29444 RVA: 0x0025A9C6 File Offset: 0x00258BC6
		// (set) Token: 0x06007305 RID: 29445 RVA: 0x0025A9CE File Offset: 0x00258BCE
		[WeaverGenerated]
		public override long[] DataProperty
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

		// Token: 0x06007306 RID: 29446 RVA: 0x0025A9D7 File Offset: 0x00258BD7
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterInt64()
		{
		}

		// Token: 0x0400842A RID: 33834
		[WeaverGenerated]
		public long[] Data = Array.Empty<long>();
	}
}
