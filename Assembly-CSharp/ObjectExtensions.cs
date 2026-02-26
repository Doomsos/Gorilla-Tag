using System;
using UnityEngine;

public static class ObjectExtensions
{
	public static void Destroy(this UnityEngine.Object target)
	{
		UnityEngine.Object.Destroy(target);
	}
}
