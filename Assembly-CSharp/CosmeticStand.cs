using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000460 RID: 1120
public class CosmeticStand : GorillaPressableButton
{
	// Token: 0x06001C61 RID: 7265 RVA: 0x00096ACC File Offset: 0x00094CCC
	public void InitializeCosmetic()
	{
		this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
		if (this.slotPriceText != null)
		{
			this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
		}
	}

	// Token: 0x06001C62 RID: 7266 RVA: 0x00096B4A File Offset: 0x00094D4A
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCosmeticStandButton(this);
	}

	// Token: 0x04002670 RID: 9840
	public CosmeticsController.CosmeticItem thisCosmeticItem;

	// Token: 0x04002671 RID: 9841
	public string thisCosmeticName;

	// Token: 0x04002672 RID: 9842
	public HeadModel thisHeadModel;

	// Token: 0x04002673 RID: 9843
	public Text slotPriceText;

	// Token: 0x04002674 RID: 9844
	public Text addToCartText;

	// Token: 0x04002675 RID: 9845
	[Tooltip("If this is true then this cosmetic stand should have already been updated when the 'Update Cosmetic Stands' button was pressed in the CosmeticsController inspector.")]
	public bool skipMe;
}
