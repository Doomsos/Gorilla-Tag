using System;
using UnityEngine;

// Token: 0x020005C8 RID: 1480
public abstract class CosmeticCritterSpawner : CosmeticCritterHoldable
{
	// Token: 0x06002581 RID: 9601 RVA: 0x000C8BA8 File Offset: 0x000C6DA8
	public GameObject GetCritterPrefab()
	{
		return this.critterPrefab;
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x000C8BB0 File Offset: 0x000C6DB0
	public CosmeticCritter GetCritter()
	{
		return this.cachedCritter;
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x000C8BB8 File Offset: 0x000C6DB8
	public Type GetCritterType()
	{
		return this.cachedType;
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void SetRandomVariables(CosmeticCritter critter)
	{
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000C8BC0 File Offset: 0x000C6DC0
	public virtual void OnSpawn(CosmeticCritter critter)
	{
		this.numCritters++;
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x000C8BD0 File Offset: 0x000C6DD0
	public virtual void OnDespawn(CosmeticCritter critter)
	{
		this.numCritters = Math.Max(this.numCritters - 1, 0);
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000C8BE6 File Offset: 0x000C6DE6
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.cachedCritter == null)
		{
			this.cachedCritter = this.critterPrefab.GetComponent<CosmeticCritter>();
			this.cachedType = this.cachedCritter.GetType();
		}
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000C8C1E File Offset: 0x000C6E1E
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04003112 RID: 12562
	[Tooltip("The critter prefab to spawn.")]
	[SerializeField]
	protected GameObject critterPrefab;

	// Token: 0x04003113 RID: 12563
	[Tooltip("The maximum number of critters that this spawner can have active at once.")]
	[SerializeField]
	protected int maxCritters;

	// Token: 0x04003114 RID: 12564
	protected CosmeticCritter cachedCritter;

	// Token: 0x04003115 RID: 12565
	protected Type cachedType;

	// Token: 0x04003116 RID: 12566
	protected int numCritters;

	// Token: 0x04003117 RID: 12567
	protected float nextLocalSpawnTime;
}
