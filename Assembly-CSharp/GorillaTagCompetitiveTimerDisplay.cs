using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020007D6 RID: 2006
public class GorillaTagCompetitiveTimerDisplay : MonoBehaviour
{
	// Token: 0x06003491 RID: 13457 RVA: 0x00119E94 File Offset: 0x00118094
	private void Awake()
	{
		this.prevTime = -1;
		if (this.waitingForPlayersBackground)
		{
			this.waitingForPlayersBackground.SetActive(true);
			this.currentBackground = this.waitingForPlayersBackground;
		}
		if (this.startCountdownBackground)
		{
			this.startCountdownBackground.SetActive(false);
		}
		if (this.playingBackground)
		{
			this.playingBackground.SetActive(false);
		}
		if (this.postRoundBackground)
		{
			this.postRoundBackground.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(false);
		if (this.timerDisplay2)
		{
			this.timerDisplay2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x00119F48 File Offset: 0x00118148
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += new Action<GorillaTagCompetitiveManager.GameState>(this.HandleOnGameStateChanged);
		GorillaTagCompetitiveManager.onUpdateRemainingTime += new Action<float>(this.HandleOnTimeChanged);
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager != null)
		{
			this.HandleOnGameStateChanged(gorillaTagCompetitiveManager.GetCurrentGameState());
		}
		this.myRig = base.GetComponentInParent<VRRig>();
		this.DisplayStandardTimer(false);
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x00119FAA File Offset: 0x001181AA
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= new Action<GorillaTagCompetitiveManager.GameState>(this.HandleOnGameStateChanged);
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= new Action<float>(this.HandleOnTimeChanged);
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x00119FD0 File Offset: 0x001181D0
	private void HandleOnGameStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		this.SetNewBackground(newState);
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.DisplayStandardTimer(false);
			this.resultsDisplay.gameObject.SetActive(false);
			return;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.DisplayStandardTimer(true);
			return;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.DoPostRoundShow();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x0011A024 File Offset: 0x00118224
	private void DisplayStandardTimer(bool bShow)
	{
		if (bShow)
		{
			this.resultsDisplay.gameObject.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(bShow);
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.gameObject.SetActive(bShow);
		}
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x0011A078 File Offset: 0x00118278
	private void DoPostRoundShow()
	{
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager == null)
		{
			return;
		}
		this.DisplayStandardTimer(false);
		this.resultsDisplay.gameObject.SetActive(true);
		List<VRRig> list = new List<VRRig>();
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = gorillaTagCompetitiveManager.GetScoring().GetSortedScores();
		float b = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[0].NumTags, sortedScores[0].PointsOnDefense);
		int num = 0;
		while (num < sortedScores.Count && num < 3)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(sortedScores[num].PlayerId, out rigContainer))
			{
				float a = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[num].NumTags, sortedScores[num].PointsOnDefense);
				if (num == 0 || a.Approx(b, 0.01f))
				{
					list.Add(rigContainer.Rig);
				}
				switch (num)
				{
				case 0:
					if (this.tintableCelebration != null)
					{
						Color playerColor = rigContainer.Rig.playerColor;
						float num2;
						float num3;
						float num4;
						Color.RGBToHSV(playerColor, ref num2, ref num3, ref num4);
						Color color = Color.HSVToRGB(num2, num3, (num4 < 0.5f) ? (num4 + 0.5f) : (num4 - 0.5f));
						this.tintableCelebration.main.startColor = new ParticleSystem.MinMaxGradient(playerColor, color);
						this.tintableCelebration.gameObject.SetActive(true);
					}
					if (this.goldCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.goldCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 1:
					if (this.silverCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.silverCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 2:
					if (this.bronzeCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.bronzeCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				}
			}
			num++;
		}
		for (int i = 0; i < this.postRoundTimerText.Length; i++)
		{
			this.postRoundTimerText[i].text = ((list.Count > 1) ? "SHARED WIN" : "WINNER");
		}
		string text = string.Empty;
		for (int j = 0; j < list.Count; j++)
		{
			text = text + list[j].playerText1.text.ToUpper() + "\n";
		}
		this.resultsDisplay.text = text.Trim();
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.text = this.resultsDisplay.text;
		}
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x0011A3A0 File Offset: 0x001185A0
	private void HandleOnTimeChanged(float time)
	{
		int num = Mathf.CeilToInt(time);
		num = Mathf.Max(num, 1);
		if (this.prevTime != num)
		{
			this.prevTime = num;
			if (this.currentState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				int num2 = this.prevTime / 60;
				int num3 = this.prevTime % 60;
				this.timerDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = string.Format("{0}:{1:D2}", num2, num3);
					return;
				}
			}
			else if (this.currentState != GorillaTagCompetitiveManager.GameState.PostRound)
			{
				this.timerDisplay.text = this.prevTime.ToString("#00");
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = this.prevTime.ToString("#00");
				}
			}
		}
	}

	// Token: 0x06003498 RID: 13464 RVA: 0x0011A488 File Offset: 0x00118688
	private void SetNewBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		if (this.currentBackground != null)
		{
			this.currentBackground.SetActive(false);
		}
		this.currentState = newState;
		GameObject gameObject = this.SelectBackground(newState);
		this.GetTextColor(newState);
		this.currentBackground = null;
		if (gameObject != null)
		{
			this.currentBackground = gameObject;
			this.currentBackground.SetActive(true);
		}
	}

	// Token: 0x06003499 RID: 13465 RVA: 0x0011A4E9 File Offset: 0x001186E9
	private GameObject SelectBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			return this.waitingForPlayersBackground;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.startCountdownBackground;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.playingBackground;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.postRoundBackground;
		default:
			return null;
		}
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x0011A522 File Offset: 0x00118722
	private Color GetTextColor(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.timerColorStart;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.timerColorPlaying;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.timerColorPostRound;
		default:
			return Color.white;
		}
	}

	// Token: 0x040042E9 RID: 17129
	public TextMeshPro timerDisplay;

	// Token: 0x040042EA RID: 17130
	public TextMeshPro timerDisplay2;

	// Token: 0x040042EB RID: 17131
	public TextMeshPro resultsDisplay;

	// Token: 0x040042EC RID: 17132
	public GameObject waitingForPlayersBackground;

	// Token: 0x040042ED RID: 17133
	public GameObject startCountdownBackground;

	// Token: 0x040042EE RID: 17134
	public Color timerColorStart = Color.white;

	// Token: 0x040042EF RID: 17135
	public GameObject playingBackground;

	// Token: 0x040042F0 RID: 17136
	public Color timerColorPlaying = Color.white;

	// Token: 0x040042F1 RID: 17137
	public GameObject postRoundBackground;

	// Token: 0x040042F2 RID: 17138
	public Color timerColorPostRound = Color.white;

	// Token: 0x040042F3 RID: 17139
	public TextMeshPro[] postRoundTimerText;

	// Token: 0x040042F4 RID: 17140
	private GorillaTagCompetitiveManager.GameState currentState;

	// Token: 0x040042F5 RID: 17141
	private GameObject currentBackground;

	// Token: 0x040042F6 RID: 17142
	private int prevTime = -1;

	// Token: 0x040042F7 RID: 17143
	[SerializeField]
	private ParticleSystem tintableCelebration;

	// Token: 0x040042F8 RID: 17144
	[SerializeField]
	private ParticleSystem goldCelebration;

	// Token: 0x040042F9 RID: 17145
	[SerializeField]
	private ParticleSystem silverCelebration;

	// Token: 0x040042FA RID: 17146
	[SerializeField]
	private ParticleSystem bronzeCelebration;

	// Token: 0x040042FB RID: 17147
	private VRRig myRig;

	// Token: 0x040042FC RID: 17148
	[SerializeField]
	private AudioSource celebrationAudio;
}
