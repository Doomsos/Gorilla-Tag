using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000DEF RID: 3567
	public class GorillaPlayerTimerCountDisplay : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060058F5 RID: 22773 RVA: 0x001C7B34 File Offset: 0x001C5D34
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060058F6 RID: 22774 RVA: 0x001C7B34 File Offset: 0x001C5D34
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x060058F7 RID: 22775 RVA: 0x001C7B3C File Offset: 0x001C5D3C
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			this.displayText.text = "TIME: --.--.-";
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.isInitialized = true;
		}

		// Token: 0x060058F8 RID: 22776 RVA: 0x001C7BC8 File Offset: 0x001C5DC8
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x060058F9 RID: 22777 RVA: 0x001C7C2D File Offset: 0x001C5E2D
		private void OnLocalTimerStarted()
		{
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x001C7C40 File Offset: 0x001C5E40
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				double num = timeDelta / 1000.0;
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds(num).ToString("mm\\:ss\\:f");
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x001C7CA4 File Offset: 0x001C5EA4
		private void UpdateLatestTime()
		{
			float timeForPlayer = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)timeForPlayer).ToString("mm\\:ss\\:f");
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x060058FC RID: 22780 RVA: 0x001C7CF4 File Offset: 0x001C5EF4
		// (set) Token: 0x060058FD RID: 22781 RVA: 0x001C7CFC File Offset: 0x001C5EFC
		public bool TickRunning { get; set; }

		// Token: 0x060058FE RID: 22782 RVA: 0x001C7D05 File Offset: 0x001C5F05
		public void Tick()
		{
			this.UpdateLatestTime();
		}

		// Token: 0x0400661C RID: 26140
		[SerializeField]
		private TMP_Text displayText;

		// Token: 0x0400661D RID: 26141
		private bool isInitialized;
	}
}
