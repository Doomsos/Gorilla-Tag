using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CosmeticRoom
{
	// Token: 0x02000EA3 RID: 3747
	public class ItemCheckout : MonoBehaviour
	{
		// Token: 0x06005DB1 RID: 23985 RVA: 0x001E14B0 File Offset: 0x001DF6B0
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene, bool useCustomCounterMesh = true)
		{
			GameObject gameObject = this.checkoutCounterMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomCounterMesh);
			}
			GameObject gameObject2 = this.purchaseScreenMesh;
			if (gameObject2 != null)
			{
				gameObject2.SetActive(useCustomCounterMesh);
			}
			this.originalScene = customMapScene;
			customMapTryOnArea.AddCollider(this.checkoutTryOnArea);
			CosmeticsController.instance.AddItemCheckout(this);
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x001E1504 File Offset: 0x001DF704
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (customMapTryOnArea.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.checkoutTryOnArea);
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x001E151C File Offset: 0x001DF71C
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticItem itemToBuy)
		{
			this.iterator = 0;
			while (this.iterator < this.checkoutCartButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isCurrentItemToBuy = currentCart[this.iterator].itemName == itemToBuy.itemName;
					this.checkoutCartButtons[this.iterator].SetItem(currentCart[this.iterator], isCurrentItemToBuy);
				}
				else
				{
					this.checkoutCartButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x001E15B0 File Offset: 0x001DF7B0
		public void UpdatePurchaseText(string newText, string leftPurchaseButtonText, string rightPurchaseButtonText, bool leftButtonOn, bool rightButtonOn)
		{
			if (this.purchaseText.IsNotNull())
			{
				this.purchaseText.text = newText;
			}
			if (this.purchaseTextTMP.IsNotNull())
			{
				this.purchaseTextTMP.text = newText;
			}
			if (!leftPurchaseButtonText.IsNullOrEmpty())
			{
				this.leftPurchaseButton.SetText(leftPurchaseButtonText);
				this.leftPurchaseButton.buttonRenderer.material = (leftButtonOn ? this.leftPurchaseButton.pressedMaterial : this.leftPurchaseButton.unpressedMaterial);
			}
			if (!rightPurchaseButtonText.IsNullOrEmpty())
			{
				this.rightPurchaseButton.SetText(rightPurchaseButtonText);
				this.rightPurchaseButton.buttonRenderer.material = (rightButtonOn ? this.rightPurchaseButton.pressedMaterial : this.rightPurchaseButton.unpressedMaterial);
			}
		}

		// Token: 0x06005DB5 RID: 23989 RVA: 0x001E166F File Offset: 0x001DF86F
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x04006BB4 RID: 27572
		public CheckoutCartButton[] checkoutCartButtons;

		// Token: 0x04006BB5 RID: 27573
		public PurchaseItemButton leftPurchaseButton;

		// Token: 0x04006BB6 RID: 27574
		public PurchaseItemButton rightPurchaseButton;

		// Token: 0x04006BB7 RID: 27575
		[HideInInspector]
		public Text purchaseText;

		// Token: 0x04006BB8 RID: 27576
		public TMP_Text purchaseTextTMP;

		// Token: 0x04006BB9 RID: 27577
		public HeadModel checkoutHeadModel;

		// Token: 0x04006BBA RID: 27578
		public Collider checkoutTryOnArea;

		// Token: 0x04006BBB RID: 27579
		public GameObject checkoutCounterMesh;

		// Token: 0x04006BBC RID: 27580
		public GameObject purchaseScreenMesh;

		// Token: 0x04006BBD RID: 27581
		private Scene originalScene;

		// Token: 0x04006BBE RID: 27582
		private int iterator;
	}
}
