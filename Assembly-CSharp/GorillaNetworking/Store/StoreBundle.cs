using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F3E RID: 3902
	[Serializable]
	public class StoreBundle
	{
		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x060061B3 RID: 25011 RVA: 0x001F7095 File Offset: 0x001F5295
		public string playfabBundleID
		{
			get
			{
				return this._storeBundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x060061B4 RID: 25012 RVA: 0x001F70A2 File Offset: 0x001F52A2
		public string bundleSKU
		{
			get
			{
				return this._storeBundleDataReference.bundleSKU;
			}
		}

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x060061B5 RID: 25013 RVA: 0x001F70AF File Offset: 0x001F52AF
		public Sprite bundleImage
		{
			get
			{
				return this._storeBundleDataReference.bundleImage;
			}
		}

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x060061B6 RID: 25014 RVA: 0x001F70BC File Offset: 0x001F52BC
		public NexusCreatorCode nexusCreatorCode
		{
			get
			{
				return this._storeBundleDataReference.creatorCode;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x060061B7 RID: 25015 RVA: 0x001F70C9 File Offset: 0x001F52C9
		public string price
		{
			get
			{
				return this._price;
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x060061B8 RID: 25016 RVA: 0x001F70D4 File Offset: 0x001F52D4
		public string bundleName
		{
			get
			{
				if (this._bundleName.IsNullOrEmpty())
				{
					int num = CosmeticsController.instance.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.playfabBundleID == x.itemName);
					if (num > -1)
					{
						if (!CosmeticsController.instance.allCosmetics[num].overrideDisplayName.IsNullOrEmpty())
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].overrideDisplayName;
						}
						else
						{
							this._bundleName = CosmeticsController.instance.allCosmetics[num].displayName;
						}
					}
					else
					{
						this._bundleName = "NULL_BUNDLE_NAME";
					}
				}
				return this._bundleName;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x060061B9 RID: 25017 RVA: 0x001F7180 File Offset: 0x001F5380
		public bool HasPrice
		{
			get
			{
				return !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
			}
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x060061BA RID: 25018 RVA: 0x001F71A1 File Offset: 0x001F53A1
		public string bundleDescriptionText
		{
			get
			{
				return this._storeBundleDataReference.bundleDescriptionText;
			}
		}

		// Token: 0x060061BB RID: 25019 RVA: 0x001F71B0 File Offset: 0x001F53B0
		public StoreBundle()
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
		}

		// Token: 0x060061BC RID: 25020 RVA: 0x001F7204 File Offset: 0x001F5404
		public StoreBundle(StoreBundleData data)
		{
			this.isOwned = false;
			this.bundleStands = new List<BundleStand>();
			this._storeBundleDataReference = data;
		}

		// Token: 0x060061BD RID: 25021 RVA: 0x001F725C File Offset: 0x001F545C
		public void InitializebundleStands()
		{
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
				bundleStand.InitializeEventListeners();
			}
		}

		// Token: 0x060061BE RID: 25022 RVA: 0x001F72B8 File Offset: 0x001F54B8
		public void TryUpdatePrice(uint bundlePrice)
		{
			this.TryUpdatePrice((bundlePrice / 100m).ToString());
		}

		// Token: 0x060061BF RID: 25023 RVA: 0x001F72E8 File Offset: 0x001F54E8
		public void TryUpdatePrice(string bundlePrice = null)
		{
			if (!string.IsNullOrEmpty(bundlePrice))
			{
				decimal num;
				this._price = (decimal.TryParse(bundlePrice, ref num) ? (StoreBundle.defaultCurrencySymbol + bundlePrice) : bundlePrice);
			}
			this.UpdatePurchaseButtonText();
		}

		// Token: 0x060061C0 RID: 25024 RVA: 0x001F7324 File Offset: 0x001F5524
		public void UpdatePurchaseButtonText()
		{
			this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, this.bundleName, this.price);
			foreach (BundleStand bundleStand in this.bundleStands)
			{
				bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
			}
		}

		// Token: 0x060061C1 RID: 25025 RVA: 0x001F7398 File Offset: 0x001F5598
		public void ValidateBundleData()
		{
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				foreach (BundleStand bundleStand in this.bundleStands)
				{
					if (bundleStand == null)
					{
						Debug.LogError("BundleStand is null");
					}
					else if (bundleStand._bundleDataReference != null)
					{
						this._storeBundleDataReference = bundleStand._bundleDataReference;
						Debug.LogError("BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
					}
				}
			}
			if (this._storeBundleDataReference == null)
			{
				Debug.LogError("StoreBundleData is null");
				return;
			}
			if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
			{
				Debug.LogError("playfabBundleID is null");
			}
			if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
			{
				Debug.LogError("bundleSKU is null");
			}
			if (this._storeBundleDataReference.bundleImage == null)
			{
				Debug.LogError("bundleImage is null");
			}
			if (this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
			{
				Debug.LogError("bundleDescriptionText is null");
			}
		}

		// Token: 0x0400706F RID: 28783
		private static readonly string defaultPrice = "$--.--";

		// Token: 0x04007070 RID: 28784
		private static readonly string defaultCurrencySymbol = "$";

		// Token: 0x04007071 RID: 28785
		[NonSerialized]
		public string purchaseButtonStringFormat = "THE {0}\n{1}";

		// Token: 0x04007072 RID: 28786
		[SerializeField]
		public List<BundleStand> bundleStands;

		// Token: 0x04007073 RID: 28787
		public bool isOwned;

		// Token: 0x04007074 RID: 28788
		private string _price = StoreBundle.defaultPrice;

		// Token: 0x04007075 RID: 28789
		private string _bundleName = "";

		// Token: 0x04007076 RID: 28790
		public string purchaseButtonText = "";

		// Token: 0x04007077 RID: 28791
		[FormerlySerializedAs("storeBundleDataReference")]
		[SerializeField]
		[ReadOnly]
		private StoreBundleData _storeBundleDataReference;
	}
}
