using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000470 RID: 1136
public class BodyDockPositions : MonoBehaviour
{
	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001CB6 RID: 7350 RVA: 0x000980B7 File Offset: 0x000962B7
	// (set) Token: 0x06001CB7 RID: 7351 RVA: 0x000980BF File Offset: 0x000962BF
	public TransferrableObject[] allObjects
	{
		get
		{
			return this._allObjects;
		}
		set
		{
			this._allObjects = value;
		}
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x000980C8 File Offset: 0x000962C8
	public void Awake()
	{
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x00098100 File Offset: 0x00096300
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		if (object.Equals(this.myRig.creator, otherPlayer))
		{
			this.DeallocateSharableInstances();
		}
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x0009811B File Offset: 0x0009631B
	public void OnLeftRoom()
	{
		this.DeallocateSharableInstances();
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x00098124 File Offset: 0x00096324
	public WorldShareableItem AllocateSharableInstance(BodyDockPositions.DropPositions position, NetPlayer owner)
	{
		switch (position)
		{
		case BodyDockPositions.DropPositions.None:
		case BodyDockPositions.DropPositions.LeftArm:
		case BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.LeftBack:
			if (this.leftBackSharableItem == null)
			{
				this.leftBackSharableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
				this.leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
				this.leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return this.leftBackSharableItem;
		default:
			if (position == BodyDockPositions.DropPositions.RightBack)
			{
				if (this.rightBackShareableItem == null)
				{
					this.rightBackShareableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
					this.rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
					this.rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
				}
				return this.rightBackShareableItem;
			}
			if (position != BodyDockPositions.DropPositions.All)
			{
			}
			break;
		}
		throw new ArgumentOutOfRangeException("position", position, null);
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x00098220 File Offset: 0x00096420
	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == this.leftBackSharableItem)
		{
			if (this.leftBackSharableItem == null)
			{
				return;
			}
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
			this.leftBackSharableItem = null;
		}
		if (worldShareable == this.rightBackShareableItem)
		{
			if (this.rightBackShareableItem == null)
			{
				return;
			}
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
			this.rightBackShareableItem = null;
		}
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x000982B4 File Offset: 0x000964B4
	public void DeallocateSharableInstances()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.rightBackShareableItem != null)
		{
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
		}
		if (this.leftBackSharableItem != null)
		{
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
		}
		this.leftBackSharableItem = null;
		this.rightBackShareableItem = null;
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x00098327 File Offset: 0x00096527
	public static bool IsPositionLeft(BodyDockPositions.DropPositions pos)
	{
		return pos == BodyDockPositions.DropPositions.LeftArm || pos == BodyDockPositions.DropPositions.LeftBack;
	}

	// Token: 0x06001CBF RID: 7359 RVA: 0x00098334 File Offset: 0x00096534
	public int DropZoneStorageUsed(BodyDockPositions.DropPositions dropPosition)
	{
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return -1;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) >= 0 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return this.myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x000983F0 File Offset: 0x000965F0
	public TransferrableObject ItemPositionInUse(BodyDockPositions.DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return null;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x000984B8 File Offset: 0x000966B8
	private int EnableTransferrableItem(int allItemsIndex, BodyDockPositions.DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= this.allObjects.Length)
		{
			return -1;
		}
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
				{
					this.DisableTransferrableItem(allItemsIndex);
				}
			}
			for (int j = 0; j < this.myRig.ActiveTransferrableObjectIndexLength(); j++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(j) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[allItemsIndex].gameObject.name);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						this.myRig.SetActiveTransferrableObjectIndex(j, allItemsIndex);
						this.myRig.SetTransferrablePosStates(j, startingState);
						this.myRig.SetTransferrableItemStates(j, (TransferrableObject.ItemStates)0);
						this.myRig.SetTransferrableDockPosition(j, startingPosition);
						this.EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return j;
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x000985B8 File Offset: 0x000967B8
	public BodyDockPositions.DropPositions ItemActive(int allItemsIndex)
	{
		if (!this.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return this.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x000985E0 File Offset: 0x000967E0
	public static BodyDockPositions.DropPositions OfflineItemActive(int allItemsIndex)
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		BodyDockPositions component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (component == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		if (!component.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return component.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x00098650 File Offset: 0x00096850
	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = this.allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
		}
		if (this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == index)
				{
					this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				}
			}
		}
	}

	// Token: 0x06001CC5 RID: 7365 RVA: 0x000986C4 File Offset: 0x000968C4
	public void DisableAllTransferableItems()
	{
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < this.allObjects.Length)
			{
				TransferrableObject transferrableObject = this.allObjects[num];
				transferrableObject.gameObject.Disable();
				transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
				this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				this.myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		this.DeallocateSharableInstances();
	}

	// Token: 0x06001CC6 RID: 7366 RVA: 0x00098751 File Offset: 0x00096951
	private bool AllItemsIndexValid(int allItemsIndex)
	{
		return allItemsIndex != -1 && allItemsIndex < this.allObjects.Length;
	}

	// Token: 0x06001CC7 RID: 7367 RVA: 0x00098764 File Offset: 0x00096964
	public bool PositionAvailable(int allItemIndex, BodyDockPositions.DropPositions startPos)
	{
		return (this.allObjects[allItemIndex].dockPositions & startPos) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x00098778 File Offset: 0x00096978
	public BodyDockPositions.DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			BodyDockPositions.DropPositions dropPositions = (BodyDockPositions.DropPositions)(1 << i);
			if ((this.allObjects[allItemIndex].dockPositions & dropPositions) != BodyDockPositions.DropPositions.None)
			{
				return dropPositions;
			}
		}
		return BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x000987AC File Offset: 0x000969AC
	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (BodyDockPositions.OfflineItemActive(allItemsIndex) != BodyDockPositions.DropPositions.None)
		{
			this.DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x000987C0 File Offset: 0x000969C0
	public void TransferrableItemDisableAtPosition(BodyDockPositions.DropPositions dropPositions)
	{
		int num = this.DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			this.TransferrableItemDisable(num);
		}
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x000987E4 File Offset: 0x000969E4
	public void TransferrableItemEnableAtPosition(string itemName, BodyDockPositions.DropPositions dropPosition)
	{
		if (this.DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = this.TransferrableObjectIndexFromName(itemName);
		if (list.Count == 0)
		{
			return;
		}
		TransferrableObject.PositionState startingState = this.MapDropPositionToState(dropPosition);
		if (list.Count == 1)
		{
			this.EnableTransferrableItem(list[0], dropPosition, startingState);
			return;
		}
		int allItemsIndex = BodyDockPositions.IsPositionLeft(dropPosition) ? list[0] : list[1];
		this.EnableTransferrableItem(allItemsIndex, dropPosition, startingState);
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x00098854 File Offset: 0x00096A54
	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			if (this.TransferrableItemActive(allItemsIndex))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x000988C0 File Offset: 0x00096AC0
	public bool TransferrableItemActiveAtPos(string transferrableItemName, BodyDockPositions.DropPositions dropPosition)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int allItemsIndex in list)
		{
			BodyDockPositions.DropPositions dropPositions = this.TransferrableItemPosition(allItemsIndex);
			if (dropPositions != BodyDockPositions.DropPositions.None && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x00098934 File Offset: 0x00096B34
	public bool TransferrableItemActive(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06001CCF RID: 7375 RVA: 0x00098940 File Offset: 0x00096B40
	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return this.allObjects[allItemsIndex];
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0009894A File Offset: 0x00096B4A
	public BodyDockPositions.DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex);
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x00098954 File Offset: 0x00096B54
	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int index in list)
		{
			this.DisableTransferrableItem(index);
		}
		return true;
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x000989B8 File Offset: 0x00096BB8
	public BodyDockPositions.DropPositions OppositePosition(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return BodyDockPositions.DropPositions.RightArm;
		}
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return BodyDockPositions.DropPositions.LeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return BodyDockPositions.DropPositions.RightBack;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return BodyDockPositions.DropPositions.LeftBack;
		}
		return pos;
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x000989D8 File Offset: 0x00096BD8
	public BodyDockPositions.DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new BodyDockPositions.DockingResult();
		}
		if (!this.AllItemsIndexValid(list[0]))
		{
			return new BodyDockPositions.DockingResult();
		}
		BodyDockPositions.DropPositions startingPos;
		if (isLeftHand)
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.RightArm : BodyDockPositions.DropPositions.LeftBack);
		}
		else
		{
			startingPos = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.LeftArm : BodyDockPositions.DropPositions.RightBack);
		}
		return this.ToggleTransferrableItem(transferrableItemName, startingPos, bothHands);
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x00098A58 File Offset: 0x00096C58
	public BodyDockPositions.DockingResult ToggleTransferrableItem(string transferrableItemName, BodyDockPositions.DropPositions startingPos, bool bothHands)
	{
		BodyDockPositions.DockingResult dockingResult = new BodyDockPositions.DockingResult();
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int allItemsIndex = list[i];
				BodyDockPositions.DropPositions dropPositions = BodyDockPositions.OfflineItemActive(allItemsIndex);
				if (dropPositions != BodyDockPositions.DropPositions.None)
				{
					this.TransferrableItemDisable(allItemsIndex);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int num = list[j];
			BodyDockPositions.DropPositions dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = this.OppositePosition(dropPositions2);
			}
			if (!this.PositionAvailable(num, dropPositions2))
			{
				dropPositions2 = this.FirstAvailablePosition(num);
				if (dropPositions2 == BodyDockPositions.DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (BodyDockPositions.OfflineItemActive(num) == dropPositions2)
			{
				this.TransferrableItemDisable(num);
				dockingResult.positionsDisabled.Add(dropPositions2);
			}
			else
			{
				this.TransferrableItemDisableAtPosition(dropPositions2);
				dockingResult.dockedPosition.Add(dropPositions2);
				TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPositions2);
				if (this.TransferrableItemActive(num))
				{
					BodyDockPositions.DropPositions dropPositions3 = this.TransferrableItemPosition(num);
					dockingResult.positionsDisabled.Add(dropPositions3);
					this.MoveTransferableItem(num, dropPositions2, positionState);
				}
				else
				{
					this.EnableTransferrableItem(num, dropPositions2, positionState);
				}
			}
		}
		return dockingResult;
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x00098B9F File Offset: 0x00096D9F
	private void MoveTransferableItem(int allItemsIndex, BodyDockPositions.DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		this.allObjects[allItemsIndex].storedZone = newPosition;
		this.allObjects[allItemsIndex].currentState = newPositionState;
		this.allObjects[allItemsIndex].ResetToDefaultState();
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x00098BCC File Offset: 0x00096DCC
	public void EnableTransferrableGameObject(int allItemsIndex, BodyDockPositions.DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		GameObject gameObject = this.allObjects[allItemsIndex].gameObject;
		TransferrableObject component = gameObject.GetComponent<TransferrableObject>();
		if ((component.dockPositions & dropZone) == BodyDockPositions.DropPositions.None || !component.ValidateState(startingPosition))
		{
			gameObject.Disable();
			return;
		}
		this.MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		gameObject.SetActive(true);
		ProjectileWeapon component2;
		if ((component2 = gameObject.GetComponent<ProjectileWeapon>()) != null)
		{
			component2.enabled = true;
		}
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x00098C30 File Offset: 0x00096E30
	public void RefreshTransferrableItems()
	{
		if (!this.myRig)
		{
			this.myRig = base.GetComponentInParent<VRRig>(true);
			if (!this.myRig)
			{
				Debug.LogError("BodyDockPositions.RefreshTransferrableItems: (should never happen) myRig is null and could not be found on same GameObject or parents. Path: " + base.transform.GetPathQ(), this);
			}
		}
		this.objectsToEnable.Clear();
		this.objectsToDisable.Clear();
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			bool flag = true;
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num != -1)
			{
				if (num < 0 || num >= this.allObjects.Length)
				{
					Debug.LogError(string.Format("Transferrable object index {0} out of range, expected [0..{1})", num, this.allObjects.Length));
				}
				else if (this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[num].gameObject.name)))
				{
					for (int j = 0; j < this.allObjects.Length; j++)
					{
						if (j == this.myRig.ActiveTransferrableObjectIndex(i) && this.allObjects[j].gameObject.activeSelf)
						{
							this.allObjects[j].objectIndex = i;
							flag = false;
						}
					}
					if (flag)
					{
						this.objectsToEnable.Add(i);
					}
				}
			}
		}
		for (int k = 0; k < this.allObjects.Length; k++)
		{
			if (this.allObjects[k] != null && this.allObjects[k].gameObject.activeSelf)
			{
				bool flag2 = true;
				for (int l = 0; l < this.myRig.ActiveTransferrableObjectIndexLength(); l++)
				{
					if (this.myRig.ActiveTransferrableObjectIndex(l) == k && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(l)].gameObject.name)))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.objectsToDisable.Add(k);
				}
			}
		}
		foreach (int index in this.objectsToDisable)
		{
			this.DisableTransferrableItem(index);
		}
		foreach (int idx in this.objectsToEnable)
		{
			this.EnableTransferrableGameObject(this.myRig.ActiveTransferrableObjectIndex(idx), this.myRig.TransferrableDockPosition(idx), this.myRig.TransferrablePosStates(idx));
		}
		this.UpdateHandState();
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x00098EFC File Offset: 0x000970FC
	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x00098F34 File Offset: 0x00097134
	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (!(this.allObjects[i] == null) && this.allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x00098F8C File Offset: 0x0009718C
	private TransferrableObject.PositionState MapDropPositionToState(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return TransferrableObject.PositionState.OnRightArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return TransferrableObject.PositionState.OnLeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return TransferrableObject.PositionState.OnLeftShoulder;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return TransferrableObject.PositionState.OnRightShoulder;
		}
		return TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001CDB RID: 7387 RVA: 0x00098FAB File Offset: 0x000971AB
	internal int PreviousLeftHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[0];
		}
	}

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001CDC RID: 7388 RVA: 0x00098FB5 File Offset: 0x000971B5
	internal int PreviousRightHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[1];
		}
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001CDD RID: 7389 RVA: 0x00098FBF File Offset: 0x000971BF
	internal float PreviousLeftHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[0];
		}
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001CDE RID: 7390 RVA: 0x00098FC9 File Offset: 0x000971C9
	internal float PreviousRightHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[1];
		}
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x00098FD4 File Offset: 0x000971D4
	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = (i == 0) ? this.leftHandThrowables : this.rightHandThrowables;
			int num = (i == 0) ? this.myRig.LeftThrowableProjectileIndex : this.myRig.RightThrowableProjectileIndex;
			for (int j = 0; j < array.Length; j++)
			{
				GameObject gameObject = array[j];
				gameObject == null;
				bool activeSelf = gameObject.activeSelf;
				bool flag = j == num;
				array[j].SetActive(flag);
				if (activeSelf && !flag)
				{
					this.throwableDisabledIndex[i] = j;
					this.throwableDisabledTime[i] = Time.time + 0.02f;
				}
			}
		}
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x00099071 File Offset: 0x00097271
	internal GameObject GetLeftHandThrowable()
	{
		return this.GetLeftHandThrowable(this.myRig.LeftThrowableProjectileIndex);
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x00099084 File Offset: 0x00097284
	internal GameObject GetLeftHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length)
		{
			throwableIndex = this.PreviousLeftHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length || this.PreviousLeftHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.leftHandThrowables[throwableIndex];
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x000990C3 File Offset: 0x000972C3
	internal GameObject GetRightHandThrowable()
	{
		return this.GetRightHandThrowable(this.myRig.RightThrowableProjectileIndex);
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x000990D6 File Offset: 0x000972D6
	internal GameObject GetRightHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length)
		{
			throwableIndex = this.PreviousRightHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length || this.PreviousRightHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.rightHandThrowables[throwableIndex];
	}

	// Token: 0x040026C2 RID: 9922
	public VRRig myRig;

	// Token: 0x040026C3 RID: 9923
	public GameObject[] leftHandThrowables;

	// Token: 0x040026C4 RID: 9924
	public GameObject[] rightHandThrowables;

	// Token: 0x040026C5 RID: 9925
	[FormerlySerializedAs("allObjects")]
	public TransferrableObject[] _allObjects;

	// Token: 0x040026C6 RID: 9926
	private List<int> objectsToEnable = new List<int>();

	// Token: 0x040026C7 RID: 9927
	private List<int> objectsToDisable = new List<int>();

	// Token: 0x040026C8 RID: 9928
	public Transform leftHandTransform;

	// Token: 0x040026C9 RID: 9929
	public Transform rightHandTransform;

	// Token: 0x040026CA RID: 9930
	public Transform chestTransform;

	// Token: 0x040026CB RID: 9931
	public Transform leftArmTransform;

	// Token: 0x040026CC RID: 9932
	public Transform rightArmTransform;

	// Token: 0x040026CD RID: 9933
	public Transform leftBackTransform;

	// Token: 0x040026CE RID: 9934
	public Transform rightBackTransform;

	// Token: 0x040026CF RID: 9935
	public WorldShareableItem leftBackSharableItem;

	// Token: 0x040026D0 RID: 9936
	public WorldShareableItem rightBackShareableItem;

	// Token: 0x040026D1 RID: 9937
	public GameObject SharableItemInstance;

	// Token: 0x040026D2 RID: 9938
	private int[] throwableDisabledIndex = new int[]
	{
		-1,
		-1
	};

	// Token: 0x040026D3 RID: 9939
	private float[] throwableDisabledTime = new float[2];

	// Token: 0x02000471 RID: 1137
	[Flags]
	public enum DropPositions
	{
		// Token: 0x040026D5 RID: 9941
		LeftArm = 1,
		// Token: 0x040026D6 RID: 9942
		RightArm = 2,
		// Token: 0x040026D7 RID: 9943
		Chest = 4,
		// Token: 0x040026D8 RID: 9944
		LeftBack = 8,
		// Token: 0x040026D9 RID: 9945
		RightBack = 16,
		// Token: 0x040026DA RID: 9946
		MaxDropPostions = 5,
		// Token: 0x040026DB RID: 9947
		All = 31,
		// Token: 0x040026DC RID: 9948
		None = 0
	}

	// Token: 0x02000472 RID: 1138
	public class DockingResult
	{
		// Token: 0x06001CE5 RID: 7397 RVA: 0x00099153 File Offset: 0x00097353
		public DockingResult()
		{
			this.dockedPosition = new List<BodyDockPositions.DropPositions>(2);
			this.positionsDisabled = new List<BodyDockPositions.DropPositions>(2);
		}

		// Token: 0x040026DD RID: 9949
		public List<BodyDockPositions.DropPositions> positionsDisabled;

		// Token: 0x040026DE RID: 9950
		public List<BodyDockPositions.DropPositions> dockedPosition;
	}
}
