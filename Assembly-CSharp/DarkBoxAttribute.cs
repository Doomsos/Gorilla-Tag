using System;
using System.Diagnostics;

// Token: 0x02000233 RID: 563
[Conditional("UNITY_EDITOR")]
public class DarkBoxAttribute : Attribute
{
	// Token: 0x06000EF5 RID: 3829 RVA: 0x000022C2 File Offset: 0x000004C2
	public DarkBoxAttribute()
	{
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x0004F7B1 File Offset: 0x0004D9B1
	public DarkBoxAttribute(bool withBorders)
	{
		this.withBorders = withBorders;
	}

	// Token: 0x04001243 RID: 4675
	public readonly bool withBorders;
}
