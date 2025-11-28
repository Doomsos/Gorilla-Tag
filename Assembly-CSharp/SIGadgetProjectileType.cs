using System;

// Token: 0x020000C6 RID: 198
public interface SIGadgetProjectileType
{
	// Token: 0x060004D4 RID: 1236
	void LocalProjectileHit(SIPlayer player = null);

	// Token: 0x060004D5 RID: 1237
	void NetworkedProjectileHit(object[] data);
}
