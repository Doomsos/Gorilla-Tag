using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200009C RID: 156
public class DevWatchSelectableItem : MonoBehaviour
{
	// Token: 0x060003D9 RID: 985 RVA: 0x00017524 File Offset: 0x00015724
	public void Init(NetworkObject obj)
	{
		this.SelectedObject = obj;
		this.ItemName.text = obj.name;
		this.Button.onClick.AddListener(delegate()
		{
			this.OnSelected.Invoke(this.ItemName.text, this.SelectedObject);
		});
	}

	// Token: 0x04000458 RID: 1112
	public Button Button;

	// Token: 0x04000459 RID: 1113
	public TextMeshProUGUI ItemName;

	// Token: 0x0400045A RID: 1114
	public NetworkObject SelectedObject;

	// Token: 0x0400045B RID: 1115
	public Action<string, NetworkObject> OnSelected;
}
