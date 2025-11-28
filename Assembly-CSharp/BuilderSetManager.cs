using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005A1 RID: 1441
public class BuilderSetManager : MonoBehaviour
{
	// Token: 0x1700039F RID: 927
	// (get) Token: 0x0600245B RID: 9307 RVA: 0x000C32A7 File Offset: 0x000C14A7
	// (set) Token: 0x0600245C RID: 9308 RVA: 0x000C32AE File Offset: 0x000C14AE
	public static bool hasInstance { get; private set; }

	// Token: 0x0600245D RID: 9309 RVA: 0x000C32B8 File Offset: 0x000C14B8
	public string GetStarterSetsConcat()
	{
		if (BuilderSetManager.concatStarterSets.Length > 0)
		{
			return BuilderSetManager.concatStarterSets;
		}
		BuilderSetManager.concatStarterSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._starterPieceSets)
		{
			BuilderSetManager.concatStarterSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatStarterSets;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000C333C File Offset: 0x000C153C
	public string GetAllSetsConcat()
	{
		if (BuilderSetManager.concatAllSets.Length > 0)
		{
			return BuilderSetManager.concatAllSets;
		}
		BuilderSetManager.concatAllSets = string.Empty;
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			BuilderSetManager.concatAllSets += builderPieceSet.playfabID;
		}
		return BuilderSetManager.concatAllSets;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000C33C0 File Offset: 0x000C15C0
	public void Awake()
	{
		if (BuilderSetManager.instance == null)
		{
			BuilderSetManager.instance = this;
			BuilderSetManager.hasInstance = true;
		}
		else if (BuilderSetManager.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.Init();
		if (this.monitor == null)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000C3428 File Offset: 0x000C1628
	private void Init()
	{
		this.InitPieceDictionary();
		this.catalog = "DLC";
		this.currencyName = "SR";
		this.pulledStoreItems = false;
		BuilderSetManager._setIdToStoreItem = new Dictionary<int, BuilderSetManager.BuilderSetStoreItem>(this._allPieceSets.Count);
		BuilderSetManager._setIdToStoreItem.Clear();
		BuilderSetManager.pieceSetInfos = new List<BuilderSetManager.BuilderPieceSetInfo>(this._allPieceSets.Count * 45);
		BuilderSetManager.pieceSetInfoMap = new Dictionary<int, int>(this._allPieceSets.Count * 45);
		this.livePieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.scheduledPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this.displayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>(this._allPieceSets.Count * 2);
		this.displayGroupMap = new Dictionary<int, int>(this._allPieceSets.Count * 2);
		this.liveDisplayGroups = new List<BuilderPieceSet.BuilderDisplayGroup>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
		foreach (BuilderPieceSet builderPieceSet in this._allPieceSets)
		{
			dictionary.Clear();
			int num = 0;
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem = new BuilderSetManager.BuilderSetStoreItem
			{
				displayName = builderPieceSet.SetName,
				playfabID = builderPieceSet.playfabID,
				setID = builderPieceSet.GetIntIdentifier(),
				cost = 0U,
				setRef = builderPieceSet,
				displayModel = builderPieceSet.displayModel,
				isNullItem = false
			};
			BuilderSetManager._setIdToStoreItem.TryAdd(builderPieceSet.GetIntIdentifier(), builderSetStoreItem);
			int num2 = -1;
			if (!string.IsNullOrEmpty(builderPieceSet.materialId))
			{
				num2 = builderPieceSet.materialId.GetHashCode();
			}
			for (int i = 0; i < builderPieceSet.subsets.Count; i++)
			{
				BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[i];
				if (!builderPieceSet.setName.Equals("HIDDEN"))
				{
					string text = builderPieceSet.subsets[i].GetShelfButtonName();
					if (text.IsNullOrEmpty())
					{
						text = builderPieceSet.setName;
					}
					text = text.ToUpper();
					int num3;
					if (dictionary.TryGetValue(text, ref num3))
					{
						int num4;
						this.displayGroupMap.TryGetValue(num3, ref num4);
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[num4];
						builderDisplayGroup.pieceSubsets.Add(builderPieceSet.subsets[i]);
						this.displayGroups[num4] = builderDisplayGroup;
					}
					else
					{
						string groupUniqueID = this.GetGroupUniqueID(builderPieceSet.playfabID, num);
						num++;
						BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup2 = new BuilderPieceSet.BuilderDisplayGroup(text, builderPieceSet.materialId, builderPieceSet.GetIntIdentifier(), groupUniqueID);
						builderDisplayGroup2.pieceSubsets.Add(builderPieceSet.subsets[i]);
						dictionary.Add(text, builderDisplayGroup2.GetDisplayGroupIdentifier());
						this.displayGroupMap.Add(builderDisplayGroup2.GetDisplayGroupIdentifier(), this.displayGroups.Count);
						this.displayGroups.Add(builderDisplayGroup2);
						if (!builderPieceSet.isScheduled)
						{
							this.liveDisplayGroups.Add(builderDisplayGroup2);
						}
					}
				}
				for (int j = 0; j < builderPieceSubset.pieceInfos.Count; j++)
				{
					BuilderPiece piecePrefab = builderPieceSubset.pieceInfos[j].piecePrefab;
					piecePrefab == null;
					int staticHash = piecePrefab.name.GetStaticHash();
					int pieceMaterial = num2;
					if (piecePrefab.materialOptions == null)
					{
						pieceMaterial = -1;
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
					else if (builderPieceSubset.pieceInfos[j].overrideSetMaterial)
					{
						if (builderPieceSubset.pieceInfos[j].pieceMaterialTypes.Length == 0)
						{
							Debug.LogErrorFormat("Material List for piece {0} in set {1} is empty", new object[]
							{
								piecePrefab.name,
								builderPieceSet.SetName
							});
						}
						foreach (string text2 in builderPieceSubset.pieceInfos[j].pieceMaterialTypes)
						{
							if (string.IsNullOrEmpty(text2))
							{
								Debug.LogErrorFormat("Material List Entry for piece {0} in set {1} is empty", new object[]
								{
									piecePrefab.name,
									builderPieceSet.SetName
								});
							}
							else
							{
								pieceMaterial = text2.GetHashCode();
								this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
							}
						}
					}
					else
					{
						Material material;
						int num5;
						piecePrefab.materialOptions.GetMaterialFromType(num2, out material, out num5);
						if (material == null)
						{
							pieceMaterial = -1;
						}
						this.AddPieceToInfoMap(staticHash, pieceMaterial, builderPieceSet.GetIntIdentifier());
					}
				}
			}
			if (!builderPieceSet.isScheduled)
			{
				this.livePieceSets.Add(builderPieceSet);
			}
			else
			{
				this.scheduledPieceSets.Add(builderPieceSet);
			}
		}
		this._unlockedPieceSets = new List<BuilderPieceSet>(this._allPieceSets.Count);
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000C390C File Offset: 0x000C1B0C
	private string GetGroupUniqueID(string setPlayfabID, int groupNumber)
	{
		return setPlayfabID.Trim('.') + ((char)(65 + groupNumber)).ToString();
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000C3934 File Offset: 0x000C1B34
	public void InitPieceDictionary()
	{
		if (this.hasPieceDictionary)
		{
			return;
		}
		BuilderSetManager.pieceTypes = new List<int>(256);
		BuilderSetManager.pieceList = new List<BuilderPiece>(256);
		BuilderSetManager.pieceTypeToIndex = new Dictionary<int, int>(256);
		int num = 0;
		for (int i = 0; i < this._allPieceSets.Count; i++)
		{
			BuilderPieceSet builderPieceSet = this._allPieceSets[i];
			if (!(builderPieceSet == null))
			{
				for (int j = 0; j < builderPieceSet.subsets.Count; j++)
				{
					BuilderPieceSet.BuilderPieceSubset builderPieceSubset = builderPieceSet.subsets[j];
					if (!(builderPieceSet == null))
					{
						for (int k = 0; k < builderPieceSubset.pieceInfos.Count; k++)
						{
							BuilderPieceSet.PieceInfo pieceInfo = builderPieceSubset.pieceInfos[k];
							if (!(pieceInfo.piecePrefab == null))
							{
								int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
								if (!BuilderSetManager.pieceTypeToIndex.ContainsKey(staticHash))
								{
									BuilderSetManager.pieceList.Add(pieceInfo.piecePrefab);
									BuilderSetManager.pieceTypes.Add(staticHash);
									BuilderSetManager.pieceTypeToIndex.Add(staticHash, num);
									num++;
								}
							}
						}
					}
				}
			}
		}
		this.hasPieceDictionary = true;
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000C3A78 File Offset: 0x000C1C78
	public BuilderPiece GetPiecePrefab(int pieceType)
	{
		int num;
		if (BuilderSetManager.pieceTypeToIndex.TryGetValue(pieceType, ref num))
		{
			return BuilderSetManager.pieceList[num];
		}
		Debug.LogErrorFormat("No Prefab found for type {0}", new object[]
		{
			pieceType
		});
		return null;
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000C3ABA File Offset: 0x000C1CBA
	private void OnEnable()
	{
		if (this.monitor == null && this.scheduledPieceSets.Count > 0)
		{
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000C3AE4 File Offset: 0x000C1CE4
	private void OnDisable()
	{
		if (this.monitor != null)
		{
			base.StopCoroutine(this.monitor);
		}
		this.monitor = null;
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000C3B01 File Offset: 0x000C1D01
	private IEnumerator MonitorTime()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
		{
			yield return null;
		}
		while (this.scheduledPieceSets.Count > 0)
		{
			bool flag = false;
			for (int i = this.scheduledPieceSets.Count - 1; i >= 0; i--)
			{
				BuilderPieceSet builderPieceSet = this.scheduledPieceSets[i];
				if (GorillaComputer.instance.GetServerTime() > builderPieceSet.GetScheduleDateTime())
				{
					flag = true;
					this.livePieceSets.Add(builderPieceSet);
					this.scheduledPieceSets.RemoveAt(i);
					int intIdentifier = builderPieceSet.GetIntIdentifier();
					foreach (BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup in this.displayGroups)
					{
						if (builderDisplayGroup != null && builderDisplayGroup.setID == intIdentifier && !this.liveDisplayGroups.Contains(builderDisplayGroup))
						{
							this.liveDisplayGroups.Add(builderDisplayGroup);
						}
					}
				}
			}
			if (flag)
			{
				this.OnLiveSetsUpdated.Invoke();
			}
			yield return new WaitForSeconds(60f);
		}
		this.monitor = null;
		yield break;
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x000C3B10 File Offset: 0x000C1D10
	private void AddPieceToInfoMap(int pieceType, int pieceMaterial, int setID)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, pieceMaterial), ref num))
		{
			BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo = BuilderSetManager.pieceSetInfos[num];
			if (!builderPieceSetInfo.setIds.Contains(setID))
			{
				builderPieceSetInfo.setIds.Add(setID);
			}
			BuilderSetManager.pieceSetInfos[num] = builderPieceSetInfo;
			return;
		}
		BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo2 = default(BuilderSetManager.BuilderPieceSetInfo);
		builderPieceSetInfo2.pieceType = pieceType;
		builderPieceSetInfo2.materialType = pieceMaterial;
		List<int> list = new List<int>();
		list.Add(setID);
		builderPieceSetInfo2.setIds = list;
		BuilderSetManager.BuilderPieceSetInfo builderPieceSetInfo3 = builderPieceSetInfo2;
		BuilderSetManager.pieceSetInfoMap.Add(HashCode.Combine<int, int>(pieceType, pieceMaterial), BuilderSetManager.pieceSetInfos.Count);
		BuilderSetManager.pieceSetInfos.Add(builderPieceSetInfo3);
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x000C3BB8 File Offset: 0x000C1DB8
	public static bool IsItemIDBuilderItem(string playfabID)
	{
		return BuilderSetManager.instance.GetAllSetsConcat().Contains(playfabID);
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000C3BCC File Offset: 0x000C1DCC
	public void OnGotInventoryItems(GetUserInventoryResult inventoryResult, GetCatalogItemsResult catalogResult)
	{
		CosmeticsController cosmeticsController = CosmeticsController.instance;
		cosmeticsController.concatStringCosmeticsAllowed += this.GetStarterSetsConcat();
		this._unlockedPieceSets.Clear();
		this._unlockedPieceSets.AddRange(this._starterPieceSets);
		foreach (CatalogItem catalogItem in catalogResult.Catalog)
		{
			BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
			if (BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId) && BuilderSetManager._setIdToStoreItem.TryGetValue(catalogItem.ItemId.GetStaticHash(), ref builderSetStoreItem))
			{
				bool hasPrice = false;
				uint cost = 0U;
				if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, ref cost))
				{
					hasPrice = true;
				}
				builderSetStoreItem.playfabID = catalogItem.ItemId;
				builderSetStoreItem.cost = cost;
				builderSetStoreItem.hasPrice = hasPrice;
				BuilderSetManager._setIdToStoreItem[builderSetStoreItem.setRef.GetIntIdentifier()] = builderSetStoreItem;
			}
		}
		foreach (ItemInstance itemInstance in inventoryResult.Inventory)
		{
			if (BuilderSetManager.IsItemIDBuilderItem(itemInstance.ItemId))
			{
				BuilderSetManager.BuilderSetStoreItem builderSetStoreItem2;
				if (BuilderSetManager._setIdToStoreItem.TryGetValue(itemInstance.ItemId.GetStaticHash(), ref builderSetStoreItem2))
				{
					Debug.LogFormat("BuilderSetManager: Unlocking Inventory Item {0}", new object[]
					{
						itemInstance.ItemId
					});
					this._unlockedPieceSets.Add(builderSetStoreItem2.setRef);
					CosmeticsController cosmeticsController2 = CosmeticsController.instance;
					cosmeticsController2.concatStringCosmeticsAllowed += itemInstance.ItemId;
				}
				else
				{
					Debug.Log("BuilderSetManager: No store item found with id" + itemInstance.ItemId);
				}
			}
		}
		this.pulledStoreItems = true;
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000C3DB0 File Offset: 0x000C1FB0
	public BuilderSetManager.BuilderSetStoreItem GetStoreItemFromSetID(int setID)
	{
		return CollectionExtensions.GetValueOrDefault<int, BuilderSetManager.BuilderSetStoreItem>(BuilderSetManager._setIdToStoreItem, setID, BuilderKiosk.nullItem);
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000C3DC4 File Offset: 0x000C1FC4
	public BuilderPieceSet GetPieceSetFromID(int setID)
	{
		BuilderSetManager.BuilderSetStoreItem builderSetStoreItem;
		if (BuilderSetManager._setIdToStoreItem.TryGetValue(setID, ref builderSetStoreItem))
		{
			return builderSetStoreItem.setRef;
		}
		return null;
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x000C3DE8 File Offset: 0x000C1FE8
	public BuilderPieceSet.BuilderDisplayGroup GetDisplayGroupFromIndex(int groupID)
	{
		int num;
		if (this.displayGroupMap.TryGetValue(groupID, ref num))
		{
			return this.displayGroups[num];
		}
		return null;
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000C3E13 File Offset: 0x000C2013
	public List<BuilderPieceSet> GetAllPieceSets()
	{
		return this._allPieceSets;
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000C3E1B File Offset: 0x000C201B
	public List<BuilderPieceSet> GetLivePieceSets()
	{
		return this.livePieceSets;
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000C3E23 File Offset: 0x000C2023
	public List<BuilderPieceSet.BuilderDisplayGroup> GetLiveDisplayGroups()
	{
		return this.liveDisplayGroups;
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000C3E2B File Offset: 0x000C202B
	public List<BuilderPieceSet> GetUnlockedPieceSets()
	{
		return this._unlockedPieceSets;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000C3E33 File Offset: 0x000C2033
	public List<BuilderPieceSet> GetPermanentSetsForSale()
	{
		return this._setsAlwaysForSale;
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000C3E3B File Offset: 0x000C203B
	public List<BuilderPieceSet> GetSeasonalSetsForSale()
	{
		return this._seasonalSetsForSale;
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000C3E44 File Offset: 0x000C2044
	public bool IsSetSeasonal(string playfabID)
	{
		return !this._seasonalSetsForSale.IsNullOrEmpty<BuilderPieceSet>() && this._seasonalSetsForSale.FindIndex((BuilderPieceSet x) => x.playfabID.Equals(playfabID)) >= 0;
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000C3E8C File Offset: 0x000C208C
	public bool DoesPlayerOwnDisplayGroup(Player player, int groupID)
	{
		if (player == null)
		{
			return false;
		}
		int num;
		if (!this.displayGroupMap.TryGetValue(groupID, ref num))
		{
			return false;
		}
		if (num < 0 || num >= this.displayGroups.Count)
		{
			return false;
		}
		BuilderPieceSet.BuilderDisplayGroup builderDisplayGroup = this.displayGroups[num];
		return builderDisplayGroup != null && this.DoesPlayerOwnPieceSet(player, builderDisplayGroup.setID);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000C3EE4 File Offset: 0x000C20E4
	public bool DoesPlayerOwnPieceSet(Player player, int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			bool flag = rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID);
			Debug.LogFormat("BuilderSetManager: does player {0} own set {1} {2}", new object[]
			{
				player.ActorNumber,
				pieceSetFromID.SetName,
				flag
			});
			return flag;
		}
		Debug.LogFormat("BuilderSetManager: could not get rig for player {0}", new object[]
		{
			player.ActorNumber
		});
		return false;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000C3F78 File Offset: 0x000C2178
	public bool DoesAnyPlayerInRoomOwnPieceSet(int setID)
	{
		BuilderPieceSet pieceSetFromID = this.GetPieceSetFromID(setID);
		if (pieceSetFromID == null)
		{
			return false;
		}
		if (this.GetStarterSetsConcat().Contains(pieceSetFromID.setName))
		{
			return true;
		}
		foreach (NetPlayer targetPlayer in RoomSystem.PlayersInRoom)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out rigContainer) && rigContainer.Rig.IsItemAllowed(pieceSetFromID.playfabID))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000C4018 File Offset: 0x000C2218
	public bool IsPieceOwnedByRoom(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), ref num))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.DoesAnyPlayerInRoomOwnPieceSet(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000C4098 File Offset: 0x000C2298
	public bool IsPieceOwnedLocally(int pieceType, int materialType)
	{
		int num;
		if (BuilderSetManager.pieceSetInfoMap.TryGetValue(HashCode.Combine<int, int>(pieceType, materialType), ref num))
		{
			foreach (int setID in BuilderSetManager.pieceSetInfos[num].setIds)
			{
				if (this.IsPieceSetOwnedLocally(setID))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000C4118 File Offset: 0x000C2318
	public bool IsPieceSetOwnedLocally(int setID)
	{
		return this._unlockedPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier()) >= 0;
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000C4150 File Offset: 0x000C2350
	public void UnlockSet(int setID)
	{
		int num = this._allPieceSets.FindIndex((BuilderPieceSet x) => setID == x.GetIntIdentifier());
		if (num >= 0 && !this._unlockedPieceSets.Contains(this._allPieceSets[num]))
		{
			Debug.Log("BuilderSetManager: unlocking set " + this._allPieceSets[num].SetName);
			this._unlockedPieceSets.Add(this._allPieceSets[num]);
		}
		UnityEvent onOwnedSetsUpdated = this.OnOwnedSetsUpdated;
		if (onOwnedSetsUpdated == null)
		{
			return;
		}
		onOwnedSetsUpdated.Invoke();
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000C41E8 File Offset: 0x000C23E8
	public void TryPurchaseItem(int setID, Action<bool> resultCallback)
	{
		BuilderSetManager.BuilderSetStoreItem storeItem;
		if (!BuilderSetManager._setIdToStoreItem.TryGetValue(setID, ref storeItem))
		{
			Debug.Log("BuilderSetManager: no store Item for set " + setID.ToString());
			Action<bool> resultCallback2 = resultCallback;
			if (resultCallback2 == null)
			{
				return;
			}
			resultCallback2.Invoke(false);
			return;
		}
		else
		{
			if (!this.IsPieceSetOwnedLocally(setID))
			{
				PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
				{
					ItemId = storeItem.playfabID,
					Price = (int)storeItem.cost,
					VirtualCurrency = this.currencyName,
					CatalogVersion = this.catalog
				}, delegate(PurchaseItemResult result)
				{
					if (result.Items.Count > 0)
					{
						foreach (ItemInstance itemInstance in result.Items)
						{
							Debug.Log("BuilderSetManager: unlocking set " + itemInstance.ItemId);
							this.UnlockSet(itemInstance.ItemId.GetStaticHash());
						}
						CosmeticsController.instance.UpdateMyCosmetics();
						if (PhotonNetwork.InRoom)
						{
							this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(storeItem.playfabID));
						}
						Action<bool> resultCallback4 = resultCallback;
						if (resultCallback4 == null)
						{
							return;
						}
						resultCallback4.Invoke(true);
						return;
					}
					else
					{
						Debug.Log("BuilderSetManager: no items purchased ");
						Action<bool> resultCallback5 = resultCallback;
						if (resultCallback5 == null)
						{
							return;
						}
						resultCallback5.Invoke(false);
						return;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogErrorFormat("BuilderSetManager: purchase {0} Error {1}", new object[]
					{
						setID,
						error.ErrorMessage
					});
					Action<bool> resultCallback4 = resultCallback;
					if (resultCallback4 == null)
					{
						return;
					}
					resultCallback4.Invoke(false);
				}, null, null);
				return;
			}
			Debug.Log("BuilderSetManager: set already owned " + setID.ToString());
			Action<bool> resultCallback3 = resultCallback;
			if (resultCallback3 == null)
			{
				return;
			}
			resultCallback3.Invoke(false);
			return;
		}
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000C42EC File Offset: 0x000C24EC
	private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
	{
		yield return new WaitForSeconds(1f);
		this.foundCosmetic = false;
		this.attempts = 0;
		while (!this.foundCosmetic && this.attempts < 10 && PhotonNetwork.InRoom)
		{
			this.playerIDList.Clear();
			if (GorillaServer.Instance != null && GorillaServer.Instance.NewCosmeticsPath())
			{
				this.playerIDList.Add("Inventory");
				PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
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
					bool flag = this.foundCosmetic;
				}, delegate(PlayFabError error)
				{
					this.attempts++;
					CosmeticsController.instance.ReauthOrBan(error);
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
			else
			{
				this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
				PlayFabClientAPI.GetSharedGroupData(new GetSharedGroupDataRequest
				{
					Keys = this.playerIDList,
					SharedGroupId = PhotonNetwork.CurrentRoom.Name + Regex.Replace(PhotonNetwork.CloudRegion, "[^a-zA-Z0-9]", "").ToUpper()
				}, delegate(GetSharedGroupDataResult result)
				{
					this.attempts++;
					foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
					{
						if (keyValuePair.Value.Value.Contains(itemToBuyID))
						{
							Debug.Log("BuilderSetManager: found it! updating others cosmetic!");
							PhotonNetwork.RaiseEvent(199, null, new RaiseEventOptions
							{
								Receivers = 0
							}, SendOptions.SendReliable);
							this.foundCosmetic = true;
						}
						else
						{
							Debug.Log("BuilderSetManager: didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString());
						}
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
					Debug.Log("BuilderSetManager: Got error retrieving user data, on attempt " + this.attempts.ToString());
					Debug.Log(error.GenerateErrorReport());
				}, null, null);
				yield return new WaitForSeconds(1f);
			}
		}
		Debug.Log("BuilderSetManager: done!");
		yield break;
	}

	// Token: 0x04002FDE RID: 12254
	private const string preLog = "[GT/MonkeBlocks/BuilderSetManager]  ";

	// Token: 0x04002FDF RID: 12255
	private const string preErr = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  ";

	// Token: 0x04002FE0 RID: 12256
	private const string preErrBeta = "[GT/MonkeBlocks/BuilderSetManager]  ERROR!!!  (beta only log)  ";

	// Token: 0x04002FE1 RID: 12257
	[SerializeField]
	private List<BuilderPieceSet> _allPieceSets;

	// Token: 0x04002FE2 RID: 12258
	[SerializeField]
	private List<BuilderPieceSet> _starterPieceSets;

	// Token: 0x04002FE3 RID: 12259
	[SerializeField]
	private List<BuilderPieceSet> _setsAlwaysForSale;

	// Token: 0x04002FE4 RID: 12260
	[SerializeField]
	private List<BuilderPieceSet> _seasonalSetsForSale;

	// Token: 0x04002FE5 RID: 12261
	private List<BuilderPieceSet> livePieceSets;

	// Token: 0x04002FE6 RID: 12262
	private List<BuilderPieceSet> scheduledPieceSets;

	// Token: 0x04002FE7 RID: 12263
	private List<BuilderPieceSet.BuilderDisplayGroup> liveDisplayGroups;

	// Token: 0x04002FE8 RID: 12264
	private Coroutine monitor;

	// Token: 0x04002FE9 RID: 12265
	private List<BuilderSetManager.BuilderSetStoreItem> _allStoreItems;

	// Token: 0x04002FEA RID: 12266
	private List<BuilderPieceSet> _unlockedPieceSets;

	// Token: 0x04002FEB RID: 12267
	private static Dictionary<int, BuilderSetManager.BuilderSetStoreItem> _setIdToStoreItem;

	// Token: 0x04002FEC RID: 12268
	private static List<BuilderSetManager.BuilderPieceSetInfo> pieceSetInfos;

	// Token: 0x04002FED RID: 12269
	private static Dictionary<int, int> pieceSetInfoMap;

	// Token: 0x04002FEE RID: 12270
	private List<BuilderPieceSet.BuilderDisplayGroup> displayGroups;

	// Token: 0x04002FEF RID: 12271
	private Dictionary<int, int> displayGroupMap;

	// Token: 0x04002FF0 RID: 12272
	[OnEnterPlay_SetNull]
	public static volatile BuilderSetManager instance;

	// Token: 0x04002FF2 RID: 12274
	[HideInInspector]
	public string catalog;

	// Token: 0x04002FF3 RID: 12275
	[HideInInspector]
	public string currencyName;

	// Token: 0x04002FF4 RID: 12276
	private string[] tempStringArray;

	// Token: 0x04002FF5 RID: 12277
	[HideInInspector]
	public UnityEvent OnLiveSetsUpdated;

	// Token: 0x04002FF6 RID: 12278
	[HideInInspector]
	public UnityEvent OnOwnedSetsUpdated;

	// Token: 0x04002FF7 RID: 12279
	[HideInInspector]
	public bool pulledStoreItems;

	// Token: 0x04002FF8 RID: 12280
	[OnEnterPlay_Set("")]
	private static string concatStarterSets = string.Empty;

	// Token: 0x04002FF9 RID: 12281
	[OnEnterPlay_Set("")]
	private static string concatAllSets = string.Empty;

	// Token: 0x04002FFA RID: 12282
	private bool foundCosmetic;

	// Token: 0x04002FFB RID: 12283
	private int attempts;

	// Token: 0x04002FFC RID: 12284
	private List<string> playerIDList = new List<string>();

	// Token: 0x04002FFD RID: 12285
	private static List<int> pieceTypes;

	// Token: 0x04002FFE RID: 12286
	[HideInInspector]
	public static List<BuilderPiece> pieceList;

	// Token: 0x04002FFF RID: 12287
	private static Dictionary<int, int> pieceTypeToIndex;

	// Token: 0x04003000 RID: 12288
	private bool hasPieceDictionary;

	// Token: 0x020005A2 RID: 1442
	[Serializable]
	public struct BuilderSetStoreItem
	{
		// Token: 0x04003001 RID: 12289
		public string displayName;

		// Token: 0x04003002 RID: 12290
		public string playfabID;

		// Token: 0x04003003 RID: 12291
		public int setID;

		// Token: 0x04003004 RID: 12292
		public uint cost;

		// Token: 0x04003005 RID: 12293
		public bool hasPrice;

		// Token: 0x04003006 RID: 12294
		public BuilderPieceSet setRef;

		// Token: 0x04003007 RID: 12295
		public GameObject displayModel;

		// Token: 0x04003008 RID: 12296
		[NonSerialized]
		public bool isNullItem;
	}

	// Token: 0x020005A3 RID: 1443
	[Serializable]
	public struct BuilderPieceSetInfo
	{
		// Token: 0x04003009 RID: 12297
		public int pieceType;

		// Token: 0x0400300A RID: 12298
		public int materialType;

		// Token: 0x0400300B RID: 12299
		public List<int> setIds;
	}
}
