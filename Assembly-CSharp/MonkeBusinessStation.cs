using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001E0 RID: 480
public class MonkeBusinessStation : MonoBehaviourPunCallbacks
{
	// Token: 0x06000D0D RID: 3341 RVA: 0x00046254 File Offset: 0x00044454
	public override void OnEnable()
	{
		base.OnEnable();
		this.FindQuestManager();
		ProgressionController.OnQuestSelectionChanged += new Action(this.OnQuestSelectionChanged);
		ProgressionController.OnProgressEvent += new Action(this.OnProgress);
		ProgressionController.RequestProgressUpdate();
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x0004628F File Offset: 0x0004448F
	public override void OnDisable()
	{
		base.OnDisable();
		ProgressionController.OnQuestSelectionChanged -= new Action(this.OnQuestSelectionChanged);
		ProgressionController.OnProgressEvent -= new Action(this.OnProgress);
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x000462B9 File Offset: 0x000444B9
	private void FindQuestManager()
	{
		if (!this._questManager)
		{
			this._questManager = Object.FindAnyObjectByType<RotatingQuestsManager>();
		}
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x000462D3 File Offset: 0x000444D3
	private void UpdateCountdownTimers()
	{
		this._dailyCountdown.SetCountdownTime(this._questManager.DailyQuestCountdown);
		this._weeklyCountdown.SetCountdownTime(this._questManager.WeeklyQuestCountdown);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00046301 File Offset: 0x00044501
	private void OnQuestSelectionChanged()
	{
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00046309 File Offset: 0x00044509
	private void OnProgress()
	{
		this.UpdateQuestStatus();
		this.UpdateProgressDisplays();
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00046318 File Offset: 0x00044518
	private void UpdateProgressDisplays()
	{
		ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
		int item = progressionData.Item1;
		int item2 = progressionData.Item2;
		this._weeklyProgress.SetProgress(item, ProgressionController.WeeklyCap);
		if (!this._isUpdatingPointCount)
		{
			this._unclaimedPoints.text = item2.ToString();
			this._claimButton.isOn = (item2 > 0);
		}
		bool flag = item2 > 0;
		this._claimablePointsObject.SetActive(flag);
		this._noClaimablePointsObject.SetActive(!flag);
		this._badgeMount.position = (flag ? this._claimablePointsBadgePosition.position : this._noClaimablePointsBadgePosition.position);
		this._claimButton.gameObject.SetActive(flag);
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x000463CC File Offset: 0x000445CC
	private void UpdateQuestStatus()
	{
		if (this._lastQuestChange >= RotatingQuestsManager.LastQuestChange)
		{
			return;
		}
		this.FindQuestManager();
		if (this._quests.Count == 0 || this._lastQuestDailyID != RotatingQuestsManager.LastQuestDailyID)
		{
			this.BuildQuestList();
		}
		foreach (QuestDisplay questDisplay in this._quests)
		{
			if (questDisplay.IsChanged)
			{
				questDisplay.UpdateDisplay();
			}
		}
		this._lastQuestChange = Time.frameCount;
		this._lastQuestDailyID = RotatingQuestsManager.LastQuestDailyID;
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00046470 File Offset: 0x00044670
	public void RedeemProgress()
	{
		if (this._claimButton.isOn)
		{
			this._isUpdatingPointCount = true;
			ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
			int item = progressionData.Item2;
			int item2 = progressionData.Item3;
			this._tempUnclaimedPoints = item;
			this._tempTotalPoints = item2;
			this._claimButton.isOn = false;
			ProgressionController.RedeemProgress();
			if (PhotonNetwork.InRoom)
			{
				base.photonView.RPC("BroadcastRedeemQuestPoints", 1, new object[]
				{
					this._tempUnclaimedPoints
				});
			}
			base.StartCoroutine(this.PerformPointRedemptionSequence());
		}
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x000464FB File Offset: 0x000446FB
	private IEnumerator PerformPointRedemptionSequence()
	{
		while (this._tempUnclaimedPoints > 0)
		{
			this._tempUnclaimedPoints--;
			this._tempTotalPoints++;
			this._unclaimedPoints.text = this._tempUnclaimedPoints.ToString();
			if (this._tempUnclaimedPoints == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this._isUpdatingPointCount = false;
		this.UpdateProgressDisplays();
		yield break;
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x0004650C File Offset: 0x0004470C
	[PunRPC]
	private void BroadcastRedeemQuestPoints(int redeemedPointCount, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRedeemQuestPoints");
		RigContainer rigContainer;
		if (new PhotonMessageInfoWrapped(info).Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			if (!FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 10, (double)Time.unscaledTime))
			{
				return;
			}
			redeemedPointCount = Mathf.Min(redeemedPointCount, 50);
			Coroutine coroutine;
			if (this.perPlayerRedemptionSequence.TryGetValue(info.Sender, ref coroutine))
			{
				if (coroutine != null)
				{
					base.StopCoroutine(coroutine);
				}
				this.perPlayerRedemptionSequence.Remove(info.Sender);
			}
			if (base.gameObject.activeInHierarchy)
			{
				Coroutine coroutine2 = base.StartCoroutine(this.PerformRemotePointRedemptionSequence(info.Sender, redeemedPointCount));
				this.perPlayerRedemptionSequence.Add(info.Sender, coroutine2);
			}
		}
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000465E8 File Offset: 0x000447E8
	private IEnumerator PerformRemotePointRedemptionSequence(NetPlayer player, int redeemedPointCount)
	{
		while (redeemedPointCount > 0)
		{
			int num = redeemedPointCount;
			redeemedPointCount = num - 1;
			if (redeemedPointCount == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this.perPlayerRedemptionSequence.Remove(player);
		yield break;
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00046608 File Offset: 0x00044808
	private void BuildQuestList()
	{
		this.DestroyQuestList();
		RotatingQuestsManager.RotatingQuestList quests = this._questManager.quests;
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in quests.DailyQuests)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				if (rotatingQuest.isQuestActive)
				{
					QuestDisplay questDisplay = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._dailyQuestContainer);
					questDisplay.quest = rotatingQuest;
					this._quests.Add(questDisplay);
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in quests.WeeklyQuests)
		{
			foreach (RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				if (rotatingQuest2.isQuestActive)
				{
					QuestDisplay questDisplay2 = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._weeklyQuestContainer);
					questDisplay2.quest = rotatingQuest2;
					this._quests.Add(questDisplay2);
				}
			}
		}
		foreach (QuestDisplay questDisplay3 in this._quests)
		{
			questDisplay3.UpdateDisplay();
		}
		if (!this._hasBuiltQuestList)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._questContainerParent);
			this._hasBuiltQuestList = true;
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(this._questContainerParent);
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x000467DC File Offset: 0x000449DC
	private void DestroyQuestList()
	{
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._dailyQuestContainer);
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|40_0(this._weeklyQuestContainer);
		this._quests.Clear();
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x0004682C File Offset: 0x00044A2C
	[CompilerGenerated]
	internal static void <DestroyQuestList>g__DestroyChildren|40_0(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}

	// Token: 0x0400100A RID: 4106
	[SerializeField]
	private RectTransform _questContainerParent;

	// Token: 0x0400100B RID: 4107
	[SerializeField]
	private RectTransform _dailyQuestContainer;

	// Token: 0x0400100C RID: 4108
	[SerializeField]
	private RectTransform _weeklyQuestContainer;

	// Token: 0x0400100D RID: 4109
	[SerializeField]
	private QuestDisplay _questDisplayPrefab;

	// Token: 0x0400100E RID: 4110
	[SerializeField]
	private List<QuestDisplay> _quests;

	// Token: 0x0400100F RID: 4111
	[SerializeField]
	private ProgressDisplay _weeklyProgress;

	// Token: 0x04001010 RID: 4112
	[SerializeField]
	private TMP_Text _unclaimedPoints;

	// Token: 0x04001011 RID: 4113
	[SerializeField]
	private GorillaPressableButton _claimButton;

	// Token: 0x04001012 RID: 4114
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04001013 RID: 4115
	[SerializeField]
	private GameObject _claimablePointsObject;

	// Token: 0x04001014 RID: 4116
	[SerializeField]
	private GameObject _noClaimablePointsObject;

	// Token: 0x04001015 RID: 4117
	[SerializeField]
	private Transform _claimablePointsBadgePosition;

	// Token: 0x04001016 RID: 4118
	[SerializeField]
	private Transform _noClaimablePointsBadgePosition;

	// Token: 0x04001017 RID: 4119
	[SerializeField]
	private Transform _badgeMount;

	// Token: 0x04001018 RID: 4120
	[Space]
	[SerializeField]
	private float _claimDelayPerPoint = 0.12f;

	// Token: 0x04001019 RID: 4121
	[SerializeField]
	private AudioClip _claimPointDefaultSFX;

	// Token: 0x0400101A RID: 4122
	[SerializeField]
	private AudioClip _claimPointFinalSFX;

	// Token: 0x0400101B RID: 4123
	[Header("Quest Timers")]
	[SerializeField]
	private CountdownText _dailyCountdown;

	// Token: 0x0400101C RID: 4124
	[SerializeField]
	private CountdownText _weeklyCountdown;

	// Token: 0x0400101D RID: 4125
	private RotatingQuestsManager _questManager;

	// Token: 0x0400101E RID: 4126
	private int _lastQuestChange = -1;

	// Token: 0x0400101F RID: 4127
	private int _lastQuestDailyID = -1;

	// Token: 0x04001020 RID: 4128
	private bool _isUpdatingPointCount;

	// Token: 0x04001021 RID: 4129
	private int _tempUnclaimedPoints;

	// Token: 0x04001022 RID: 4130
	private int _tempTotalPoints;

	// Token: 0x04001023 RID: 4131
	private bool _hasBuiltQuestList;

	// Token: 0x04001024 RID: 4132
	private Dictionary<NetPlayer, Coroutine> perPlayerRedemptionSequence = new Dictionary<NetPlayer, Coroutine>();
}
