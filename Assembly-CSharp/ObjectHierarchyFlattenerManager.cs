using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class ObjectHierarchyFlattenerManager : MonoBehaviourPostTick
{
	// Token: 0x06001202 RID: 4610 RVA: 0x0005F03E File Offset: 0x0005D23E
	protected void Awake()
	{
		if (ObjectHierarchyFlattenerManager.hasInstance && ObjectHierarchyFlattenerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ObjectHierarchyFlattenerManager.SetInstance(this);
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x0005F061 File Offset: 0x0005D261
	public static void CreateManager()
	{
		ObjectHierarchyFlattenerManager.SetInstance(new GameObject("ObjectHierarchyFlattenerManager").AddComponent<ObjectHierarchyFlattenerManager>());
	}

	// Token: 0x06001204 RID: 4612 RVA: 0x0005F077 File Offset: 0x0005D277
	private static void SetInstance(ObjectHierarchyFlattenerManager manager)
	{
		ObjectHierarchyFlattenerManager.instance = manager;
		ObjectHierarchyFlattenerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001205 RID: 4613 RVA: 0x0005F092 File Offset: 0x0005D292
	public static void RegisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (!ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Add(rbWI);
		}
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x0005F0B8 File Offset: 0x0005D2B8
	public static void UnregisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Remove(rbWI);
		}
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x0005F0E0 File Offset: 0x0005D2E0
	public override void PostTick()
	{
		for (int i = 0; i < ObjectHierarchyFlattenerManager.alloHF.Count; i++)
		{
			ObjectHierarchyFlattenerManager.alloHF[i].InvokeLateUpdate();
		}
	}

	// Token: 0x040016AB RID: 5803
	public static ObjectHierarchyFlattenerManager instance;

	// Token: 0x040016AC RID: 5804
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x040016AD RID: 5805
	public static List<ObjectHierarchyFlattener> alloHF = new List<ObjectHierarchyFlattener>();
}
