using System;

namespace GorillaNetworking
{
	// Token: 0x02000EFE RID: 3838
	public class GorillaNetworkRankedJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06006053 RID: 24659 RVA: 0x001F122E File Offset: 0x001EF42E
		public override string GetFullDesiredGameModeString()
		{
			return this.networkZone + base.GetDesiredGameType();
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001F1241 File Offset: 0x001EF441
		public override void OnBoxTriggered()
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoom(this, JoinType.Solo);
		}
	}
}
