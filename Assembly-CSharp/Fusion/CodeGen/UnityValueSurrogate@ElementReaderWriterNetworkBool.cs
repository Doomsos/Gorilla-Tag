using System;
using Fusion.Internal;

namespace Fusion.CodeGen
{
	// Token: 0x020011D7 RID: 4567
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterNetworkBool : UnityValueSurrogate<NetworkBool, ElementReaderWriterNetworkBool>
	{
		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x060072E0 RID: 29408 RVA: 0x0025A7F5 File Offset: 0x002589F5
		// (set) Token: 0x060072E1 RID: 29409 RVA: 0x0025A7FD File Offset: 0x002589FD
		[WeaverGenerated]
		public override NetworkBool DataProperty
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

		// Token: 0x060072E2 RID: 29410 RVA: 0x0025A806 File Offset: 0x00258A06
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterNetworkBool()
		{
		}

		// Token: 0x0400831A RID: 33562
		[WeaverGenerated]
		public NetworkBool Data;
	}
}
