using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000744 RID: 1860
public class GRUIScoreboard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600301A RID: 12314 RVA: 0x0010715A File Offset: 0x0010535A
	public void SliceUpdate()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			this.Refresh(GhostReactor.instance.vrRigs);
		}
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x00107178 File Offset: 0x00105378
	public void Refresh(List<VRRig> vrRigs)
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			GhostReactor.instance.shiftManager.CalculatePlayerPercentages();
		}
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (!(this.entries[i] == null))
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.entries[i].gameObject.SetActive(true);
					this.entries[i].Setup(vrRigs[i], vrRigs[i].OwningNetPlayer.ActorNumber, this.currentScreen);
				}
				else
				{
					this.entries[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00107258 File Offset: 0x00105458
	public void SwitchToScreen(GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.currentScreen = screenType;
		GRUIScoreboard.ScoreboardScreen scoreboardScreen = this.currentScreen;
		if (scoreboardScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.infoTextParent.SetActive(true);
			this.calcTextParent.SetActive(false);
			this.buttonText.text = "SHOW CUT CALC";
			return;
		}
		if (scoreboardScreen != GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			return;
		}
		this.infoTextParent.SetActive(false);
		this.calcTextParent.SetActive(true);
		this.buttonText.text = "SHOW INFO";
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x001072CC File Offset: 0x001054CC
	public void SwitchState()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation);
		}
		else
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.DefaultInfo);
		}
		this.Refresh(GhostReactor.instance.vrRigs);
		GhostReactor.instance.UpdateRemoteScoreboardScreen(this.currentScreen);
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x00107306 File Offset: 0x00105506
	public static bool ValidPage(GRUIScoreboard.ScoreboardScreen screen)
	{
		return screen == GRUIScoreboard.ScoreboardScreen.DefaultInfo || screen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation;
	}

	// Token: 0x04003F13 RID: 16147
	public List<GRUIScoreboardEntry> entries;

	// Token: 0x04003F14 RID: 16148
	public TMP_Text total;

	// Token: 0x04003F15 RID: 16149
	public TMP_Text buttonText;

	// Token: 0x04003F16 RID: 16150
	public GRUIScoreboard.ScoreboardScreen currentScreen;

	// Token: 0x04003F17 RID: 16151
	public GameObject infoTextParent;

	// Token: 0x04003F18 RID: 16152
	public GameObject calcTextParent;

	// Token: 0x02000745 RID: 1861
	public enum ScoreboardScreen
	{
		// Token: 0x04003F1A RID: 16154
		DefaultInfo,
		// Token: 0x04003F1B RID: 16155
		ShiftCutCalculation
	}
}
