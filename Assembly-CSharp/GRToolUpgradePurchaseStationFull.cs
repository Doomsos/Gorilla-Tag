using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000736 RID: 1846
public class GRToolUpgradePurchaseStationFull : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06002F8E RID: 12174 RVA: 0x00102D0C File Offset: 0x00100F0C
	public int SelectedShelf
	{
		get
		{
			return this.selectedShelf;
		}
	}

	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06002F8F RID: 12175 RVA: 0x00102D14 File Offset: 0x00100F14
	public int SelectedItem
	{
		get
		{
			return this.selectedItem;
		}
	}

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x06002F90 RID: 12176 RVA: 0x00102D1C File Offset: 0x00100F1C
	// (set) Token: 0x06002F91 RID: 12177 RVA: 0x00102D24 File Offset: 0x00100F24
	public bool TickRunning { get; set; }

	// Token: 0x06002F92 RID: 12178 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x00102D30 File Offset: 0x00100F30
	public void Init(GRToolProgressionManager progression, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.grManager = reactor.grManager;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += new Action(this.ProgressionUpdated);
		this.nextVisibleShelfIndex = -1;
		this.prefabMagnetHeightOffset = this.ropeTop.position.y;
		this.frontBackShelfMovement = new GRSpringMovement(0.5f, 0.7f);
		this.raiseLowerShelfMovement = new GRSpringMovement(1f, 0.7f);
		this.magnetMovement = new GRSpringMovement(1f, 0.7f);
		ProgressionManager.Instance.OnGetShiftCredit += new Action<string, int>(this.OnShiftCreditChanged);
		this.needsUIRefresh = true;
		this.InitPageSelectionWheel();
		this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
		this.SetActivePlayer(-1);
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x00102DFB File Offset: 0x00100FFB
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x00102E04 File Offset: 0x00101004
	public void HideOrShowTextBasedOnLocalPlayerDistance()
	{
		Vector3 position = GRPlayer.Get(VRRig.LocalRig).transform.position;
		Vector3 position2 = base.transform.position;
		float num = this.currentlyShowingText ? 8f : 6f;
		bool flag = (position - position2).sqrMagnitude < num * num;
		if (flag != this.currentlyShowingText)
		{
			this.shelfSelectionText.enabled = flag;
			this.playerInfo.enabled = flag;
			this.itemDescription.enabled = flag;
			this.itemDescriptionName.enabled = flag;
			this.itemDescriptionAnnotation.enabled = flag;
			this.purchaseButtonText.enabled = flag;
			this.pageSelectionWheel.ShowText(flag);
			for (int i = 0; i < this.gameShelves.Count; i++)
			{
				if (!(this.gameShelves[i] == null))
				{
					foreach (GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot in this.gameShelves[i].gRPurchaseSlots)
					{
						if (grpurchaseSlot.Name != null)
						{
							grpurchaseSlot.Name.enabled = flag;
						}
						if (grpurchaseSlot.Price != null)
						{
							grpurchaseSlot.Price.enabled = flag;
						}
					}
				}
			}
		}
		this.currentlyShowingText = flag;
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x00102F7C File Offset: 0x0010117C
	public void Tick()
	{
		this.HideOrShowTextBasedOnLocalPlayerDistance();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.toolProgressionManager == null)
		{
			return;
		}
		if (grplayer != null && (this.lastKnownLocalPlayerCredits != grplayer.ShiftCredits || this.lastKnownLocalPlayerJuice != this.toolProgressionManager.GetNumberOfResearchPoints()))
		{
			this.needsUIRefresh = true;
			this.lastKnownLocalPlayerCredits = grplayer.ShiftCredits;
			this.lastKnownLocalPlayerJuice = this.toolProgressionManager.GetNumberOfResearchPoints();
		}
		this.UpdateActivePlayer();
		this.UpdateSelectionLever();
		this.UpdateShelf();
		this.UpdateMagnet();
		if (this.disablePurchaseButton)
		{
			if (this.purchaseButtonPressed > 0f)
			{
				this.purchaseButtonPressed -= Time.deltaTime;
			}
			else
			{
				this.disablePurchaseButton = false;
			}
		}
		if (this.needsUIRefresh)
		{
			this.needsUIRefresh = false;
			this.UpdateShelfDisplayElements(this.currentVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.nextVisibleShelfIndex);
			this.UpdateShelfDisplayElements(this.selectedShelf);
			this.UpdatePlayerCurrencyUI();
			this.UpdatePurchaseButtonText();
		}
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x00103080 File Offset: 0x00101280
	public void SetActivePlayer(int actorNum)
	{
		this.currentActivePlayerActorNumber = actorNum;
		this.needsUIRefresh = true;
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.itemDescriptionName.text = "SWIPE FOR ACCESS";
			this.itemDescription.text = "Welcome to the Tool-o-matic v2 automated vending machine. Please swipe your ID card for access.";
			this.itemDescriptionAnnotation.text = "Remember: Compliance leads to success!";
			return;
		}
		if (this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem) && this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem].PurchaseID);
			if (partMetadata != null)
			{
				this.itemDescriptionName.text = partMetadata.name;
				this.itemDescription.text = partMetadata.description;
				this.itemDescriptionAnnotation.text = partMetadata.annotation;
			}
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x001031B8 File Offset: 0x001013B8
	public void UpdateActivePlayer()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		if (this.currentActivePlayerActorNumber != -1)
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
			if (grplayer != null)
			{
				BoxCollider component = base.GetComponent<BoxCollider>();
				Vector3 position = grplayer.transform.position;
				Vector3 vector = component.transform.worldToLocalMatrix.MultiplyPoint(position) - component.center;
				Vector3 vector2 = component.size * 0.5f;
				if (Mathf.Abs(vector.x) > vector2.x || Mathf.Abs(vector.y) > vector2.y || Mathf.Abs(vector.z) > vector2.z)
				{
					this.grManager.SetActivePlayerAuthority(this, -1);
					return;
				}
			}
			else
			{
				this.currentActivePlayerActorNumber = -1;
			}
		}
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x00103290 File Offset: 0x00101490
	private void UpdateShelf()
	{
		switch (this.shelfMovementState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		{
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
				return;
			}
			this.frontBackShelfMovement.target = 1f;
			this.frontBackShelfMovement.Update();
			float pos = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		{
			if (this.currentVisibleShelfIndex != this.selectedShelf)
			{
				this.SetNextShelf(this.selectedShelf);
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			this.frontBackShelfMovement.target = 0f;
			this.frontBackShelfMovement.Update();
			float pos2 = this.frontBackShelfMovement.pos;
			this.gameShelves[this.currentVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfRootTransform.position, this.shelfBackTransform.position, pos2);
			this.UpdateSoundsForMovement(this.frontBackShelfMovement);
			if (this.frontBackShelfMovement.IsAtTarget())
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
		{
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf && this.raiseLowerShelfMovement.pos <= 0.5f)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward);
				return;
			}
			this.raiseLowerShelfMovement.target = 1f;
			this.raiseLowerShelfMovement.Update();
			float pos3 = this.raiseLowerShelfMovement.pos;
			this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos3);
			this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
			if (this.raiseLowerShelfMovement.IsAtTarget())
			{
				this.SetCurrentShelf(this.nextVisibleShelfIndex);
				if (this.nextVisibleShelfIndex == this.selectedShelf)
				{
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
					return;
				}
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward);
				return;
			}
			break;
		}
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			if (this.nextVisibleShelfIndex == -1)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle);
				return;
			}
			if (this.nextVisibleShelfIndex != this.selectedShelf)
			{
				this.raiseLowerShelfMovement.target = 0f;
				this.raiseLowerShelfMovement.Update();
				float pos4 = this.raiseLowerShelfMovement.pos;
				this.gameShelves[this.nextVisibleShelfIndex].transform.position = Vector3.Lerp(this.shelfLowerTransform.position, this.shelfRootTransform.position, pos4);
				this.UpdateSoundsForMovement(this.raiseLowerShelfMovement);
				if (this.raiseLowerShelfMovement.IsAtTarget())
				{
					this.SetNextShelf(this.selectedShelf);
					this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
					return;
				}
			}
			else
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x001035C4 File Offset: 0x001017C4
	private void UpdateSoundsForMovement(GRSpringMovement movement)
	{
		if (movement.IsAtTarget())
		{
			this.audioSourceLooping.volume = 0f;
			if (movement.HitTargetLastUpdate())
			{
				this.audioSourceClang.Play();
				return;
			}
		}
		else
		{
			this.audioSourceLooping.volume = Mathf.Clamp01(Math.Abs(movement.speed) * this.audioSourceLoopingVolume);
		}
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x00103620 File Offset: 0x00101820
	public void SetCurrentShelf(int idx)
	{
		if (idx == -1)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (!this.IsValidShelfItemIndex(idx, 0))
		{
			return;
		}
		if (idx == this.nextVisibleShelfIndex)
		{
			this.SetNextShelf(-1);
		}
		this.UpdateShelfVisibility(this.currentVisibleShelfIndex, false);
		this.frontBackShelfMovement.Reset();
		this.gameShelves[idx].transform.position = this.shelfRootTransform.position;
		this.UpdateShelfVisibility(idx, true);
		this.currentVisibleShelfIndex = idx;
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x001036A0 File Offset: 0x001018A0
	public void SetNextShelf(int idx)
	{
		if (idx == this.nextVisibleShelfIndex)
		{
			return;
		}
		if (idx == this.currentVisibleShelfIndex)
		{
			return;
		}
		if (this.nextVisibleShelfIndex != -1)
		{
			this.UpdateShelfVisibility(this.nextVisibleShelfIndex, false);
		}
		if (idx != -1)
		{
			this.raiseLowerShelfMovement.Reset();
			this.gameShelves[idx].transform.position = this.shelfLowerTransform.position;
			this.UpdateShelfVisibility(idx, true);
		}
		this.nextVisibleShelfIndex = idx;
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x00103718 File Offset: 0x00101918
	public void ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState newState)
	{
		this.shelfMovementState = newState;
		switch (newState)
		{
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle:
			this.SetCurrentShelf(this.selectedShelf);
			this.SetNextShelf(-1);
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfBackward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward:
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfDownward:
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		case GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveNextShelfUpward:
			if (this.currentVisibleShelfIndex == this.selectedShelf)
			{
				this.ChangeShelfMovementState(GRToolUpgradePurchaseStationFull.ShelfMovementState.MoveCurrentShelfForward);
			}
			else
			{
				this.SetNextShelf(this.selectedShelf);
			}
			this.audioSourceLooping.volume = 0f;
			this.audioSourceLooping.GTPlay();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x001037B5 File Offset: 0x001019B5
	public void UpdateShelfVisibility(int shelfID, bool isVisible)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		this.gameShelves[shelfID].gameObject.SetActive(isVisible);
		if (isVisible)
		{
			this.UpdateShelfDisplayElements(shelfID);
		}
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x001037E4 File Offset: 0x001019E4
	public void UpdateShelfDisplayElements(int shelfID)
	{
		if (!this.IsValidShelfItemIndex(shelfID, 0))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf grtoolUpgradePurchaseStationShelf = this.gameShelves[shelfID];
		for (int i = 0; i < grtoolUpgradePurchaseStationShelf.gRPurchaseSlots.Count; i++)
		{
			this.UpdateShelfItemDisplayElements(shelfID, i);
		}
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x00103828 File Offset: 0x00101A28
	public void UpdatePurchaseButtonText()
	{
		if (!this.IsValidShelfItemIndex(this.selectedShelf, this.selectedItem))
		{
			this.purchaseButtonText.text = "ERROR";
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[this.selectedShelf].gRPurchaseSlots[this.selectedItem];
		Color color = grpurchaseSlot.canAfford ? this.colorPurchaseButtonCanAfford : this.colorCantBuy;
		string purchaseText = grpurchaseSlot.purchaseText;
		if (color != this.purchaseButtonText.color)
		{
			this.purchaseButtonText.color = color;
		}
		if (purchaseText != this.purchaseButtonText.text)
		{
			this.purchaseButtonText.text = purchaseText;
		}
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x001038D8 File Offset: 0x00101AD8
	public void UpdateShelfItemDisplayElements(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[slotID];
		if (this.toolProgressionManager)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
			if (partMetadata == null)
			{
				grpurchaseSlot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			grpurchaseSlot.canAfford = false;
			grpurchaseSlot.purchaseText = "LOCKED";
			bool flag2;
			if (this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag2))
			{
				if (flag2)
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, null);
					if (this.toolProgressionManager.GetShiftCreditCost(grpurchaseSlot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag3 = num2 >= num;
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorSelectedItem : this.colorUnselectedItem);
					grpurchaseSlot.Price.text = text;
					grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyCredits : this.colorCantBuy);
					grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					grpurchaseSlot.canAfford = flag3;
					if (flag3)
					{
						grpurchaseSlot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
					}
					else
					{
						grpurchaseSlot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					}
				}
				else
				{
					this.gameShelves[shelf].SetMaterialOverride(slotID, this.unresearchedItemMaterial);
					grpurchaseSlot.Name.text = partMetadata.name;
					grpurchaseSlot.Name.color = ((slotID == this.selectedItem) ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(grpurchaseSlot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(grpurchaseSlot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts part in this.cachedRequiredPartsList)
						{
							bool flag4 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(part);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(part, out flag4) || !flag4)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag = false;
							}
						}
					}
					if (!flag)
					{
						grpurchaseSlot.Price.text = text2;
						grpurchaseSlot.Price.color = this.colorCantBuy;
						grpurchaseSlot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = false;
						grpurchaseSlot.purchaseText = "LOCKED";
					}
					else
					{
						if (this.toolProgressionManager.GetPartUnlockJuiceCost(grpurchaseSlot.PurchaseID, out num))
						{
							text = string.Format("⑮ {0}", num);
						}
						bool flag3 = numberOfResearchPoints >= num;
						grpurchaseSlot.Price.text = text;
						grpurchaseSlot.Price.color = (flag3 ? this.colorCanBuyJuice : this.colorCantBuy);
						grpurchaseSlot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						grpurchaseSlot.canAfford = flag3;
						if (flag3)
						{
							grpurchaseSlot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						}
						else
						{
							grpurchaseSlot.purchaseText = string.Format("NEED\n⑮ {0}", num);
						}
					}
				}
			}
			if (slotID != this.selectedItem)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, false, this.backlightLocked);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyJuice)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightResearch);
				return;
			}
			if (grpurchaseSlot.Price.color == this.colorCanBuyCredits)
			{
				this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightPurchase);
				return;
			}
			this.gameShelves[shelf].SetBacklightStateAndMaterial(slotID, true, this.backlightLocked);
		}
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x00103DB8 File Offset: 0x00101FB8
	public void UpdatePlayerCurrencyUI()
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer2 == null)
		{
			this.currentActivePlayerActorNumber = -1;
			this.playerInfo.text = "AVAILABLE";
			return;
		}
		string text2;
		if (grplayer2 == grplayer)
		{
			int shiftCredits = grplayer2.ShiftCredits;
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			string text = (player != null) ? player.SanitizedNickName : "RANDO MONKE";
			string employeeLevelDisplayName = this.toolProgressionManager.GetEmployeeLevelDisplayName(this.toolProgressionManager.GetCurrentEmployeeLevel());
			text2 = string.Format("<color=#c0c0c0>{0}\n{1}</color>\n\n<color=purple><size=2>⑮ {2}</size></color>\n<color=white><size=2>⑭ {3}</size></color>\n", new object[]
			{
				text,
				employeeLevelDisplayName,
				numberOfResearchPoints,
				shiftCredits
			});
		}
		else
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(this.currentActivePlayerActorNumber);
			text2 = (((player2 != null) ? player2.SanitizedNickName : "RANDO MONKE") ?? "");
		}
		this.playerInfo.text = text2;
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x00103EE8 File Offset: 0x001020E8
	public bool CanLocalPlayerPurchaseItem(int shelf, int slotID)
	{
		if (!this.IsValidShelfItemIndex(shelf, slotID))
		{
			return false;
		}
		if (this.grManager && this.grManager.DebugIsToolStationHacked())
		{
			return true;
		}
		this.UpdateShelfItemDisplayElements(shelf, slotID);
		return this.gameShelves[shelf].gRPurchaseSlots[slotID].canAfford;
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x00103F44 File Offset: 0x00102144
	public bool CheckActivePlayer()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.currentActivePlayerActorNumber == -1)
		{
			this.RequestActivePlayerToken();
			return false;
		}
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer2 == null)
		{
			this.currentActivePlayerActorNumber = -1;
		}
		return !(grplayer2 != grplayer);
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x00103F93 File Offset: 0x00102193
	public void SelectOption1()
	{
		this.OnLocalSelectionButtonPressed(0);
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x00103F9C File Offset: 0x0010219C
	public void SelectOption2()
	{
		this.OnLocalSelectionButtonPressed(1);
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x00103FA5 File Offset: 0x001021A5
	public void SelectOption3()
	{
		this.OnLocalSelectionButtonPressed(2);
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x00103FAE File Offset: 0x001021AE
	public void SelectOption4()
	{
		this.OnLocalSelectionButtonPressed(3);
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x00103FB8 File Offset: 0x001021B8
	public void OnLocalSelectionButtonPressed(int index)
	{
		if (!this.CheckActivePlayer())
		{
			if (index == 0 && this.selectedItem != 0)
			{
				this.select1.SetButtonState(false);
			}
			if (index == 1 && this.selectedItem != 1)
			{
				this.select2.SetButtonState(false);
			}
			if (index == 2 && this.selectedItem != 2)
			{
				this.select3.SetButtonState(false);
			}
			if (index == 3 && this.selectedItem != 3)
			{
				this.select4.SetButtonState(false);
			}
			return;
		}
		if (index != 0)
		{
			this.select1.SetButtonState(false);
		}
		if (index != 1)
		{
			this.select2.SetButtonState(false);
		}
		if (index != 2)
		{
			this.select3.SetButtonState(false);
		}
		if (index != 3)
		{
			this.select4.SetButtonState(false);
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle)
		{
			this.SetSelectedShelfAndItem(this.selectedShelf, index, false);
		}
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x00104085 File Offset: 0x00102285
	public void SelectPageDown()
	{
		this.OnLocalSelectionPageChange(1);
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x0010408E File Offset: 0x0010228E
	public void SelectPageUp()
	{
		this.OnLocalSelectionPageChange(-1);
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x00104097 File Offset: 0x00102297
	public void OnLocalSelectionPageChange(int delta)
	{
		if (!this.CheckActivePlayer())
		{
			return;
		}
		this.pageSelectionWheel.SetTargetShelf((this.pageSelectionWheel.targetPage + delta + this.gameShelves.Count) % this.gameShelves.Count);
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x001040D2 File Offset: 0x001022D2
	public void CardSwiped()
	{
		this.RequestActivePlayerToken();
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x001040DC File Offset: 0x001022DC
	public void PurchaseButtonPressed()
	{
		if (this.disablePurchaseButton)
		{
			return;
		}
		this.purchaseButtonPressed = this.purchaseButtonCooldown;
		this.disablePurchaseButton = true;
		if (!this.CheckActivePlayer())
		{
			return;
		}
		if (this.shelfMovementState == GRToolUpgradePurchaseStationFull.ShelfMovementState.Idle && this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId)
		{
			this.RequestPurchaseItem(this.selectedShelf, this.selectedItem);
		}
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x00002789 File Offset: 0x00000989
	public void DEBUGSetHackToolStation()
	{
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x00104136 File Offset: 0x00102336
	public void RequestActivePlayerToken()
	{
		if (this.lastRequestedActivePlayerTokenTime > Time.time || this.lastRequestedActivePlayerTokenTime + this.requestActivePlayerTokenThrottleTime < Time.time)
		{
			this.lastRequestedActivePlayerTokenTime = Time.time;
			this.grManager.RequestStationExclusivity(this);
		}
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x00104170 File Offset: 0x00102370
	private void UpdateMagnet()
	{
		if (this.desiredMagnetEntityTypeId != this.currentMagnetEntityTypeId || this.currentMagnetEntityTypeId == -1 || this.currentMagnetEntity == null)
		{
			this.magnetMovement.SetHardStopAtTarget(true);
			this.magnetMovement.target = 0f;
			this.magnetMovement.Update();
			Vector3 position = this.ropeTop.transform.position;
			position.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (position.y != this.ropeTop.transform.position.y)
			{
				this.ropeTop.transform.position = position;
			}
			if (this.magnetMovement.IsAtTarget() && this.grManager.IsAuthority() && this.grManager.IsZoneActive())
			{
				if (this.currentMagnetEntity != null)
				{
					this.currentMagnetEntity.transform.parent = null;
					this.currentMagnetEntity.gameObject.SetActive(false);
					this.grManager.gameEntityManager.RequestDestroyItem(this.currentMagnetEntity.id);
					this.currentMagnetEntity = null;
					this.currentMagnetEntityTypeId = -1;
				}
				if (this.desiredMagnetEntityTypeId != -1)
				{
					GhostReactor.ToolEntityCreateData toolEntityCreateData = default(GhostReactor.ToolEntityCreateData);
					toolEntityCreateData.decayTime = 0f;
					toolEntityCreateData.stationIndex = this.grManager.GetIndexForToolUpgradeStationFull(this);
					this.grManager.gameEntityManager.RequestCreateItem(this.desiredMagnetEntityTypeId, this.ropeEnd.position, this.ropeEnd.rotation, toolEntityCreateData.Pack());
					this.currentMagnetEntityTypeId = this.desiredMagnetEntityTypeId;
					return;
				}
			}
		}
		else if (this.desiredMagnetEntityTypeId == this.currentMagnetEntityTypeId && this.currentMagnetEntity != null)
		{
			this.magnetMovement.SetHardStopAtTarget(false);
			this.magnetMovement.target = 1f;
			this.magnetMovement.Update();
			Vector3 position2 = this.ropeTop.transform.position;
			position2.y = Mathf.Lerp(this.prefabMagnetHeightOffset, this.prefabMagnetHeightOffset - this.maxMagnetDistance, this.magnetMovement.pos);
			if (this.ropeTop.transform.position.y != position2.y)
			{
				this.ropeTop.transform.position = position2;
			}
		}
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x001043E4 File Offset: 0x001025E4
	public void InitLinkedEntity(GameEntity entity)
	{
		if (this.currentMagnetEntity != null)
		{
			this.currentMagnetEntity.gameObject.SetActive(false);
		}
		entity.pickupable = false;
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
		}
		GRToolUpgradePurchaseStationMagnetPoint component2 = entity.GetComponent<GRToolUpgradePurchaseStationMagnetPoint>();
		GameDockable component3 = entity.GetComponent<GameDockable>();
		Transform dock = (component2 != null) ? component2.magnetAttachTransform : ((component3 != null) ? component3.dockablePoint : entity.transform);
		GRToolUpgradePurchaseStationFull.AttachEntityToMagnet_DockGoesToLocation(this.magnet, entity.transform, dock, new Vector3(0f, -0.03f, 0f));
		float num = 0f;
		float num2 = 0f;
		bool flag = false;
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			for (int j = 0; j < this.gameShelves[i].gRPurchaseSlots.Count; j++)
			{
				GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[i].gRPurchaseSlots[j];
				if (grpurchaseSlot != null && !(grpurchaseSlot.ToolEntityPrefab == null) && grpurchaseSlot.ToolEntityPrefab.name != null && grpurchaseSlot.ToolEntityPrefab.name.GetStaticHash() == entity.typeId)
				{
					num = grpurchaseSlot.RopeYaw;
					num2 = grpurchaseSlot.RopePitch;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Quaternion quaternion = Quaternion.Euler(0f, 0f, 180f);
		quaternion = Quaternion.AngleAxis(num, Vector3.up) * quaternion;
		quaternion = Quaternion.AngleAxis(num2, Vector3.forward) * quaternion;
		this.magnet.localRotation = quaternion;
		this.magnet.localPosition = quaternion * new Vector3(0f, 0.055f, 0f);
		this.currentMagnetEntity = entity;
		this.currentMagnetEntityTypeId = entity.typeId;
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x001045DC File Offset: 0x001027DC
	public void UpdateSelectionLever()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer grplayer2 = GRPlayer.Get(this.currentActivePlayerActorNumber);
		bool flag = ControllerInputPoller.GripFloat(4) > 0.7f;
		bool flag2 = ControllerInputPoller.GripFloat(5) > 0.7f;
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		Transform handTransform = GamePlayer.GetHandTransform(offlineVRRig, 0);
		Transform handTransform2 = GamePlayer.GetHandTransform(offlineVRRig, 1);
		Vector3 position = this.pageSelectionHandle.transform.position;
		Vector3 vector = handTransform.position - position;
		Vector3 vector2 = handTransform2.position - position;
		float num = this.pageSelectionLever.transform.localRotation.eulerAngles.x;
		float num2 = 0.2f;
		float num3 = this.bIsGrippingLeft ? 0.15f : 0.1f;
		float num4 = this.bIsGrippingRight ? 0.15f : 0.1f;
		if (vector.sqrMagnitude > num3 * num3)
		{
			flag = false;
		}
		if (vector2.sqrMagnitude > num4 * num4)
		{
			flag2 = false;
		}
		if (!this.bGripLeftLastFrame && flag)
		{
			this.bIsGrippingLeft = true;
		}
		else if (this.bGripLeftLastFrame && flag)
		{
			Vector3 forward = this.pageSelectionHandle.transform.forward;
			float num5 = Vector3.Dot(vector, forward);
			num += num5 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingLeft = false;
		}
		if (!this.bGripRightLastFrame && flag2)
		{
			this.bIsGrippingRight = true;
		}
		else if (this.bGripRightLastFrame && flag2)
		{
			Vector3 forward2 = this.pageSelectionHandle.transform.forward;
			float num6 = Vector3.Dot(vector2, forward2);
			num += num6 / num2 * 180f / 3.1415925f;
		}
		else
		{
			this.bIsGrippingRight = false;
		}
		if (!this.bIsGrippingLeft && !this.bIsGrippingRight && grplayer == grplayer2)
		{
			num = 30f + (num - 30f) * Mathf.Exp(-20f * Time.deltaTime);
		}
		num = Mathf.Clamp(num, 0f, 60f);
		if ((grplayer == grplayer2 || this.currentActivePlayerActorNumber == -1) && this.lastHandleAngle != num)
		{
			this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
			this.lastHandleAngle = num;
		}
		float rotationSpeed = 0f;
		if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			rotationSpeed = (num - 30f) / 30f;
		}
		this.bGripLeftLastFrame = flag;
		this.bGripRightLastFrame = flag2;
		if (grplayer == grplayer2)
		{
			this.pageSelectionWheel.isBeingDrivenRemotely = false;
			this.pageSelectionWheel.SetRotationSpeed(rotationSpeed);
			if (this.pageSelectionWheel.targetPage != this.selectedShelf)
			{
				this.SetSelectedShelfAndItem(this.pageSelectionWheel.targetPage, 0, false);
			}
			float num7 = 0.25f;
			this.timeSinceLastHandleBroadcast += Time.deltaTime;
			if (this.timeSinceLastHandleBroadcast > num7 && (Math.Abs(num - this.angleOfLastHandleBroadcast) > 0.02f || Math.Abs(this.pageSelectionWheel.currentAngle - this.selectionWheelAngleOfLastBroadcast) > 0.02f))
			{
				this.timeSinceLastHandleBroadcast = 0f;
				this.angleOfLastHandleBroadcast = num;
				this.selectionWheelAngleOfLastBroadcast = this.pageSelectionWheel.currentAngle;
				this.grManager.BroadcastHandleAndSelectionWheelPosition(this, (int)(num * this.quantMult), (int)(this.selectionWheelAngleOfLastBroadcast * this.quantMult));
				return;
			}
		}
		else if (this.bIsGrippingLeft || this.bIsGrippingRight)
		{
			this.CheckActivePlayer();
		}
	}

	// Token: 0x06002FB5 RID: 12213 RVA: 0x0010495C File Offset: 0x00102B5C
	public static void AttachEntityToMagnet_DockGoesToLocation(Transform magnet, Transform entity, Transform dock, Vector3 magnetDockOffset)
	{
		if (magnet == null || entity == null || dock == null)
		{
			return;
		}
		if (!dock.IsChildOf(entity))
		{
			return;
		}
		Matrix4x4 m = entity.worldToLocalMatrix * dock.localToWorldMatrix;
		Vector3 vector = GRToolUpgradePurchaseStationFull.ExtractLossyScale(m);
		Vector3 localPosition;
		Quaternion localRotation;
		Vector3 localScale;
		GRToolUpgradePurchaseStationFull.DecomposeTRS(Matrix4x4.TRS(magnetDockOffset, Quaternion.identity, vector) * m.inverse, out localPosition, out localRotation, out localScale);
		entity.SetParent(magnet, false);
		entity.localPosition = localPosition;
		entity.localRotation = localRotation;
		entity.localScale = localScale;
	}

	// Token: 0x06002FB6 RID: 12214 RVA: 0x001049EC File Offset: 0x00102BEC
	public void SetHandleAndSelectionWheelPositionRemote(int handlePos, int wheelPos)
	{
		this.pageSelectionWheel.isBeingDrivenRemotely = true;
		float num = (float)handlePos / this.quantMult;
		num = Mathf.Clamp(num, 0f, 60f);
		this.pageSelectionLever.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.pageSelectionWheel.SetTargetAngle((float)wheelPos / this.quantMult);
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00102DFB File Offset: 0x00100FFB
	public void ProgressionUpdated()
	{
		this.needsUIRefresh = true;
	}

	// Token: 0x06002FB8 RID: 12216 RVA: 0x00104A54 File Offset: 0x00102C54
	public void SetSelectedShelfAndItem(int shelf, int item, bool fromNetworkRPC)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (this.toolProgressionManager == null)
		{
			return;
		}
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.gameShelves[shelf].gRPurchaseSlots[item].PurchaseID);
		if (partMetadata != null)
		{
			this.itemDescriptionName.text = partMetadata.name;
			this.itemDescription.text = partMetadata.description;
			this.itemDescriptionAnnotation.text = partMetadata.annotation;
		}
		this.shelfSelectionText.text = this.gameShelves[shelf].ShelfName;
		if (this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab != null)
		{
			this.desiredMagnetEntityTypeId = this.gameShelves[shelf].gRPurchaseSlots[item].ToolEntityPrefab.name.GetStaticHash();
		}
		else
		{
			this.desiredMagnetEntityTypeId = -1;
		}
		bool flag = this.selectedShelf != shelf;
		bool flag2 = this.selectedItem != item;
		this.selectedShelf = shelf;
		this.selectedItem = item;
		this.needsUIRefresh = true;
		if (!fromNetworkRPC)
		{
			if (flag || flag2)
			{
				this.grManager.RequestNetworkShelfAndItemChange(this, this.selectedShelf, this.selectedItem);
				return;
			}
		}
		else
		{
			this.pageSelectionWheel.SetTargetShelf(this.selectedShelf);
			this.select1.SetButtonState(this.selectedItem == 0);
			this.select2.SetButtonState(this.selectedItem == 1);
			this.select3.SetButtonState(this.selectedItem == 2);
			this.select4.SetButtonState(this.selectedItem == 3);
		}
	}

	// Token: 0x06002FB9 RID: 12217 RVA: 0x00104C00 File Offset: 0x00102E00
	public void RequestPurchaseItem(int shelf, int item)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		if (!this.CanLocalPlayerPurchaseItem(shelf, item))
		{
			if (this.scanner != null)
			{
				UnityEvent onFailed = this.scanner.onFailed;
				if (onFailed == null)
				{
					return;
				}
				onFailed.Invoke();
			}
			return;
		}
		if (this.scanner != null)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
		}
		bool flag;
		if (!this.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(grpurchaseSlot.PurchaseID, out flag) || !flag))
		{
			this.toolProgressionManager.AttemptToUnlockPart(grpurchaseSlot.PurchaseID);
			return;
		}
		this.grManager.RequestPurchaseToolOrUpgrade(this, shelf, item);
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x00104CC8 File Offset: 0x00102EC8
	public ValueTuple<bool, bool> TryPurchaseAuthority(GRPlayer player, int shelf, int item)
	{
		if (this.currentActivePlayerActorNumber == -1)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentActivePlayerActorNumber);
		if (grplayer == null)
		{
			this.currentActivePlayerActorNumber = -1;
			return new ValueTuple<bool, bool>(false, false);
		}
		if (player != grplayer)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.grManager.IsAuthority())
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		if (!this.toolProgressionManager)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		return new ValueTuple<bool, bool>(true, true);
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x00104D88 File Offset: 0x00102F88
	public void ToolPurchaseResponseLocal(GRPlayer player, int shelf, int item, bool success)
	{
		if (!this.IsValidShelfItemIndex(shelf, item))
		{
			return;
		}
		if (!this.toolProgressionManager)
		{
			return;
		}
		GRToolUpgradePurchaseStationShelf.GRPurchaseSlot grpurchaseSlot = this.gameShelves[shelf].gRPurchaseSlots[item];
		GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(grpurchaseSlot.PurchaseID);
		if (partMetadata == null)
		{
			return;
		}
		if (success)
		{
			int shiftCreditCost = partMetadata.shiftCreditCost;
			if (player != null)
			{
				if (player == GRPlayer.Get(VRRig.LocalRig))
				{
					player.IncrementCoresSpentPlayer(shiftCreditCost);
					player.SendToolPurchasedTelemetry(partMetadata.name, item, shiftCreditCost, 0);
				}
				else
				{
					player.IncrementCoresSpentGroup(shiftCreditCost);
				}
				player.AddItemPurchased(partMetadata.name);
				player.SubtractShiftCredit(shiftCreditCost);
				player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.SpentCredits, (float)shiftCreditCost);
				this.reactor.RefreshScoreboards();
			}
			if (this.currentMagnetEntity != null)
			{
				this.currentMagnetEntity.transform.parent = null;
				this.currentMagnetEntity.GetComponent<Rigidbody>().isKinematic = false;
				this.currentMagnetEntity.pickupable = true;
				this.currentMagnetEntity.createData = 0L;
				this.currentMagnetEntity = null;
				this.currentMagnetEntityTypeId = -1;
			}
			UnityEvent unityEvent = this.purchaseSucceded;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
			return;
		}
		else
		{
			UnityEvent unityEvent2 = this.purchaseFailed;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
			return;
		}
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x00104EC4 File Offset: 0x001030C4
	public void InitPageSelectionWheel()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < this.gameShelves.Count; i++)
		{
			list.Add(this.gameShelves[i].ShelfName);
		}
		this.pageSelectionWheel.InitFromNameList(list);
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x00104F10 File Offset: 0x00103110
	public static Color ColorFromRGB32(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x00104F30 File Offset: 0x00103130
	public bool IsValidShelfItemIndex(int shelf, int idx)
	{
		return shelf >= 0 && shelf < this.gameShelves.Count && this.gameShelves[shelf].gRPurchaseSlots != null && idx >= 0 && idx < this.gameShelves[shelf].gRPurchaseSlots.Count && this.gameShelves[shelf].gRPurchaseSlots[idx].PurchaseID > GRToolProgressionManager.ToolParts.None;
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x00104FA0 File Offset: 0x001031A0
	private static Vector3 ExtractLossyScale(Matrix4x4 m)
	{
		float magnitude = new Vector3(m.m00, m.m10, m.m20).magnitude;
		float magnitude2 = new Vector3(m.m01, m.m11, m.m21).magnitude;
		float magnitude3 = new Vector3(m.m02, m.m12, m.m22).magnitude;
		return new Vector3(magnitude, magnitude2, magnitude3);
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x00105014 File Offset: 0x00103214
	private static void DecomposeTRS(Matrix4x4 m, out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = m.GetColumn(3);
		Vector3 vector = m.GetColumn(0);
		Vector3 vector2 = m.GetColumn(1);
		Vector3 vector3 = m.GetColumn(2);
		scale = new Vector3(vector.magnitude, vector2.magnitude, vector3.magnitude);
		vector / scale.x;
		Vector3 vector4 = vector2 / scale.y;
		Vector3 vector5 = vector3 / scale.z;
		rot = Quaternion.LookRotation(vector5, vector4);
	}

	// Token: 0x04003E46 RID: 15942
	private GhostReactor reactor;

	// Token: 0x04003E47 RID: 15943
	private GhostReactorManager grManager;

	// Token: 0x04003E48 RID: 15944
	public List<GRToolUpgradePurchaseStationShelf> gameShelves;

	// Token: 0x04003E49 RID: 15945
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04003E4A RID: 15946
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x04003E4B RID: 15947
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x04003E4C RID: 15948
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x04003E4D RID: 15949
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x04003E4E RID: 15950
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x04003E4F RID: 15951
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x04003E50 RID: 15952
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x04003E51 RID: 15953
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x04003E52 RID: 15954
	private int selectedShelf;

	// Token: 0x04003E53 RID: 15955
	private int selectedItem;

	// Token: 0x04003E54 RID: 15956
	[NonSerialized]
	public int currentActivePlayerActorNumber = -1;

	// Token: 0x04003E55 RID: 15957
	private GRToolUpgradePurchaseStationFull.ShelfMovementState shelfMovementState;

	// Token: 0x04003E56 RID: 15958
	private int currentVisibleShelfIndex;

	// Token: 0x04003E57 RID: 15959
	private int nextVisibleShelfIndex;

	// Token: 0x04003E58 RID: 15960
	private GRSpringMovement frontBackShelfMovement;

	// Token: 0x04003E59 RID: 15961
	private GRSpringMovement raiseLowerShelfMovement;

	// Token: 0x04003E5A RID: 15962
	public Transform shelfRootTransform;

	// Token: 0x04003E5B RID: 15963
	public Transform shelfBackTransform;

	// Token: 0x04003E5C RID: 15964
	public Transform shelfLowerTransform;

	// Token: 0x04003E5D RID: 15965
	public TMP_Text shelfSelectionText;

	// Token: 0x04003E5E RID: 15966
	public TMP_Text playerInfo;

	// Token: 0x04003E5F RID: 15967
	public TMP_Text itemDescription;

	// Token: 0x04003E60 RID: 15968
	public TMP_Text itemDescriptionName;

	// Token: 0x04003E61 RID: 15969
	public TMP_Text itemDescriptionAnnotation;

	// Token: 0x04003E62 RID: 15970
	public TMP_Text purchaseButtonText;

	// Token: 0x04003E63 RID: 15971
	public GorillaPhysicalButton select1;

	// Token: 0x04003E64 RID: 15972
	public GorillaPhysicalButton select2;

	// Token: 0x04003E65 RID: 15973
	public GorillaPhysicalButton select3;

	// Token: 0x04003E66 RID: 15974
	public GorillaPhysicalButton select4;

	// Token: 0x04003E67 RID: 15975
	public AudioSource audioSourceLooping;

	// Token: 0x04003E68 RID: 15976
	public AudioSource audioSourceClang;

	// Token: 0x04003E69 RID: 15977
	public float audioSourceLoopingVolume = 0.5f;

	// Token: 0x04003E6A RID: 15978
	public Material unresearchedItemMaterial;

	// Token: 0x04003E6B RID: 15979
	public AudioSource interactAudioSource;

	// Token: 0x04003E6C RID: 15980
	public IDCardScanner scanner;

	// Token: 0x04003E6D RID: 15981
	public UnityEvent purchaseSucceded;

	// Token: 0x04003E6E RID: 15982
	public UnityEvent purchaseFailed;

	// Token: 0x04003E6F RID: 15983
	public Material backlightPurchase;

	// Token: 0x04003E70 RID: 15984
	public Material backlightResearch;

	// Token: 0x04003E71 RID: 15985
	public Material backlightLocked;

	// Token: 0x04003E72 RID: 15986
	private int lastKnownLocalPlayerCredits;

	// Token: 0x04003E73 RID: 15987
	private int lastKnownLocalPlayerJuice;

	// Token: 0x04003E74 RID: 15988
	private bool needsUIRefresh;

	// Token: 0x04003E75 RID: 15989
	public Transform ropeTop;

	// Token: 0x04003E76 RID: 15990
	public Transform ropeEnd;

	// Token: 0x04003E77 RID: 15991
	public Transform magnet;

	// Token: 0x04003E78 RID: 15992
	private GameEntity currentMagnetEntity;

	// Token: 0x04003E79 RID: 15993
	private int currentMagnetEntityTypeId = -1;

	// Token: 0x04003E7A RID: 15994
	private int desiredMagnetEntityTypeId = -1;

	// Token: 0x04003E7B RID: 15995
	private float prefabMagnetHeightOffset;

	// Token: 0x04003E7C RID: 15996
	public float maxMagnetDistance = 0.75f;

	// Token: 0x04003E7D RID: 15997
	private GRSpringMovement magnetMovement;

	// Token: 0x04003E7E RID: 15998
	public GRSelectionWheel pageSelectionWheel;

	// Token: 0x04003E7F RID: 15999
	public GameObject pageSelectionHandle;

	// Token: 0x04003E80 RID: 16000
	public GameObject pageSelectionLever;

	// Token: 0x04003E81 RID: 16001
	public float playerQueueTimeLimit = 30f;

	// Token: 0x04003E82 RID: 16002
	private bool disablePurchaseButton;

	// Token: 0x04003E83 RID: 16003
	private float purchaseButtonCooldown = 2f;

	// Token: 0x04003E84 RID: 16004
	private float purchaseButtonPressed;

	// Token: 0x04003E85 RID: 16005
	private const int ShelfIndex_None = -1;

	// Token: 0x04003E87 RID: 16007
	public bool currentlyShowingText = true;

	// Token: 0x04003E88 RID: 16008
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x04003E89 RID: 16009
	private float lastRequestedActivePlayerTokenTime;

	// Token: 0x04003E8A RID: 16010
	private float requestActivePlayerTokenThrottleTime = 0.25f;

	// Token: 0x04003E8B RID: 16011
	private bool bIsGrippingLeft;

	// Token: 0x04003E8C RID: 16012
	private bool bIsGrippingRight;

	// Token: 0x04003E8D RID: 16013
	private bool bGripLeftLastFrame;

	// Token: 0x04003E8E RID: 16014
	private bool bGripRightLastFrame;

	// Token: 0x04003E8F RID: 16015
	private float maxHandleRange = 0.09f;

	// Token: 0x04003E90 RID: 16016
	private float timeSinceLastHandleBroadcast;

	// Token: 0x04003E91 RID: 16017
	private float angleOfLastHandleBroadcast;

	// Token: 0x04003E92 RID: 16018
	private float selectionWheelAngleOfLastBroadcast;

	// Token: 0x04003E93 RID: 16019
	private float quantMult = 100000f;

	// Token: 0x04003E94 RID: 16020
	private float lastHandleAngle = -10000f;

	// Token: 0x02000737 RID: 1847
	public enum ShelfMovementState
	{
		// Token: 0x04003E96 RID: 16022
		Idle,
		// Token: 0x04003E97 RID: 16023
		MoveCurrentShelfBackward,
		// Token: 0x04003E98 RID: 16024
		MoveCurrentShelfForward,
		// Token: 0x04003E99 RID: 16025
		MoveNextShelfUpward,
		// Token: 0x04003E9A RID: 16026
		MoveNextShelfDownward,
		// Token: 0x04003E9B RID: 16027
		Count
	}
}
