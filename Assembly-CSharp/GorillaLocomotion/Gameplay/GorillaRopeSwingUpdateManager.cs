using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F9B RID: 3995
	public class GorillaRopeSwingUpdateManager : MonoBehaviour
	{
		// Token: 0x06006454 RID: 25684 RVA: 0x0020BA71 File Offset: 0x00209C71
		protected void Awake()
		{
			if (GorillaRopeSwingUpdateManager.hasInstance && GorillaRopeSwingUpdateManager.instance != null && GorillaRopeSwingUpdateManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			GorillaRopeSwingUpdateManager.SetInstance(this);
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x0020BAA1 File Offset: 0x00209CA1
		public static void CreateManager()
		{
			GorillaRopeSwingUpdateManager.SetInstance(new GameObject("GorillaRopeSwingUpdateManager").AddComponent<GorillaRopeSwingUpdateManager>());
		}

		// Token: 0x06006456 RID: 25686 RVA: 0x0020BAB7 File Offset: 0x00209CB7
		private static void SetInstance(GorillaRopeSwingUpdateManager manager)
		{
			GorillaRopeSwingUpdateManager.instance = manager;
			GorillaRopeSwingUpdateManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x0020BAD2 File Offset: 0x00209CD2
		public static void RegisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (!GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Add(ropeSwing);
			}
		}

		// Token: 0x06006458 RID: 25688 RVA: 0x0020BAF8 File Offset: 0x00209CF8
		public static void UnregisterRopeSwing(GorillaRopeSwing ropeSwing)
		{
			if (!GorillaRopeSwingUpdateManager.hasInstance)
			{
				GorillaRopeSwingUpdateManager.CreateManager();
			}
			if (GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Remove(ropeSwing);
			}
		}

		// Token: 0x06006459 RID: 25689 RVA: 0x0020BB20 File Offset: 0x00209D20
		public void Update()
		{
			for (int i = 0; i < GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Count; i++)
			{
				GorillaRopeSwingUpdateManager.allGorillaRopeSwings[i].InvokeUpdate();
			}
		}

		// Token: 0x04007400 RID: 29696
		public static GorillaRopeSwingUpdateManager instance;

		// Token: 0x04007401 RID: 29697
		public static bool hasInstance = false;

		// Token: 0x04007402 RID: 29698
		public static List<GorillaRopeSwing> allGorillaRopeSwings = new List<GorillaRopeSwing>();
	}
}
