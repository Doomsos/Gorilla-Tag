using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020011F1 RID: 4593
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ElementReaderWriterVector3 : UnityLinkedListSurrogate<Vector3, ElementReaderWriterVector3>
	{
		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06007313 RID: 29459 RVA: 0x0025AA9C File Offset: 0x00258C9C
		// (set) Token: 0x06007314 RID: 29460 RVA: 0x0025AAA4 File Offset: 0x00258CA4
		[WeaverGenerated]
		public override Vector3[] DataProperty
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

		// Token: 0x06007315 RID: 29461 RVA: 0x0025AAAD File Offset: 0x00258CAD
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ElementReaderWriterVector3()
		{
		}

		// Token: 0x040084C8 RID: 33992
		[WeaverGenerated]
		public Vector3[] Data = Array.Empty<Vector3>();
	}
}
