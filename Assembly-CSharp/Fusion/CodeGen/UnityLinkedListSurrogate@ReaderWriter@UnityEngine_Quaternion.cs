using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020011F4 RID: 4596
	[WeaverGenerated]
	[Serializable]
	internal class UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityLinkedListSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06007316 RID: 29462 RVA: 0x0025AAC0 File Offset: 0x00258CC0
		// (set) Token: 0x06007317 RID: 29463 RVA: 0x0025AAC8 File Offset: 0x00258CC8
		[WeaverGenerated]
		public override Quaternion[] DataProperty
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

		// Token: 0x06007318 RID: 29464 RVA: 0x0025AAD1 File Offset: 0x00258CD1
		[WeaverGenerated]
		public UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x04008581 RID: 34177
		[WeaverGenerated]
		public Quaternion[] Data = Array.Empty<Quaternion>();
	}
}
