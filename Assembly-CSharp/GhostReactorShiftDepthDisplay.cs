using System;
using System.Collections.Generic;
using System.Text;
using GorillaTagScripts.GhostReactor;
using TMPro;
using UnityEngine;

// Token: 0x02000655 RID: 1621
[Serializable]
public class GhostReactorShiftDepthDisplay
{
	// Token: 0x06002990 RID: 10640 RVA: 0x000DFA19 File Offset: 0x000DDC19
	public void Setup()
	{
		this.StopDelveDeeperFX();
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000DFA21 File Offset: 0x000DDC21
	public int GetRewardXP()
	{
		return this.reactor.GetDepthLevel() * 10 + 10;
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000DFA34 File Offset: 0x000DDC34
	public void RefreshDisplay()
	{
		int depthLevel = this.reactor.GetDepthLevel();
		this.reactor.GetDepthLevelConfig(depthLevel);
		this.reactor.GetDepthLevelConfig(depthLevel + 1);
		switch (this.shiftManager.GetState())
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
		case GhostReactorShiftManager.State.ShiftActive:
		{
			foreach (TMP_Text tmp_Text in this.logoFrames)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			this.cachedStringBuilder.Clear();
			this.cachedStringBuilder.Append("<color=grey>Team Goals:</color>\n");
			int num = 0;
			if (this.shiftManager.coresRequiredToDelveDeeper > 0)
			{
				int num2 = Math.Min(this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected), this.shiftManager.coresRequiredToDelveDeeper);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Format("Deposit {0} Cores ", this.shiftManager.coresRequiredToDelveDeeper));
				stringBuilder.Append(string.Format("({0}/{1})", num2, this.shiftManager.coresRequiredToDelveDeeper));
				stringBuilder.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder);
				num++;
			}
			if (this.shiftManager.sentientCoresRequiredToDelveDeeper > 0)
			{
				int num3 = Math.Min(this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected), this.shiftManager.sentientCoresRequiredToDelveDeeper);
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(string.Format("Collect {0} Seeds ", this.shiftManager.sentientCoresRequiredToDelveDeeper));
				stringBuilder2.Append(string.Format("({0}/{1})", num3, this.shiftManager.sentientCoresRequiredToDelveDeeper));
				stringBuilder2.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder2);
				num++;
			}
			foreach (GREnemyCount grenemyCount in this.shiftManager.killsRequiredToDelveDeeper)
			{
				if (grenemyCount.Count > 0)
				{
					int num4 = Math.Min(this.shiftManager.shiftStats.EnemyKills[grenemyCount.EnemyType], grenemyCount.Count);
					StringBuilder stringBuilder3 = new StringBuilder();
					stringBuilder3.Append(string.Format("Kill {0} {1}s ", grenemyCount.Count, grenemyCount.EnemyType));
					stringBuilder3.Append(string.Format("({0}/{1})", num4, grenemyCount.Count));
					stringBuilder3.Append("\n");
					this.cachedStringBuilder.Append(stringBuilder3);
				}
			}
			if (this.shiftManager.maxPlayerDeaths >= 0)
			{
				StringBuilder stringBuilder4 = new StringBuilder();
				stringBuilder4.Append(string.Format("Limit Incidents to {0} ", this.shiftManager.maxPlayerDeaths));
				stringBuilder4.Append(string.Format("({0} so far)", this.shiftManager.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths)));
				stringBuilder4.Append("\n");
				this.cachedStringBuilder.Append(stringBuilder4);
				num++;
			}
			this.jumbotronRequirements.text = this.cachedStringBuilder.ToString();
			int num5 = this.reactor.GetCurrLevelGenConfig().coresRequired * 5;
			int rewardXP = this.GetRewardXP();
			this.cachedStringBuilder.Clear();
			this.cachedStringBuilder.Append("<color=grey>Rewards:</color>\n");
			this.cachedStringBuilder.Append(string.Format("+⑭{0}\n", num5));
			this.cachedStringBuilder.Append(string.Format("+{0} XP\n", rewardXP));
			this.jumbotronRewards.text = this.cachedStringBuilder.ToString();
			break;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.jumbotronRequirements.text = "";
			this.jumbotronRewards.text = "";
			break;
		case GhostReactorShiftManager.State.Drilling:
			this.jumbotronRequirements.text = "";
			this.jumbotronRewards.text = "";
			break;
		}
		if (this.jumbotronState != null)
		{
			int state = (int)this.shiftManager.GetState();
			if (state >= 0 && state < GhostReactorShiftDepthDisplay.STATE_NAMES.Length)
			{
				this.jumbotronState.text = GhostReactorShiftDepthDisplay.STATE_NAMES[state];
			}
			else
			{
				this.jumbotronState.text = null;
			}
		}
		this.RefreshObjectives();
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000DFEF4 File Offset: 0x000DE0F4
	public void RefreshObjectives()
	{
		GRShiftStat shiftStats = this.shiftManager.shiftStats;
		bool flag = shiftStats.GetShiftStat(GRShiftStatType.CoresCollected) >= this.shiftManager.coresRequiredToDelveDeeper;
		bool flag2 = shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected) >= this.shiftManager.sentientCoresRequiredToDelveDeeper;
		bool flag3 = this.shiftManager.maxPlayerDeaths < 0 || shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths) <= this.shiftManager.maxPlayerDeaths;
		bool flag4 = true;
		foreach (GREnemyCount grenemyCount in this.shiftManager.killsRequiredToDelveDeeper)
		{
			if (CollectionExtensions.GetValueOrDefault<GREnemyType, int>(shiftStats.EnemyKills, grenemyCount.EnemyType) < grenemyCount.Count)
			{
				flag4 = false;
				break;
			}
		}
		if (this.shiftManager.ShiftActive && flag && flag2 && flag3 && flag4)
		{
			this.shiftManager.authorizedToDelveDeeper = true;
		}
		if (this.shiftManager.IsSoaking())
		{
			this.shiftManager.authorizedToDelveDeeper = true;
		}
		if (this.shiftManager.authorizedToDelveDeeper && this.jumbotronRequirements != null)
		{
			this.jumbotronRequirements.text = "<color=green>AUTHORIZED TO\nDELVE DEEPER</color>";
		}
		bool authorizedToDelveDeeper = this.shiftManager.authorizedToDelveDeeper;
		if (this.delveDeeperButton != null)
		{
			this.delveDeeperButton.SetActive(authorizedToDelveDeeper && !this.shiftManager.ShiftActive);
		}
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000E0070 File Offset: 0x000DE270
	public void StartDelveDeeperFX()
	{
		this.delveDeeperAudio.Play();
		this.delveDeeperNonspatializedAudio.Play();
		for (int i = 0; i < this.delveDeeperAnims.Count; i++)
		{
			this.delveDeeperAnims[i].Play();
		}
		for (int j = 0; j < this.delveDeeperAnimators.Count; j++)
		{
			this.delveDeeperAnimators[j].enabled = true;
		}
		for (int k = 0; k < this.delveDeeperParticles.Count; k++)
		{
			this.delveDeeperParticles[k].emission.enabled = true;
		}
		GorillaTagger.Instance.StartVibration(false, 0.1f, (float)this.shiftManager.GetDrillingDuration());
		GorillaTagger.Instance.StartVibration(true, 0.1f, (float)this.shiftManager.GetDrillingDuration());
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000E014C File Offset: 0x000DE34C
	public void StopDelveDeeperFX()
	{
		this.delveDeeperAudio.Stop();
		this.delveDeeperNonspatializedAudio.Stop();
		for (int i = 0; i < this.delveDeeperAnimators.Count; i++)
		{
			this.delveDeeperAnimators[i].enabled = false;
		}
		for (int j = 0; j < this.delveDeeperParticles.Count; j++)
		{
			this.delveDeeperParticles[j].emission.enabled = false;
		}
	}

	// Token: 0x04003529 RID: 13609
	public GhostReactorShiftManager shiftManager;

	// Token: 0x0400352A RID: 13610
	public GhostReactor reactor;

	// Token: 0x0400352B RID: 13611
	[SerializeField]
	public TMP_Text jumbotronTitle;

	// Token: 0x0400352C RID: 13612
	[SerializeField]
	public TMP_Text jumbotronState;

	// Token: 0x0400352D RID: 13613
	[SerializeField]
	public TMP_Text jumbotronTime;

	// Token: 0x0400352E RID: 13614
	[SerializeField]
	public TMP_Text jumbotronRequirements;

	// Token: 0x0400352F RID: 13615
	[SerializeField]
	public TMP_Text jumbotronRewards;

	// Token: 0x04003530 RID: 13616
	[SerializeField]
	public List<TMP_Text> logoFrames;

	// Token: 0x04003531 RID: 13617
	[SerializeField]
	private GameObject delveDeeperButton;

	// Token: 0x04003532 RID: 13618
	[SerializeField]
	private AudioSource delveDeeperAudio;

	// Token: 0x04003533 RID: 13619
	[SerializeField]
	private AudioSource delveDeeperNonspatializedAudio;

	// Token: 0x04003534 RID: 13620
	[SerializeField]
	private List<Animation> delveDeeperAnims;

	// Token: 0x04003535 RID: 13621
	[SerializeField]
	private List<Animator> delveDeeperAnimators;

	// Token: 0x04003536 RID: 13622
	[SerializeField]
	private List<ParticleSystem> delveDeeperParticles;

	// Token: 0x04003537 RID: 13623
	private static readonly string[] STATE_NAMES = new string[]
	{
		"--",
		"PREPARING ENTRY",
		"PREPARING ENTRY",
		"READY",
		"ACTIVE",
		"EVALUATING SHIFT",
		"PREPARE TO DIVE",
		"DIVING"
	};

	// Token: 0x04003538 RID: 13624
	private StringBuilder cachedStringBuilder = new StringBuilder(256);
}
