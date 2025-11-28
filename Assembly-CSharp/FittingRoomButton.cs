using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000467 RID: 1127
public class FittingRoomButton : GorillaPressableButton
{
	// Token: 0x06001C86 RID: 7302 RVA: 0x000974AF File Offset: 0x000956AF
	public override void Start()
	{
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
	}

	// Token: 0x06001C87 RID: 7303 RVA: 0x000974C4 File Offset: 0x000956C4
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

	// Token: 0x06001C88 RID: 7304 RVA: 0x000975F7 File Offset: 0x000957F7
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		CosmeticsController.instance.PressFittingRoomButton(this, isLeftHand);
	}

	// Token: 0x06001C89 RID: 7305 RVA: 0x0009760E File Offset: 0x0009580E
	public void SetItem(CosmeticsController.CosmeticItem item, bool isInTryOnSet)
	{
		this.currentCosmeticItem = item;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.currentCosmeticItem.itemPicture;
		}
		this.isOn = isInTryOnSet;
		this.UpdateColor();
	}

	// Token: 0x06001C8A RID: 7306 RVA: 0x00097648 File Offset: 0x00095848
	public void ClearItem()
	{
		if (this.currentCosmeticItem.isNullItem)
		{
			return;
		}
		this.currentCosmeticItem = CosmeticsController.instance.nullItem;
		if (this.currentCosmeticSprite.IsNotNull())
		{
			this.currentCosmeticSprite.sprite = this.blankSprite;
		}
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04002693 RID: 9875
	public CosmeticsController.CosmeticItem currentCosmeticItem;

	// Token: 0x04002694 RID: 9876
	[SerializeField]
	private SpriteRenderer currentCosmeticSprite;

	// Token: 0x04002695 RID: 9877
	[SerializeField]
	private Sprite blankSprite;

	// Token: 0x04002696 RID: 9878
	public string noCosmeticText;
}
