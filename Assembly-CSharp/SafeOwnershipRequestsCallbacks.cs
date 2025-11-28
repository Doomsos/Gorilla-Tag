using System;
using UnityEngine;

// Token: 0x02000766 RID: 1894
public class SafeOwnershipRequestsCallbacks : MonoBehaviour, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x0600311A RID: 12570 RVA: 0x0010B2C4 File Offset: 0x001094C4
	private void Awake()
	{
		this._requestableOwnershipGuard.AddCallbackTarget(this);
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x00002789 File Offset: 0x00000989
	void IRequestableOwnershipGuardCallbacks.OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x00002789 File Offset: 0x00000989
	void IRequestableOwnershipGuardCallbacks.OnMyOwnerLeft()
	{
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x00002076 File Offset: 0x00000276
	bool IRequestableOwnershipGuardCallbacks.OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x00002789 File Offset: 0x00000989
	void IRequestableOwnershipGuardCallbacks.OnMyCreatorLeft()
	{
	}

	// Token: 0x04003FE1 RID: 16353
	[SerializeField]
	private RequestableOwnershipGuard _requestableOwnershipGuard;
}
