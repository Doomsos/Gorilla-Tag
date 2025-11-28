using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000007 RID: 7
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(27524, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000022CA File Offset: 0x000004CA
		public NullableAttribute(byte A_1)
		{
			this.NullableFlags = new byte[]
			{
				A_1
			};
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022E2 File Offset: 0x000004E2
		public NullableAttribute(byte[] A_1)
		{
			this.NullableFlags = A_1;
		}

		// Token: 0x04000005 RID: 5
		public readonly byte[] NullableFlags;
	}
}
