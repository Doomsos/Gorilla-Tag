using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000458 RID: 1112
public class CheckoutCartButton : GorillaPressableButton
{
	// Token: 0x06001C4B RID: 7243 RVA: 0x00096477 File Offset: 0x00094677
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06001C4C RID: 7244 RVA: 0x0009648C File Offset: 0x0009468C
	public override void UpdateColor()
	{
		if (this.currentCosmeticItem.itemName == "null")
		{
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			if (this.myText.IsNotNull())
			{
				this.myText.text = this.noCosmeticText;
			}
			if (this.myTmpText.IsNotNull())
			{
				this.myTmpText.text = this.noCosmeticText;
			}
			if (this.myTmpText2.IsNotNull())
			{
				this.myTmpText2.text = this.noCosmeticText;
				return;
			}
		}
		else
		{
			if (this.isOn)
			{
				if (this.buttonRenderer.IsNotNull())
				{
					this.buttonRenderer.material = this.pressedMaterial;
				}
				this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
				return;
			}
			if (this.buttonRenderer.IsNotNull())
			{
				this.buttonRenderer.material = this.unpressedMaterial;
			}
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
		}
	}

	// Token: 0x06001C4D RID: 7245 RVA: 0x000965BF File Offset: 0x000947BF
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressCheckoutCartButton(this, isLeftHand);
	}

	// Token: 0x06001C4E RID: 7246 RVA: 0x000965D5 File Offset: 0x000947D5
	public void SetItem(CosmeticsController.CosmeticItem item, bool isCurrentItemToBuy)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isCurrentItemToBuy;
		this.UpdateColor();
	}

	// Token: 0x06001C4F RID: 7247 RVA: 0x0009660E File Offset: 0x0009480E
	public void ClearItem()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002652 RID: 9810
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002653 RID: 9811
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002654 RID: 9812
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002655 RID: 9813
	public string noCosmeticText;
}
