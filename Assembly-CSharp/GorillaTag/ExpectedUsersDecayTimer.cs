using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001010 RID: 4112
	[Serializable]
	internal class ExpectedUsersDecayTimer : TickSystemTimerAbstract
	{
		// Token: 0x06006817 RID: 26647 RVA: 0x0021F6F8 File Offset: 0x0021D8F8
		public override void OnTimedEvent()
		{
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				int num = 0;
				if (PhotonNetwork.CurrentRoom.ExpectedUsers != null && PhotonNetwork.CurrentRoom.ExpectedUsers.Length != 0)
				{
					foreach (string text in PhotonNetwork.CurrentRoom.ExpectedUsers)
					{
						float num2;
						if (this.expectedUsers.TryGetValue(text, ref num2))
						{
							if (num2 + this.decayTime < Time.time)
							{
								num++;
							}
						}
						else
						{
							this.expectedUsers.Add(text, Time.time);
						}
					}
					if (num >= PhotonNetwork.CurrentRoom.ExpectedUsers.Length && num != 0)
					{
						PhotonNetwork.CurrentRoom.ClearExpectedUsers();
						this.expectedUsers.Clear();
					}
				}
			}
		}

		// Token: 0x06006818 RID: 26648 RVA: 0x0021F7BF File Offset: 0x0021D9BF
		public override void Stop()
		{
			base.Stop();
			this.expectedUsers.Clear();
		}

		// Token: 0x040076F0 RID: 30448
		public float decayTime = 15f;

		// Token: 0x040076F1 RID: 30449
		private Dictionary<string, float> expectedUsers = new Dictionary<string, float>(10);
	}
}
