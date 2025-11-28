using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000127 RID: 295
[DefaultExecutionOrder(500)]
public class SIPurchaseTerminal : MonoBehaviour, ITouchScreenStation
{
	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0002B028 File Offset: 0x00029228
	public SIScreenRegion ScreenRegion
	{
		get
		{
			return this.screenRegion;
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002B030 File Offset: 0x00029230
	private void OnEnable()
	{
		if (CosmeticsController.hasInstance)
		{
			this.DelayedOnEnable();
			return;
		}
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0002B060 File Offset: 0x00029260
	private void DelayedOnEnable()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.DelayedOnEnable));
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Combine(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
		this.OnUpdateCurrencyBalance();
		this.PopupBackgroundScreen.SetActive(false);
		this.ConfirmPurchasePopupScreen.SetActive(false);
		this.PendingPurchasePopupScreen.SetActive(false);
		this.PurchaseCompletePopupScreen.SetActive(false);
		this.InsufficientFundsPopupScreen.SetActive(false);
		this.UnableToCompletePurchasePopupScreen.SetActive(false);
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, true);
		this.purchaseSize = 1;
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0002B118 File Offset: 0x00029318
	private void OnDisable()
	{
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnUpdateCurrencyBalance));
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002B142 File Offset: 0x00029342
	public void UpdateCurrentTechPoints()
	{
		this.PurchaseAmountCurrentTechPointsCount.text = SIPlayer.LocalPlayer.CurrentProgression.resourceArray[0].ToString();
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002B169 File Offset: 0x00029369
	private void OnUpdateCurrencyBalance()
	{
		this.PurchaseAmountCurrentShinyRockCount.text = CosmeticsController.instance.currencyBalance.ToString().ToUpperInvariant();
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00002789 File Offset: 0x00000989
	public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
	{
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002B18C File Offset: 0x0002938C
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
	{
		switch (this.currentState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Purchase)
			{
				this.SelectPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Next)
			{
				this.IncreasePurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.DecreasePurcahse();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ConfirmPurchase();
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
				return;
			}
			break;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
			{
				this.ReturnToBaseScreen();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002B217 File Offset: 0x00029417
	private void IncreasePurchase()
	{
		this.purchaseSize = Math.Min(this.purchaseSize + 1, this.maxPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002B238 File Offset: 0x00029438
	private void DecreasePurcahse()
	{
		this.purchaseSize = Math.Max(this.purchaseSize - 1, this.minPurchaseSize);
		this.UpdatePurchaseAmount();
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002B25C File Offset: 0x0002945C
	private void UpdatePurchaseAmount()
	{
		this.PurchaseAmountShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.PurchaseAmountTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.ConfirmPurchaseShinyRockCount.text = (this.purchaseSize * this.costPerTechPoint).ToString().ToUpperInvariant();
		this.ConfirmPurchaseTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
		this.PurchasedTechPointCount.text = this.purchaseSize.ToString().ToUpperInvariant();
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002B304 File Offset: 0x00029504
	private void SelectPurchase()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup, false);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0002B310 File Offset: 0x00029510
	private void ConfirmPurchase()
	{
		int num = this.purchaseSize * this.costPerTechPoint;
		if (CosmeticsController.instance.currencyBalance < num)
		{
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup, false);
			return;
		}
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup, false);
		ProgressionManager.Instance.PurchaseTechPoints(this.purchaseSize, delegate
		{
			SIProgression.Instance.SendPurchaseTechPointsData(this.purchaseSize);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup, false);
			ProgressionManager.Instance.RefreshUserInventory();
		}, delegate(string error)
		{
			Debug.LogError("[SIPurchaseTerminal] PurchaseTechPoints failed: " + error);
			this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup, false);
		});
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002B373 File Offset: 0x00029573
	private void ReturnToBaseScreen()
	{
		this.UpdateState(SIPurchaseTerminal.PurchaseTerminalState.PurchaseAmountSelection, false);
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002B37D File Offset: 0x0002957D
	private void UpdateState(SIPurchaseTerminal.PurchaseTerminalState newState, bool forceUpdate = false)
	{
		if (!forceUpdate && this.currentState == newState)
		{
			return;
		}
		this.SetScreenVisibility(this.currentState, false);
		this.currentState = newState;
		this.SetScreenVisibility(this.currentState, true);
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002B3B0 File Offset: 0x000295B0
	private void SetScreenVisibility(SIPurchaseTerminal.PurchaseTerminalState screenState, bool isEnabled)
	{
		switch (screenState)
		{
		case SIPurchaseTerminal.PurchaseTerminalState.ConfirmPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.ConfirmPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PendingPurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PendingPurchasePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.PurchaseCompletePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.PurchaseCompletePopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.InsufficientFundsPopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.InsufficientFundsPopupScreen.SetActive(isEnabled);
			return;
		case SIPurchaseTerminal.PurchaseTerminalState.UnableToCompletePurchasePopup:
			this.PopupBackgroundScreen.SetActive(isEnabled);
			this.UnableToCompletePurchasePopupScreen.SetActive(isEnabled);
			return;
		default:
			return;
		}
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00013E33 File Offset: 0x00012033
	GameObject ITouchScreenStation.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040009B2 RID: 2482
	private SIPurchaseTerminal.PurchaseTerminalState currentState;

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private SIScreenRegion screenRegion;

	// Token: 0x040009B4 RID: 2484
	[SerializeField]
	private GameObject PopupBackgroundScreen;

	// Token: 0x040009B5 RID: 2485
	[SerializeField]
	private GameObject ConfirmPurchasePopupScreen;

	// Token: 0x040009B6 RID: 2486
	[SerializeField]
	private GameObject PurchaseCompletePopupScreen;

	// Token: 0x040009B7 RID: 2487
	[SerializeField]
	private GameObject PendingPurchasePopupScreen;

	// Token: 0x040009B8 RID: 2488
	[SerializeField]
	private GameObject InsufficientFundsPopupScreen;

	// Token: 0x040009B9 RID: 2489
	[SerializeField]
	private GameObject UnableToCompletePurchasePopupScreen;

	// Token: 0x040009BA RID: 2490
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountShinyRockCount;

	// Token: 0x040009BB RID: 2491
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountTechPointCount;

	// Token: 0x040009BC RID: 2492
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentShinyRockCount;

	// Token: 0x040009BD RID: 2493
	[SerializeField]
	private TextMeshProUGUI PurchaseAmountCurrentTechPointsCount;

	// Token: 0x040009BE RID: 2494
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseShinyRockCount;

	// Token: 0x040009BF RID: 2495
	[SerializeField]
	private TextMeshProUGUI ConfirmPurchaseTechPointCount;

	// Token: 0x040009C0 RID: 2496
	[SerializeField]
	private TextMeshProUGUI PurchasedTechPointCount;

	// Token: 0x040009C1 RID: 2497
	[SerializeField]
	private int maxPurchaseSize = 10;

	// Token: 0x040009C2 RID: 2498
	[SerializeField]
	private int minPurchaseSize = 1;

	// Token: 0x040009C3 RID: 2499
	[SerializeField]
	private int costPerTechPoint = 100;

	// Token: 0x040009C4 RID: 2500
	private int purchaseSize = 1;

	// Token: 0x02000128 RID: 296
	public enum PurchaseTerminalState
	{
		// Token: 0x040009C6 RID: 2502
		PurchaseAmountSelection,
		// Token: 0x040009C7 RID: 2503
		ConfirmPurchasePopup,
		// Token: 0x040009C8 RID: 2504
		PendingPurchasePopup,
		// Token: 0x040009C9 RID: 2505
		PurchaseCompletePopup,
		// Token: 0x040009CA RID: 2506
		InsufficientFundsPopup,
		// Token: 0x040009CB RID: 2507
		UnableToCompletePurchasePopup
	}
}
