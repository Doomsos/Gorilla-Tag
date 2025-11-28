using System;
using GameObjectScheduling;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000850 RID: 2128
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x0600380D RID: 14349 RVA: 0x0012CAA9 File Offset: 0x0012ACA9
	// (set) Token: 0x0600380E RID: 14350 RVA: 0x0012CAB1 File Offset: 0x0012ACB1
	public PartyGameModeWarning WarningScreen
	{
		get
		{
			return this.warningScreen;
		}
		set
		{
			this.warningScreen = value;
		}
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x0012CABA File Offset: 0x0012ACBA
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	// Token: 0x06003810 RID: 14352 RVA: 0x0012CAE0 File Offset: 0x0012ACE0
	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	// Token: 0x06003811 RID: 14353 RVA: 0x0012CB06 File Offset: 0x0012AD06
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		if (this.warningScreen.ShouldShowWarning)
		{
			this.warningScreen.Show();
			return;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x06003812 RID: 14354 RVA: 0x0012CB3B File Offset: 0x0012AD3B
	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode.ToLower() == this.gameMode.ToLower()) ? this.pressedMaterial : this.unpressedMaterial);
	}

	// Token: 0x06003813 RID: 14355 RVA: 0x0012CB70 File Offset: 0x0012AD70
	public void SetInfo(string Mode, string ModeTitle, bool NewMode, CountdownTextDate CountdownTo)
	{
		this.gameModeTitle.text = ModeTitle;
		this.gameMode = Mode;
		this.newModeSplash.SetActive(NewMode);
		this.limitedCountdown.gameObject.SetActive(false);
		if (CountdownTo == null)
		{
			return;
		}
		this.limitedCountdown.Countdown = CountdownTo;
		this.limitedCountdown.gameObject.SetActive(true);
	}

	// Token: 0x06003814 RID: 14356 RVA: 0x0012CBD6 File Offset: 0x0012ADD6
	public void HideNewAndLimitedTimeInfo()
	{
		this.limitedCountdown.gameObject.SetActive(false);
		this.newModeSplash.SetActive(false);
	}

	// Token: 0x04004734 RID: 18228
	[SerializeField]
	public string gameMode;

	// Token: 0x04004735 RID: 18229
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x04004736 RID: 18230
	[SerializeField]
	private TMP_Text gameModeTitle;

	// Token: 0x04004737 RID: 18231
	[SerializeField]
	private GameObject newModeSplash;

	// Token: 0x04004738 RID: 18232
	[SerializeField]
	private CountdownText limitedCountdown;
}
