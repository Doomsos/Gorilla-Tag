using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DF2 RID: 3570
	public class GorillaTimerManager : MonoBehaviour
	{
		// Token: 0x06005914 RID: 22804 RVA: 0x001C7E5B File Offset: 0x001C605B
		protected void Awake()
		{
			if (GorillaTimerManager.hasInstance && GorillaTimerManager.instance != null && GorillaTimerManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaTimerManager.SetInstance(this);
		}

		// Token: 0x06005915 RID: 22805 RVA: 0x001C7E8B File Offset: 0x001C608B
		public static void CreateManager()
		{
			GorillaTimerManager.SetInstance(new GameObject("GorillaTimerManager").AddComponent<GorillaTimerManager>());
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x001C7EA1 File Offset: 0x001C60A1
		private static void SetInstance(GorillaTimerManager manager)
		{
			GorillaTimerManager.instance = manager;
			GorillaTimerManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x001C7EBC File Offset: 0x001C60BC
		public static void RegisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (!GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Add(gTimer);
			}
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x001C7EE2 File Offset: 0x001C60E2
		public static void UnregisterGorillaTimer(GorillaTimer gTimer)
		{
			if (!GorillaTimerManager.hasInstance)
			{
				GorillaTimerManager.CreateManager();
			}
			if (GorillaTimerManager.allTimers.Contains(gTimer))
			{
				GorillaTimerManager.allTimers.Remove(gTimer);
			}
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x001C7F0C File Offset: 0x001C610C
		public void Update()
		{
			for (int i = 0; i < GorillaTimerManager.allTimers.Count; i++)
			{
				GorillaTimerManager.allTimers[i].InvokeUpdate();
			}
		}

		// Token: 0x0400662C RID: 26156
		public static GorillaTimerManager instance;

		// Token: 0x0400662D RID: 26157
		public static bool hasInstance = false;

		// Token: 0x0400662E RID: 26158
		public static List<GorillaTimer> allTimers = new List<GorillaTimer>();
	}
}
