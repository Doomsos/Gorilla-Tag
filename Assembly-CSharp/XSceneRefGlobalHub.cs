using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200033E RID: 830
public static class XSceneRefGlobalHub
{
	// Token: 0x060013FC RID: 5116 RVA: 0x000738E8 File Offset: 0x00071AE8
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	// Token: 0x060013FD RID: 5117 RVA: 0x00073918 File Offset: 0x00071B18
	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			if (sceneIndex < 0 || sceneIndex >= XSceneRefGlobalHub.registry.Count)
			{
				Debug.LogErrorFormat(obj, "Invalid scene index {0} cannot remove ID {1}", new object[]
				{
					sceneIndex,
					ID
				});
			}
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	// Token: 0x060013FE RID: 5118 RVA: 0x0007397A File Offset: 0x00071B7A
	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, ref result);
	}

	// Token: 0x060013FF RID: 5119 RVA: 0x00073990 File Offset: 0x00071B90
	// Note: this type is marked as 'beforefieldinit'.
	static XSceneRefGlobalHub()
	{
		List<Dictionary<int, XSceneRefTarget>> list = new List<Dictionary<int, XSceneRefTarget>>();
		Dictionary<int, XSceneRefTarget> dictionary = new Dictionary<int, XSceneRefTarget>();
		dictionary.Add(0, null);
		list.Add(dictionary);
		Dictionary<int, XSceneRefTarget> dictionary2 = new Dictionary<int, XSceneRefTarget>();
		dictionary2.Add(0, null);
		list.Add(dictionary2);
		Dictionary<int, XSceneRefTarget> dictionary3 = new Dictionary<int, XSceneRefTarget>();
		dictionary3.Add(0, null);
		list.Add(dictionary3);
		Dictionary<int, XSceneRefTarget> dictionary4 = new Dictionary<int, XSceneRefTarget>();
		dictionary4.Add(0, null);
		list.Add(dictionary4);
		Dictionary<int, XSceneRefTarget> dictionary5 = new Dictionary<int, XSceneRefTarget>();
		dictionary5.Add(0, null);
		list.Add(dictionary5);
		Dictionary<int, XSceneRefTarget> dictionary6 = new Dictionary<int, XSceneRefTarget>();
		dictionary6.Add(0, null);
		list.Add(dictionary6);
		Dictionary<int, XSceneRefTarget> dictionary7 = new Dictionary<int, XSceneRefTarget>();
		dictionary7.Add(0, null);
		list.Add(dictionary7);
		Dictionary<int, XSceneRefTarget> dictionary8 = new Dictionary<int, XSceneRefTarget>();
		dictionary8.Add(0, null);
		list.Add(dictionary8);
		Dictionary<int, XSceneRefTarget> dictionary9 = new Dictionary<int, XSceneRefTarget>();
		dictionary9.Add(0, null);
		list.Add(dictionary9);
		Dictionary<int, XSceneRefTarget> dictionary10 = new Dictionary<int, XSceneRefTarget>();
		dictionary10.Add(0, null);
		list.Add(dictionary10);
		Dictionary<int, XSceneRefTarget> dictionary11 = new Dictionary<int, XSceneRefTarget>();
		dictionary11.Add(0, null);
		list.Add(dictionary11);
		Dictionary<int, XSceneRefTarget> dictionary12 = new Dictionary<int, XSceneRefTarget>();
		dictionary12.Add(0, null);
		list.Add(dictionary12);
		Dictionary<int, XSceneRefTarget> dictionary13 = new Dictionary<int, XSceneRefTarget>();
		dictionary13.Add(0, null);
		list.Add(dictionary13);
		Dictionary<int, XSceneRefTarget> dictionary14 = new Dictionary<int, XSceneRefTarget>();
		dictionary14.Add(0, null);
		list.Add(dictionary14);
		Dictionary<int, XSceneRefTarget> dictionary15 = new Dictionary<int, XSceneRefTarget>();
		dictionary15.Add(0, null);
		list.Add(dictionary15);
		Dictionary<int, XSceneRefTarget> dictionary16 = new Dictionary<int, XSceneRefTarget>();
		dictionary16.Add(0, null);
		list.Add(dictionary16);
		Dictionary<int, XSceneRefTarget> dictionary17 = new Dictionary<int, XSceneRefTarget>();
		dictionary17.Add(0, null);
		list.Add(dictionary17);
		Dictionary<int, XSceneRefTarget> dictionary18 = new Dictionary<int, XSceneRefTarget>();
		dictionary18.Add(0, null);
		list.Add(dictionary18);
		Dictionary<int, XSceneRefTarget> dictionary19 = new Dictionary<int, XSceneRefTarget>();
		dictionary19.Add(0, null);
		list.Add(dictionary19);
		XSceneRefGlobalHub.registry = list;
	}

	// Token: 0x04001E95 RID: 7829
	private static List<Dictionary<int, XSceneRefTarget>> registry;
}
