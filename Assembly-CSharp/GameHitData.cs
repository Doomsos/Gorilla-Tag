using System;
using UnityEngine;

// Token: 0x0200061C RID: 1564
public struct GameHitData
{
	// Token: 0x04003347 RID: 13127
	public GameEntityId hitEntityId;

	// Token: 0x04003348 RID: 13128
	public GameEntityId hitByEntityId;

	// Token: 0x04003349 RID: 13129
	public int hitTypeId;

	// Token: 0x0400334A RID: 13130
	public Vector3 hitEntityPosition;

	// Token: 0x0400334B RID: 13131
	public Vector3 hitPosition;

	// Token: 0x0400334C RID: 13132
	public Vector3 hitImpulse;

	// Token: 0x0400334D RID: 13133
	public int hitAmount;
}
