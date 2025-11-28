using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200119B RID: 4507
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x060071B6 RID: 29110 RVA: 0x002529EB File Offset: 0x00250BEB
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x060071B7 RID: 29111 RVA: 0x002529EB File Offset: 0x00250BEB
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x060071B8 RID: 29112 RVA: 0x002529F3 File Offset: 0x00250BF3
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x060071B9 RID: 29113 RVA: 0x00252A1D File Offset: 0x00250C1D
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x040081E9 RID: 33257
		private int m_lastPumpedFrame = -1;
	}
}
