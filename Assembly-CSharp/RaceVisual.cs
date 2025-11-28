using System;
using TMPro;
using UnityEngine;

// Token: 0x020002EE RID: 750
public class RaceVisual : MonoBehaviour
{
	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001251 RID: 4689 RVA: 0x000602BF File Offset: 0x0005E4BF
	// (set) Token: 0x06001252 RID: 4690 RVA: 0x000602C7 File Offset: 0x0005E4C7
	public int raceId { get; private set; }

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001253 RID: 4691 RVA: 0x000602D0 File Offset: 0x0005E4D0
	// (set) Token: 0x06001254 RID: 4692 RVA: 0x000602D8 File Offset: 0x0005E4D8
	public bool TickRunning { get; set; }

	// Token: 0x06001255 RID: 4693 RVA: 0x000602E1 File Offset: 0x0005E4E1
	private void Awake()
	{
		this.checkpoints = base.GetComponent<RaceCheckpointManager>();
		this.finishLineText.text = "";
		this.SetScoreboardText("", "");
		this.SetRaceStartScoreboardText("", "");
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x0006031F File Offset: 0x0005E51F
	private void OnEnable()
	{
		RacingManager.instance.RegisterVisual(this);
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x0006032C File Offset: 0x0005E52C
	public void Button_StartRace(int laps)
	{
		RacingManager.instance.Button_StartRace(this.raceId, laps);
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x0006033F File Offset: 0x0005E53F
	public void ShowFinishLineText(string text)
	{
		this.finishLineText.text = text;
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x0006034D File Offset: 0x0005E54D
	public void UpdateCountdown(int timeRemaining)
	{
		if (timeRemaining != this.lastDisplayedCountdown)
		{
			this.countdownText.text = timeRemaining.ToString();
			this.finishLineText.text = "";
			this.lastDisplayedCountdown = timeRemaining;
		}
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x00060384 File Offset: 0x0005E584
	public void SetScoreboardText(string mainText, string timesText)
	{
		foreach (RacingScoreboard racingScoreboard in this.raceScoreboards)
		{
			racingScoreboard.mainDisplay.text = mainText;
			racingScoreboard.timesDisplay.text = timesText;
		}
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x000603C0 File Offset: 0x0005E5C0
	public void SetRaceStartScoreboardText(string mainText, string timesText)
	{
		this.raceStartScoreboard.mainDisplay.text = mainText;
		this.raceStartScoreboard.timesDisplay.text = timesText;
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x000603E4 File Offset: 0x0005E5E4
	public void ActivateStartingWall(bool enable)
	{
		this.startingWall.SetActive(enable);
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x000603F2 File Offset: 0x0005E5F2
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpoint)
	{
		return this.checkpoints.IsPlayerNearCheckpoint(player, checkpoint);
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x00060401 File Offset: 0x0005E601
	public void OnCountdownStart(int laps, float goAfterInterval)
	{
		this.raceConsoleVisual.ShowRaceInProgress(laps);
		this.countdownSoundPlayer.Play();
		this.countdownSoundPlayer.time = this.countdownSoundGoTime - goAfterInterval;
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x0006042D File Offset: 0x0005E62D
	public void OnRaceStart()
	{
		this.finishLineText.text = "GO!";
		this.checkpoints.OnRaceStart();
		this.lastDisplayedCountdown = 0;
		this.startingWall.SetActive(false);
		this.isRaceEndSoundEnabled = false;
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00060464 File Offset: 0x0005E664
	public void OnRaceEnded()
	{
		this.finishLineText.text = "";
		this.lastDisplayedCountdown = 0;
		this.checkpoints.OnRaceEnd();
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00060488 File Offset: 0x0005E688
	public void OnRaceReset()
	{
		this.raceConsoleVisual.ShowCanStartRace();
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x00060495 File Offset: 0x0005E695
	public void EnableRaceEndSound()
	{
		this.isRaceEndSoundEnabled = true;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0006049E File Offset: 0x0005E69E
	public void OnCheckpointPassed(int index, SoundBankPlayer checkpointSound)
	{
		if (index == 0 && this.isRaceEndSoundEnabled)
		{
			this.countdownSoundPlayer.PlayOneShot(this.raceEndSound);
		}
		else
		{
			checkpointSound.Play();
		}
		RacingManager.instance.OnCheckpointPassed(this.raceId, index);
	}

	// Token: 0x040016ED RID: 5869
	[SerializeField]
	private TextMeshPro finishLineText;

	// Token: 0x040016EE RID: 5870
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x040016EF RID: 5871
	[SerializeField]
	private RacingScoreboard[] raceScoreboards;

	// Token: 0x040016F0 RID: 5872
	[SerializeField]
	private RacingScoreboard raceStartScoreboard;

	// Token: 0x040016F1 RID: 5873
	[SerializeField]
	private RaceConsoleVisual raceConsoleVisual;

	// Token: 0x040016F2 RID: 5874
	private float nextVisualRefreshTimestamp;

	// Token: 0x040016F3 RID: 5875
	private RaceCheckpointManager checkpoints;

	// Token: 0x040016F4 RID: 5876
	[SerializeField]
	private AudioClip raceEndSound;

	// Token: 0x040016F5 RID: 5877
	[SerializeField]
	private float countdownSoundGoTime;

	// Token: 0x040016F6 RID: 5878
	[SerializeField]
	private AudioSource countdownSoundPlayer;

	// Token: 0x040016F7 RID: 5879
	[SerializeField]
	private GameObject startingWall;

	// Token: 0x040016F8 RID: 5880
	private int lastDisplayedCountdown;

	// Token: 0x040016F9 RID: 5881
	private bool isRaceEndSoundEnabled;
}
