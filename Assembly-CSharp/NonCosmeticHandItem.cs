using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020003F9 RID: 1017
public class NonCosmeticHandItem : MonoBehaviour
{
	// Token: 0x060018EE RID: 6382 RVA: 0x000858AB File Offset: 0x00083AAB
	public void EnableItem(bool enable)
	{
		if (this.itemPrefab)
		{
			this.itemPrefab.gameObject.SetActive(enable);
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x060018EF RID: 6383 RVA: 0x000858CB File Offset: 0x00083ACB
	public bool IsEnabled
	{
		get
		{
			return this.itemPrefab && this.itemPrefab.gameObject.activeSelf;
		}
	}

	// Token: 0x04002245 RID: 8773
	public CosmeticsController.CosmeticSlots cosmeticSlots;

	// Token: 0x04002246 RID: 8774
	public GameObject itemPrefab;
}
