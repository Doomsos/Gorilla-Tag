using System;
using UnityEngine;

// Token: 0x020005C4 RID: 1476
public abstract class CosmeticCritterCatcher : CosmeticCritterHoldable
{
	// Token: 0x06002558 RID: 9560 RVA: 0x000C819B File Offset: 0x000C639B
	public CosmeticCritterSpawner GetLinkedSpawner()
	{
		return this.optionalLinkedSpawner;
	}

	// Token: 0x06002559 RID: 9561
	public abstract CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter);

	// Token: 0x0600255A RID: 9562 RVA: 0x000C81A3 File Offset: 0x000C63A3
	public virtual bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x0600255B RID: 9563
	public abstract void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime);

	// Token: 0x0600255C RID: 9564 RVA: 0x000C81B1 File Offset: 0x000C63B1
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterCatcher(this);
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000C81C4 File Offset: 0x000C63C4
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterCatcher(this);
	}

	// Token: 0x040030FB RID: 12539
	[SerializeField]
	[Tooltip("If this catcher is capable of spawning immediately after catching, the linked spawner must be assigned here.")]
	protected CosmeticCritterSpawner optionalLinkedSpawner;
}
