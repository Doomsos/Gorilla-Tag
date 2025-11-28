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
		// (get) Token: 0x060072EC RID: 29420 RVA: 0x0025A8C5 File Offset: 0x00258AC5
		// (set) Token: 0x060072ED RID: 29421 RVA: 0x0025A8CD File Offset: 0x00258ACD
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

		// Token: 0x060072EE RID: 29422 RVA: 0x0025A8D6 File Offset: 0x00258AD6
		[WeaverGenerated]
		public UnityValueSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x04008343 RID: 33603
		[WeaverGenerated]
		public Vector3 Data;
	}
}
