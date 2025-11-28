using System;
using System.Collections.Generic;

// Token: 0x02000C45 RID: 3141
public static class DictArrayUtils
{
	// Token: 0x06004D08 RID: 19720 RVA: 0x0018FFAE File Offset: 0x0018E1AE
	public static void TryGetOrAddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, out List<TValue> list, int capacity)
	{
		if (dict.TryGetValue(key, ref list) && list != null)
		{
			return;
		}
		list = new List<TValue>(capacity);
		dict.Add(key, list);
	}

	// Token: 0x06004D09 RID: 19721 RVA: 0x0018FFD0 File Offset: 0x0018E1D0
	public static void TryGetOrAddArray<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, out TValue[] array, int size)
	{
		if (dict.TryGetValue(key, ref array) && array != null)
		{
			return;
		}
		array = new TValue[size];
		dict.Add(key, array);
	}
}
