using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class CosmeticCritterSpawnerShadeFleeing : CosmeticCritterSpawner
{
	// Token: 0x0600046C RID: 1132 RVA: 0x0001971F File Offset: 0x0001791F
	public void SetSpawnPosition(Vector3 pos)
	{
		this.spawnPosition = pos;
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x00019728 File Offset: 0x00017928
	public override void OnSpawn(CosmeticCritter critter)
	{
		base.OnSpawn(critter);
		(critter as CosmeticCritterShadeFleeing).SetFleePosition(this.spawnPosition, base.transform.position);
	}

	// Token: 0x04000503 RID: 1283
	private Vector3 spawnPosition;
}
