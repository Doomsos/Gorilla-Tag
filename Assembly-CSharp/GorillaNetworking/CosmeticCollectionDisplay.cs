using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GorillaNetworking;

public class CosmeticCollectionDisplay : MonoBehaviour
{
	private static readonly Dictionary<(int, string), CosmeticCollectionDisplay> Registered = new Dictionary<(int, string), CosmeticCollectionDisplay>();

	private bool isCycling;

	private bool isVisible = true;

	private int activeIndex;

	private int registeredRigID;

	private string registeredParentID;

	private readonly List<GameObject> spawnedAnchors = new List<GameObject>();

	private readonly List<AsyncOperationHandle<GameObject>> loadOps = new List<AsyncOperationHandle<GameObject>>();

	private readonly List<CosmeticsController.CosmeticItem> placedCollectables = new List<CosmeticsController.CosmeticItem>();

	public string ParentPlayFabID { get; private set; }

	public int ActiveIndex => activeIndex;

	public int Count => spawnedAnchors.Count;

	public CosmeticsController.CosmeticItem? ActiveCollectable
	{
		get
		{
			if (placedCollectables.Count <= 0)
			{
				return null;
			}
			return placedCollectables[activeIndex];
		}
	}

	public static void Register(int rigID, string parentID, CosmeticCollectionDisplay display)
	{
		display.registeredRigID = rigID;
		display.registeredParentID = parentID;
		display.ParentPlayFabID = parentID;
		Registered[(rigID, parentID)] = display;
	}

	public static CosmeticCollectionDisplay FindForRig(int rigID, string parentID)
	{
		Registered.TryGetValue((rigID, parentID), out var value);
		return value;
	}

	public static void GetDisplaysForRig(int rigID, List<CosmeticCollectionDisplay> result)
	{
		result.Clear();
		foreach (KeyValuePair<(int, string), CosmeticCollectionDisplay> item in Registered)
		{
			if (item.Key.Item1 == rigID)
			{
				result.Add(item.Value);
			}
		}
	}

	public CosmeticsController.CosmeticItem? GetCollectableAt(int index)
	{
		if (index < 0 || index >= placedCollectables.Count)
		{
			return null;
		}
		return placedCollectables[index];
	}

	public bool ContentMatches(IReadOnlyList<CosmeticsController.CosmeticItem> items)
	{
		if (placedCollectables.Count != items.Count)
		{
			return false;
		}
		for (int i = 0; i < placedCollectables.Count; i++)
		{
			if (placedCollectables[i].itemName != items[i].itemName)
			{
				return false;
			}
		}
		return true;
	}

	public void Populate(IReadOnlyList<CosmeticsController.CosmeticItem> ownedCollectables, CosmeticInfoV2 parentInfo, Transform rootXform)
	{
		ClearSpawnedAnchors();
		placedCollectables.Clear();
		isCycling = parentInfo.collectionIsCycling;
		bool collectionUsesIndexTargeting = parentInfo.collectionUsesIndexTargeting;
		if (isCycling)
		{
			CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition = parentInfo.collectionSlots[0];
			Vector3 localScale = cosmeticCollectionSlotDefinition.offset.scale;
			if (Mathf.Abs(localScale.x) < 0.001f || Mathf.Abs(localScale.y) < 0.001f || Mathf.Abs(localScale.z) < 0.001f)
			{
				localScale = Vector3.one;
			}
			for (int i = 0; i < ownedCollectables.Count; i++)
			{
				GameObject gameObject = new GameObject($"CollectionSlot_{i}");
				gameObject.transform.SetParent(rootXform, worldPositionStays: false);
				gameObject.transform.localPosition = cosmeticCollectionSlotDefinition.offset.pos;
				gameObject.transform.localRotation = cosmeticCollectionSlotDefinition.offset.rot;
				gameObject.transform.localScale = localScale;
				spawnedAnchors.Add(gameObject);
				placedCollectables.Add(ownedCollectables[i]);
				InstantiateIntoAnchor(ownedCollectables[i], gameObject.transform);
			}
		}
		else
		{
			int num = 0;
			for (int j = 0; j < parentInfo.collectionSlots.Length; j++)
			{
				CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition2 = parentInfo.collectionSlots[j];
				CosmeticsController.CosmeticItem? cosmeticItem = null;
				if (collectionUsesIndexTargeting)
				{
					for (int k = 0; k < ownedCollectables.Count; k++)
					{
						if (ownedCollectables[k].collectionTargetSlotIndex == j)
						{
							cosmeticItem = ownedCollectables[k];
							break;
						}
					}
				}
				else if (num < ownedCollectables.Count)
				{
					cosmeticItem = ownedCollectables[num++];
				}
				if (cosmeticItem.HasValue)
				{
					Vector3 localScale2 = cosmeticCollectionSlotDefinition2.offset.scale;
					if (Mathf.Abs(localScale2.x) < 0.001f || Mathf.Abs(localScale2.y) < 0.001f || Mathf.Abs(localScale2.z) < 0.001f)
					{
						localScale2 = Vector3.one;
					}
					GameObject gameObject2 = new GameObject($"CollectionSlot_{j}");
					gameObject2.transform.SetParent(rootXform, worldPositionStays: false);
					gameObject2.transform.localPosition = cosmeticCollectionSlotDefinition2.offset.pos;
					gameObject2.transform.localRotation = cosmeticCollectionSlotDefinition2.offset.rot;
					gameObject2.transform.localScale = localScale2;
					spawnedAnchors.Add(gameObject2);
					placedCollectables.Add(cosmeticItem.Value);
					InstantiateIntoAnchor(cosmeticItem.Value, gameObject2.transform);
				}
			}
		}
		activeIndex = 0;
		ApplyCyclingVisibility();
	}

	public void SetActiveIndex(int index)
	{
		if (spawnedAnchors.Count != 0)
		{
			activeIndex = Mathf.Clamp(index, 0, spawnedAnchors.Count - 1);
			RefreshAnchorVisibility();
		}
	}

	public void CycleActive(int direction)
	{
		if (isCycling && spawnedAnchors.Count != 0)
		{
			activeIndex = (activeIndex + direction + spawnedAnchors.Count) % spawnedAnchors.Count;
			RefreshAnchorVisibility();
		}
	}

	public void SetVisible(bool visible)
	{
		isVisible = visible;
		RefreshAnchorVisibility();
	}

	private void InstantiateIntoAnchor(CosmeticsController.CosmeticItem collectable, Transform anchor)
	{
		if (!CosmeticsController.instance.TryGetCosmeticInfoV2(collectable.itemName, out var cosmeticInfo))
		{
			return;
		}
		CosmeticPart[] array = (cosmeticInfo.hasStoreParts ? cosmeticInfo.storeParts : cosmeticInfo.functionalParts);
		if (array == null || array.Length == 0)
		{
			return;
		}
		GTAssetRef<GameObject> prefabAssetRef = array[0].prefabAssetRef;
		if (prefabAssetRef == null || !prefabAssetRef.RuntimeKeyIsValid())
		{
			return;
		}
		Vector3 attachScale = Vector3.one;
		CosmeticPart[] functionalParts = cosmeticInfo.functionalParts;
		if (functionalParts != null && functionalParts.Length != 0)
		{
			CosmeticAttachInfo[] attachAnchors = functionalParts[0].attachAnchors;
			if (attachAnchors != null && attachAnchors.Length != 0)
			{
				Vector3 scale = attachAnchors[0].offset.scale;
				if (Mathf.Abs(scale.x) >= 0.001f && Mathf.Abs(scale.y) >= 0.001f && Mathf.Abs(scale.z) >= 0.001f)
				{
					attachScale = scale;
				}
			}
		}
		AsyncOperationHandle<GameObject> item = prefabAssetRef.InstantiateAsync(anchor);
		loadOps.Add(item);
		item.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				if (anchor == null || handle.Result == null)
				{
					Addressables.ReleaseInstance(handle);
				}
				else
				{
					handle.Result.transform.localPosition = Vector3.zero;
					handle.Result.transform.localRotation = Quaternion.identity;
					handle.Result.transform.localScale = attachScale;
				}
			}
		};
	}

	private void ApplyCyclingVisibility()
	{
		RefreshAnchorVisibility();
	}

	private void RefreshAnchorVisibility()
	{
		for (int i = 0; i < spawnedAnchors.Count; i++)
		{
			if (spawnedAnchors[i] != null)
			{
				bool active = isVisible && (!isCycling || i == activeIndex);
				spawnedAnchors[i].SetActive(active);
			}
		}
	}

	private void ClearSpawnedAnchors()
	{
		for (int i = 0; i < loadOps.Count; i++)
		{
			if (loadOps[i].IsValid())
			{
				Addressables.ReleaseInstance(loadOps[i]);
			}
		}
		loadOps.Clear();
		for (int j = 0; j < spawnedAnchors.Count; j++)
		{
			if (spawnedAnchors[j] != null)
			{
				Object.Destroy(spawnedAnchors[j]);
			}
		}
		spawnedAnchors.Clear();
		placedCollectables.Clear();
	}

	private void OnDisable()
	{
		Registered.Remove((registeredRigID, registeredParentID));
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(registeredParentID))
		{
			Registered[(registeredRigID, registeredParentID)] = this;
		}
	}

	private void OnDestroy()
	{
		Registered.Remove((registeredRigID, registeredParentID));
		ClearSpawnedAnchors();
	}
}
