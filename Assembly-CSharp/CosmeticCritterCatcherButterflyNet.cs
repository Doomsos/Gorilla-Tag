using System;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class CosmeticCritterCatcherButterflyNet : CosmeticCritterCatcher
{
	// Token: 0x06000450 RID: 1104 RVA: 0x00018E7C File Offset: 0x0001707C
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (!(critter is CosmeticCritterButterfly) || (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude > this.maxCatchRadius * this.maxCatchRadius || this.velocityEstimator.linearVelocity.sqrMagnitude < this.minCatchSpeed * this.minCatchSpeed)
		{
			return CosmeticCritterAction.None;
		}
		return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00018EF0 File Offset: 0x000170F0
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return base.ValidateRemoteCatchAction(critter, catchAction, serverTime) && critter is CosmeticCritterButterfly && (critter.transform.position - this.velocityEstimator.transform.position).sqrMagnitude <= this.maxCatchRadius * this.maxCatchRadius + 1f && this.velocityEstimator.linearVelocity.sqrMagnitude >= this.minCatchSpeed * this.minCatchSpeed - 1f && catchAction == (CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn);
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00018F7B File Offset: 0x0001717B
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.caughtButterflyParticleSystem.Emit((critter as CosmeticCritterButterfly).GetEmitParams, 1);
		this.catchFX.Play();
		this.catchSFX.Play();
	}

	// Token: 0x040004DF RID: 1247
	[Tooltip("Use this for calculating the catch position and velocity.")]
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040004E0 RID: 1248
	[Tooltip("Catch the Butterfly if it is within this radius.")]
	[SerializeField]
	private float maxCatchRadius;

	// Token: 0x040004E1 RID: 1249
	[Tooltip("Only catch the Butterfly if the net is moving faster than this speed.")]
	[SerializeField]
	private float minCatchSpeed;

	// Token: 0x040004E2 RID: 1250
	[Tooltip("Spawn a particle inside the net representing the caught Butterfly.")]
	[SerializeField]
	private ParticleSystem caughtButterflyParticleSystem;

	// Token: 0x040004E3 RID: 1251
	[Tooltip("Play this particle effect when catching a Butterfly.")]
	[SerializeField]
	private ParticleSystem catchFX;

	// Token: 0x040004E4 RID: 1252
	[Tooltip("Play this sound when catching a Butterfly.")]
	[SerializeField]
	private AudioSource catchSFX;
}
