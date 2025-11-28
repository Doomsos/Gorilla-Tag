using System;
using System.IO;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F4E RID: 3918
	[Serializable]
	public class StoreItem
	{
		// Token: 0x0600622F RID: 25135 RVA: 0x001F9E78 File Offset: 0x001F8078
		public static void SerializeItemsAsJSON(StoreItem[] items)
		{
			string text = "";
			foreach (StoreItem storeItem in items)
			{
				text = text + JsonUtility.ToJson(storeItem) + ";";
			}
			Debug.LogError(text);
			File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", text);
		}

		// Token: 0x06006230 RID: 25136 RVA: 0x001F9ECC File Offset: 0x001F80CC
		public static void ConvertCosmeticItemToSToreItem(CosmeticsController.CosmeticItem cosmeticItem, ref StoreItem storeItem)
		{
			storeItem.itemName = cosmeticItem.itemName;
			storeItem.itemCategory = (int)cosmeticItem.itemCategory;
			storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
			storeItem.displayName = cosmeticItem.displayName;
			storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
			storeItem.bundledItems = cosmeticItem.bundledItems;
			storeItem.canTryOn = cosmeticItem.canTryOn;
			storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
			storeItem.AssetBundleName = "";
			storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
			storeItem.MeshResourceName = cosmeticItem.meshResourceString;
			storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
			storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
		}

		// Token: 0x040070D0 RID: 28880
		public string itemName = "";

		// Token: 0x040070D1 RID: 28881
		public int itemCategory;

		// Token: 0x040070D2 RID: 28882
		public string itemPictureResourceString = "";

		// Token: 0x040070D3 RID: 28883
		public string displayName = "";

		// Token: 0x040070D4 RID: 28884
		public string overrideDisplayName = "";

		// Token: 0x040070D5 RID: 28885
		public string[] bundledItems = new string[0];

		// Token: 0x040070D6 RID: 28886
		public bool canTryOn;

		// Token: 0x040070D7 RID: 28887
		public bool bothHandsHoldable;

		// Token: 0x040070D8 RID: 28888
		public string AssetBundleName = "";

		// Token: 0x040070D9 RID: 28889
		public bool bUsesMeshAtlas;

		// Token: 0x040070DA RID: 28890
		public string MeshAtlasResourceName = "";

		// Token: 0x040070DB RID: 28891
		public string MeshResourceName = "";

		// Token: 0x040070DC RID: 28892
		public string MaterialResrouceName = "";

		// Token: 0x040070DD RID: 28893
		public Vector3 translationOffset = Vector3.zero;

		// Token: 0x040070DE RID: 28894
		public Vector3 rotationOffset = Vector3.zero;

		// Token: 0x040070DF RID: 28895
		public Vector3 scale = Vector3.one;
	}
}
