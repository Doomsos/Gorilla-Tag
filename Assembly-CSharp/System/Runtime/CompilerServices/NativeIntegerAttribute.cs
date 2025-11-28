using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000009 RID: 9
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(27524, AllowMultiple = false, Inherited = false)]
	internal sealed class NativeIntegerAttribute : Attribute
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002300 File Offset: 0x00000500
		public NativeIntegerAttribute()
		{
			this.TransformFlags = new bool[]
			{
				true
			};
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002318 File Offset: 0x00000518
		public NativeIntegerAttribute(bool[] A_1)
		{
			this.TransformFlags = A_1;
		}

		// Token: 0x04000007 RID: 7
		public readonly bool[] TransformFlags;
	}
}
