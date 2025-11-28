using System;
using System.Diagnostics;
using UnityEngine;

namespace BuildSafe
{
	// Token: 0x02000EA5 RID: 3749
	public static class AssetDatabase
	{
		// Token: 0x06005DB9 RID: 23993 RVA: 0x001E16B0 File Offset: 0x001DF8B0
		public static T LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return default(T);
		}

		// Token: 0x06005DBA RID: 23994 RVA: 0x001E16C6 File Offset: 0x001DF8C6
		public static T[] LoadAssetsOfType<T>() where T : Object
		{
			return Array.Empty<T>();
		}

		// Token: 0x06005DBB RID: 23995 RVA: 0x001E16CD File Offset: 0x001DF8CD
		public static string[] FindAssetsOfType<T>() where T : Object
		{
			return Array.Empty<string>();
		}

		// Token: 0x06005DBC RID: 23996 RVA: 0x001E16D4 File Offset: 0x001DF8D4
		[Conditional("UNITY_EDITOR")]
		public static void SaveToDisk(params Object[] assetsToSave)
		{
			AssetDatabase.SaveAssetsToDisk(assetsToSave, true);
		}

		// Token: 0x06005DBD RID: 23997 RVA: 0x00002789 File Offset: 0x00000989
		public static void SaveAssetsToDisk(Object[] assetsToSave, bool saveProject = true)
		{
		}
	}
}
