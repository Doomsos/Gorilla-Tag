using System;
using UnityEngine;

// Token: 0x0200023E RID: 574
public class AudioFader : MonoBehaviour
{
	// Token: 0x06000F12 RID: 3858 RVA: 0x0004FE1B File Offset: 0x0004E01B
	private void Start()
	{
		this.fadeInSpeed = this.maxVolume / this.fadeInDuration;
		this.fadeOutSpeed = this.maxVolume / this.fadeOutDuration;
	}

	// Token: 0x06000F13 RID: 3859 RVA: 0x0004FE44 File Offset: 0x0004E044
	public void FadeIn()
	{
		this.targetVolume = this.maxVolume;
		if (this.fadeInDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeInSpeed;
		}
		else
		{
			this.currentVolume = this.maxVolume;
		}
		this.audioToFade.volume = this.currentVolume;
		if (!this.audioToFade.isPlaying)
		{
			this.audioToFade.GTPlay();
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0004FEB4 File Offset: 0x0004E0B4
	public void FadeOut()
	{
		this.targetVolume = 0f;
		if (this.fadeOutDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeOutSpeed;
		}
		else
		{
			this.currentVolume = 0f;
			if (this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
		if (this.outro != null && this.currentVolume > 0f)
		{
			this.outro.volume = this.currentVolume;
			this.outro.GTPlay();
		}
	}

	// Token: 0x06000F15 RID: 3861 RVA: 0x0004FF48 File Offset: 0x0004E148
	private void Update()
	{
		this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.targetVolume, this.currentFadeSpeed * Time.deltaTime);
		this.audioToFade.volume = this.currentVolume;
		if (this.currentVolume == this.targetVolume)
		{
			base.enabled = false;
			if (this.currentVolume == 0f && this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
	}

	// Token: 0x04001261 RID: 4705
	[SerializeField]
	private AudioSource audioToFade;

	// Token: 0x04001262 RID: 4706
	[SerializeField]
	private AudioSource outro;

	// Token: 0x04001263 RID: 4707
	[SerializeField]
	private float fadeInDuration = 0.3f;

	// Token: 0x04001264 RID: 4708
	[SerializeField]
	private float fadeOutDuration = 0.3f;

	// Token: 0x04001265 RID: 4709
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04001266 RID: 4710
	private float currentVolume;

	// Token: 0x04001267 RID: 4711
	private float targetVolume;

	// Token: 0x04001268 RID: 4712
	private float currentFadeSpeed;

	// Token: 0x04001269 RID: 4713
	private float fadeInSpeed;

	// Token: 0x0400126A RID: 4714
	private float fadeOutSpeed;
}
