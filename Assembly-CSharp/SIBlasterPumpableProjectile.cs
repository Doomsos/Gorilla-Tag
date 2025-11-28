using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class SIBlasterPumpableProjectile : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x060004A0 RID: 1184 RVA: 0x0001A8A0 File Offset: 0x00018AA0
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		SIGadgetPumpBlaster component = projectile.parentBlaster.GetComponent<SIGadgetPumpBlaster>();
		if (component == null)
		{
			return;
		}
		this.pumpChargedAmount = Mathf.Min(this.maxPump, component.currentPumpChargeAmount);
		projectile.startingVelocity += this.pumpChargedAmount;
		if (this.strengthPerPumpCharge > 0f)
		{
			SIBlasterDirectHitProjectile component2 = projectile.GetComponent<SIBlasterDirectHitProjectile>();
			if (component2 != null)
			{
				component2.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSplashProjectile component3 = projectile.GetComponent<SIBlasterSplashProjectile>();
			if (component3 != null)
			{
				component3.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSprayProjectile component4 = projectile.GetComponent<SIBlasterSprayProjectile>();
			if (component4 != null)
			{
				component4.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
		}
	}

	// Token: 0x0400056D RID: 1389
	public float maxPump;

	// Token: 0x0400056E RID: 1390
	public float pumpChargedAmount;

	// Token: 0x0400056F RID: 1391
	public float velocityPerPumpCharge;

	// Token: 0x04000570 RID: 1392
	public float strengthPerPumpCharge;
}
