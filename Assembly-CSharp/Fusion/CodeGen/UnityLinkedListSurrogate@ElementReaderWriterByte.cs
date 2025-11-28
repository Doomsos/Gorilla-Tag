using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011FE RID: 4606
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterByte : UnityLinkedListSurrogate<byte, ElementReaderWriterByte>
	{
		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06007325 RID: 29477 RVA: 0x0025AB9D File Offset: 0x00258D9D
		// (set) Token: 0x06007326 RID: 29478 RVA: 0x0025ABA5 File Offset: 0x00258DA5
		[WeaverGenerated]
		public override byte[] DataProperty
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

		// Token: 0x06007327 RID: 29479 RVA: 0x0025ABAE File Offset: 0x00258DAE
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterByte()
		{
		}

		// Token: 0x04008656 RID: 34390
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}
