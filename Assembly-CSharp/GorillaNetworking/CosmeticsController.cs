using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CosmeticRoom;
using Cosmetics;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x02000EBD RID: 3773
	public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
	{
		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x06005E0B RID: 24075 RVA: 0x001E2224 File Offset: 0x001E0424
		// (set) Token: 0x06005E0C RID: 24076 RVA: 0x001E222C File Offset: 0x001E042C
		public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x06005E0D RID: 24077 RVA: 0x001E2235 File Offset: 0x001E0435
		// (set) Token: 0x06005E0E RID: 24078 RVA: 0x001E223D File Offset: 0x001E043D
		public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x06005E0F RID: 24079 RVA: 0x001E2246 File Offset: 0x001E0446
		// (set) Token: 0x06005E10 RID: 24080 RVA: 0x001E224E File Offset: 0x001E044E
		public bool v2_isGetCosmeticsPlayCatalogDataWaitingForCallback { get; private set; }

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x06005E11 RID: 24081 RVA: 0x001E2257 File Offset: 0x001E0457
		// (set) Token: 0x06005E12 RID: 24082 RVA: 0x001E225F File Offset: 0x001E045F
		public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

		// Token: 0x06005E13 RID: 24083 RVA: 0x001E2268 File Offset: 0x001E0468
		private void V2Awake()
		{
			this._allCosmetics = null;
			base.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
		}

		// Token: 0x06005E14 RID: 24084 RVA: 0x001E227E File Offset: 0x001E047E
		private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
		{
			while (!PlayFabAuthenticator.instance)
			{
				yield return new WaitForSeconds(1f);
			}
			float[] retryWaitTimes = new float[]
			{
				1f,
				2f,
				4f,
				4f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				10f,
				30f
			};
			int retryCount = 0;
			AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
			for (;;)
			{
				Debug.Log(string.Format("Attempting to load runtime key \"{0}\" ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + string.Format("(Attempt: {0})", retryCount + 1));
				newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
				yield return newSysAllCosmeticsAsyncOp;
				if (ApplicationQuittingState.IsQuitting)
				{
					break;
				}
				if (!newSysAllCosmeticsAsyncOp.IsValid())
				{
					Debug.LogError("`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
				}
				if (newSysAllCosmeticsAsyncOp.Status == 1)
				{
					goto Block_4;
				}
				Debug.LogError(string.Format("Failed to load \"{0}\". ", this.v2_allCosmeticsInfoAssetRef.RuntimeKey) + "Error: " + newSysAllCosmeticsAsyncOp.OperationException.Message);
				float num = retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)];
				yield return new WaitForSecondsRealtime(num);
				int num2 = retryCount;
				retryCount = num2 + 1;
				newSysAllCosmeticsAsyncOp = default(AsyncOperationHandle<AllCosmeticsArraySO>);
			}
			yield break;
			Block_4:
			this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
			yield break;
		}

		// Token: 0x06005E15 RID: 24085 RVA: 0x001E2290 File Offset: 0x001E0490
		private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
		{
			this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
			for (int i = 0; i < allCosmeticsSO.sturdyAssetRefs.Length; i++)
			{
				this.v2_allCosmetics[i] = allCosmeticsSO.sturdyAssetRefs[i].obj.info;
			}
			this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
			for (int j = 0; j < this.v2_allCosmetics.Length; j++)
			{
				CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[j];
				string playFabID = cosmeticInfoV.playFabID;
				this._allCosmeticsDictV2[playFabID] = cosmeticInfoV;
				CosmeticsController.CosmeticItem cosmeticItem = new CosmeticsController.CosmeticItem
				{
					itemName = playFabID,
					itemCategory = cosmeticInfoV.category,
					isHoldable = cosmeticInfoV.hasHoldableParts,
					displayName = playFabID,
					itemPicture = cosmeticInfoV.icon,
					overrideDisplayName = cosmeticInfoV.displayName,
					bothHandsHoldable = cosmeticInfoV.usesBothHandSlots,
					isNullItem = false
				};
				this._allCosmetics.Add(cosmeticItem);
			}
			this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
			Action v2_allCosmeticsInfoAssetRef_OnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
			if (v2_allCosmeticsInfoAssetRef_OnPostLoad == null)
			{
				return;
			}
			v2_allCosmeticsInfoAssetRef_OnPostLoad.Invoke();
		}

		// Token: 0x06005E16 RID: 24086 RVA: 0x001E23C1 File Offset: 0x001E05C1
		public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
		{
			return this._allCosmeticsDictV2.TryGetValue(playFabId, ref cosmeticInfo);
		}

		// Token: 0x06005E17 RID: 24087 RVA: 0x001E23D0 File Offset: 0x001E05D0
		private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
		{
			if (cosmetic.itemName == cosmetic.displayName)
			{
				return;
			}
			cosmetic.overrideDisplayName = cosmetic.displayName;
			cosmetic.displayName = cosmetic.itemName;
		}

		// Token: 0x06005E18 RID: 24088 RVA: 0x001E2400 File Offset: 0x001E0600
		internal void InitializeCosmeticStands()
		{
			foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
			{
				if (cosmeticStand != null)
				{
					cosmeticStand.InitializeCosmetic();
				}
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x06005E19 RID: 24089 RVA: 0x001E2435 File Offset: 0x001E0635
		// (set) Token: 0x06005E1A RID: 24090 RVA: 0x001E243C File Offset: 0x001E063C
		public static bool hasInstance { get; private set; }

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x06005E1B RID: 24091 RVA: 0x001E2444 File Offset: 0x001E0644
		// (set) Token: 0x06005E1C RID: 24092 RVA: 0x001E244C File Offset: 0x001E064C
		public List<CosmeticsController.CosmeticItem> allCosmetics
		{
			get
			{
				return this._allCosmetics;
			}
			set
			{
				this._allCosmetics = value;
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x06005E1D RID: 24093 RVA: 0x001E2455 File Offset: 0x001E0655
		// (set) Token: 0x06005E1E RID: 24094 RVA: 0x001E245D File Offset: 0x001E065D
		public bool allCosmeticsDict_isInitialized { get; private set; }

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x06005E1F RID: 24095 RVA: 0x001E2466 File Offset: 0x001E0666
		public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
		{
			get
			{
				return this._allCosmeticsDict;
			}
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x06005E20 RID: 24096 RVA: 0x001E246E File Offset: 0x001E066E
		// (set) Token: 0x06005E21 RID: 24097 RVA: 0x001E2476 File Offset: 0x001E0676
		public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x06005E22 RID: 24098 RVA: 0x001E247F File Offset: 0x001E067F
		public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
		{
			get
			{
				return this._allCosmeticsItemIDsfromDisplayNamesDict;
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x06005E23 RID: 24099 RVA: 0x001E2487 File Offset: 0x001E0687
		public CosmeticAnchorAntiIntersectOffsets defaultClipOffsets
		{
			get
			{
				return CosmeticAnchorAntiIntersectOffsets.Identity;
			}
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x06005E24 RID: 24100 RVA: 0x001E248E File Offset: 0x001E068E
		// (set) Token: 0x06005E25 RID: 24101 RVA: 0x001E2496 File Offset: 0x001E0696
		public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

		// Token: 0x06005E26 RID: 24102 RVA: 0x001E249F File Offset: 0x001E069F
		public void AddWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Add(instance);
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06005E27 RID: 24103 RVA: 0x001E24BA File Offset: 0x001E06BA
		public void RemoveWardrobeInstance(WardrobeInstance instance)
		{
			this.wardrobes.Remove(instance);
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06005E28 RID: 24104 RVA: 0x001E24C9 File Offset: 0x001E06C9
		public int CurrencyBalance
		{
			get
			{
				return this.currencyBalance;
			}
		}

		// Token: 0x06005E29 RID: 24105 RVA: 0x001E24D4 File Offset: 0x001E06D4
		public void Awake()
		{
			if (CosmeticsController.instance == null)
			{
				CosmeticsController.instance = this;
				CosmeticsController.hasInstance = true;
			}
			else if (CosmeticsController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.V2Awake();
			if (base.gameObject.activeSelf)
			{
				this.catalog = "DLC";
				this.currencyName = "SR";
				this.nullItem = default(CosmeticsController.CosmeticItem);
				this.nullItem.itemName = "null";
				this.nullItem.displayName = "NOTHING";
				this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
				this.nullItem.itemPictureResourceString = "";
				this.nullItem.overrideDisplayName = "NOTHING";
				this.nullItem.meshAtlasResourceString = "";
				this.nullItem.meshResourceString = "";
				this.nullItem.materialResourceString = "";
				this.nullItem.isNullItem = true;
				this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
				this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
				for (int i = 0; i < 16; i++)
				{
					this.tryOnSet.items[i] = this.nullItem;
					this.tempUnlockedSet.items[i] = this.nullItem;
					this.activeMergedSet.items[i] = this.nullItem;
				}
				this.cosmeticsPages[0] = 0;
				this.cosmeticsPages[1] = 0;
				this.cosmeticsPages[2] = 0;
				this.cosmeticsPages[3] = 0;
				this.cosmeticsPages[4] = 0;
				this.cosmeticsPages[5] = 0;
				this.cosmeticsPages[6] = 0;
				this.cosmeticsPages[7] = 0;
				this.cosmeticsPages[8] = 0;
				this.cosmeticsPages[9] = 0;
				this.cosmeticsPages[10] = 0;
				this.itemLists[0] = this.unlockedHats;
				this.itemLists[1] = this.unlockedFaces;
				this.itemLists[2] = this.unlockedBadges;
				this.itemLists[3] = this.unlockedPaws;
				this.itemLists[4] = this.unlockedFurs;
				this.itemLists[5] = this.unlockedShirts;
				this.itemLists[6] = this.unlockedPants;
				this.itemLists[7] = this.unlockedArms;
				this.itemLists[8] = this.unlockedBacks;
				this.itemLists[9] = this.unlockedChests;
				this.itemLists[10] = this.unlockedTagFX;
				this.updateCosmeticsRetries = 0;
				this.maxUpdateCosmeticsRetries = 5;
				this.inventoryStringList.Clear();
				this.inventoryStringList.Add("Inventory");
				base.StartCoroutine(this.CheckCanGetDaily());
			}
			CreatorCodes.Initialize();
		}

		// Token: 0x06005E2A RID: 24106 RVA: 0x001E27B0 File Offset: 0x001E09B0
		public void Start()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("BundleData", delegate(string data)
			{
				this.bundleList.FromJson(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting bundle data: {0}", e));
			}, false);
			this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
		}

		// Token: 0x06005E2B RID: 24107 RVA: 0x001E280D File Offset: 0x001E0A0D
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (SteamManager.Initialized && this._steamMicroTransactionAuthorizationResponse == null)
			{
				this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
			}
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x001E283C File Offset: 0x001E0A3C
		public void OnDisable()
		{
			Callback<MicroTxnAuthorizationResponse_t> steamMicroTransactionAuthorizationResponse = this._steamMicroTransactionAuthorizationResponse;
			if (steamMicroTransactionAuthorizationResponse != null)
			{
				steamMicroTransactionAuthorizationResponse.Unregister();
			}
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x00002789 File Offset: 0x00000989
		public void SliceUpdate()
		{
		}

		// Token: 0x06005E2E RID: 24110 RVA: 0x001E2858 File Offset: 0x001E0A58
		public static bool CompareCategoryToSavedCosmeticSlots(CosmeticsController.CosmeticCategory category, CosmeticsController.CosmeticSlots slot)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return slot == CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge == slot;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face == slot;
			case CosmeticsController.CosmeticCategory.Paw:
				return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest == slot;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur == slot;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt == slot;
			case CosmeticsController.CosmeticCategory.Back:
				return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
			case CosmeticsController.CosmeticCategory.Arms:
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants == slot;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect == slot;
			default:
				return false;
			}
		}

		// Token: 0x06005E2F RID: 24111 RVA: 0x001E28EC File Offset: 0x001E0AEC
		public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return CosmeticsController.CosmeticSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return CosmeticsController.CosmeticSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return CosmeticsController.CosmeticSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				return CosmeticsController.CosmeticSlots.HandRight;
			case CosmeticsController.CosmeticCategory.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			case CosmeticsController.CosmeticCategory.Fur:
				return CosmeticsController.CosmeticSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return CosmeticsController.CosmeticSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Back:
				return CosmeticsController.CosmeticSlots.Back;
			case CosmeticsController.CosmeticCategory.Arms:
				return CosmeticsController.CosmeticSlots.Arms;
			case CosmeticsController.CosmeticCategory.Pants:
				return CosmeticsController.CosmeticSlots.Pants;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return CosmeticsController.CosmeticSlots.TagEffect;
			default:
				return CosmeticsController.CosmeticSlots.Count;
			}
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x001E294E File Offset: 0x001E0B4E
		private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(BodyDockPositions.DropPositions pos)
		{
			switch (pos)
			{
			case BodyDockPositions.DropPositions.LeftArm:
				return CosmeticsController.CosmeticSlots.ArmLeft;
			case BodyDockPositions.DropPositions.RightArm:
				return CosmeticsController.CosmeticSlots.ArmRight;
			case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
				break;
			case BodyDockPositions.DropPositions.Chest:
				return CosmeticsController.CosmeticSlots.Chest;
			default:
				if (pos == BodyDockPositions.DropPositions.LeftBack)
				{
					return CosmeticsController.CosmeticSlots.BackLeft;
				}
				if (pos == BodyDockPositions.DropPositions.RightBack)
				{
					return CosmeticsController.CosmeticSlots.BackRight;
				}
				break;
			}
			return CosmeticsController.CosmeticSlots.Count;
		}

		// Token: 0x06005E31 RID: 24113 RVA: 0x001E2980 File Offset: 0x001E0B80
		private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(CosmeticsController.CosmeticSlots slot)
		{
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.ArmLeft:
				return BodyDockPositions.DropPositions.LeftArm;
			case CosmeticsController.CosmeticSlots.ArmRight:
				return BodyDockPositions.DropPositions.RightArm;
			case CosmeticsController.CosmeticSlots.BackLeft:
				return BodyDockPositions.DropPositions.LeftBack;
			case CosmeticsController.CosmeticSlots.BackRight:
				return BodyDockPositions.DropPositions.RightBack;
			case CosmeticsController.CosmeticSlots.Chest:
				return BodyDockPositions.DropPositions.Chest;
			}
			return BodyDockPositions.DropPositions.None;
		}

		// Token: 0x06005E32 RID: 24114 RVA: 0x001E29B4 File Offset: 0x001E0BB4
		public void AddItemCheckout(ItemCheckout newItemCheckout)
		{
			if (this.itemCheckouts.Contains(newItemCheckout))
			{
				return;
			}
			this.itemCheckouts.Add(newItemCheckout);
			this.UpdateShoppingCart();
			this.FormattedPurchaseText(this.finalLine, this.leftCheckoutPurchaseButtonString, this.rightCheckoutPurchaseButtonString, this.leftCheckoutPurchaseButtonOn, this.rightCheckoutPurchaseButtonOn);
			if (!this.itemToBuy.isNullItem)
			{
				this.RefreshItemToBuyPreview();
			}
		}

		// Token: 0x06005E33 RID: 24115 RVA: 0x001E2A19 File Offset: 0x001E0C19
		public void RemoveItemCheckout(ItemCheckout checkoutToRemove)
		{
			this.itemCheckouts.Remove(checkoutToRemove);
		}

		// Token: 0x06005E34 RID: 24116 RVA: 0x001E2A28 File Offset: 0x001E0C28
		public void AddFittingRoom(FittingRoom newFittingRoom)
		{
			if (this.fittingRooms.Contains(newFittingRoom))
			{
				return;
			}
			this.fittingRooms.Add(newFittingRoom);
			this.UpdateShoppingCart();
		}

		// Token: 0x06005E35 RID: 24117 RVA: 0x001E2A4B File Offset: 0x001E0C4B
		public void RemoveFittingRoom(FittingRoom fittingRoomToRemove)
		{
			this.fittingRooms.Remove(fittingRoomToRemove);
		}

		// Token: 0x06005E36 RID: 24118 RVA: 0x001E2A5A File Offset: 0x001E0C5A
		private void SaveItemPreference(CosmeticsController.CosmeticSlots slot, int slotIdx, CosmeticsController.CosmeticItem newItem)
		{
			PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
			PlayerPrefs.Save();
		}

		// Token: 0x06005E37 RID: 24119 RVA: 0x001E2A74 File Offset: 0x001E0C74
		public void SaveCurrentItemPreferences()
		{
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
				this.SaveItemPreference(slot, i, this.currentWornSet.items[i]);
			}
		}

		// Token: 0x06005E38 RID: 24120 RVA: 0x001E2AAC File Offset: 0x001E0CAC
		private void ApplyCosmeticToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, int slotIdx, CosmeticsController.CosmeticSlots slot, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			CosmeticsController.CosmeticItem cosmeticItem = (set.items[slotIdx].itemName == newItem.itemName) ? this.nullItem : newItem;
			set.items[slotIdx] = cosmeticItem;
			if (applyToPlayerPrefs)
			{
				this.SaveItemPreference(slot, slotIdx, cosmeticItem);
			}
			appliedSlots.Add(slot);
		}

		// Token: 0x06005E39 RID: 24121 RVA: 0x001E2B08 File Offset: 0x001E0D08
		private void PrivApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> appliedSlots)
		{
			if (newItem.isNullItem)
			{
				return;
			}
			if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
			{
				BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
				foreach (BodyDockPositions.DropPositions pos in dockingResult.positionsDisabled)
				{
					CosmeticsController.CosmeticSlots cosmeticSlots = this.DropPositionToCosmeticSlot(pos);
					if (cosmeticSlots != CosmeticsController.CosmeticSlots.Count)
					{
						int num = (int)cosmeticSlots;
						set.items[num] = this.nullItem;
						if (applyToPlayerPrefs)
						{
							this.SaveItemPreference(cosmeticSlots, num, this.nullItem);
						}
					}
				}
				using (List<BodyDockPositions.DropPositions>.Enumerator enumerator = dockingResult.dockedPosition.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BodyDockPositions.DropPositions dropPositions = enumerator.Current;
						if (dropPositions != BodyDockPositions.DropPositions.None)
						{
							CosmeticsController.CosmeticSlots cosmeticSlots2 = this.DropPositionToCosmeticSlot(dropPositions);
							int num2 = (int)cosmeticSlots2;
							set.items[num2] = newItem;
							if (applyToPlayerPrefs)
							{
								this.SaveItemPreference(cosmeticSlots2, num2, newItem);
							}
							appliedSlots.Add(cosmeticSlots2);
						}
					}
					return;
				}
			}
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots3 = isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight;
				int slotIdx = (int)cosmeticSlots3;
				this.ApplyCosmeticToSet(set, newItem, slotIdx, cosmeticSlots3, applyToPlayerPrefs, appliedSlots);
				CosmeticsController.CosmeticSlots cosmeticSlots4 = CosmeticsController.CosmeticSet.OppositeSlot(cosmeticSlots3);
				int num3 = (int)cosmeticSlots4;
				if (newItem.bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
				if (set.items[num3].itemName == newItem.itemName)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
				}
				if (set.items[num3].bothHandsHoldable)
				{
					this.ApplyCosmeticToSet(set, this.nullItem, num3, cosmeticSlots4, applyToPlayerPrefs, appliedSlots);
					return;
				}
			}
			else
			{
				CosmeticsController.CosmeticSlots cosmeticSlots5 = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
				int slotIdx2 = (int)cosmeticSlots5;
				this.ApplyCosmeticToSet(set, newItem, slotIdx2, cosmeticSlots5, applyToPlayerPrefs, appliedSlots);
			}
		}

		// Token: 0x06005E3A RID: 24122 RVA: 0x001E2D0C File Offset: 0x001E0F0C
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs)
		{
			this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
		}

		// Token: 0x06005E3B RID: 24123 RVA: 0x001E2D20 File Offset: 0x001E0F20
		public void ApplyCosmeticItemToSet(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem newItem, bool isLeftHand, bool applyToPlayerPrefs, List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
		{
			outAppliedSlotsList.Clear();
			if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
			{
				bool flag = false;
				Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
				foreach (string itemID in newItem.bundledItems)
				{
					CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
					if (this.AnyMatch(set, itemFromDict))
					{
						flag = true;
						dictionary.Add(itemFromDict, true);
					}
					else
					{
						dictionary.Add(itemFromDict, false);
					}
				}
				using (Dictionary<CosmeticsController.CosmeticItem, bool>.Enumerator enumerator = dictionary.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair = enumerator.Current;
						if (flag)
						{
							if (keyValuePair.Value)
							{
								this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
							}
						}
						else
						{
							this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
						}
					}
					return;
				}
			}
			this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
		}

		// Token: 0x06005E3C RID: 24124 RVA: 0x001E2E0C File Offset: 0x001E100C
		public void RemoveCosmeticItemFromSet(CosmeticsController.CosmeticSet set, string itemName, bool applyToPlayerPrefs)
		{
			this.cachedSet.CopyItems(set);
			for (int i = 0; i < 16; i++)
			{
				if (set.items[i].displayName == itemName)
				{
					set.items[i] = this.nullItem;
					if (applyToPlayerPrefs)
					{
						this.SaveItemPreference((CosmeticsController.CosmeticSlots)i, i, this.nullItem);
					}
				}
			}
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			BodyDockPositions component = offlineVRRig.GetComponent<BodyDockPositions>();
			set.ActivateCosmetics(this.cachedSet, offlineVRRig, component, offlineVRRig.cosmeticsObjectRegistry);
		}

		// Token: 0x06005E3D RID: 24125 RVA: 0x001E2E94 File Offset: 0x001E1094
		public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
		{
			BundleManager.instance._tryOnBundlesStand.ClearSelectedBundle();
			this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005E3E RID: 24126 RVA: 0x001E2EC8 File Offset: 0x001E10C8
		public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(CosmeticsController.CosmeticSet set, string itemName)
		{
			CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
			CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
			if (cosmeticItem.bundledItems.Length != 0)
			{
				foreach (string text in cosmeticItem.bundledItems)
				{
					if (this.AnyMatch(set, this.allCosmeticsDict[text]))
					{
						if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
						}
						else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotWearing)
						{
							ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
						}
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.NotASet)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
					}
					else if (ewearingCosmeticSet == CosmeticsController.EWearingCosmeticSet.Complete)
					{
						ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
					}
				}
			}
			return ewearingCosmeticSet;
		}

		// Token: 0x06005E3F RID: 24127 RVA: 0x001E2F3C File Offset: 0x001E113C
		public void PressCosmeticStandButton(CosmeticStand pressedStand)
		{
			this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
				this.currentCart.RemoveAt(this.searchIndex);
				pressedStand.isOn = false;
				for (int i = 0; i < 16; i++)
				{
					if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
				this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
				pressedStand.isOn = true;
				if (this.currentCart.Count > this.numFittingRoomButtons)
				{
					foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
					{
						if (!(cosmeticStand == null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.numFittingRoomButtons].itemName)
						{
							cosmeticStand.isOn = false;
							cosmeticStand.UpdateColor();
							break;
						}
					}
					this.currentCart.RemoveAt(this.numFittingRoomButtons);
				}
			}
			pressedStand.UpdateColor();
			this.UpdateShoppingCart();
		}

		// Token: 0x06005E40 RID: 24128 RVA: 0x001E30A0 File Offset: 0x001E12A0
		public void PressWardrobeItemButton(CosmeticsController.CosmeticItem cosmeticItem, bool isLeftHand, bool isTempCosm)
		{
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
			if (isTempCosm)
			{
				this.PressTemporaryWardrobeItemButton(itemFromDict, isLeftHand);
			}
			else
			{
				this.PressWardrobeItemButton(itemFromDict, isLeftHand);
			}
			this.UpdateWornCosmetics(true);
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated.Invoke();
		}

		// Token: 0x06005E41 RID: 24129 RVA: 0x001E30F0 File Offset: 0x001E12F0
		private void PressWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
			if (list.Capacity < 16)
			{
				list.Capacity = 16;
			}
			this.ApplyCosmeticItemToSet(this.currentWornSet, item, isLeftHand, true, list);
			foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
			{
				this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
			}
			CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
			this.UpdateShoppingCart();
		}

		// Token: 0x06005E42 RID: 24130 RVA: 0x001E3184 File Offset: 0x001E1384
		private void PressTemporaryWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
		{
			this.ApplyCosmeticItemToSet(this.tempUnlockedSet, item, isLeftHand, false);
		}

		// Token: 0x06005E43 RID: 24131 RVA: 0x001E3198 File Offset: 0x001E1398
		public void PressWardrobeFunctionButton(string function)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(function);
			if (num <= 2554875734U)
			{
				if (num <= 895779448U)
				{
					if (num != 292255708U)
					{
						if (num != 306900080U)
						{
							if (num == 895779448U)
							{
								if (function == "badge")
								{
									if (this.wardrobeType == 2)
									{
										return;
									}
									this.wardrobeType = 2;
								}
							}
						}
						else if (function == "left")
						{
							this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
							if (this.cosmeticsPages[this.wardrobeType] < 0)
							{
								this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
							}
						}
					}
					else if (function == "face")
					{
						if (this.wardrobeType == 1)
						{
							return;
						}
						this.wardrobeType = 1;
					}
				}
				else if (num != 1538531746U)
				{
					if (num != 2028154341U)
					{
						if (num == 2554875734U)
						{
							if (function == "chest")
							{
								if (this.wardrobeType == 8)
								{
									return;
								}
								this.wardrobeType = 8;
							}
						}
					}
					else if (function == "right")
					{
						this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
						if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
						{
							this.cosmeticsPages[this.wardrobeType] = 0;
						}
					}
				}
				else if (function == "back")
				{
					if (this.wardrobeType == 7)
					{
						return;
					}
					this.wardrobeType = 7;
				}
			}
			else if (num <= 3034286914U)
			{
				if (num != 2633735346U)
				{
					if (num != 2953262278U)
					{
						if (num == 3034286914U)
						{
							if (function == "fur")
							{
								if (this.wardrobeType == 4)
								{
									return;
								}
								this.wardrobeType = 4;
							}
						}
					}
					else if (function == "outfit")
					{
						if (this.wardrobeType == 5)
						{
							return;
						}
						this.wardrobeType = 5;
					}
				}
				else if (function == "arms")
				{
					if (this.wardrobeType == 6)
					{
						return;
					}
					this.wardrobeType = 6;
				}
			}
			else if (num <= 3300536096U)
			{
				if (num != 3081164502U)
				{
					if (num == 3300536096U)
					{
						if (function == "hand")
						{
							if (this.wardrobeType == 3)
							{
								return;
							}
							this.wardrobeType = 3;
						}
					}
				}
				else if (function == "tagEffect")
				{
					if (this.wardrobeType == 10)
					{
						return;
					}
					this.wardrobeType = 10;
				}
			}
			else if (num != 3568683773U)
			{
				if (num == 4072609730U)
				{
					if (function == "hat")
					{
						if (this.wardrobeType == 0)
						{
							return;
						}
						this.wardrobeType = 0;
					}
				}
			}
			else if (function == "reserved")
			{
				if (this.wardrobeType == 9)
				{
					return;
				}
				this.wardrobeType = 9;
			}
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated.Invoke();
		}

		// Token: 0x06005E44 RID: 24132 RVA: 0x001E3524 File Offset: 0x001E1724
		public void ClearCheckout(bool sendEvent)
		{
			if (sendEvent)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.currentCart);
			}
			this.itemToBuy = this.nullItem;
			this.RefreshItemToBuyPreview();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState(null, false);
		}

		// Token: 0x06005E45 RID: 24133 RVA: 0x001E3560 File Offset: 0x001E1760
		public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
		{
			this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
			if (this.searchIndex != -1)
			{
				this.currentCart.RemoveAt(this.searchIndex);
				for (int i = 0; i < 16; i++)
				{
					if (cosmeticItem.itemName == this.tryOnSet.items[i].itemName)
					{
						this.tryOnSet.items[i] = this.nullItem;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005E46 RID: 24134 RVA: 0x001E35E3 File Offset: 0x001E17E3
		public void ClearCheckoutAndCart(bool sendEvent)
		{
			this.currentCart.Clear();
			this.tryOnSet.ClearSet(this.nullItem);
			this.ClearCheckout(sendEvent);
		}

		// Token: 0x06005E47 RID: 24135 RVA: 0x001E3608 File Offset: 0x001E1808
		public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
		{
			if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.tryOnSet.ClearSet(this.nullItem);
				if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
				{
					this.itemToBuy = this.nullItem;
					this.RefreshItemToBuyPreview();
				}
				else
				{
					this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
					this.checkoutCartButtonPressedWithLeft = isLeftHand;
					this.RefreshItemToBuyPreview();
				}
				this.ProcessPurchaseItemState(null, isLeftHand);
				this.UpdateShoppingCart();
			}
		}

		// Token: 0x06005E48 RID: 24136 RVA: 0x001E3690 File Offset: 0x001E1890
		private void RefreshItemToBuyPreview()
		{
			if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
			{
				List<string> list = new List<string>();
				foreach (string itemID in this.itemToBuy.bundledItems)
				{
					this.tempItem = this.GetItemFromDict(itemID);
					list.Add(this.tempItem.displayName);
				}
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActiveArray(list.ToArray(), new bool[list.Count]);
					}
					this.iterator++;
				}
			}
			else
			{
				this.iterator = 0;
				while (this.iterator < this.itemCheckouts.Count)
				{
					if (!this.itemCheckouts[this.iterator].IsNull())
					{
						this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName, false);
					}
					this.iterator++;
				}
			}
			this.ApplyCosmeticItemToSet(this.tryOnSet, this.itemToBuy, this.checkoutCartButtonPressedWithLeft, false);
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005E49 RID: 24137 RVA: 0x001E37F9 File Offset: 0x001E19F9
		public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
		}

		// Token: 0x06005E4A RID: 24138 RVA: 0x001E3808 File Offset: 0x001E1A08
		public void PurchaseBundle(StoreBundle bundleToPurchase, ICreatorCodeProvider ccp)
		{
			CosmeticsController.<PurchaseBundle>d__183 <PurchaseBundle>d__;
			<PurchaseBundle>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PurchaseBundle>d__.<>4__this = this;
			<PurchaseBundle>d__.bundleToPurchase = bundleToPurchase;
			<PurchaseBundle>d__.ccp = ccp;
			<PurchaseBundle>d__.<>1__state = -1;
			<PurchaseBundle>d__.<>t__builder.Start<CosmeticsController.<PurchaseBundle>d__183>(ref <PurchaseBundle>d__);
		}

		// Token: 0x06005E4B RID: 24139 RVA: 0x001E384F File Offset: 0x001E1A4F
		private void OnCreatorCodeFailure()
		{
			this.buyingBundle = false;
		}

		// Token: 0x06005E4C RID: 24140 RVA: 0x001E3858 File Offset: 0x001E1A58
		private void OnCreatorCodeValid(NexusGroupId id, string creatorCode)
		{
			if (this.buyingBundle)
			{
				this.SetValidatedCreatorCode(new NexusManager.MemberCode
				{
					memberCode = creatorCode,
					groupId = id
				});
				this.SteamPurchase();
			}
		}

		// Token: 0x06005E4D RID: 24141 RVA: 0x001E3884 File Offset: 0x001E1A84
		public void PressEarlyAccessButton()
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
			this.ProcessPurchaseItemState("left", false);
			this.buyingBundle = true;
			this.itemToPurchase = this.BundlePlayfabItemName;
			ATM_Manager.instance.shinyRocksCost = (float)this.BundleShinyRocks;
			this.SteamPurchase();
		}

		// Token: 0x06005E4E RID: 24142 RVA: 0x001E38D0 File Offset: 0x001E1AD0
		public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
		{
			switch (this.currentPurchaseItemStage)
			{
			case CosmeticsController.PurchaseItemStages.Start:
				this.itemToBuy = this.nullItem;
				this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!", null, null, false, false);
				this.UpdateShoppingCart();
				return;
			case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.currentCart);
				this.searchIndex = this.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => this.itemToBuy.itemName == x.itemName);
				if (this.searchIndex > -1)
				{
					this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
					return;
				}
				if (this.itemToBuy.cost <= this.currencyBalance)
				{
					this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?", "NO!", "YES!", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
					return;
				}
				this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!", "-", "-", true, true);
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			case CosmeticsController.PurchaseItemStages.ItemSelected:
				if (buttonSide == "right")
				{
					GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
					this.FormattedPurchaseText("ARE YOU REALLY SURE?", "YES! I NEED IT!", "LET ME THINK ABOUT IT", false, false);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.ItemOwned:
			case CosmeticsController.PurchaseItemStages.Buying:
				break;
			case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
				if (buttonSide == "left")
				{
					this.FormattedPurchaseText("PURCHASING ITEM...", "-", "-", true, true);
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
					this.isLastHandTouchedLeft = isLeftHand;
					this.PurchaseItem();
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
				this.ProcessPurchaseItemState(null, isLeftHand);
				return;
			case CosmeticsController.PurchaseItemStages.Success:
			{
				this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!", "-", "-", true, true);
				VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
				offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.itemName;
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
				if (itemFromDict.bundledItems != null)
				{
					foreach (string text in itemFromDict.bundledItems)
					{
						VRRig offlineVRRig2 = GorillaTagger.Instance.offlineVRRig;
						offlineVRRig2.concatStringOfCosmeticsAllowed += text;
					}
				}
				this.tryOnSet.ClearSet(this.nullItem);
				this.UpdateShoppingCart();
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics();
				this.UpdateWardrobeModelsAndButtons();
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated.Invoke();
				break;
			}
			case CosmeticsController.PurchaseItemStages.Failure:
				this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.", "-", "-", true, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005E4F RID: 24143 RVA: 0x001E3B60 File Offset: 0x001E1D60
		public void FormattedPurchaseText(string finalLineVar, string leftPurchaseButtonText = null, string rightPurchaseButtonText = null, bool leftButtonOn = false, bool rightButtonOn = false)
		{
			this.finalLine = finalLineVar;
			if (leftPurchaseButtonText != null)
			{
				this.leftCheckoutPurchaseButtonString = leftPurchaseButtonText;
				this.leftCheckoutPurchaseButtonOn = leftButtonOn;
			}
			if (rightPurchaseButtonText != null)
			{
				this.rightCheckoutPurchaseButtonString = rightPurchaseButtonText;
				this.rightCheckoutPurchaseButtonOn = rightButtonOn;
			}
			string newText = string.Concat(new string[]
			{
				"SELECTION: ",
				this.GetItemDisplayName(this.itemToBuy),
				"\nITEM COST: ",
				this.itemToBuy.cost.ToString(),
				"\nYOU HAVE: ",
				this.currencyBalance.ToString(),
				"\n\n",
				this.finalLine
			});
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdatePurchaseText(newText, leftPurchaseButtonText, rightPurchaseButtonText, leftButtonOn, rightButtonOn);
				}
				this.iterator++;
			}
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x001E3C5C File Offset: 0x001E1E5C
		public void PurchaseItem()
		{
			PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
			{
				ItemId = this.itemToBuy.itemName,
				Price = this.itemToBuy.cost,
				VirtualCurrency = this.currencyName,
				CatalogVersion = this.catalog
			}, delegate(PurchaseItemResult result)
			{
				if (result.Items.Count > 0)
				{
					foreach (ItemInstance itemInstance in result.Items)
					{
						CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
						if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
						{
							this.UnlockItem(itemInstance.ItemId, false);
							foreach (string itemIdToUnlock in itemFromDict.bundledItems)
							{
								this.UnlockItem(itemIdToUnlock, false);
							}
						}
						else
						{
							this.UnlockItem(itemInstance.ItemId, false);
						}
					}
					this.UpdateMyCosmetics();
					if (NetworkSystem.Instance.InRoom)
					{
						base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
					}
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
					this.currencyBalance -= this.itemToBuy.cost;
					this.UpdateShoppingCart();
					this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
					return;
				}
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, delegate(PlayFabError error)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
				this.ProcessPurchaseItemState(null, false);
			}, null, null);
		}

		// Token: 0x06005E51 RID: 24145 RVA: 0x001E3CC8 File Offset: 0x001E1EC8
		private void UnlockItem(string itemIdToUnlock, bool relock = false)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => itemIdToUnlock == x.itemName);
			if (num > -1)
			{
				this.ModifyUnlockList(this.unlockedCosmetics, num, relock);
				if (relock)
				{
					this.concatStringCosmeticsAllowed.Replace(this.allCosmetics[num].itemName, string.Empty);
				}
				else
				{
					this.concatStringCosmeticsAllowed += this.allCosmetics[num].itemName;
				}
				switch (this.allCosmetics[num].itemCategory)
				{
				case CosmeticsController.CosmeticCategory.Hat:
					this.ModifyUnlockList(this.unlockedHats, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Badge:
					this.ModifyUnlockList(this.unlockedBadges, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Face:
					this.ModifyUnlockList(this.unlockedFaces, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Paw:
					if (!this.allCosmetics[num].isThrowable)
					{
						this.ModifyUnlockList(this.unlockedPaws, num, relock);
						return;
					}
					this.ModifyUnlockList(this.unlockedThrowables, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Chest:
					this.ModifyUnlockList(this.unlockedChests, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Fur:
					this.ModifyUnlockList(this.unlockedFurs, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Shirt:
					this.ModifyUnlockList(this.unlockedShirts, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Back:
					this.ModifyUnlockList(this.unlockedBacks, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Arms:
					this.ModifyUnlockList(this.unlockedArms, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Pants:
					this.ModifyUnlockList(this.unlockedPants, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.TagEffect:
					this.ModifyUnlockList(this.unlockedTagFX, num, relock);
					return;
				case CosmeticsController.CosmeticCategory.Count:
					break;
				case CosmeticsController.CosmeticCategory.Set:
					foreach (string itemIdToUnlock2 in this.allCosmetics[num].bundledItems)
					{
						this.UnlockItem(itemIdToUnlock2, false);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x001E3EA4 File Offset: 0x001E20A4
		private void ModifyUnlockList(List<CosmeticsController.CosmeticItem> list, int index, bool relock)
		{
			if (!relock && !list.Contains(this.allCosmetics[index]))
			{
				list.Add(this.allCosmetics[index]);
				return;
			}
			if (relock && list.Contains(this.allCosmetics[index]))
			{
				list.Remove(this.allCosmetics[index]);
			}
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x001E3F05 File Offset: 0x001E2105
		private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
		{
			Debug.Log("Cosmetic updated check!");
			yield return new WaitForSeconds(1f);
			this.foundCosmetic = false;
			this.attempts = 0;
			while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
			{
				this.playerIDList.Clear();
				if (this.UseNewCosmeticsPath())
				{
					this.playerIDList.Add("Inventory");
					PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Value.Value.Contains(itemToBuyID))
							{
								PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
								{
									Receivers = 0
								}, SendOptions.SendReliable);
								this.foundCosmetic = true;
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						this.ReauthOrBan(error);
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
				else
				{
					this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
					PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
					{
						Keys = this.playerIDList,
						SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper()
					}, delegate(GetSharedGroupDataResult result)
					{
						this.attempts++;
						foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
						{
							if (keyValuePair.Value.Value.Contains(itemToBuyID))
							{
								NetworkSystemRaiseEvent.RaiseEvent(199, null, NetworkSystemRaiseEvent.neoOthers, true);
								this.foundCosmetic = true;
							}
							else
							{
								Debug.Log("didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
							}
						}
						if (this.foundCosmetic)
						{
							this.UpdateWornCosmetics(true);
						}
					}, delegate(PlayFabError error)
					{
						this.attempts++;
						if (error.Error == 1074)
						{
							PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
						}
						else if (error.Error == 1002)
						{
							GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
						}
						Debug.Log("Got error retrieving user data, on attempt " + this.attempts.ToString());
						Debug.Log(error.GenerateErrorReport());
					}, null, null);
					yield return new WaitForSeconds(1f);
				}
			}
			Debug.Log("done!");
			yield break;
		}

		// Token: 0x06005E54 RID: 24148 RVA: 0x001E3F1C File Offset: 0x001E211C
		public void UpdateWardrobeModelsAndButtons()
		{
			if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				return;
			}
			foreach (WardrobeInstance wardrobeInstance in this.wardrobes)
			{
				wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem);
				wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem = ((this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count) ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem);
				this.iterator = 0;
				while (this.iterator < wardrobeInstance.wardrobeItemButtons.Length)
				{
					CosmeticsController.CosmeticItem currentCosmeticItem = wardrobeInstance.wardrobeItemButtons[this.iterator].currentCosmeticItem;
					wardrobeInstance.wardrobeItemButtons[this.iterator].isOn = (!currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem));
					wardrobeInstance.wardrobeItemButtons[this.iterator].UpdateColor();
					this.iterator++;
				}
				wardrobeInstance.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[0].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[1].currentCosmeticItem.displayName, false);
				wardrobeInstance.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobeInstance.wardrobeItemButtons[2].currentCosmeticItem.displayName, false);
				wardrobeInstance.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
			}
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x001E419C File Offset: 0x001E239C
		public int GetCategorySize(CosmeticsController.CosmeticCategory category)
		{
			int indexForCategory = this.GetIndexForCategory(category);
			if (indexForCategory != -1)
			{
				return this.itemLists[indexForCategory].Count;
			}
			return 0;
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x001E41C4 File Offset: 0x001E23C4
		public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
		{
			if (cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0)
			{
				return this.nullItem;
			}
			return this.itemLists[category][cosmeticIndex];
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x001E41EF File Offset: 0x001E23EF
		public CosmeticsController.CosmeticItem GetCosmetic(CosmeticsController.CosmeticCategory category, int cosmeticIndex)
		{
			return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x001E4200 File Offset: 0x001E2400
		private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return 0;
			case CosmeticsController.CosmeticCategory.Badge:
				return 2;
			case CosmeticsController.CosmeticCategory.Face:
				return 1;
			case CosmeticsController.CosmeticCategory.Paw:
				return 3;
			case CosmeticsController.CosmeticCategory.Chest:
				return 9;
			case CosmeticsController.CosmeticCategory.Fur:
				return 4;
			case CosmeticsController.CosmeticCategory.Shirt:
				return 5;
			case CosmeticsController.CosmeticCategory.Back:
				return 8;
			case CosmeticsController.CosmeticCategory.Arms:
				return 7;
			case CosmeticsController.CosmeticCategory.Pants:
				return 6;
			case CosmeticsController.CosmeticCategory.TagEffect:
				return 10;
			default:
				return 0;
			}
		}

		// Token: 0x06005E59 RID: 24153 RVA: 0x001E425C File Offset: 0x001E245C
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.currentWornSet, cosmetic);
		}

		// Token: 0x06005E5A RID: 24154 RVA: 0x001E426B File Offset: 0x001E246B
		public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic, bool tempSet)
		{
			if (!tempSet)
			{
				return this.IsCosmeticEquipped(cosmetic);
			}
			return this.IsTemporaryCosmeticEquipped(cosmetic);
		}

		// Token: 0x06005E5B RID: 24155 RVA: 0x001E427F File Offset: 0x001E247F
		public bool IsTemporaryCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
		{
			return this.AnyMatch(this.tempUnlockedSet, cosmetic);
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x001E4290 File Offset: 0x001E2490
		public CosmeticsController.CosmeticItem GetSlotItem(CosmeticsController.CosmeticSlots slot, bool checkOpposite = true, bool tempSet = false)
		{
			int num = (int)slot;
			if (checkOpposite)
			{
				num = (int)CosmeticsController.CosmeticSet.OppositeSlot(slot);
			}
			if (!tempSet)
			{
				return this.currentWornSet.items[num];
			}
			return this.tempUnlockedSet.items[num];
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x001E42CF File Offset: 0x001E24CF
		public string[] GetCurrentlyWornCosmetics(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToDisplayNameArray();
			}
			return this.tempUnlockedSet.ToDisplayNameArray();
		}

		// Token: 0x06005E5E RID: 24158 RVA: 0x001E42EB File Offset: 0x001E24EB
		public bool[] GetCurrentRightEquippedSided(bool tempSet = false)
		{
			if (!tempSet)
			{
				return this.currentWornSet.ToOnRightSideArray();
			}
			return this.tempUnlockedSet.ToOnRightSideArray();
		}

		// Token: 0x06005E5F RID: 24159 RVA: 0x001E4308 File Offset: 0x001E2508
		public void UpdateShoppingCart()
		{
			this.iterator = 0;
			while (this.iterator < this.itemCheckouts.Count)
			{
				if (!this.itemCheckouts[this.iterator].IsNull())
				{
					this.itemCheckouts[this.iterator].UpdateFromCart(this.currentCart, this.itemToBuy);
				}
				this.iterator++;
			}
			this.iterator = 0;
			while (this.iterator < this.fittingRooms.Count)
			{
				if (!this.fittingRooms[this.iterator].IsNull())
				{
					this.fittingRooms[this.iterator].UpdateFromCart(this.currentCart, this.tryOnSet);
				}
				this.iterator++;
			}
			if (CosmeticsV2Spawner_Dirty.allPartsInstantiated)
			{
				this.UpdateWardrobeModelsAndButtons();
			}
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x001E43EA File Offset: 0x001E25EA
		public void UpdateWornCosmetics()
		{
			this.UpdateWornCosmetics(false, false);
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x001E43F4 File Offset: 0x001E25F4
		public void UpdateWornCosmetics(bool sync)
		{
			this.UpdateWornCosmetics(sync, false);
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x001E4400 File Offset: 0x001E2600
		public void UpdateWornCosmetics(bool sync, bool playfx)
		{
			VRRig localRig = VRRig.LocalRig;
			this.activeMergedSet.MergeInSets(this.currentWornSet, this.tempUnlockedSet, (string id) => PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(localRig, id));
			GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.activeMergedSet, this.tryOnSet, playfx);
			if (sync && GorillaTagger.Instance.myVRRig != null)
			{
				if (this.isHidingCosmeticsFromRemotePlayers)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", 0, Array.Empty<object>());
					return;
				}
				int[] array = this.activeMergedSet.ToPackedIDArray();
				int[] array2 = this.tryOnSet.ToPackedIDArray();
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", 1, new object[]
				{
					array,
					array2,
					playfx
				});
			}
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x001E44D9 File Offset: 0x001E26D9
		public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
		{
			if (!this.allCosmeticsDict.TryGetValue(itemID, ref this.cosmeticItemVar))
			{
				return this.nullItem;
			}
			return this.cosmeticItemVar;
		}

		// Token: 0x06005E64 RID: 24164 RVA: 0x001E44FC File Offset: 0x001E26FC
		public string GetItemNameFromDisplayName(string displayName)
		{
			if (!this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, ref this.returnString))
			{
				return "null";
			}
			return this.returnString;
		}

		// Token: 0x06005E65 RID: 24165 RVA: 0x001E4520 File Offset: 0x001E2720
		public CosmeticSO GetCosmeticSOFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return null;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return null;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return null;
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x001E4590 File Offset: 0x001E2790
		public CosmeticAnchorAntiIntersectOffsets GetClipOffsetsFromDisplayName(string displayName)
		{
			string itemNameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
			if (itemNameFromDisplayName.Equals("null"))
			{
				return this.defaultClipOffsets;
			}
			AllCosmeticsArraySO allCosmeticsArraySO = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
			if (allCosmeticsArraySO == null)
			{
				GTDev.LogWarning<string>("null AllCosmeticsArraySO", null);
				return this.defaultClipOffsets;
			}
			CosmeticSO cosmeticSO = allCosmeticsArraySO.SearchForCosmeticSO(itemNameFromDisplayName);
			if (cosmeticSO != null)
			{
				return cosmeticSO.info.anchorAntiIntersectOffsets;
			}
			GTDev.Log<string>("Could not find cosmetic info for " + itemNameFromDisplayName, null);
			return this.defaultClipOffsets;
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x001E461C File Offset: 0x001E281C
		public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
		{
			if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
			{
				return set.IsActive(item.displayName);
			}
			if (item.bundledItems.Length == 1)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
			}
			if (item.bundledItems.Length == 2)
			{
				return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
			}
			return item.bundledItems.Length >= 3 && (this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2])));
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x001E46F0 File Offset: 0x001E28F0
		public void Initialize()
		{
			if (!base.gameObject.activeSelf || this.v2_isCosmeticPlayFabCatalogDataLoaded || this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback)
			{
				return;
			}
			if (this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				this.GetCosmeticsPlayFabCatalogData();
				return;
			}
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = true;
			this.V2_allCosmeticsInfoAssetRef_OnPostLoad = (Action)Delegate.Combine(this.V2_allCosmeticsInfoAssetRef_OnPostLoad, new Action(this.GetCosmeticsPlayFabCatalogData));
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x001E4753 File Offset: 0x001E2953
		public void GetLastDailyLogin()
		{
			PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest(), delegate(GetUserDataResult result)
			{
				if (result.Data.TryGetValue("DailyLogin", ref this.userDataRecord))
				{
					this.lastDailyLogin = this.userDataRecord.Value;
					return;
				}
				this.lastDailyLogin = "NONE";
				base.StartCoroutine(this.GetMyDaily());
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error getting read-only user data:");
				Debug.Log(error.GenerateErrorReport());
				this.lastDailyLogin = "FAILED";
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(0);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x001E4779 File Offset: 0x001E2979
		private IEnumerator CheckCanGetDaily()
		{
			while (!KIDManager.InitialisationComplete)
			{
				yield return new WaitForSeconds(1f);
			}
			for (;;)
			{
				if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
				{
					this.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) * 10000L);
					this.secondsUntilTomorrow = (int)(this.currentTime.AddDays(1.0).Date - this.currentTime).TotalSeconds;
					if (this.lastDailyLogin == null || this.lastDailyLogin == "")
					{
						this.GetLastDailyLogin();
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) == this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = true;
					}
					else if (this.currentTime.ToString("o").Substring(0, 10) != this.lastDailyLogin)
					{
						this.checkedDaily = true;
						this.gotMyDaily = false;
						base.StartCoroutine(this.GetMyDaily());
					}
					else if (this.lastDailyLogin == "FAILED")
					{
						this.GetLastDailyLogin();
					}
					this.secondsToWaitToCheckDaily = (this.checkedDaily ? 60f : 10f);
					this.UpdateCurrencyBoards();
					yield return new WaitForSeconds(this.secondsToWaitToCheckDaily);
				}
				else
				{
					yield return new WaitForSeconds(1f);
				}
			}
			yield break;
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x001E4788 File Offset: 0x001E2988
		private IEnumerator GetMyDaily()
		{
			yield return new WaitForSeconds(10f);
			GorillaServer.Instance.TryDistributeCurrency(delegate(ExecuteFunctionResult result)
			{
				this.GetCurrencyBalance();
				this.GetLastDailyLogin();
			}, delegate(PlayFabError error)
			{
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(0);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			});
			yield break;
		}

		// Token: 0x06005E6C RID: 24172 RVA: 0x001E4797 File Offset: 0x001E2997
		public void GetCosmeticsPlayFabCatalogData()
		{
			this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = false;
			if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
			}
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest
				{
					CatalogVersion = this.catalog
				}, delegate(GetCatalogItemsResult result2)
				{
					this.unlockedCosmetics.Clear();
					this.unlockedHats.Clear();
					this.unlockedBadges.Clear();
					this.unlockedFaces.Clear();
					this.unlockedPaws.Clear();
					this.unlockedFurs.Clear();
					this.unlockedShirts.Clear();
					this.unlockedPants.Clear();
					this.unlockedArms.Clear();
					this.unlockedBacks.Clear();
					this.unlockedChests.Clear();
					this.unlockedTagFX.Clear();
					this.unlockedThrowables.Clear();
					this.catalogItems = result2.Catalog;
					using (List<CatalogItem>.Enumerator enumerator = this.catalogItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CatalogItem catalogItem = enumerator.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
							{
								this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => catalogItem.ItemId == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempStringArray = null;
									this.hasPrice = false;
									if (catalogItem.Bundle != null)
									{
										this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
									}
									uint cost;
									if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, ref cost))
									{
										this.hasPrice = true;
									}
									CosmeticsController.CosmeticItem cosmeticItem = this.allCosmetics[this.searchIndex];
									cosmeticItem.itemName = catalogItem.ItemId;
									cosmeticItem.displayName = catalogItem.DisplayName;
									cosmeticItem.cost = (int)cost;
									cosmeticItem.bundledItems = this.tempStringArray;
									cosmeticItem.canTryOn = this.hasPrice;
									if (cosmeticItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
									{
										CosmeticInfoV2 cosmeticInfoV = this.v2_allCosmetics[this.searchIndex];
										cosmeticItem.isThrowable = (cosmeticInfoV.isThrowable && !cosmeticInfoV.hasWardrobeParts);
									}
									if (cosmeticItem.displayName == null)
									{
										string text = "null";
										if (this.allCosmetics[this.searchIndex].itemPicture)
										{
											text = this.allCosmetics[this.searchIndex].itemPicture.name;
										}
										string debugCosmeticSOName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cosmetic encountered with a null displayName at index {0}! ", this.searchIndex),
											"Setting displayName to id: \"",
											this.allCosmetics[this.searchIndex].itemName,
											"\". iconName=\"",
											text,
											"\".cosmeticSOName=\"",
											debugCosmeticSOName,
											"\". "
										}));
										cosmeticItem.displayName = cosmeticItem.itemName;
									}
									this.V2_ConformCosmeticItemV1DisplayName(ref cosmeticItem);
									this._allCosmetics[this.searchIndex] = cosmeticItem;
									this._allCosmeticsDict[cosmeticItem.itemName] = cosmeticItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.displayName] = cosmeticItem.itemName;
									this._allCosmeticsItemIDsfromDisplayNamesDict[cosmeticItem.overrideDisplayName] = cosmeticItem.itemName;
								}
							}
						}
					}
					for (int i = this._allCosmetics.Count - 1; i > -1; i--)
					{
						this.tempItem = this._allCosmetics[i];
						if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
						{
							string[] bundledItems = this.tempItem.bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
					}
					foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
					{
						string text2;
						StoreBundle bundleData2;
						keyValuePair.Deconstruct(ref text2, ref bundleData2);
						string text3 = text2;
						StoreBundle bundleData = bundleData2;
						int num = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => bundleData.playfabBundleID == x.itemName);
						if (num > 0 && this._allCosmetics[num].bundledItems != null)
						{
							string[] bundledItems = this._allCosmetics[num].bundledItems;
							for (int j = 0; j < bundledItems.Length; j++)
							{
								string setItemName = bundledItems[j];
								this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => setItemName == x.itemName);
								if (this.searchIndex > -1)
								{
									this.tempItem = this._allCosmetics[this.searchIndex];
									this.tempItem.canTryOn = true;
									this._allCosmetics[this.searchIndex] = this.tempItem;
									this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
									this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
								}
							}
						}
						if (!bundleData.HasPrice)
						{
							num = this.catalogItems.FindIndex((CatalogItem ci) => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID);
							if (num > 0)
							{
								uint bundlePrice;
								if (this.catalogItems[num].VirtualCurrencyPrices.TryGetValue("RM", ref bundlePrice))
								{
									BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(bundlePrice);
								}
								else
								{
									BundleManager.instance.storeBundlesById[text3].TryUpdatePrice(null);
								}
							}
						}
					}
					this.searchIndex = this._allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => "Slingshot" == x.itemName);
					if (this.searchIndex < 0)
					{
						throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
					}
					this._allCosmeticsDict["Slingshot"] = this._allCosmetics[this.searchIndex];
					this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
					this.allCosmeticsDict_isInitialized = true;
					this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					using (List<ItemInstance>.Enumerator enumerator3 = result.Inventory.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ItemInstance item = enumerator3.Current;
							if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
							{
								if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
								{
									foreach (CosmeticSO cosmeticSO in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
									{
										CosmeticsController.CosmeticItem cosmeticItem2;
										if (this.allCosmeticsDict.TryGetValue(cosmeticSO.info.playFabID, ref cosmeticItem2))
										{
											this.unlockedCosmetics.Add(cosmeticItem2);
										}
									}
								}
								BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
								if (!dictionary.ContainsKey(item.ItemId))
								{
									this.searchIndex = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.ItemId == x.itemName);
									if (this.searchIndex > -1)
									{
										dictionary[item.ItemId] = item.ItemId;
										this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
									}
								}
							}
						}
					}
					foreach (CosmeticsController.CosmeticItem cosmeticItem3 in this.unlockedCosmetics)
					{
						if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(cosmeticItem3))
						{
							this.unlockedHats.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(cosmeticItem3))
						{
							this.unlockedFaces.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(cosmeticItem3))
						{
							this.unlockedBadges.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Paw)
						{
							if (!cosmeticItem3.isThrowable && !this.unlockedPaws.Contains(cosmeticItem3))
							{
								this.unlockedPaws.Add(cosmeticItem3);
							}
							else if (cosmeticItem3.isThrowable && !this.unlockedThrowables.Contains(cosmeticItem3))
							{
								this.unlockedThrowables.Add(cosmeticItem3);
							}
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(cosmeticItem3))
						{
							this.unlockedFurs.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(cosmeticItem3))
						{
							this.unlockedShirts.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(cosmeticItem3))
						{
							this.unlockedArms.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(cosmeticItem3))
						{
							this.unlockedBacks.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(cosmeticItem3))
						{
							this.unlockedChests.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(cosmeticItem3))
						{
							this.unlockedPants.Add(cosmeticItem3);
						}
						else if (cosmeticItem3.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(cosmeticItem3))
						{
							this.unlockedTagFX.Add(cosmeticItem3);
						}
						this.concatStringCosmeticsAllowed += cosmeticItem3.itemName;
					}
					BuilderSetManager.instance.OnGotInventoryItems(result, result2);
					this.currencyBalance = result.VirtualCurrency[this.currencyName];
					int num2;
					this.playedInBeta = (result.VirtualCurrency.TryGetValue("TC", ref num2) && num2 > 0);
					Action onGetCurrency = this.OnGetCurrency;
					if (onGetCurrency != null)
					{
						onGetCurrency.Invoke();
					}
					BundleManager.instance.CheckIfBundlesOwned();
					StoreUpdater.instance.Initialize();
					this.currentWornSet.LoadFromPlayerPreferences(this);
					this.LoadSavedOutfits();
					if (!ATM_Manager.instance.alreadyBegan)
					{
						ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
						ATM_Manager.instance.alreadyBegan = true;
					}
					this.ProcessPurchaseItemState(null, false);
					this.UpdateShoppingCart();
					this.UpdateCurrencyBoards();
					if (this.UseNewCosmeticsPath())
					{
						this.ConfirmIndividualCosmeticsSharedGroup(result);
					}
					Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
					if (onCosmeticsUpdated != null)
					{
						onCosmeticsUpdated.Invoke();
					}
					this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
					Action v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
					if (v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess != null)
					{
						v2_OnGetCosmeticsPlayFabCatalogData_PostSuccess.Invoke();
					}
					if (!CosmeticsV2Spawner_Dirty.startedAllPartsInstantiated && !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
					{
						CosmeticsV2Spawner_Dirty.StartInstantiatingPrefabs();
					}
				}, delegate(PlayFabError error)
				{
					if (error.Error == 1074)
					{
						PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					}
					else if (error.Error == 1002)
					{
						Application.Quit();
						NetworkSystem.Instance.ReturnToSinglePlayer();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsByType<GameObject>(0);
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
					}
					if (!this.tryTwice)
					{
						this.tryTwice = true;
						this.GetCosmeticsPlayFabCatalogData();
					}
				}, null, null);
			}, delegate(PlayFabError error)
			{
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				}
				else if (error.Error == 1002)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(0);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
				if (!this.tryTwice)
				{
					this.tryTwice = true;
					this.GetCosmeticsPlayFabCatalogData();
				}
			}, null, null);
		}

		// Token: 0x06005E6D RID: 24173 RVA: 0x001E47D8 File Offset: 0x001E29D8
		public void SteamPurchase()
		{
			if (string.IsNullOrEmpty(this.itemToPurchase))
			{
				Debug.Log("Unable to start steam purchase process. itemToPurchase is not set.");
				return;
			}
			Debug.Log(string.Format("attempting to purchase item through steam. Is this a bundle purchase: {0}", this.buyingBundle));
			PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x001E483C File Offset: 0x001E2A3C
		private StartPurchaseRequest GetStartPurchaseRequest()
		{
			StartPurchaseRequest startPurchaseRequest = new StartPurchaseRequest();
			startPurchaseRequest.CatalogVersion = this.catalog;
			List<ItemPurchaseRequest> list = new List<ItemPurchaseRequest>();
			list.Add(new ItemPurchaseRequest
			{
				ItemId = this.itemToPurchase,
				Quantity = 1U,
				Annotation = "Purchased via in-game store"
			});
			startPurchaseRequest.Items = list;
			return startPurchaseRequest;
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x001E4890 File Offset: 0x001E2A90
		private void ProcessStartPurchaseResponse(StartPurchaseResult result)
		{
			Debug.Log("successfully started purchase. attempted to pay for purchase through steam");
			this.currentPurchaseID = result.OrderId;
			PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError), null, null);
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x001E48DD File Offset: 0x001E2ADD
		private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
		{
			return new PayForPurchaseRequest
			{
				OrderId = orderId,
				ProviderName = "Steam",
				Currency = "RM"
			};
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x001E4901 File Offset: 0x001E2B01
		private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
		{
			Debug.Log("succeeded on sending request for paying with steam! waiting for response");
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x001E4910 File Offset: 0x001E2B10
		private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
		{
			Debug.Log("Steam has called back that the user has finished the payment interaction");
			if (callBackResponse.m_bAuthorized == 0)
			{
				Debug.Log("Steam has indicated that the payment was not authorised.");
			}
			if (this.buyingBundle)
			{
				PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), delegate(ConfirmPurchaseResult _)
				{
					this.ProcessConfirmPurchaseSuccess();
				}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
				return;
			}
			PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), delegate(ConfirmPurchaseResult _)
			{
				this.ProcessConfirmPurchaseSuccess();
			}, new Action<PlayFabError>(this.ProcessConfirmPurchaseError), null, null);
		}

		// Token: 0x06005E73 RID: 24179 RVA: 0x001E498C File Offset: 0x001E2B8C
		private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("PlayerName", GorillaComputer.instance.savedName);
			dictionary.Add("Location", GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			if (this.validatedCreatorCode != null)
			{
				dictionary2.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary2.Add("NexusGroupId", this.validatedCreatorCode.groupId.Code);
				this.validatedCreatorCode = null;
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary2
			};
		}

		// Token: 0x06005E74 RID: 24180 RVA: 0x001E4A3C File Offset: 0x001E2C3C
		private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("PlayerName", GorillaComputer.instance.savedName);
			dictionary.Add("Location", GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			if (this.validatedCreatorCode != null)
			{
				dictionary2.Add("NexusCreatorId", this.validatedCreatorCode.memberCode);
				dictionary2.Add("NexusGroupId", this.validatedCreatorCode.groupId.Code);
				this.validatedCreatorCode = null;
			}
			return new ConfirmPurchaseRequest
			{
				OrderId = this.currentPurchaseID,
				CustomTags = dictionary2
			};
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x001E4AEC File Offset: 0x001E2CEC
		private void ProcessConfirmPurchaseSuccess()
		{
			if (this.buyingBundle)
			{
				this.buyingBundle = false;
				if (PhotonNetwork.InRoom)
				{
					object[] data = new object[0];
					NetworkSystemRaiseEvent.RaiseEvent(9, data, NetworkSystemRaiseEvent.newWeb, true);
				}
				base.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
			}
			else
			{
				ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
			}
			this.GetCurrencyBalance();
			this.UpdateCurrencyBoards();
			this.GetCosmeticsPlayFabCatalogData();
			GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x001E4B67 File Offset: 0x001E2D67
		private void ProcessConfirmPurchaseError(PlayFabError error)
		{
			this.ProcessSteamPurchaseError(error);
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
			this.UpdateCurrencyBoards();
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x001E4B84 File Offset: 0x001E2D84
		private void ProcessSteamPurchaseError(PlayFabError error)
		{
			PlayFabErrorCode error2 = error.Error;
			if (error2 <= 1064)
			{
				if (error2 <= 1015)
				{
					if (error2 == 1002)
					{
						PhotonNetwork.Disconnect();
						Object.DestroyImmediate(PhotonNetworkController.Instance);
						Object.DestroyImmediate(GTPlayer.Instance);
						GameObject[] array = Object.FindObjectsByType<GameObject>(0);
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						Application.Quit();
						goto IL_1A2;
					}
					if (error2 != 1015)
					{
						goto IL_192;
					}
					Debug.Log(string.Format("Attempted to pay for order, but has been Failed by Steam with error: {0}", error));
					goto IL_1A2;
				}
				else
				{
					if (error2 == 1059)
					{
						Debug.Log(string.Format("Attempting to do purchase through steam, steam has returned insufficient funds: {0}", error));
						goto IL_1A2;
					}
					if (error2 == 1063)
					{
						Debug.Log(string.Format("Attempted to connect to steam as payment provider, but received error: {0}", error));
						goto IL_1A2;
					}
					if (error2 != 1064)
					{
						goto IL_192;
					}
				}
			}
			else if (error2 <= 1081)
			{
				if (error2 == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					goto IL_1A2;
				}
				if (error2 == 1080)
				{
					Debug.Log(string.Format("Attempting to confirm purchase for order {0} but received error: {1}", this.currentPurchaseID, error));
					goto IL_1A2;
				}
				if (error2 != 1081)
				{
					goto IL_192;
				}
			}
			else
			{
				if (error2 == 1110)
				{
					Debug.Log(string.Format("PlayFab threw an internal server error: {0}", error));
					goto IL_1A2;
				}
				if (error2 == 1221)
				{
					Debug.Log(string.Format("Attempted to load {0} from {1} but received an error: {2}", this.itemToPurchase, this.catalog, error));
					goto IL_1A2;
				}
				if (error2 != 1489)
				{
					goto IL_192;
				}
			}
			Debug.Log(string.Format("Attempted to pay for order {0}, however received an error: {1}", this.currentPurchaseID, error));
			goto IL_1A2;
			IL_192:
			Debug.Log(string.Format("Steam purchase flow returned error: {0}", error));
			IL_1A2:
			ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x001E4D40 File Offset: 0x001E2F40
		public void UpdateCurrencyBoards()
		{
			this.FormattedPurchaseText(this.finalLine, null, null, false, false);
			this.iterator = 0;
			while (this.iterator < this.currencyBoards.Count)
			{
				if (this.currencyBoards[this.iterator].IsNotNull())
				{
					this.currencyBoards[this.iterator].UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
				}
				this.iterator++;
			}
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x001E4DCD File Offset: 0x001E2FCD
		public void AddCurrencyBoard(CurrencyBoard newCurrencyBoard)
		{
			if (this.currencyBoards.Contains(newCurrencyBoard))
			{
				return;
			}
			this.currencyBoards.Add(newCurrencyBoard);
			newCurrencyBoard.UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x001E4E08 File Offset: 0x001E3008
		public void RemoveCurrencyBoard(CurrencyBoard currencyBoardToRemove)
		{
			this.currencyBoards.Remove(currencyBoardToRemove);
		}

		// Token: 0x06005E7B RID: 24187 RVA: 0x001E4E17 File Offset: 0x001E3017
		public void GetCurrencyBalance()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				this.currencyBalance = result.VirtualCurrency[this.currencyName];
				this.UpdateCurrencyBoards();
				Action onGetCurrency = this.OnGetCurrency;
				if (onGetCurrency == null)
				{
					return;
				}
				onGetCurrency.Invoke();
			}, delegate(PlayFabError error)
			{
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					Application.Quit();
					NetworkSystem.Instance.ReturnToSinglePlayer();
					Object.DestroyImmediate(PhotonNetworkController.Instance);
					Object.DestroyImmediate(GTPlayer.Instance);
					GameObject[] array = Object.FindObjectsByType<GameObject>(0);
					for (int i = 0; i < array.Length; i++)
					{
						Object.Destroy(array[i]);
					}
				}
			}, null, null);
		}

		// Token: 0x06005E7C RID: 24188 RVA: 0x001E4E50 File Offset: 0x001E3050
		public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
		{
			if (item.overrideDisplayName != null && item.overrideDisplayName != "")
			{
				return item.overrideDisplayName;
			}
			return item.displayName;
		}

		// Token: 0x06005E7D RID: 24189 RVA: 0x001E4E7C File Offset: 0x001E307C
		public void UpdateMyCosmetics()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
				{
					this.UpdateMyCosmeticsForRoom(true);
				}
				if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetRoomData())
				{
					this.UpdateMyCosmeticsForRoom(false);
					return;
				}
			}
			else if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
			{
				this.UpdateMyCosmeticsNotInRoom();
			}
		}

		// Token: 0x06005E7E RID: 24190 RVA: 0x001E4F01 File Offset: 0x001E3101
		private void UpdateMyCosmeticsNotInRoom()
		{
			if (GorillaServer.Instance != null)
			{
				GorillaServer.Instance.UpdateUserCosmetics();
			}
		}

		// Token: 0x06005E7F RID: 24191 RVA: 0x001E4F20 File Offset: 0x001E3120
		private void UpdateMyCosmeticsForRoom(bool shouldSetSharedGroupData)
		{
			byte b = 9;
			if (shouldSetSharedGroupData)
			{
				b = 10;
			}
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			WebFlags flags = new WebFlags(3);
			raiseEventOptions.Flags = flags;
			object[] array = new object[0];
			PhotonNetwork.RaiseEvent(b, array, raiseEventOptions, SendOptions.SendReliable);
		}

		// Token: 0x06005E80 RID: 24192 RVA: 0x001E4F60 File Offset: 0x001E3160
		private void AlreadyOwnAllBundleButtons()
		{
			EarlyAccessButton[] array = this.earlyAccessButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AlreadyOwn();
			}
		}

		// Token: 0x06005E81 RID: 24193 RVA: 0x001E4F8A File Offset: 0x001E318A
		private bool UseNewCosmeticsPath()
		{
			return GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData();
		}

		// Token: 0x06005E82 RID: 24194 RVA: 0x001E4FA9 File Offset: 0x001E31A9
		public void CheckCosmeticsSharedGroup()
		{
			this.updateCosmeticsRetries++;
			if (this.updateCosmeticsRetries < this.maxUpdateCosmeticsRetries)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
			}
		}

		// Token: 0x06005E83 RID: 24195 RVA: 0x001E4FD4 File Offset: 0x001E31D4
		private IEnumerator WaitForNextCosmeticsAttempt()
		{
			int num = (int)Mathf.Pow(3f, (float)(this.updateCosmeticsRetries + 1));
			yield return new WaitForSeconds((float)num);
			this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
			yield break;
		}

		// Token: 0x06005E84 RID: 24196 RVA: 0x001E4FE4 File Offset: 0x001E31E4
		private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
		{
			Debug.Log("confirming individual cosmetics with shared group");
			this.latestInventory = inventory;
			if (PhotonNetwork.LocalPlayer.UserId == null)
			{
				base.StartCoroutine(this.WaitForNextCosmeticsAttempt());
				return;
			}
			PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
			{
				Keys = this.inventoryStringList,
				SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
			}, delegate(GetSharedGroupDataResult result)
			{
				bool flag = true;
				foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (keyValuePair.Key != "Inventory")
					{
						break;
					}
					foreach (ItemInstance itemInstance in inventory.Inventory)
					{
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag || result.Data.Count == 0)
				{
					this.UpdateMyCosmetics();
					return;
				}
				this.updateCosmeticsRetries = 0;
			}, delegate(PlayFabError error)
			{
				this.ReauthOrBan(error);
				this.CheckCosmeticsSharedGroup();
			}, null, null);
		}

		// Token: 0x06005E85 RID: 24197 RVA: 0x001E5080 File Offset: 0x001E3280
		public void ReauthOrBan(PlayFabError error)
		{
			if (error.Error == 1074)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == 1002)
			{
				Application.Quit();
				PhotonNetwork.Disconnect();
				Object.DestroyImmediate(PhotonNetworkController.Instance);
				Object.DestroyImmediate(GTPlayer.Instance);
				GameObject[] array = Object.FindObjectsByType<GameObject>(0);
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i]);
				}
			}
		}

		// Token: 0x06005E86 RID: 24198 RVA: 0x001E50F4 File Offset: 0x001E32F4
		public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
		{
			this.UnlockItem(itemID, false);
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			offlineVRRig.concatStringOfCosmeticsAllowed += itemID;
			this.UpdateMyCosmetics();
			if (autoEquip)
			{
				CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
				List<CosmeticsController.CosmeticSlots> list = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
				if (list.Capacity < 16)
				{
					list.Capacity = 16;
				}
				this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, list);
				foreach (CosmeticsController.CosmeticSlots cosmeticSlots in list)
				{
					this.tryOnSet.items[(int)cosmeticSlots] = this.nullItem;
				}
				CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(list);
				this.UpdateShoppingCart();
				this.UpdateWornCosmetics(true);
				Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
				if (onCosmeticsUpdated == null)
				{
					return;
				}
				onCosmeticsUpdated.Invoke();
			}
		}

		// Token: 0x06005E87 RID: 24199 RVA: 0x001E51E8 File Offset: 0x001E33E8
		public void AddTempUnlockToWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, false);
				}
				else
				{
					this.ModifyUnlockList(this.unlockedThrowables, num, false);
				}
				break;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, false);
				break;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, false);
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string cosmeticID2 in this.allCosmetics[num].bundledItems)
				{
					this.AddTempUnlockToWardrobe(cosmeticID2);
				}
				break;
			}
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated.Invoke();
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x001E539C File Offset: 0x001E359C
		public void RemoveTempUnlockFromWardrobe(string cosmeticID)
		{
			int num = this.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => cosmeticID == x.itemName);
			if (num < 0)
			{
				return;
			}
			switch (this.allCosmetics[num].itemCategory)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				this.ModifyUnlockList(this.unlockedHats, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Badge:
				this.ModifyUnlockList(this.unlockedBadges, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Face:
				this.ModifyUnlockList(this.unlockedFaces, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!this.allCosmetics[num].isThrowable)
				{
					this.ModifyUnlockList(this.unlockedPaws, num, true);
				}
				else
				{
					this.ModifyUnlockList(this.unlockedThrowables, num, true);
				}
				break;
			case CosmeticsController.CosmeticCategory.Chest:
				this.ModifyUnlockList(this.unlockedChests, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Fur:
				this.ModifyUnlockList(this.unlockedFurs, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Shirt:
				this.ModifyUnlockList(this.unlockedShirts, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Back:
				this.ModifyUnlockList(this.unlockedBacks, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Arms:
				this.ModifyUnlockList(this.unlockedArms, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Pants:
				this.ModifyUnlockList(this.unlockedPants, num, true);
				break;
			case CosmeticsController.CosmeticCategory.TagEffect:
				this.ModifyUnlockList(this.unlockedTagFX, num, true);
				break;
			case CosmeticsController.CosmeticCategory.Set:
				foreach (string cosmeticID2 in this.allCosmetics[num].bundledItems)
				{
					this.RemoveTempUnlockFromWardrobe(cosmeticID2);
				}
				break;
			}
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated.Invoke();
		}

		// Token: 0x06005E89 RID: 24201 RVA: 0x001E5550 File Offset: 0x001E3750
		public bool BuildValidationCheck()
		{
			if (this.m_earlyAccessSupporterPackCosmeticSO == null)
			{
				Debug.LogError("m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
				return false;
			}
			return true;
		}

		// Token: 0x06005E8A RID: 24202 RVA: 0x001E556D File Offset: 0x001E376D
		public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
		{
			if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
			{
				return;
			}
			this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
			GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
			this.UpdateWornCosmetics(true);
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x001E559C File Offset: 0x001E379C
		public bool ValidatePackedItems(int[] packed)
		{
			if (packed.Length == 0)
			{
				return true;
			}
			int num = 0;
			int num2 = packed[0];
			for (int i = 0; i < 16; i++)
			{
				if ((num2 & 1 << i) != 0)
				{
					num++;
				}
			}
			return packed.Length == num + 1;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x001E55D8 File Offset: 0x001E37D8
		public void SetValidatedCreatorCode(NexusManager.MemberCode memberCode)
		{
			this.validatedCreatorCode = memberCode;
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x06005E8D RID: 24205 RVA: 0x001E55E1 File Offset: 0x001E37E1
		public static int SelectedOutfit
		{
			get
			{
				return CosmeticsController.selectedOutfit;
			}
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x001E55E8 File Offset: 0x001E37E8
		public static bool CanScrollOutfits()
		{
			return CosmeticsController.loadedSavedOutfits && !CosmeticsController.saveOutfitInProgress;
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x001E55FC File Offset: 0x001E37FC
		public void PressWardrobeScrollOutfit(bool forward)
		{
			int num = CosmeticsController.selectedOutfit;
			if (forward)
			{
				num = (num + 1) % this.outfitSystemConfig.maxOutfits;
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = this.outfitSystemConfig.maxOutfits - 1;
				}
			}
			this.LoadSavedOutfit(num);
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x001E5644 File Offset: 0x001E3844
		public void LoadSavedOutfit(int newOutfitIndex)
		{
			if (!CosmeticsController.CanScrollOutfits() || newOutfitIndex == CosmeticsController.selectedOutfit || newOutfitIndex < 0 || newOutfitIndex >= this.outfitSystemConfig.maxOutfits)
			{
				return;
			}
			this.savedOutfits[CosmeticsController.selectedOutfit].CopyItems(this.currentWornSet);
			this.savedColors[CosmeticsController.selectedOutfit] = new Vector3(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b);
			this.SaveOutfitsToMothership();
			CosmeticsController.selectedOutfit = newOutfitIndex;
			PlayerPrefs.SetInt(this.outfitSystemConfig.selectedOutfitPref, CosmeticsController.selectedOutfit);
			PlayerPrefs.Save();
			CosmeticsController.CosmeticSet outfit = this.savedOutfits[CosmeticsController.selectedOutfit];
			bool flag = true;
			for (int i = 0; i < 16; i++)
			{
				CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots)i;
				if ((cosmeticSlots != CosmeticsController.CosmeticSlots.ArmLeft && cosmeticSlots != CosmeticsController.CosmeticSlots.ArmRight) || flag)
				{
					this.ApplyNewItem(outfit, i);
				}
			}
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			this.SaveCurrentItemPreferences();
			this.UpdateShoppingCart();
			this.UpdateWornCosmetics(true, true);
			this.UpdateWardrobeModelsAndButtons();
			Action onCosmeticsUpdated = this.OnCosmeticsUpdated;
			if (onCosmeticsUpdated == null)
			{
				return;
			}
			onCosmeticsUpdated.Invoke();
		}

		// Token: 0x06005E91 RID: 24209 RVA: 0x001E576C File Offset: 0x001E396C
		private void ApplyNewItem(CosmeticsController.CosmeticSet outfit, int i)
		{
			this.currentWornSet.items[i] = outfit.items[i];
			if (!outfit.items[i].isNullItem)
			{
				this.tryOnSet.items[i] = this.nullItem;
			}
		}

		// Token: 0x06005E92 RID: 24210 RVA: 0x001E57C0 File Offset: 0x001E39C0
		private void LoadSavedOutfits()
		{
			if (CosmeticsController.loadedSavedOutfits || CosmeticsController.loadOutfitsInProgress)
			{
				return;
			}
			CosmeticsController.loadOutfitsInProgress = true;
			this.savedOutfits = new CosmeticsController.CosmeticSet[this.outfitSystemConfig.maxOutfits];
			this.savedColors = new Vector3[this.outfitSystemConfig.maxOutfits];
			if (!MothershipClientApiUnity.GetUserDataValue(this.outfitSystemConfig.mothershipKey, new Action<MothershipUserData>(this.GetSavedOutfitsSuccess), new Action<MothershipError, int>(this.GetSavedOutfitsFail), ""))
			{
				GTDev.LogError<string>("CosmeticsController LoadSavedOutfits GetUserDataValue failed", null);
				this.ClearOutfits();
				CosmeticsController.loadOutfitsInProgress = false;
				CosmeticsController.loadedSavedOutfits = true;
				Action onOutfitsUpdated = this.OnOutfitsUpdated;
				if (onOutfitsUpdated == null)
				{
					return;
				}
				onOutfitsUpdated.Invoke();
			}
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x001E586C File Offset: 0x001E3A6C
		private void GetSavedOutfitsSuccess(MothershipUserData response)
		{
			if (response != null && response.value != null && response.value.Length > 0)
			{
				try
				{
					byte[] array = Convert.FromBase64String(response.value);
					this.outfitStringMothership = Encoding.UTF8.GetString(array);
					this.StringToOutfits(this.outfitStringMothership);
					goto IL_6E;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("CosmeticsController GetSavedOutfitsSuccess error decoding " + ex.Message, null);
					this.ClearOutfits();
					goto IL_6E;
				}
			}
			this.ClearOutfits();
			IL_6E:
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x001E5900 File Offset: 0x001E3B00
		private void GetSavedOutfitsFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController GetSavedOutfitsFail {0} {1}", status, error.Message), null);
			this.ClearOutfits();
			this.GetSavedOutfitsComplete();
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x001E592C File Offset: 0x001E3B2C
		private void GetSavedOutfitsComplete()
		{
			int num = PlayerPrefs.GetInt(this.outfitSystemConfig.selectedOutfitPref, 0);
			if (num < 0 || num >= this.outfitSystemConfig.maxOutfits)
			{
				num = 0;
			}
			else
			{
				CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet();
				cosmeticSet.LoadFromPlayerPreferences(this);
				if (cosmeticSet.HasAnyItems())
				{
					this.savedOutfits[num].CopyItems(cosmeticSet);
				}
				float @float = PlayerPrefs.GetFloat("redValue", 0f);
				float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
				float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
				if (@float > 0f || float2 > 0f || float3 > 0f)
				{
					this.savedColors[num] = new Vector3(@float, float2, float3);
				}
			}
			CosmeticsController.selectedOutfit = num;
			this.currentWornSet.CopyItems(this.savedOutfits[CosmeticsController.selectedOutfit]);
			this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
			CosmeticsController.loadedSavedOutfits = true;
			CosmeticsController.loadOutfitsInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated == null)
			{
				return;
			}
			onOutfitsUpdated.Invoke();
		}

		// Token: 0x06005E96 RID: 24214 RVA: 0x001E5A38 File Offset: 0x001E3C38
		private void UpdateMonkeColor(Vector3 col, bool saveToPrefs)
		{
			float num = Mathf.Clamp(col.x, 0f, 1f);
			float num2 = Mathf.Clamp(col.y, 0f, 1f);
			float num3 = Mathf.Clamp(col.z, 0f, 1f);
			GorillaTagger.Instance.UpdateColor(num, num2, num3);
			GorillaComputer.instance.UpdateColor(num, num2, num3);
			if (CosmeticsController.OnPlayerColorSet != null)
			{
				CosmeticsController.OnPlayerColorSet.Invoke(num, num2, num3);
			}
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
				{
					num,
					num2,
					num3
				});
			}
			if (saveToPrefs)
			{
				PlayerPrefs.SetFloat("redValue", num);
				PlayerPrefs.SetFloat("greenValue", num2);
				PlayerPrefs.SetFloat("blueValue", num3);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x06005E97 RID: 24215 RVA: 0x001E5B24 File Offset: 0x001E3D24
		private void SaveOutfitsToMothership()
		{
			if (!CosmeticsController.loadedSavedOutfits || CosmeticsController.saveOutfitInProgress)
			{
				return;
			}
			string mothershipKey = this.outfitSystemConfig.mothershipKey;
			this.outfitStringPendingSave = this.OutfitsToString();
			if (this.outfitStringPendingSave.Equals(this.outfitStringMothership))
			{
				return;
			}
			CosmeticsController.saveOutfitInProgress = true;
			if (!MothershipClientApiUnity.SetUserDataValue(mothershipKey, this.outfitStringPendingSave, new Action<SetUserDataResponse>(this.SaveOutfitsToMothershipSuccess), new Action<MothershipError, int>(this.SaveOutfitsToMothershipFail), ""))
			{
				GTDev.LogError<string>("CosmeticsController SaveOutfitToMothership SetUserDataValue failed", null);
				CosmeticsController.saveOutfitInProgress = false;
			}
		}

		// Token: 0x06005E98 RID: 24216 RVA: 0x001E5BAE File Offset: 0x001E3DAE
		private void SaveOutfitsToMothershipSuccess(SetUserDataResponse response)
		{
			this.outfitStringMothership = this.outfitStringPendingSave;
			CosmeticsController.saveOutfitInProgress = false;
			Action onOutfitsUpdated = this.OnOutfitsUpdated;
			if (onOutfitsUpdated != null)
			{
				onOutfitsUpdated.Invoke();
			}
			response.Dispose();
		}

		// Token: 0x06005E99 RID: 24217 RVA: 0x001E5BD9 File Offset: 0x001E3DD9
		private void SaveOutfitsToMothershipFail(MothershipError error, int status)
		{
			GTDev.LogError<string>(string.Format("CosmeticsController SaveOutfitsToMothershipFail {0} ", status) + error.Message, null);
			CosmeticsController.saveOutfitInProgress = false;
		}

		// Token: 0x06005E9A RID: 24218 RVA: 0x001E5C04 File Offset: 0x001E3E04
		private string OutfitsToString()
		{
			if (!CosmeticsController.loadedSavedOutfits)
			{
				return string.Empty;
			}
			CosmeticsController.outfitDataTemp = new CosmeticsController.OutfitData();
			this.sb.Clear();
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				CosmeticsController.outfitDataTemp.Clear();
				CosmeticsController.CosmeticSet cosmeticSet = this.savedOutfits[i];
				for (int j = 0; j < cosmeticSet.items.Length; j++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = cosmeticSet.items[j];
					string text = (cosmeticItem.isNullItem || string.IsNullOrEmpty(cosmeticItem.displayName)) ? "null" : cosmeticItem.displayName;
					CosmeticsController.outfitDataTemp.itemIDs.Add(text);
				}
				if (VRRig.LocalRig != null)
				{
					CosmeticsController.outfitDataTemp.color = this.savedColors[i];
				}
				this.sb.Append(JsonUtility.ToJson(CosmeticsController.outfitDataTemp));
				if (i < this.savedOutfits.Length - 1)
				{
					this.sb.Append(this.outfitSystemConfig.outfitSeparator);
				}
			}
			return this.sb.ToString();
		}

		// Token: 0x06005E9B RID: 24219 RVA: 0x001E5D20 File Offset: 0x001E3F20
		private void ClearOutfits()
		{
			for (int i = 0; i < this.savedOutfits.Length; i++)
			{
				this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
				this.savedOutfits[i].ClearSet(this.nullItem);
				this.savedColors[i] = CosmeticsController.defaultColor;
			}
		}

		// Token: 0x06005E9C RID: 24220 RVA: 0x001E5D74 File Offset: 0x001E3F74
		private void StringToOutfits(string response)
		{
			if (response.IsNullOrEmpty())
			{
				this.ClearOutfits();
				return;
			}
			try
			{
				string[] array = response.Split(this.outfitSystemConfig.outfitSeparator, 0);
				for (int i = 0; i < this.outfitSystemConfig.maxOutfits; i++)
				{
					this.savedOutfits[i] = new CosmeticsController.CosmeticSet();
					if (i >= array.Length)
					{
						this.savedOutfits[i].ClearSet(this.nullItem);
						this.savedColors[i] = CosmeticsController.defaultColor;
					}
					else
					{
						string text = array[i];
						if (text.IsNullOrEmpty())
						{
							this.savedOutfits[i].ClearSet(this.nullItem);
							this.savedColors[i] = CosmeticsController.defaultColor;
						}
						else
						{
							Vector3 vector;
							this.savedOutfits[i].ParseSetFromString(this, text, out vector);
							this.savedColors[i] = vector;
						}
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("CosmeticsController StringToOutfit Error parsing " + ex.Message, null);
				this.ClearOutfits();
			}
		}

		// Token: 0x04006C03 RID: 27651
		[FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
		[FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
		[SerializeField]
		public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;

		// Token: 0x04006C05 RID: 27653
		private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();

		// Token: 0x04006C06 RID: 27654
		public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;

		// Token: 0x04006C0A RID: 27658
		public const int maximumTransferrableItems = 5;

		// Token: 0x04006C0B RID: 27659
		[OnEnterPlay_SetNull]
		public static volatile CosmeticsController instance;

		// Token: 0x04006C0D RID: 27661
		public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;

		// Token: 0x04006C0E RID: 27662
		public Action OnGetCurrency;

		// Token: 0x04006C0F RID: 27663
		[FormerlySerializedAs("allCosmetics")]
		[SerializeField]
		private List<CosmeticsController.CosmeticItem> _allCosmetics;

		// Token: 0x04006C11 RID: 27665
		public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04006C13 RID: 27667
		public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048);

		// Token: 0x04006C14 RID: 27668
		public CosmeticsController.CosmeticItem nullItem;

		// Token: 0x04006C15 RID: 27669
		public string catalog;

		// Token: 0x04006C16 RID: 27670
		private string[] tempStringArray;

		// Token: 0x04006C17 RID: 27671
		private CosmeticsController.CosmeticItem tempItem;

		// Token: 0x04006C18 RID: 27672
		private VRRigAnchorOverrides anchorOverrides;

		// Token: 0x04006C19 RID: 27673
		public List<CatalogItem> catalogItems;

		// Token: 0x04006C1A RID: 27674
		public bool tryTwice;

		// Token: 0x04006C1B RID: 27675
		public CustomMapCosmeticsData customMapCosmeticsData;

		// Token: 0x04006C1C RID: 27676
		[NonSerialized]
		public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04006C1D RID: 27677
		public int numFittingRoomButtons = 12;

		// Token: 0x04006C1E RID: 27678
		public List<FittingRoom> fittingRooms = new List<FittingRoom>();

		// Token: 0x04006C1F RID: 27679
		public CosmeticStand[] cosmeticStands;

		// Token: 0x04006C20 RID: 27680
		public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();

		// Token: 0x04006C21 RID: 27681
		public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

		// Token: 0x04006C22 RID: 27682
		public List<ItemCheckout> itemCheckouts = new List<ItemCheckout>();

		// Token: 0x04006C23 RID: 27683
		public CosmeticsController.CosmeticItem itemToBuy;

		// Token: 0x04006C24 RID: 27684
		private List<string> playerIDList = new List<string>();

		// Token: 0x04006C25 RID: 27685
		private List<string> inventoryStringList = new List<string>();

		// Token: 0x04006C26 RID: 27686
		private bool foundCosmetic;

		// Token: 0x04006C27 RID: 27687
		private int attempts;

		// Token: 0x04006C28 RID: 27688
		private string finalLine;

		// Token: 0x04006C29 RID: 27689
		private string leftCheckoutPurchaseButtonString;

		// Token: 0x04006C2A RID: 27690
		private string rightCheckoutPurchaseButtonString;

		// Token: 0x04006C2B RID: 27691
		private bool leftCheckoutPurchaseButtonOn;

		// Token: 0x04006C2C RID: 27692
		private bool rightCheckoutPurchaseButtonOn;

		// Token: 0x04006C2D RID: 27693
		private bool isLastHandTouchedLeft;

		// Token: 0x04006C2E RID: 27694
		private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04006C30 RID: 27696
		public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();

		// Token: 0x04006C31 RID: 27697
		public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048);

		// Token: 0x04006C32 RID: 27698
		public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C33 RID: 27699
		public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C34 RID: 27700
		public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C35 RID: 27701
		public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C36 RID: 27702
		public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C37 RID: 27703
		public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C38 RID: 27704
		public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C39 RID: 27705
		public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C3A RID: 27706
		public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C3B RID: 27707
		public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C3C RID: 27708
		public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C3D RID: 27709
		public List<CosmeticsController.CosmeticItem> unlockedThrowables = new List<CosmeticsController.CosmeticItem>(512);

		// Token: 0x04006C3E RID: 27710
		public int[] cosmeticsPages = new int[11];

		// Token: 0x04006C3F RID: 27711
		private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];

		// Token: 0x04006C40 RID: 27712
		private int wardrobeType;

		// Token: 0x04006C41 RID: 27713
		[NonSerialized]
		public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04006C42 RID: 27714
		[NonSerialized]
		public CosmeticsController.CosmeticSet tempUnlockedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04006C43 RID: 27715
		[NonSerialized]
		public CosmeticsController.CosmeticSet activeMergedSet = new CosmeticsController.CosmeticSet();

		// Token: 0x04006C44 RID: 27716
		public string concatStringCosmeticsAllowed = "";

		// Token: 0x04006C45 RID: 27717
		public Action OnCosmeticsUpdated;

		// Token: 0x04006C46 RID: 27718
		public int currencyBalance;

		// Token: 0x04006C47 RID: 27719
		public string currencyName;

		// Token: 0x04006C48 RID: 27720
		public List<CurrencyBoard> currencyBoards;

		// Token: 0x04006C49 RID: 27721
		public string itemToPurchase;

		// Token: 0x04006C4A RID: 27722
		public bool buyingBundle;

		// Token: 0x04006C4B RID: 27723
		public bool confirmedDidntPlayInBeta;

		// Token: 0x04006C4C RID: 27724
		public bool playedInBeta;

		// Token: 0x04006C4D RID: 27725
		public bool gotMyDaily;

		// Token: 0x04006C4E RID: 27726
		public bool checkedDaily;

		// Token: 0x04006C4F RID: 27727
		public string currentPurchaseID;

		// Token: 0x04006C50 RID: 27728
		public bool hasPrice;

		// Token: 0x04006C51 RID: 27729
		private int searchIndex;

		// Token: 0x04006C52 RID: 27730
		private int iterator;

		// Token: 0x04006C53 RID: 27731
		private CosmeticsController.CosmeticItem cosmeticItemVar;

		// Token: 0x04006C54 RID: 27732
		[SerializeField]
		private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;

		// Token: 0x04006C55 RID: 27733
		public EarlyAccessButton[] earlyAccessButtons;

		// Token: 0x04006C56 RID: 27734
		private BundleList bundleList = new BundleList();

		// Token: 0x04006C57 RID: 27735
		public string BundleSkuName = "2024_i_lava_you_pack";

		// Token: 0x04006C58 RID: 27736
		public string BundlePlayfabItemName = "LSABG.";

		// Token: 0x04006C59 RID: 27737
		public int BundleShinyRocks = 10000;

		// Token: 0x04006C5A RID: 27738
		public DateTime currentTime;

		// Token: 0x04006C5B RID: 27739
		public string lastDailyLogin;

		// Token: 0x04006C5C RID: 27740
		public UserDataRecord userDataRecord;

		// Token: 0x04006C5D RID: 27741
		public int secondsUntilTomorrow;

		// Token: 0x04006C5E RID: 27742
		public float secondsToWaitToCheckDaily = 10f;

		// Token: 0x04006C5F RID: 27743
		private int updateCosmeticsRetries;

		// Token: 0x04006C60 RID: 27744
		private int maxUpdateCosmeticsRetries;

		// Token: 0x04006C61 RID: 27745
		private GetUserInventoryResult latestInventory;

		// Token: 0x04006C62 RID: 27746
		private string returnString;

		// Token: 0x04006C63 RID: 27747
		private bool checkoutCartButtonPressedWithLeft;

		// Token: 0x04006C64 RID: 27748
		private NexusManager.MemberCode validatedCreatorCode;

		// Token: 0x04006C65 RID: 27749
		private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;

		// Token: 0x04006C66 RID: 27750
		private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16);

		// Token: 0x04006C67 RID: 27751
		[SerializeField]
		private CosmeticOutfitSystemConfig outfitSystemConfig;

		// Token: 0x04006C68 RID: 27752
		private CosmeticsController.CosmeticSet[] savedOutfits;

		// Token: 0x04006C69 RID: 27753
		private Vector3[] savedColors;

		// Token: 0x04006C6A RID: 27754
		private static CosmeticsController.OutfitData outfitDataTemp;

		// Token: 0x04006C6B RID: 27755
		private string outfitStringMothership = string.Empty;

		// Token: 0x04006C6C RID: 27756
		private string outfitStringPendingSave = string.Empty;

		// Token: 0x04006C6D RID: 27757
		private static bool saveOutfitInProgress = false;

		// Token: 0x04006C6E RID: 27758
		private static bool loadOutfitsInProgress = false;

		// Token: 0x04006C6F RID: 27759
		private static bool loadedSavedOutfits = false;

		// Token: 0x04006C70 RID: 27760
		private static int selectedOutfit = 0;

		// Token: 0x04006C71 RID: 27761
		private static readonly Vector3 defaultColor = new Vector3(0f, 0f, 0f);

		// Token: 0x04006C72 RID: 27762
		public Action OnOutfitsUpdated;

		// Token: 0x04006C73 RID: 27763
		public static Action<float, float, float> OnPlayerColorSet;

		// Token: 0x04006C74 RID: 27764
		private StringBuilder sb = new StringBuilder(256);

		// Token: 0x02000EBE RID: 3774
		public enum PurchaseItemStages
		{
			// Token: 0x04006C76 RID: 27766
			Start,
			// Token: 0x04006C77 RID: 27767
			CheckoutButtonPressed,
			// Token: 0x04006C78 RID: 27768
			ItemSelected,
			// Token: 0x04006C79 RID: 27769
			ItemOwned,
			// Token: 0x04006C7A RID: 27770
			FinalPurchaseAcknowledgement,
			// Token: 0x04006C7B RID: 27771
			Buying,
			// Token: 0x04006C7C RID: 27772
			Success,
			// Token: 0x04006C7D RID: 27773
			Failure
		}

		// Token: 0x02000EBF RID: 3775
		public enum CosmeticCategory
		{
			// Token: 0x04006C7F RID: 27775
			None,
			// Token: 0x04006C80 RID: 27776
			Hat,
			// Token: 0x04006C81 RID: 27777
			Badge,
			// Token: 0x04006C82 RID: 27778
			Face,
			// Token: 0x04006C83 RID: 27779
			Paw,
			// Token: 0x04006C84 RID: 27780
			Chest,
			// Token: 0x04006C85 RID: 27781
			Fur,
			// Token: 0x04006C86 RID: 27782
			Shirt,
			// Token: 0x04006C87 RID: 27783
			Back,
			// Token: 0x04006C88 RID: 27784
			Arms,
			// Token: 0x04006C89 RID: 27785
			Pants,
			// Token: 0x04006C8A RID: 27786
			TagEffect,
			// Token: 0x04006C8B RID: 27787
			Count,
			// Token: 0x04006C8C RID: 27788
			Set
		}

		// Token: 0x02000EC0 RID: 3776
		public enum CosmeticSlots
		{
			// Token: 0x04006C8E RID: 27790
			Hat,
			// Token: 0x04006C8F RID: 27791
			Badge,
			// Token: 0x04006C90 RID: 27792
			Face,
			// Token: 0x04006C91 RID: 27793
			ArmLeft,
			// Token: 0x04006C92 RID: 27794
			ArmRight,
			// Token: 0x04006C93 RID: 27795
			BackLeft,
			// Token: 0x04006C94 RID: 27796
			BackRight,
			// Token: 0x04006C95 RID: 27797
			HandLeft,
			// Token: 0x04006C96 RID: 27798
			HandRight,
			// Token: 0x04006C97 RID: 27799
			Chest,
			// Token: 0x04006C98 RID: 27800
			Fur,
			// Token: 0x04006C99 RID: 27801
			Shirt,
			// Token: 0x04006C9A RID: 27802
			Pants,
			// Token: 0x04006C9B RID: 27803
			Back,
			// Token: 0x04006C9C RID: 27804
			Arms,
			// Token: 0x04006C9D RID: 27805
			TagEffect,
			// Token: 0x04006C9E RID: 27806
			Count
		}

		// Token: 0x02000EC1 RID: 3777
		[Serializable]
		public class CosmeticSet
		{
			// Token: 0x140000AD RID: 173
			// (add) Token: 0x06005EAC RID: 24236 RVA: 0x001E64D4 File Offset: 0x001E46D4
			// (remove) Token: 0x06005EAD RID: 24237 RVA: 0x001E650C File Offset: 0x001E470C
			public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

			// Token: 0x06005EAE RID: 24238 RVA: 0x001E6541 File Offset: 0x001E4741
			protected void OnSetActivated(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer)
			{
				if (this.onSetActivatedEvent != null)
				{
					this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
				}
			}

			// Token: 0x170008C7 RID: 2247
			// (get) Token: 0x06005EAF RID: 24239 RVA: 0x001E655C File Offset: 0x001E475C
			public static CosmeticsController.CosmeticSet EmptySet
			{
				get
				{
					if (CosmeticsController.CosmeticSet._emptySet == null)
					{
						string[] array = new string[16];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = "NOTHING";
						}
						CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(array, CosmeticsController.instance);
					}
					return CosmeticsController.CosmeticSet._emptySet;
				}
			}

			// Token: 0x06005EB0 RID: 24240 RVA: 0x001E65A5 File Offset: 0x001E47A5
			public CosmeticSet()
			{
				this.items = new CosmeticsController.CosmeticItem[16];
			}

			// Token: 0x06005EB1 RID: 24241 RVA: 0x001E65C8 File Offset: 0x001E47C8
			public CosmeticSet(string[] itemNames, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				for (int i = 0; i < itemNames.Length; i++)
				{
					string displayName = itemNames[i];
					string itemNameFromDisplayName = controller.GetItemNameFromDisplayName(displayName);
					this.items[i] = controller.GetItemFromDict(itemNameFromDisplayName);
				}
			}

			// Token: 0x06005EB2 RID: 24242 RVA: 0x001E6624 File Offset: 0x001E4824
			public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
			{
				this.items = new CosmeticsController.CosmeticItem[16];
				int num = (itemNamesPacked.Length != 0) ? itemNamesPacked[0] : 0;
				int num2 = 1;
				for (int i = 0; i < this.items.Length; i++)
				{
					if ((num & 1 << i) != 0)
					{
						int num3 = itemNamesPacked[num2];
						CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char)(65 + num3 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char)(65 + num3 / 26 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char)(65 + num3 / 676 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char)(65 + num3 / 17576 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char)(65 + num3 / 456976 % 26);
						CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
						this.items[i] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
						num2++;
					}
					else
					{
						this.items[i] = controller.GetItemFromDict("null");
					}
				}
			}

			// Token: 0x06005EB3 RID: 24243 RVA: 0x001E672C File Offset: 0x001E492C
			public void CopyItems(CosmeticsController.CosmeticSet other)
			{
				for (int i = 0; i < this.items.Length; i++)
				{
					this.items[i] = other.items[i];
				}
			}

			// Token: 0x06005EB4 RID: 24244 RVA: 0x001E6764 File Offset: 0x001E4964
			public void MergeSets(CosmeticsController.CosmeticSet tryOn, CosmeticsController.CosmeticSet current)
			{
				for (int i = 0; i < 16; i++)
				{
					if (tryOn == null)
					{
						this.items[i] = current.items[i];
					}
					else
					{
						this.items[i] = (tryOn.items[i].isNullItem ? current.items[i] : tryOn.items[i]);
					}
				}
			}

			// Token: 0x06005EB5 RID: 24245 RVA: 0x001E67D4 File Offset: 0x001E49D4
			public void MergeInSets(CosmeticsController.CosmeticSet playerPref, CosmeticsController.CosmeticSet tempOverrideSet, Predicate<string> predicate)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					bool flag = predicate.Invoke(tempOverrideSet.items[i].itemName);
					this.items[i] = (flag ? tempOverrideSet.items[i] : playerPref.items[i]);
				}
			}

			// Token: 0x06005EB6 RID: 24246 RVA: 0x001E6834 File Offset: 0x001E4A34
			public void ClearSet(CosmeticsController.CosmeticItem nullItem)
			{
				for (int i = 0; i < 16; i++)
				{
					this.items[i] = nullItem;
				}
			}

			// Token: 0x06005EB7 RID: 24247 RVA: 0x001E685C File Offset: 0x001E4A5C
			public bool IsActive(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06005EB8 RID: 24248 RVA: 0x001E6894 File Offset: 0x001E4A94
			public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemCategory == category)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06005EB9 RID: 24249 RVA: 0x001E68DC File Offset: 0x001E4ADC
			public bool HasItem(string name)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].displayName == name)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06005EBA RID: 24250 RVA: 0x001E6928 File Offset: 0x001E4B28
			public bool HasAnyItems()
			{
				if (this.items == null || this.items.Length < 1)
				{
					return false;
				}
				for (int i = 0; i < this.items.Length; i++)
				{
					if (!this.items[i].isNullItem)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06005EBB RID: 24251 RVA: 0x001E6973 File Offset: 0x001E4B73
			public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
			}

			// Token: 0x06005EBC RID: 24252 RVA: 0x001E6983 File Offset: 0x001E4B83
			public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
			{
				return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
			}

			// Token: 0x06005EBD RID: 24253 RVA: 0x001E6993 File Offset: 0x001E4B93
			public static bool IsHoldable(CosmeticsController.CosmeticItem item)
			{
				return item.isHoldable;
			}

			// Token: 0x06005EBE RID: 24254 RVA: 0x001E699C File Offset: 0x001E4B9C
			public static CosmeticsController.CosmeticSlots OppositeSlot(CosmeticsController.CosmeticSlots slot)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Hat:
					return CosmeticsController.CosmeticSlots.Hat;
				case CosmeticsController.CosmeticSlots.Badge:
					return CosmeticsController.CosmeticSlots.Badge;
				case CosmeticsController.CosmeticSlots.Face:
					return CosmeticsController.CosmeticSlots.Face;
				case CosmeticsController.CosmeticSlots.ArmLeft:
					return CosmeticsController.CosmeticSlots.ArmRight;
				case CosmeticsController.CosmeticSlots.ArmRight:
					return CosmeticsController.CosmeticSlots.ArmLeft;
				case CosmeticsController.CosmeticSlots.BackLeft:
					return CosmeticsController.CosmeticSlots.BackRight;
				case CosmeticsController.CosmeticSlots.BackRight:
					return CosmeticsController.CosmeticSlots.BackLeft;
				case CosmeticsController.CosmeticSlots.HandLeft:
					return CosmeticsController.CosmeticSlots.HandRight;
				case CosmeticsController.CosmeticSlots.HandRight:
					return CosmeticsController.CosmeticSlots.HandLeft;
				case CosmeticsController.CosmeticSlots.Chest:
					return CosmeticsController.CosmeticSlots.Chest;
				case CosmeticsController.CosmeticSlots.Fur:
					return CosmeticsController.CosmeticSlots.Fur;
				case CosmeticsController.CosmeticSlots.Shirt:
					return CosmeticsController.CosmeticSlots.Shirt;
				case CosmeticsController.CosmeticSlots.Pants:
					return CosmeticsController.CosmeticSlots.Pants;
				case CosmeticsController.CosmeticSlots.Back:
					return CosmeticsController.CosmeticSlots.Back;
				case CosmeticsController.CosmeticSlots.Arms:
					return CosmeticsController.CosmeticSlots.Arms;
				case CosmeticsController.CosmeticSlots.TagEffect:
					return CosmeticsController.CosmeticSlots.TagEffect;
				default:
					return CosmeticsController.CosmeticSlots.Count;
				}
			}

			// Token: 0x06005EBF RID: 24255 RVA: 0x001E6A1A File Offset: 0x001E4C1A
			public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
			{
				return "slot_" + slot.ToString();
			}

			// Token: 0x06005EC0 RID: 24256 RVA: 0x001E6A34 File Offset: 0x001E4C34
			private void ActivateCosmetic(CosmeticsController.CosmeticSet prevSet, VRRig rig, int slotIndex, CosmeticItemRegistry cosmeticsObjectRegistry, BodyDockPositions bDock)
			{
				CosmeticsController.CosmeticItem cosmeticItem = prevSet.items[slotIndex];
				string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem.displayName);
				CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
				string itemNameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
				BodyDockPositions.DropPositions dropPositions = CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)slotIndex);
				if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, (CosmeticsController.CosmeticSlots)slotIndex))
				{
					return;
				}
				if (cosmeticItem2.isHoldable && dropPositions == BodyDockPositions.DropPositions.None)
				{
					return;
				}
				if (!(itemNameFromDisplayName == itemNameFromDisplayName2))
				{
					if (!cosmeticItem.isNullItem)
					{
						if (cosmeticItem.isHoldable)
						{
							bDock.TransferrableItemDisableAtPosition(dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						}
					}
					if (!cosmeticItem2.isNullItem)
					{
						if (cosmeticItem2.isHoldable)
						{
							bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPositions);
						}
						CosmeticItemInstance cosmeticItemInstance2 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
						if (rig.IsItemAllowed(itemNameFromDisplayName2) && cosmeticItemInstance2 != null)
						{
							cosmeticItemInstance2.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
							if (rig.isLocal && (slotIndex == 0 || slotIndex == 2))
							{
								PlayerPrefFlags.TouchIf(PlayerPrefFlags.Flag.SHOW_1P_COSMETICS, false);
							}
						}
					}
					return;
				}
				if (cosmeticItem2.isNullItem)
				{
					return;
				}
				CosmeticItemInstance cosmeticItemInstance3 = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
				if (cosmeticItemInstance3 != null)
				{
					if (!rig.IsItemAllowed(itemNameFromDisplayName2))
					{
						cosmeticItemInstance3.DisableItem((CosmeticsController.CosmeticSlots)slotIndex);
						return;
					}
					cosmeticItemInstance3.EnableItem((CosmeticsController.CosmeticSlots)slotIndex, rig);
				}
			}

			// Token: 0x06005EC1 RID: 24257 RVA: 0x001E6B88 File Offset: 0x001E4D88
			public void ActivateCosmetics(CosmeticsController.CosmeticSet prevSet, VRRig rig, BodyDockPositions bDock, CosmeticItemRegistry cosmeticsObjectRegistry)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.ActivateCosmetic(prevSet, rig, i, cosmeticsObjectRegistry, bDock);
				}
				this.OnSetActivated(prevSet, this, rig.creator);
			}

			// Token: 0x06005EC2 RID: 24258 RVA: 0x001E6BC0 File Offset: 0x001E4DC0
			public void DeactivateAllCosmetcs(BodyDockPositions bDock, CosmeticsController.CosmeticItem nullItem, CosmeticItemRegistry cosmeticObjectRegistry)
			{
				bDock.DisableAllTransferableItems();
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticItem cosmeticItem = this.items[i];
					if (!cosmeticItem.isNullItem)
					{
						CosmeticsController.CosmeticSlots cosmeticSlot = (CosmeticsController.CosmeticSlots)i;
						CosmeticItemInstance cosmeticItemInstance = cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName);
						if (cosmeticItemInstance != null)
						{
							cosmeticItemInstance.DisableItem(cosmeticSlot);
						}
						this.items[i] = nullItem;
					}
				}
			}

			// Token: 0x06005EC3 RID: 24259 RVA: 0x001E6C20 File Offset: 0x001E4E20
			public void LoadFromPlayerPreferences(CosmeticsController controller)
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
					string @string = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
					if (@string == "null" || @string == "NOTHING")
					{
						this.items[i] = controller.nullItem;
					}
					else
					{
						CosmeticsController.CosmeticItem item = controller.GetItemFromDict(@string);
						if (item.isNullItem)
						{
							Debug.Log("LoadFromPlayerPreferences: Could not find item stored in player prefs: \"" + @string + "\"");
							this.items[i] = controller.nullItem;
						}
						else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot))
						{
							this.items[i] = controller.nullItem;
						}
						else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
						{
							this.items[i] = item;
						}
						else
						{
							this.items[i] = controller.nullItem;
						}
					}
				}
			}

			// Token: 0x06005EC4 RID: 24260 RVA: 0x001E6D3C File Offset: 0x001E4F3C
			public void ParseSetFromString(CosmeticsController controller, string setString, out Vector3 color)
			{
				color = CosmeticsController.defaultColor;
				if (setString.IsNullOrEmpty())
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController ParseSetFromString: null string", null);
					return;
				}
				int num = 16;
				CosmeticsController.OutfitData outfitData = new CosmeticsController.OutfitData();
				try
				{
					outfitData = JsonUtility.FromJson<CosmeticsController.OutfitData>(setString);
					color = outfitData.color;
				}
				catch (Exception)
				{
					char c = ',';
					if (controller.outfitSystemConfig != null)
					{
						c = controller.outfitSystemConfig.itemSeparator;
					}
					string[] array = setString.Split(c, num, 0);
					if (array == null || array.Length > num)
					{
						this.ClearSet(controller.nullItem);
						GTDev.LogError<string>(string.Format("CosmeticsController ParseSetFromString: wrong number of slots {0} {1}", array.Length, setString), null);
						return;
					}
					outfitData.Clear();
					outfitData.itemIDs = new List<string>(array);
				}
				try
				{
					for (int i = 0; i < num; i++)
					{
						CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots)i;
						string text = (i < outfitData.itemIDs.Count) ? outfitData.itemIDs[i] : "null";
						if (text.IsNullOrEmpty() || text == "null" || text == "NOTHING")
						{
							this.items[i] = controller.nullItem;
						}
						else
						{
							CosmeticsController.CosmeticItem item = controller.GetItemFromDict(text);
							if (item.isNullItem)
							{
								GTDev.Log<string>("CosmeticsController ParseSetFromString: Could not find item stored in player prefs: \"" + text + "\"", null);
								this.items[i] = controller.nullItem;
							}
							else if (!CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot))
							{
								this.items[i] = controller.nullItem;
							}
							else if (controller.unlockedCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => item.itemName == x.itemName) >= 0)
							{
								this.items[i] = item;
							}
							else
							{
								this.items[i] = controller.nullItem;
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.ClearSet(controller.nullItem);
					GTDev.LogError<string>("CosmeticsController: Issue parsing saved outfit string: " + ex.Message, null);
				}
			}

			// Token: 0x06005EC5 RID: 24261 RVA: 0x001E6F9C File Offset: 0x001E519C
			public string[] ToDisplayNameArray()
			{
				int num = 16;
				for (int i = 0; i < num; i++)
				{
					this.returnArray[i] = (string.IsNullOrEmpty(this.items[i].displayName) ? "null" : this.items[i].displayName);
				}
				return this.returnArray;
			}

			// Token: 0x06005EC6 RID: 24262 RVA: 0x001E6FF8 File Offset: 0x001E51F8
			public int[] ToPackedIDArray()
			{
				int num = 0;
				int num2 = 0;
				int num3 = 16;
				for (int i = 0; i < num3; i++)
				{
					if (!this.items[i].isNullItem && this.items[i].itemName.Length == 6)
					{
						num |= 1 << i;
						num2++;
					}
				}
				if (num == 0)
				{
					return CosmeticsController.CosmeticSet.intArrays[0];
				}
				int[] array = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
				array[0] = num;
				int num4 = 1;
				for (int j = 0; j < num3; j++)
				{
					if ((num & 1 << j) != 0)
					{
						string itemName = this.items[j].itemName;
						array[num4] = (int)(itemName.get_Chars(0) - 'A' + '\u001a' * (itemName.get_Chars(1) - 'A' + '\u001a' * (itemName.get_Chars(2) - 'A' + '\u001a' * (itemName.get_Chars(3) - 'A' + '\u001a' * (itemName.get_Chars(4) - 'A')))));
						num4++;
					}
				}
				return array;
			}

			// Token: 0x06005EC7 RID: 24263 RVA: 0x001E70F8 File Offset: 0x001E52F8
			public string[] HoldableDisplayNames(bool leftHoldables)
			{
				int num = 16;
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i)))
						{
							num2++;
						}
					}
				}
				if (num2 == 0)
				{
					return null;
				}
				int num3 = 0;
				string[] array = new string[num2];
				for (int j = 0; j < num; j++)
				{
					if (this.items[j].isHoldable)
					{
						if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
						else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)j)))
						{
							array[num3] = this.items[j].displayName;
							num3++;
						}
					}
				}
				return array;
			}

			// Token: 0x06005EC8 RID: 24264 RVA: 0x001E720C File Offset: 0x001E540C
			public bool[] ToOnRightSideArray()
			{
				int num = 16;
				bool[] array = new bool[num];
				for (int i = 0; i < num; i++)
				{
					if (this.items[i].isHoldable && this.items[i].itemCategory != CosmeticsController.CosmeticCategory.Chest)
					{
						array[i] = !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots)i));
					}
					else
					{
						array[i] = false;
					}
				}
				return array;
			}

			// Token: 0x04006C9F RID: 27807
			public CosmeticsController.CosmeticItem[] items;

			// Token: 0x04006CA1 RID: 27809
			public string[] returnArray = new string[16];

			// Token: 0x04006CA2 RID: 27810
			private static int[][] intArrays = new int[][]
			{
				new int[0],
				new int[1],
				new int[2],
				new int[3],
				new int[4],
				new int[5],
				new int[6],
				new int[7],
				new int[8],
				new int[9],
				new int[10],
				new int[11],
				new int[12],
				new int[13],
				new int[14],
				new int[15],
				new int[16],
				new int[17],
				new int[18],
				new int[19],
				new int[20],
				new int[21]
			};

			// Token: 0x04006CA3 RID: 27811
			private static CosmeticsController.CosmeticSet _emptySet;

			// Token: 0x04006CA4 RID: 27812
			private static char[] nameScratchSpace = new char[6];

			// Token: 0x02000EC2 RID: 3778
			// (Invoke) Token: 0x06005ECB RID: 24267
			public delegate void OnSetActivatedHandler(CosmeticsController.CosmeticSet prevSet, CosmeticsController.CosmeticSet currentSet, NetPlayer netPlayer);
		}

		// Token: 0x02000EC5 RID: 3781
		[Serializable]
		public struct CosmeticItem
		{
			// Token: 0x04006CA7 RID: 27815
			[Tooltip("Should match the spreadsheet item name.")]
			public string itemName;

			// Token: 0x04006CA8 RID: 27816
			[Tooltip("Determines what wardrobe section the item will show up in.")]
			public CosmeticsController.CosmeticCategory itemCategory;

			// Token: 0x04006CA9 RID: 27817
			[Tooltip("If this is a holdable item.")]
			public bool isHoldable;

			// Token: 0x04006CAA RID: 27818
			[Tooltip("If this is a throwable item and hidden on the wardrobe.")]
			public bool isThrowable;

			// Token: 0x04006CAB RID: 27819
			[Tooltip("Icon shown in the store menus & hunt watch.")]
			public Sprite itemPicture;

			// Token: 0x04006CAC RID: 27820
			public string displayName;

			// Token: 0x04006CAD RID: 27821
			public string itemPictureResourceString;

			// Token: 0x04006CAE RID: 27822
			[Tooltip("The name shown on the store checkout screen.")]
			public string overrideDisplayName;

			// Token: 0x04006CAF RID: 27823
			[DebugReadout]
			[NonSerialized]
			public int cost;

			// Token: 0x04006CB0 RID: 27824
			[DebugReadout]
			[NonSerialized]
			public string[] bundledItems;

			// Token: 0x04006CB1 RID: 27825
			[DebugReadout]
			[NonSerialized]
			public bool canTryOn;

			// Token: 0x04006CB2 RID: 27826
			[Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
			public bool bothHandsHoldable;

			// Token: 0x04006CB3 RID: 27827
			public bool bLoadsFromResources;

			// Token: 0x04006CB4 RID: 27828
			public bool bUsesMeshAtlas;

			// Token: 0x04006CB5 RID: 27829
			public Vector3 rotationOffset;

			// Token: 0x04006CB6 RID: 27830
			public Vector3 positionOffset;

			// Token: 0x04006CB7 RID: 27831
			public string meshAtlasResourceString;

			// Token: 0x04006CB8 RID: 27832
			public string meshResourceString;

			// Token: 0x04006CB9 RID: 27833
			public string materialResourceString;

			// Token: 0x04006CBA RID: 27834
			[HideInInspector]
			public bool isNullItem;
		}

		// Token: 0x02000EC6 RID: 3782
		[Serializable]
		public class IAPRequestBody
		{
			// Token: 0x04006CBB RID: 27835
			public string userID;

			// Token: 0x04006CBC RID: 27836
			public string nonce;

			// Token: 0x04006CBD RID: 27837
			public string platform;

			// Token: 0x04006CBE RID: 27838
			public string sku;

			// Token: 0x04006CBF RID: 27839
			public string mothershipEnvId;

			// Token: 0x04006CC0 RID: 27840
			public string mothershipDeploymentId;

			// Token: 0x04006CC1 RID: 27841
			public Dictionary<string, string> customTags;
		}

		// Token: 0x02000EC7 RID: 3783
		public enum EWearingCosmeticSet
		{
			// Token: 0x04006CC3 RID: 27843
			NotASet,
			// Token: 0x04006CC4 RID: 27844
			NotWearing,
			// Token: 0x04006CC5 RID: 27845
			Partial,
			// Token: 0x04006CC6 RID: 27846
			Complete
		}

		// Token: 0x02000EC8 RID: 3784
		public class OutfitData
		{
			// Token: 0x06005ED3 RID: 24275 RVA: 0x001E73A4 File Offset: 0x001E55A4
			public OutfitData()
			{
				this.version = 1;
				this.itemIDs = new List<string>(16);
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x06005ED4 RID: 24276 RVA: 0x001E73CB File Offset: 0x001E55CB
			public void Clear()
			{
				this.itemIDs.Clear();
				this.color = CosmeticsController.defaultColor;
			}

			// Token: 0x04006CC7 RID: 27847
			public const int OUTFIT_DATA_VERSION = 1;

			// Token: 0x04006CC8 RID: 27848
			public int version;

			// Token: 0x04006CC9 RID: 27849
			public List<string> itemIDs;

			// Token: 0x04006CCA RID: 27850
			public Vector3 color;
		}
	}
}
