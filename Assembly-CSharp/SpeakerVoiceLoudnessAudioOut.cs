using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class SpeakerVoiceLoudnessAudioOut : UnityAudioOut
{
	// Token: 0x06001350 RID: 4944 RVA: 0x0006FC28 File Offset: 0x0006DE28
	public SpeakerVoiceLoudnessAudioOut(SpeakerVoiceToLoudness speaker, AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo) : base(audioSource, playDelayConfig, logger, logPrefix, debugInfo)
	{
		this.voiceToLoudness = speaker;
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0006FC40 File Offset: 0x0006DE40
	public override void OutWrite(float[] data, int offsetSamples)
	{
		float num = 0f;
		for (int i = 0; i < data.Length; i++)
		{
			float num2 = data[i];
			if (!float.IsFinite(num2))
			{
				num2 = 0f;
				data[i] = num2;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
				data[i] = num2;
			}
			else if (num2 < -1f)
			{
				num2 = -1f;
				data[i] = num2;
			}
			num += Mathf.Abs(num2);
		}
		if (num > 0f)
		{
			this.voiceToLoudness.loudness = num / (float)data.Length;
		}
		base.OutWrite(data, offsetSamples);
	}

	// Token: 0x04001CD6 RID: 7382
	private SpeakerVoiceToLoudness voiceToLoudness;
}
