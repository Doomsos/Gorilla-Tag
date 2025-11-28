using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001121 RID: 4385
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		// Token: 0x06006DBD RID: 28093 RVA: 0x00240630 File Offset: 0x0023E830
		private void Awake()
		{
			if (this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
			}
			if (this.soundBank != null && this.audioSource != null)
			{
				this.soundBank.audioSource = this.audioSource;
			}
		}

		// Token: 0x06006DBE RID: 28094 RVA: 0x0024068C File Offset: 0x0023E88C
		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float num = Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude);
			float num2 = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, num);
			this.audioSource.volume = num2;
			if (this.audioSource != null && !this.audioSource.isPlaying && this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (this.soundBank != null && this.soundBank.soundBank != null && !this.soundBank.isPlaying)
			{
				this.soundBank.Play(new float?(num2), default(float?));
			}
		}

		// Token: 0x04007F3C RID: 32572
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04007F3D RID: 32573
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007F3E RID: 32574
		[SerializeField]
		private AudioClip audioClip;

		// Token: 0x04007F3F RID: 32575
		[SerializeField]
		private SoundBankPlayer soundBank;

		// Token: 0x04007F40 RID: 32576
		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold = 0.5f;

		// Token: 0x04007F41 RID: 32577
		[SerializeField]
		private float maxVelocity = 2f;

		// Token: 0x04007F42 RID: 32578
		[SerializeField]
		private float minOutputVolume;

		// Token: 0x04007F43 RID: 32579
		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
