using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000474 RID: 1140
[DefaultExecutionOrder(2000)]
public class ChestObjectHysteresisManager : MonoBehaviourTick
{
	// Token: 0x06001CF1 RID: 7409 RVA: 0x000992CA File Offset: 0x000974CA
	protected void Awake()
	{
		if (ChestObjectHysteresisManager.hasInstance && ChestObjectHysteresisManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ChestObjectHysteresisManager.SetInstance(this);
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000992ED File Offset: 0x000974ED
	public static void CreateManager()
	{
		ChestObjectHysteresisManager.SetInstance(new GameObject("ChestObjectHysteresisManager").AddComponent<ChestObjectHysteresisManager>());
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x00099303 File Offset: 0x00097503
	private static void SetInstance(ChestObjectHysteresisManager manager)
	{
		ChestObjectHysteresisManager.instance = manager;
		ChestObjectHysteresisManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x0009931E File Offset: 0x0009751E
	public static void RegisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (!ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Add(cOH);
		}
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x00099344 File Offset: 0x00097544
	public static void UnregisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Remove(cOH);
		}
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0009936C File Offset: 0x0009756C
	public override void Tick()
	{
		for (int i = 0; i < ChestObjectHysteresisManager.allChests.Count; i++)
		{
			ChestObjectHysteresisManager.allChests[i].InvokeUpdate();
		}
	}

	// Token: 0x040026E7 RID: 9959
	public static ChestObjectHysteresisManager instance;

	// Token: 0x040026E8 RID: 9960
	public static bool hasInstance = false;

	// Token: 0x040026E9 RID: 9961
	public static List<ChestObjectHysteresis> allChests = new List<ChestObjectHysteresis>();
}
