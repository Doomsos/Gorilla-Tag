using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x06000AC7 RID: 2759 RVA: 0x0003A830 File Offset: 0x00038A30
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x0003A880 File Offset: 0x00038A80
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play();
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x04000D36 RID: 3382
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000D37 RID: 3383
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x04000D38 RID: 3384
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x04000D39 RID: 3385
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000D3A RID: 3386
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04000D3B RID: 3387
	private float maxBurstProbability;

	// Token: 0x04000D3C RID: 3388
	private int previousParticleCount;
}
