using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000662 RID: 1634
[Serializable]
public class AbilitySound
{
	// Token: 0x060029DD RID: 10717 RVA: 0x000E2493 File Offset: 0x000E0693
	public bool IsValid()
	{
		return this.sounds != null && this.sounds.Count > 0;
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x000E24B0 File Offset: 0x000E06B0
	private void UpdateNextSound()
	{
		AbilitySound.SoundSelectMode soundSelectMode = this.soundSelectMode;
		if (soundSelectMode == AbilitySound.SoundSelectMode.Sequential)
		{
			this.nextSound = (this.nextSound + 1) % this.sounds.Count;
			return;
		}
		if (soundSelectMode != AbilitySound.SoundSelectMode.Random)
		{
			return;
		}
		this.nextSound = Random.Range(0, this.sounds.Count);
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x000E2500 File Offset: 0x000E0700
	public void Play(AudioSource audioSourceIn)
	{
		this.usedAudioSource = ((audioSourceIn != null) ? audioSourceIn : this.audioSource);
		if (this.sounds != null && this.sounds.Count > 0 && this.usedAudioSource != null)
		{
			if (this.nextSound < 0)
			{
				this.UpdateNextSound();
			}
			AudioClip audioClip = this.sounds[this.nextSound];
			this.UpdateNextSound();
			if (audioClip != null)
			{
				this.usedAudioSource.clip = audioClip;
				this.usedAudioSource.volume = this.volume;
				this.usedAudioSource.pitch = this.pitch;
				this.usedAudioSource.loop = this.loop;
				if (this.delay <= 0f)
				{
					this.usedAudioSource.Play();
				}
				else
				{
					this.usedAudioSource.PlayDelayed(this.delay);
				}
				this.currentSound = audioClip;
			}
		}
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000E25F4 File Offset: 0x000E07F4
	public void Stop()
	{
		if (this.usedAudioSource != null && this.usedAudioSource.clip == this.currentSound)
		{
			this.usedAudioSource.Stop();
			this.currentSound = null;
			this.usedAudioSource = null;
		}
	}

	// Token: 0x0400360A RID: 13834
	public float volume = 1f;

	// Token: 0x0400360B RID: 13835
	public float pitch = 1f;

	// Token: 0x0400360C RID: 13836
	public bool loop;

	// Token: 0x0400360D RID: 13837
	public float delay;

	// Token: 0x0400360E RID: 13838
	public List<AudioClip> sounds;

	// Token: 0x0400360F RID: 13839
	private AudioClip currentSound;

	// Token: 0x04003610 RID: 13840
	public AudioSource audioSource;

	// Token: 0x04003611 RID: 13841
	private AudioSource usedAudioSource;

	// Token: 0x04003612 RID: 13842
	private int nextSound = -1;

	// Token: 0x04003613 RID: 13843
	public AbilitySound.SoundSelectMode soundSelectMode;

	// Token: 0x02000663 RID: 1635
	public enum SoundSelectMode
	{
		// Token: 0x04003615 RID: 13845
		Sequential,
		// Token: 0x04003616 RID: 13846
		Random
	}
}
