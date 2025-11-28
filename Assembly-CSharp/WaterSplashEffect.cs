using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class WaterSplashEffect : MonoBehaviour
{
	// Token: 0x060013BC RID: 5052 RVA: 0x000728D7 File Offset: 0x00070AD7
	private void OnEnable()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x000728E4 File Offset: 0x00070AE4
	public void Destroy()
	{
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.DeactivateParticleSystems(this.smallSplashParticleSystems);
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x00072918 File Offset: 0x00070B18
	public void PlayEffect(bool isBigSplash, bool isEntry, float scale, WaterVolume volume = null)
	{
		this.waterVolume = volume;
		if (isBigSplash)
		{
			this.DeactivateParticleSystems(this.smallSplashParticleSystems);
			this.SetParticleEffectParameters(this.bigSplashParticleSystems, scale, this.bigSplashBaseGravityMultiplier, this.bigSplashBaseStartSpeed, this.bigSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.bigSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.bigSplashAudioClips, ref WaterSplashEffect.lastPlayedBigSplashAudioClipIndex);
			return;
		}
		if (isEntry)
		{
			this.DeactivateParticleSystems(this.bigSplashParticleSystems);
			this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
			this.PlayParticleEffects(this.smallSplashParticleSystems);
			this.PlayRandomAudioClipWithoutRepeats(this.smallSplashEntryAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashEntryAudioClipIndex);
			return;
		}
		this.DeactivateParticleSystems(this.bigSplashParticleSystems);
		this.SetParticleEffectParameters(this.smallSplashParticleSystems, scale, this.smallSplashBaseGravityMultiplier, this.smallSplashBaseStartSpeed, this.smallSplashBaseSimulationSpeed, this.waterVolume);
		this.PlayParticleEffects(this.smallSplashParticleSystems);
		this.PlayRandomAudioClipWithoutRepeats(this.smallSplashExitAudioClips, ref WaterSplashEffect.lastPlayedSmallSplashExitAudioClipIndex);
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x00072A20 File Offset: 0x00070C20
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 vector = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - vector;
		}
		if ((Time.time - this.startTime) / this.lifeTime >= 1f)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x00072AE8 File Offset: 0x00070CE8
	private void DeactivateParticleSystems(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x00072B14 File Offset: 0x00070D14
	private void PlayParticleEffects(ParticleSystem[] particleSystems)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i].gameObject.SetActive(true);
				particleSystems[i].Play();
			}
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x00072B48 File Offset: 0x00070D48
	private void SetParticleEffectParameters(ParticleSystem[] particleSystems, float scale, float baseGravMultiplier, float baseStartSpeed, float baseSimulationSpeed, WaterVolume waterVolume = null)
	{
		if (particleSystems != null)
		{
			for (int i = 0; i < particleSystems.Length; i++)
			{
				ParticleSystem.MainModule main = particleSystems[i].main;
				main.startSpeed = baseStartSpeed;
				main.gravityModifier = baseGravMultiplier;
				if (scale < 0.99f)
				{
					main.startSpeed = baseStartSpeed * scale * 2f;
					main.gravityModifier = baseGravMultiplier * scale * 0.5f;
				}
				if (waterVolume != null && waterVolume.Parameters != null)
				{
					particleSystems[i].colorBySpeed.color = waterVolume.Parameters.splashColorBySpeedGradient;
				}
			}
		}
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x00072C00 File Offset: 0x00070E00
	private void PlayRandomAudioClipWithoutRepeats(AudioClip[] audioClips, ref int lastPlayedAudioClipIndex)
	{
		if (this.audioSource != null && audioClips != null && audioClips.Length != 0)
		{
			int num = 0;
			if (audioClips.Length > 1)
			{
				int num2 = Random.Range(0, audioClips.Length);
				if (num2 == lastPlayedAudioClipIndex)
				{
					num2 = ((Random.Range(0f, 1f) > 0.5f) ? ((num2 + 1) % audioClips.Length) : (num2 - 1));
					if (num2 < 0)
					{
						num2 = audioClips.Length - 1;
					}
				}
				num = num2;
			}
			lastPlayedAudioClipIndex = num;
			this.audioSource.clip = audioClips[num];
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x04001E24 RID: 7716
	private static int lastPlayedBigSplashAudioClipIndex = -1;

	// Token: 0x04001E25 RID: 7717
	private static int lastPlayedSmallSplashEntryAudioClipIndex = -1;

	// Token: 0x04001E26 RID: 7718
	private static int lastPlayedSmallSplashExitAudioClipIndex = -1;

	// Token: 0x04001E27 RID: 7719
	public ParticleSystem[] bigSplashParticleSystems;

	// Token: 0x04001E28 RID: 7720
	public ParticleSystem[] smallSplashParticleSystems;

	// Token: 0x04001E29 RID: 7721
	public float bigSplashBaseGravityMultiplier = 0.9f;

	// Token: 0x04001E2A RID: 7722
	public float bigSplashBaseStartSpeed = 1.9f;

	// Token: 0x04001E2B RID: 7723
	public float bigSplashBaseSimulationSpeed = 0.9f;

	// Token: 0x04001E2C RID: 7724
	public float smallSplashBaseGravityMultiplier = 0.6f;

	// Token: 0x04001E2D RID: 7725
	public float smallSplashBaseStartSpeed = 0.6f;

	// Token: 0x04001E2E RID: 7726
	public float smallSplashBaseSimulationSpeed = 0.6f;

	// Token: 0x04001E2F RID: 7727
	public float lifeTime = 1f;

	// Token: 0x04001E30 RID: 7728
	private float startTime = -1f;

	// Token: 0x04001E31 RID: 7729
	public AudioSource audioSource;

	// Token: 0x04001E32 RID: 7730
	public AudioClip[] bigSplashAudioClips;

	// Token: 0x04001E33 RID: 7731
	public AudioClip[] smallSplashEntryAudioClips;

	// Token: 0x04001E34 RID: 7732
	public AudioClip[] smallSplashExitAudioClips;

	// Token: 0x04001E35 RID: 7733
	private WaterVolume waterVolume;
}
