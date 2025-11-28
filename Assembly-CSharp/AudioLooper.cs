using System;
using UnityEngine;

// Token: 0x02000C24 RID: 3108
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x06004C70 RID: 19568 RVA: 0x0018D4FF File Offset: 0x0018B6FF
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06004C71 RID: 19569 RVA: 0x0018D510 File Offset: 0x0018B710
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			if (this.audioSource.clip == this.loopClip && this.interjectionClips.Length != 0 && Random.value < this.interjectionLikelyhood)
			{
				this.audioSource.clip = this.interjectionClips[Random.Range(0, this.interjectionClips.Length)];
			}
			else
			{
				this.audioSource.clip = this.loopClip;
			}
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x04005C49 RID: 23625
	private AudioSource audioSource;

	// Token: 0x04005C4A RID: 23626
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04005C4B RID: 23627
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x04005C4C RID: 23628
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
