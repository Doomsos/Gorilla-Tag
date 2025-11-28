using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200119A RID: 4506
	public class BoingManagerPostUpdatePump : MonoBehaviour
	{
		// Token: 0x060071B0 RID: 29104 RVA: 0x00252953 File Offset: 0x00250B53
		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x060071B1 RID: 29105 RVA: 0x00252960 File Offset: 0x00250B60
		private bool TryDestroyDuplicate()
		{
			if (BoingManager.s_managerGo == base.gameObject)
			{
				return false;
			}
			Object.Destroy(base.gameObject);
			return true;
		}

		// Token: 0x060071B2 RID: 29106 RVA: 0x00252982 File Offset: 0x00250B82
		private void FixedUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.FixedUpdate);
		}

		// Token: 0x060071B3 RID: 29107 RVA: 0x00252993 File Offset: 0x00250B93
		private void Update()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.Execute(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.EarlyUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.EarlyUpdate);
		}

		// Token: 0x060071B4 RID: 29108 RVA: 0x002529B6 File Offset: 0x00250BB6
		private void LateUpdate()
		{
			if (this.TryDestroyDuplicate())
			{
				return;
			}
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.FixedUpdate);
			BoingManager.Execute(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBehaviorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullReactorResults(BoingManager.UpdateMode.LateUpdate);
			BoingManager.PullBonesResults(BoingManager.UpdateMode.LateUpdate);
		}
	}
}
