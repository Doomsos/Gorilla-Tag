using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x06002188 RID: 8584 RVA: 0x000AFC50 File Offset: 0x000ADE50
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x04002C40 RID: 11328
	public Collider[] collidersToAdd;

	// Token: 0x04002C41 RID: 11329
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x04002C42 RID: 11330
	private int count;

	// Token: 0x04002C43 RID: 11331
	private int index;
}
