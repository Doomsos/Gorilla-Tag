using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class AudioAnimator : MonoBehaviour
{
	// Token: 0x06000EFB RID: 3835 RVA: 0x0004F7D6 File Offset: 0x0004D9D6
	private void Start()
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0004F7E8 File Offset: 0x0004D9E8
	private void InitBaseVolume()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			this.targets[i].baseVolume = this.targets[i].audioSource.volume;
		}
		this.didInitBaseVolume = true;
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x0004F836 File Offset: 0x0004DA36
	public void UpdateValue(float value, bool ignoreSmoothing = false)
	{
		this.UpdatePitchAndVolume(value, value, ignoreSmoothing);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0004F844 File Offset: 0x0004DA44
	public void UpdatePitchAndVolume(float pitchValue, float volumeValue, bool ignoreSmoothing = false)
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
		for (int i = 0; i < this.targets.Length; i++)
		{
			AudioAnimator.AudioTarget audioTarget = this.targets[i];
			float num = audioTarget.pitchCurve.Evaluate(pitchValue);
			float pitch = Mathf.Pow(1.05946f, num);
			audioTarget.audioSource.pitch = pitch;
			float num2 = audioTarget.volumeCurve.Evaluate(volumeValue);
			float volume = audioTarget.audioSource.volume;
			float num3 = audioTarget.baseVolume * num2;
			if (ignoreSmoothing)
			{
				audioTarget.audioSource.volume = num3;
			}
			else if (volume > num3)
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num2, (1f - audioTarget.lowerSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
			else
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num2, (1f - audioTarget.riseSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
		}
	}

	// Token: 0x04001246 RID: 4678
	private bool didInitBaseVolume;

	// Token: 0x04001247 RID: 4679
	[SerializeField]
	private AudioAnimator.AudioTarget[] targets;

	// Token: 0x02000239 RID: 569
	[Serializable]
	private struct AudioTarget
	{
		// Token: 0x04001248 RID: 4680
		public AudioSource audioSource;

		// Token: 0x04001249 RID: 4681
		public AnimationCurve pitchCurve;

		// Token: 0x0400124A RID: 4682
		public AnimationCurve volumeCurve;

		// Token: 0x0400124B RID: 4683
		[NonSerialized]
		public float baseVolume;

		// Token: 0x0400124C RID: 4684
		public float riseSmoothing;

		// Token: 0x0400124D RID: 4685
		public float lowerSmoothing;
	}
}
