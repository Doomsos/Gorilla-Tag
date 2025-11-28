using System;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
public class GorillaSpeakerLoudness : MonoBehaviour, IGorillaSliceableSimple, IDynamicFloat
{
	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x060033C0 RID: 13248 RVA: 0x00116C9D File Offset: 0x00114E9D
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x060033C1 RID: 13249 RVA: 0x00116CA5 File Offset: 0x00114EA5
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x060033C2 RID: 13250 RVA: 0x00116CAD File Offset: 0x00114EAD
	public float LoudnessNormalized
	{
		get
		{
			return Mathf.Min(this.loudness / this.normalizedMax, 1f);
		}
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x060033C3 RID: 13251 RVA: 0x00116CC6 File Offset: 0x00114EC6
	public float floatValue
	{
		get
		{
			return this.LoudnessNormalized;
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x060033C4 RID: 13252 RVA: 0x00116CCE File Offset: 0x00114ECE
	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	// Token: 0x1700049D RID: 1181
	// (get) Token: 0x060033C5 RID: 13253 RVA: 0x00116CD6 File Offset: 0x00114ED6
	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x00116CDE File Offset: 0x00114EDE
	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x00116D02 File Offset: 0x00114F02
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	// Token: 0x060033CA RID: 13258 RVA: 0x00116D34 File Offset: 0x00114F34
	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.permission = (this.permission || MicPermissionsManager.HasMicPermission());
			if (this.permission && !this.micConnected && Microphone.devices != null)
			{
				this.micConnected = (Microphone.devices.Length != 0);
			}
			this.isMicEnabled = (this.permission && this.micConnected);
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	// Token: 0x060033CB RID: 13259 RVA: 0x00116DD0 File Offset: 0x00114FD0
	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		if (this.recorder != null && this.offlineMic != null)
		{
			Microphone.End(UnityMicrophone.devices[0]);
			Object.Destroy(this.offlineMic);
			this.offlineMic = null;
			this.recorder.RestartRecording(true);
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig && this.recorder == null && this.isMicEnabled && !Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.offlineMic = Microphone.Start(UnityMicrophone.devices[0], true, 1, 16000);
		}
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else if (this.offlineMic != null && this.recorder == null && this.isMicEnabled && Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.isSpeaking = true;
			int num = Mathf.Min(Mathf.CeilToInt(this.deltaTime * 16000f), 16000);
			if (num > this.voiceSampleBuffer.Length)
			{
				Array.Resize<float>(ref this.voiceSampleBuffer, num);
			}
			if (this.offlineMic.samples >= num && this.offlineMic.GetData(this.voiceSampleBuffer, this.offlineMic.samples - num))
			{
				float num2 = 0f;
				for (int i = 0; i < this.voiceSampleBuffer.Length; i++)
				{
					num2 += Mathf.Abs(this.voiceSampleBuffer[i]);
				}
				this.loudness = num2 / (float)this.voiceSampleBuffer.Length;
				return;
			}
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x0011712C File Offset: 0x0011532C
	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
		this.timeSinceLoudnessChange += this.deltaTime;
	}

	// Token: 0x04004234 RID: 16948
	private bool isSpeaking;

	// Token: 0x04004235 RID: 16949
	private float loudness;

	// Token: 0x04004236 RID: 16950
	[SerializeField]
	private float normalizedMax = 0.175f;

	// Token: 0x04004237 RID: 16951
	private bool isMicEnabled;

	// Token: 0x04004238 RID: 16952
	private RigContainer rigContainer;

	// Token: 0x04004239 RID: 16953
	private Speaker speaker;

	// Token: 0x0400423A RID: 16954
	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	// Token: 0x0400423B RID: 16955
	private Recorder recorder;

	// Token: 0x0400423C RID: 16956
	private VoiceToLoudness voiceToLoudness;

	// Token: 0x0400423D RID: 16957
	private float smoothedLoudness;

	// Token: 0x0400423E RID: 16958
	private float lastLoudness;

	// Token: 0x0400423F RID: 16959
	private float timeSinceLoudnessChange;

	// Token: 0x04004240 RID: 16960
	private float loudnessUpdateCheckRate = 0.2f;

	// Token: 0x04004241 RID: 16961
	private float loudnessBlendStrength = 2f;

	// Token: 0x04004242 RID: 16962
	private bool permission;

	// Token: 0x04004243 RID: 16963
	private bool micConnected;

	// Token: 0x04004244 RID: 16964
	private float timeLastUpdated;

	// Token: 0x04004245 RID: 16965
	private float deltaTime;

	// Token: 0x04004246 RID: 16966
	private AudioClip offlineMic;

	// Token: 0x04004247 RID: 16967
	private float[] voiceSampleBuffer = new float[128];
}
