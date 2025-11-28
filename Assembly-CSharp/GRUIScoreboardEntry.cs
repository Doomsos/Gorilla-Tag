using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000746 RID: 1862
public class GRUIScoreboardEntry : MonoBehaviour
{
	// Token: 0x06003022 RID: 12322 RVA: 0x00107311 File Offset: 0x00105511
	public void Setup(VRRig vrRig, int playerActorId, GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.playerActorId = playerActorId;
		this.Refresh(vrRig, screenType);
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x00107324 File Offset: 0x00105524
	private void Refresh(VRRig vrRig, GRUIScoreboard.ScoreboardScreen screenType)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (!(vrRig != null) || !(grplayer != null))
		{
			this.playerNameLabel.text = "";
			this.playerCurrencyLabel.text = "";
			this.playerTitleLabel.text = "";
			this.playerCutLabel.text = "";
			this.currencySet = 0;
			return;
		}
		if (!this.playerNameLabel.text.Equals(vrRig.playerNameVisible))
		{
			this.playerNameLabel.text = vrRig.playerNameVisible;
		}
		if (screenType != GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			if (screenType == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
			{
				this.defaultUIParent.SetActive(false);
				this.shiftCutParent.SetActive(true);
				if (GhostReactor.instance.shiftManager != null && (GhostReactor.instance.shiftManager.ShiftActive || GhostReactor.instance.shiftManager.ShiftTotalEarned >= 0))
				{
					int num = Mathf.FloorToInt(grplayer.ShiftPlayTime / 60f);
					int num2 = Mathf.FloorToInt(grplayer.ShiftPlayTime - (float)(num * 60));
					this.playerTimeLabel.text = string.Format("{0:00}:{1:00}", num, num2);
					this.playerPercentageLabel.text = "%" + Mathf.Floor(grplayer.ShiftPlayTime / GhostReactor.instance.shiftManager.TotalPlayTime * 100f).ToString();
				}
				else
				{
					this.playerTimeLabel.text = "n/a";
					this.playerPercentageLabel.text = "n/a";
				}
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		else
		{
			this.defaultUIParent.SetActive(true);
			this.shiftCutParent.SetActive(false);
			if (grplayer.ShiftCredits != this.currencySet)
			{
				this.currencySet = grplayer.ShiftCredits;
				this.playerCurrencyLabel.text = this.currencySet.ToString();
			}
			string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
			if (titleNameAndGrade != this.titleSet)
			{
				this.titleSet = titleNameAndGrade;
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		if (GhostReactor.instance.shiftManager == null || GhostReactor.instance.shiftManager.ShiftActive)
		{
			this.playerCutLabel.text = "-";
			return;
		}
		this.playerCutLabel.text = grplayer.LastShiftCut.ToString();
	}

	// Token: 0x04003F1C RID: 16156
	[SerializeField]
	private TMP_Text playerNameLabel;

	// Token: 0x04003F1D RID: 16157
	[SerializeField]
	private TMP_Text playerCutLabel;

	// Token: 0x04003F1E RID: 16158
	public GameObject defaultUIParent;

	// Token: 0x04003F1F RID: 16159
	[SerializeField]
	private TMP_Text playerTitleLabel;

	// Token: 0x04003F20 RID: 16160
	[SerializeField]
	private TMP_Text playerCurrencyLabel;

	// Token: 0x04003F21 RID: 16161
	public GameObject shiftCutParent;

	// Token: 0x04003F22 RID: 16162
	[SerializeField]
	private TMP_Text playerTimeLabel;

	// Token: 0x04003F23 RID: 16163
	[SerializeField]
	private TMP_Text playerPercentageLabel;

	// Token: 0x04003F24 RID: 16164
	private int playerActorId = -1;

	// Token: 0x04003F25 RID: 16165
	private int currencySet = -1;

	// Token: 0x04003F26 RID: 16166
	private string titleSet = "";
}
