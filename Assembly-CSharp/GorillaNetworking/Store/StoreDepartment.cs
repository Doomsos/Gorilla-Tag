using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F49 RID: 3913
	public class StoreDepartment : MonoBehaviour
	{
		// Token: 0x0600620E RID: 25102 RVA: 0x001F99D4 File Offset: 0x001F7BD4
		private void FindAllDisplays()
		{
			this.Displays = base.GetComponentsInChildren<StoreDisplay>();
			for (int i = this.Displays.Length - 1; i >= 0; i--)
			{
				if (string.IsNullOrEmpty(this.Displays[i].displayName))
				{
					this.Displays[i] = this.Displays[this.Displays.Length - 1];
					Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
				}
			}
		}

		// Token: 0x040070BE RID: 28862
		public StoreDisplay[] Displays;

		// Token: 0x040070BF RID: 28863
		public string departmentName = "";
	}
}
