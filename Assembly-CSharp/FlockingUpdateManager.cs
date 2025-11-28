using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005E4 RID: 1508
public class FlockingUpdateManager : MonoBehaviour
{
	// Token: 0x06002601 RID: 9729 RVA: 0x000CB5E0 File Offset: 0x000C97E0
	protected void Awake()
	{
		if (FlockingUpdateManager.hasInstance && FlockingUpdateManager.instance != null && FlockingUpdateManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		FlockingUpdateManager.SetInstance(this);
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x000CB610 File Offset: 0x000C9810
	public static void CreateManager()
	{
		FlockingUpdateManager.SetInstance(new GameObject("FlockingUpdateManager").AddComponent<FlockingUpdateManager>());
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x000CB626 File Offset: 0x000C9826
	private static void SetInstance(FlockingUpdateManager manager)
	{
		FlockingUpdateManager.instance = manager;
		FlockingUpdateManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000CB641 File Offset: 0x000C9841
	public static void RegisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (!FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Add(flocking);
		}
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x000CB667 File Offset: 0x000C9867
	public static void UnregisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Remove(flocking);
		}
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x000CB690 File Offset: 0x000C9890
	public void Update()
	{
		for (int i = 0; i < FlockingUpdateManager.allFlockings.Count; i++)
		{
			FlockingUpdateManager.allFlockings[i].InvokeUpdate();
		}
	}

	// Token: 0x040031E3 RID: 12771
	public static FlockingUpdateManager instance;

	// Token: 0x040031E4 RID: 12772
	public static bool hasInstance = false;

	// Token: 0x040031E5 RID: 12773
	public static List<Flocking> allFlockings = new List<Flocking>();
}
