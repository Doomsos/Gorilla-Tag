using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class CrittersStunBomb : CrittersToolThrowable
{
	// Token: 0x060002CF RID: 719 RVA: 0x000110E4 File Offset: 0x0000F2E4
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			Vector3 position = base.transform.position;
			List<CrittersPawn> crittersPawns = CrittersManager.instance.crittersPawns;
			for (int i = 0; i < crittersPawns.Count; i++)
			{
				CrittersPawn crittersPawn = crittersPawns[i];
				if (crittersPawn.isActiveAndEnabled && Vector3.Distance(crittersPawn.transform.position, position) < this.radius)
				{
					crittersPawn.Stunned(this.stunDuration);
				}
			}
			CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StunExplosion, this.actorId, position, Quaternion.LookRotation(hitNormal));
		}
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00011178 File Offset: 0x0000F378
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			impactedCritter.Stunned(this.stunDuration);
		}
	}

	// Token: 0x04000339 RID: 825
	[Header("Stun Bomb")]
	public float radius = 1f;

	// Token: 0x0400033A RID: 826
	public float stunDuration = 5f;
}
