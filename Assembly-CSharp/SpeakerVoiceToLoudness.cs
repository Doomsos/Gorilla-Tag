using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

[RequireComponent(typeof(Speaker))]
public class SpeakerVoiceToLoudness : MonoBehaviour
{
	private void Awake()
	{
		Speaker component = base.GetComponent<Speaker>();
		component.CustomAudioOutFactory = this.GetVolumeTracking(component);
	}

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

	public SpeakerVoiceToLoudness()
	{
		PlaybackDelaySettings playbackDelaySettings = default(PlaybackDelaySettings);
		playbackDelaySettings.MinDelaySoft = 200;
		playbackDelaySettings.MaxDelaySoft = 400;
		playbackDelaySettings.MaxDelayHard = 1000;
		this.playbackDelaySettings = playbackDelaySettings;
		base..ctor();
	}

	[SerializeField]
	private PlaybackDelaySettings playbackDelaySettings;

	public float loudness;
}
