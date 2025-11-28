using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C92 RID: 3218
public static class UnityObjectUtils
{
	// Token: 0x06004EAA RID: 20138 RVA: 0x001973E0 File Offset: 0x001955E0
	[MethodImpl(256)]
	public static T AsNull<T>(this T obj) where T : Object
	{
		if (obj == null)
		{
			return default(T);
		}
		if (!(obj == null))
		{
			return obj;
		}
		return default(T);
	}

	// Token: 0x06004EAB RID: 20139 RVA: 0x0005BEDF File Offset: 0x0005A0DF
	[MethodImpl(256)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
