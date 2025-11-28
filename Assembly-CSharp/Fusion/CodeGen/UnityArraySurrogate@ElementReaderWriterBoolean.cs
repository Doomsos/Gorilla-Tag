using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x02001200 RID: 4608
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterBoolean : UnityArraySurrogate<bool, ElementReaderWriterBoolean>
	{
		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x0600732B RID: 29483 RVA: 0x0025ABE5 File Offset: 0x00258DE5
		// (set) Token: 0x0600732C RID: 29484 RVA: 0x0025ABED File Offset: 0x00258DED
		[WeaverGenerated]
		public override bool[] DataProperty
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

		// Token: 0x0600732D RID: 29485 RVA: 0x0025ABF6 File Offset: 0x00258DF6
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterBoolean()
		{
		}

		// Token: 0x04008658 RID: 34392
		[WeaverGenerated]
		public bool[] Data = Array.Empty<bool>();
	}
}
