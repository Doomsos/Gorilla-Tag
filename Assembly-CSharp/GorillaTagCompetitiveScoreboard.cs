using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007BE RID: 1982
public class GorillaTagCompetitiveScoreboard : MonoBehaviour
{
	// Token: 0x06003438 RID: 13368 RVA: 0x00118738 File Offset: 0x00116938
	private void Awake()
	{
		GorillaTagCompetitiveManager.RegisterScoreboard(this);
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x00118771 File Offset: 0x00116971
	private void OnDestroy()
	{
		GorillaTagCompetitiveManager.DeregisterScoreboard(this);
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x0011877C File Offset: 0x0011697C
	public void UpdateScores(GorillaTagCompetitiveManager.GameState gameState, float activeRoundTime, List<RankedMultiplayerScore.PlayerScoreInRound> scores, Dictionary<int, int> PlayerRankedTiers, Dictionary<int, float> PlayerPredictedEloDeltas, List<NetPlayer> infectedPlayers, RankedProgressionManager progressionManager)
	{
		this.waitingForPlayers.SetActive(gameState == GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		for (int i = 0; i < this.lines.Length; i++)
		{
			if (gameState != GorillaTagCompetitiveManager.GameState.WaitingForPlayers && scores != null && scores.Count > i)
			{
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = scores[i];
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerScoreInRound.PlayerId);
				if (netPlayerByID != null)
				{
					this.lines[i].gameObject.SetActive(true);
					if (PlayerRankedTiers == null || !PlayerRankedTiers.ContainsKey(playerScoreInRound.PlayerId))
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, null);
					}
					else
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, progressionManager.GetProgressionRankIcon(PlayerRankedTiers[playerScoreInRound.PlayerId]));
					}
					if (playerScoreInRound.TaggedTime.Approx(0f, 1E-06f))
					{
						this.lines[i].SetScore(Mathf.Max(activeRoundTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					else
					{
						this.lines[i].SetScore(Mathf.Max(playerScoreInRound.TaggedTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					if (PlayerPredictedEloDeltas.ContainsKey(playerScoreInRound.PlayerId))
					{
						float num = PlayerPredictedEloDeltas[playerScoreInRound.PlayerId];
						GorillaTagCompetitiveScoreboard.PredictedResult predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Even;
						if (num > this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Great;
						}
						else if (num > this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Good;
						}
						else if (num < -this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Poor;
						}
						else if (num < -this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Bad;
						}
						this.lines[i].SetPredictedResult(predictedResult);
					}
					this.lines[i].SetInfected(gameState == GorillaTagCompetitiveManager.GameState.Playing && infectedPlayers.Contains(netPlayerByID));
				}
			}
			else
			{
				this.lines[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x0011894C File Offset: 0x00116B4C
	public void DisplayPredictedResults(bool bShow)
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x04004284 RID: 17028
	public GorillaTagCompetitiveScoreboardLine[] lines;

	// Token: 0x04004285 RID: 17029
	public GameObject waitingForPlayers;

	// Token: 0x04004286 RID: 17030
	public float smallEloDelta = 10f;

	// Token: 0x04004287 RID: 17031
	public float largeEloDelta = 25f;

	// Token: 0x020007BF RID: 1983
	public enum PredictedResult
	{
		// Token: 0x04004289 RID: 17033
		Great,
		// Token: 0x0400428A RID: 17034
		Good,
		// Token: 0x0400428B RID: 17035
		Even,
		// Token: 0x0400428C RID: 17036
		Bad,
		// Token: 0x0400428D RID: 17037
		Poor
	}
}
