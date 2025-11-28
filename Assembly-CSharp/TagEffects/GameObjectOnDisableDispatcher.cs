using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000F79 RID: 3961
	public class GameObjectOnDisableDispatcher : MonoBehaviour
	{
		// Token: 0x140000AF RID: 175
		// (add) Token: 0x06006300 RID: 25344 RVA: 0x001FE9C8 File Offset: 0x001FCBC8
		// (remove) Token: 0x06006301 RID: 25345 RVA: 0x001FEA00 File Offset: 0x001FCC00
		public event GameObjectOnDisableDispatcher.OnDisabledEvent OnDisabled;

		// Token: 0x06006302 RID: 25346 RVA: 0x001FEA35 File Offset: 0x001FCC35
		private void OnDisable()
		{
			if (this.OnDisabled != null)
			{
				this.OnDisabled(this);
			}
		}

		// Token: 0x02000F7A RID: 3962
		// (Invoke) Token: 0x06006305 RID: 25349
		public delegate void OnDisabledEvent(GameObjectOnDisableDispatcher me);
	}
}
