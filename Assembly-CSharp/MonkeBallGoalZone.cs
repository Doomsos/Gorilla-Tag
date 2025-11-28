using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000551 RID: 1361
public class MonkeBallGoalZone : MonoBehaviourTick
{
	// Token: 0x06002273 RID: 8819 RVA: 0x000B47AC File Offset: 0x000B29AC
	public override void Tick()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (MonkeBallGame.Instance.GetGameState() == MonkeBallGame.GameState.Playing)
		{
			for (int i = 0; i < this.playersInGoalZone.Count; i++)
			{
				MonkeBallPlayer monkeBallPlayer = this.playersInGoalZone[i];
				if (monkeBallPlayer.gamePlayer.teamId != this.teamId)
				{
					GameBallId gameBallId = monkeBallPlayer.gamePlayer.GetGameBallId();
					if (gameBallId.IsValid())
					{
						MonkeBallGame.Instance.RequestScore(monkeBallPlayer.gamePlayer.teamId);
						GameBallId gameBallId2 = monkeBallPlayer.gamePlayer.GetGameBallId();
						int otherTeam = MonkeBallGame.Instance.GetOtherTeam(monkeBallPlayer.gamePlayer.teamId);
						if (MonkeBallGame.Instance.resetBallPositionOnScore)
						{
							MonkeBallGame.Instance.RequestResetBall(gameBallId2, otherTeam);
						}
						MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(gameBallId2, otherTeam);
						monkeBallPlayer.gamePlayer.ClearGrabbedIfHeld(gameBallId);
					}
				}
			}
		}
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000B488C File Offset: 0x000B2A8C
	private void OnTriggerEnter(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = this;
				this.playersInGoalZone.Add(component);
			}
		}
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000B48DC File Offset: 0x000B2ADC
	private void OnTriggerExit(Collider other)
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(other, true);
		if (gamePlayer != null && gamePlayer.teamId != this.teamId)
		{
			MonkeBallPlayer component = gamePlayer.GetComponent<MonkeBallPlayer>();
			if (component != null)
			{
				component.currGoalZone = null;
				this.playersInGoalZone.Remove(component);
			}
		}
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000B492C File Offset: 0x000B2B2C
	public void CleanupPlayer(MonkeBallPlayer player)
	{
		this.playersInGoalZone.Remove(player);
	}

	// Token: 0x04002D05 RID: 11525
	public int teamId;

	// Token: 0x04002D06 RID: 11526
	public List<MonkeBallPlayer> playersInGoalZone;
}
