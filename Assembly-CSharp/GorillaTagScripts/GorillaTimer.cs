using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DF0 RID: 3568
	public class GorillaTimer : MonoBehaviourPun
	{
		// Token: 0x06005900 RID: 22784 RVA: 0x001C7D0D File Offset: 0x001C5F0D
		private void Awake()
		{
			this.ResetTimer();
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x001C7D15 File Offset: 0x001C5F15
		public void StartTimer()
		{
			this.startTimer = true;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06005902 RID: 22786 RVA: 0x001C7D2F File Offset: 0x001C5F2F
		public IEnumerator DelayedReStartTimer(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			this.RestartTimer();
			yield break;
		}

		// Token: 0x06005903 RID: 22787 RVA: 0x001C7D45 File Offset: 0x001C5F45
		private void StopTimer()
		{
			this.startTimer = false;
			UnityEvent<GorillaTimer> unityEvent = this.onTimerStopped;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x001C7D5F File Offset: 0x001C5F5F
		private void ResetTimer()
		{
			this.passedTime = 0f;
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x001C7D6C File Offset: 0x001C5F6C
		public void RestartTimer()
		{
			if (this.useRandomDuration)
			{
				this.SetTimerDuration(Random.Range(this.randTimeMin, this.randTimeMax));
			}
			this.ResetTimer();
			this.StartTimer();
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x001C7D99 File Offset: 0x001C5F99
		public void SetTimerDuration(float timer)
		{
			this.timerDuration = timer;
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x001C7DA2 File Offset: 0x001C5FA2
		public void InvokeUpdate()
		{
			if (this.startTimer)
			{
				this.passedTime += Time.deltaTime;
			}
			if (this.startTimer && this.passedTime >= this.timerDuration)
			{
				this.StopTimer();
				this.ResetTimer();
			}
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x001C7DE0 File Offset: 0x001C5FE0
		public float GetPassedTime()
		{
			return this.passedTime;
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x001C7DE8 File Offset: 0x001C5FE8
		public void SetPassedTime(float time)
		{
			this.passedTime = time;
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x001C7DF1 File Offset: 0x001C5FF1
		public float GetRemainingTime()
		{
			return this.timerDuration - this.passedTime;
		}

		// Token: 0x0600590B RID: 22795 RVA: 0x001C7E00 File Offset: 0x001C6000
		public void OnEnable()
		{
			GorillaTimerManager.RegisterGorillaTimer(this);
		}

		// Token: 0x0600590C RID: 22796 RVA: 0x001C7E08 File Offset: 0x001C6008
		public void OnDisable()
		{
			GorillaTimerManager.UnregisterGorillaTimer(this);
		}

		// Token: 0x0400661F RID: 26143
		[SerializeField]
		private float timerDuration;

		// Token: 0x04006620 RID: 26144
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04006621 RID: 26145
		[SerializeField]
		private float randTimeMin;

		// Token: 0x04006622 RID: 26146
		[SerializeField]
		private float randTimeMax;

		// Token: 0x04006623 RID: 26147
		private float passedTime;

		// Token: 0x04006624 RID: 26148
		private bool startTimer;

		// Token: 0x04006625 RID: 26149
		private bool resetTimer;

		// Token: 0x04006626 RID: 26150
		public UnityEvent<GorillaTimer> onTimerStarted;

		// Token: 0x04006627 RID: 26151
		public UnityEvent<GorillaTimer> onTimerStopped;
	}
}
