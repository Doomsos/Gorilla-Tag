using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F3F RID: 3903
	public class StoreBundleData : ScriptableObject
	{
		// Token: 0x060061C5 RID: 25029 RVA: 0x001F7534 File Offset: 0x001F5734
		public void OnValidate()
		{
			if (this.playfabBundleID.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + base.name);
			}
			if (this.bundleSKU.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + base.name);
			}
		}

		// Token: 0x04007078 RID: 28792
		public string playfabBundleID = "NULL";

		// Token: 0x04007079 RID: 28793
		public string bundleSKU = "NULL SKU";

		// Token: 0x0400707A RID: 28794
		public NexusCreatorCode creatorCode;

		// Token: 0x0400707B RID: 28795
		public Sprite bundleImage;

		// Token: 0x0400707C RID: 28796
		public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";
	}
}
