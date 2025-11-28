using System;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public interface IGameProjectileLauncher
{
	// Token: 0x06002D53 RID: 11603 RVA: 0x00002789 File Offset: 0x00000989
	void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x00002789 File Offset: 0x00000989
	void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}
}
