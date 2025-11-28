using System;
using GorillaTag.Cosmetics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000D96 RID: 3478
	public class GorillaIntervalTimer : MonoBehaviourPun
	{
		// Token: 0x0600556F RID: 21871 RVA: 0x001ADF33 File Offset: 0x001AC133
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
			this.ResetElapsed();
			this.ResetRun();
		}

		// Token: 0x06005570 RID: 21872 RVA: 0x001ADF5B File Offset: 0x001AC15B
		private void OnEnable()
		{
			if (this.runOnEnable)
			{
				if (!this.isRegistered)
				{
					GorillaIntervalTimerManager.RegisterGorillaTimer(this);
					this.isRegistered = true;
				}
				this.StartTimer();
			}
		}

		// Token: 0x06005571 RID: 21873 RVA: 0x001ADF80 File Offset: 0x001AC180
		private void OnDisable()
		{
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
			this.StopTimer();
		}

		// Token: 0x06005572 RID: 21874 RVA: 0x001ADFA0 File Offset: 0x001AC1A0
		public void StartTimer()
		{
			if (!this.isRegistered)
			{
				GorillaIntervalTimerManager.RegisterGorillaTimer(this);
				this.isRegistered = true;
			}
			this.ResetRun();
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			if (this.useInitialDelay && this.initialDelay > 0f)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.initialDelay));
			}
			else
			{
				this.RollNextInterval();
			}
			this.isRunning = true;
			this.isPaused = false;
			UnityEvent unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005573 RID: 21875 RVA: 0x001AE034 File Offset: 0x001AC234
		public void StopTimer()
		{
			this.isRunning = false;
			this.isPaused = false;
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			UnityEvent unityEvent = this.onTimerStopped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
		}

		// Token: 0x06005574 RID: 21876 RVA: 0x001AE087 File Offset: 0x001AC287
		public void Pause()
		{
			this.isPaused = true;
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x001AE090 File Offset: 0x001AC290
		public void Resume()
		{
			this.isPaused = false;
		}

		// Token: 0x06005576 RID: 21878 RVA: 0x001AE09C File Offset: 0x001AC29C
		public void SetFixedIntervalSeconds(float seconds)
		{
			this.useRandomDuration = false;
			this.fixedInterval = Mathf.Max(0f, seconds);
			this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
			this.elapsed = 0f;
		}

		// Token: 0x06005577 RID: 21879 RVA: 0x001AE0E8 File Offset: 0x001AC2E8
		public void OverrideNextIntervalSeconds(float seconds)
		{
			this.currentIntervalSeconds = Mathf.Max(0.001f, seconds);
			this.elapsed = 0f;
		}

		// Token: 0x06005578 RID: 21880 RVA: 0x001AE106 File Offset: 0x001AC306
		public void ResetRun()
		{
			this.runFiredSoFar = 0;
		}

		// Token: 0x06005579 RID: 21881 RVA: 0x001AE110 File Offset: 0x001AC310
		public void InvokeUpdate()
		{
			if (!this.isRunning || this.isPaused)
			{
				return;
			}
			this.elapsed += Time.deltaTime;
			if (this.elapsed >= this.currentIntervalSeconds)
			{
				if (this.isInPostFireDelay)
				{
					this.isInPostFireDelay = false;
					this.elapsed = 0f;
					this.RollNextInterval();
					return;
				}
				UnityEvent unityEvent = this.onIntervalFired;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.runFiredSoFar++;
				if (this.runLength == GorillaIntervalTimer.RunLength.Finite && this.runFiredSoFar >= Mathf.Max(1, this.maxFiresPerRun))
				{
					if (this.requireManualReset)
					{
						this.StopTimer();
						return;
					}
					this.runFiredSoFar = 0;
				}
				if (this.usePostIntervalDelay && this.postIntervalDelay > 0f)
				{
					this.isInPostFireDelay = true;
					this.elapsed = 0f;
					this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.postIntervalDelay));
					return;
				}
				this.elapsed = 0f;
				this.RollNextInterval();
			}
		}

		// Token: 0x0600557A RID: 21882 RVA: 0x001AE219 File Offset: 0x001AC419
		private void ResetElapsed()
		{
			this.elapsed = 0f;
		}

		// Token: 0x0600557B RID: 21883 RVA: 0x001AE228 File Offset: 0x001AC428
		private void RollNextInterval()
		{
			if (!this.useRandomDuration)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
				return;
			}
			float num = Mathf.Max(0f, this.ToSeconds(this.randTimeMin));
			float num2 = Mathf.Max(num, this.ToSeconds(this.randTimeMax));
			float num3;
			if (this.intervalSource == GorillaIntervalTimer.IntervalSource.NetworkedRandom && this.networkProvider != null)
			{
				switch (this.distribution)
				{
				default:
					num3 = this.networkProvider.NextFloat(num, num2);
					break;
				case GorillaIntervalTimer.RandomDistribution.Normal:
				{
					double num4 = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num5 = Math.Max(double.Epsilon, 1.0 - (double)this.networkProvider.NextFloat01());
					double num6 = Math.Sqrt(-2.0 * Math.Log(num4)) * Math.Sin(6.283185307179586 * num5);
					float num7 = 0.5f * (num + num2);
					float num8 = (num2 - num) / 6f;
					num3 = Mathf.Clamp(num7 + (float)(num6 * (double)num8), num, num2);
					break;
				}
				case GorillaIntervalTimer.RandomDistribution.Exponential:
				{
					double num9 = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num10 = 0.5 * (double)(num + num2);
					double num11 = (num10 > 0.0) ? (1.0 / num10) : 1.0;
					num3 = Mathf.Clamp((float)(-(float)Math.Log(num9) / num11), num, num2);
					break;
				}
				}
				this.currentIntervalSeconds = Mathf.Max(0.001f, num3);
				return;
			}
			switch (this.distribution)
			{
			default:
				num3 = Random.Range(num, num2);
				break;
			case GorillaIntervalTimer.RandomDistribution.Normal:
			{
				float num12 = Mathf.Max(float.Epsilon, 1f - Random.value);
				float num13 = 1f - Random.value;
				float num14 = Mathf.Sqrt(-2f * Mathf.Log(num12)) * Mathf.Sin(6.2831855f * num13);
				float num15 = 0.5f * (num + num2);
				float num16 = (num2 - num) / 6f;
				num3 = Mathf.Clamp(num15 + num14 * num16, num, num2);
				break;
			}
			case GorillaIntervalTimer.RandomDistribution.Exponential:
			{
				float num17 = 0.5f * (num + num2);
				float num18 = (num17 > 0f) ? (1f / num17) : 1f;
				num3 = Mathf.Clamp(-Mathf.Log(Mathf.Max(float.Epsilon, 1f - Random.value)) / num18, num, num2);
				break;
			}
			}
			this.currentIntervalSeconds = Mathf.Max(0.001f, num3);
		}

		// Token: 0x0600557C RID: 21884 RVA: 0x001AE4F8 File Offset: 0x001AC6F8
		private float ToSeconds(float value)
		{
			switch (this.unit)
			{
			default:
				return value;
			case GorillaIntervalTimer.TimeUnit.Minutes:
				return value * 60f;
			case GorillaIntervalTimer.TimeUnit.Hours:
				return value * 3600f;
			}
		}

		// Token: 0x0600557D RID: 21885 RVA: 0x001AE52F File Offset: 0x001AC72F
		public void RestartTimer()
		{
			this.ResetElapsed();
			this.RollNextInterval();
			this.StartTimer();
		}

		// Token: 0x0600557E RID: 21886 RVA: 0x001AE543 File Offset: 0x001AC743
		public float GetPassedTime()
		{
			return this.elapsed;
		}

		// Token: 0x0600557F RID: 21887 RVA: 0x001AE54B File Offset: 0x001AC74B
		public float GetRemainingTime()
		{
			return Mathf.Max(0f, this.currentIntervalSeconds - this.elapsed);
		}

		// Token: 0x04006251 RID: 25169
		[Header("Scheduling")]
		[Tooltip("If true, the timer will automatically start when this component is enabled.")]
		[SerializeField]
		private bool runOnEnable = true;

		// Token: 0x04006252 RID: 25170
		[Tooltip("If true, apply an initial delay before the first interval is fired.")]
		[SerializeField]
		private bool useInitialDelay;

		// Token: 0x04006253 RID: 25171
		[Tooltip("Delay (in seconds or minutes depending on Unit) before the first fire if 'Use Initial Delay' is enabled.")]
		[SerializeField]
		private float initialDelay;

		// Token: 0x04006254 RID: 25172
		[Header("Interval")]
		[Tooltip("Unit of time for Fixed Interval, Min and Max values.")]
		[SerializeField]
		private GorillaIntervalTimer.TimeUnit unit;

		// Token: 0x04006255 RID: 25173
		[Tooltip("Distribution type used for generating random intervals when Interval Source = LocalRandom.")]
		[SerializeField]
		private GorillaIntervalTimer.RandomDistribution distribution;

		// Token: 0x04006256 RID: 25174
		[Tooltip("Fixed interval duration (interpreted by Unit) when Use Random Duration = false.")]
		[SerializeField]
		private float fixedInterval = 1f;

		// Token: 0x04006257 RID: 25175
		[Space]
		[Tooltip("If false, 'Fixed Interval' is used. If true, a random interval is sampled each cycle.")]
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04006258 RID: 25176
		[Tooltip("Minimum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMin = 0.5f;

		// Token: 0x04006259 RID: 25177
		[Tooltip("Maximum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMax = 2f;

		// Token: 0x0400625A RID: 25178
		[Tooltip("Determines whether to use a local random generator or a networked random source.")]
		[SerializeField]
		private GorillaIntervalTimer.IntervalSource intervalSource;

		// Token: 0x0400625B RID: 25179
		[Header("Networked Interval (optional)")]
		[Tooltip("If Interval Source = NetworkedRandom, the timer queries this component for the next interval")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x0400625C RID: 25180
		[Space]
		[Tooltip("If true, wait this additional delay after onIntervalFired() before starting the next interval.")]
		[SerializeField]
		private bool usePostIntervalDelay;

		// Token: 0x0400625D RID: 25181
		[Tooltip("Additional delay (in selected Unit) to wait after onIntervalFired(), before the next interval begins.")]
		[SerializeField]
		private float postIntervalDelay;

		// Token: 0x0400625E RID: 25182
		[Header("Run Length")]
		[Tooltip("Infinite runs forever. Finite stops after Max Fires Per Run.")]
		[SerializeField]
		private GorillaIntervalTimer.RunLength runLength;

		// Token: 0x0400625F RID: 25183
		[Tooltip("Number of times the timer fires before the run completes (when Run Length = Finite).")]
		[SerializeField]
		private int maxFiresPerRun = 3;

		// Token: 0x04006260 RID: 25184
		[Tooltip("If true, the timer stops at the end of a finite run and requires ResetRun() / StartTimer() to continue. If false, the run counter auto-resets and continues.")]
		[SerializeField]
		private bool requireManualReset = true;

		// Token: 0x04006261 RID: 25185
		[Header("Events")]
		public UnityEvent onIntervalFired;

		// Token: 0x04006262 RID: 25186
		public UnityEvent onTimerStarted;

		// Token: 0x04006263 RID: 25187
		public UnityEvent onTimerStopped;

		// Token: 0x04006264 RID: 25188
		private const float minIntervalEpsilon = 0.001f;

		// Token: 0x04006265 RID: 25189
		private float currentIntervalSeconds = 1f;

		// Token: 0x04006266 RID: 25190
		private float elapsed;

		// Token: 0x04006267 RID: 25191
		private bool isRunning;

		// Token: 0x04006268 RID: 25192
		private bool isPaused;

		// Token: 0x04006269 RID: 25193
		private bool isRegistered;

		// Token: 0x0400626A RID: 25194
		private int runFiredSoFar;

		// Token: 0x0400626B RID: 25195
		private bool isInPostFireDelay;

		// Token: 0x02000D97 RID: 3479
		private enum TimeUnit
		{
			// Token: 0x0400626D RID: 25197
			Seconds,
			// Token: 0x0400626E RID: 25198
			Minutes,
			// Token: 0x0400626F RID: 25199
			Hours
		}

		// Token: 0x02000D98 RID: 3480
		private enum RandomDistribution
		{
			// Token: 0x04006271 RID: 25201
			Uniform,
			// Token: 0x04006272 RID: 25202
			Normal,
			// Token: 0x04006273 RID: 25203
			Exponential
		}

		// Token: 0x02000D99 RID: 3481
		private enum IntervalSource
		{
			// Token: 0x04006275 RID: 25205
			LocalRandom,
			// Token: 0x04006276 RID: 25206
			NetworkedRandom
		}

		// Token: 0x02000D9A RID: 3482
		private enum RunLength
		{
			// Token: 0x04006278 RID: 25208
			Infinite,
			// Token: 0x04006279 RID: 25209
			Finite
		}
	}
}
