using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class SIResourceMonkeIdol : SIResource
{
	// Token: 0x06000856 RID: 2134 RVA: 0x0002D46A File Offset: 0x0002B66A
	protected override void OnEnable()
	{
		base.OnEnable();
		this.depositEnabledParticle.SetActive(SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType));
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002D48D File Offset: 0x0002B68D
	public override void HandleDepositAuth(SIPlayer depositingPlayer)
	{
		SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(base.transform.position);
	}

	// Token: 0x04000A37 RID: 2615
	[SerializeField]
	private GameObject depositEnabledParticle;
}
