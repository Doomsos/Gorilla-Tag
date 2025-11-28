using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000151 RID: 337
public class SIUIPlayerQuestDisplay : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060008E7 RID: 2279 RVA: 0x00030074 File Offset: 0x0002E274
	public void RefreshDisplay()
	{
		SIPlayer siplayer = SIPlayer.Get(this.activePlayerActorNumber);
		bool flag = siplayer != null && siplayer.gamePlayer != null && siplayer.gamePlayer.rig != null && siplayer.gamePlayer.rig.Creator != null && this.activePlayerActorNumber > 0;
		if (!flag || !SIProgression.Instance.ClientReady)
		{
			if (this.activePlayer.activeSelf)
			{
				this.activePlayer.SetActive(false);
			}
			if (!this.waitingForPlayer.activeSelf)
			{
				this.waitingForPlayer.SetActive(true);
			}
			this.displayBackground.color = this.noPlayerColor;
			this.smallDisplayBackground.color = this.noPlayerColor;
			return;
		}
		if (this.activePlayer.activeSelf != flag)
		{
			this.activePlayer.SetActive(flag);
		}
		if (this.waitingForPlayer.activeSelf == flag)
		{
			this.waitingForPlayer.SetActive(!flag);
		}
		if (!flag)
		{
			this.displayBackground.color = this.noPlayerColor;
			this.smallDisplayBackground.color = this.noPlayerColor;
			return;
		}
		Color color = (siplayer == SIPlayer.LocalPlayer) ? this.localPlayerColor : this.remotePlayerColor;
		this.displayBackground.color = color;
		this.smallDisplayBackground.color = color;
		string sanitizedNickName = siplayer.gamePlayer.rig.Creator.SanitizedNickName;
		if (this.lastNickName != sanitizedNickName)
		{
			this.playerName.text = sanitizedNickName;
		}
		this.lastNickName = sanitizedNickName;
		int num = siplayer.CurrentProgression.resourceArray[0];
		if (this.lastTechPoints != num)
		{
			this.playerTechPoints.text = string.Format("TECH POINTS: {0}", num);
		}
		this.lastTechPoints = num;
		bool flag2 = siplayer.HasLimitedResourceBeenDeposited(SIResource.LimitedDepositType.MonkeIdol);
		if (flag2 != this.monkeIdolIcon.enabled)
		{
			this.monkeIdolIcon.enabled = flag2;
		}
		int stashedQuests = siplayer.CurrentProgression.stashedQuests;
		if (this.lastStashedQuests != stashedQuests)
		{
			this.stashedQuestCount.text = string.Format("STASHED QUESTS: {0}/{1}", Mathf.Max(0, stashedQuests - 3), 6);
		}
		this.lastStashedQuests = stashedQuests;
		int stashedBonusPoints = siplayer.CurrentProgression.stashedBonusPoints;
		if (this.lastStashedBonusPoints != stashedBonusPoints)
		{
			this.stashedBonusPointCount.text = string.Format("STASHED BONUS: {0}/{1}", Mathf.Max(0, stashedBonusPoints - 1), 2);
		}
		this.lastStashedBonusPoints = stashedBonusPoints;
		int bonusProgress = siplayer.CurrentProgression.bonusProgress;
		if (this.lastBonusProgress != bonusProgress)
		{
			this.sharedProgress.UpdateFillPercent((float)bonusProgress / 10f);
			this.sharedProgress.progressText.text = string.Format("{0}%", Mathf.Min(100, bonusProgress * 10));
		}
		this.lastBonusProgress = bonusProgress;
		bool flag3 = siplayer.CurrentProgression.stashedBonusPoints > 0;
		if (this.bonusPointsInProgress.activeSelf != flag3)
		{
			this.bonusPointsInProgress.SetActive(flag3);
		}
		if (this.bonusPointsCompleted.activeSelf == flag3)
		{
			this.bonusPointsCompleted.SetActive(!flag3);
		}
		bool flag4 = siplayer.CurrentProgression.bonusProgress >= 10;
		if (this.collectBonusButton.activeSelf != flag4)
		{
			this.collectBonusButton.SetActive(flag4);
		}
		if (this.questEntries == null || siplayer.CurrentProgression.currentQuestIds == null || siplayer.CurrentProgression.currentQuestProgresses == null)
		{
			return;
		}
		for (int i = 0; i < this.questEntries.Length; i++)
		{
			this.ProcessQuestEntry(this.questEntries[i], siplayer.CurrentProgression.currentQuestIds[i], siplayer.CurrentProgression.currentQuestProgresses[i]);
		}
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00030434 File Offset: 0x0002E634
	public void ProcessQuestEntry(SIUIPlayerQuestEntry entry, int questId, int questProgress)
	{
		if (SIProgression.Instance.questSourceList == null)
		{
			if (entry.questInfo.activeSelf)
			{
				entry.questInfo.SetActive(false);
			}
			if (!entry.noQuestAvailable.activeSelf)
			{
				entry.noQuestAvailable.SetActive(true);
			}
			if (entry.completeOverlay.activeSelf)
			{
				entry.completeOverlay.SetActive(false);
			}
			entry.lastQuestId = -1;
			entry.lastQuestProgress = -1;
			return;
		}
		RotatingQuest questById = SIProgression.Instance.questSourceList.GetQuestById(questId);
		bool flag = questId != -1 && questById != null;
		if (entry.completeOverlay.activeSelf && !flag)
		{
			entry.completeOverlay.SetActive(false);
		}
		if (entry.questInfo.activeSelf != flag)
		{
			entry.questInfo.SetActive(flag);
		}
		if (entry.noQuestAvailable.activeSelf == flag)
		{
			entry.noQuestAvailable.SetActive(!flag);
		}
		if (!flag)
		{
			entry.lastQuestId = -1;
			return;
		}
		if (questId != entry.lastQuestId)
		{
			entry.questDescription.text = questById.GetTextDescription();
		}
		if (entry.lastQuestProgress != questProgress || questId != entry.lastQuestId)
		{
			entry.progress.UpdateFillPercent((float)questProgress / (float)questById.requiredOccurenceCount);
			entry.progress.progressText.text = questProgress.ToString() + "/" + questById.requiredOccurenceCount.ToString();
		}
		if (entry.lastQuestId != -1 && entry.lastQuestId != questById.questID)
		{
			entry.newQuestTag.SetActive(true);
		}
		entry.lastQuestId = questById.questID;
		entry.lastQuestProgress = questProgress;
		bool flag2 = questProgress >= questById.requiredOccurenceCount;
		if (entry.completeOverlay.activeSelf != flag2)
		{
			entry.completeOverlay.SetActive(flag2);
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000305EC File Offset: 0x0002E7EC
	public void BonusPointCollectButtonPress()
	{
		if (this.activePlayerActorNumber == SIPlayer.LocalPlayer.ActorNr)
		{
			SIProgression.Instance.AttemptRedeemBonusPoint();
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0003060A File Offset: 0x0002E80A
	public void QuestPointCollectButtonPress(int questIndex)
	{
		if (this.activePlayerActorNumber == SIPlayer.LocalPlayer.ActorNr && SIPlayer.LocalPlayer.QuestAvailableToClaim(questIndex))
		{
			SIProgression.Instance.AttemptRedeemCompletedQuest(questIndex);
		}
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00030636 File Offset: 0x0002E836
	void IGorillaSliceableSimple.SliceUpdate()
	{
		this.RefreshDisplay();
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x04000AEA RID: 2794
	public TextMeshProUGUI playerName;

	// Token: 0x04000AEB RID: 2795
	[FormerlySerializedAs("playerTestPoints")]
	public TextMeshProUGUI playerTechPoints;

	// Token: 0x04000AEC RID: 2796
	public TextMeshProUGUI stashedQuestCount;

	// Token: 0x04000AED RID: 2797
	public TextMeshProUGUI stashedBonusPointCount;

	// Token: 0x04000AEE RID: 2798
	public Image displayBackground;

	// Token: 0x04000AEF RID: 2799
	public Image smallDisplayBackground;

	// Token: 0x04000AF0 RID: 2800
	public Image monkeIdolIcon;

	// Token: 0x04000AF1 RID: 2801
	public Color localPlayerColor;

	// Token: 0x04000AF2 RID: 2802
	public Color remotePlayerColor;

	// Token: 0x04000AF3 RID: 2803
	public Color noPlayerColor;

	// Token: 0x04000AF4 RID: 2804
	public SIUIPlayerQuestEntry[] questEntries;

	// Token: 0x04000AF5 RID: 2805
	public GameObject collectBonusButton;

	// Token: 0x04000AF6 RID: 2806
	public GameObject bonusPointsInProgress;

	// Token: 0x04000AF7 RID: 2807
	public GameObject bonusPointsCompleted;

	// Token: 0x04000AF8 RID: 2808
	public SIUIProgressBar sharedProgress;

	// Token: 0x04000AF9 RID: 2809
	public GameObject activePlayer;

	// Token: 0x04000AFA RID: 2810
	public GameObject waitingForPlayer;

	// Token: 0x04000AFB RID: 2811
	public int activePlayerActorNumber;

	// Token: 0x04000AFC RID: 2812
	private string lastNickName;

	// Token: 0x04000AFD RID: 2813
	private int lastStashedQuests = -1;

	// Token: 0x04000AFE RID: 2814
	private int lastStashedBonusPoints = -1;

	// Token: 0x04000AFF RID: 2815
	private int lastTechPoints = -1;

	// Token: 0x04000B00 RID: 2816
	private int lastBonusProgress = -1;
}
