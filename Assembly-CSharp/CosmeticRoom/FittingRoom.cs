using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000EA2 RID: 3746
	public class FittingRoom : MonoBehaviour
	{
		// Token: 0x06005DAE RID: 23982 RVA: 0x001E13FC File Offset: 0x001DF5FC
		public void InitializeForCustomMap(bool useCustomConsoleMesh = true)
		{
			GameObject gameObject = this.consoleMesh;
			if (gameObject != null)
			{
				gameObject.SetActive(!useCustomConsoleMesh);
			}
			CosmeticsController.instance.AddFittingRoom(this);
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x001E1420 File Offset: 0x001DF620
		public void UpdateFromCart(List<CosmeticsController.CosmeticItem> currentCart, CosmeticsController.CosmeticSet tryOnSet)
		{
			this.iterator = 0;
			while (this.iterator < this.fittingRoomButtons.Length)
			{
				if (this.iterator < currentCart.Count)
				{
					bool isInTryOnSet = CosmeticsController.instance.AnyMatch(tryOnSet, currentCart[this.iterator]);
					this.fittingRoomButtons[this.iterator].SetItem(currentCart[this.iterator], isInTryOnSet);
				}
				else
				{
					this.fittingRoomButtons[this.iterator].ClearItem();
				}
				this.iterator++;
			}
		}

		// Token: 0x04006BB1 RID: 27569
		public FittingRoomButton[] fittingRoomButtons;

		// Token: 0x04006BB2 RID: 27570
		public GameObject consoleMesh;

		// Token: 0x04006BB3 RID: 27571
		private int iterator;
	}
}
