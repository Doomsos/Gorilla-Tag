using System;
using System.Collections.Generic;
using System.Linq;
using Cosmetics;
using GorillaNetworking;
using GorillaNetworking.Store;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004C3 RID: 1219
public class TryOnBundlesStand : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06001F6D RID: 8045 RVA: 0x000A7254 File Offset: 0x000A5454
	private string SelectedBundlePlayFabID
	{
		get
		{
			return this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID;
		}
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000A7268 File Offset: 0x000A5468
	public static string CleanUpTitleDataValues(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text.get_Chars(0) == '"' && text.get_Chars(text.Length - 1) == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		return text;
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000A72C4 File Offset: 0x000A54C4
	private void InitalizeButtons()
	{
		this.GetTryOnButtons();
		for (int i = 0; i < this.TryOnBundleButtons.Length; i++)
		{
			if (!CosmeticsController.instance.GetItemFromDict(this.TryOnBundleButtons[i].playfabBundleID).isNullItem)
			{
				this.TryOnBundleButtons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000A7318 File Offset: 0x000A5518
	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerDefaultTextTitleDataKey, new Action<string>(this.OnComputerDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerDefaultTextTitleDataFailure), false);
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerAlreadyOwnTextTitleDataKey, new Action<string>(this.OnComputerAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerAlreadyOwnTextTitleDataFailure), false);
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonDefaultTextTitleDataKey, new Action<string>(this.OnPurchaseButtonDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonDefaultTextTitleDataFailure), false);
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonAlreadyOwnTextTitleDataKey, new Action<string>(this.OnPurchaseButtonAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonAlreadyOwnTextTitleDataFailure), false);
		this.InitalizeButtons();
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000A73CF File Offset: 0x000A55CF
	private void OnComputerDefaultTextTitleDataSuccess(string data)
	{
		this.ComputerDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000A73EE File Offset: 0x000A55EE
	private void OnComputerDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerDefaultTextTitleDataValue = "Failed to get TD Key : " + this.ComputerDefaultTextTitleDataKey;
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Computer Screen Title Data: {0}", error));
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000A7427 File Offset: 0x000A5627
	private void OnComputerAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000A7435 File Offset: 0x000A5635
	private void OnComputerAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.ComputerAlreadyOwnTextTitleDataKey;
		Debug.LogError(string.Format("Error getting Computer Already Screen Title Data: {0}", error));
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000A745D File Offset: 0x000A565D
	private void OnPurchaseButtonDefaultTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000A7488 File Offset: 0x000A5688
	private void OnPurchaseButtonDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonDefaultTextTitleDataKey;
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Default Text Title Data: {0}", error));
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000A74D7 File Offset: 0x000A56D7
	private void OnPurchaseButtonAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000A74F6 File Offset: 0x000A56F6
	private void OnPurchaseButtonAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonAlreadyOwnTextTitleDataKey;
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Already Own Text Title Data: {0}", error));
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000A7530 File Offset: 0x000A5730
	public void ClearSelectedBundle()
	{
		if (this.SelectedButtonIndex != -1)
		{
			this.TryOnBundleButtons[this.SelectedButtonIndex].isOn = false;
			if (this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID != "NULL" || this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID != "")
			{
				this.RemoveBundle(this.SelectedBundlePlayFabID);
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
				this.purchaseButton.ResetButton();
				this.selectedBundleImage.sprite = null;
				this.TryOnBundleButtons[this.SelectedButtonIndex].UpdateColor();
				this.SelectedButtonIndex = -1;
			}
		}
		this.computerScreenText.text = (this.bError ? this.computerScreeErrorText : this.ComputerDefaultTextTitleDataValue);
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000A7608 File Offset: 0x000A5808
	private void RemoveBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (string itemName in itemFromDict.bundledItems)
		{
			CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, itemName, false);
		}
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000A7660 File Offset: 0x000A5860
	private void TryOnBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (CosmeticsController.CosmeticItem cosmeticItem in CosmeticsController.instance.tryOnSet.items)
		{
			if (!Enumerable.Contains<string>(itemFromDict.bundledItems, cosmeticItem.itemName))
			{
				CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, cosmeticItem.itemName, false);
			}
		}
		foreach (string text in itemFromDict.bundledItems)
		{
			if (!CosmeticsController.instance.tryOnSet.HasItem(text))
			{
				CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, CosmeticsController.instance.GetItemFromDict(text), false, false);
			}
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000A7738 File Offset: 0x000A5938
	public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
	{
		if (pressedTryOnBundleButton.playfabBundleID == "NULL")
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Invalid bundle ID");
			return;
		}
		if (CosmeticsController.instance.GetItemFromDict(pressedTryOnBundleButton.playfabBundleID).isNullItem)
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Bundle is Null + " + pressedTryOnBundleButton.playfabBundleID);
			return;
		}
		if (this.SelectedButtonIndex != pressedTryOnBundleButton.buttonIndex)
		{
			this.ClearSelectedBundle();
		}
		switch (CosmeticsController.instance.CheckIfCosmeticSetMatchesItemSet(CosmeticsController.instance.tryOnSet, pressedTryOnBundleButton.playfabBundleID))
		{
		case CosmeticsController.EWearingCosmeticSet.NotASet:
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Item is Not A Set");
			break;
		case CosmeticsController.EWearingCosmeticSet.NotWearing:
			this.TryOnBundle(pressedTryOnBundleButton.playfabBundleID);
			this.SelectedButtonIndex = pressedTryOnBundleButton.buttonIndex;
			break;
		case CosmeticsController.EWearingCosmeticSet.Partial:
			if (pressedTryOnBundleButton.isOn)
			{
				this.ClearSelectedBundle();
			}
			else
			{
				this.TryOnBundle(pressedTryOnBundleButton.playfabBundleID);
				this.SelectedButtonIndex = pressedTryOnBundleButton.buttonIndex;
			}
			break;
		case CosmeticsController.EWearingCosmeticSet.Complete:
			this.ClearSelectedBundle();
			break;
		}
		if (this.SelectedButtonIndex != -1)
		{
			if (!this.bError)
			{
				this.selectedBundleImage.sprite = BundleManager.instance.storeBundlesById[pressedTryOnBundleButton.playfabBundleID].bundleImage;
				pressedTryOnBundleButton.isOn = true;
				this.purchaseButton.offText = this.GetPurchaseButtonText(pressedTryOnBundleButton.playfabBundleID);
				this.computerScreenText.text = this.GetComputerScreenText(pressedTryOnBundleButton.playfabBundleID);
				this.AlreadyOwnCheck();
			}
			pressedTryOnBundleButton.UpdateColor();
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			}
			pressedTryOnBundleButton.isOn = false;
			this.selectedBundleImage.sprite = null;
			this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			this.purchaseButton.ResetButton();
			this.purchaseButton.UpdateColor();
		}
		CosmeticsController.instance.UpdateShoppingCart();
		CosmeticsController.instance.UpdateWornCosmetics(true);
		pressedTryOnBundleButton.UpdateColor();
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000A7930 File Offset: 0x000A5B30
	private string GetComputerScreenText(string playfabBundleID)
	{
		return BundleManager.instance.storeBundlesById[playfabBundleID].bundleDescriptionText;
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000A7949 File Offset: 0x000A5B49
	private string GetPurchaseButtonText(string playfabBundleID)
	{
		return BundleManager.instance.storeBundlesById[playfabBundleID].purchaseButtonText;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000A7962 File Offset: 0x000A5B62
	public void PurchaseButtonPressed()
	{
		if (this.SelectedButtonIndex == -1)
		{
			return;
		}
		CosmeticsController.instance.PurchaseBundle(BundleManager.instance.storeBundlesById[this.SelectedBundlePlayFabID], this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>());
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x000A799C File Offset: 0x000A5B9C
	public void AlreadyOwnCheck()
	{
		if (this.SelectedButtonIndex == -1)
		{
			return;
		}
		if (BundleManager.instance.storeBundlesById[this.SelectedBundlePlayFabID].isOwned)
		{
			this.purchaseButton.AlreadyOwn();
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerAlreadyOwnTextTitleDataValue;
				return;
			}
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = this.GetBundleComputerText(this.SelectedBundlePlayFabID);
			}
			this.purchaseButton.UpdateColor();
		}
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x000A7A20 File Offset: 0x000A5C20
	public void GetTryOnButtons()
	{
		StoreBundleData[] tryOnButtons = BundleManager.instance.GetTryOnButtons();
		for (int i = 0; i < this.TryOnBundleButtons.Length; i++)
		{
			if (i < tryOnButtons.Length)
			{
				if (tryOnButtons[i] != null && tryOnButtons[i].playfabBundleID != "NULL" && tryOnButtons[i].bundleImage != null)
				{
					this.TryOnBundleButtons[i].playfabBundleID = tryOnButtons[i].playfabBundleID;
					this.BundleIcons[i].sprite = tryOnButtons[i].bundleImage;
				}
				else
				{
					this.TryOnBundleButtons[i].playfabBundleID = "NULL";
					this.BundleIcons[i].sprite = null;
				}
			}
			else
			{
				this.TryOnBundleButtons[i].playfabBundleID = "NULL";
				this.BundleIcons[i].sprite = null;
			}
			this.TryOnBundleButtons[i].UpdateColor();
		}
	}

	// Token: 0x06001F82 RID: 8066 RVA: 0x000A7B03 File Offset: 0x000A5D03
	public void UpdateBundles(StoreBundleData[] Bundles)
	{
		Debug.LogWarning("TryOnBundlesStand - UpdateBundles is an editor only function!");
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x000A7B10 File Offset: 0x000A5D10
	private string GetBundleComputerText(string PlayFabID)
	{
		StoreBundle storeBundle;
		if (BundleManager.instance.storeBundlesById.TryGetValue(PlayFabID, ref storeBundle))
		{
			return storeBundle.bundleDescriptionText;
		}
		return "ERROR THIS DOES NOT EXIST YET";
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000A7B3F File Offset: 0x000A5D3F
	public void ErrorCompleting()
	{
		this.bError = true;
		this.purchaseButton.ErrorHappened();
		this.computerScreenText.text = this.computerScreeErrorText;
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000A7B64 File Offset: 0x000A5D64
	bool IBuildValidation.BuildValidationCheck()
	{
		ICreatorCodeProvider creatorCodeProvider;
		if (this.creatorCodeProvider == null || !this.creatorCodeProvider.TryGetComponent<ICreatorCodeProvider>(ref creatorCodeProvider))
		{
			Debug.LogError(base.name + " has no Creator Code Provider. This will break bundle purchasing.");
			return false;
		}
		return true;
	}

	// Token: 0x040029CD RID: 10701
	[SerializeField]
	private TryOnBundleButton[] TryOnBundleButtons;

	// Token: 0x040029CE RID: 10702
	[SerializeField]
	private Image[] BundleIcons;

	// Token: 0x040029CF RID: 10703
	[SerializeField]
	private GameObject creatorCodeProvider;

	// Token: 0x040029D0 RID: 10704
	[Header("The Index of the Selected Bundle from CosmeticsBundle Array in CosmeticsController")]
	private int SelectedButtonIndex = -1;

	// Token: 0x040029D1 RID: 10705
	public TryOnPurchaseButton purchaseButton;

	// Token: 0x040029D2 RID: 10706
	public Image selectedBundleImage;

	// Token: 0x040029D3 RID: 10707
	public Text computerScreenText;

	// Token: 0x040029D4 RID: 10708
	public string ComputerDefaultTextTitleDataKey;

	// Token: 0x040029D5 RID: 10709
	[SerializeField]
	private string ComputerDefaultTextTitleDataValue = "";

	// Token: 0x040029D6 RID: 10710
	public string ComputerAlreadyOwnTextTitleDataKey;

	// Token: 0x040029D7 RID: 10711
	[SerializeField]
	private string ComputerAlreadyOwnTextTitleDataValue = "";

	// Token: 0x040029D8 RID: 10712
	public string PurchaseButtonDefaultTextTitleDataKey;

	// Token: 0x040029D9 RID: 10713
	[SerializeField]
	private string PurchaseButtonDefaultTextTitleDataValue = "";

	// Token: 0x040029DA RID: 10714
	public string PurchaseButtonAlreadyOwnTextTitleDataKey;

	// Token: 0x040029DB RID: 10715
	[SerializeField]
	private string PurchaseButtonAlreadyOwnTextTitleDataValue = "";

	// Token: 0x040029DC RID: 10716
	private bool bError;

	// Token: 0x040029DD RID: 10717
	[Header("Error Text for Computer Screen")]
	public string computerScreeErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME, AND MAKE SURE YOU HAVE A STABLE INTERNET CONNECTION. ";

	// Token: 0x040029DE RID: 10718
	private List<StoreBundle> storeBundles = new List<StoreBundle>();
}
