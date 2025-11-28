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
		// (get) Token: 0x06007325 RID: 29477 RVA: 0x0025ABBD File Offset: 0x00258DBD
		// (set) Token: 0x06007326 RID: 29478 RVA: 0x0025ABC5 File Offset: 0x00258DC5
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

		// Token: 0x06007327 RID: 29479 RVA: 0x0025ABCE File Offset: 0x00258DCE
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterByte()
		{
		}

		// Token: 0x04008656 RID: 34390
		[WeaverGenerated]
		public byte[] Data = Array.Empty<byte>();
	}
}
