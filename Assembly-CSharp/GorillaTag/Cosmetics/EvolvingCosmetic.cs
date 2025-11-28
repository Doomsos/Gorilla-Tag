using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010EB RID: 4331
	public class EvolvingCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06006C94 RID: 27796 RVA: 0x0023A9F9 File Offset: 0x00238BF9
		private int LoopMaxValue
		{
			get
			{
				return this.stages.Length;
			}
		}

		// Token: 0x06006C95 RID: 27797 RVA: 0x0023AA04 File Offset: 0x00238C04
		private void Awake()
		{
			base.gameObject.GetOrAddComponent(ref this.networkEvents);
			this.myRig = base.GetComponentInParent<VRRig>();
			for (int i = 0; i < this.stages.Length; i++)
			{
				this.totalDuration += this.stages[i].Duration;
				if (this.enableLooping)
				{
					if (i < this.loopToStageOnComplete - 1)
					{
						this.timeAtLoopStart += this.stages[i].Duration;
					}
					else
					{
						this.loopDuration += this.stages[i].Duration;
					}
				}
			}
		}

		// Token: 0x06006C96 RID: 27798 RVA: 0x0023AAA8 File Offset: 0x00238CA8
		private void OnEnable()
		{
			if (this.stages.Length == 0)
			{
				return;
			}
			NetPlayer netPlayer = this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this.networkEvents.Init(netPlayer);
				TickSystem<object>.AddTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.SendElapsedTime);
				this.networkEvents.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ReceiveElapsedTime);
				this.FirstStage();
				return;
			}
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}

		// Token: 0x06006C97 RID: 27799 RVA: 0x0023AB44 File Offset: 0x00238D44
		private void OnDisable()
		{
			if (this.networkEvents != null)
			{
				TickSystem<object>.RemoveTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(this.SendElapsedTime);
				this.networkEvents.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ReceiveElapsedTime);
				this.FirstStage();
			}
			CallLimiter callLimiter = this.callLimiter;
			if (callLimiter == null)
			{
				return;
			}
			callLimiter.Reset();
		}

		// Token: 0x06006C98 RID: 27800 RVA: 0x00002789 File Offset: 0x00000989
		private void Log(bool isComplete, bool isEvent)
		{
		}

		// Token: 0x06006C99 RID: 27801 RVA: 0x0023ABC0 File Offset: 0x00238DC0
		private void FirstStage()
		{
			this.activeStageIndex = 0;
			this.activeStage = this.stages[0];
			this.nextEventIndex = 0;
			this.nextEvent = this.activeStage.GetEventOrNull(0);
			this.totalElapsedTime = 0f;
			this.totalTimeOfPreviousStages = 0f;
			this.HandleStages();
		}

		// Token: 0x06006C9A RID: 27802 RVA: 0x0023AC18 File Offset: 0x00238E18
		private void HandleStages()
		{
			for (;;)
			{
				float num = this.totalElapsedTime - this.totalTimeOfPreviousStages;
				float f = Mathf.Min(num / this.activeStage.Duration, 1f);
				this.activeStage.continuousProperties.ApplyAll(f);
				while (this.nextEvent != null && num >= this.nextEvent.absoluteTime)
				{
					UnityEvent onTimeReached = this.nextEvent.onTimeReached;
					if (onTimeReached != null)
					{
						onTimeReached.Invoke();
					}
					this.Log(false, true);
					EvolvingCosmetic.EvolutionStage evolutionStage = this.activeStage;
					int index = this.nextEventIndex + 1;
					this.nextEventIndex = index;
					this.nextEvent = evolutionStage.GetEventOrNull(index);
				}
				if (num < this.activeStage.Duration)
				{
					break;
				}
				this.activeStageIndex++;
				if (this.activeStageIndex >= this.stages.Length && !this.enableLooping)
				{
					goto Block_4;
				}
				if (this.activeStageIndex >= this.stages.Length)
				{
					this.activeStageIndex = this.loopToStageOnComplete - 1;
					this.totalTimeOfPreviousStages = this.timeAtLoopStart;
					this.totalElapsedTime -= this.loopDuration;
				}
				else
				{
					this.totalTimeOfPreviousStages += this.activeStage.Duration;
				}
				this.activeStage = this.stages[this.activeStageIndex];
				this.nextEventIndex = 0;
				this.nextEvent = this.activeStage.GetEventOrNull(0);
				if (!this.activeStage.HasDuration)
				{
					this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration * 0.5f;
					TickSystem<object>.RemoveTickCallback(this);
				}
				else
				{
					TickSystem<object>.AddTickCallback(this);
				}
				this.Log(false, false);
			}
			return;
			Block_4:
			this.totalElapsedTime = this.totalDuration;
			TickSystem<object>.RemoveTickCallback(this);
			this.Log(true, false);
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06006C9B RID: 27803 RVA: 0x0023ADCC File Offset: 0x00238FCC
		// (set) Token: 0x06006C9C RID: 27804 RVA: 0x0023ADD4 File Offset: 0x00238FD4
		public bool TickRunning { get; set; }

		// Token: 0x06006C9D RID: 27805 RVA: 0x0023ADE0 File Offset: 0x00238FE0
		public void Tick()
		{
			this.totalElapsedTime = Mathf.Clamp(this.totalElapsedTime + Mathf.Max(this.activeStage.DeltaTime(Time.deltaTime), 0f), 0f, this.totalDuration * 1.01f);
			this.HandleStages();
		}

		// Token: 0x06006C9E RID: 27806 RVA: 0x0023AE30 File Offset: 0x00239030
		public void CompleteManualStage()
		{
			if (!this.activeStage.HasDuration)
			{
				this.ForceNextStage();
			}
		}

		// Token: 0x06006C9F RID: 27807 RVA: 0x0023AE45 File Offset: 0x00239045
		public void ForceNextStage()
		{
			this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration;
			this.HandleStages();
		}

		// Token: 0x06006CA0 RID: 27808 RVA: 0x0023AE65 File Offset: 0x00239065
		private void SendElapsedTime(NetPlayer player)
		{
			if (this.sendProgressDelayCoroutine != null)
			{
				base.StopCoroutine(this.sendProgressDelayCoroutine);
			}
			this.sendProgressDelayCoroutine = base.StartCoroutine(this.SendElapsedTimeDelayed());
		}

		// Token: 0x06006CA1 RID: 27809 RVA: 0x0023AE8D File Offset: 0x0023908D
		private IEnumerator SendElapsedTimeDelayed()
		{
			yield return new WaitForSeconds(1f);
			this.sendProgressDelayCoroutine = null;
			this.networkEvents.Activate.RaiseOthers(new object[]
			{
				this.totalElapsedTime
			});
			yield break;
		}

		// Token: 0x06006CA2 RID: 27810 RVA: 0x0023AE9C File Offset: 0x0023909C
		private void ReceiveElapsedTime(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "ReceiveElapsedTime");
			if (info.senderID == this.myRig.creator.ActorNumber && this.callLimiter.CheckCallServerTime((double)Time.unscaledTime) && args.Length == 1)
			{
				object obj = args[0];
				if (obj is float)
				{
					float num = (float)obj;
					if (float.IsFinite(num) && num <= this.totalDuration && num >= 0f)
					{
						this.totalElapsedTime = num;
						this.HandleStages();
						return;
					}
				}
			}
		}

		// Token: 0x04007D7F RID: 32127
		[SerializeField]
		private bool enableLooping;

		// Token: 0x04007D80 RID: 32128
		[SerializeField]
		private int loopToStageOnComplete = 1;

		// Token: 0x04007D81 RID: 32129
		[SerializeField]
		private EvolvingCosmetic.EvolutionStage[] stages;

		// Token: 0x04007D82 RID: 32130
		private RubberDuckEvents networkEvents;

		// Token: 0x04007D83 RID: 32131
		private VRRig myRig;

		// Token: 0x04007D84 RID: 32132
		private CallLimiter callLimiter = new CallLimiter(5, 10f, 0.5f);

		// Token: 0x04007D85 RID: 32133
		private int activeStageIndex;

		// Token: 0x04007D86 RID: 32134
		private EvolvingCosmetic.EvolutionStage activeStage;

		// Token: 0x04007D87 RID: 32135
		private int nextEventIndex;

		// Token: 0x04007D88 RID: 32136
		private EvolvingCosmetic.EvolutionStage.EventAtTime nextEvent;

		// Token: 0x04007D89 RID: 32137
		private float totalElapsedTime;

		// Token: 0x04007D8A RID: 32138
		private float totalTimeOfPreviousStages;

		// Token: 0x04007D8B RID: 32139
		private float totalDuration;

		// Token: 0x04007D8C RID: 32140
		private float timeAtLoopStart;

		// Token: 0x04007D8D RID: 32141
		private float loopDuration;

		// Token: 0x04007D8E RID: 32142
		private Coroutine sendProgressDelayCoroutine;

		// Token: 0x020010EC RID: 4332
		[Serializable]
		private class EvolutionStage
		{
			// Token: 0x06006CA4 RID: 27812 RVA: 0x0023AF4B File Offset: 0x0023914B
			private bool HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags flag)
			{
				return (this.progressionFlags & flag) > EvolvingCosmetic.EvolutionStage.ProgressionFlags.None;
			}

			// Token: 0x17000A4E RID: 2638
			// (get) Token: 0x06006CA5 RID: 27813 RVA: 0x0023AF58 File Offset: 0x00239158
			public bool HasDuration
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time | EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000A4F RID: 2639
			// (get) Token: 0x06006CA6 RID: 27814 RVA: 0x0023AF61 File Offset: 0x00239161
			public bool HasTime
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time);
				}
			}

			// Token: 0x17000A50 RID: 2640
			// (get) Token: 0x06006CA7 RID: 27815 RVA: 0x0023AF6A File Offset: 0x0023916A
			public bool HasTemperature
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000A51 RID: 2641
			// (get) Token: 0x06006CA8 RID: 27816 RVA: 0x0023AF73 File Offset: 0x00239173
			public float Duration
			{
				get
				{
					if (!this.HasDuration)
					{
						return 1f;
					}
					return this.durationSeconds;
				}
			}

			// Token: 0x06006CA9 RID: 27817 RVA: 0x0023AF89 File Offset: 0x00239189
			public float DeltaTime(float deltaTime)
			{
				return (this.HasTime ? deltaTime : 0f) + (this.HasTemperature ? (deltaTime * this.celsiusSpeedupMult.Evaluate(this.thermalReceiver.celsius)) : 0f);
			}

			// Token: 0x06006CAA RID: 27818 RVA: 0x0023AFC3 File Offset: 0x002391C3
			public EvolvingCosmetic.EvolutionStage.EventAtTime GetEventOrNull(int index)
			{
				if (this.events == null || index < 0 || index >= this.events.Length)
				{
					return null;
				}
				return this.events[index];
			}

			// Token: 0x04007D90 RID: 32144
			private const float MIN_STAGE_TIME = 0.01f;

			// Token: 0x04007D91 RID: 32145
			public string debugName;

			// Token: 0x04007D92 RID: 32146
			public EvolvingCosmetic.EvolutionStage.ProgressionFlags progressionFlags = EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time;

			// Token: 0x04007D93 RID: 32147
			[SerializeField]
			private float durationSeconds = float.NaN;

			// Token: 0x04007D94 RID: 32148
			public ThermalReceiver thermalReceiver;

			// Token: 0x04007D95 RID: 32149
			public AnimationCurve celsiusSpeedupMult = AnimationCurve.Linear(0f, 0f, 100f, 2f);

			// Token: 0x04007D96 RID: 32150
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x04007D97 RID: 32151
			[SerializeField]
			private EvolvingCosmetic.EvolutionStage.EventAtTime[] events;

			// Token: 0x020010ED RID: 4333
			[Flags]
			public enum ProgressionFlags
			{
				// Token: 0x04007D99 RID: 32153
				None = 0,
				// Token: 0x04007D9A RID: 32154
				Time = 1,
				// Token: 0x04007D9B RID: 32155
				Temperature = 2
			}

			// Token: 0x020010EE RID: 4334
			[Serializable]
			public class EventAtTime : IComparable<EvolvingCosmetic.EvolutionStage.EventAtTime>
			{
				// Token: 0x17000A52 RID: 2642
				// (get) Token: 0x06006CAC RID: 27820 RVA: 0x0023B01F File Offset: 0x0023921F
				private string DynamicTimeLabel
				{
					get
					{
						if (this.type != EvolvingCosmetic.EvolutionStage.EventAtTime.Type.DurationFraction)
						{
							return "Time";
						}
						return "Fraction";
					}
				}

				// Token: 0x06006CAD RID: 27821 RVA: 0x0023B035 File Offset: 0x00239235
				public int CompareTo(EvolvingCosmetic.EvolutionStage.EventAtTime other)
				{
					return this.absoluteTime.CompareTo(other.absoluteTime);
				}

				// Token: 0x04007D9C RID: 32156
				public string debugName;

				// Token: 0x04007D9D RID: 32157
				public float time;

				// Token: 0x04007D9E RID: 32158
				public EvolvingCosmetic.EvolutionStage.EventAtTime.Type type;

				// Token: 0x04007D9F RID: 32159
				public float absoluteTime;

				// Token: 0x04007DA0 RID: 32160
				public UnityEvent onTimeReached;

				// Token: 0x020010EF RID: 4335
				public enum Type
				{
					// Token: 0x04007DA2 RID: 32162
					SecondsFromBeginning,
					// Token: 0x04007DA3 RID: 32163
					SecondsBeforeEnd,
					// Token: 0x04007DA4 RID: 32164
					DurationFraction
				}
			}
		}
	}
}
