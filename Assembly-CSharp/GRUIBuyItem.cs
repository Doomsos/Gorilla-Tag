using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200073F RID: 1855
public class GRUIBuyItem : MonoBehaviour
{
	// Token: 0x06002FE4 RID: 12260 RVA: 0x00105EFE File Offset: 0x001040FE
	public void Setup(int standId)
	{
		this.standId = standId;
		this.buyItemButton.onPressButton.AddListener(new UnityAction(this.OnBuyItem));
		this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
	}

	// Token: 0x06002FE5 RID: 12261 RVA: 0x00002789 File Offset: 0x00000989
	public void OnBuyItem()
	{
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x00105F3E File Offset: 0x0010413E
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x04003ED4 RID: 16084
	[SerializeField]
	private GorillaPressableButton buyItemButton;

	// Token: 0x04003ED5 RID: 16085
	[SerializeField]
	private Text itemInfoLabel;

	// Token: 0x04003ED6 RID: 16086
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04003ED7 RID: 16087
	[SerializeField]
	private GameEntity entityPrefab;

	// Token: 0x04003ED8 RID: 16088
	private int entityTypeId;

	// Token: 0x04003ED9 RID: 16089
	private int standId;
}
