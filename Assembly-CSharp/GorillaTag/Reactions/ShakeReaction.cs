using System;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x0200102F RID: 4143
	public class ShakeReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x060068A6 RID: 26790 RVA: 0x002216CE File Offset: 0x0021F8CE
		private float loopSoundTotalDuration
		{
			get
			{
				return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
			}
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x060068A7 RID: 26791 RVA: 0x002216E4 File Offset: 0x0021F8E4
		// (set) Token: 0x060068A8 RID: 26792 RVA: 0x002216EC File Offset: 0x0021F8EC
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x060068A9 RID: 26793 RVA: 0x002216F8 File Offset: 0x0021F8F8
		protected void Awake()
		{
			this.sampleHistoryPos = new Vector3[256];
			this.sampleHistoryTime = new float[256];
			this.sampleHistoryVel = new Vector3[256];
			if (this.particles != null)
			{
				this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
			}
			Application.quitting += new Action(this.HandleApplicationQuitting);
		}

		// Token: 0x060068AA RID: 26794 RVA: 0x00221778 File Offset: 0x0021F978
		protected void OnEnable()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			for (int i = 0; i < 256; i++)
			{
				this.sampleHistoryTime[i] = unscaledTime;
				this.sampleHistoryPos[i] = position;
				this.sampleHistoryVel[i] = Vector3.zero;
			}
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.loop = true;
				this.loopSoundAudioSource.GTPlay();
			}
			this.hasLoopSound = (this.loopSoundAudioSource != null);
			this.hasShakeSound = (this.shakeSoundBankPlayer != null);
			this.hasParticleSystem = (this.particles != null);
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x060068AB RID: 26795 RVA: 0x0022182F File Offset: 0x0021FA2F
		protected void OnDisable()
		{
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.GTStop();
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060068AC RID: 26796 RVA: 0x001338F3 File Offset: 0x00131AF3
		private void HandleApplicationQuitting()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x060068AD RID: 26797 RVA: 0x00221850 File Offset: 0x0021FA50
		void ITickSystemPost.PostTick()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			int num = (this.currentIndex - 1 + 256) % 256;
			this.currentIndex = (this.currentIndex + 1) % 256;
			this.sampleHistoryTime[this.currentIndex] = unscaledTime;
			float num2 = unscaledTime - this.sampleHistoryTime[num];
			this.sampleHistoryPos[this.currentIndex] = position;
			if (num2 > 0f)
			{
				Vector3 vector = position - this.sampleHistoryPos[num];
				this.sampleHistoryVel[this.currentIndex] = vector / num2;
			}
			else
			{
				this.sampleHistoryVel[this.currentIndex] = Vector3.zero;
			}
			float sqrMagnitude = (this.sampleHistoryVel[num] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
			this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
			float num3 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
			if (sqrMagnitude >= num3)
			{
				this.lastShakeTime = unscaledTime;
			}
			float num4 = unscaledTime - this.lastShakeTime;
			float num5 = Mathf.Clamp01(num4 / this.particleDuration);
			if (this.hasParticleSystem)
			{
				this.particles.emission.rateOverTime = this.emissionCurve.Evaluate(num5) * this.maxEmissionRate;
			}
			if (this.hasShakeSound && this.lastShakeTime - this.lastShakeSoundTime > this.shakeSoundCooldown)
			{
				this.shakeSoundBankPlayer.Play();
				this.lastShakeSoundTime = unscaledTime;
			}
			if (this.hasLoopSound)
			{
				if (num4 < this.loopSoundFadeInDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num4 / this.loopSoundFadeInDuration));
					return;
				}
				if (num4 < this.loopSoundFadeInDuration + this.loopSoundSustainDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
					return;
				}
				this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num4 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
			}
		}

		// Token: 0x04007756 RID: 30550
		[SerializeField]
		private Transform shakeXform;

		// Token: 0x04007757 RID: 30551
		[SerializeField]
		private float velocityThreshold = 5f;

		// Token: 0x04007758 RID: 30552
		[SerializeField]
		private SoundBankPlayer shakeSoundBankPlayer;

		// Token: 0x04007759 RID: 30553
		[SerializeField]
		private float shakeSoundCooldown = 1f;

		// Token: 0x0400775A RID: 30554
		[SerializeField]
		private AudioSource loopSoundAudioSource;

		// Token: 0x0400775B RID: 30555
		[SerializeField]
		private float loopSoundBaseVolume = 1f;

		// Token: 0x0400775C RID: 30556
		[SerializeField]
		private float loopSoundSustainDuration = 1f;

		// Token: 0x0400775D RID: 30557
		[SerializeField]
		private float loopSoundFadeInDuration = 1f;

		// Token: 0x0400775E RID: 30558
		[SerializeField]
		private AnimationCurve loopSoundFadeInCurve;

		// Token: 0x0400775F RID: 30559
		[SerializeField]
		private float loopSoundFadeOutDuration = 1f;

		// Token: 0x04007760 RID: 30560
		[SerializeField]
		private AnimationCurve loopSoundFadeOutCurve;

		// Token: 0x04007761 RID: 30561
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x04007762 RID: 30562
		[SerializeField]
		private AnimationCurve emissionCurve;

		// Token: 0x04007763 RID: 30563
		[SerializeField]
		private float particleDuration = 5f;

		// Token: 0x04007765 RID: 30565
		private const int sampleHistorySize = 256;

		// Token: 0x04007766 RID: 30566
		private float[] sampleHistoryTime;

		// Token: 0x04007767 RID: 30567
		private Vector3[] sampleHistoryPos;

		// Token: 0x04007768 RID: 30568
		private Vector3[] sampleHistoryVel;

		// Token: 0x04007769 RID: 30569
		private int currentIndex;

		// Token: 0x0400776A RID: 30570
		private float lastShakeSoundTime = float.MinValue;

		// Token: 0x0400776B RID: 30571
		private float lastShakeTime = float.MinValue;

		// Token: 0x0400776C RID: 30572
		private float maxEmissionRate;

		// Token: 0x0400776D RID: 30573
		private bool hasLoopSound;

		// Token: 0x0400776E RID: 30574
		private bool hasShakeSound;

		// Token: 0x0400776F RID: 30575
		private bool hasParticleSystem;

		// Token: 0x04007770 RID: 30576
		[DebugReadout]
		private float poopVelocity;
	}
}
