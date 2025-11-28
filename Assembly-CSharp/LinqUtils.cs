using System;
using System.Collections.Generic;

// Token: 0x02000C52 RID: 3154
public static class LinqUtils
{
	// Token: 0x06004D23 RID: 19747 RVA: 0x0019035F File Offset: 0x0018E55F
	public static IEnumerable<TResult> SelectManyNullSafe<TSource, TResult>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TResult>> selector)
	{
		if (sources == null)
		{
			yield break;
		}
		if (selector == null)
		{
			yield break;
		}
		foreach (TSource tsource in sources)
		{
			if (tsource != null)
			{
				IEnumerable<TResult> enumerable = selector.Invoke(tsource);
				foreach (TResult tresult in enumerable)
				{
					if (tresult != null)
					{
						yield return tresult;
					}
				}
				IEnumerator<TResult> enumerator2 = null;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06004D24 RID: 19748 RVA: 0x00190376 File Offset: 0x0018E576
	public static IEnumerable<TSource> DistinctBy<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
	{
		HashSet<TResult> set = new HashSet<TResult>();
		foreach (TSource tsource in source)
		{
			TResult tresult = selector.Invoke(tsource);
			if (set.Add(tresult))
			{
				yield return tsource;
			}
		}
		IEnumerator<TSource> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x06004D25 RID: 19749 RVA: 0x00190390 File Offset: 0x0018E590
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T t in source)
		{
			action.Invoke(t);
		}
		return source;
	}

	// Token: 0x06004D26 RID: 19750 RVA: 0x001903DC File Offset: 0x0018E5DC
	public static T[] AsArray<T>(this IEnumerable<T> source)
	{
		return (T[])source;
	}

	// Token: 0x06004D27 RID: 19751 RVA: 0x001903E4 File Offset: 0x0018E5E4
	public static List<T> AsList<T>(this IEnumerable<T> source)
	{
		return (List<T>)source;
	}

	// Token: 0x06004D28 RID: 19752 RVA: 0x001903EC File Offset: 0x0018E5EC
	public static IList<T> Transform<T>(this IList<T> list, Func<T, T> action)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = action.Invoke(list[i]);
		}
		return list;
	}

	// Token: 0x06004D29 RID: 19753 RVA: 0x0019041F File Offset: 0x0018E61F
	public static IEnumerable<T> Self<T>(this T value)
	{
		yield return value;
		yield break;
	}
}
