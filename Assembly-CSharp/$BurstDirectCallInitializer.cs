using System;
using Unity.Burst;
using UnityEngine;

internal static class $BurstDirectCallInitializer
{
	[RuntimeInitializeOnLoadMethod(2)]
	private static void Initialize()
	{
		BurstCompilerOptions options = BurstCompiler.Options;
	}
}
