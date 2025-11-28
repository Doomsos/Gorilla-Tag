using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001125 RID: 4389
	[RequireComponent(typeof(LoudSpeakerActivator))]
	public class VoiceBroadcastCosmetic : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006DCC RID: 28108 RVA: 0x00240DEE File Offset: 0x0023EFEE
		private void Awake()
		{
			this.loudSpeaker = base.GetComponent<LoudSpeakerActivator>();
			this.animator = base.GetComponent<Animator>();
			this.talkAnimationTrigger = Animator.StringToHash(this.talkAnimationTriggerName);
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>();
		}

		// Token: 0x06006DCD RID: 28109 RVA: 0x00240E25 File Offset: 0x0023F025
		public void SetWearable(VoiceBroadcastCosmeticWearable wearable)
		{
			this.wearable = wearable;
		}

		// Token: 0x06006DCE RID: 28110 RVA: 0x00240E2E File Offset: 0x0023F02E
		private void StartBroadcast()
		{
			this.loudSpeaker.StartLocalBroadcast();
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.wearable.OnCosmeticStartListening();
			this.lastSliceUpdateTime = Time.time;
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06006DCF RID: 28111 RVA: 0x00240E69 File Offset: 0x0023F069
		private void StopBroadcast()
		{
			this.loudSpeaker.StopLocalBroadcast();
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.wearable.OnCosmeticStopListening();
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06006DD0 RID: 28112 RVA: 0x00240E99 File Offset: 0x0023F099
		public void OnEnable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
		}

		// Token: 0x06006DD1 RID: 28113 RVA: 0x00240EAD File Offset: 0x0023F0AD
		public void OnDisable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
			this.StopBroadcast();
		}

		// Token: 0x06006DD2 RID: 28114 RVA: 0x00240EC8 File Offset: 0x0023F0C8
		public void SetListenState(bool listening)
		{
			if (this.isListening == listening || !base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.isListening = listening;
			this.speakingTime = 0f;
			if (listening)
			{
				this.StartBroadcast();
				return;
			}
			this.StopBroadcast();
		}

		// Token: 0x06006DD3 RID: 28115 RVA: 0x00240F18 File Offset: 0x0023F118
		public void SliceUpdate()
		{
			float num = Time.time - this.lastSliceUpdateTime;
			this.lastSliceUpdateTime = Time.time;
			if (this.gsl != null && this.gsl.IsSpeaking && this.gsl.LoudnessNormalized >= this.minVolume)
			{
				this.speakingTime += num;
				if (this.speakingTime >= this.minSpeakingTime)
				{
					if (this.animator != null)
					{
						this.animator.SetTrigger(this.talkAnimationTrigger);
					}
					if (this.simpleAnimation != null && !this.simpleAnimation.isPlaying)
					{
						this.simpleAnimation.Play();
					}
					if (!this.isSpeaking)
					{
						UnityEvent unityEvent = this.onStartSpeaking;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
						this.isSpeaking = true;
						return;
					}
				}
			}
			else
			{
				this.speakingTime = 0f;
				if (this.isSpeaking)
				{
					UnityEvent unityEvent2 = this.onStopSpeaking;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.isSpeaking = false;
				}
			}
		}

		// Token: 0x06006DD4 RID: 28116 RVA: 0x00241025 File Offset: 0x0023F225
		private void ResetToFirstFrame()
		{
			this.simpleAnimation.Rewind();
			this.simpleAnimation.Play();
			this.simpleAnimation.Sample();
			this.simpleAnimation.Stop();
		}

		// Token: 0x04007F64 RID: 32612
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04007F65 RID: 32613
		[Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
		[SerializeField]
		public float minVolume = 0.1f;

		// Token: 0x04007F66 RID: 32614
		[Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
		[SerializeField]
		public float minSpeakingTime = 0.15f;

		// Token: 0x04007F67 RID: 32615
		[SerializeField]
		private Animation simpleAnimation;

		// Token: 0x04007F68 RID: 32616
		[SerializeField]
		private string talkAnimationTriggerName;

		// Token: 0x04007F69 RID: 32617
		private int talkAnimationTrigger;

		// Token: 0x04007F6A RID: 32618
		private const string EVENTS = "Events";

		// Token: 0x04007F6B RID: 32619
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04007F6C RID: 32620
		[SerializeField]
		private UnityEvent onStartSpeaking;

		// Token: 0x04007F6D RID: 32621
		[SerializeField]
		private UnityEvent onStopSpeaking;

		// Token: 0x04007F6E RID: 32622
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04007F6F RID: 32623
		private float speakingTime;

		// Token: 0x04007F70 RID: 32624
		private bool isListening;

		// Token: 0x04007F71 RID: 32625
		private bool isSpeaking;

		// Token: 0x04007F72 RID: 32626
		private VoiceBroadcastCosmeticWearable wearable;

		// Token: 0x04007F73 RID: 32627
		private LoudSpeakerActivator loudSpeaker;

		// Token: 0x04007F74 RID: 32628
		private GorillaSpeakerLoudness gsl;

		// Token: 0x04007F75 RID: 32629
		private Animator animator;

		// Token: 0x04007F76 RID: 32630
		private float lastSliceUpdateTime;
	}
}
