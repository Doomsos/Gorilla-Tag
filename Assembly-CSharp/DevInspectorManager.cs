using System;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class DevInspectorManager : MonoBehaviour
{
	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06001123 RID: 4387 RVA: 0x0005BC0F File Offset: 0x00059E0F
	public static DevInspectorManager instance
	{
		get
		{
			if (DevInspectorManager._instance == null)
			{
				DevInspectorManager._instance = Object.FindAnyObjectByType<DevInspectorManager>();
			}
			return DevInspectorManager._instance;
		}
	}

	// Token: 0x040015B3 RID: 5555
	private static DevInspectorManager _instance;
}
