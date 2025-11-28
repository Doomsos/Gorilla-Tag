using System;

// Token: 0x02000314 RID: 788
public interface IRequestableOwnershipGuardCallbacks
{
	// Token: 0x0600133C RID: 4924
	void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer);

	// Token: 0x0600133D RID: 4925
	bool OnOwnershipRequest(NetPlayer fromPlayer);

	// Token: 0x0600133E RID: 4926
	void OnMyOwnerLeft();

	// Token: 0x0600133F RID: 4927
	bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer);

	// Token: 0x06001340 RID: 4928
	void OnMyCreatorLeft();
}
