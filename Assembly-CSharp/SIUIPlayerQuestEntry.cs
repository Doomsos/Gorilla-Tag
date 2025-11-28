using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000152 RID: 338
public class SIUIPlayerQuestEntry : MonoBehaviour
{
	// Token: 0x060008EF RID: 2287 RVA: 0x00030662 File Offset: 0x0002E862
	private void Awake()
	{
		this.lastQuestId = -1;
		this.lastQuestProgress = -1;
	}

	// Token: 0x04000B01 RID: 2817
	public Image background;

	// Token: 0x04000B02 RID: 2818
	public SIUIProgressBar progress;

	// Token: 0x04000B03 RID: 2819
	public TextMeshProUGUI questDescription;

	// Token: 0x04000B04 RID: 2820
	public GameObject completeOverlay;

	// Token: 0x04000B05 RID: 2821
	public GameObject questInfo;

	// Token: 0x04000B06 RID: 2822
	public GameObject noQuestAvailable;

	// Token: 0x04000B07 RID: 2823
	public GameObject newQuestTag;

	// Token: 0x04000B08 RID: 2824
	public int lastQuestId;

	// Token: 0x04000B09 RID: 2825
	public int lastQuestProgress;
}
