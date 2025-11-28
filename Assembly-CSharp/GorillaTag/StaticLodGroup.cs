using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FE4 RID: 4068
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour
	{
		// Token: 0x060066E7 RID: 26343 RVA: 0x00217825 File Offset: 0x00215A25
		protected void Awake()
		{
			this.index = StaticLodManager.Register(this);
		}

		// Token: 0x060066E8 RID: 26344 RVA: 0x00217833 File Offset: 0x00215A33
		protected void OnEnable()
		{
			StaticLodManager.SetEnabled(this.index, true);
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x00217841 File Offset: 0x00215A41
		protected void OnDisable()
		{
			StaticLodManager.SetEnabled(this.index, false);
		}

		// Token: 0x060066EA RID: 26346 RVA: 0x0021784F File Offset: 0x00215A4F
		private void OnDestroy()
		{
			StaticLodManager.Unregister(this.index);
		}

		// Token: 0x0400756C RID: 30060
		public const int k_monoDefaultExecutionOrder = 2000;

		// Token: 0x0400756D RID: 30061
		private int index;

		// Token: 0x0400756E RID: 30062
		public float collisionEnableDistance = 3f;

		// Token: 0x0400756F RID: 30063
		public float uiFadeDistanceMax = 10f;
	}
}
