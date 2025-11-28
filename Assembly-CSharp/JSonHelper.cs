using System;
using UnityEngine;

// Token: 0x0200045A RID: 1114
public static class JSonHelper
{
	// Token: 0x06001C55 RID: 7253 RVA: 0x000967BF File Offset: 0x000949BF
	public static T[] FromJson<T>(string json)
	{
		return JsonUtility.FromJson<JSonHelper.Wrapper<T>>(json).Items;
	}

	// Token: 0x06001C56 RID: 7254 RVA: 0x000967CC File Offset: 0x000949CC
	public static string ToJson<T>(T[] array)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		});
	}

	// Token: 0x06001C57 RID: 7255 RVA: 0x000967DF File Offset: 0x000949DF
	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		return JsonUtility.ToJson(new JSonHelper.Wrapper<T>
		{
			Items = array
		}, prettyPrint);
	}

	// Token: 0x0200045B RID: 1115
	[Serializable]
	private class Wrapper<T>
	{
		// Token: 0x04002658 RID: 9816
		public T[] Items;
	}
}
