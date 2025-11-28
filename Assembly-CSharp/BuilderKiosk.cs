using System;
using System.Collections;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000576 RID: 1398
public class BuilderKiosk : MonoBehaviour
{
	// Token: 0x0600233E RID: 9022 RVA: 0x000B84A8 File Offset: 0x000B66A8
	private void Awake()
	{
		BuilderKiosk.nullItem = new BuilderSetManager.BuilderSetStoreItem
		{
			displayName = "NOTHING",
			playfabID = "NULL",
			isNullItem = true
		};
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000B84E4 File Offset: 0x000B66E4
	private void Start()
	{
		this._puchaseTextLocStr = this._puchaseTextLoc.StringReference;
		this._itemNameVar = (this._puchaseTextLocStr["item-name"] as StringVariable);
		this._itemCostVar = (this._puchaseTextLocStr["item-cost"] as IntVariable);
		this._currencyBalanceVar = (this._puchaseTextLocStr["currency-balance"] as IntVariable);
		this._finalLineVar = (this._puchaseTextLocStr["final-line-index"] as IntVariable);
		this.purchaseParticles.Stop();
		BuilderSetManager.instance.OnOwnedSetsUpdated.AddListener(new UnityAction(this.OnOwnedSetsUpdated));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.leftPurchaseButton.onPressed += new Action<GorillaPressableButton, bool>(this.PressLeftPurchaseItemButton);
		this.rightPurchaseButton.onPressed += new Action<GorillaPressableButton, bool>(this.PressRightPurchaseItemButton);
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.AddListener(new UnityAction(this.UpdateCountdown));
		}
		this.UpdateCountdown();
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (!this.isMiniKiosk)
		{
			this.SetupSetButtons();
		}
		if (this.availableItems.Count <= 0 || !BuilderSetManager.instance.pulledStoreItems)
		{
			this.itemToBuy = BuilderKiosk.nullItem;
			return;
		}
		this.hasInitFromPlayfab = true;
		if (this.pieceSetForSale != null)
		{
			this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
			this.UpdateLabels();
			this.UpdateDiorama();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, false);
			return;
		}
		this.itemToBuy = BuilderKiosk.nullItem;
		this.UpdateLabels();
		this.UpdateDiorama();
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
		this.ProcessPurchaseItemState(null, false);
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000B8718 File Offset: 0x000B6918
	private void UpdateCountdown()
	{
		if (!this.useTitleCountDown)
		{
			return;
		}
		if (!string.IsNullOrEmpty(BuilderTable.nextUpdateOverride) && !BuilderTable.nextUpdateOverride.Equals(this.countdownOverride))
		{
			this.countdownOverride = BuilderTable.nextUpdateOverride;
			CountdownTextDate countdown = this.countdownText.Countdown;
			countdown.CountdownTo = this.countdownOverride;
			this.countdownText.Countdown = countdown;
		}
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000B877C File Offset: 0x000B697C
	private void SetupSetButtons()
	{
		this.setsPerPage = this.setButtons.Length;
		this.totalPages = this.availableItems.Count / this.setsPerPage;
		if (this.availableItems.Count % this.setsPerPage > 0)
		{
			this.totalPages++;
		}
		this.previousPageButton.gameObject.SetActive(this.totalPages > 1);
		this.nextPageButton.gameObject.SetActive(this.totalPages > 1);
		this.previousPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.nextPageButton.myTmpText.enabled = (this.totalPages > 1);
		this.previousPageButton.onPressButton.AddListener(new UnityAction(this.OnPreviousPageClicked));
		this.nextPageButton.onPressButton.AddListener(new UnityAction(this.OnNextPageClicked));
		GorillaPressableButton[] array = this.setButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].onPressed += new Action<GorillaPressableButton, bool>(this.OnSetButtonPressed);
		}
		this.UpdateLabels();
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000B88A0 File Offset: 0x000B6AA0
	private void OnDestroy()
	{
		if (this.leftPurchaseButton != null)
		{
			this.leftPurchaseButton.onPressed -= new Action<GorillaPressableButton, bool>(this.PressLeftPurchaseItemButton);
		}
		if (this.rightPurchaseButton != null)
		{
			this.rightPurchaseButton.onPressed -= new Action<GorillaPressableButton, bool>(this.PressRightPurchaseItemButton);
		}
		if (BuilderSetManager.instance != null)
		{
			BuilderSetManager.instance.OnOwnedSetsUpdated.RemoveListener(new UnityAction(this.OnOwnedSetsUpdated));
		}
		if (CosmeticsController.instance != null)
		{
			CosmeticsController instance = CosmeticsController.instance;
			instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		}
		if (!this.isMiniKiosk)
		{
			foreach (GorillaPressableButton gorillaPressableButton in this.setButtons)
			{
				if (gorillaPressableButton != null)
				{
					gorillaPressableButton.onPressed -= new Action<GorillaPressableButton, bool>(this.OnSetButtonPressed);
				}
			}
			if (this.previousPageButton != null)
			{
				this.previousPageButton.onPressButton.RemoveListener(new UnityAction(this.OnPreviousPageClicked));
			}
			if (this.nextPageButton != null)
			{
				this.nextPageButton.onPressButton.RemoveListener(new UnityAction(this.OnNextPageClicked));
			}
		}
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		BuilderTable builderTable;
		if (BuilderTable.TryGetBuilderTableForZone(GTZone.monkeBlocks, out builderTable))
		{
			builderTable.OnTableConfigurationUpdated.RemoveListener(new UnityAction(this.UpdateCountdown));
		}
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000B8A50 File Offset: 0x000B6C50
	private void OnOwnedSetsUpdated()
	{
		if (this.hasInitFromPlayfab || !BuilderSetManager.instance.pulledStoreItems)
		{
			if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed)
			{
				this.ProcessPurchaseItemState(null, false);
			}
			return;
		}
		this.hasInitFromPlayfab = true;
		this.availableItems.Clear();
		if (this.isMiniKiosk)
		{
			this.availableItems.Add(this.pieceSetForSale);
		}
		else
		{
			this.availableItems.AddRange(BuilderSetManager.instance.GetPermanentSetsForSale());
			this.availableItems.AddRange(BuilderSetManager.instance.GetSeasonalSetsForSale());
		}
		if (this.pieceSetForSale != null)
		{
			this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(this.pieceSetForSale.GetIntIdentifier());
			this.UpdateLabels();
			this.UpdateDiorama();
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			this.ProcessPurchaseItemState(null, false);
			return;
		}
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
		this.ProcessPurchaseItemState(null, false);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000B8B44 File Offset: 0x000B6D44
	private void OnSetButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Buying && !this.animating)
		{
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
			int num = 0;
			for (int i = 0; i < this.setButtons.Length; i++)
			{
				if (button.Equals(this.setButtons[i]))
				{
					num = i;
					break;
				}
			}
			int num2 = this.pageIndex * this.setsPerPage + num;
			if (num2 < this.availableItems.Count)
			{
				BuilderPieceSet builderPieceSet = this.availableItems[num2];
				if (builderPieceSet.SetName.Equals(this.itemToBuy.displayName))
				{
					this.itemToBuy = BuilderKiosk.nullItem;
					this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				}
				else
				{
					this.itemToBuy = BuilderSetManager.instance.GetStoreItemFromSetID(builderPieceSet.GetIntIdentifier());
					this.UpdateLabels();
					this.UpdateDiorama();
				}
				this.ProcessPurchaseItemState(null, isLeft);
			}
		}
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000B8C1C File Offset: 0x000B6E1C
	private void OnPreviousPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex - 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000B8C58 File Offset: 0x000B6E58
	private void OnNextPageClicked()
	{
		int num = Mathf.Clamp(this.pageIndex + 1, 0, this.totalPages - 1);
		if (num != this.pageIndex)
		{
			this.pageIndex = num;
			this.UpdateLabels();
		}
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000B8C94 File Offset: 0x000B6E94
	private void UpdateLabels()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		for (int i = 0; i < this.setButtons.Length; i++)
		{
			int num = this.pageIndex * this.setsPerPage + i;
			if (num < this.availableItems.Count && this.availableItems[num] != null)
			{
				if (!this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(true);
					this.setButtons[i].myTmpText.gameObject.SetActive(true);
				}
				if (this.setButtons[i].myTmpText.text != this.availableItems[num].SetName.ToUpper())
				{
					this.setButtons[i].myTmpText.text = this.availableItems[num].SetName.ToUpper();
				}
				bool flag = !this.itemToBuy.isNullItem && this.availableItems[num].playfabID == this.itemToBuy.playfabID;
				if (flag != this.setButtons[i].isOn || !this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = flag;
					this.setButtons[i].buttonRenderer.material = (flag ? this.setButtons[i].pressedMaterial : this.setButtons[i].unpressedMaterial);
				}
				this.setButtons[i].enabled = true;
			}
			else
			{
				if (this.setButtons[i].gameObject.activeSelf)
				{
					this.setButtons[i].gameObject.SetActive(false);
					this.setButtons[i].myTmpText.gameObject.SetActive(false);
				}
				if (this.setButtons[i].isOn || this.setButtons[i].enabled)
				{
					this.setButtons[i].isOn = false;
					this.setButtons[i].enabled = false;
				}
			}
		}
		bool flag2 = this.pageIndex > 0 && this.totalPages > 1;
		bool flag3 = this.pageIndex < this.totalPages - 1 && this.totalPages > 1;
		if (this.previousPageButton.myTmpText.enabled != flag2)
		{
			this.previousPageButton.myTmpText.enabled = flag2;
		}
		if (this.nextPageButton.myTmpText.enabled != flag3)
		{
			this.nextPageButton.myTmpText.enabled = flag3;
		}
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000B8F2C File Offset: 0x000B712C
	public void UpdateDiorama()
	{
		if (this.isMiniKiosk)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.itemToBuy.isNullItem)
		{
			this.countdownText.gameObject.SetActive(false);
		}
		else
		{
			this.countdownText.gameObject.SetActive(BuilderSetManager.instance.IsSetSeasonal(this.itemToBuy.playfabID));
		}
		if (this.animating)
		{
			base.StopCoroutine(this.PlaySwapAnimation());
			if (this.currentDiorama != null)
			{
				Object.Destroy(this.currentDiorama);
				this.currentDiorama = null;
			}
			this.currentDiorama = this.nextDiorama;
			this.nextDiorama = null;
		}
		this.animating = true;
		if (this.nextDiorama != null)
		{
			Object.Destroy(this.nextDiorama);
			this.nextDiorama = null;
		}
		if (!this.itemToBuy.isNullItem && this.itemToBuy.displayModel != null)
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.itemToBuy.displayModel, this.nextItemDisplayPos);
		}
		else
		{
			this.nextDiorama = Object.Instantiate<GameObject>(this.emptyDisplay, this.nextItemDisplayPos);
		}
		this.itemDisplayAnimation.Rewind();
		if (this.currentDiorama != null)
		{
			this.currentDiorama.transform.SetParent(this.itemDisplayPos, false);
		}
		base.StartCoroutine(this.PlaySwapAnimation());
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000B9097 File Offset: 0x000B7297
	private IEnumerator PlaySwapAnimation()
	{
		this.itemDisplayAnimation.Play();
		yield return new WaitForSeconds(this.itemDisplayAnimation.clip.length);
		if (this.currentDiorama != null)
		{
			Object.Destroy(this.currentDiorama);
			this.currentDiorama = null;
		}
		this.currentDiorama = this.nextDiorama;
		this.nextDiorama = null;
		this.animating = false;
		yield break;
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000B90A6 File Offset: 0x000B72A6
	public void PressLeftPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("left", isLeftHand);
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000B90C4 File Offset: 0x000B72C4
	public void PressRightPurchaseItemButton(GorillaPressableButton pressedPurchaseItemButton, bool isLeftHand)
	{
		if (this.currentPurchaseItemStage != CosmeticsController.PurchaseItemStages.Start && !this.animating)
		{
			this.ProcessPurchaseItemState("right", isLeftHand);
		}
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000B90E2 File Offset: 0x000B72E2
	public void OnUpdateCurrencyBalance()
	{
		if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Start || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.CheckoutButtonPressed || this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.ItemOwned)
		{
			this.ProcessPurchaseItemState(null, false);
		}
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000B9106 File Offset: 0x000B7306
	public void ClearCheckout()
	{
		GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, this.itemToBuy);
		this.itemToBuy = BuilderKiosk.nullItem;
		this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
	}

	// Token: 0x0600234E RID: 9038 RVA: 0x000B9130 File Offset: 0x000B7330
	public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
	{
		switch (this.currentPurchaseItemStage)
		{
		case CosmeticsController.PurchaseItemStages.Start:
			this.itemToBuy = BuilderKiosk.nullItem;
			this.FormattedPurchaseText(0);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.UpdateLabels();
			this.UpdateDiorama();
			return;
		case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
			if (this.availableItems.Count > 1)
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, this.itemToBuy);
			}
			if (BuilderSetManager.instance.IsPieceSetOwnedLocally(this.itemToBuy.setID))
			{
				this.FormattedPurchaseText(1);
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
				return;
			}
			if ((ulong)this.itemToBuy.cost <= (ulong)((long)CosmeticsController.instance.currencyBalance))
			{
				this.FormattedPurchaseText(2);
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL", out text, "NO!");
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM", out text2, "YES!");
				this.leftPurchaseButton.myTmpText.text = text;
				this.rightPurchaseButton.myTmpText.text = text2;
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
				return;
			}
			this.FormattedPurchaseText(3);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			if (!this.isMiniKiosk)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
				return;
			}
			break;
		case CosmeticsController.PurchaseItemStages.ItemSelected:
			if (buttonSide == "right")
			{
				GorillaTelemetry.PostBuilderKioskEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
				this.FormattedPurchaseText(4);
				string text3;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL", out text3, "LET ME THINK ABOUT IT");
				string text4;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM", out text4, "YES! I NEED IT!");
				this.leftPurchaseButton.myTmpText.text = text4;
				this.rightPurchaseButton.myTmpText.text = text3;
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.unpressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.unpressedMaterial;
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
				this.FormattedPurchaseText(5);
				this.leftPurchaseButton.myTmpText.text = "-";
				this.rightPurchaseButton.myTmpText.text = "-";
				this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
				this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
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
			this.FormattedPurchaseText(7);
			this.audioSource.GTPlayOneShot(this.purchaseSetAudioClip, 1f);
			this.purchaseParticles.Play();
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.playfabID;
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			break;
		}
		case CosmeticsController.PurchaseItemStages.Failure:
			this.FormattedPurchaseText(6);
			this.leftPurchaseButton.myTmpText.text = "-";
			this.rightPurchaseButton.myTmpText.text = "-";
			this.leftPurchaseButton.buttonRenderer.material = this.leftPurchaseButton.pressedMaterial;
			this.rightPurchaseButton.buttonRenderer.material = this.rightPurchaseButton.pressedMaterial;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600234F RID: 9039 RVA: 0x000B95F4 File Offset: 0x000B77F4
	public void FormattedPurchaseText(int finalLineVar)
	{
		if (this._itemNameVar == null || this._itemCostVar == null || this._currencyBalanceVar == null || this._finalLineVar == null)
		{
			Debug.LogError("[LOCALIZATION::BUILDER_KIOSK] One of the dynamic variables is NULL and cannot update the [purchaseText] screen");
			return;
		}
		this._itemNameVar.Value = this.itemToBuy.displayName.ToUpper();
		this._itemCostVar.Value = (int)this.itemToBuy.cost;
		this._currencyBalanceVar.Value = CosmeticsController.instance.currencyBalance;
		this._finalLineVar.Value = finalLineVar;
	}

	// Token: 0x06002350 RID: 9040 RVA: 0x000B9680 File Offset: 0x000B7880
	public void PurchaseItem()
	{
		BuilderSetManager.instance.TryPurchaseItem(this.itemToBuy.setID, delegate(bool result)
		{
			if (result)
			{
				this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
				CosmeticsController.instance.currencyBalance -= (int)this.itemToBuy.cost;
				this.ProcessPurchaseItemState(null, this.isLastHandTouchedLeft);
				return;
			}
			this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
			this.ProcessPurchaseItemState(null, false);
		});
	}

	// Token: 0x04002E09 RID: 11785
	private const string MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM";

	// Token: 0x04002E0A RID: 11786
	private const string MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL";

	// Token: 0x04002E0B RID: 11787
	private const string MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM";

	// Token: 0x04002E0C RID: 11788
	private const string MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL_KEY = "MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL";

	// Token: 0x04002E0D RID: 11789
	public BuilderPieceSet pieceSetForSale;

	// Token: 0x04002E0E RID: 11790
	public GorillaPressableButton leftPurchaseButton;

	// Token: 0x04002E0F RID: 11791
	public GorillaPressableButton rightPurchaseButton;

	// Token: 0x04002E10 RID: 11792
	public TMP_Text purchaseText;

	// Token: 0x04002E11 RID: 11793
	[SerializeField]
	private bool isMiniKiosk;

	// Token: 0x04002E12 RID: 11794
	[SerializeField]
	private bool useTitleCountDown = true;

	// Token: 0x04002E13 RID: 11795
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableButton[] setButtons;

	// Token: 0x04002E14 RID: 11796
	[SerializeField]
	private GorillaPressableButton previousPageButton;

	// Token: 0x04002E15 RID: 11797
	[SerializeField]
	private GorillaPressableButton nextPageButton;

	// Token: 0x04002E16 RID: 11798
	private BuilderPieceSet currentSet;

	// Token: 0x04002E17 RID: 11799
	private int pageIndex;

	// Token: 0x04002E18 RID: 11800
	private int setsPerPage = 3;

	// Token: 0x04002E19 RID: 11801
	private int totalPages = 1;

	// Token: 0x04002E1A RID: 11802
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002E1B RID: 11803
	[SerializeField]
	private AudioClip purchaseSetAudioClip;

	// Token: 0x04002E1C RID: 11804
	[SerializeField]
	private ParticleSystem purchaseParticles;

	// Token: 0x04002E1D RID: 11805
	[SerializeField]
	private GameObject emptyDisplay;

	// Token: 0x04002E1E RID: 11806
	private List<BuilderPieceSet> availableItems = new List<BuilderPieceSet>(10);

	// Token: 0x04002E1F RID: 11807
	internal CosmeticsController.PurchaseItemStages currentPurchaseItemStage;

	// Token: 0x04002E20 RID: 11808
	private bool hasInitFromPlayfab;

	// Token: 0x04002E21 RID: 11809
	internal BuilderSetManager.BuilderSetStoreItem itemToBuy;

	// Token: 0x04002E22 RID: 11810
	public static BuilderSetManager.BuilderSetStoreItem nullItem;

	// Token: 0x04002E23 RID: 11811
	private GameObject currentDiorama;

	// Token: 0x04002E24 RID: 11812
	private GameObject nextDiorama;

	// Token: 0x04002E25 RID: 11813
	private bool animating;

	// Token: 0x04002E26 RID: 11814
	[SerializeField]
	private Transform itemDisplayPos;

	// Token: 0x04002E27 RID: 11815
	[SerializeField]
	private Transform nextItemDisplayPos;

	// Token: 0x04002E28 RID: 11816
	[SerializeField]
	private Animation itemDisplayAnimation;

	// Token: 0x04002E29 RID: 11817
	[SerializeField]
	private CountdownText countdownText;

	// Token: 0x04002E2A RID: 11818
	private string countdownOverride = string.Empty;

	// Token: 0x04002E2B RID: 11819
	private bool isLastHandTouchedLeft;

	// Token: 0x04002E2C RID: 11820
	private string finalLine;

	// Token: 0x04002E2D RID: 11821
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _puchaseTextLoc;

	// Token: 0x04002E2E RID: 11822
	private LocalizedString _puchaseTextLocStr;

	// Token: 0x04002E2F RID: 11823
	private StringVariable _itemNameVar;

	// Token: 0x04002E30 RID: 11824
	private IntVariable _finalLineVar;

	// Token: 0x04002E31 RID: 11825
	private IntVariable _itemCostVar;

	// Token: 0x04002E32 RID: 11826
	private IntVariable _currencyBalanceVar;
}
