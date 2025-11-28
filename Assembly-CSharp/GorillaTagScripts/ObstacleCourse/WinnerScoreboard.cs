using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E3A RID: 3642
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x06005ADF RID: 23263 RVA: 0x001D1868 File Offset: 0x001CFA68
		public void UpdateBoard(string winner, ObstacleCourse.RaceState _currentState)
		{
			if (this.output == null)
			{
				return;
			}
			switch (_currentState)
			{
			case ObstacleCourse.RaceState.Started:
				Debug.Log(this.raceStarted);
				this.output.text = this.raceStarted;
				return;
			case ObstacleCourse.RaceState.Waiting:
				Debug.Log(this.raceLoading);
				this.output.text = this.raceLoading;
				return;
			case ObstacleCourse.RaceState.Finished:
				Debug.Log(winner + " WON!!");
				this.output.text = winner + " WON!!";
				return;
			default:
				return;
			}
		}

		// Token: 0x04006801 RID: 26625
		public string raceStarted = "RACE STARTED!";

		// Token: 0x04006802 RID: 26626
		public string raceLoading = "RACE LOADING...";

		// Token: 0x04006803 RID: 26627
		[SerializeField]
		private TextMeshPro output;
	}
}
