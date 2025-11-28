using System;
using System.Collections.Generic;

// Token: 0x02000C43 RID: 3139
public static class DictRefTypeUtils
{
	// Token: 0x06004D06 RID: 19718 RVA: 0x0018FF5B File Offset: 0x0018E15B
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : class, new()
	{
		if (dict.TryGetValue(key, ref value) && value != null)
		{
			return;
		}
		value = Activator.CreateInstance<TValue>();
		dict.Add(key, value);
	}
}
