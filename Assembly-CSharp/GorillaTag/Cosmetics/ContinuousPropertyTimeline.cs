using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010D5 RID: 4309
	public class ContinuousPropertyTimeline : MonoBehaviour, ITickSystemTick, ISpawnable
	{
		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06006BFC RID: 27644 RVA: 0x002370DA File Offset: 0x002352DA
		// (set) Token: 0x06006BFD RID: 27645 RVA: 0x002370E5 File Offset: 0x002352E5
		private bool IsBackward
		{
			get
			{
				return !this.IsForward;
			}
			set
			{
				this.IsForward = !value;
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06006BFE RID: 27646 RVA: 0x002370F1 File Offset: 0x002352F1
		// (set) Token: 0x06006BFF RID: 27647 RVA: 0x002370FC File Offset: 0x002352FC
		private bool IsPaused
		{
			get
			{
				return !this.IsPlaying;
			}
			set
			{
				this.IsPlaying = !value;
			}
		}

		// Token: 0x06006C00 RID: 27648 RVA: 0x00237108 File Offset: 0x00235308
		public void TimelinePlay()
		{
			this.IsPlaying = true;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06006C01 RID: 27649 RVA: 0x00237117 File Offset: 0x00235317
		public void TimelinePause()
		{
			this.IsPaused = true;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006C02 RID: 27650 RVA: 0x00237126 File Offset: 0x00235326
		public void TimelineToggleDirection()
		{
			this.IsForward = !this.IsForward;
		}

		// Token: 0x06006C03 RID: 27651 RVA: 0x00237137 File Offset: 0x00235337
		public void TimelineTogglePlay()
		{
			if (this.IsPlaying)
			{
				this.TimelinePause();
				return;
			}
			this.TimelinePlay();
		}

		// Token: 0x06006C04 RID: 27652 RVA: 0x0023714E File Offset: 0x0023534E
		public void TimelinePlayForward()
		{
			this.IsForward = true;
			this.TimelinePlay();
		}

		// Token: 0x06006C05 RID: 27653 RVA: 0x0023715D File Offset: 0x0023535D
		public void TimelinePlayBackward()
		{
			this.IsBackward = true;
			this.TimelinePlay();
		}

		// Token: 0x06006C06 RID: 27654 RVA: 0x0023716C File Offset: 0x0023536C
		public void TimelinePlayFromBeginning()
		{
			this.time = 0f;
			this.TimelinePlayForward();
			this.OnReachedBeginning();
		}

		// Token: 0x06006C07 RID: 27655 RVA: 0x00237185 File Offset: 0x00235385
		public void TimelinePlayFromEnd()
		{
			this.time = this.durationSeconds;
			this.TimelinePlayBackward();
			this.OnReachedEnd();
		}

		// Token: 0x06006C08 RID: 27656 RVA: 0x0023719F File Offset: 0x0023539F
		public void TimelineScrubToTime(float t)
		{
			if (t <= 0f)
			{
				this.time = 0f;
				this.OnReachedBeginning();
				return;
			}
			if (t >= this.durationSeconds)
			{
				this.time = this.durationSeconds;
				this.OnReachedEnd();
				return;
			}
			this.time = t;
		}

		// Token: 0x06006C09 RID: 27657 RVA: 0x002371DE File Offset: 0x002353DE
		public void TimelineScrubToFraction(float f)
		{
			this.TimelineScrubToTime(f * this.durationSeconds);
		}

		// Token: 0x06006C0A RID: 27658 RVA: 0x002371EE File Offset: 0x002353EE
		public void TimelineSetDuration(float d)
		{
			this.durationSeconds = d;
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x06006C0B RID: 27659 RVA: 0x0023721C File Offset: 0x0023541C
		public void TimelineSetBackwardDuration(float d)
		{
			this.separateBackwardDuration = true;
			this.backwardDuration = d;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x06006C0C RID: 27660 RVA: 0x0023723F File Offset: 0x0023543F
		private void Awake()
		{
			this.IsPlaying = this.startPlaying;
		}

		// Token: 0x06006C0D RID: 27661 RVA: 0x00237250 File Offset: 0x00235450
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnEnable, this.myRig != null && this.myRig.isLocal);
			if (this.IsPlaying)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006C0E RID: 27662 RVA: 0x002372D2 File Offset: 0x002354D2
		private void OnDisable()
		{
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnDisable, this.myRig != null && this.myRig.isLocal);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006C0F RID: 27663 RVA: 0x00237304 File Offset: 0x00235504
		private void OnReachedEnd()
		{
			if (this.IsForward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = this.durationSeconds;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromBeginning();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsBackward = true;
					this.time = this.durationSeconds;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(1f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedEnd, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x06006C10 RID: 27664 RVA: 0x002373C4 File Offset: 0x002355C4
		private void OnReachedBeginning()
		{
			if (this.IsBackward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = 0f;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromEnd();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsForward = true;
					this.time = 0f;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(0f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedBeginning, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x06006C11 RID: 27665 RVA: 0x00237480 File Offset: 0x00235680
		private void InBetween()
		{
			float f = this.time * this.inverseDuration;
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(f);
		}

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06006C12 RID: 27666 RVA: 0x002374CE File Offset: 0x002356CE
		// (set) Token: 0x06006C13 RID: 27667 RVA: 0x002374D6 File Offset: 0x002356D6
		public bool TickRunning { get; set; }

		// Token: 0x06006C14 RID: 27668 RVA: 0x002374E0 File Offset: 0x002356E0
		public void Tick()
		{
			if (this.IsForward)
			{
				this.time += Time.deltaTime;
				if (this.time >= this.durationSeconds)
				{
					this.OnReachedEnd();
					return;
				}
				this.InBetween();
				return;
			}
			else
			{
				this.time -= Time.deltaTime * this.backwardDeltaMult;
				if (this.time <= 0f)
				{
					this.OnReachedBeginning();
					return;
				}
				this.InBetween();
				return;
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06006C15 RID: 27669 RVA: 0x00237556 File Offset: 0x00235756
		// (set) Token: 0x06006C16 RID: 27670 RVA: 0x0023755E File Offset: 0x0023575E
		public bool IsSpawned { get; set; }

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06006C17 RID: 27671 RVA: 0x00237567 File Offset: 0x00235767
		// (set) Token: 0x06006C18 RID: 27672 RVA: 0x0023756F File Offset: 0x0023576F
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006C19 RID: 27673 RVA: 0x00237578 File Offset: 0x00235778
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06006C1A RID: 27674 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x04007C9C RID: 31900
		[SerializeField]
		private float durationSeconds = 1f;

		// Token: 0x04007C9D RID: 31901
		[SerializeField]
		private float backwardDuration = 1f;

		// Token: 0x04007C9E RID: 31902
		[Tooltip("If true, the the timeline can move at a different speed when playing backwards.")]
		[SerializeField]
		private bool separateBackwardDuration;

		// Token: 0x04007C9F RID: 31903
		[Tooltip("When this object is enabled for the first time, should it immediately start playing from the beginning?")]
		[SerializeField]
		private bool startPlaying;

		// Token: 0x04007CA0 RID: 31904
		[Tooltip("Determine what happens when the timeline reaches the end (or beginning while playing backwards).")]
		[SerializeField]
		private ContinuousPropertyTimeline.TimelineEndBehavior endBehavior;

		// Token: 0x04007CA1 RID: 31905
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04007CA2 RID: 31906
		[SerializeField]
		private FlagEvents<ContinuousPropertyTimeline.TimelineEvent> events;

		// Token: 0x04007CA3 RID: 31907
		private float time;

		// Token: 0x04007CA4 RID: 31908
		private float inverseDuration;

		// Token: 0x04007CA5 RID: 31909
		private float backwardDeltaMult;

		// Token: 0x04007CA6 RID: 31910
		private bool IsForward = true;

		// Token: 0x04007CA7 RID: 31911
		private bool IsPlaying;

		// Token: 0x04007CA9 RID: 31913
		private VRRig myRig;

		// Token: 0x020010D6 RID: 4310
		private enum TimelineEndBehavior
		{
			// Token: 0x04007CAD RID: 31917
			Stop,
			// Token: 0x04007CAE RID: 31918
			Loop,
			// Token: 0x04007CAF RID: 31919
			PingPong
		}

		// Token: 0x020010D7 RID: 4311
		[Flags]
		private enum TimelineEvent
		{
			// Token: 0x04007CB1 RID: 31921
			OnReachedEnd = 1,
			// Token: 0x04007CB2 RID: 31922
			OnReachedBeginning = 2,
			// Token: 0x04007CB3 RID: 31923
			OnEnable = 4,
			// Token: 0x04007CB4 RID: 31924
			OnDisable = 8
		}
	}
}
