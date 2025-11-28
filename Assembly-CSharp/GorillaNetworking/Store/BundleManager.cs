using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmetics;
using GorillaExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F36 RID: 3894
	public class BundleManager : MonoBehaviour
	{
		// Token: 0x06006179 RID: 24953 RVA: 0x001F6173 File Offset: 0x001F4373
		private IEnumerable GetStoreBundles()
		{
			List<StoreBundleData> list = new List<StoreBundleData>();
			list.Add(this.nullBundleData);
			list.AddRange(this._bundleScriptableObjects);
			return list;
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x001F6192 File Offset: 0x001F4392
		public void Awake()
		{
			if (BundleManager.instance == null)
			{
				BundleManager.instance = this;
				return;
			}
			if (BundleManager.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x001F61C7 File Offset: 0x001F43C7
		private void Start()
		{
			this.GenerateBundleDictionaries();
			this.Initialize();
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x001F61D8 File Offset: 0x001F43D8
		private void Initialize()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.InitializebundleStands();
			}
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x001F6228 File Offset: 0x001F4428
		private void ValidateBundleData()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				storeBundle.ValidateBundleData();
			}
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x001F6278 File Offset: 0x001F4478
		private void SpawnBundleStands()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(0);
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
			for (int j = 0; j < this.BundleStands.Count; j++)
			{
				if (this.BundleStands[j].spawnLocation == null)
				{
					Debug.LogError("No spawn location set for Bundle Stand " + j.ToString());
				}
				else if (this.BundleStands[j].bundleStand == null)
				{
					Debug.LogError("No Bundle Stand set for Bundle Stand " + j.ToString());
				}
			}
			this.GenerateAllStoreBundleReferences();
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton1))
			{
				this.tryOnBundleButton1 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton2))
			{
				this.tryOnBundleButton2 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton3))
			{
				this.tryOnBundleButton3 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton4))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
			if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton5))
			{
				this.tryOnBundleButton4 = this.nullBundleData;
			}
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x001F648C File Offset: 0x001F468C
		public void ClearEverything()
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					if (bundleStand != null)
					{
						Object.DestroyImmediate(bundleStand.gameObject);
					}
				}
			}
			this._spawnedBundleStands.Clear();
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			this._storeBundles.Clear();
			this._bundleScriptableObjects.Clear();
			this.tryOnBundleButton1 = this.nullBundleData;
			this.tryOnBundleButton2 = this.nullBundleData;
			this.tryOnBundleButton3 = this.nullBundleData;
			this.tryOnBundleButton4 = this.nullBundleData;
			this.tryOnBundleButton5 = this.nullBundleData;
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(0);
			for (int i = 0; i < array.Length; i++)
			{
				Object.DestroyImmediate(array[i].gameObject);
			}
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x00002789 File Offset: 0x00000989
		public void GenerateAllStoreBundleReferences()
		{
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x001F65C0 File Offset: 0x001F47C0
		private void AddNewBundleStand(BundleStand bundleStand)
		{
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				if (storeBundle.playfabBundleID == bundleStand._bundleDataReference.playfabBundleID)
				{
					storeBundle.bundleStands.Add(bundleStand);
					return;
				}
			}
			StoreBundle storeBundle2 = new StoreBundle(bundleStand._bundleDataReference);
			storeBundle2.bundleStands.Add(bundleStand);
			this._storeBundles.Add(storeBundle2);
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x001F6658 File Offset: 0x001F4858
		public void GenerateBundleDictionaries()
		{
			this.storeBundlesById.Clear();
			this.storeBundlesBySKU.Clear();
			foreach (StoreBundle storeBundle in this._storeBundles)
			{
				this.storeBundlesById.Add(storeBundle.playfabBundleID, storeBundle);
				this.storeBundlesBySKU.Add(storeBundle.bundleSKU, storeBundle);
			}
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x001F66E0 File Offset: 0x001F48E0
		public void BundlePurchaseButtonPressed(string playFabItemName, ICreatorCodeProvider ccp)
		{
			CosmeticsController.instance.PurchaseBundle(this.storeBundlesById[playFabItemName], ccp);
		}

		// Token: 0x06006184 RID: 24964 RVA: 0x001F66FC File Offset: 0x001F48FC
		public void FixBundles()
		{
			this._storeBundles.Clear();
			for (int i = this._spawnedBundleStands.Count - 1; i >= 0; i--)
			{
				if (this._spawnedBundleStands[i].bundleStand == null)
				{
					this._spawnedBundleStands.RemoveAt(i);
				}
			}
			BundleStand[] array = Object.FindObjectsByType<BundleStand>(0);
			for (int j = 0; j < array.Length; j++)
			{
				BundleStand bundle = array[j];
				if (Enumerable.Any<SpawnedBundle>(this._spawnedBundleStands, (SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3)))
				{
					SpawnedBundle spawnedBundle = Enumerable.First<SpawnedBundle>(this._spawnedBundleStands, (SpawnedBundle x) => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3));
					if (spawnedBundle != null && spawnedBundle.bundleStand != bundle)
					{
						Object.DestroyImmediate(spawnedBundle.bundleStand.gameObject);
						spawnedBundle.bundleStand = bundle;
					}
				}
				else
				{
					this._spawnedBundleStands.Add(new SpawnedBundle
					{
						spawnLocationPath = bundle.transform.parent.gameObject.GetPath(3),
						bundleStand = bundle
					});
				}
			}
			this.GenerateAllStoreBundleReferences();
		}

		// Token: 0x06006185 RID: 24965 RVA: 0x001F6828 File Offset: 0x001F4A28
		public StoreBundleData[] GetTryOnButtons()
		{
			return new StoreBundleData[]
			{
				this.tryOnBundleButton1,
				this.tryOnBundleButton2,
				this.tryOnBundleButton3,
				this.tryOnBundleButton4,
				this.tryOnBundleButton5
			};
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x001F6860 File Offset: 0x001F4A60
		public void NotifyBundleOfErrorByPlayFabID(string ItemId)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesById.TryGetValue(ItemId, ref storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x001F68C0 File Offset: 0x001F4AC0
		public void NotifyBundleOfErrorBySKU(string ItemSKU)
		{
			StoreBundle storeBundle;
			if (this.storeBundlesBySKU.TryGetValue(ItemSKU, ref storeBundle))
			{
				foreach (BundleStand bundleStand in storeBundle.bundleStands)
				{
					bundleStand.ErrorHappened();
				}
			}
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x001F6920 File Offset: 0x001F4B20
		public void MarkBundleOwnedByPlayFabID(string ItemId)
		{
			if (this.storeBundlesById.ContainsKey(ItemId))
			{
				this.storeBundlesById[ItemId].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesById[ItemId].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x001F699C File Offset: 0x001F4B9C
		public void MarkBundleOwnedBySKU(string SKU)
		{
			if (this.storeBundlesBySKU.ContainsKey(SKU))
			{
				this.storeBundlesBySKU[SKU].isOwned = true;
				foreach (BundleStand bundleStand in this.storeBundlesBySKU[SKU].bundleStands)
				{
					bundleStand.NotifyAlreadyOwn();
				}
			}
		}

		// Token: 0x0600618A RID: 24970 RVA: 0x001F6A18 File Offset: 0x001F4C18
		public void CheckIfBundlesOwned()
		{
			foreach (StoreBundle storeBundle in this.storeBundlesById.Values)
			{
				if (storeBundle.isOwned)
				{
					foreach (BundleStand bundleStand in storeBundle.bundleStands)
					{
						bundleStand.NotifyAlreadyOwn();
					}
				}
			}
		}

		// Token: 0x0600618B RID: 24971 RVA: 0x001F6AB0 File Offset: 0x001F4CB0
		public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
		{
			if (this._tryOnBundlesStand.IsNotNull())
			{
				this._tryOnBundlesStand.PressTryOnBundleButton(pressedTryOnBundleButton, isLeftHand);
			}
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x001F6ACC File Offset: 0x001F4CCC
		public void PressPurchaseTryOnBundleButton()
		{
			this._tryOnBundlesStand.PurchaseButtonPressed();
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x001F6AD9 File Offset: 0x001F4CD9
		public void UpdateBundlePrice(string productSku, string productFormattedPrice)
		{
			if (this.storeBundlesBySKU.ContainsKey(productSku))
			{
				this.storeBundlesBySKU[productSku].TryUpdatePrice(productFormattedPrice);
			}
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x001F6AFC File Offset: 0x001F4CFC
		public void CheckForNoPriceBundlesAndDefaultPrice()
		{
			foreach (KeyValuePair<string, StoreBundle> keyValuePair in this.storeBundlesBySKU)
			{
				string text;
				StoreBundle storeBundle;
				keyValuePair.Deconstruct(ref text, ref storeBundle);
				StoreBundle storeBundle2 = storeBundle;
				if (!storeBundle2.HasPrice)
				{
					storeBundle2.TryUpdatePrice(null);
				}
			}
		}

		// Token: 0x04007048 RID: 28744
		public static volatile BundleManager instance;

		// Token: 0x04007049 RID: 28745
		[FormerlySerializedAs("_TryOnBundlesStand")]
		public TryOnBundlesStand _tryOnBundlesStand;

		// Token: 0x0400704A RID: 28746
		[SerializeField]
		private StoreBundleData nullBundleData;

		// Token: 0x0400704B RID: 28747
		private List<StoreBundleData> _bundleScriptableObjects = new List<StoreBundleData>();

		// Token: 0x0400704C RID: 28748
		[SerializeField]
		private List<StoreBundle> _storeBundles = new List<StoreBundle>();

		// Token: 0x0400704D RID: 28749
		[FormerlySerializedAs("_SpawnedBundleStands")]
		[SerializeField]
		private List<SpawnedBundle> _spawnedBundleStands = new List<SpawnedBundle>();

		// Token: 0x0400704E RID: 28750
		public Dictionary<string, StoreBundle> storeBundlesById = new Dictionary<string, StoreBundle>();

		// Token: 0x0400704F RID: 28751
		public Dictionary<string, StoreBundle> storeBundlesBySKU = new Dictionary<string, StoreBundle>();

		// Token: 0x04007050 RID: 28752
		[Header("Enable Advanced Search window in your settings to easily see all bundle prefabs")]
		[SerializeField]
		private List<BundleManager.BundleStandSpawn> BundleStands = new List<BundleManager.BundleStandSpawn>();

		// Token: 0x04007051 RID: 28753
		[SerializeField]
		private StoreBundleData tryOnBundleButton1;

		// Token: 0x04007052 RID: 28754
		[SerializeField]
		private StoreBundleData tryOnBundleButton2;

		// Token: 0x04007053 RID: 28755
		[SerializeField]
		private StoreBundleData tryOnBundleButton3;

		// Token: 0x04007054 RID: 28756
		[SerializeField]
		private StoreBundleData tryOnBundleButton4;

		// Token: 0x04007055 RID: 28757
		[SerializeField]
		private StoreBundleData tryOnBundleButton5;

		// Token: 0x02000F37 RID: 3895
		[Serializable]
		public class BundleStandSpawn
		{
			// Token: 0x06006190 RID: 24976 RVA: 0x001F6BBD File Offset: 0x001F4DBD
			private static IEnumerable GetEndCapSpawnPoints()
			{
				return Enumerable.Select<EndCapSpawnPoint, ValueDropdownItem>(Object.FindObjectsByType<EndCapSpawnPoint>(0), (EndCapSpawnPoint x) => new ValueDropdownItem(string.Concat(new string[]
				{
					x.transform.parent.parent.name,
					"/",
					x.transform.parent.name,
					"/",
					x.name
				}), x));
			}

			// Token: 0x04007056 RID: 28758
			public EndCapSpawnPoint spawnLocation;

			// Token: 0x04007057 RID: 28759
			public BundleStand bundleStand;
		}
	}
}
