using System;
using System.Diagnostics;

// Token: 0x02000235 RID: 565
[Conditional("UNITY_EDITOR")]
[AttributeUsage(32767)]
public class InlineAttribute : Attribute
{
	// Token: 0x06000EF8 RID: 3832 RVA: 0x0004F7C0 File Offset: 0x0004D9C0
	public InlineAttribute(bool keepLabel = false, bool asGroup = false)
	{
		this.keepLabel = keepLabel;
		this.asGroup = asGroup;
	}

	// Token: 0x04001244 RID: 4676
	public readonly bool keepLabel;

	// Token: 0x04001245 RID: 4677
	public readonly bool asGroup;
}
