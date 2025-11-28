using System;
using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EBA RID: 3770
	[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
	public class SO_NetworkVoiceSettings : ScriptableObject
	{
		// Token: 0x04006BDA RID: 27610
		[Header("Voice settings")]
		public bool AutoConnectAndJoin = true;

		// Token: 0x04006BDB RID: 27611
		public bool AutoLeaveAndDisconnect = true;

		// Token: 0x04006BDC RID: 27612
		public bool WorkInOfflineMode = true;

		// Token: 0x04006BDD RID: 27613
		public DebugLevel LogLevel = 1;

		// Token: 0x04006BDE RID: 27614
		public DebugLevel GlobalRecordersLogLevel = 3;

		// Token: 0x04006BDF RID: 27615
		public DebugLevel GlobalSpeakersLogLevel = 3;

		// Token: 0x04006BE0 RID: 27616
		public bool CreateSpeakerIfNotFound;

		// Token: 0x04006BE1 RID: 27617
		public int UpdateInterval = 50;

		// Token: 0x04006BE2 RID: 27618
		public bool SupportLogger;

		// Token: 0x04006BE3 RID: 27619
		public int BackgroundTimeout = 60000;

		// Token: 0x04006BE4 RID: 27620
		[Header("Recorder Settings")]
		public bool RecordOnlyWhenEnabled;

		// Token: 0x04006BE5 RID: 27621
		public bool RecordOnlyWhenJoined = true;

		// Token: 0x04006BE6 RID: 27622
		public bool StopRecordingWhenPaused;

		// Token: 0x04006BE7 RID: 27623
		public bool TransmitEnabled = true;

		// Token: 0x04006BE8 RID: 27624
		public bool AutoStart = true;

		// Token: 0x04006BE9 RID: 27625
		public bool Encrypt;

		// Token: 0x04006BEA RID: 27626
		public byte InterestGroup;

		// Token: 0x04006BEB RID: 27627
		public bool DebugEcho;

		// Token: 0x04006BEC RID: 27628
		public bool ReliableMode;

		// Token: 0x04006BED RID: 27629
		[Header("Recorder Codec Parameters")]
		public OpusCodec.FrameDuration FrameDuration = 60000;

		// Token: 0x04006BEE RID: 27630
		public SamplingRate SamplingRate = 16000;

		// Token: 0x04006BEF RID: 27631
		[Range(6000f, 510000f)]
		public int Bitrate = 20000;

		// Token: 0x04006BF0 RID: 27632
		[Header("Recorder Audio Source Settings")]
		public Recorder.InputSourceType InputSourceType;

		// Token: 0x04006BF1 RID: 27633
		public Recorder.MicType MicrophoneType;

		// Token: 0x04006BF2 RID: 27634
		public bool UseFallback = true;

		// Token: 0x04006BF3 RID: 27635
		public bool Detect = true;

		// Token: 0x04006BF4 RID: 27636
		[Range(0f, 1f)]
		public float Threshold = 0.07f;

		// Token: 0x04006BF5 RID: 27637
		public int Delay = 500;
	}
}
