using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000C23 RID: 3107
public static class AssetUtils
{
	// Token: 0x06004C6A RID: 19562 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	public static void ExecAndUnloadUnused(Action action)
	{
	}

	// Token: 0x06004C6B RID: 19563 RVA: 0x0018D4C6 File Offset: 0x0018B6C6
	[Conditional("UNITY_EDITOR")]
	public static void LoadAssetOfType<T>(ref T result, ref string resultPath) where T : Object
	{
		result = default(T);
		resultPath = null;
	}

	// Token: 0x06004C6C RID: 19564 RVA: 0x0018D4D2 File Offset: 0x0018B6D2
	[Conditional("UNITY_EDITOR")]
	public static void FindAllAssetsOfType<T>(ref T[] results, ref string[] assetPaths) where T : Object
	{
		results = Array.Empty<T>();
	}

	// Token: 0x06004C6D RID: 19565 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave<T>(this IList<T> assets, Action<T> onPreSave = null, bool unloadUnusedAfter = false) where T : Object
	{
	}

	// Token: 0x06004C6E RID: 19566 RVA: 0x00002789 File Offset: 0x00000989
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void ForceSave(this Object asset)
	{
	}

	// Token: 0x06004C6F RID: 19567 RVA: 0x0018D4DB File Offset: 0x0018B6DB
	public static long ComputeAssetId(this Object asset, bool unsigned = false)
	{
		return 0L;
	}
}
