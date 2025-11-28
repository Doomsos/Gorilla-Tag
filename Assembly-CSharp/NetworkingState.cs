using System;

// Token: 0x0200030F RID: 783
public enum NetworkingState
{
	// Token: 0x04001CA5 RID: 7333
	IsOwner,
	// Token: 0x04001CA6 RID: 7334
	IsBlindClient,
	// Token: 0x04001CA7 RID: 7335
	IsClient,
	// Token: 0x04001CA8 RID: 7336
	ForcefullyTakingOver,
	// Token: 0x04001CA9 RID: 7337
	RequestingOwnership,
	// Token: 0x04001CAA RID: 7338
	RequestingOwnershipWaitingForSight,
	// Token: 0x04001CAB RID: 7339
	ForcefullyTakingOverWaitingForSight
}
