using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000F23 RID: 3875
	public static class ExtensionMethods
	{
		// Token: 0x0600611F RID: 24863 RVA: 0x001F4C74 File Offset: 0x001F2E74
		public static void SafeInvoke<T>(this Action<T> action, T data)
		{
			try
			{
				if (action != null)
				{
					action.Invoke(data);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SafeInvoke] Failure invoking action: {0}", ex));
			}
		}

		// Token: 0x06006120 RID: 24864 RVA: 0x001F4CB0 File Offset: 0x001F2EB0
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] = value;
				return;
			}
			dict.Add(key, value);
		}
	}
}
