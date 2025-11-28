using System;
using TMPro;
using UnityEngine;

// Token: 0x020001F4 RID: 500
public class QuestDisplay : MonoBehaviour
{
	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x0004868A File Offset: 0x0004688A
	public bool IsChanged
	{
		get
		{
			return this.quest.lastChange > this._lastUpdate;
		}
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x000486A0 File Offset: 0x000468A0
	public void UpdateDisplay()
	{
		this.text.text = this.quest.GetTextDescription();
		if (this.quest.isQuestComplete)
		{
			this.progressDisplay.SetVisible(false);
		}
		else if (this.quest.requiredOccurenceCount > 1)
		{
			this.progressDisplay.SetProgress(this.quest.occurenceCount, this.quest.requiredOccurenceCount);
			this.progressDisplay.SetVisible(true);
		}
		else
		{
			this.progressDisplay.SetVisible(false);
		}
		this.UpdateCompletionIndicator();
		this._lastUpdate = Time.frameCount;
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00048738 File Offset: 0x00046938
	private void UpdateCompletionIndicator()
	{
		bool isQuestComplete = this.quest.isQuestComplete;
		bool flag = !isQuestComplete && this.quest.requiredOccurenceCount == 1;
		this.dailyIncompleteIndicator.SetActive(this.quest.isDailyQuest && flag);
		this.dailyCompleteIndicator.SetActive(this.quest.isDailyQuest && isQuestComplete);
		this.weeklyIncompleteIndicator.SetActive(!this.quest.isDailyQuest && flag);
		this.weeklyCompleteIndicator.SetActive(!this.quest.isDailyQuest && isQuestComplete);
	}

	// Token: 0x0400109A RID: 4250
	[SerializeField]
	private ProgressDisplay progressDisplay;

	// Token: 0x0400109B RID: 4251
	[SerializeField]
	private TMP_Text text;

	// Token: 0x0400109C RID: 4252
	[SerializeField]
	private TMP_Text statusText;

	// Token: 0x0400109D RID: 4253
	[SerializeField]
	private GameObject dailyIncompleteIndicator;

	// Token: 0x0400109E RID: 4254
	[SerializeField]
	private GameObject dailyCompleteIndicator;

	// Token: 0x0400109F RID: 4255
	[SerializeField]
	private GameObject weeklyIncompleteIndicator;

	// Token: 0x040010A0 RID: 4256
	[SerializeField]
	private GameObject weeklyCompleteIndicator;

	// Token: 0x040010A1 RID: 4257
	[NonSerialized]
	public RotatingQuest quest;

	// Token: 0x040010A2 RID: 4258
	private int _lastUpdate = -1;
}
