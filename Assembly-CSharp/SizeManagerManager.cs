using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000832 RID: 2098
public class SizeManagerManager : MonoBehaviour
{
	// Token: 0x0600372D RID: 14125 RVA: 0x001297B0 File Offset: 0x001279B0
	protected void Awake()
	{
		if (SizeManagerManager.hasInstance && SizeManagerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SizeManagerManager.SetInstance(this);
	}

	// Token: 0x0600372E RID: 14126 RVA: 0x001297D3 File Offset: 0x001279D3
	public static void CreateManager()
	{
		SizeManagerManager.SetInstance(new GameObject("SizeManagerManager").AddComponent<SizeManagerManager>());
	}

	// Token: 0x0600372F RID: 14127 RVA: 0x001297E9 File Offset: 0x001279E9
	private static void SetInstance(SizeManagerManager manager)
	{
		SizeManagerManager.instance = manager;
		SizeManagerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x00129804 File Offset: 0x00127A04
	public static void RegisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (!SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Add(sM);
		}
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x0012982A File Offset: 0x00127A2A
	public static void UnregisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Remove(sM);
		}
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x00129854 File Offset: 0x00127A54
	public void FixedUpdate()
	{
		for (int i = 0; i < SizeManagerManager.allSM.Count; i++)
		{
			SizeManagerManager.allSM[i].InvokeFixedUpdate();
		}
	}

	// Token: 0x040046A5 RID: 18085
	[OnEnterPlay_SetNull]
	public static SizeManagerManager instance;

	// Token: 0x040046A6 RID: 18086
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x040046A7 RID: 18087
	[OnEnterPlay_Clear]
	public static List<SizeManager> allSM = new List<SizeManager>();
}
