using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020011DE RID: 4574
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ElementReaderWriterVector3 : UnityValueSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x060072EC RID: 29420 RVA: 0x0025A8A5 File Offset: 0x00258AA5
		// (set) Token: 0x060072ED RID: 29421 RVA: 0x0025A8AD File Offset: 0x00258AAD
		[WeaverGenerated]
		public override Vector3 DataProperty
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

		// Token: 0x060072EE RID: 29422 RVA: 0x0025A8B6 File Offset: 0x00258AB6
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x04008343 RID: 33603
		[WeaverGenerated]
		public Vector3 Data;
	}
}
