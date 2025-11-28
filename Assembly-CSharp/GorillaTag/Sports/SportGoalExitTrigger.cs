using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001020 RID: 4128
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x06006862 RID: 26722 RVA: 0x0021FD88 File Offset: 0x0021DF88
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x040076FE RID: 30462
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
