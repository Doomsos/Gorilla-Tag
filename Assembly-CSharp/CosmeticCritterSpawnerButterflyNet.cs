using System;
using UnityEngine;

// Token: 0x020000AB RID: 171
public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	// Token: 0x06000454 RID: 1108 RVA: 0x00018FB4 File Offset: 0x000171B4
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 startPos = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(startPos);
	}

	// Token: 0x040004E5 RID: 1253
	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
