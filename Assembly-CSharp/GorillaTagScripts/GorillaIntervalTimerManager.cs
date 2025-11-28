using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DED RID: 3565
	public class GorillaIntervalTimerManager : MonoBehaviour
	{
		// Token: 0x060058E3 RID: 22755 RVA: 0x001C77F2 File Offset: 0x001C59F2
		protected void Awake()
		{
			if (GorillaIntervalTimerManager.hasInstance && GorillaIntervalTimerManager.instance != null && GorillaIntervalTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaIntervalTimerManager.SetInstance(this);
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x001C7822 File Offset: 0x001C5A22
		private static void CreateManager()
		{
			GorillaIntervalTimerManager.SetInstance(new GameObject("GorillaIntervalTimerManager").AddComponent<GorillaIntervalTimerManager>());
		}

		// Token: 0x060058E5 RID: 22757 RVA: 0x001C7838 File Offset: 0x001C5A38
		private static void SetInstance(GorillaIntervalTimerManager manager)
		{
			GorillaIntervalTimerManager.instance = manager;
			GorillaIntervalTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060058E6 RID: 22758 RVA: 0x001C7853 File Offset: 0x001C5A53
		public static void RegisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (!GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x060058E7 RID: 22759 RVA: 0x001C7879 File Offset: 0x001C5A79
		public static void UnregisterGorillaTimer(GorillaIntervalTimer gTimer)
		{
			if (!GorillaIntervalTimerManager.hasInstance)
			{
				GorillaIntervalTimerManager.CreateManager();
			}
			if (GorillaIntervalTimerManager.allTimers.Contains(gTimer))
			{
				GorillaIntervalTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x060058E8 RID: 22760 RVA: 0x001C78A0 File Offset: 0x001C5AA0
		private void Update()
		{
			for (int i = 0; i < GorillaIntervalTimerManager.allTimers.Count; i++)
			{
				GorillaIntervalTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x04006610 RID: 26128
		private static GorillaIntervalTimerManager instance;

		// Token: 0x04006611 RID: 26129
		private static bool hasInstance = false;

		// Token: 0x04006612 RID: 26130
		private static List<GorillaIntervalTimer> allTimers = new List<GorillaIntervalTimer>();
	}
}
