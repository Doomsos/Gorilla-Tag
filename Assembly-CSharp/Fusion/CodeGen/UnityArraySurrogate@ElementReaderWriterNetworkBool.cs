using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011E2 RID: 4578
	[WeaverGenerated]
	[Serializable]
	internal class UnityArraySurrogate@ElementReaderWriterNetworkBool : UnityArraySurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x060072F2 RID: 29426 RVA: 0x0025A8D7 File Offset: 0x00258AD7
		// (set) Token: 0x060072F3 RID: 29427 RVA: 0x0025A8DF File Offset: 0x00258ADF
		[WeaverGenerated]
		public override NetworkBool[] DataProperty
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

		// Token: 0x060072F4 RID: 29428 RVA: 0x0025A8E8 File Offset: 0x00258AE8
		[WeaverGenerated]
		public UnityArraySurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x04008350 RID: 33616
		[WeaverGenerated]
		public NetworkBool[] Data = Array.Empty<NetworkBool>();
	}
}
