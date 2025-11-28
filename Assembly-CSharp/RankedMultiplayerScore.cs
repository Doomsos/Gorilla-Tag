using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200083B RID: 2107
public class RankedMultiplayerScore : MonoBehaviourTick
{
	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x06003766 RID: 14182 RVA: 0x0012A76B File Offset: 0x0012896B
	// (set) Token: 0x06003767 RID: 14183 RVA: 0x0012A773 File Offset: 0x00128973
	public RankedProgressionManager Progression { get; private set; }

	// Token: 0x06003768 RID: 14184 RVA: 0x0012A77C File Offset: 0x0012897C
	public void Initialize()
	{
		GorillaTagCompetitiveManager.onStateChanged += new Action<GorillaTagCompetitiveManager.GameState>(this.OnStateChanged);
		GorillaTagCompetitiveManager.onRoundStart += new Action(this.OnGameStarted);
		GorillaTagCompetitiveManager.onRoundEnd += new Action(this.OnGameEnded);
		GorillaTagCompetitiveManager.onPlayerJoined += new Action<NetPlayer>(this.OnPlayerJoined);
		GorillaTagCompetitiveManager.onPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeft);
		GorillaTagCompetitiveManager.onTagOccurred += new Action<NetPlayer, NetPlayer>(this.OnTagReported);
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null)
		{
			this.CompetitiveManager = (instance as GorillaTagCompetitiveManager);
		}
		this.Progression = RankedProgressionManager.Instance;
		RankedProgressionManager progression = this.Progression;
		progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Combine(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x0012A83C File Offset: 0x00128A3C
	private void HandlePlayerEloAcquired(int playerId, float elo, int tier)
	{
		this.CachePlayerRankedProgressionData(playerId, tier, elo);
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0012A847 File Offset: 0x00128A47
	private void OnDestroy()
	{
		this.Unsubscribe();
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0012A850 File Offset: 0x00128A50
	public void Unsubscribe()
	{
		GorillaTagCompetitiveManager.onStateChanged -= new Action<GorillaTagCompetitiveManager.GameState>(this.OnStateChanged);
		GorillaTagCompetitiveManager.onRoundStart -= new Action(this.OnGameStarted);
		GorillaTagCompetitiveManager.onRoundEnd -= new Action(this.OnGameEnded);
		GorillaTagCompetitiveManager.onPlayerJoined -= new Action<NetPlayer>(this.OnPlayerJoined);
		GorillaTagCompetitiveManager.onPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeft);
		GorillaTagCompetitiveManager.onTagOccurred -= new Action<NetPlayer, NetPlayer>(this.OnTagReported);
		if (this.Progression != null)
		{
			RankedProgressionManager progression = this.Progression;
			progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Remove(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
		}
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x0012A8F8 File Offset: 0x00128AF8
	public override void Tick()
	{
		if (this.PerSecondTimer > 0f && Time.time >= this.PerSecondTimer + 1f)
		{
			if (this.CompetitiveManager == null)
			{
				return;
			}
			this.OnPerSecondTimerElapsed(NetworkSystem.Instance.AllNetPlayers.Length, this.CompetitiveManager.currentInfected.Count);
			this.PerSecondTimer = Time.time;
		}
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x0012A964 File Offset: 0x00128B64
	private void OnPerSecondTimerElapsed(int playersInGame, int infectedPlayers)
	{
		foreach (int num in Enumerable.ToList<int>(this.AllPlayerInRoundScores.Keys))
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			playerScoreInRound.Infected = this.CompetitiveManager.IsInfected(NetworkSystem.Instance.GetPlayer(num));
			if (!playerScoreInRound.Infected)
			{
				float num2 = (float)infectedPlayers / (float)playersInGame;
				playerScoreInRound.PointsOnDefense += Mathf.Lerp(this.PointsPerUninfectedSecMin, this.PointsPerUninfectedSecMax, num2);
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x0012AA1C File Offset: 0x00128C1C
	public void ResetMatch()
	{
		this.AllFinalPlayerScores.Clear();
		this.AllPlayerInRoundScores.Clear();
	}

	// Token: 0x0600376F RID: 14191 RVA: 0x0012AA34 File Offset: 0x00128C34
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState state)
	{
		if (state == GorillaTagCompetitiveManager.GameState.StartingCountdown)
		{
			this.OnGameStarted();
			this.Progression.AcquireRoomRankInformation(true);
		}
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x0012AA4C File Offset: 0x00128C4C
	public void OnGameStarted()
	{
		this.PerSecondTimer = Time.time;
		if (!this.IsLateJoiner)
		{
			this.ResetMatch();
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				this.StartTrackingPlayer(NetworkSystem.Instance.AllNetPlayers[i], false);
			}
		}
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x0012AA9C File Offset: 0x00128C9C
	public void OnGameEnded()
	{
		foreach (int num in Enumerable.ToList<int>(this.AllPlayerInRoundScores.Keys))
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			if (!playerScoreInRound.Infected)
			{
				playerScoreInRound.TaggedTime = Time.time;
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
		this.PerSecondTimer = -1f;
		this.ReportScore();
		this.WasInfectedInitially = false;
		this.IsLateJoiner = false;
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0012AB40 File Offset: 0x00128D40
	private void OnPlayerJoined(NetPlayer player)
	{
		if (NetworkSystem.Instance.IsMasterClient && this.CompetitiveManager.IsMatchActive())
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			List<bool> list5 = new List<bool>();
			List<float> list6 = new List<float>();
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				list.Add(keyValuePair.Value.PlayerId);
				list2.Add(keyValuePair.Value.NumTags);
				list3.Add(keyValuePair.Value.PointsOnDefense);
				list4.Add(Time.time - keyValuePair.Value.JoinTime);
				list5.Add(keyValuePair.Value.Infected);
				if (!keyValuePair.Value.Infected)
				{
					list6.Add(0f);
				}
				else
				{
					list6.Add(Time.time - keyValuePair.Value.TaggedTime);
				}
			}
			GameMode.ActiveNetworkHandler.SendRPC("SendScoresToLateJoinerRPC", player, new object[]
			{
				list.ToArray(),
				list2.ToArray(),
				list3.ToArray(),
				list4.ToArray(),
				list5.ToArray(),
				list6.ToArray()
			});
		}
		this.StartTrackingPlayer(player, true);
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x0012ACC8 File Offset: 0x00128EC8
	public void ReceivedScoresForLateJoiner(int[] playerIds, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			this.IsLateJoiner = true;
			for (int i = 0; i < playerIds.Length; i++)
			{
				int num = playerIds[i];
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = new RankedMultiplayerScore.PlayerScoreInRound(num, infected[i]);
				playerScoreInRound.NumTags = numTags[i];
				playerScoreInRound.PointsOnDefense = pointsOnDefense[i];
				playerScoreInRound.JoinTime = Time.time - joinTime[i];
				if (!infected[i])
				{
					playerScoreInRound.TaggedTime = 0f;
				}
				else
				{
					playerScoreInRound.TaggedTime = Time.time - taggedTime[i];
				}
				this.AllPlayerInRoundScores.TryAdd(num, playerScoreInRound);
			}
		}
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x0012AD5E File Offset: 0x00128F5E
	private void OnPlayerLeft(NetPlayer player)
	{
		this.AllPlayerInRoundScores.Remove(player.ActorNumber);
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x0012AD74 File Offset: 0x00128F74
	private void StartTrackingPlayer(NetPlayer player, bool lateJoin)
	{
		bool initInfected = lateJoin;
		if (!lateJoin && this.CompetitiveManager != null)
		{
			initInfected = this.CompetitiveManager.IsInfected(player);
			if (player.ActorNumber == NetworkSystem.Instance.LocalPlayerID)
			{
				this.WasInfectedInitially = true;
			}
		}
		if (player == NetworkSystem.Instance.LocalPlayer)
		{
			this.CachePlayerRankedProgressionData(player.ActorNumber, this.Progression.GetProgressionRankIndex(), this.Progression.GetEloScore());
		}
		this.AllPlayerInRoundScores.TryAdd(player.ActorNumber, new RankedMultiplayerScore.PlayerScoreInRound(player.ActorNumber, initInfected));
	}

	// Token: 0x06003776 RID: 14198 RVA: 0x0012AE08 File Offset: 0x00129008
	public RankedMultiplayerScore.PlayerScoreInRound GetInGameScoreForSelf()
	{
		RankedMultiplayerScore.PlayerScoreInRound result;
		if (this.AllPlayerInRoundScores.TryGetValue(NetworkSystem.Instance.LocalPlayerID, ref result))
		{
			return result;
		}
		return default(RankedMultiplayerScore.PlayerScoreInRound);
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x0012AE3C File Offset: 0x0012903C
	public void OnTagReported(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound;
		if (this.AllPlayerInRoundScores.TryGetValue(taggingPlayer.ActorNumber, ref playerScoreInRound))
		{
			playerScoreInRound.NumTags++;
			this.AllPlayerInRoundScores[taggingPlayer.ActorNumber] = playerScoreInRound;
		}
		RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound2;
		if (this.AllPlayerInRoundScores.TryGetValue(taggedPlayer.ActorNumber, ref playerScoreInRound2))
		{
			playerScoreInRound2.Infected = true;
			playerScoreInRound2.TaggedTime = Time.time;
			this.AllPlayerInRoundScores[taggedPlayer.ActorNumber] = playerScoreInRound2;
		}
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0012AEB8 File Offset: 0x001290B8
	private void ReportScore()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", ref obj))
		{
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				this.AllFinalPlayerScores.Add(new RankedMultiplayerScore.PlayerScore
				{
					PlayerId = keyValuePair.Key,
					GameScore = this.ComputeGameScore(keyValuePair.Value.NumTags, keyValuePair.Value.PointsOnDefense),
					EloScore = (this.PlayerRankedElos.ContainsKey(keyValuePair.Key) ? this.PlayerRankedElos[keyValuePair.Key] : 0f),
					NumTags = keyValuePair.Value.NumTags,
					TimeUntagged = keyValuePair.Value.TaggedTime - keyValuePair.Value.JoinTime,
					PointsOnDefense = keyValuePair.Value.PointsOnDefense
				});
			}
			GorillaTagCompetitiveServerApi.Instance.RequestSubmitMatchScores((string)obj, this.AllFinalPlayerScores);
		}
		this.PredictPlayerEloChanges();
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x0012B004 File Offset: 0x00129204
	public float ComputeGameScore(int tags, float pointsOnDefense)
	{
		return (float)(tags * this.PointsPerTag) + pointsOnDefense;
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0012B014 File Offset: 0x00129214
	private void PredictPlayerEloChanges()
	{
		this.VisitedScoreCombintations.Clear();
		this.AllFinalPlayerScores = Enumerable.ToList<RankedMultiplayerScore.PlayerScore>(Enumerable.OrderByDescending<RankedMultiplayerScore.PlayerScore, float>(this.AllFinalPlayerScores, (RankedMultiplayerScore.PlayerScore s) => s.GameScore));
		float k = this.Progression.MaxEloConstant / (float)(this.AllFinalPlayerScores.Count - 1);
		this.InProgressEloDeltaPerPlayer.Clear();
		for (int i = 0; i < this.AllFinalPlayerScores.Count; i++)
		{
			this.InProgressEloDeltaPerPlayer.Add(this.AllFinalPlayerScores[i].PlayerId, 0f);
		}
		for (int j = 0; j < this.AllFinalPlayerScores.Count; j++)
		{
			for (int l = 0; l < this.AllFinalPlayerScores.Count; l++)
			{
				if (j != l)
				{
					bool flag = this.AllFinalPlayerScores[j].GameScore.Approx(this.AllFinalPlayerScores[l].GameScore, 1E-06f);
					float eloWinProbability = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[l].EloScore, this.AllFinalPlayerScores[j].EloScore);
					float eloWinProbability2 = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[j].EloScore, this.AllFinalPlayerScores[l].EloScore);
					int num = j * this.AllFinalPlayerScores.Count + l;
					if (!this.VisitedScoreCombintations.ContainsKey(num))
					{
						RankedMultiplayerScore.PlayerScore playerScore = this.AllFinalPlayerScores[j];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((j < l) ? 1 : 0);
						}
						float eloScore = playerScore.EloScore;
						float num2 = RankedProgressionManager.UpdateEloScore(eloScore, eloWinProbability, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num2 - eloScore;
						this.VisitedScoreCombintations.Add(num, true);
					}
					int num3 = l * this.AllFinalPlayerScores.Count + j;
					if (!this.VisitedScoreCombintations.ContainsKey(num3))
					{
						RankedMultiplayerScore.PlayerScore playerScore2 = this.AllFinalPlayerScores[l];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((l < j) ? 1 : 0);
						}
						float eloScore2 = playerScore2.EloScore;
						float num4 = RankedProgressionManager.UpdateEloScore(eloScore2, eloWinProbability2, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore2.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num4 - eloScore2;
						this.VisitedScoreCombintations.Add(num3, true);
					}
				}
			}
		}
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x0012B2A4 File Offset: 0x001294A4
	public void CachePlayerRankedProgressionData(int playerId, int tierIdx, float elo)
	{
		if (this.PlayerRankedTierIndices.ContainsKey(playerId))
		{
			this.PlayerRankedTierIndices[playerId] = tierIdx;
		}
		else
		{
			this.PlayerRankedTierIndices.Add(playerId, tierIdx);
		}
		if (this.PlayerRankedElos.ContainsKey(playerId))
		{
			this.PlayerRankedElos[playerId] = elo;
			return;
		}
		this.PlayerRankedElos.Add(playerId, elo);
	}

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x0600377C RID: 14204 RVA: 0x0012B304 File Offset: 0x00129504
	// (set) Token: 0x0600377D RID: 14205 RVA: 0x0012B30C File Offset: 0x0012950C
	public Dictionary<int, int> PlayerRankedTiers
	{
		get
		{
			return this.PlayerRankedTierIndices;
		}
		set
		{
			this.PlayerRankedTierIndices = value;
		}
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x0600377E RID: 14206 RVA: 0x0012B315 File Offset: 0x00129515
	// (set) Token: 0x0600377F RID: 14207 RVA: 0x0012B31D File Offset: 0x0012951D
	public Dictionary<int, float> PlayerRankedEloScores
	{
		get
		{
			return this.PlayerRankedElos;
		}
		set
		{
			this.PlayerRankedElos = value;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x06003780 RID: 14208 RVA: 0x0012B326 File Offset: 0x00129526
	// (set) Token: 0x06003781 RID: 14209 RVA: 0x0012B32E File Offset: 0x0012952E
	public Dictionary<int, float> ProjectedEloDeltas
	{
		get
		{
			return this.InProgressEloDeltaPerPlayer;
		}
		set
		{
			this.InProgressEloDeltaPerPlayer = value;
		}
	}

	// Token: 0x06003782 RID: 14210 RVA: 0x0012B338 File Offset: 0x00129538
	public List<RankedMultiplayerScore.PlayerScoreInRound> GetSortedScores()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> list = new List<RankedMultiplayerScore.PlayerScoreInRound>();
		foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
		{
			list.Add(keyValuePair.Value);
		}
		list.Sort((RankedMultiplayerScore.PlayerScoreInRound s1, RankedMultiplayerScore.PlayerScoreInRound s2) => this.ComputeGameScore(s2.NumTags, s2.PointsOnDefense).CompareTo(this.ComputeGameScore(s1.NumTags, s1.PointsOnDefense)));
		return list;
	}

	// Token: 0x040046C7 RID: 18119
	public static float LongestUntaggedTieEpsilon = 0.2f;

	// Token: 0x040046C8 RID: 18120
	public static int RESULT_TIE = -1;

	// Token: 0x040046C9 RID: 18121
	[SerializeField]
	private int PointsPerTag = 30;

	// Token: 0x040046CA RID: 18122
	[SerializeField]
	private float PointsPerUninfectedSecMin = 0.5f;

	// Token: 0x040046CB RID: 18123
	[SerializeField]
	private float PointsPerUninfectedSecMax = 2f;

	// Token: 0x040046CC RID: 18124
	private float PerSecondTimer = -1f;

	// Token: 0x040046CD RID: 18125
	private bool WasInfectedInitially;

	// Token: 0x040046CE RID: 18126
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x040046CF RID: 18127
	protected Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound> AllPlayerInRoundScores = new Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound>();

	// Token: 0x040046D0 RID: 18128
	protected List<RankedMultiplayerScore.PlayerScore> AllFinalPlayerScores = new List<RankedMultiplayerScore.PlayerScore>();

	// Token: 0x040046D1 RID: 18129
	protected Dictionary<int, bool> VisitedScoreCombintations = new Dictionary<int, bool>();

	// Token: 0x040046D2 RID: 18130
	protected Dictionary<int, float> InProgressEloDeltaPerPlayer = new Dictionary<int, float>();

	// Token: 0x040046D3 RID: 18131
	protected Dictionary<int, int> PlayerRankedTierIndices = new Dictionary<int, int>();

	// Token: 0x040046D4 RID: 18132
	protected Dictionary<int, float> PlayerRankedElos = new Dictionary<int, float>();

	// Token: 0x040046D5 RID: 18133
	private RankedMultiplayerScore.ResultData PendingResults;

	// Token: 0x040046D6 RID: 18134
	private RankedMultiplayerScore.RecordHolder<int> ResultsMostTags;

	// Token: 0x040046D7 RID: 18135
	private RankedMultiplayerScore.RecordHolder<float> ResultsLongestUntagged;

	// Token: 0x040046D8 RID: 18136
	private bool IsLateJoiner;

	// Token: 0x0200083C RID: 2108
	public struct PlayerScore
	{
		// Token: 0x040046DA RID: 18138
		public int PlayerId;

		// Token: 0x040046DB RID: 18139
		public float GameScore;

		// Token: 0x040046DC RID: 18140
		public float EloScore;

		// Token: 0x040046DD RID: 18141
		public int NumTags;

		// Token: 0x040046DE RID: 18142
		public float TimeUntagged;

		// Token: 0x040046DF RID: 18143
		public float PointsOnDefense;
	}

	// Token: 0x0200083D RID: 2109
	public struct PlayerScoreInRound
	{
		// Token: 0x06003786 RID: 14214 RVA: 0x0012B478 File Offset: 0x00129678
		public PlayerScoreInRound(int id, bool initInfected = false)
		{
			this.PlayerId = id;
			this.NumTags = 0;
			this.PointsOnDefense = 0f;
			this.JoinTime = Time.time;
			this.Infected = initInfected;
			this.TaggedTime = (initInfected ? Time.time : 0f);
		}

		// Token: 0x040046E0 RID: 18144
		public int PlayerId;

		// Token: 0x040046E1 RID: 18145
		public int NumTags;

		// Token: 0x040046E2 RID: 18146
		public float PointsOnDefense;

		// Token: 0x040046E3 RID: 18147
		public float JoinTime;

		// Token: 0x040046E4 RID: 18148
		public float TaggedTime;

		// Token: 0x040046E5 RID: 18149
		public bool Infected;
	}

	// Token: 0x0200083E RID: 2110
	public struct ResultData
	{
		// Token: 0x06003787 RID: 14215 RVA: 0x0012B4C5 File Offset: 0x001296C5
		public bool IsMostTagsTied()
		{
			return this.MostTagsPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x06003788 RID: 14216 RVA: 0x0012B4D4 File Offset: 0x001296D4
		public bool IsLongestUntaggedTied()
		{
			return this.LongestUntaggedPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x040046E6 RID: 18150
		public float Elo;

		// Token: 0x040046E7 RID: 18151
		public int Rank;

		// Token: 0x040046E8 RID: 18152
		public int MostTags;

		// Token: 0x040046E9 RID: 18153
		public float LongestUntagged;

		// Token: 0x040046EA RID: 18154
		public int MostTagsPlayerId;

		// Token: 0x040046EB RID: 18155
		public int LongestUntaggedPlayerId;
	}

	// Token: 0x0200083F RID: 2111
	public struct RecordHolder<T>
	{
		// Token: 0x040046EC RID: 18156
		public int PlayerId;

		// Token: 0x040046ED RID: 18157
		public T Value;
	}
}
