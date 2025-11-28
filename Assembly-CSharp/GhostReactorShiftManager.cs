using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000656 RID: 1622
public class GhostReactorShiftManager : MonoBehaviourTick
{
	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002998 RID: 10648 RVA: 0x000E0238 File Offset: 0x000DE438
	public int ShiftTotalEarned
	{
		get
		{
			return this.shiftTotalEarned;
		}
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x06002999 RID: 10649 RVA: 0x000E0240 File Offset: 0x000DE440
	public bool ShiftActive
	{
		get
		{
			return this.shiftStarted;
		}
	}

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x0600299A RID: 10650 RVA: 0x000E0248 File Offset: 0x000DE448
	public double ShiftStartNetworkTime
	{
		get
		{
			return this.shiftStartNetworkTime;
		}
	}

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x0600299B RID: 10651 RVA: 0x000E0250 File Offset: 0x000DE450
	public bool LocalPlayerInside
	{
		get
		{
			return this.localPlayerInside;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x0600299C RID: 10652 RVA: 0x000E0258 File Offset: 0x000DE458
	public float TotalPlayTime
	{
		get
		{
			return this.totalPlayTime;
		}
	}

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x0600299D RID: 10653 RVA: 0x000E0260 File Offset: 0x000DE460
	public string ShiftId
	{
		get
		{
			return this.gameIdGuid;
		}
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x000E0268 File Offset: 0x000DE468
	public void SetShiftId(string shiftId)
	{
		this.gameIdGuid = shiftId;
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000E0271 File Offset: 0x000DE471
	public void Init(GhostReactorManager grManager)
	{
		this.grManager = grManager;
		this.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		this.depthDisplay.Setup();
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000E0290 File Offset: 0x000DE490
	public void RefreshShiftStatsDisplay()
	{
		this.shiftStatsText.text = string.Concat(new string[]
		{
			"\n\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.SentientCoresCollected).ToString("D2"),
			"\n",
			this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths).ToString("D2")
		});
		this.depthDisplay.RefreshObjectives();
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x000E034E File Offset: 0x000DE54E
	public void StartShiftButtonPressed()
	{
		this.RequestShiftStart();
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x00002789 File Offset: 0x00000989
	public void RequestShiftStart()
	{
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000E0356 File Offset: 0x000DE556
	public void EndShift()
	{
		this.grManager.RequestShiftEnd();
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000E0363 File Offset: 0x000DE563
	public void ClearEntities()
	{
		Debug.LogError("Need to re-implement whatever this was doing");
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000E0370 File Offset: 0x000DE570
	public void RefreshShiftTimer()
	{
		if (this.shiftTimerText != null)
		{
			this.shiftTimerText.text = Mathf.FloorToInt(this.shiftDurationMinutes).ToString("D2") + ":00";
		}
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000E03B8 File Offset: 0x000DE5B8
	public void UpdateLogoAnimations(List<TMP_Text> frames)
	{
		float num = 300f;
		float num2 = 0.5f;
		double time = PhotonNetwork.Time;
		if (frames.Count < 4)
		{
			return;
		}
		if (this.lastReactorLogoAnimationTime + (double)num < time || time < this.lastReactorLogoAnimationTime)
		{
			this.isPlayingLogoAnimation = true;
			this.lastReactorLogoAnimationTime = time;
		}
		if (this.isPlayingLogoAnimation)
		{
			if (this.lastReactorLogoAnimationTime + (double)num2 < time)
			{
				this.isPlayingLogoAnimation = false;
			}
			float num3 = Mathf.Clamp01((float)(time - this.lastReactorLogoAnimationTime) / num2) * 3.1415925f;
			int num4 = (int)(3.5f - Mathf.Abs(Mathf.Cos(num3) * 3f));
			if (!this.isPlayingLogoAnimation)
			{
				num4 = 0;
			}
			if (this.lastReactorLogoAnimFrame != num4)
			{
				frames[this.lastReactorLogoAnimFrame].gameObject.SetActive(false);
				frames[num4].gameObject.SetActive(true);
				this.lastReactorLogoAnimFrame = num4;
			}
			return;
		}
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000E049C File Offset: 0x000DE69C
	public void UpdateReactorDisplayMainShared(float countDownTotal)
	{
		if (this.reactorTextMain == null)
		{
			return;
		}
		double time = PhotonNetwork.Time;
		float num = 0.5f;
		if (this.lastReactorDisplayUpdate < time && this.lastReactorDisplayUpdate + (double)num > time)
		{
			return;
		}
		this.lastReactorDisplayUpdate = time;
		this.cachedStringBuilder.Clear();
		int num2 = Mathf.FloorToInt(countDownTotal / 60f);
		int num3 = Mathf.FloorToInt(countDownTotal % 60f);
		switch (this.state)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.ShiftActive:
		{
			int shiftStat = this.shiftStats.GetShiftStat(GRShiftStatType.CoresCollected);
			int num4 = this.coresRequiredToDelveDeeper;
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + 1000));
			this.cachedStringBuilder.AppendLine("ANOMALY COLLAPSE IN " + num2.ToString("D2") + ":" + num3.ToString("D2"));
			if (shiftStat >= num4)
			{
				this.cachedStringBuilder.Append("\nPOWER REQUIREMENTS MET\n");
			}
			else
			{
				this.cachedStringBuilder.Append(string.Format("\nCORES REQUIRED ({0}/{1})\n", shiftStat, num4));
			}
			int num5 = (int)((float)shiftStat / (float)num4 * 30f);
			if (shiftStat > 1 && num5 == 0)
			{
				num5 = 1;
			}
			int num6 = num5 / 3;
			int num7 = num5 - num6 * 3;
			for (int i = 0; i < 10; i++)
			{
				if (i < num6)
				{
					this.cachedStringBuilder.Append("▐█");
				}
				else if (i > num6 || num7 == 0)
				{
					this.cachedStringBuilder.Append(" ░");
				}
				else if (num7 == 1)
				{
					this.cachedStringBuilder.Append("▐░");
				}
				else
				{
					this.cachedStringBuilder.Append("▐▌");
				}
			}
			this.cachedStringBuilder.Append("\n");
			if (shiftStat > 0)
			{
				this.cachedStringBuilder.Append(string.Format("\nTOTAL BONUS EARNED: +⑭{0}", shiftStat * 5));
			}
			break;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000));
			this.cachedStringBuilder.AppendLine("STAND BY");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + 1000);
			break;
		case GhostReactorShiftManager.State.Drilling:
		{
			int num8 = (int)((time - this.stateStartTime) / (double)this.GetDrillingDuration() * 1000.0);
			this.cachedStringBuilder.AppendLine(string.Format("DEPTH {0}m", this.reactor.GetDepthLevel() * 1000 + num8));
			this.cachedStringBuilder.AppendLine("DRILLING");
			this.depthDisplay.jumbotronTitle.text = string.Format("<size=1>CURRENT DEPTH</size>\n{0}m", this.reactor.GetDepthLevel() * 1000 + num8);
			break;
		}
		}
		this.reactorTextMain.text = this.cachedStringBuilder.ToString();
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000E0898 File Offset: 0x000DEA98
	public void OnShiftStarted(string gameId, double shiftStartTime, bool wasPlayerInAtStart, bool isFirstShift)
	{
		this.gameIdGuid = gameId;
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (!this.shiftStarted && grplayer != null)
		{
			float num = (float)(PhotonNetwork.Time - shiftStartTime);
			grplayer.ResetTelemetryTracking(this.gameIdGuid, num);
			grplayer.IncrementShiftsPlayed(1);
			grplayer.SendFloorStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "");
			if (grplayer.isFirstShift)
			{
				grplayer.SendGameStartedTelemetry(num, wasPlayerInAtStart, this.reactor.GetDepthLevel());
				grplayer.gameStartTime = (float)PhotonNetwork.Time;
			}
		}
		this.shiftStarted = true;
		this.shiftJustStarted = true;
		this.shiftStartNetworkTime = shiftStartTime;
		this.frontGate.OpenGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.gateBlockerTransform.gameObject.SetActive(false);
		this.prevCountDownTotal = this.shiftDurationMinutes * 60f;
		this.shiftTotalEarned = -1;
		this.authorizedToDelveDeeper = false;
		this.ResetJoinTimes();
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		this.isRoomClosed = false;
		if (grplayer != null)
		{
			grplayer.RefreshPlayerVisuals();
		}
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000E09F4 File Offset: 0x000DEBF4
	public void OnShiftEnded(double shiftEndTime, bool isShiftActuallyEnding, ZoneClearReason zoneClearReason = ZoneClearReason.JoinZone)
	{
		if (this.shiftStarted)
		{
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			if (component != null)
			{
				component.SendFloorEndedTelemetry(isShiftActuallyEnding, (float)this.shiftStartNetworkTime, zoneClearReason, this.reactor.GetDepthLevel(), this.reactor.GetCurrLevelGenConfig().name, "", this.authorizedToDelveDeeper, ((this.reactor.GetDepthLevel() + 1) / 5).ToString(), this.authorizedToDelveDeeper ? (10 * this.reactor.GetDepthLevel()) : 0);
			}
		}
		this.shiftStarted = false;
		this.shiftEndNetworkTime = shiftEndTime;
		this.RefreshShiftTimer();
		this.frontGate.CloseGate();
		this.ringTransform.gameObject.SetActive(false);
		this.anomalyLoop1.Stop();
		this.anomalyLoop2.Stop();
		this.anomalyLoop3.Stop();
		this.anomalyAlert.Stop();
		this.TeleportLocalPlayerIfOutOfBounds();
		if (this.shiftEndNetworkTime > 0.0 && this.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) > this.shiftStats.GetShiftStat(GRShiftStatType.PlayerDeaths))
		{
			PlayerGameEvents.MiscEvent("GRShiftGoodKD", 1);
		}
		if (PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("ghostReactorShiftStarted", "false");
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
			this.isRoomClosed = false;
		}
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000E0B68 File Offset: 0x000DED68
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		double num = PhotonNetwork.Time - this.shiftStartNetworkTime;
		float num2 = 60f * this.shiftDurationMinutes - (float)num;
		if (this.grManager.IsAuthority())
		{
			this.AuthorityUpdate(num2);
		}
		num2 = Mathf.Clamp(num2, 0f, 60f * this.shiftDurationMinutes);
		this.SharedUpdate(num2);
		this.prevCountDownTotal = num2;
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000E0BDC File Offset: 0x000DEDDC
	private void AuthorityUpdate(float countDownTotal)
	{
		if (PhotonNetwork.InRoom && this.grManager.IsAuthority())
		{
			if (this.shiftStarted && !NetworkSystem.Instance.SessionIsPrivate && !this.isRoomClosed && 60f * this.shiftDurationMinutes - countDownTotal >= this.roomCloseTimeSeconds)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("ghostReactorShiftStarted", "true");
				PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				this.isRoomClosed = true;
			}
			if (this.shiftStarted && countDownTotal <= 0f)
			{
				this.grManager.RequestShiftEnd();
			}
			this.UpdateStateAuthority();
		}
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000E0C80 File Offset: 0x000DEE80
	private void SharedUpdate(float countDownTotal)
	{
		this.UpdateStateShared();
		this.UpdateReactorDisplayMainShared(countDownTotal);
		if (this.lastLeaderboardRefreshTime + (double)this.leaderboardUpdateFrequency < (double)Time.time || (double)Time.time < this.lastLeaderboardRefreshTime)
		{
			this.RefreshShiftLeaderboard();
			this.lastLeaderboardRefreshTime = (double)Time.time;
		}
		if (this.shiftStarted)
		{
			if (this.debugFastForwarding)
			{
				float num = this.debugFastForwardRate * Time.deltaTime;
				this.shiftStartNetworkTime -= (double)num;
			}
			int num2 = Mathf.FloorToInt(countDownTotal / 60f);
			int num3 = Mathf.FloorToInt(countDownTotal % 60f);
			this.shiftTimerText.text = num2.ToString("D2") + ":" + num3.ToString("D2");
			for (int i = 0; i < this.warnings.Count; i++)
			{
				if (countDownTotal < (float)this.warnings[i].time && this.prevCountDownTotal >= (float)this.warnings[i].time && !this.shiftJustStarted)
				{
					this.warnings[i].sound.Play(this.announceAudioSource);
					break;
				}
			}
			if (this.state == GhostReactorShiftManager.State.ShiftActive && countDownTotal > 0f && countDownTotal < this.anomalyAlertCountdownTimeToStartPlayingInMinutes * 60f && !this.anomalyAlert.isPlaying)
			{
				this.anomalyAlert.Play();
			}
			if (this.localPlayerInside)
			{
				if (countDownTotal >= 0f && countDownTotal < this.ringClosingDuration * 60f)
				{
					this.ringTransform.gameObject.SetActive(true);
					float num4 = Mathf.Lerp(this.ringClosingMinRadius, this.ringClosingMaxRadius, countDownTotal / (this.ringClosingDuration * 60f));
					this.ringTransform.localScale = new Vector3(num4, 1f, num4);
					Vector3 position = VRRig.LocalRig.bodyTransform.position;
					Vector3 vector = position - this.ringTransform.position;
					vector.y = 0f;
					Vector3 normalized = vector.normalized;
					float num5 = 0.5235988f;
					Vector3 position2 = this.ringTransform.position + normalized * num4;
					Quaternion quaternion = Quaternion.AngleAxis(num5, Vector3.up);
					Quaternion quaternion2 = Quaternion.AngleAxis(-num5, Vector3.up);
					Vector3 position3 = this.ringTransform.position + quaternion * normalized * num4;
					Vector3 position4 = this.ringTransform.position + quaternion2 * normalized * num4;
					position2.y = position.y;
					position3.y = position.y;
					position4.y = position.y;
					this.anomalyLoop1.transform.position = position2;
					this.anomalyLoop2.transform.position = position3;
					this.anomalyLoop3.transform.position = position4;
					if (!this.anomalyLoop1.isPlaying)
					{
						this.anomalyLoop1.Play();
					}
					if (!this.anomalyLoop2.isPlaying)
					{
						this.anomalyLoop2.Play();
					}
					if (!this.anomalyLoop3.isPlaying)
					{
						this.anomalyLoop3.Play();
					}
					if (vector.sqrMagnitude > num4 * num4)
					{
						this.TeleportLocalPlayerIfOutOfBounds();
					}
				}
			}
			else if (this.ringTransform.gameObject.activeSelf)
			{
				this.ringTransform.gameObject.SetActive(false);
			}
			this.shiftJustStarted = false;
			return;
		}
		if (!this.shiftStarted)
		{
			this.TeleportLocalPlayerIfOutOfBounds();
		}
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000E100C File Offset: 0x000DF20C
	private void TeleportLocalPlayerIfOutOfBounds()
	{
		if (this.localPlayerInside || (this.localPlayerOverlapping && Vector3.Dot(GTPlayer.Instance.headCollider.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f))
		{
			this.grManager.ReportLocalPlayerHit();
			GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
			component.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this.grManager);
			GTPlayer.Instance.TeleportTo(this.playerTeleportTransform, true, true);
			this.localPlayerInside = false;
			this.localPlayerOverlapping = false;
			component.caughtByAnomaly = true;
		}
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000E10B0 File Offset: 0x000DF2B0
	public void RevealJudgment(int evaluation)
	{
		if (evaluation <= 0)
		{
			this.shiftJugmentText.text = "DON'T QUIT YOUR DAY JOB.";
			return;
		}
		switch (evaluation)
		{
		case 1:
			this.shiftJugmentText.text = "YOU'RE LEARNING. GOOD.";
			return;
		case 2:
			this.shiftJugmentText.text = "YOU MIGHT EARN A PROMOTION.";
			return;
		case 3:
			this.shiftJugmentText.text = "YOU DID A MANAGER-TIER JOB.";
			return;
		case 4:
			this.shiftJugmentText.text = "NICE. YOU GET EXTRA SHIFTS.";
			return;
		default:
			this.shiftJugmentText.text = "YOU WORK FOR US NOW.";
			if (this.wrongStumpGoo != null)
			{
				this.wrongStumpGoo.SetActive(true);
			}
			return;
		}
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000E115A File Offset: 0x000DF35A
	public void ResetJudgment()
	{
		this.shiftJugmentText.text = "";
		if (this.wrongStumpGoo != null)
		{
			this.wrongStumpGoo.SetActive(false);
		}
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000E1188 File Offset: 0x000DF388
	public void ResetJoinTimes()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer.Get(this.reactor.vrRigs[i]).shiftJoinTime = this.shiftStartNetworkTime;
		}
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000E11E0 File Offset: 0x000DF3E0
	public void CalculatePlayerPercentages()
	{
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else if (this.shiftStarted)
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(this.shiftEndNetworkTime - grplayer.shiftJoinTime));
				}
				this.totalPlayTime += grplayer.ShiftPlayTime;
			}
		}
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000E12E8 File Offset: 0x000DF4E8
	public void CalculateShiftTotal()
	{
		this.shiftTotalEarned = 0;
		int count = this.reactor.vrRigs.Count;
		double num = 0.0;
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (this.reactor.vrRigs[i] != null && grplayer != null)
			{
				this.shiftTotalEarned += grplayer.ShiftCredits;
				if (this.reactor.vrRigs[i].OwningNetPlayer == null)
				{
					grplayer.ShiftPlayTime = 0.1f;
				}
				else
				{
					grplayer.ShiftPlayTime = Mathf.Min(this.shiftDurationMinutes * 60f, (float)(PhotonNetwork.Time - grplayer.shiftJoinTime));
				}
				num += (double)grplayer.ShiftPlayTime;
			}
		}
		this.shiftTotalEarned = Mathf.Clamp(this.shiftTotalEarned, 0, this.shiftSanityMaximumEarned);
		num = (double)Mathf.Clamp((float)num, 0.1f, this.shiftDurationMinutes * 10f * 60f);
		for (int j = 0; j < count; j++)
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
			if (this.reactor.vrRigs[j] != null && grplayer2 != null && this.depthDisplay != null)
			{
				int rewardXP = this.depthDisplay.GetRewardXP();
				if (this.authorizedToDelveDeeper)
				{
					grplayer2.LastShiftCut = rewardXP;
					grplayer2.CollectShiftCut();
				}
			}
		}
		this.reactor.RefreshScoreboards();
		this.reactor.promotionBot.Refresh();
		this.reactor.RefreshDepth();
	}

	// Token: 0x060029B3 RID: 10675 RVA: 0x000E14A0 File Offset: 0x000DF6A0
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.localPlayerOverlapping = true;
		}
	}

	// Token: 0x060029B4 RID: 10676 RVA: 0x000E14BC File Offset: 0x000DF6BC
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			bool flag = Vector3.Dot(other.transform.position - this.gatePlaneTransform.position, this.gatePlaneTransform.forward) < 0f;
			this.localPlayerInside = flag;
			this.localPlayerOverlapping = false;
		}
	}

	// Token: 0x060029B5 RID: 10677 RVA: 0x000E151C File Offset: 0x000DF71C
	public void OnButtonDelveDeeper()
	{
		if (this.ShiftActive)
		{
			bool flag = this.authorizedToDelveDeeper;
			return;
		}
	}

	// Token: 0x060029B6 RID: 10678 RVA: 0x000E152E File Offset: 0x000DF72E
	public void OnButtonDEBUGResetDepth()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_ResetDepth);
	}

	// Token: 0x060029B7 RID: 10679 RVA: 0x000E153D File Offset: 0x000DF73D
	public void OnButtonDEBUGDelveDeeper()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveDeeper);
	}

	// Token: 0x060029B8 RID: 10680 RVA: 0x000E154C File Offset: 0x000DF74C
	public void OnButtonDEBUGDelveShallower()
	{
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DEBUG_DelveShallower);
	}

	// Token: 0x060029B9 RID: 10681 RVA: 0x000E155B File Offset: 0x000DF75B
	public void RequestState(GhostReactorShiftManager.State newState)
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveState, (int)newState);
	}

	// Token: 0x060029BA RID: 10682 RVA: 0x000E1578 File Offset: 0x000DF778
	public void SetState(GhostReactorShiftManager.State newState, bool force = false)
	{
		if (this.state == newState && !force)
		{
			return;
		}
		GhostReactorShiftManager.State state = this.state;
		if (state != GhostReactorShiftManager.State.ReadyForShift)
		{
			if (state == GhostReactorShiftManager.State.Drilling)
			{
				this.reactor.shiftManager.depthDisplay.StopDelveDeeperFX();
			}
		}
		else if (this.startShiftButton != null)
		{
			this.startShiftButton.SetActive(false);
		}
		this.state = newState;
		this.stateStartTime = PhotonNetwork.Time;
		switch (this.state)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			this.announceBell.Play(this.announceBellAudioSource);
			this.announceTip.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
			goto IL_21F;
		case GhostReactorShiftManager.State.ShiftActive:
			this.announceStartShift.Play(this.announceAudioSource);
			using (List<VRRig>.Enumerator enumerator = this.reactor.vrRigs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					VRRig vrrig = enumerator.Current;
					GRPlayer component = vrrig.GetComponent<GRPlayer>();
					if (component != null)
					{
						component.startingShiftCreditCache = component.ShiftCredits;
					}
				}
				goto IL_21F;
			}
			break;
		case GhostReactorShiftManager.State.PostShift:
			if (this.authorizedToDelveDeeper)
			{
				this.announceCompleteShift.Play(this.announceAudioSource);
				if (!string.IsNullOrEmpty(this.ShiftId))
				{
					ProgressionManager.Instance.EndOfShiftReward(this.ShiftId);
					int count = this.reactor.vrRigs.Count;
					for (int i = 0; i < count; i++)
					{
						GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
						if (grplayer != null)
						{
							grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)this.shiftRewardCredits);
						}
					}
				}
				Debug.LogError("ShiftId is null or empty, skipping reward of end of shift.");
				goto IL_21F;
			}
			this.announceFailShift.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.PreparingToDrill:
			this.announcePrepareDrill.Play(this.announceAudioSource);
			goto IL_21F;
		case GhostReactorShiftManager.State.Drilling:
			this.reactor.DelveToNextDepth();
			this.reactor.shiftManager.depthDisplay.StartDelveDeeperFX();
			goto IL_21F;
		default:
			goto IL_21F;
		}
		this.announceBell.Play(this.announceBellAudioSource);
		this.announceTip.Play(this.announceAudioSource);
		IL_21F:
		this.RefreshDepthDisplay();
	}

	// Token: 0x060029BB RID: 10683 RVA: 0x000E17BC File Offset: 0x000DF9BC
	public GhostReactorShiftManager.State GetState()
	{
		return this.state;
	}

	// Token: 0x060029BC RID: 10684 RVA: 0x000E17C4 File Offset: 0x000DF9C4
	public bool IsSoaking()
	{
		return GhostReactorSoak.instance != null && GhostReactorSoak.instance.IsSoaking();
	}

	// Token: 0x060029BD RID: 10685 RVA: 0x000E17D9 File Offset: 0x000DF9D9
	private int GetPreShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDuration;
	}

	// Token: 0x060029BE RID: 10686 RVA: 0x000E17EB File Offset: 0x000DF9EB
	private int GetPreShiftDurationFirstArrive()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.preShiftDurationFirstArrive;
	}

	// Token: 0x060029BF RID: 10687 RVA: 0x000E17FD File Offset: 0x000DF9FD
	private int GetPostShiftDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.postShiftDuration;
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000E180F File Offset: 0x000DFA0F
	private int GetPreparingToDrillDuration()
	{
		this.IsSoaking();
		return 5;
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000E1819 File Offset: 0x000DFA19
	public int GetDrillingDuration()
	{
		if (this.IsSoaking())
		{
			return 5;
		}
		return this.drillDuration;
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x000E182C File Offset: 0x000DFA2C
	private void UpdateStateAuthority()
	{
		if (!this.grManager.IsAuthority())
		{
			return;
		}
		double time = PhotonNetwork.Time;
		switch (this.state)
		{
		case GhostReactorShiftManager.State.WaitingForConnect:
			if (this.reactor.grManager.IsZoneReady())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForFirstShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDuration())
			{
				this.reactor.grManager.RequestShiftStartAuthority(false);
				return;
			}
			break;
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
			if (time - this.stateStartTime > (double)this.GetPreShiftDurationFirstArrive())
			{
				this.reactor.grManager.RequestShiftStartAuthority(true);
				return;
			}
			break;
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
			if (time - this.stateStartTime > (double)this.GetPostShiftDuration())
			{
				if (this.authorizedToDelveDeeper)
				{
					this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.DelveDeeper);
					this.RequestState(GhostReactorShiftManager.State.PreparingToDrill);
					return;
				}
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
				return;
			}
			break;
		case GhostReactorShiftManager.State.PreparingToDrill:
			if (time - this.stateStartTime > (double)this.GetPreparingToDrillDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.Drilling);
				return;
			}
			break;
		case GhostReactorShiftManager.State.Drilling:
			if (time - this.stateStartTime > (double)this.GetDrillingDuration())
			{
				this.RequestState(GhostReactorShiftManager.State.WaitingForShiftStart);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000E194C File Offset: 0x000DFB4C
	private void UpdateStateShared()
	{
		double time = PhotonNetwork.Time;
		switch (this.state)
		{
		case GhostReactorShiftManager.State.WaitingForShiftStart:
		{
			int num = this.GetPreShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num = Mathf.Max(0, num);
			this.shiftTimerText.text = ":" + num.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.WaitingForFirstShiftStart:
		{
			int num2 = this.GetPreShiftDurationFirstArrive() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num2 = Mathf.Max(0, num2);
			this.shiftTimerText.text = ":" + num2.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.ReadyForShift:
		case GhostReactorShiftManager.State.ShiftActive:
			break;
		case GhostReactorShiftManager.State.PostShift:
		{
			int num3 = this.GetPostShiftDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num3 = Mathf.Max(0, num3);
			this.shiftTimerText.text = ":" + num3.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.PreparingToDrill:
		{
			int num4 = 5 - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num4 = Mathf.Max(0, num4);
			this.shiftTimerText.text = ":" + num4.ToString("D2");
			return;
		}
		case GhostReactorShiftManager.State.Drilling:
		{
			int num5 = this.GetDrillingDuration() - Mathf.FloorToInt((float)(time - this.stateStartTime));
			num5 = Mathf.Max(0, num5);
			this.shiftTimerText.text = ":" + num5.ToString("D2");
			this.UpdateLogoAnimations(this.depthDisplay.logoFrames);
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000E1AE0 File Offset: 0x000DFCE0
	public void RefreshDepthDisplay()
	{
		GhostReactorLevelGenConfig currLevelGenConfig = this.reactor.GetCurrLevelGenConfig();
		int num = this.reactor.GetDepthLevel() + 1;
		int num2 = num / 4 + 1 + ((num % 5 == 4) ? 2 : 0);
		this.shiftRewardCoresForMothership = currLevelGenConfig.coresRequired + num2;
		this.coresRequiredToDelveDeeper = ((currLevelGenConfig.coresRequired > 0) ? ((int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.coresRequired) + num2) : 0);
		this.killsRequiredToDelveDeeper = currLevelGenConfig.minEnemyKills;
		this.shiftRewardCredits = currLevelGenConfig.coresRequired * 5;
		this.sentientCoresRequiredToDelveDeeper = (int)(this.reactor.difficultyScalingForCurrentFloor * (float)currLevelGenConfig.sentientCoresRequired);
		this.shiftDurationMinutes = (float)(currLevelGenConfig.shiftDuration / 60);
		if (this.IsSoaking())
		{
			this.shiftDurationMinutes = (float)Random.Range(1, 3);
		}
		this.maxPlayerDeaths = currLevelGenConfig.maxPlayerDeaths;
		if (this.depthDisplay != null)
		{
			this.depthDisplay.RefreshDisplay();
		}
		this.RefreshShiftTimer();
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000E1BCD File Offset: 0x000DFDCD
	public void RefreshShiftLeaderboard()
	{
		if (this.nextRefreshLeaderboardSafety)
		{
			this.RefreshShiftLeaderboard_Safety();
		}
		else
		{
			this.RefreshShiftLeaderboard_Efficiency();
		}
		this.nextRefreshLeaderboardSafety = !this.nextRefreshLeaderboardSafety;
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000E1BF4 File Offset: 0x000DFDF4
	public void RefreshShiftLeaderboard_Safety()
	{
		if (this.shiftLeaderboardSafety == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>SAFETY          GHOSTS   WORKPLACE  TEAM    CHAOS\nREPORT          BANISHED INCIDENTS  ASSISTS EXPOSURE\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[4];
				int num2 = (int)grplayer.synchronizedSessionStats[5];
				int num3 = (int)grplayer.synchronizedSessionStats[6];
				float num4 = grplayer.synchronizedSessionStats[7];
				int num5 = (int)num4 / 60;
				int num6 = (int)num4 % 60;
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,5}{2,7}{3,7}{4,10}", new object[]
				{
					playerNameVisible,
					num2,
					num,
					num3,
					string.Format("{0,3}:{1:00}", num5, num6)
				}));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardSafety.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000E1D90 File Offset: 0x000DFF90
	public void RefreshShiftLeaderboard_Efficiency()
	{
		if (this.shiftLeaderboardEfficiency == null)
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		this.totalPlayTime = 0f;
		this.leaderboardDisplay.Clear();
		this.leaderboardDisplay.Append("<color=#c0c0c0c0><size=-0.4>KEY PERFORMANCE   CORES   EARNED   SPENT    DISTANCE\nINDICATORS        FOUND   CREDITS  CREDITS  TRAVELED\n----------------------------------------------------</size></color>\n");
		for (int i = 0; i < count; i++)
		{
			GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
			if (!(this.reactor.vrRigs[i] == null) && !(grplayer == null) && !(grplayer.gamePlayer == null))
			{
				string playerNameVisible = grplayer.gamePlayer.rig.playerNameVisible;
				int num = (int)grplayer.synchronizedSessionStats[0];
				int num2 = (int)grplayer.synchronizedSessionStats[1];
				int num3 = (int)grplayer.synchronizedSessionStats[2];
				int num4 = (int)grplayer.synchronizedSessionStats[3];
				this.leaderboardDisplay.Append((i % 2 == 0) ? "<color=#e0e0ff>" : "<color=#a0a0ff>");
				this.leaderboardDisplay.Append(string.Format("{0,-12}{1,6}{2,7}{3,7}{4,8}", new object[]
				{
					playerNameVisible,
					num,
					num2,
					num3,
					num4
				}));
				this.leaderboardDisplay.Append("</color>\n");
			}
		}
		this.shiftLeaderboardEfficiency.text = this.leaderboardDisplay.ToString();
	}

	// Token: 0x04003539 RID: 13625
	private const string EVENT_GOOD_KD = "GRShiftGoodKD";

	// Token: 0x0400353A RID: 13626
	[SerializeField]
	private GhostReactor reactor;

	// Token: 0x0400353B RID: 13627
	[SerializeField]
	private GRMetalEnergyGate frontGate;

	// Token: 0x0400353C RID: 13628
	[SerializeField]
	private GameObject startShiftButton;

	// Token: 0x0400353D RID: 13629
	[SerializeField]
	private TMP_Text shiftTimerText;

	// Token: 0x0400353E RID: 13630
	[SerializeField]
	private TMP_Text shiftStatsText;

	// Token: 0x0400353F RID: 13631
	[SerializeField]
	private TMP_Text shiftJugmentText;

	// Token: 0x04003540 RID: 13632
	[SerializeField]
	private TMP_Text reactorTextMain;

	// Token: 0x04003541 RID: 13633
	[SerializeField]
	private GameObject wrongStumpGoo;

	// Token: 0x04003542 RID: 13634
	[SerializeField]
	private float shiftDurationMinutes = 20f;

	// Token: 0x04003543 RID: 13635
	[SerializeField]
	private Transform playerTeleportTransform;

	// Token: 0x04003544 RID: 13636
	[SerializeField]
	private Transform gatePlaneTransform;

	// Token: 0x04003545 RID: 13637
	[SerializeField]
	private Transform gateBlockerTransform;

	// Token: 0x04003546 RID: 13638
	[SerializeField]
	private AudioSource anomalyLoop1;

	// Token: 0x04003547 RID: 13639
	[SerializeField]
	private AudioSource anomalyLoop2;

	// Token: 0x04003548 RID: 13640
	[SerializeField]
	private AudioSource anomalyLoop3;

	// Token: 0x04003549 RID: 13641
	[SerializeField]
	private AudioSource anomalyAlert;

	// Token: 0x0400354A RID: 13642
	[SerializeField]
	private float anomalyAlertCountdownTimeToStartPlayingInMinutes = 3f;

	// Token: 0x0400354B RID: 13643
	[SerializeField]
	private float roomCloseTimeSeconds = 60f;

	// Token: 0x0400354C RID: 13644
	private bool isRoomClosed;

	// Token: 0x0400354D RID: 13645
	[SerializeField]
	private int preShiftDuration = 10;

	// Token: 0x0400354E RID: 13646
	private int preShiftDurationFirstArrive = 60;

	// Token: 0x0400354F RID: 13647
	private int postShiftDuration = 10;

	// Token: 0x04003550 RID: 13648
	[SerializeField]
	public int drillDuration = 50;

	// Token: 0x04003551 RID: 13649
	private bool bIsStartingFloorAuthorityOnly;

	// Token: 0x04003552 RID: 13650
	[Header("Drill Announcements")]
	[SerializeField]
	private AudioSource announceAudioSource;

	// Token: 0x04003553 RID: 13651
	[SerializeField]
	private AudioSource announceBellAudioSource;

	// Token: 0x04003554 RID: 13652
	public AbilitySound announcePrepareShift;

	// Token: 0x04003555 RID: 13653
	public AbilitySound announceStartShift;

	// Token: 0x04003556 RID: 13654
	public AbilitySound announceCompleteShift;

	// Token: 0x04003557 RID: 13655
	public AbilitySound announceFailShift;

	// Token: 0x04003558 RID: 13656
	public AbilitySound announcePrepareDrill;

	// Token: 0x04003559 RID: 13657
	public AbilitySound announceTip;

	// Token: 0x0400355A RID: 13658
	public AbilitySound announceBell;

	// Token: 0x0400355B RID: 13659
	[Header("Warning")]
	public List<GhostReactorShiftManager.WarningPres> warnings;

	// Token: 0x0400355C RID: 13660
	[SerializeField]
	private AudioClip warningAudio;

	// Token: 0x0400355D RID: 13661
	[SerializeField]
	[Tooltip("Must be ordered from largest time (first played) to smallest time (last played)")]
	private List<int> warningClipPlayTimes = new List<int>();

	// Token: 0x0400355E RID: 13662
	[Header("Ring")]
	[SerializeField]
	private Transform ringTransform;

	// Token: 0x0400355F RID: 13663
	[SerializeField]
	private float ringClosingDuration = 3f;

	// Token: 0x04003560 RID: 13664
	[SerializeField]
	private float ringClosingMaxRadius = 100f;

	// Token: 0x04003561 RID: 13665
	[SerializeField]
	private float ringClosingMinRadius = 7f;

	// Token: 0x04003562 RID: 13666
	[Header("Debug")]
	[SerializeField]
	private float debugFastForwardRate = 30f;

	// Token: 0x04003563 RID: 13667
	[SerializeField]
	private bool debugFastForwarding;

	// Token: 0x04003564 RID: 13668
	private bool shiftStarted;

	// Token: 0x04003565 RID: 13669
	private bool shiftJustStarted;

	// Token: 0x04003566 RID: 13670
	private double shiftStartNetworkTime;

	// Token: 0x04003567 RID: 13671
	private double shiftEndNetworkTime;

	// Token: 0x04003568 RID: 13672
	private float prevCountDownTotal;

	// Token: 0x04003569 RID: 13673
	[SerializeField]
	private int shiftTotalEarned = -1;

	// Token: 0x0400356A RID: 13674
	[SerializeField]
	private int shiftSanityMaximumEarned = 10000;

	// Token: 0x0400356B RID: 13675
	public GhostReactorShiftDepthDisplay depthDisplay;

	// Token: 0x0400356C RID: 13676
	public bool authorizedToDelveDeeper;

	// Token: 0x0400356D RID: 13677
	public int shiftRewardCoresForMothership;

	// Token: 0x0400356E RID: 13678
	public int coresRequiredToDelveDeeper;

	// Token: 0x0400356F RID: 13679
	public int sentientCoresRequiredToDelveDeeper;

	// Token: 0x04003570 RID: 13680
	public List<GREnemyCount> killsRequiredToDelveDeeper;

	// Token: 0x04003571 RID: 13681
	public int maxPlayerDeaths;

	// Token: 0x04003572 RID: 13682
	public int shiftRewardCredits;

	// Token: 0x04003573 RID: 13683
	private bool localPlayerInside;

	// Token: 0x04003574 RID: 13684
	private bool localPlayerOverlapping;

	// Token: 0x04003575 RID: 13685
	private float totalPlayTime;

	// Token: 0x04003576 RID: 13686
	private string gameIdGuid = "";

	// Token: 0x04003577 RID: 13687
	public GRShiftStat shiftStats = new GRShiftStat();

	// Token: 0x04003578 RID: 13688
	[NonSerialized]
	private GhostReactorManager grManager;

	// Token: 0x04003579 RID: 13689
	[SerializeField]
	private TMP_Text shiftLeaderboardEfficiency;

	// Token: 0x0400357A RID: 13690
	[SerializeField]
	private TMP_Text shiftLeaderboardSafety;

	// Token: 0x0400357B RID: 13691
	private double lastLeaderboardRefreshTime;

	// Token: 0x0400357C RID: 13692
	private float leaderboardUpdateFrequency = 0.5f;

	// Token: 0x0400357D RID: 13693
	private GhostReactorShiftManager.State state;

	// Token: 0x0400357E RID: 13694
	public double stateStartTime;

	// Token: 0x0400357F RID: 13695
	private double lastReactorLogoAnimationTime;

	// Token: 0x04003580 RID: 13696
	private int lastReactorLogoAnimFrame;

	// Token: 0x04003581 RID: 13697
	private bool isPlayingLogoAnimation;

	// Token: 0x04003582 RID: 13698
	private double lastReactorDisplayUpdate;

	// Token: 0x04003583 RID: 13699
	private StringBuilder cachedStringBuilder = new StringBuilder(256);

	// Token: 0x04003584 RID: 13700
	private bool nextRefreshLeaderboardSafety;

	// Token: 0x04003585 RID: 13701
	private StringBuilder leaderboardDisplay = new StringBuilder(1024);

	// Token: 0x02000657 RID: 1623
	[Serializable]
	public class WarningPres
	{
		// Token: 0x04003586 RID: 13702
		public int time;

		// Token: 0x04003587 RID: 13703
		public AbilitySound sound;
	}

	// Token: 0x02000658 RID: 1624
	public enum State
	{
		// Token: 0x04003589 RID: 13705
		WaitingForConnect,
		// Token: 0x0400358A RID: 13706
		WaitingForShiftStart,
		// Token: 0x0400358B RID: 13707
		WaitingForFirstShiftStart,
		// Token: 0x0400358C RID: 13708
		ReadyForShift,
		// Token: 0x0400358D RID: 13709
		ShiftActive,
		// Token: 0x0400358E RID: 13710
		PostShift,
		// Token: 0x0400358F RID: 13711
		PreparingToDrill,
		// Token: 0x04003590 RID: 13712
		Drilling
	}
}
