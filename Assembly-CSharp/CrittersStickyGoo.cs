using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class CrittersStickyGoo : CrittersActor
{
	// Token: 0x060002BE RID: 702 RVA: 0x00010D77 File Offset: 0x0000EF77
	public override void Initialize()
	{
		base.Initialize();
		this.readyToDisable = false;
	}

	// Token: 0x060002BF RID: 703 RVA: 0x00010D88 File Offset: 0x0000EF88
	public bool CanAffect(Vector3 position)
	{
		return (base.transform.position - position).magnitude < this.range;
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x00010DB6 File Offset: 0x0000EFB6
	public void EffectApplied(CrittersPawn critter)
	{
		if (this.destroyOnApply)
		{
			this.readyToDisable = true;
		}
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, this.actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00010DF8 File Offset: 0x0000EFF8
	public override bool ProcessLocal()
	{
		bool result = base.ProcessLocal();
		if (this.readyToDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return result;
	}

	// Token: 0x04000331 RID: 817
	[Header("Sticky Goo")]
	public float range = 1f;

	// Token: 0x04000332 RID: 818
	public float slowModifier = 0.3f;

	// Token: 0x04000333 RID: 819
	public float slowDuration = 3f;

	// Token: 0x04000334 RID: 820
	public bool destroyOnApply = true;

	// Token: 0x04000335 RID: 821
	private bool readyToDisable;
}
