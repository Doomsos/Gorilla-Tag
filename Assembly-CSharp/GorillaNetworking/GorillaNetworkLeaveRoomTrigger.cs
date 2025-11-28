using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EFB RID: 3835
	public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
	{
		// Token: 0x0600604D RID: 24653 RVA: 0x001F1068 File Offset: 0x001EF268
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (NetworkSystem.Instance.InRoom && (!this.excludePrivateRooms || !NetworkSystem.Instance.SessionIsPrivate))
			{
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x0600604E RID: 24654 RVA: 0x001F10C8 File Offset: 0x001EF2C8
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaNetworkLeaveRoomTrigger.<DisconnectAfterDelay>d__2>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x04006EF7 RID: 28407
		[SerializeField]
		private bool excludePrivateRooms;
	}
}
