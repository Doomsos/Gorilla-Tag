using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
public class CosmeticWardrobe : MonoBehaviour
{
	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06001F9F RID: 8095 RVA: 0x000A811C File Offset: 0x000A631C
	// (set) Token: 0x06001FA0 RID: 8096 RVA: 0x000A8124 File Offset: 0x000A6324
	public bool UseTemporarySet
	{
		get
		{
			return this.m_useTemporarySet;
		}
		set
		{
			bool flag = value != this.m_useTemporarySet;
			this.m_useTemporarySet = value;
			if (flag)
			{
				this.HandleCosmeticsUpdated();
			}
		}
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000A8144 File Offset: 0x000A6344
	private void Start()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (this.cosmeticCategoryButtons[i].category == CosmeticWardrobe.selectedCategory)
			{
				CosmeticWardrobe.selectedCategoryIndex = i;
				break;
			}
		}
		for (int j = 0; j < this.cosmeticCollectionDisplays.Length; j++)
		{
			this.cosmeticCollectionDisplays[j].displayHead.transform.localScale = this.startingHeadSize;
		}
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += new Action<Color>(this.HandleLocalColorChanged);
			this.HandleLocalColorChanged(GorillaTagger.Instance.offlineVRRig.playerColor);
		}
		this.nextSelection.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedNextSelection);
		this.prevSelection.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedPrevSelection);
		for (int k = 0; k < this.cosmeticCollectionDisplays.Length; k++)
		{
			this.cosmeticCollectionDisplays[k].selectButton.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedSelectCosmeticButton);
		}
		for (int l = 0; l < this.cosmeticCategoryButtons.Length; l++)
		{
			this.cosmeticCategoryButtons[l].button.onPressed += new Action<GorillaPressableButton, bool>(this.HandleChangeCategory);
			this.cosmeticCategoryButtons[l].slot1RemovedItem = CosmeticsController.instance.nullItem;
			this.cosmeticCategoryButtons[l].slot2RemovedItem = CosmeticsController.instance.nullItem;
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Combine(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Combine(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Combine(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedPrevOutfitButton);
		this.nextOutfit.onPressed += new Action<GorillaPressableButton, bool>(this.HandlePressedNextOutfitButton);
		this.HandleCosmeticsUpdated();
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000A8384 File Offset: 0x000A6584
	private void OnDestroy()
	{
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= new Action<Color>(this.HandleLocalColorChanged);
		}
		this.nextSelection.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedNextSelection);
		this.prevSelection.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedPrevSelection);
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			this.cosmeticCollectionDisplays[i].selectButton.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedSelectCosmeticButton);
		}
		for (int j = 0; j < this.cosmeticCategoryButtons.Length; j++)
		{
			this.cosmeticCategoryButtons[j].button.onPressed -= new Action<GorillaPressableButton, bool>(this.HandleChangeCategory);
		}
		CosmeticsController instance = CosmeticsController.instance;
		instance.OnCosmeticsUpdated = (Action)Delegate.Remove(instance.OnCosmeticsUpdated, new Action(this.HandleCosmeticsUpdated));
		CosmeticsController instance2 = CosmeticsController.instance;
		instance2.OnOutfitsUpdated = (Action)Delegate.Remove(instance2.OnOutfitsUpdated, new Action(this.UpdateOutfitButtons));
		CosmeticWardrobe.OnWardrobeUpdateCategories = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateCategories, new Action(this.UpdateCategoryButtons));
		CosmeticWardrobe.OnWardrobeUpdateDisplays = (Action)Delegate.Remove(CosmeticWardrobe.OnWardrobeUpdateDisplays, new Action(this.UpdateCosmeticDisplays));
		this.previousOutfit.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedPrevOutfitButton);
		this.nextOutfit.onPressed -= new Action<GorillaPressableButton, bool>(this.HandlePressedNextOutfitButton);
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000A8518 File Offset: 0x000A6718
	private void HandlePressedNextSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex += this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex >= CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory))
		{
			CosmeticWardrobe.startingDisplayIndex = 0;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays.Invoke();
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000A8568 File Offset: 0x000A6768
	private void HandlePressedPrevSelection(GorillaPressableButton button, bool isLeft)
	{
		CosmeticWardrobe.startingDisplayIndex -= this.cosmeticCollectionDisplays.Length;
		if (CosmeticWardrobe.startingDisplayIndex < 0)
		{
			int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
			int num;
			if (categorySize % this.cosmeticCollectionDisplays.Length == 0)
			{
				num = categorySize - this.cosmeticCollectionDisplays.Length;
			}
			else
			{
				num = categorySize / this.cosmeticCollectionDisplays.Length;
				num *= this.cosmeticCollectionDisplays.Length;
			}
			CosmeticWardrobe.startingDisplayIndex = num;
		}
		Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
		if (onWardrobeUpdateDisplays == null)
		{
			return;
		}
		onWardrobeUpdateDisplays.Invoke();
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000A85E8 File Offset: 0x000A67E8
	private void HandlePressedSelectCosmeticButton(GorillaPressableButton button, bool isLeft)
	{
		int i = 0;
		while (i < this.cosmeticCollectionDisplays.Length)
		{
			if (this.cosmeticCollectionDisplays[i].selectButton == button)
			{
				CosmeticsController.instance.PressWardrobeItemButton(this.cosmeticCollectionDisplays[i].currentCosmeticItem, isLeft, this.m_useTemporarySet);
				if (isLeft)
				{
					this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot2RemovedItem = CosmeticsController.instance.nullItem;
					return;
				}
				this.cosmeticCategoryButtons[CosmeticWardrobe.selectedCategoryIndex].slot1RemovedItem = CosmeticsController.instance.nullItem;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000A8680 File Offset: 0x000A6880
	private void HandleChangeCategory(GorillaPressableButton button, bool isLeft)
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.button == button)
			{
				if (CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category)
				{
					CosmeticsController.CosmeticItem cosmeticItem = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, true, this.m_useTemporarySet);
					}
					CosmeticsController.CosmeticItem cosmeticItem2 = CosmeticsController.instance.nullItem;
					if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
					{
						cosmeticItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, true, this.m_useTemporarySet);
					}
					bool flag = CosmeticWardrobe.selectedCategory == CosmeticsController.CosmeticCategory.Arms;
					if (!cosmeticItem.isNullItem || !cosmeticItem2.isNullItem)
					{
						if (!cosmeticItem.isNullItem)
						{
							cosmeticWardrobeCategory.slot1RemovedItem = cosmeticItem;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem, flag, this.m_useTemporarySet);
						}
						if (!cosmeticItem2.isNullItem)
						{
							cosmeticWardrobeCategory.slot2RemovedItem = cosmeticItem2;
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticItem2, !flag, this.m_useTemporarySet);
						}
						Action onWardrobeUpdateDisplays = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays != null)
						{
							onWardrobeUpdateDisplays.Invoke();
						}
						Action onWardrobeUpdateCategories = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories == null)
						{
							return;
						}
						onWardrobeUpdateCategories.Invoke();
						return;
					}
					else if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem || !cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
					{
						if (!cosmeticWardrobeCategory.slot1RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot1RemovedItem, flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot1RemovedItem = CosmeticsController.instance.nullItem;
						}
						if (!cosmeticWardrobeCategory.slot2RemovedItem.isNullItem)
						{
							CosmeticsController.instance.PressWardrobeItemButton(cosmeticWardrobeCategory.slot2RemovedItem, !flag, this.m_useTemporarySet);
							cosmeticWardrobeCategory.slot2RemovedItem = CosmeticsController.instance.nullItem;
						}
						Action onWardrobeUpdateDisplays2 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
						if (onWardrobeUpdateDisplays2 != null)
						{
							onWardrobeUpdateDisplays2.Invoke();
						}
						Action onWardrobeUpdateCategories2 = CosmeticWardrobe.OnWardrobeUpdateCategories;
						if (onWardrobeUpdateCategories2 == null)
						{
							return;
						}
						onWardrobeUpdateCategories2.Invoke();
						return;
					}
				}
				else
				{
					CosmeticWardrobe.selectedCategory = cosmeticWardrobeCategory.category;
					CosmeticWardrobe.selectedCategoryIndex = i;
					CosmeticWardrobe.startingDisplayIndex = 0;
					Action onWardrobeUpdateDisplays3 = CosmeticWardrobe.OnWardrobeUpdateDisplays;
					if (onWardrobeUpdateDisplays3 != null)
					{
						onWardrobeUpdateDisplays3.Invoke();
					}
					Action onWardrobeUpdateCategories3 = CosmeticWardrobe.OnWardrobeUpdateCategories;
					if (onWardrobeUpdateCategories3 == null)
					{
						return;
					}
					onWardrobeUpdateCategories3.Invoke();
				}
				return;
			}
		}
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000A88A4 File Offset: 0x000A6AA4
	private void HandleCosmeticsUpdated()
	{
		string[] currentlyWornCosmetics = CosmeticsController.instance.GetCurrentlyWornCosmetics(this.m_useTemporarySet);
		bool[] currentRightEquippedSided = CosmeticsController.instance.GetCurrentRightEquippedSided(this.m_useTemporarySet);
		this.currentEquippedDisplay.SetCosmeticActiveArray(currentlyWornCosmetics, currentRightEquippedSided);
		this.UpdateCategoryButtons();
		this.UpdateCosmeticDisplays();
		this.UpdateOutfitButtons();
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000A88F8 File Offset: 0x000A6AF8
	private void HandleLocalColorChanged(Color newColor)
	{
		MeshRenderer component = this.currentEquippedDisplay.GetComponent<MeshRenderer>();
		if (component != null)
		{
			component.material.color = newColor;
		}
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000A8926 File Offset: 0x000A6B26
	private void HandlePressedPrevOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(false);
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000A8935 File Offset: 0x000A6B35
	private void HandlePressedNextOutfitButton(GorillaPressableButton button, bool isLeft)
	{
		CosmeticsController.instance.PressWardrobeScrollOutfit(true);
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000A8944 File Offset: 0x000A6B44
	private void UpdateCosmeticDisplays()
	{
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			CosmeticsController.CosmeticItem cosmetic = CosmeticsController.instance.GetCosmetic(CosmeticWardrobe.selectedCategory, CosmeticWardrobe.startingDisplayIndex + i);
			this.cosmeticCollectionDisplays[i].currentCosmeticItem = cosmetic;
			this.cosmeticCollectionDisplays[i].displayHead.SetCosmeticActive(cosmetic.displayName, false);
			this.cosmeticCollectionDisplays[i].selectButton.enabled = !cosmetic.isNullItem;
			this.cosmeticCollectionDisplays[i].selectButton.isOn = (!cosmetic.isNullItem && CosmeticsController.instance.IsCosmeticEquipped(cosmetic, this.m_useTemporarySet));
			this.cosmeticCollectionDisplays[i].selectButton.UpdateColor();
		}
		int categorySize = CosmeticsController.instance.GetCategorySize(CosmeticWardrobe.selectedCategory);
		this.nextSelection.enabled = (categorySize > this.cosmeticCollectionDisplays.Length);
		this.nextSelection.UpdateColor();
		this.prevSelection.enabled = (categorySize > this.cosmeticCollectionDisplays.Length);
		this.prevSelection.UpdateColor();
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000A8A5C File Offset: 0x000A6C5C
	private void UpdateCategoryButtons()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			CosmeticWardrobe.CosmeticWardrobeCategory cosmeticWardrobeCategory = this.cosmeticCategoryButtons[i];
			if (cosmeticWardrobeCategory.slot1 != CosmeticsController.CosmeticSlots.Count)
			{
				CosmeticsController.CosmeticItem slotItem = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot1, false, this.m_useTemporarySet);
				if (cosmeticWardrobeCategory.slot2 != CosmeticsController.CosmeticSlots.Count)
				{
					CosmeticsController.CosmeticItem slotItem2 = CosmeticsController.instance.GetSlotItem(cosmeticWardrobeCategory.slot2, false, this.m_useTemporarySet);
					if (slotItem.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
					}
					else if (slotItem2.bothHandsHoldable)
					{
						cosmeticWardrobeCategory.button.SetIcon(slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
					else
					{
						cosmeticWardrobeCategory.button.SetDualIcon(slotItem.isNullItem ? null : slotItem.itemPicture, slotItem2.isNullItem ? null : slotItem2.itemPicture);
					}
				}
				else
				{
					cosmeticWardrobeCategory.button.SetIcon(slotItem.isNullItem ? null : slotItem.itemPicture);
				}
			}
			int categorySize = CosmeticsController.instance.GetCategorySize(cosmeticWardrobeCategory.category);
			cosmeticWardrobeCategory.button.enabled = (categorySize > 0);
			cosmeticWardrobeCategory.button.isOn = (CosmeticWardrobe.selectedCategory == cosmeticWardrobeCategory.category);
			cosmeticWardrobeCategory.button.UpdateColor();
		}
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000A8BBC File Offset: 0x000A6DBC
	private void UpdateOutfitButtons()
	{
		bool enabled = CosmeticsController.CanScrollOutfits();
		int num = CosmeticsController.SelectedOutfit + 1;
		this.nextOutfit.enabled = enabled;
		this.previousOutfit.enabled = enabled;
		this.nextOutfit.UpdateColor();
		this.previousOutfit.UpdateColor();
		this.outfitText.text = "Outfit #" + num.ToString();
		this.outfitTextOutline.text = "Outfit #" + num.ToString();
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000A8C40 File Offset: 0x000A6E40
	public bool WardrobeButtonsInitialized()
	{
		for (int i = 0; i < this.cosmeticCategoryButtons.Length; i++)
		{
			if (!this.cosmeticCategoryButtons[i].button.Initialized)
			{
				return false;
			}
		}
		for (int i = 0; i < this.cosmeticCollectionDisplays.Length; i++)
		{
			if (!this.cosmeticCollectionDisplays[i].selectButton.Initialized)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040029F3 RID: 10739
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeSelection[] cosmeticCollectionDisplays;

	// Token: 0x040029F4 RID: 10740
	[SerializeField]
	private CosmeticWardrobe.CosmeticWardrobeCategory[] cosmeticCategoryButtons;

	// Token: 0x040029F5 RID: 10741
	[SerializeField]
	private HeadModel currentEquippedDisplay;

	// Token: 0x040029F6 RID: 10742
	[SerializeField]
	private GorillaPressableButton nextSelection;

	// Token: 0x040029F7 RID: 10743
	[SerializeField]
	private GorillaPressableButton prevSelection;

	// Token: 0x040029F8 RID: 10744
	[SerializeField]
	private bool m_useTemporarySet;

	// Token: 0x040029F9 RID: 10745
	[SerializeField]
	private CosmeticButton previousOutfit;

	// Token: 0x040029FA RID: 10746
	[SerializeField]
	private CosmeticButton nextOutfit;

	// Token: 0x040029FB RID: 10747
	[SerializeField]
	private TMP_Text outfitText;

	// Token: 0x040029FC RID: 10748
	[SerializeField]
	private TMP_Text outfitTextOutline;

	// Token: 0x040029FD RID: 10749
	private static int selectedCategoryIndex = 0;

	// Token: 0x040029FE RID: 10750
	private static CosmeticsController.CosmeticCategory selectedCategory = CosmeticsController.CosmeticCategory.Hat;

	// Token: 0x040029FF RID: 10751
	private static int startingDisplayIndex = 0;

	// Token: 0x04002A00 RID: 10752
	private static int selectedOutfitIndex = 0;

	// Token: 0x04002A01 RID: 10753
	private static Action OnWardrobeUpdateCategories;

	// Token: 0x04002A02 RID: 10754
	private static Action OnWardrobeUpdateDisplays;

	// Token: 0x04002A03 RID: 10755
	public Vector3 startingHeadSize = new Vector3(0.25f, 0.25f, 0.25f);

	// Token: 0x020004CA RID: 1226
	[Serializable]
	public class CosmeticWardrobeSelection
	{
		// Token: 0x04002A04 RID: 10756
		public HeadModel displayHead;

		// Token: 0x04002A05 RID: 10757
		public CosmeticButton selectButton;

		// Token: 0x04002A06 RID: 10758
		public CosmeticsController.CosmeticItem currentCosmeticItem;
	}

	// Token: 0x020004CB RID: 1227
	[Serializable]
	public class CosmeticWardrobeCategory
	{
		// Token: 0x04002A07 RID: 10759
		public CosmeticCategoryButton button;

		// Token: 0x04002A08 RID: 10760
		public CosmeticsController.CosmeticCategory category;

		// Token: 0x04002A09 RID: 10761
		public CosmeticsController.CosmeticSlots slot1 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002A0A RID: 10762
		public CosmeticsController.CosmeticSlots slot2 = CosmeticsController.CosmeticSlots.Count;

		// Token: 0x04002A0B RID: 10763
		public CosmeticsController.CosmeticItem slot1RemovedItem;

		// Token: 0x04002A0C RID: 10764
		public CosmeticsController.CosmeticItem slot2RemovedItem;
	}
}
