using System;
using System.Collections;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E23 RID: 3619
	public class VirtualStumpReturnWatch : MonoBehaviour
	{
		// Token: 0x06005A62 RID: 23138 RVA: 0x001CF434 File Offset: 0x001CD634
		private void Start()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.AddListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.AddListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.AddListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x001CF4A4 File Offset: 0x001CD6A4
		private void OnDestroy()
		{
			if (this.returnButton != null)
			{
				this.returnButton.onStartPressingButton.RemoveListener(new UnityAction(this.OnStartedPressingButton));
				this.returnButton.onStopPressingButton.RemoveListener(new UnityAction(this.OnStoppedPressingButton));
				this.returnButton.onPressButton.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x001CF514 File Offset: 0x001CD714
		public static void SetWatchProperties(VirtualStumpReturnWatchProps props)
		{
			VirtualStumpReturnWatch.currentCustomMapProps = props;
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection, 0.5f, 5f);
			VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom = Mathf.Clamp(VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom, 0.5f, 5f);
		}

		// Token: 0x06005A65 RID: 23141 RVA: 0x001CF590 File Offset: 0x001CD790
		private float GetCurrentHoldDuration()
		{
			if (GorillaGameManager.instance.IsNull())
			{
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			switch (GorillaGameManager.instance.GameType())
			{
			case GameModeType.Infection:
				if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Infection;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			case GameModeType.Custom:
				if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
				{
					return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration_Custom;
				}
				return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
			}
			return VirtualStumpReturnWatch.currentCustomMapProps.holdDuration;
		}

		// Token: 0x06005A66 RID: 23142 RVA: 0x001CF639 File Offset: 0x001CD839
		private void OnStartedPressingButton()
		{
			this.startPressingButtonTime = Time.time;
			this.currentlyBeingPressed = true;
			this.returnButton.pressDuration = this.GetCurrentHoldDuration();
			this.ShowCountdownText();
			this.updateCountdownCoroutine = base.StartCoroutine(this.UpdateCountdownText());
		}

		// Token: 0x06005A67 RID: 23143 RVA: 0x001CF676 File Offset: 0x001CD876
		private void OnStoppedPressingButton()
		{
			this.currentlyBeingPressed = false;
			this.HideCountdownText();
			if (this.updateCountdownCoroutine != null)
			{
				base.StopCoroutine(this.updateCountdownCoroutine);
				this.updateCountdownCoroutine = null;
			}
		}

		// Token: 0x06005A68 RID: 23144 RVA: 0x001CF6A0 File Offset: 0x001CD8A0
		private void OnButtonPressed()
		{
			this.currentlyBeingPressed = false;
			if (ZoneManagement.IsInZone(GTZone.customMaps) && !CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				bool flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer;
				bool flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer;
				if (GorillaGameManager.instance.IsNotNull())
				{
					switch (GorillaGameManager.instance.GameType())
					{
					case GameModeType.Infection:
						if (VirtualStumpReturnWatch.currentCustomMapProps.infectionOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_Infection;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_Infection;
						}
						break;
					case GameModeType.Custom:
						if (VirtualStumpReturnWatch.currentCustomMapProps.customModeOverride)
						{
							flag = VirtualStumpReturnWatch.currentCustomMapProps.shouldTagPlayer_CustomMode;
							flag2 = VirtualStumpReturnWatch.currentCustomMapProps.shouldKickPlayer_CustomMode;
						}
						break;
					}
				}
				if (flag2 && NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				else if (flag)
				{
					GameMode.ReportHit();
				}
				CustomMapManager.ReturnToVirtualStump();
			}
		}

		// Token: 0x06005A69 RID: 23145 RVA: 0x001CF7A0 File Offset: 0x001CD9A0
		private void ShowCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			int num = 1 + Mathf.FloorToInt(this.GetCurrentHoldDuration());
			this.countdownText.text = num.ToString();
			this.countdownText.gameObject.SetActive(true);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005A6A RID: 23146 RVA: 0x001CF80C File Offset: 0x001CDA0C
		private void HideCountdownText()
		{
			if (this.countdownText.IsNull())
			{
				return;
			}
			this.countdownText.text = "";
			this.countdownText.gameObject.SetActive(false);
			if (this.buttonText.IsNotNull())
			{
				this.buttonText.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005A6B RID: 23147 RVA: 0x001CF866 File Offset: 0x001CDA66
		private IEnumerator UpdateCountdownText()
		{
			while (this.currentlyBeingPressed)
			{
				if (this.countdownText.IsNull())
				{
					yield break;
				}
				float num = this.GetCurrentHoldDuration() - (Time.time - this.startPressingButtonTime);
				int num2 = 1 + Mathf.FloorToInt(num);
				this.countdownText.text = num2.ToString();
				yield return null;
			}
			yield break;
		}

		// Token: 0x0400675D RID: 26461
		[SerializeField]
		private HeldButton returnButton;

		// Token: 0x0400675E RID: 26462
		[SerializeField]
		private TMP_Text buttonText;

		// Token: 0x0400675F RID: 26463
		[SerializeField]
		private TMP_Text countdownText;

		// Token: 0x04006760 RID: 26464
		private static VirtualStumpReturnWatchProps currentCustomMapProps;

		// Token: 0x04006761 RID: 26465
		private float startPressingButtonTime = -1f;

		// Token: 0x04006762 RID: 26466
		private bool currentlyBeingPressed;

		// Token: 0x04006763 RID: 26467
		private Coroutine updateCountdownCoroutine;
	}
}
