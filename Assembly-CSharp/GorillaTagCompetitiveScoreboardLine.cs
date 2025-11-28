using System;
using TMPro;
using UnityEngine;

// Token: 0x020007C0 RID: 1984
public class GorillaTagCompetitiveScoreboardLine : MonoBehaviour
{
	// Token: 0x0600343D RID: 13373 RVA: 0x00118998 File Offset: 0x00116B98
	public void SetPlayer(string playerName, Sprite icon)
	{
		this.playerNameDisplay.text = playerName;
		this.rankSprite.sprite = icon;
	}

	// Token: 0x0600343E RID: 13374 RVA: 0x001189B4 File Offset: 0x00116BB4
	public void SetScore(float untaggedTime, int tagCount)
	{
		int num = Mathf.FloorToInt(untaggedTime);
		int num2 = num / 60;
		int num3 = num % 60;
		this.untaggedTimeDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
		this.tagCountDisplay.text = tagCount.ToString();
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x00118A03 File Offset: 0x00116C03
	public void SetPredictedResult(GorillaTagCompetitiveScoreboard.PredictedResult result)
	{
		this.resultSprite.sprite = this.resultSprites[(int)result];
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x00118A18 File Offset: 0x00116C18
	public void DisplayPredictedResults(bool bShow)
	{
		this.resultSprite.gameObject.SetActive(bShow);
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x00118A2B File Offset: 0x00116C2B
	public void SetInfected(bool infected)
	{
		this.playerNameDisplay.color = (infected ? Color.red : Color.white);
	}

	// Token: 0x0400428E RID: 17038
	public SpriteRenderer rankSprite;

	// Token: 0x0400428F RID: 17039
	public TMP_Text playerNameDisplay;

	// Token: 0x04004290 RID: 17040
	public TMP_Text untaggedTimeDisplay;

	// Token: 0x04004291 RID: 17041
	public TMP_Text tagCountDisplay;

	// Token: 0x04004292 RID: 17042
	public SpriteRenderer resultSprite;

	// Token: 0x04004293 RID: 17043
	public Sprite[] resultSprites;
}
