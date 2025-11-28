using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.GuidedRefs;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02001006 RID: 4102
	public class VolcanoEffects : BaseGuidedRefTargetMono
	{
		// Token: 0x060067CC RID: 26572 RVA: 0x0021E42C File Offset: 0x0021C62C
		protected override void Awake()
		{
			base.Awake();
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
			{
				this.LogNullsFoundInArray("lavaSpewParticleSystems");
			}
			if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
			{
				this.LogNullsFoundInArray("smokeParticleSystems");
			}
			this.hasVolcanoAudioSrc = (this.volcanoAudioSource != null);
			this.hasForestSpeakerAudioSrc = (this.forestSpeakerAudioSrc != null);
			this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
			this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
			this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
			for (int i = 0; i < this.lavaSpewParticleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[i].emission;
				this.lavaSpewEmissionDefaultRateMultipliers[i] = emission.rateOverTimeMultiplier;
				this.lavaSpewDefaultEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.lavaSpewAdjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					ParticleSystem.Burst burst = emission.GetBurst(j);
					this.lavaSpewDefaultEmitBursts[i][j] = burst;
					this.lavaSpewAdjustedEmitBursts[i][j] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
					this.lavaSpewAdjustedEmitBursts[i][j].count = burst.count;
				}
				this.lavaSpewEmissionModules[i] = emission;
			}
			this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
			this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
			this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
			for (int k = 0; k < this.smokeParticleSystems.Length; k++)
			{
				this.smokeMainModules[k] = this.smokeParticleSystems[k].main;
				this.smokeEmissionModules[k] = this.smokeParticleSystems[k].emission;
				this.smokeEmissionDefaultRateMultipliers[k] = this.smokeEmissionModules[k].rateOverTimeMultiplier;
			}
			this.InitState(this.drainedStateFX);
			this.InitState(this.eruptingStateFX);
			this.InitState(this.risingStateFX);
			this.InitState(this.fullStateFX);
			this.InitState(this.drainingStateFX);
			this.currentStateFX = this.drainedStateFX;
			this.UpdateDrainedState(0f);
		}

		// Token: 0x060067CD RID: 26573 RVA: 0x0021E6BC File Offset: 0x0021C8BC
		public void OnVolcanoBellyEmpty()
		{
			if (!this.hasForestSpeakerAudioSrc)
			{
				return;
			}
			if (Time.time - this.timeVolcanoBellyWasLastEmpty < this.warnVolcanoBellyEmptied.length)
			{
				return;
			}
			this.forestSpeakerAudioSrc.gameObject.SetActive(true);
			this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied, 1f);
		}

		// Token: 0x060067CE RID: 26574 RVA: 0x0021E714 File Offset: 0x0021C914
		public void OnStoneAccepted(double activationProgress)
		{
			if (!this.hasVolcanoAudioSrc)
			{
				return;
			}
			this.volcanoAudioSource.gameObject.SetActive(true);
			if (activationProgress > 1.0)
			{
				this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone, 1f);
				return;
			}
			this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone, 1f);
		}

		// Token: 0x060067CF RID: 26575 RVA: 0x0021E774 File Offset: 0x0021C974
		private void InitState(VolcanoEffects.LavaStateFX fx)
		{
			fx.startSoundExists = (fx.startSound != null);
			fx.endSoundExists = (fx.endSound != null);
			fx.loop1Exists = (fx.loop1AudioSrc != null);
			fx.loop2Exists = (fx.loop2AudioSrc != null);
			if (fx.loop1Exists)
			{
				fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
				fx.loop1AudioSrc.volume = 0f;
			}
			if (fx.loop2Exists)
			{
				fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
				fx.loop2AudioSrc.volume = 0f;
			}
		}

		// Token: 0x060067D0 RID: 26576 RVA: 0x0021E81C File Offset: 0x0021CA1C
		private void SetLavaAudioEnabled(bool toEnable)
		{
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x060067D1 RID: 26577 RVA: 0x0021E84C File Offset: 0x0021CA4C
		private void SetLavaAudioEnabled(bool toEnable, float volume)
		{
			foreach (AudioSource audioSource in this.lavaSurfaceAudioSrcs)
			{
				audioSource.volume = volume;
				audioSource.gameObject.SetActive(toEnable);
			}
		}

		// Token: 0x060067D2 RID: 26578 RVA: 0x0021E884 File Offset: 0x0021CA84
		private void ResetState()
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			this.currentStateFX.startSoundPlayed = false;
			this.currentStateFX.endSoundPlayed = false;
			if (this.currentStateFX.startSoundExists)
			{
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.endSoundExists)
			{
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
			}
		}

		// Token: 0x060067D3 RID: 26579 RVA: 0x0021E940 File Offset: 0x0021CB40
		private void UpdateState(float time, float timeRemaining, float progress)
		{
			if (this.currentStateFX == null)
			{
				return;
			}
			if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && time >= this.currentStateFX.startSoundDelay)
			{
				this.currentStateFX.startSoundPlayed = true;
				this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
			}
			if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && timeRemaining <= this.currentStateFX.endSound.length + this.currentStateFX.endSoundPadTime)
			{
				this.currentStateFX.endSoundPlayed = true;
				this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
				this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
			}
			if (this.currentStateFX.loop1Exists)
			{
				this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
				if (!this.currentStateFX.loop1AudioSrc.isPlaying)
				{
					this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop1AudioSrc.GTPlay();
				}
			}
			if (this.currentStateFX.loop2Exists)
			{
				this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
				if (!this.currentStateFX.loop2AudioSrc.isPlaying)
				{
					this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
					this.currentStateFX.loop2AudioSrc.GTPlay();
				}
			}
			for (int i = 0; i < this.smokeMainModules.Length; i++)
			{
				this.smokeMainModules[i].startColor = this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
				this.smokeEmissionModules[i].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[i];
			}
			this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
			if (this.applyShaderGlobals)
			{
				Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
				Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
			}
		}

		// Token: 0x060067D4 RID: 26580 RVA: 0x0021EC15 File Offset: 0x0021CE15
		public void SetDrainedState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false);
			this.currentStateFX = this.drainedStateFX;
		}

		// Token: 0x060067D5 RID: 26581 RVA: 0x0021EC30 File Offset: 0x0021CE30
		public void UpdateDrainedState(float time)
		{
			this.ResetState();
			this.UpdateState(time, float.MaxValue, float.MinValue);
		}

		// Token: 0x060067D6 RID: 26582 RVA: 0x0021EC49 File Offset: 0x0021CE49
		public void SetEruptingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(false, 0f);
			this.currentStateFX = this.eruptingStateFX;
		}

		// Token: 0x060067D7 RID: 26583 RVA: 0x0021EC69 File Offset: 0x0021CE69
		public void UpdateEruptingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x060067D8 RID: 26584 RVA: 0x0021EC74 File Offset: 0x0021CE74
		public void SetRisingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 0f);
			this.currentStateFX = this.risingStateFX;
		}

		// Token: 0x060067D9 RID: 26585 RVA: 0x0021EC94 File Offset: 0x0021CE94
		public void UpdateRisingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(0f, 1f, Mathf.Clamp01(time));
			}
		}

		// Token: 0x060067DA RID: 26586 RVA: 0x0021ECDC File Offset: 0x0021CEDC
		public void SetFullState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.fullStateFX;
		}

		// Token: 0x060067DB RID: 26587 RVA: 0x0021EC69 File Offset: 0x0021CE69
		public void UpdateFullState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
		}

		// Token: 0x060067DC RID: 26588 RVA: 0x0021ECFC File Offset: 0x0021CEFC
		public void SetDrainingState()
		{
			this.ResetState();
			this.SetLavaAudioEnabled(true, 1f);
			this.currentStateFX = this.drainingStateFX;
		}

		// Token: 0x060067DD RID: 26589 RVA: 0x0021ED1C File Offset: 0x0021CF1C
		public void UpdateDrainingState(float time, float timeRemaining, float progress)
		{
			this.UpdateState(time, timeRemaining, progress);
			AudioSource[] array = this.lavaSurfaceAudioSrcs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volume = Mathf.Lerp(1f, 0f, progress);
			}
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x0021ED60 File Offset: 0x0021CF60
		private void SetParticleEmissionRateAndBurst(float multiplier, ParticleSystem.EmissionModule[] emissionModules, float[] defaultRateMultipliers, ParticleSystem.Burst[][] defaultEmitBursts, ParticleSystem.Burst[][] adjustedEmitBursts)
		{
			for (int i = 0; i < emissionModules.Length; i++)
			{
				emissionModules[i].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[i];
				int num = Mathf.Min(emissionModules[i].burstCount, defaultEmitBursts[i].Length);
				for (int j = 0; j < num; j++)
				{
					adjustedEmitBursts[i][j].probability = defaultEmitBursts[i][j].probability * multiplier;
				}
				emissionModules[i].SetBursts(adjustedEmitBursts[i]);
			}
		}

		// Token: 0x060067DF RID: 26591 RVA: 0x0021EDE0 File Offset: 0x0021CFE0
		private bool RemoveNullsFromArray<T>(ref T[] array) where T : Object
		{
			List<T> list = new List<T>(array.Length);
			foreach (T t in array)
			{
				if (t != null)
				{
					list.Add(t);
				}
			}
			int num = array.Length;
			array = list.ToArray();
			return num != array.Length;
		}

		// Token: 0x060067E0 RID: 26592 RVA: 0x0021EE3A File Offset: 0x0021D03A
		private void LogNullsFoundInArray(string nameOfArray)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Null reference found in ",
				nameOfArray,
				" array of component: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
		}

		// Token: 0x040076A4 RID: 30372
		[Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
		[SerializeField]
		private bool applyShaderGlobals = true;

		// Token: 0x040076A5 RID: 30373
		[Tooltip("Game trigger notification sounds will play through this.")]
		[SerializeField]
		private AudioSource forestSpeakerAudioSrc;

		// Token: 0x040076A6 RID: 30374
		[Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
		[SerializeField]
		private AudioClip warnVolcanoBellyEmptied;

		// Token: 0x040076A7 RID: 30375
		[Tooltip("Accept stone sounds will play through here.")]
		[SerializeField]
		private AudioSource volcanoAudioSource;

		// Token: 0x040076A8 RID: 30376
		[Tooltip("volcano ate rock but needs more.")]
		[SerializeField]
		private AudioClip volcanoAcceptStone;

		// Token: 0x040076A9 RID: 30377
		[Tooltip("volcano ate last needed rock.")]
		[SerializeField]
		private AudioClip volcanoAcceptLastStone;

		// Token: 0x040076AA RID: 30378
		[Tooltip("This will be faded in while lava is rising.")]
		[SerializeField]
		private AudioSource[] lavaSurfaceAudioSrcs;

		// Token: 0x040076AB RID: 30379
		[Tooltip("Emission will be adjusted for these particles during eruption.")]
		[SerializeField]
		private ParticleSystem[] lavaSpewParticleSystems;

		// Token: 0x040076AC RID: 30380
		[Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
		[SerializeField]
		private ParticleSystem[] smokeParticleSystems;

		// Token: 0x040076AD RID: 30381
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainedStateFX;

		// Token: 0x040076AE RID: 30382
		[SerializeField]
		private VolcanoEffects.LavaStateFX eruptingStateFX;

		// Token: 0x040076AF RID: 30383
		[SerializeField]
		private VolcanoEffects.LavaStateFX risingStateFX;

		// Token: 0x040076B0 RID: 30384
		[SerializeField]
		private VolcanoEffects.LavaStateFX fullStateFX;

		// Token: 0x040076B1 RID: 30385
		[SerializeField]
		private VolcanoEffects.LavaStateFX drainingStateFX;

		// Token: 0x040076B2 RID: 30386
		private VolcanoEffects.LavaStateFX currentStateFX;

		// Token: 0x040076B3 RID: 30387
		private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;

		// Token: 0x040076B4 RID: 30388
		private float[] lavaSpewEmissionDefaultRateMultipliers;

		// Token: 0x040076B5 RID: 30389
		private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;

		// Token: 0x040076B6 RID: 30390
		private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;

		// Token: 0x040076B7 RID: 30391
		private ParticleSystem.MainModule[] smokeMainModules;

		// Token: 0x040076B8 RID: 30392
		private ParticleSystem.EmissionModule[] smokeEmissionModules;

		// Token: 0x040076B9 RID: 30393
		private float[] smokeEmissionDefaultRateMultipliers;

		// Token: 0x040076BA RID: 30394
		private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");

		// Token: 0x040076BB RID: 30395
		private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");

		// Token: 0x040076BC RID: 30396
		private float timeVolcanoBellyWasLastEmpty;

		// Token: 0x040076BD RID: 30397
		private bool hasVolcanoAudioSrc;

		// Token: 0x040076BE RID: 30398
		private bool hasForestSpeakerAudioSrc;

		// Token: 0x02001007 RID: 4103
		[Serializable]
		public class LavaStateFX
		{
			// Token: 0x040076BF RID: 30399
			public AudioClip startSound;

			// Token: 0x040076C0 RID: 30400
			public AudioSource startSoundAudioSrc;

			// Token: 0x040076C1 RID: 30401
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float startSoundVol = 1f;

			// Token: 0x040076C2 RID: 30402
			[FormerlySerializedAs("startSoundPad")]
			public float startSoundDelay;

			// Token: 0x040076C3 RID: 30403
			public AudioClip endSound;

			// Token: 0x040076C4 RID: 30404
			public AudioSource endSoundAudioSrc;

			// Token: 0x040076C5 RID: 30405
			[Tooltip("Multiplied by the AudioSource's volume.")]
			public float endSoundVol = 1f;

			// Token: 0x040076C6 RID: 30406
			[Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
			public float endSoundPadTime;

			// Token: 0x040076C7 RID: 30407
			public AudioSource loop1AudioSrc;

			// Token: 0x040076C8 RID: 30408
			public AnimationCurve loop1VolAnim;

			// Token: 0x040076C9 RID: 30409
			public AudioSource loop2AudioSrc;

			// Token: 0x040076CA RID: 30410
			public AnimationCurve loop2VolAnim;

			// Token: 0x040076CB RID: 30411
			public AnimationCurve lavaSpewEmissionAnim;

			// Token: 0x040076CC RID: 30412
			public AnimationCurve smokeEmissionAnim;

			// Token: 0x040076CD RID: 30413
			public Gradient smokeStartColorAnim;

			// Token: 0x040076CE RID: 30414
			public Gradient lavaLightColor;

			// Token: 0x040076CF RID: 30415
			public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0f, 1f, 60f);

			// Token: 0x040076D0 RID: 30416
			public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0f, 1f, 0.1f);

			// Token: 0x040076D1 RID: 30417
			[NonSerialized]
			public bool startSoundExists;

			// Token: 0x040076D2 RID: 30418
			[NonSerialized]
			public bool startSoundPlayed;

			// Token: 0x040076D3 RID: 30419
			[NonSerialized]
			public bool endSoundExists;

			// Token: 0x040076D4 RID: 30420
			[NonSerialized]
			public bool endSoundPlayed;

			// Token: 0x040076D5 RID: 30421
			[NonSerialized]
			public bool loop1Exists;

			// Token: 0x040076D6 RID: 30422
			[NonSerialized]
			public float loop1DefaultVolume;

			// Token: 0x040076D7 RID: 30423
			[NonSerialized]
			public bool loop2Exists;

			// Token: 0x040076D8 RID: 30424
			[NonSerialized]
			public float loop2DefaultVolume;
		}
	}
}
