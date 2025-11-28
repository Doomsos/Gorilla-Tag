using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x0200043A RID: 1082
public class WorldShareableItemManager : MonoBehaviour
{
	// Token: 0x06001A99 RID: 6809 RVA: 0x0008CD52 File Offset: 0x0008AF52
	protected void Awake()
	{
		if (WorldShareableItemManager.hasInstance && WorldShareableItemManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		WorldShareableItemManager.SetInstance(this);
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x0008CD75 File Offset: 0x0008AF75
	protected void OnDestroy()
	{
		if (WorldShareableItemManager.instance == this)
		{
			WorldShareableItemManager.hasInstance = false;
			WorldShareableItemManager.instance = null;
		}
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x0008CD90 File Offset: 0x0008AF90
	protected void Update()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < WorldShareableItemManager.worldShareableItems.Count; i++)
		{
			if (WorldShareableItemManager.worldShareableItems[i] != null)
			{
				WorldShareableItemManager.worldShareableItems[i].TriggeredUpdate();
			}
		}
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x0008CDDD File Offset: 0x0008AFDD
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.SetInstance(new GameObject("WorldShareableItemManager").AddComponent<WorldShareableItemManager>());
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x0008CDFB File Offset: 0x0008AFFB
	private static void SetInstance(WorldShareableItemManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		WorldShareableItemManager.instance = manager;
		WorldShareableItemManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x0008CE1E File Offset: 0x0008B01E
	public static void Register(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (!WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Add(worldShareableItem);
		}
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x0008CE4C File Offset: 0x0008B04C
	public static void Unregister(WorldShareableItem worldShareableItem)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!WorldShareableItemManager.hasInstance)
		{
			WorldShareableItemManager.CreateManager();
		}
		if (WorldShareableItemManager.worldShareableItems.Contains(worldShareableItem))
		{
			WorldShareableItemManager.worldShareableItems.Remove(worldShareableItem);
		}
	}

	// Token: 0x04002409 RID: 9225
	public static WorldShareableItemManager instance;

	// Token: 0x0400240A RID: 9226
	private static bool hasInstance = false;

	// Token: 0x0400240B RID: 9227
	public static readonly List<WorldShareableItem> worldShareableItems = new List<WorldShareableItem>(1024);
}
