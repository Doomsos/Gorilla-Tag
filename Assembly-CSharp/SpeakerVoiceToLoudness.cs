using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x02000319 RID: 793
[RequireComponent(typeof(Speaker))]
public class SpeakerVoiceToLoudness : MonoBehaviour
{
	// Token: 0x0600134B RID: 4939 RVA: 0x0006FB14 File Offset: 0x0006DD14
	private void Awake()
	{
		Speaker component = base.GetComponent<Speaker>();
		component.CustomAudioOutFactory = this.GetVolumeTracking(component);
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x0006FB38 File Offset: 0x0006DD38
	private Func<IAudioOut<float>> GetVolumeTracking(Speaker speaker)
	{
		AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
		{
			Low = this.playbackDelaySettings.MinDelaySoft,
			High = this.playbackDelaySettings.MaxDelaySoft,
			Max = this.playbackDelaySettings.MaxDelayHard
		};
		return () => new SpeakerVoiceLoudnessAudioOut(this, speaker.GetComponent<AudioSource>(), pdc, speaker.Logger, string.Empty, speaker.Logger.IsDebugEnabled);
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x0006FBA4 File Offset: 0x0006DDA4
	public SpeakerVoiceToLoudness()
	{
		PlaybackDelaySettings playbackDelaySettings = default(PlaybackDelaySettings);
		playbackDelaySettings.MinDelaySoft = 200;
		playbackDelaySettings.MaxDelaySoft = 400;
		playbackDelaySettings.MaxDelayHard = 1000;
		this.playbackDelaySettings = playbackDelaySettings;
		base..ctor();
	}

	// Token: 0x04001CD1 RID: 7377
	[SerializeField]
	private PlaybackDelaySettings playbackDelaySettings;

	// Token: 0x04001CD2 RID: 7378
	public float loudness;
}
