using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DB1 RID: 3505
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06005633 RID: 22067 RVA: 0x001B19CD File Offset: 0x001AFBCD
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x04006355 RID: 25429
		public Transform center;
	}
}
