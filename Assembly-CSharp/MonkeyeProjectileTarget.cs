using System;
using UnityEngine;

// Token: 0x02000396 RID: 918
public class MonkeyeProjectileTarget : MonoBehaviour
{
	// Token: 0x060015F9 RID: 5625 RVA: 0x0007AAA5 File Offset: 0x00078CA5
	private void Awake()
	{
		this.monkeyeAI = base.GetComponent<MonkeyeAI>();
		this.notifier = base.GetComponentInChildren<SlingshotProjectileHitNotifier>();
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x0007AABF File Offset: 0x00078CBF
	private void OnEnable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit += this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit += this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x0007AAFD File Offset: 0x00078CFD
	private void OnDisable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit -= this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit -= this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x0007AB3B File Offset: 0x00078D3B
	private void Notifier_OnProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x0007AB3B File Offset: 0x00078D3B
	private void Notifier_OnPaperPlaneHit(PaperPlaneProjectile projectile, Collider collider)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x04002040 RID: 8256
	private MonkeyeAI monkeyeAI;

	// Token: 0x04002041 RID: 8257
	private SlingshotProjectileHitNotifier notifier;
}
