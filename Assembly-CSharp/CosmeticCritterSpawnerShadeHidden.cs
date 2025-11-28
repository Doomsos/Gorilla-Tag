using System;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public class CosmeticCritterSpawnerShadeHidden : CosmeticCritterSpawnerTimed
{
	// Token: 0x0600046F RID: 1135 RVA: 0x00019758 File Offset: 0x00017958
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		float num = Random.Range(this.orbitHeightOffsetMinMax.x, this.orbitHeightOffsetMinMax.y);
		float radius = Random.Range(this.orbitRadiusMinMax.x, this.orbitRadiusMinMax.y);
		(critter as CosmeticCritterShadeHidden).SetCenterAndRadius(base.transform.position + new Vector3(0f, num, 0f), radius);
	}

	// Token: 0x04000504 RID: 1284
	[Tooltip("Add between X and Y extra height to the base orbit height.")]
	[SerializeField]
	private Vector2 orbitHeightOffsetMinMax = new Vector2(0f, 2f);

	// Token: 0x04000505 RID: 1285
	[Tooltip("Orbit between X (green sphere) and Y (red sphere) units away from this spawner's position when first spawned.")]
	[SerializeField]
	private Vector2 orbitRadiusMinMax = new Vector2(5f, 10f);
}
