using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000EAA RID: 3754
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(4)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
