using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class UnityObjectUtils
{
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

	[MethodImpl(256)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
