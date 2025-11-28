using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02000FB4 RID: 4020
	public static class EnumerableExtensions
	{
		// Token: 0x060064EC RID: 25836 RVA: 0x0020F654 File Offset: 0x0020D854
		public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> ts, Func<TValue, TKey> keyGetter) where TKey : struct, IComparable<TKey>
		{
			TValue result = default(TValue);
			TKey? tkey = default(TKey?);
			foreach (TValue tvalue in ts)
			{
				TKey tkey2 = keyGetter.Invoke(tvalue);
				if (tkey == null || tkey2.CompareTo(tkey.Value) < 0)
				{
					result = tvalue;
					tkey = new TKey?(tkey2);
				}
			}
			if (tkey == null)
			{
				throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
			}
			return result;
		}

		// Token: 0x060064ED RID: 25837 RVA: 0x0020F6F0 File Offset: 0x0020D8F0
		public static IEnumerable<T> Peek<T>(this IEnumerable<T> ts, Action<T> action)
		{
			foreach (T t in ts)
			{
				action.Invoke(t);
				yield return t;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
