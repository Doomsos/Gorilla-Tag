using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E38 RID: 3640
	public class TappableBell : Tappable
	{
		// Token: 0x14000099 RID: 153
		// (add) Token: 0x06005AD7 RID: 23255 RVA: 0x001D179C File Offset: 0x001CF99C
		// (remove) Token: 0x06005AD8 RID: 23256 RVA: 0x001D17D4 File Offset: 0x001CF9D4
		public event TappableBell.ObstacleCourseTriggerEvent OnTapped;

		// Token: 0x06005AD9 RID: 23257 RVA: 0x001D180C File Offset: 0x001CFA0C
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				return;
			}
			if (!this.rpcCooldown.CheckCallTime(Time.time))
			{
				return;
			}
			this.winnerRig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (this.winnerRig != null)
			{
				TappableBell.ObstacleCourseTriggerEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(this.winnerRig);
			}
		}

		// Token: 0x040067FE RID: 26622
		private VRRig winnerRig;

		// Token: 0x04006800 RID: 26624
		public CallLimiter rpcCooldown;

		// Token: 0x02000E39 RID: 3641
		// (Invoke) Token: 0x06005ADC RID: 23260
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
