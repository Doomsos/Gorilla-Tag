using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E61 RID: 3681
	public class BuilderScaleAudioRadius : MonoBehaviour
	{
		// Token: 0x06005BFD RID: 23549 RVA: 0x001D8509 File Offset: 0x001D6709
		private void OnEnable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06005BFE RID: 23550 RVA: 0x001D8525 File Offset: 0x001D6725
		private void OnDisable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x001D8535 File Offset: 0x001D6735
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScaleOnEnable)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x001D8571 File Offset: 0x001D6771
		private void PlaySound()
		{
			if (this.autoPlaySoundBank != null)
			{
				this.autoPlaySoundBank.Play();
				return;
			}
			if (this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x001D85AC File Offset: 0x001D67AC
		public void SetScale(float inScale)
		{
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > 1)
			{
				if (rolloffMode == 2)
				{
					this.maxDist = this.audioSource.maxDistance;
					this.audioSource.maxDistance *= this.scale;
				}
			}
			else
			{
				this.minDist = this.audioSource.minDistance;
				this.maxDist = this.audioSource.maxDistance;
				this.audioSource.maxDistance *= this.scale;
				this.audioSource.minDistance *= this.scale;
			}
			if (this.autoPlay)
			{
				this.PlaySound();
			}
			this.shouldRevert = true;
		}

		// Token: 0x06005C02 RID: 23554 RVA: 0x001D86AC File Offset: 0x001D68AC
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > 1)
			{
				if (rolloffMode == 2)
				{
					this.audioSource.maxDistance = this.maxDist;
				}
			}
			else
			{
				this.audioSource.minDistance = this.minDist;
				this.audioSource.maxDistance = this.maxDist;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04006947 RID: 26951
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScaleOnEnable;

		// Token: 0x04006948 RID: 26952
		[Tooltip("Play sound after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04006949 RID: 26953
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x0400694A RID: 26954
		[FormerlySerializedAs("soundBankToPlay")]
		[SerializeField]
		private SoundBankPlayer autoPlaySoundBank;

		// Token: 0x0400694B RID: 26955
		private float minDist;

		// Token: 0x0400694C RID: 26956
		private float maxDist = 1f;

		// Token: 0x0400694D RID: 26957
		private AnimationCurve customCurve;

		// Token: 0x0400694E RID: 26958
		private AnimationCurve scaledCurve = new AnimationCurve();

		// Token: 0x0400694F RID: 26959
		private float scale = 1f;

		// Token: 0x04006950 RID: 26960
		private bool shouldRevert;

		// Token: 0x04006951 RID: 26961
		private bool setScaleNextFrame;

		// Token: 0x04006952 RID: 26962
		private int enableFrame;
	}
}
