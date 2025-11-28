using System;
using System.Collections.Generic;

// Token: 0x02000C44 RID: 3140
public static class DictValueTypeUtils
{
	// Token: 0x06004D07 RID: 19719 RVA: 0x0018FF6D File Offset: 0x0018E16D
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : struct
	{
		if (dict.TryGetValue(key, ref value))
		{
			return;
		}
		value = default(TValue);
		dict.Add(key, value);
	}
}
