using System;
using UnityEngine;

public class AudioAnimator : MonoBehaviour
{
	private void Start()
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
	}

	private void InitBaseVolume()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			this.targets[i].baseVolume = this.targets[i].audioSource.volume;
		}
		this.didInitBaseVolume = true;
	}

	public void UpdateValue(float value, bool ignoreSmoothing = false)
	{
		this.UpdatePitchAndVolume(value, value, ignoreSmoothing);
	}

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

	private bool didInitBaseVolume;

	[SerializeField]
	private AudioAnimator.AudioTarget[] targets;

	[Serializable]
	private struct AudioTarget
	{
		public AudioSource audioSource;

		public AnimationCurve pitchCurve;

		public AnimationCurve volumeCurve;

		[NonSerialized]
		public float baseVolume;

		public float riseSmoothing;

		public float lowerSmoothing;
	}
}
