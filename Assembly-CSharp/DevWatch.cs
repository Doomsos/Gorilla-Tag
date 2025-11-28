using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200009A RID: 154
public class DevWatch : MonoBehaviour
{
	// Token: 0x060003D0 RID: 976 RVA: 0x00017350 File Offset: 0x00015550
	private void Awake()
	{
		this.SearchButton.SearchEvent.AddListener(new UnityAction(this.SearchItems));
		this.TakeOwnershipButton.onClick.AddListener(new UnityAction(this.TakeOwneshipOfItem));
		this.DestroyObjectButton.onClick.AddListener(new UnityAction(this.TryDestroyItem));
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000173B4 File Offset: 0x000155B4
	public void SearchItems()
	{
		this.FoundNetworkObjects.Clear();
		RaycastHit[] array = Physics.SphereCastAll(new Ray(this.RayCastStartPos.position, this.RayCastDirection.position - this.RayCastStartPos.position), 0.3f, 100f);
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				NetworkObject networkObject;
				if (raycastHit.collider.gameObject.TryGetComponent<NetworkObject>(ref networkObject))
				{
					this.FoundNetworkObjects.Add(networkObject);
				}
			}
		}
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00017448 File Offset: 0x00015648
	public void Cleanup()
	{
		this.FoundNetworkObjects.Clear();
		if (this.Items.Count > 0)
		{
			for (int i = this.Items.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.Items[i]);
			}
		}
		this.Items.Clear();
		this.Panel1.SetActive(true);
		this.Panel2.SetActive(false);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x000174BA File Offset: 0x000156BA
	public void ItemSelected(DevWatchSelectableItem item)
	{
		this.Panel1.SetActive(false);
		this.Panel2.SetActive(true);
		this.SelectedItem = item;
		this.SelectedItemName.text = item.ItemName.text;
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00002789 File Offset: 0x00000989
	public void TryDestroyItem()
	{
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00002789 File Offset: 0x00000989
	public void TakeOwneshipOfItem()
	{
	}

	// Token: 0x0400044A RID: 1098
	public DevWatchButton SearchButton;

	// Token: 0x0400044B RID: 1099
	public GameObject Panel1;

	// Token: 0x0400044C RID: 1100
	public GameObject Panel2;

	// Token: 0x0400044D RID: 1101
	public DevWatchSelectableItem SelectableItemPrefab;

	// Token: 0x0400044E RID: 1102
	public List<DevWatchSelectableItem> Items;

	// Token: 0x0400044F RID: 1103
	public Transform RayCastStartPos;

	// Token: 0x04000450 RID: 1104
	public Transform RayCastDirection;

	// Token: 0x04000451 RID: 1105
	public Transform ItemsFoundContainer;

	// Token: 0x04000452 RID: 1106
	public Button TakeOwnershipButton;

	// Token: 0x04000453 RID: 1107
	public Button DestroyObjectButton;

	// Token: 0x04000454 RID: 1108
	public List<NetworkObject> FoundNetworkObjects = new List<NetworkObject>();

	// Token: 0x04000455 RID: 1109
	public TextMeshProUGUI SelectedItemName;

	// Token: 0x04000456 RID: 1110
	public DevWatchSelectableItem SelectedItem;
}
