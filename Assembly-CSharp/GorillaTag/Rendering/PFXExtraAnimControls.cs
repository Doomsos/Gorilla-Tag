using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001082 RID: 4226
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x060069E7 RID: 27111 RVA: 0x002283AC File Offset: 0x002265AC
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x060069E8 RID: 27112 RVA: 0x0022848C File Offset: 0x0022668C
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x04007942 RID: 31042
		public float emitRateMult = 1f;

		// Token: 0x04007943 RID: 31043
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x04007944 RID: 31044
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04007945 RID: 31045
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04007946 RID: 31046
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x04007947 RID: 31047
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
