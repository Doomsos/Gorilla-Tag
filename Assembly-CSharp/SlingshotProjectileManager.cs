using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class SlingshotProjectileManager : MonoBehaviourTick
{
	// Token: 0x06001A3C RID: 6716 RVA: 0x0008BCC7 File Offset: 0x00089EC7
	protected void Awake()
	{
		if (SlingshotProjectileManager.hasInstance && SlingshotProjectileManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SlingshotProjectileManager.SetInstance(this);
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x0008BCEA File Offset: 0x00089EEA
	public static void CreateManager()
	{
		SlingshotProjectileManager.SetInstance(new GameObject("SlingshotProjectileManager").AddComponent<SlingshotProjectileManager>());
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x0008BD00 File Offset: 0x00089F00
	private static void SetInstance(SlingshotProjectileManager manager)
	{
		SlingshotProjectileManager.instance = manager;
		SlingshotProjectileManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x0008BD1B File Offset: 0x00089F1B
	public static void RegisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (!SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Add(sP);
		}
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x0008BD41 File Offset: 0x00089F41
	public static void UnregisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Remove(sP);
		}
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x0008BD68 File Offset: 0x00089F68
	public override void Tick()
	{
		for (int i = 0; i < SlingshotProjectileManager.allsP.Count; i++)
		{
			SlingshotProjectileManager.allsP[i].InvokeUpdate();
		}
	}

	// Token: 0x040023C4 RID: 9156
	public static SlingshotProjectileManager instance;

	// Token: 0x040023C5 RID: 9157
	public static bool hasInstance = false;

	// Token: 0x040023C6 RID: 9158
	public static List<SlingshotProjectile> allsP = new List<SlingshotProjectile>();
}
