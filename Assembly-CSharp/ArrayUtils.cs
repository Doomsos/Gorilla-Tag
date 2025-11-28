using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C22 RID: 3106
public static class ArrayUtils
{
	// Token: 0x06004C60 RID: 19552 RVA: 0x0018D2E2 File Offset: 0x0018B4E2
	[MethodImpl(256)]
	public static int BinarySearch<T>(this T[] array, T value) where T : IComparable<T>
	{
		return Array.BinarySearch<T>(array, 0, array.Length, value);
	}

	// Token: 0x06004C61 RID: 19553 RVA: 0x0018D2EF File Offset: 0x0018B4EF
	[MethodImpl(256)]
	public static bool IsNullOrEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

	// Token: 0x06004C62 RID: 19554 RVA: 0x0018D2FB File Offset: 0x0018B4FB
	[MethodImpl(256)]
	public static bool IsNullOrEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	// Token: 0x06004C63 RID: 19555 RVA: 0x0018D30C File Offset: 0x0018B50C
	[MethodImpl(256)]
	public static void Swap<T>(this T[] array, int from, int to)
	{
		T t = array[from];
		T t2 = array[to];
		array[to] = t;
		array[from] = t2;
	}

	// Token: 0x06004C64 RID: 19556 RVA: 0x0018D344 File Offset: 0x0018B544
	[MethodImpl(256)]
	public static void Swap<T>(this List<T> list, int from, int to)
	{
		T t = list[from];
		T t2 = list[to];
		list[to] = t;
		list[from] = t2;
	}

	// Token: 0x06004C65 RID: 19557 RVA: 0x0018D380 File Offset: 0x0018B580
	[MethodImpl(256)]
	public static T[] Clone<T>(T[] source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Length == 0)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[source.Length];
		for (int i = 0; i < source.Length; i++)
		{
			array[i] = source[i];
		}
		return array;
	}

	// Token: 0x06004C66 RID: 19558 RVA: 0x0018D3C2 File Offset: 0x0018B5C2
	[MethodImpl(256)]
	public static List<T> Clone<T>(List<T> source)
	{
		if (source == null)
		{
			return null;
		}
		if (source.Count == 0)
		{
			return new List<T>();
		}
		return new List<T>(source);
	}

	// Token: 0x06004C67 RID: 19559 RVA: 0x0018D3E0 File Offset: 0x0018B5E0
	[MethodImpl(256)]
	public static int IndexOfRef<T>(this T[] array, T value) where T : class
	{
		if (array == null || array.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06004C68 RID: 19560 RVA: 0x0018D41C File Offset: 0x0018B61C
	[MethodImpl(256)]
	public static int IndexOfRef<T>(this List<T> list, T value) where T : class
	{
		if (list == null || list.Count == 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06004C69 RID: 19561 RVA: 0x0018D460 File Offset: 0x0018B660
	public static bool GTEnsureNoNulls<T>(ref T[] unityObjs) where T : Object
	{
		if (unityObjs == null)
		{
			unityObjs = Array.Empty<T>();
		}
		int num = 0;
		for (int i = 0; i < unityObjs.Length; i++)
		{
			if (!(unityObjs[i] == null))
			{
				unityObjs[num] = unityObjs[i];
				num++;
			}
		}
		bool flag = num != unityObjs.Length;
		if (flag)
		{
			Array.Resize<T>(ref unityObjs, num);
		}
		return flag;
	}
}
