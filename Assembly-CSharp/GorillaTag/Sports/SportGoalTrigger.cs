using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001021 RID: 4129
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x06006864 RID: 26724 RVA: 0x0021FD9F File Offset: 0x0021DF9F
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x06006865 RID: 26725 RVA: 0x0021FDBC File Offset: 0x0021DFBC
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x06006866 RID: 26726 RVA: 0x0021FE48 File Offset: 0x0021E048
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x040076FF RID: 30463
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04007700 RID: 30464
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x04007701 RID: 30465
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x04007702 RID: 30466
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
