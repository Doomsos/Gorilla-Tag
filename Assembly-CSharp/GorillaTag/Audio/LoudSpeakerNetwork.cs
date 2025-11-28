using System;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02001074 RID: 4212
	public class LoudSpeakerNetwork : MonoBehaviour
	{
		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x060069B7 RID: 27063 RVA: 0x002263F8 File Offset: 0x002245F8
		public AudioSource[] SpeakerSources
		{
			get
			{
				return this._speakerSources;
			}
		}

		// Token: 0x060069B8 RID: 27064 RVA: 0x00226400 File Offset: 0x00224600
		private void Awake()
		{
			if (this._speakerSources == null || this._speakerSources.Length == 0)
			{
				this._speakerSources = base.transform.GetComponentsInChildren<AudioSource>();
			}
			this._currentSpeakers = new List<Speaker>();
		}

		// Token: 0x060069B9 RID: 27065 RVA: 0x00226430 File Offset: 0x00224630
		private void Start()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer) && rigContainer.Voice != null)
			{
				GTSpeaker gtspeaker = (GTSpeaker)rigContainer.Voice.SpeakerInUse;
				if (gtspeaker != null)
				{
					gtspeaker.AddExternalAudioSources(this._speakerSources);
				}
			}
		}

		// Token: 0x060069BA RID: 27066 RVA: 0x0022647B File Offset: 0x0022467B
		private bool GetParentRigContainer(out RigContainer rigContainer)
		{
			if (this._rigContainer == null)
			{
				this._rigContainer = base.transform.GetComponentInParent<RigContainer>();
			}
			rigContainer = this._rigContainer;
			return rigContainer != null;
		}

		// Token: 0x060069BB RID: 27067 RVA: 0x002264AC File Offset: 0x002246AC
		private void OnEnable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.AddLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x060069BC RID: 27068 RVA: 0x002264CC File Offset: 0x002246CC
		private void OnDisable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.RemoveLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x060069BD RID: 27069 RVA: 0x002264EA File Offset: 0x002246EA
		public void AddSpeaker(Speaker speaker)
		{
			if (this._currentSpeakers.Contains(speaker))
			{
				return;
			}
			this._currentSpeakers.Add(speaker);
		}

		// Token: 0x060069BE RID: 27070 RVA: 0x00226507 File Offset: 0x00224707
		public void RemoveSpeaker(Speaker speaker)
		{
			this._currentSpeakers.Remove(speaker);
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x00226516 File Offset: 0x00224716
		public void StartBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(true, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x060069C0 RID: 27072 RVA: 0x00226534 File Offset: 0x00224734
		public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = true;
					if (this.ReparentLocalSpeaker)
					{
						Transform transform = this._rigContainer.Voice.SpeakerInUse.transform;
						transform.transform.SetParent(base.transform, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
					gtspeaker.ToggleAudioSource(true);
					gtspeaker.BroadcastExternal = true;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
					{
						Transform transform2 = rigContainer.Voice.SpeakerInUse.transform;
						transform2.SetParent(base.transform, false);
						transform2.localPosition = Vector3.zero;
					}
				}
			}
			this._currentSpeakerActor = actorNumber;
		}

		// Token: 0x060069C1 RID: 27073 RVA: 0x00226654 File Offset: 0x00224854
		public void StopBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(false, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x060069C2 RID: 27074 RVA: 0x00226674 File Offset: 0x00224874
		public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = false;
					RigContainer rigContainer;
					if (this.ReparentLocalSpeaker && this.GetParentRigContainer(out rigContainer))
					{
						Transform transform = rigContainer.Voice.SpeakerInUse.transform;
						transform.SetParent(rigContainer.SpeakerHead, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			if (actorNumber == this._currentSpeakerActor)
			{
				using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
						gtspeaker.ToggleAudioSource(false);
						gtspeaker.BroadcastExternal = false;
						RigContainer rigContainer2;
						if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer2))
						{
							Transform transform2 = rigContainer2.Voice.SpeakerInUse.transform;
							transform2.SetParent(rigContainer2.SpeakerHead, false);
							transform2.localPosition = Vector3.zero;
						}
					}
				}
				this._currentSpeakerActor = -1;
			}
		}

		// Token: 0x04007905 RID: 30981
		[SerializeField]
		private AudioSource[] _speakerSources;

		// Token: 0x04007906 RID: 30982
		[SerializeField]
		private List<Speaker> _currentSpeakers;

		// Token: 0x04007907 RID: 30983
		[SerializeField]
		private int _currentSpeakerActor = -1;

		// Token: 0x04007908 RID: 30984
		public bool ReparentLocalSpeaker = true;

		// Token: 0x04007909 RID: 30985
		private RigContainer _rigContainer;

		// Token: 0x0400790A RID: 30986
		private GTRecorder _localRecorder;
	}
}
