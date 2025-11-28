using System;

// Token: 0x020005C9 RID: 1481
public class CosmeticCritterSpawnerIndependent : CosmeticCritterSpawner
{
	// Token: 0x0600258A RID: 9610 RVA: 0x000C8C26 File Offset: 0x000C6E26
	public virtual bool CanSpawnLocal()
	{
		return this.numCritters < this.maxCritters;
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x000C8C36 File Offset: 0x000C6E36
	public virtual bool CanSpawnRemote(double serverTime)
	{
		return this.numCritters < this.maxCritters && this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000C8C54 File Offset: 0x000C6E54
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterIndependentSpawner(this);
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000C8C67 File Offset: 0x000C6E67
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterIndependentSpawner(this);
	}
}
