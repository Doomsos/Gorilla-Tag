using System;
using System.Collections.Generic;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Audio
{
	// Token: 0x02001071 RID: 4209
	public class GTSpeaker : Speaker
	{
		// Token: 0x060069A3 RID: 27043 RVA: 0x00225D90 File Offset: 0x00223F90
		public void Start()
		{
			LoudSpeakerNetwork componentInChildren = base.transform.root.GetComponentInChildren<LoudSpeakerNetwork>();
			if (componentInChildren != null)
			{
				this.AddExternalAudioSources(componentInChildren.SpeakerSources);
			}
		}

		// Token: 0x060069A4 RID: 27044 RVA: 0x00225DC3 File Offset: 0x00223FC3
		public void AddExternalAudioSources(AudioSource[] audioSources)
		{
			if (this._initializedExternalAudioSources)
			{
				return;
			}
			this._externalAudioSources = audioSources;
			this.InitializeExternalAudioSources();
			if (this._audioOutputStarted)
			{
				this.ExternalAudioOutputStart(this._frequency, this._channels, this._frameSamplesPerChannel);
			}
		}

		// Token: 0x060069A5 RID: 27045 RVA: 0x00225DFB File Offset: 0x00223FFB
		protected override void Initialize()
		{
			if (base.IsInitialized)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Already initialized.", Array.Empty<object>());
				}
				return;
			}
			base.Initialize();
		}

		// Token: 0x060069A6 RID: 27046 RVA: 0x00225E30 File Offset: 0x00224030
		private void InitializeExternalAudioSources()
		{
			this._initializedExternalAudioSources = true;
			this._externalAudioOutputs = new List<IAudioOut<float>>();
			AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig
			{
				Low = this.playbackDelaySettings.MinDelaySoft,
				High = this.playbackDelaySettings.MaxDelaySoft,
				Max = this.playbackDelaySettings.MaxDelayHard
			};
			foreach (AudioSource source in this._externalAudioSources)
			{
				this._externalAudioOutputs.Add(this.GetAudioOutFactoryFromSource(source, pdc).Invoke());
			}
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00225EB9 File Offset: 0x002240B9
		private Func<IAudioOut<float>> GetAudioOutFactoryFromSource(AudioSource source, AudioOutDelayControl.PlayDelayConfig pdc)
		{
			return () => new UnityAudioOut(source, pdc, this.Logger, string.Empty, this.Logger.IsDebugEnabled);
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x00225EE0 File Offset: 0x002240E0
		protected override void OnAudioFrame(FrameOut<float> frame)
		{
			base.OnAudioFrame(frame);
			if (this.BroadcastExternal)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Push(frame.Buf);
					if (frame.EndOfStream)
					{
						audioOut.Flush();
					}
				}
			}
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x00225F58 File Offset: 0x00224158
		protected override void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			this._audioOutputStarted = true;
			this._frequency = frequency;
			this._channels = channels;
			this._frameSamplesPerChannel = frameSamplesPerChannel;
			base.AudioOutputStart(frequency, channels, frameSamplesPerChannel);
			this.ExternalAudioOutputStart(frequency, channels, frameSamplesPerChannel);
		}

		// Token: 0x060069AA RID: 27050 RVA: 0x00225F88 File Offset: 0x00224188
		private void ExternalAudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
		{
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Start(frequency, channels, frameSamplesPerChannel);
						audioOut.ToggleAudioSource(false);
					}
				}
			}
		}

		// Token: 0x060069AB RID: 27051 RVA: 0x00225FF4 File Offset: 0x002241F4
		protected override void AudioOutputStop()
		{
			this._audioOutputStarted = false;
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					audioOut.Stop();
				}
			}
			base.AudioOutputStop();
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x0022605C File Offset: 0x0022425C
		protected override void AudioOutputService()
		{
			base.AudioOutputService();
			if (this._externalAudioOutputs != null)
			{
				foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
				{
					if (!audioOut.IsPlaying)
					{
						audioOut.Service();
					}
				}
			}
		}

		// Token: 0x060069AD RID: 27053 RVA: 0x002260C4 File Offset: 0x002242C4
		public void ToggleAudioSource(bool toggle)
		{
			if (this._externalAudioOutputs == null)
			{
				return;
			}
			foreach (IAudioOut<float> audioOut in this._externalAudioOutputs)
			{
				audioOut.ToggleAudioSource(toggle);
			}
		}

		// Token: 0x040078F3 RID: 30963
		[FormerlySerializedAs("UseExternalAudioSources")]
		public bool BroadcastExternal;

		// Token: 0x040078F4 RID: 30964
		[SerializeField]
		private AudioSource[] _externalAudioSources;

		// Token: 0x040078F5 RID: 30965
		private List<IAudioOut<float>> _externalAudioOutputs;

		// Token: 0x040078F6 RID: 30966
		private int _frequency;

		// Token: 0x040078F7 RID: 30967
		private int _channels;

		// Token: 0x040078F8 RID: 30968
		private int _frameSamplesPerChannel;

		// Token: 0x040078F9 RID: 30969
		private bool _initializedExternalAudioSources;

		// Token: 0x040078FA RID: 30970
		private bool _audioOutputStarted;
	}
}
