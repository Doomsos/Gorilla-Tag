using System;
using System.Diagnostics;

namespace BuildSafe
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(4)]
	public class EditorOnlyScriptAttribute : Attribute
	{
	}
}
