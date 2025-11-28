using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F8A RID: 3978
	public class RigidbodyWaterInteractionManager : MonoBehaviour
	{
		// Token: 0x060063D9 RID: 25561 RVA: 0x0020793E File Offset: 0x00205B3E
		protected void Awake()
		{
			if (RigidbodyWaterInteractionManager.hasInstance && RigidbodyWaterInteractionManager.instance != this)
			{
				Object.Destroy(this);
				return;
			}
			RigidbodyWaterInteractionManager.SetInstance(this);
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x00207961 File Offset: 0x00205B61
		public static void CreateManager()
		{
			RigidbodyWaterInteractionManager.SetInstance(new GameObject("RigidbodyWaterInteractionManager").AddComponent<RigidbodyWaterInteractionManager>());
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x00207977 File Offset: 0x00205B77
		private static void SetInstance(RigidbodyWaterInteractionManager manager)
		{
			RigidbodyWaterInteractionManager.instance = manager;
			RigidbodyWaterInteractionManager.hasInstance = true;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(manager);
			}
		}

		// Token: 0x060063DC RID: 25564 RVA: 0x00207992 File Offset: 0x00205B92
		public static void RegisterRBWI(RigidbodyWaterInteraction rbWI)
		{
			if (!RigidbodyWaterInteractionManager.hasInstance)
			{
				RigidbodyWaterInteractionManager.CreateManager();
			}
			if (!RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
			{
				RigidbodyWaterInteractionManager.allrBWI.Add(rbWI);
			}
		}

		// Token: 0x060063DD RID: 25565 RVA: 0x002079B8 File Offset: 0x00205BB8
		public static void UnregisterRBWI(RigidbodyWaterInteraction rbWI)
		{
			if (!RigidbodyWaterInteractionManager.hasInstance)
			{
				RigidbodyWaterInteractionManager.CreateManager();
			}
			if (RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
			{
				RigidbodyWaterInteractionManager.allrBWI.Remove(rbWI);
			}
		}

		// Token: 0x060063DE RID: 25566 RVA: 0x002079E0 File Offset: 0x00205BE0
		public void FixedUpdate()
		{
			for (int i = 0; i < RigidbodyWaterInteractionManager.allrBWI.Count; i++)
			{
				RigidbodyWaterInteractionManager.allrBWI[i].InvokeFixedUpdate();
			}
		}

		// Token: 0x04007356 RID: 29526
		public static RigidbodyWaterInteractionManager instance;

		// Token: 0x04007357 RID: 29527
		[OnEnterPlay_Set(false)]
		public static bool hasInstance = false;

		// Token: 0x04007358 RID: 29528
		public static List<RigidbodyWaterInteraction> allrBWI = new List<RigidbodyWaterInteraction>();
	}
}
