using System;
using System.Diagnostics;
using UnityEngine.SceneManagement;

// Token: 0x02000CA6 RID: 3238
public static class GTSceneUtils
{
	// Token: 0x06004F0E RID: 20238 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	public static void AddToBuild(GTScene scene)
	{
	}

	// Token: 0x06004F0F RID: 20239 RVA: 0x001986ED File Offset: 0x001968ED
	public static bool Equals(GTScene x, Scene y)
	{
		return !(x == null) && y.IsValid() && x.Equals(y);
	}

	// Token: 0x06004F10 RID: 20240 RVA: 0x00198711 File Offset: 0x00196911
	public static GTScene[] ScenesInBuild()
	{
		return Array.Empty<GTScene>();
	}

	// Token: 0x06004F11 RID: 20241 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	public static void SyncBuildScenes()
	{
	}
}
