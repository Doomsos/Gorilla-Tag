using System;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class SIBlasterRandomAngularVelocity : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x060004A2 RID: 1186 RVA: 0x0001A978 File Offset: 0x00018B78
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.rb.angularVelocity = new Vector3(Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel));
	}

	// Token: 0x04000571 RID: 1393
	public float maxVel;
}
