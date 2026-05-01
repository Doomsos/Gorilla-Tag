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

	public CosmeticsController.CosmeticItem? GetCollectableAt(int index)
	{
		if (index < 0 || index >= placedCollectables.Count)
		{
			return null;
		}
		return placedCollectables[index];
	}

	public void Populate(IReadOnlyList<CosmeticsController.CosmeticItem> ownedCollectables, CosmeticInfoV2 parentInfo, Transform rootXform)
	{
		ClearSpawnedAnchors();
		placedCollectables.Clear();
		isCycling = parentInfo.collectionIsCycling;
		bool collectionUsesIndexTargeting = parentInfo.collectionUsesIndexTargeting;
		int num = 0;
		for (int i = 0; i < parentInfo.collectionSlots.Length; i++)
		{
			CosmeticCollectionSlotDefinition cosmeticCollectionSlotDefinition = parentInfo.collectionSlots[i];
			CosmeticsController.CosmeticItem? cosmeticItem = null;
			if (collectionUsesIndexTargeting)
			{
				for (int j = 0; j < ownedCollectables.Count; j++)
				{
					if (ownedCollectables[j].collectionTargetSlotIndex == i)
					{
						cosmeticItem = ownedCollectables[j];
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
				Vector3 localScale = cosmeticCollectionSlotDefinition.offset.scale;
				if (Mathf.Abs(localScale.x) < 0.001f || Mathf.Abs(localScale.y) < 0.001f || Mathf.Abs(localScale.z) < 0.001f)
				{
					localScale = Vector3.one;
				}
				GameObject gameObject = new GameObject($"CollectionSlot_{i}");
				gameObject.transform.SetParent(rootXform, worldPositionStays: false);
				gameObject.transform.localPosition = cosmeticCollectionSlotDefinition.offset.pos;
				gameObject.transform.localRotation = cosmeticCollectionSlotDefinition.offset.rot;
				gameObject.transform.localScale = localScale;
				spawnedAnchors.Add(gameObject);
				placedCollectables.Add(cosmeticItem.Value);
				InstantiateIntoAnchor(cosmeticItem.Value, gameObject.transform);
			}
		}
		activeIndex = 0;
		ApplyCyclingVisibility();
	}

	public void SetActiveIndex(int index)
	{
		if (spawnedAnchors.Count == 0)
		{
			return;
		}
		activeIndex = Mathf.Clamp(index, 0, spawnedAnchors.Count - 1);
		for (int i = 0; i < spawnedAnchors.Count; i++)
		{
			if (spawnedAnchors[i] != null)
			{
				spawnedAnchors[i].SetActive(i == activeIndex);
			}
		}
	}

	public void CycleActive(int direction)
	{
		if (isCycling && spawnedAnchors.Count != 0)
		{
			activeIndex = (activeIndex + direction + spawnedAnchors.Count) % spawnedAnchors.Count;
			ApplyCyclingVisibility();
		}
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
		if (!isCycling)
		{
			return;
		}
		for (int i = 0; i < spawnedAnchors.Count; i++)
		{
			if (spawnedAnchors[i] != null)
			{
				spawnedAnchors[i].SetActive(i == activeIndex);
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

	private void OnDestroy()
	{
		Registered.Remove((registeredRigID, registeredParentID));
		ClearSpawnedAnchors();
	}
}
