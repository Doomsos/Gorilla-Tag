using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200074C RID: 1868
public class GRVendingMachine : MonoBehaviour
{
	// Token: 0x0600303F RID: 12351 RVA: 0x00108311 File Offset: 0x00106511
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x0010831A File Offset: 0x0010651A
	public Transform GetSpawnMarker()
	{
		return this.itemSpawnLocation;
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00108322 File Offset: 0x00106522
	public void NavButtonPressedLeft()
	{
		this.hIndex = Mathf.Max(0, this.hIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x0010833E File Offset: 0x0010653E
	public void NavButtonPressedRight()
	{
		this.hIndex = Mathf.Min(this.hIndex + 1, this.horizontalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x00108361 File Offset: 0x00106561
	public void NavButtonPressedUp()
	{
		this.vIndex = Mathf.Max(0, this.vIndex - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x0010837D File Offset: 0x0010657D
	public void NavButtonPressedDown()
	{
		this.vIndex = Mathf.Min(this.vIndex + 1, this.verticalSteps - 1);
		this.RefreshCardReaderDisplay();
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x001083A0 File Offset: 0x001065A0
	public void RequestPurchase()
	{
		if (!this.currentlyVending)
		{
			int num = this.vIndex * this.horizontalSteps + this.hIndex;
			if (num >= 0 && num < this.vendingEntries.Count)
			{
				this.vendingIndex = num;
				if (this.vendingCoroutine != null)
				{
					base.StopCoroutine(this.vendingCoroutine);
				}
				this.vendingCoroutine = base.StartCoroutine(this.VendingCoroutine());
			}
		}
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x0010840C File Offset: 0x0010660C
	private void RefreshCardReaderDisplay()
	{
		int num = this.vIndex * this.horizontalSteps + this.hIndex;
		if (num >= 0 && num < this.vendingEntries.Count)
		{
			int entityTypeId = this.vendingEntries[num].GetEntityTypeId();
			int itemCost = this.reactor.GetItemCost(entityTypeId);
			this.cardDisplayText.text = this.vendingEntries[num].itemName + "\n" + itemCost.ToString();
		}
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x0010848F File Offset: 0x0010668F
	private void Update()
	{
		if (!this.currentlyVending)
		{
			this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime);
		}
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x001084CC File Offset: 0x001066CC
	private bool MoveTransportToSlot(int x, int y, int rows, int cols, float xSpeed, float ySpeed, float dt)
	{
		Vector3 vector = Vector3.Lerp(this.horizontalMin.position, this.horizontalMax.position, (float)x / (float)(rows - 1));
		Vector3 vector2 = Vector3.Lerp(this.verticalMin.position, this.verticalMax.position, (float)y / (float)(cols - 1));
		this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, vector, xSpeed * dt);
		this.verticalTransport.position = Vector3.MoveTowards(this.verticalTransport.position, vector2, ySpeed * dt);
		float sqrMagnitude = (this.horizontalTransport.position - vector).sqrMagnitude;
		float sqrMagnitude2 = (this.verticalTransport.position - vector2).sqrMagnitude;
		return sqrMagnitude > 0.001f || sqrMagnitude2 > 0.001f;
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x001085A6 File Offset: 0x001067A6
	private IEnumerator VendingCoroutine()
	{
		this.currentlyVending = true;
		while (this.MoveTransportToSlot(this.hIndex, this.vIndex, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
		{
			yield return null;
		}
		int entityTypeId = this.vendingEntries[this.vendingIndex].GetEntityTypeId();
		int itemCost = this.reactor.GetItemCost(entityTypeId);
		if (this.debugUnlimitedPurchasing || VRRig.LocalRig.GetComponent<GRPlayer>().ShiftCredits >= itemCost)
		{
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(true);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
			float depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
			while (depositPosSqDist > 0.001f)
			{
				this.horizontalTransport.position = Vector3.MoveTowards(this.horizontalTransport.position, this.depositLocation.position, this.horizontalSpeed * Time.deltaTime);
				depositPosSqDist = (this.horizontalTransport.position - this.depositLocation.position).sqrMagnitude;
				yield return null;
			}
			this.vendingEntries[this.vendingIndex].transportVisual.gameObject.SetActive(false);
			while (this.MoveTransportToSlot(this.horizontalSteps - 1, this.verticalSteps - 1, this.horizontalSteps, this.verticalSteps, this.horizontalSpeed, this.verticalSpeed, Time.deltaTime))
			{
				yield return null;
			}
		}
		this.currentlyVending = false;
		yield break;
	}

	// Token: 0x04003F45 RID: 16197
	[SerializeField]
	private Transform horizontalTransport;

	// Token: 0x04003F46 RID: 16198
	[SerializeField]
	private Transform verticalTransport;

	// Token: 0x04003F47 RID: 16199
	[SerializeField]
	private Transform horizontalMin;

	// Token: 0x04003F48 RID: 16200
	[SerializeField]
	private Transform horizontalMax;

	// Token: 0x04003F49 RID: 16201
	[SerializeField]
	private Transform verticalMin;

	// Token: 0x04003F4A RID: 16202
	[SerializeField]
	private Transform verticalMax;

	// Token: 0x04003F4B RID: 16203
	[SerializeField]
	private Transform depositLocation;

	// Token: 0x04003F4C RID: 16204
	[SerializeField]
	private Transform itemSpawnLocation;

	// Token: 0x04003F4D RID: 16205
	[SerializeField]
	private TMP_Text cardDisplayText;

	// Token: 0x04003F4E RID: 16206
	[SerializeField]
	private int horizontalSteps = 4;

	// Token: 0x04003F4F RID: 16207
	[SerializeField]
	private int verticalSteps = 3;

	// Token: 0x04003F50 RID: 16208
	[SerializeField]
	private float horizontalSpeed = 0.25f;

	// Token: 0x04003F51 RID: 16209
	[SerializeField]
	private float verticalSpeed = 0.25f;

	// Token: 0x04003F52 RID: 16210
	[SerializeField]
	private bool debugUnlimitedPurchasing;

	// Token: 0x04003F53 RID: 16211
	[SerializeField]
	private List<GRVendingMachine.VendingEntry> vendingEntries = new List<GRVendingMachine.VendingEntry>();

	// Token: 0x04003F54 RID: 16212
	private int hIndex;

	// Token: 0x04003F55 RID: 16213
	private int vIndex;

	// Token: 0x04003F56 RID: 16214
	private bool currentlyVending;

	// Token: 0x04003F57 RID: 16215
	private int vendingIndex;

	// Token: 0x04003F58 RID: 16216
	private Coroutine vendingCoroutine;

	// Token: 0x04003F59 RID: 16217
	public int VendingMachineId;

	// Token: 0x04003F5A RID: 16218
	private GhostReactor reactor;

	// Token: 0x0200074D RID: 1869
	[Serializable]
	public struct VendingEntry
	{
		// Token: 0x0600304B RID: 12363 RVA: 0x001085EC File Offset: 0x001067EC
		public int GetEntityTypeId()
		{
			if (!this.entityTypeIdSet)
			{
				this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
				this.entityTypeIdSet = true;
			}
			return this.entityTypeId;
		}

		// Token: 0x04003F5B RID: 16219
		public Transform transportVisual;

		// Token: 0x04003F5C RID: 16220
		public GameEntity entityPrefab;

		// Token: 0x04003F5D RID: 16221
		public string itemName;

		// Token: 0x04003F5E RID: 16222
		private int entityTypeId;

		// Token: 0x04003F5F RID: 16223
		private bool entityTypeIdSet;
	}
}
