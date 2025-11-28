using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001D5 RID: 469
public class MonkeVoteMachine : MonoBehaviour
{
	// Token: 0x06000CB6 RID: 3254 RVA: 0x000448DA File Offset: 0x00042ADA
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x000448E2 File Offset: 0x00042AE2
	private void Awake()
	{
		this._proximityTrigger.OnEnter += new Action(this.OnPlayerEnteredVoteProximity);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x000448FC File Offset: 0x00042AFC
	private void Start()
	{
		MonkeVoteController.instance.OnPollsUpdated += new Action(this.HandleOnPollsUpdated);
		MonkeVoteController.instance.OnVoteAccepted += new Action(this.HandleOnVoteAccepted);
		MonkeVoteController.instance.OnVoteFailed += new Action(this.HandleOnVoteFailed);
		MonkeVoteController.instance.OnCurrentPollEnded += new Action(this.HandleCurrentPollEnded);
		this.Init();
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x00044968 File Offset: 0x00042B68
	private void OnDestroy()
	{
		this._proximityTrigger.OnEnter -= new Action(this.OnPlayerEnteredVoteProximity);
		MonkeVoteController.instance.OnPollsUpdated -= new Action(this.HandleOnPollsUpdated);
		MonkeVoteController.instance.OnVoteAccepted -= new Action(this.HandleOnVoteAccepted);
		MonkeVoteController.instance.OnVoteFailed -= new Action(this.HandleOnVoteFailed);
		MonkeVoteController.instance.OnCurrentPollEnded -= new Action(this.HandleCurrentPollEnded);
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x000449E4 File Offset: 0x00042BE4
	public void Init()
	{
		this._isTestingPoll = false;
		this._previousPoll = (this._currentPoll = null);
		this._waitingOnVote = false;
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.ResetState();
			monkeVoteOption.OnVote += new Action<MonkeVoteOption, Collider>(this.OnVoteEntered);
		}
		this.UpdatePollDisplays();
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x00044A44 File Offset: 0x00042C44
	private void OnPlayerEnteredVoteProximity()
	{
		MonkeVoteController.instance.RequestPolls();
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x00044A50 File Offset: 0x00042C50
	private void HandleOnPollsUpdated()
	{
		this.UpdatePollDisplays();
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x00044A58 File Offset: 0x00042C58
	private void UpdatePollDisplays()
	{
		if (MonkeVoteController.instance == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			this.ShowResults(null);
			return;
		}
		MonkeVoteController.FetchPollsResponse lastPollData = MonkeVoteController.instance.GetLastPollData();
		if (lastPollData != null)
		{
			this._previousPoll = new MonkeVoteMachine.PollEntry(lastPollData);
			this.ShowResults(this._previousPoll);
		}
		else
		{
			this.ShowResults(null);
		}
		MonkeVoteController.FetchPollsResponse currentPollData = MonkeVoteController.instance.GetCurrentPollData();
		if (currentPollData == null)
		{
			this.SetState(MonkeVoteMachine.VotingState.None, true);
			return;
		}
		this._nextPollUpdate = MonkeVoteController.instance.GetCurrentPollCompletionTime();
		this._currentPoll = new MonkeVoteMachine.PollEntry(currentPollData);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState newState = (item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete);
			this.SetState(newState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x00044B3C File Offset: 0x00042D3C
	private void HandleOnVoteAccepted()
	{
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, true);
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00044B74 File Offset: 0x00042D74
	private void HandleOnVoteFailed()
	{
		this._waitingOnVote = false;
		int lastVotePollId = MonkeVoteController.instance.GetLastVotePollId();
		int lastVoteSelectedOption = MonkeVoteController.instance.GetLastVoteSelectedOption();
		bool lastVoteWasPrediction = MonkeVoteController.instance.GetLastVoteWasPrediction();
		this.OnVoteResponseReceived(lastVotePollId, lastVoteSelectedOption, lastVoteWasPrediction, false);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00044BB3 File Offset: 0x00042DB3
	private void HandleCurrentPollEnded()
	{
		if (this._proximityTrigger.isPlayerNearby)
		{
			MonkeVoteController.instance.RequestPolls();
		}
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00044BCC File Offset: 0x00042DCC
	[Tooltip("Hide dynamic child meshes to avoid them getting combined into the parent mesh on awake")]
	private void HideDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(false);
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00044BD5 File Offset: 0x00042DD5
	[Tooltip("Show dynamic child meshes to allow easy visualization")]
	private void ShowDynamicMeshes()
	{
		this.SetDynamicMeshesVisible(true);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00044BE0 File Offset: 0x00042DE0
	private void SetDynamicMeshesVisible(bool enabled)
	{
		MonkeVoteOption[] votingOptions = this._votingOptions;
		for (int i = 0; i < votingOptions.Length; i++)
		{
			votingOptions[i].SetDynamicMeshesVisible(enabled);
		}
		MonkeVoteResult[] results = this._results;
		for (int i = 0; i < results.Length; i++)
		{
			results[i].SetDynamicMeshesVisible(enabled);
		}
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00044C29 File Offset: 0x00042E29
	private void Configure()
	{
		this._audio = base.GetComponentInChildren<AudioSource>();
		this._audio.spatialBlend = 1f;
		this._votingOptions = base.GetComponentsInChildren<MonkeVoteOption>();
		this._results = base.GetComponentsInChildren<MonkeVoteResult>();
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00044C60 File Offset: 0x00042E60
	public void CreateNextDummyPoll()
	{
		this._isTestingPoll = true;
		if (this._currentPoll != null)
		{
			this._previousPoll = this._currentPoll;
		}
		else
		{
			this._previousPoll = null;
		}
		this.ShowResults(this._previousPoll);
		int pollId = 0;
		if (this._previousPoll != null)
		{
			pollId = this._previousPoll.PollId + 1;
		}
		string question = "Test Question Number: " + Random.Range(1, 101).ToString();
		string text = "Answer " + Random.Range(1, 101).ToString();
		string text2 = "Answer " + Random.Range(1, 101).ToString();
		string[] voteOptions = new string[]
		{
			text,
			text2
		};
		this._currentPoll = new MonkeVoteMachine.PollEntry(pollId, question, voteOptions);
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		if (currentPoll != null && currentPoll.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			MonkeVoteMachine.VotingState newState = (item < 0) ? MonkeVoteMachine.VotingState.Voting : ((item2 < 0) ? MonkeVoteMachine.VotingState.Predicting : MonkeVoteMachine.VotingState.Complete);
			this.SetState(newState, true);
			return;
		}
		this.SetState(MonkeVoteMachine.VotingState.None, true);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x00044D82 File Offset: 0x00042F82
	private void VoteLeft()
	{
		this.OnVoteEntered(this._votingOptions[0], null);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00044D93 File Offset: 0x00042F93
	private void VoteRight()
	{
		this.OnVoteEntered(this._votingOptions[1], null);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00044DA4 File Offset: 0x00042FA4
	private void VoteWinner()
	{
		if (this._currentPoll != null)
		{
			if (this._currentPoll.VoteCount[0] > this._currentPoll.VoteCount[1])
			{
				this.OnVoteEntered(this._votingOptions[0], null);
				return;
			}
			this.OnVoteEntered(this._votingOptions[1], null);
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00044DF4 File Offset: 0x00042FF4
	private void ClearLocalData()
	{
		this.ClearLocalVoteAndPredictionData();
		this.UpdatePollDisplays();
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00044E04 File Offset: 0x00043004
	private void SetState(MonkeVoteMachine.VotingState newState, bool instant = true)
	{
		this._state = newState;
		MonkeVoteMachine.PollEntry currentPoll = this._currentPoll;
		bool flag = currentPoll != null && currentPoll.IsValid;
		if (this._state < MonkeVoteMachine.VotingState.None || this._state > MonkeVoteMachine.VotingState.Complete || (this._state != MonkeVoteMachine.VotingState.None && !flag))
		{
			this._state = MonkeVoteMachine.VotingState.None;
		}
		if (flag)
		{
			int item = this.GetVote(this._currentPoll.PollId).Item2;
			if (this._state < MonkeVoteMachine.VotingState.Predicting)
			{
				this.SaveVote(this._currentPoll.PollId, -1, item);
			}
			int item2 = this.GetVote(this._currentPoll.PollId).Item1;
			if (this._state < MonkeVoteMachine.VotingState.Complete)
			{
				this.SaveVote(this._currentPoll.PollId, item2, -1);
			}
		}
		bool flag2 = true;
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.None:
			this._timerText.SetFixedText(this._pollsClosedText);
			this._titleText.text = this._defaultTitle;
			this._questionText.text = this._defaultQuestion;
			flag2 = false;
			break;
		case MonkeVoteMachine.VotingState.Voting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._voteTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		case MonkeVoteMachine.VotingState.Predicting:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._predictTitle;
			this._questionText.text = this._predictQuestion;
			break;
		case MonkeVoteMachine.VotingState.Complete:
			this._timerText.SetCountdownTime(this._nextPollUpdate);
			this._titleText.text = this._completeTitle;
			this._questionText.text = this._currentPoll.Question;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		int num;
		int num2;
		if (!flag)
		{
			num = -1;
			num2 = -1;
		}
		else
		{
			ValueTuple<int, int> vote = this.GetVote(this._currentPoll.PollId);
			num = vote.Item1;
			num2 = vote.Item2;
		}
		if (flag2)
		{
			for (int i = 0; i < this._votingOptions.Length; i++)
			{
				this._votingOptions[i].Text = this._currentPoll.VoteOptions[i];
				this._votingOptions[i].ShowIndicators(num == i, num2 == i, instant);
			}
			return;
		}
		foreach (MonkeVoteOption monkeVoteOption in this._votingOptions)
		{
			monkeVoteOption.Text = string.Empty;
			monkeVoteOption.ShowIndicators(false, false, true);
		}
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00045080 File Offset: 0x00043280
	private void ShowResults(MonkeVoteMachine.PollEntry entry)
	{
		if (entry != null && entry.IsValid)
		{
			ValueTuple<int, int> vote = this.GetVote(entry.PollId);
			int item = vote.Item1;
			int item2 = vote.Item2;
			GTDev.Log<string>(string.Format("Showing {0} V:{1} P:{2}", entry.Question, item, item2), null);
			List<int> list = this.ConvertToPercentages(entry.VoteCount);
			int num = 0;
			int num2 = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] > num)
				{
					num = list[i];
					num2 = i;
				}
			}
			this._resultsTitleText.text = this._defaultResultsTitle;
			this._resultsQuestionText.text = entry.Question;
			for (int j = 0; j < entry.VoteOptions.Length; j++)
			{
				this._results[j].ShowResult(entry.VoteOptions[j], list[j], item == j, item2 == j, num2 == j);
			}
			int prePollStreak = this.GetPrePollStreak(entry.PollId);
			int postPollStreak = this.GetPostPollStreak(entry);
			this._resultsStreakText.text = ((postPollStreak >= prePollStreak) ? string.Format(this._streakBlurb, postPollStreak) : string.Format(this._streakLostBlurb, prePollStreak, postPollStreak));
			return;
		}
		this._resultsTitleText.text = this._defaultResultsTitle;
		this._resultsQuestionText.text = this._defaultQuestion;
		this._resultsStreakText.text = string.Empty;
		MonkeVoteResult[] results = this._results;
		for (int k = 0; k < results.Length; k++)
		{
			results[k].HideResult();
		}
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x00045230 File Offset: 0x00043430
	private List<int> ConvertToPercentages(int[] votes)
	{
		List<int> list = new List<int>();
		List<float> list2 = new List<float>();
		if (votes == null || votes.Length == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		if (votes.Length == 1)
		{
			list.Add(100);
			list.Add(0);
			return list;
		}
		int num = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(votes);
		if (num == 0)
		{
			list.Add(-1);
			list.Add(-1);
			return list;
		}
		int num2 = -1;
		int num3 = 0;
		for (int i = 0; i < votes.Length; i++)
		{
			if (votes[i] > num2)
			{
				num2 = votes[i];
				num3 = i;
			}
			float num4 = (float)votes[i] / (float)num * 100f;
			list.Add((int)num4);
			list2.Add(num4 - (float)((int)num4));
		}
		int num5 = MonkeVoteMachine.<ConvertToPercentages>g__Sum|64_0(list);
		int num6 = 100 - num5;
		for (int j = 0; j < num6; j++)
		{
			int num7 = MonkeVoteMachine.<ConvertToPercentages>g__LargestFractionIndex|64_1(list2);
			List<int> list3 = list;
			int num8 = num7;
			int num9 = list3[num8];
			list3[num8] = num9 + 1;
			list2[num7] = 0f;
		}
		if (list.Count == 2 && list[num3] == 50)
		{
			List<int> list4 = list;
			int num9 = num3;
			list4[num9]++;
			list4 = list;
			num9 = 1 - num3;
			list4[num9]--;
		}
		return list;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0004537C File Offset: 0x0004357C
	private void OnVoteEntered(MonkeVoteOption option, Collider votingCollider)
	{
		if (this._waitingOnVote || (Time.time < this._voteCooldownEnd && !this._isTestingPoll))
		{
			this.PlayVoteFailEffects();
			return;
		}
		int num = Array.IndexOf<MonkeVoteOption>(this._votingOptions, option);
		if (num < 0)
		{
			return;
		}
		switch (this._state)
		{
		case MonkeVoteMachine.VotingState.Voting:
			this.Vote(this._currentPoll.PollId, num, false);
			return;
		case MonkeVoteMachine.VotingState.Predicting:
			this.Vote(this._currentPoll.PollId, num, true);
			return;
		}
		this.PlayVoteFailEffects();
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0004540C File Offset: 0x0004360C
	private void Vote(int id, int option, bool isPrediction)
	{
		if (option < 0 || this._waitingOnVote)
		{
			return;
		}
		this._waitingOnVote = true;
		if (this._isTestingPoll)
		{
			this.OnVoteResponseReceived(id, option, isPrediction, true);
			return;
		}
		MonkeVoteController.instance.Vote(id, option, isPrediction);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00045444 File Offset: 0x00043644
	private void OnVoteResponseReceived(int id, int option, bool isPrediction, bool success)
	{
		this._waitingOnVote = false;
		if (success)
		{
			this.PlayVoteSuccessEffects();
			this._voteCooldownEnd = Time.time + this._voteCooldown;
			ValueTuple<int, int> vote = this.GetVote(id);
			int num = vote.Item1;
			int num2 = vote.Item2;
			if (!isPrediction)
			{
				int num3 = num2;
				num = option;
				num2 = num3;
			}
			else
			{
				num = num;
				num2 = option;
			}
			this.SaveVote(id, num, num2);
			MonkeVoteMachine.VotingState state = this._state;
			if (state != MonkeVoteMachine.VotingState.Voting)
			{
				if (state == MonkeVoteMachine.VotingState.Predicting)
				{
					this.SetState(MonkeVoteMachine.VotingState.Complete, false);
				}
			}
			else
			{
				this.SetState(MonkeVoteMachine.VotingState.Predicting, false);
			}
			if (isPrediction && id == this._currentPoll.PollId)
			{
				this.SavePrePollStreak(id, this.GetPostPollStreak(this._previousPoll));
				return;
			}
		}
		else
		{
			this.PlayVoteFailEffects();
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000454F8 File Offset: 0x000436F8
	private void PlayVoteSuccessEffects()
	{
		MonkeVoteMachine.<PlayVoteSuccessEffects>d__68 <PlayVoteSuccessEffects>d__;
		<PlayVoteSuccessEffects>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PlayVoteSuccessEffects>d__.<>4__this = this;
		<PlayVoteSuccessEffects>d__.<>1__state = -1;
		<PlayVoteSuccessEffects>d__.<>t__builder.Start<MonkeVoteMachine.<PlayVoteSuccessEffects>d__68>(ref <PlayVoteSuccessEffects>d__);
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x0004552F File Offset: 0x0004372F
	private void PlayVoteFailEffects()
	{
		this._audio.GTPlayOneShot(this._voteFailSound, this._audio.volume);
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x00045550 File Offset: 0x00043750
	private void SaveVote(int id, int voteOption, int predictionOption)
	{
		int @int = PlayerPrefs.GetInt("Vote_Current_Id", -1);
		if (@int == -1 || @int == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
		}
		else
		{
			PlayerPrefs.SetInt("Vote_Previous_Id", @int);
			PlayerPrefs.SetInt("Vote_Previous_Option", PlayerPrefs.GetInt("Vote_Current_Option"));
			PlayerPrefs.SetInt("Vote_Previous_Prediction", PlayerPrefs.GetInt("Vote_Current_Prediction"));
			PlayerPrefs.SetInt("Vote_Previous_Streak", PlayerPrefs.GetInt("Vote_Current_Streak"));
			PlayerPrefs.SetInt("Vote_Current_Id", id);
			PlayerPrefs.SetInt("Vote_Current_Option", voteOption);
			PlayerPrefs.SetInt("Vote_Current_Prediction", predictionOption);
			PlayerPrefs.SetInt("Vote_Current_Streak", 0);
		}
		PlayerPrefs.Save();
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0004560C File Offset: 0x0004380C
	[return: TupleElementNames(new string[]
	{
		"voteOption",
		"predictionOption"
	})]
	private ValueTuple<int, int> GetVote(int voteId)
	{
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == voteId)
		{
			int @int = PlayerPrefs.GetInt("Vote_Current_Option", -1);
			int int2 = PlayerPrefs.GetInt("Vote_Current_Prediction", -1);
			return new ValueTuple<int, int>(@int, int2);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == voteId)
		{
			int int3 = PlayerPrefs.GetInt("Vote_Previous_Option", -1);
			int int4 = PlayerPrefs.GetInt("Vote_Previous_Prediction", -1);
			return new ValueTuple<int, int>(int3, int4);
		}
		return new ValueTuple<int, int>(-1, -1);
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00045678 File Offset: 0x00043878
	private void SavePrePollStreak(int id, int streak)
	{
		if (id < 0)
		{
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Current_Streak", streak);
			return;
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			PlayerPrefs.SetInt("Vote_Previous_Streak", streak);
		}
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000456B2 File Offset: 0x000438B2
	private int GetPrePollStreak(int id)
	{
		if (id < 0)
		{
			return 0;
		}
		if (PlayerPrefs.GetInt("Vote_Current_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Current_Streak", 0);
		}
		if (PlayerPrefs.GetInt("Vote_Previous_Id", -1) == id)
		{
			return PlayerPrefs.GetInt("Vote_Previous_Streak", 0);
		}
		return 0;
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x000456F0 File Offset: 0x000438F0
	private int GetPostPollStreak(MonkeVoteMachine.PollEntry entry)
	{
		if (entry == null || !entry.IsValid)
		{
			return 0;
		}
		int item = this.GetVote(entry.PollId).Item2;
		if (item < 0)
		{
			return 0;
		}
		int prePollStreak = this.GetPrePollStreak(entry.PollId);
		if (item != entry.GetWinner())
		{
			return 0;
		}
		return prePollStreak + 1;
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00045740 File Offset: 0x00043940
	private void ClearLocalVoteAndPredictionData()
	{
		PlayerPrefs.DeleteKey("Vote_Current_Id");
		PlayerPrefs.DeleteKey("Vote_Current_Option");
		PlayerPrefs.DeleteKey("Vote_Current_Prediction");
		PlayerPrefs.DeleteKey("Vote_Current_Streak");
		PlayerPrefs.DeleteKey("Vote_Previous_Id");
		PlayerPrefs.DeleteKey("Vote_Previous_Option");
		PlayerPrefs.DeleteKey("Vote_Previous_Prediction");
		PlayerPrefs.DeleteKey("Vote_Previous_Streak");
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0004582C File Offset: 0x00043A2C
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__Sum|64_0(IList<int> items)
	{
		int num = 0;
		foreach (int num2 in items)
		{
			num += num2;
		}
		return num;
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00045874 File Offset: 0x00043A74
	[CompilerGenerated]
	internal static int <ConvertToPercentages>g__LargestFractionIndex|64_1(IList<float> fractions)
	{
		float num = float.NegativeInfinity;
		int result = -1;
		for (int i = 0; i < fractions.Count; i++)
		{
			if (fractions[i] > num)
			{
				num = fractions[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x04000FAB RID: 4011
	private const string kVoteCurrentIdKey = "Vote_Current_Id";

	// Token: 0x04000FAC RID: 4012
	private const string kVoteCurrentOptionKey = "Vote_Current_Option";

	// Token: 0x04000FAD RID: 4013
	private const string kVoteCurrentPredictionKey = "Vote_Current_Prediction";

	// Token: 0x04000FAE RID: 4014
	private const string kVoteCurrentStreak = "Vote_Current_Streak";

	// Token: 0x04000FAF RID: 4015
	private const string kVotePreviousIdKey = "Vote_Previous_Id";

	// Token: 0x04000FB0 RID: 4016
	private const string kVotePreviousOptionKey = "Vote_Previous_Option";

	// Token: 0x04000FB1 RID: 4017
	private const string kVotePreviousPredictionKey = "Vote_Previous_Prediction";

	// Token: 0x04000FB2 RID: 4018
	private const string kVotePreviousStreak = "Vote_Previous_Streak";

	// Token: 0x04000FB3 RID: 4019
	[SerializeField]
	private MonkeVoteProximityTrigger _proximityTrigger;

	// Token: 0x04000FB4 RID: 4020
	[Header("VOTING")]
	[SerializeField]
	private string _pollsClosedText = "POLLS CLOSED";

	// Token: 0x04000FB5 RID: 4021
	[SerializeField]
	private string _defaultTitle = "MONKE VOTE";

	// Token: 0x04000FB6 RID: 4022
	[SerializeField]
	private string _voteTitle = "VOTE";

	// Token: 0x04000FB7 RID: 4023
	[SerializeField]
	private string _predictTitle = "GUESS";

	// Token: 0x04000FB8 RID: 4024
	[SerializeField]
	private string _completeTitle = "VOTING COMPLETE";

	// Token: 0x04000FB9 RID: 4025
	[SerializeField]
	private string _defaultQuestion = "COME BACK LATER";

	// Token: 0x04000FBA RID: 4026
	[SerializeField]
	private string _predictQuestion = "WHICH WILL BE MORE POPULAR?";

	// Token: 0x04000FBB RID: 4027
	[Tooltip("Must be in the format \"STREAK: {0}\"")]
	[SerializeField]
	private string _streakBlurb = "PREDICTION STREAK: {0}";

	// Token: 0x04000FBC RID: 4028
	[Tooltip("Must be in the format \"LOST {0} PREDICTION STREAK! STREAK: {1}\"")]
	[SerializeField]
	private string _streakLostBlurb = "<color=red>{0} POLL STREAK LOST!</color>  STREAK: {1}";

	// Token: 0x04000FBD RID: 4029
	[SerializeField]
	private float _voteCooldown = 1f;

	// Token: 0x04000FBE RID: 4030
	[SerializeField]
	private MonkeVoteOption[] _votingOptions;

	// Token: 0x04000FBF RID: 4031
	[SerializeField]
	private CountdownText _timerText;

	// Token: 0x04000FC0 RID: 4032
	[SerializeField]
	private TMP_Text _titleText;

	// Token: 0x04000FC1 RID: 4033
	[SerializeField]
	private TMP_Text _questionText;

	// Token: 0x04000FC2 RID: 4034
	[Header("RESULTS")]
	[SerializeField]
	private string _defaultResultsTitle = "PREVIOUS QUESTION";

	// Token: 0x04000FC3 RID: 4035
	[SerializeField]
	private TMP_Text _resultsTitleText;

	// Token: 0x04000FC4 RID: 4036
	[SerializeField]
	private TMP_Text _resultsQuestionText;

	// Token: 0x04000FC5 RID: 4037
	[SerializeField]
	private TMP_Text _resultsStreakText;

	// Token: 0x04000FC6 RID: 4038
	[SerializeField]
	private MonkeVoteResult[] _results;

	// Token: 0x04000FC7 RID: 4039
	[FormerlySerializedAs("_sound")]
	[Header("FX")]
	[SerializeField]
	private AudioSource _audio;

	// Token: 0x04000FC8 RID: 4040
	[FormerlySerializedAs("_voteProcessingAudio")]
	[SerializeField]
	private AudioSource _voteTubeAudio;

	// Token: 0x04000FC9 RID: 4041
	[SerializeField]
	private AudioClip[] _voteFailSound;

	// Token: 0x04000FCA RID: 4042
	[SerializeField]
	private AudioClip[] _voteSuccessDing;

	// Token: 0x04000FCB RID: 4043
	[FormerlySerializedAs("_voteSuccessSound")]
	[SerializeField]
	private AudioClip[] _voteProcessingSound;

	// Token: 0x04000FCC RID: 4044
	private MonkeVoteMachine.VotingState _state;

	// Token: 0x04000FCD RID: 4045
	private float _voteCooldownEnd;

	// Token: 0x04000FCE RID: 4046
	private bool _waitingOnVote;

	// Token: 0x04000FCF RID: 4047
	private MonkeVoteMachine.PollEntry _currentPoll;

	// Token: 0x04000FD0 RID: 4048
	private MonkeVoteMachine.PollEntry _previousPoll;

	// Token: 0x04000FD1 RID: 4049
	private DateTime _nextPollUpdate;

	// Token: 0x04000FD2 RID: 4050
	private bool _isTestingPoll;

	// Token: 0x020001D6 RID: 470
	public enum VotingState
	{
		// Token: 0x04000FD4 RID: 4052
		None,
		// Token: 0x04000FD5 RID: 4053
		Voting,
		// Token: 0x04000FD6 RID: 4054
		Predicting,
		// Token: 0x04000FD7 RID: 4055
		Complete
	}

	// Token: 0x020001D7 RID: 471
	public class PollEntry
	{
		// Token: 0x06000CDB RID: 3291 RVA: 0x000458B0 File Offset: 0x00043AB0
		public PollEntry(int pollId, string question, string[] voteOptions)
		{
			this.PollId = pollId;
			this.Question = question;
			this.VoteOptions = voteOptions;
			this.VoteCount = new int[2];
			this.VoteCount[0] = Random.Range(0, 50000);
			this.VoteCount[1] = Random.Range(0, 50000);
			this.PredictionCount = new int[2];
			this.PredictionCount[0] = Random.Range(0, 50000);
			this.PredictionCount[1] = Random.Range(0, 50000);
			this.StartTime = DateTime.Now;
			this.EndTime = DateTime.Now + TimeSpan.FromSeconds(20.0);
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x00045968 File Offset: 0x00043B68
		public PollEntry(MonkeVoteController.FetchPollsResponse poll)
		{
			this.PollId = poll.PollId;
			this.Question = poll.Question;
			this.VoteOptions = poll.VoteOptions.ToArray();
			this.VoteCount = poll.VoteCount.ToArray();
			this.PredictionCount = poll.PredictionCount.ToArray();
			this.StartTime = poll.StartTime;
			this.EndTime = poll.EndTime;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x000459E0 File Offset: 0x00043BE0
		public int GetWinner()
		{
			if (this.VoteCount == null || this.VoteCount.Length == 0)
			{
				return -1;
			}
			int num = int.MinValue;
			int result = -1;
			for (int i = 0; i < this.VoteCount.Length; i++)
			{
				if (this.VoteCount[i] > num)
				{
					num = this.VoteCount[i];
					result = i;
				}
			}
			return result;
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000CDE RID: 3294 RVA: 0x00045A34 File Offset: 0x00043C34
		public bool IsValid
		{
			get
			{
				string[] voteOptions = this.VoteOptions;
				return voteOptions != null && voteOptions.Length == 2;
			}
		}

		// Token: 0x04000FD8 RID: 4056
		public int PollId;

		// Token: 0x04000FD9 RID: 4057
		public string Question;

		// Token: 0x04000FDA RID: 4058
		public string[] VoteOptions;

		// Token: 0x04000FDB RID: 4059
		public int[] VoteCount;

		// Token: 0x04000FDC RID: 4060
		public int[] PredictionCount;

		// Token: 0x04000FDD RID: 4061
		public DateTime StartTime;

		// Token: 0x04000FDE RID: 4062
		public DateTime EndTime;
	}
}
