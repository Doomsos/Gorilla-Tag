using System;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

// Token: 0x020009C7 RID: 2503
public static class ContextLog
{
	// Token: 0x06004004 RID: 16388 RVA: 0x0015811F File Offset: 0x0015631F
	public static void Log<T0, T1>(this T0 ctx, T1 arg1)
	{
		Debug.Log(ZString.Concat<string, T1>(ContextLog.GetPrefix<T0>(ref ctx), arg1));
	}

	// Token: 0x06004005 RID: 16389 RVA: 0x00158134 File Offset: 0x00156334
	public static void LogCall<T0, T1>(this T0 ctx, T1 arg1, [CallerMemberName] string call = null)
	{
		string prefix = ContextLog.GetPrefix<T0>(ref ctx);
		string text = ZString.Concat<string, string, string>("{.", call, "()} ");
		Debug.Log(ZString.Concat<string, string, T1>(prefix, text, arg1));
	}

	// Token: 0x06004006 RID: 16390 RVA: 0x00158168 File Offset: 0x00156368
	private static string GetPrefix<T>(ref T ctx)
	{
		if (ctx == null)
		{
			return string.Empty;
		}
		Type type = ctx as Type;
		string text;
		if (type != null)
		{
			text = type.Name;
		}
		else
		{
			string text2 = ctx as string;
			if (text2 != null)
			{
				text = text2;
			}
			else
			{
				text = ctx.GetType().Name;
			}
		}
		return ZString.Concat<string, string, string>("[", text, "] ");
	}
}
