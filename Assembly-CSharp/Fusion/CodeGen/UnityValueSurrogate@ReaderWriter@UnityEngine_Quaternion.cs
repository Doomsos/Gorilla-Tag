using System;
using Fusion.Internal;
using UnityEngine;

namespace Fusion.CodeGen
{
	// Token: 0x020011E6 RID: 4582
	[WeaverGenerated]
	[Serializable]
	internal class UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion : UnityValueSurrogate<Quaternion, ReaderWriter@UnityEngine_Quaternion>
	{
		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x060072FB RID: 29435 RVA: 0x0025A970 File Offset: 0x00258B70
		// (set) Token: 0x060072FC RID: 29436 RVA: 0x0025A978 File Offset: 0x00258B78
		[WeaverGenerated]
		public override Quaternion DataProperty
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

		// Token: 0x060072FD RID: 29437 RVA: 0x0025A981 File Offset: 0x00258B81
		[WeaverGenerated]
		public UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion()
		{
		}

		// Token: 0x04008357 RID: 33623
		[WeaverGenerated]
		public Quaternion Data;
	}
}
