using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class AnimationEventListener : MonoBehaviour
{
	// Token: 0x06002193 RID: 8595 RVA: 0x000AFE14 File Offset: 0x000AE014
	public void PlaySoundAtIndex(int index)
	{
		if (this.audioClips.Length <= index || index < 0)
		{
			return;
		}
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioClips[index] == null)
		{
			return;
		}
		this.audioSource.GTPlayOneShot(this.audioClips[index], 1f);
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x000AFE69 File Offset: 0x000AE069
	public void StopAudio()
	{
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000AFE92 File Offset: 0x000AE092
	public void ActivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(true);
		}
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000AFEAE File Offset: 0x000AE0AE
	public void DeactivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(false);
		}
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000AFECA File Offset: 0x000AE0CA
	public void ToggleObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(!this.targetObject.activeSelf);
		}
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000AFEF3 File Offset: 0x000AE0F3
	public void PlayParticles()
	{
		if (this.particles != null && !this.particles.isPlaying)
		{
			this.particles.Play();
		}
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000AFF1B File Offset: 0x000AE11B
	public void StopParticles()
	{
		if (this.particles != null && this.particles.isPlaying)
		{
			this.particles.Stop();
		}
	}

	// Token: 0x04002C4B RID: 11339
	[Tooltip("Set this if calling ActivateObject, DeactivateObject, or ToggleObject")]
	[SerializeField]
	private GameObject targetObject;

	// Token: 0x04002C4C RID: 11340
	[Tooltip("Set this if calling PlayParticles or StopParticles")]
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04002C4D RID: 11341
	[Tooltip("Set this if calling PlaySoundAtIndex or StopAudio")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002C4E RID: 11342
	[Tooltip("Set this if calling PlaySoundAtIndex")]
	[SerializeField]
	private AudioClip[] audioClips;
}
