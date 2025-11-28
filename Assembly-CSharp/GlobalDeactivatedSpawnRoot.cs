using System;
using UnityEngine;

// Token: 0x02000296 RID: 662
public static class GlobalDeactivatedSpawnRoot
{
	// Token: 0x060010F1 RID: 4337 RVA: 0x0005B490 File Offset: 0x00059690
	public static Transform GetOrCreate()
	{
		if (!GlobalDeactivatedSpawnRoot._xform)
		{
			GlobalDeactivatedSpawnRoot._xform = new GameObject("GlobalDeactivatedSpawnRoot").transform;
			GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
			Object.DontDestroyOnLoad(GlobalDeactivatedSpawnRoot._xform.gameObject);
		}
		GlobalDeactivatedSpawnRoot._xform.gameObject.SetActive(false);
		return GlobalDeactivatedSpawnRoot._xform;
	}

	// Token: 0x0400153D RID: 5437
	private static Transform _xform;
}
