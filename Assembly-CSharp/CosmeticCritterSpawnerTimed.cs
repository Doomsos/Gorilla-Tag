using System;
using UnityEngine;

// Token: 0x020005CA RID: 1482
public abstract class CosmeticCritterSpawnerTimed : CosmeticCritterSpawnerIndependent
{
	// Token: 0x0600258F RID: 9615 RVA: 0x000C8C7A File Offset: 0x000C6E7A
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(5, this.spawnIntervalMinMax.x, 0.5f);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000C8C92 File Offset: 0x000C6E92
	public override bool CanSpawnLocal()
	{
		if (Time.time >= this.nextLocalSpawnTime)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
			return base.CanSpawnLocal();
		}
		return false;
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000C8CD0 File Offset: 0x000C6ED0
	public override bool CanSpawnRemote(double serverTime)
	{
		return base.CanSpawnRemote(serverTime);
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000C8CD9 File Offset: 0x000C6ED9
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.IsLocal)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
		}
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000C8D10 File Offset: 0x000C6F10
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x04003118 RID: 12568
	[Tooltip("The minimum and maximum time to wait between spawn attempts.")]
	[SerializeField]
	private Vector2 spawnIntervalMinMax = new Vector2(2f, 5f);

	// Token: 0x04003119 RID: 12569
	[Tooltip("Currently does nothing.")]
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance = 1f;
}
