using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E56 RID: 3670
	public class BuilderPieceTimer : MonoBehaviour, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x06005B9F RID: 23455 RVA: 0x001D6919 File Offset: 0x001D4B19
		private void Awake()
		{
			this.buttonTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnButtonPressed));
		}

		// Token: 0x06005BA0 RID: 23456 RVA: 0x001D6937 File Offset: 0x001D4B37
		private void OnDestroy()
		{
			if (this.buttonTrigger != null)
			{
				this.buttonTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06005BA1 RID: 23457 RVA: 0x001D6964 File Offset: 0x001D4B64
		private void OnButtonPressed()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (Time.time > this.lastTriggeredTime + this.debounceTime)
			{
				this.lastTriggeredTime = Time.time;
				if (!this.isStart && this.stopSoundBank != null)
				{
					this.stopSoundBank.Play();
				}
				else if (this.activateSoundBank != null)
				{
					this.activateSoundBank.Play();
				}
				if (this.isBoth && this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: 00:00:0";
				}
				PlayerTimerManager.instance.RequestTimerToggle(this.isStart);
			}
		}

		// Token: 0x06005BA2 RID: 23458 RVA: 0x001D6A1C File Offset: 0x001D4C1C
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isStart && !this.isBoth)
			{
				return;
			}
			double num = timeDelta;
			this.latestTime = num / 1000.0;
			if (this.latestTime > 3599.989990234375)
			{
				this.latestTime = 3599.989990234375;
			}
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds(this.latestTime).ToString("mm\\:ss\\:ff");
			if (this.isBoth && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStart = true;
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06005BA3 RID: 23459 RVA: 0x001D6ACB File Offset: 0x001D4CCB
		private void OnLocalTimerStarted()
		{
			if (this.isBoth)
			{
				this.isStart = false;
			}
			if (this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06005BA4 RID: 23460 RVA: 0x001D6AF8 File Offset: 0x001D4CF8
		private void OnZoneChanged()
		{
			bool active = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.displayText != null)
			{
				this.displayText.gameObject.SetActive(active);
			}
		}

		// Token: 0x06005BA5 RID: 23461 RVA: 0x001D6B40 File Offset: 0x001D4D40
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.latestTime = double.MaxValue;
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
				this.OnZoneChanged();
				this.displayText.text = "TIME: __:__:_";
			}
		}

		// Token: 0x06005BA6 RID: 23462 RVA: 0x001D6BA6 File Offset: 0x001D4DA6
		public void OnPieceDestroy()
		{
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06005BA7 RID: 23463 RVA: 0x00002789 File Offset: 0x00000989
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06005BA8 RID: 23464 RVA: 0x001D6BDC File Offset: 0x001D4DDC
		public void OnPieceActivate()
		{
			this.lastTriggeredTime = 0f;
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBoth)
			{
				this.isStart = !PlayerTimerManager.instance.IsLocalTimerStarted();
				if (!this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: __:__:_";
				}
			}
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06005BA9 RID: 23465 RVA: 0x001D6C88 File Offset: 0x001D4E88
		public void OnPieceDeactivate()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.displayText != null)
			{
				this.displayText.text = "TIME: --:--:-";
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x06005BAA RID: 23466 RVA: 0x001D6D04 File Offset: 0x001D4F04
		// (set) Token: 0x06005BAB RID: 23467 RVA: 0x001D6D0C File Offset: 0x001D4F0C
		public bool TickRunning { get; set; }

		// Token: 0x06005BAC RID: 23468 RVA: 0x001D6D18 File Offset: 0x001D4F18
		public void Tick()
		{
			if (this.displayText != null)
			{
				float num = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
				num = Mathf.Clamp(num, 0f, 3599.99f);
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)num).ToString("mm\\:ss\\:f");
			}
		}

		// Token: 0x040068EE RID: 26862
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040068EF RID: 26863
		[SerializeField]
		private bool isStart;

		// Token: 0x040068F0 RID: 26864
		[SerializeField]
		private bool isBoth;

		// Token: 0x040068F1 RID: 26865
		[SerializeField]
		private BuilderSmallHandTrigger buttonTrigger;

		// Token: 0x040068F2 RID: 26866
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x040068F3 RID: 26867
		[SerializeField]
		private SoundBankPlayer stopSoundBank;

		// Token: 0x040068F4 RID: 26868
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x040068F5 RID: 26869
		private float lastTriggeredTime;

		// Token: 0x040068F6 RID: 26870
		private double latestTime = 3.4028234663852886E+38;

		// Token: 0x040068F7 RID: 26871
		[SerializeField]
		private TMP_Text displayText;
	}
}
