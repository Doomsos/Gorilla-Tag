using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityObjectUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AsNull<T>(this T obj) where T : UnityEngine.Object
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this UnityEngine.Object obj)
	{
		UnityEngine.Object.Destroy(obj);
	}
}
