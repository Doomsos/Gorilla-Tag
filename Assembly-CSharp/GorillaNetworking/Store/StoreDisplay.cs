using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F4A RID: 3914
	public class StoreDisplay : MonoBehaviour
	{
		// Token: 0x06006210 RID: 25104 RVA: 0x001F9A77 File Offset: 0x001F7C77
		private void GetAllDynamicCosmeticStands()
		{
			this.Stands = base.GetComponentsInChildren<DynamicCosmeticStand>();
		}

		// Token: 0x06006211 RID: 25105 RVA: 0x001F9A88 File Offset: 0x001F7C88
		private void SetDisplayNameForAllStands()
		{
			DynamicCosmeticStand[] componentsInChildren = base.GetComponentsInChildren<DynamicCosmeticStand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CopyChildsName();
			}
		}

		// Token: 0x040070C0 RID: 28864
		public string displayName = "";

		// Token: 0x040070C1 RID: 28865
		public DynamicCosmeticStand[] Stands;
	}
}
