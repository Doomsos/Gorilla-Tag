using System;
using TMPro;
using UnityEngine;

// Token: 0x020007BC RID: 1980
public class GorillaTagCompetitiveRankDisplay : MonoBehaviour
{
	// Token: 0x0600342B RID: 13355 RVA: 0x001183F2 File Offset: 0x001165F2
	private void OnEnable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged += new Action<int, int>(this.HandleRankedSubtierChanged);
		this.HandleRankedSubtierChanged(0, 0);
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x00118412 File Offset: 0x00116612
	private void OnDisable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged -= new Action<int, int>(this.HandleRankedSubtierChanged);
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x0011842C File Offset: 0x0011662C
	public void HandleRankedSubtierChanged(int questSubTier, int pcSubTier)
	{
		float currentELO = RankedProgressionManager.Instance.GetCurrentELO();
		int progressionRankIndex = RankedProgressionManager.Instance.GetProgressionRankIndex(currentELO);
		this.UpdateRankIcons(progressionRankIndex);
		this.UpdateRankProgress(RankedProgressionManager.Instance.GetProgressionRankProgress());
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x00118468 File Offset: 0x00116668
	private void UpdateRankIcons(int currentRank)
	{
		this.currentRankSprite.sprite = RankedProgressionManager.Instance.GetProgressionRankIcon(currentRank);
		this.currentRank_Name.text = RankedProgressionManager.Instance.GetProgressionRankName().ToUpper();
		bool flag = currentRank < RankedProgressionManager.Instance.MaxRank;
		bool flag2 = currentRank > 0;
		this.nextRankSprite.gameObject.SetActive(flag);
		this.nextText.gameObject.SetActive(flag);
		this.nextRank_Name.gameObject.SetActive(flag);
		if (flag)
		{
			this.nextRankSprite.sprite = RankedProgressionManager.Instance.GetNextProgressionRankIcon(currentRank);
			this.nextRank_Name.text = RankedProgressionManager.Instance.GetNextProgressionRankName(currentRank).ToUpper();
		}
		this.prevRankSprite.gameObject.SetActive(flag2);
		this.prevText.gameObject.SetActive(flag2);
		this.prevRank_Name.gameObject.SetActive(flag2);
		if (flag2)
		{
			this.prevRankSprite.sprite = RankedProgressionManager.Instance.GetPrevProgressionRankIcon(currentRank);
			this.prevRank_Name.text = RankedProgressionManager.Instance.GetPrevProgressionRankName(currentRank).ToUpper();
		}
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x00118588 File Offset: 0x00116788
	private void UpdateRankProgress(float percent)
	{
		percent = Mathf.Clamp01(percent);
		Vector2 size = this.progressBar.size;
		size.x = this.progressBarSize * percent;
		this.progressBar.size = size;
	}

	// Token: 0x04004271 RID: 17009
	[SerializeField]
	private SpriteRenderer progressBar;

	// Token: 0x04004272 RID: 17010
	[SerializeField]
	private float progressBarSize = 100f;

	// Token: 0x04004273 RID: 17011
	[SerializeField]
	private SpriteRenderer currentRankSprite;

	// Token: 0x04004274 RID: 17012
	[SerializeField]
	private SpriteRenderer prevRankSprite;

	// Token: 0x04004275 RID: 17013
	[SerializeField]
	private SpriteRenderer nextRankSprite;

	// Token: 0x04004276 RID: 17014
	[SerializeField]
	private TextMeshPro currentRank_Name;

	// Token: 0x04004277 RID: 17015
	[SerializeField]
	private TextMeshPro prevText;

	// Token: 0x04004278 RID: 17016
	[SerializeField]
	private TextMeshPro nextText;

	// Token: 0x04004279 RID: 17017
	[SerializeField]
	private TextMeshPro prevRank_Name;

	// Token: 0x0400427A RID: 17018
	[SerializeField]
	private TextMeshPro nextRank_Name;
}
