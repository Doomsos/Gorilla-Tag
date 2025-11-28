using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000D9F RID: 3487
	[NetworkBehaviourWeaved(210)]
	public class WhackAMole : NetworkComponent
	{
		// Token: 0x0600559A RID: 21914 RVA: 0x001AEB14 File Offset: 0x001ACD14
		private void UpdateMeshRendererList()
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			ZoneBasedObject[] array = this.zoneBasedVisuals;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>(true))
				{
					if (meshRenderer.enabled)
					{
						list.Add(meshRenderer);
					}
				}
			}
			this.zoneBasedMeshRenderers = list.ToArray();
		}

		// Token: 0x0600559B RID: 21915 RVA: 0x001AEB7C File Offset: 0x001ACD7C
		protected override void Awake()
		{
			base.Awake();
			if (this.molesContainerRight != null)
			{
				this.rightMolesList = new List<Mole>(this.molesContainerRight.GetComponentsInChildren<Mole>());
				if (this.rightMolesList.Count > 0)
				{
					this.molesList.AddRange(this.rightMolesList);
				}
			}
			if (this.molesContainerLeft != null)
			{
				this.leftMolesList = new List<Mole>(this.molesContainerLeft.GetComponentsInChildren<Mole>());
				if (this.leftMolesList.Count > 0)
				{
					this.molesList.AddRange(this.leftMolesList);
					foreach (Mole mole in this.leftMolesList)
					{
						mole.IsLeftSideMole = true;
					}
				}
			}
			this.currentLevelIndex = -1;
			foreach (Mole mole2 in this.molesList)
			{
				mole2.OnTapped += this.OnMoleTapped;
			}
			List<Mole> list = this.leftMolesList;
			bool flag;
			if (list != null && list.Count > 0)
			{
				list = this.rightMolesList;
				flag = (list != null && list.Count > 0);
			}
			else
			{
				flag = false;
			}
			this.isMultiplayer = flag;
			this.welcomeUI.SetActive(false);
			this.ongoingGameUI.SetActive(false);
			this.levelEndedUI.SetActive(false);
			this.ContinuePressedUI.SetActive(false);
			this.multiplyareScoresUI.SetActive(false);
			this.bestScore = 0;
			this.bestScoreText.text = string.Empty;
			this.highScorePlayerName = string.Empty;
			this.victoryParticles = this.victoryFX.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x0600559C RID: 21916 RVA: 0x001AED4C File Offset: 0x001ACF4C
		protected override void Start()
		{
			base.Start();
			this.SwitchState(WhackAMole.GameState.Off);
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Register(this);
			}
		}

		// Token: 0x0600559D RID: 21917 RVA: 0x001AED74 File Offset: 0x001ACF74
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (Mole mole in this.molesList)
			{
				mole.OnTapped -= this.OnMoleTapped;
			}
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Unregister(this);
			}
			this.molesList.Clear();
		}

		// Token: 0x0600559E RID: 21918 RVA: 0x001AEDF8 File Offset: 0x001ACFF8
		public void InvokeUpdate()
		{
			bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
			bool flag = this.zoneBasedVisuals[0].IsLocalPlayerInZone();
			if (isMasterClient != this.wasMasterClient || flag != this.wasLocalPlayerInZone)
			{
				MeshRenderer[] array = this.zoneBasedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = flag;
				}
				bool active = isMasterClient || flag;
				ZoneBasedObject[] array2 = this.zoneBasedVisuals;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(active);
				}
				this.wasMasterClient = isMasterClient;
				this.wasLocalPlayerInZone = flag;
			}
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x001AEE90 File Offset: 0x001AD090
		private void SwitchState(WhackAMole.GameState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.ResetGame();
				this.currentLevelIndex = -1;
				this.currentLevel = null;
				this.UpdateLevelUI(1);
				break;
			case WhackAMole.GameState.ContinuePressed:
				this.continuePressedTime = Time.time;
				this.audioSource.GTStop();
				this.audioSource.GTPlayOneShot(this.counterClip, 1f);
				if (base.IsMine)
				{
					this.pickedMolesIndex.Clear();
				}
				this.ResetGame();
				if (base.IsMine)
				{
					this.LoadNextLevel();
				}
				break;
			case WhackAMole.GameState.Ongoing:
				this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
				break;
			case WhackAMole.GameState.TimesUp:
				if (this.currentLevel != null)
				{
					foreach (Mole mole in this.molesList)
					{
						mole.HideMole(false);
					}
					this.curentGameResult = this.GetGameResult();
					this.UpdateResultUI(this.curentGameResult);
					this.levelEndedTotalScoreText.text = "SCORE " + this.totalScore.ToString();
					this.levelEndedCurrentScoreText.text = string.Format("{0}/{1}", this.currentScore, this.currentLevel.GetMinScore(this.isMultiplayer));
					if (this.totalScore > this.bestScore)
					{
						this.bestScore = this.totalScore;
						this.highScorePlayerName = this.playerName;
					}
					this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
					this.audioSource.GTStop();
					if (this.curentGameResult == WhackAMole.GameResult.LevelComplete)
					{
						this.audioSource.GTPlayOneShot(this.levelCompleteClip, 1f);
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					else if (this.curentGameResult == WhackAMole.GameResult.GameOver)
					{
						this.audioSource.GTPlayOneShot(this.gameOverClip, 1f);
					}
					else if (this.curentGameResult == WhackAMole.GameResult.Win)
					{
						this.audioSource.GTPlayOneShot(this.winClip, 1f);
						if (this.victoryFX)
						{
							ParticleSystem[] array = this.victoryParticles;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].Play();
							}
						}
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					int minScore = this.currentLevel.GetMinScore(this.isMultiplayer);
					if (this.levelGoodMolesPicked < minScore)
					{
						GTDev.LogError<string>(string.Format("[WAM] Lvl:{0} Only Picked {1}/{2} good moles!", this.currentLevel.levelNumber, this.levelGoodMolesPicked, minScore), null);
					}
					if (base.IsMine)
					{
						GorillaTelemetry.WamLevelEnd(this.playerId, this.gameId, this.machineId, this.currentLevel.levelNumber, this.levelGoodMolesPicked, this.levelHazardMolesPicked, minScore, this.currentScore, this.levelHazardMolesHit, this.curentGameResult.ToString());
					}
				}
				break;
			}
			this.UpdateScreenData();
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x001AF258 File Offset: 0x001AD458
		private void UpdateScreenData()
		{
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.welcomeUI.SetActive(true);
				this.ContinuePressedUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.levelEndedUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				return;
			case WhackAMole.GameState.ContinuePressed:
				this.levelEndedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				this.ContinuePressedUI.SetActive(true);
				break;
			case WhackAMole.GameState.Ongoing:
				this.ContinuePressedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(true);
				this.levelEndedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
					return;
				}
				break;
			case WhackAMole.GameState.PickMoles:
				break;
			case WhackAMole.GameState.TimesUp:
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.ContinuePressedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
				}
				this.levelEndedUI.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060055A1 RID: 21921 RVA: 0x001AF390 File Offset: 0x001AD590
		public static int CreateNewGameID()
		{
			int num = (int)((DateTime.Now - WhackAMole.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
			if (num <= WhackAMole.lastAssignedID)
			{
				WhackAMole.lastAssignedID++;
				return WhackAMole.lastAssignedID;
			}
			WhackAMole.lastAssignedID = num;
			return num;
		}

		// Token: 0x060055A2 RID: 21922 RVA: 0x001AF3F0 File Offset: 0x001AD5F0
		private void OnMoleTapped(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeftHand)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.Off || gameState == WhackAMole.GameState.TimesUp)
			{
				return;
			}
			AudioClip clip = moleType.isHazard ? this.whackHazardClips[Random.Range(0, this.whackHazardClips.Length)] : this.whackMonkeClips[Random.Range(0, this.whackMonkeClips.Length)];
			if (moleType.isHazard)
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
				this.levelHazardMolesHit++;
			}
			else
			{
				this.audioSource.GTPlayOneShot(clip, 1f);
			}
			if (moleType.monkeMoleHitMaterial != null)
			{
				moleType.MeshRenderer.material = moleType.monkeMoleHitMaterial;
			}
			this.currentScore += moleType.scorePoint;
			this.totalScore += moleType.scorePoint;
			if (moleType.IsLeftSideMoleType)
			{
				this.leftPlayerScore += moleType.scorePoint;
			}
			else
			{
				this.rightPlayerScore += moleType.scorePoint;
			}
			this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
			moleType.MoleContainerParent.HideMole(true);
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x001AF514 File Offset: 0x001AD714
		public void HandleOnTimerStopped()
		{
			this.gameEndedTime = Time.time;
			this.SwitchState(WhackAMole.GameState.TimesUp);
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x001AF528 File Offset: 0x001AD728
		private IEnumerator PlayHazardAudio(AudioClip clip)
		{
			this.audioSource.clip = clip;
			this.audioSource.GTPlay();
			yield return new WaitForSeconds(this.audioSource.clip.length);
			this.audioSource.clip = this.errorClip;
			this.audioSource.GTPlay();
			yield break;
		}

		// Token: 0x060055A5 RID: 21925 RVA: 0x001AF540 File Offset: 0x001AD740
		private bool PickMoles()
		{
			WhackAMole.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.pickedMolesIndex.Clear();
			float passedTime = this.timer.GetPassedTime();
			if (passedTime > this.currentLevel.levelDuration - this.currentLevel.showMoleDuration)
			{
				return true;
			}
			float num = passedTime / this.currentLevel.levelDuration;
			CS$<>8__locals1.minMoleCount = Mathf.Lerp(this.currentLevel.minimumMoleCount.x, this.currentLevel.minimumMoleCount.y, num);
			CS$<>8__locals1.maxMoleCount = Mathf.Lerp(this.currentLevel.maximumMoleCount.x, this.currentLevel.maximumMoleCount.y, num);
			this.curentTime = Time.time;
			CS$<>8__locals1.hazardMoleChance = Mathf.Lerp(this.currentLevel.hazardMoleChance.x, this.currentLevel.hazardMoleChance.y, num);
			if (this.isMultiplayer)
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.rightMolesList, ref CS$<>8__locals1);
				this.<PickMoles>g__PickMolesFrom|85_0(this.leftMolesList, ref CS$<>8__locals1);
			}
			else
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.molesList, ref CS$<>8__locals1);
			}
			return this.pickedMolesIndex.Count != 0;
		}

		// Token: 0x060055A6 RID: 21926 RVA: 0x001AF66C File Offset: 0x001AD86C
		private void LoadNextLevel()
		{
			if (this.currentLevel != null)
			{
				this.resetToFirstLevel = (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer));
				if (this.resetToFirstLevel)
				{
					this.currentLevelIndex = 0;
				}
				else
				{
					this.currentLevelIndex++;
				}
				if (this.currentLevelIndex >= this.allLevels.Length)
				{
					this.currentLevelIndex = 0;
				}
			}
			else
			{
				this.currentLevelIndex++;
			}
			this.currentLevel = this.allLevels[this.currentLevelIndex];
			this.timer.SetTimerDuration(this.currentLevel.levelDuration);
			this.timer.RestartTimer();
			this.curentTime = Time.time;
			this.currentScore = 0;
			this.leftPlayerScore = 0;
			this.rightPlayerScore = 0;
			this.levelGoodMolesPicked = (this.levelHazardMolesPicked = 0);
			this.levelHazardMolesHit = 0;
			if (this.currentLevelIndex == 0)
			{
				this.totalScore = 0;
			}
			if (this.currentLevelIndex == 0 && base.IsMine)
			{
				this.gameId = WhackAMole.CreateNewGameID();
				Debug.LogWarning("GAME ID" + this.gameId.ToString());
			}
		}

		// Token: 0x060055A7 RID: 21927 RVA: 0x001AF79C File Offset: 0x001AD99C
		private bool PickSingleMole(int randomMoleIndex, float hazardMoleChance)
		{
			bool flag = hazardMoleChance > 0f && Random.value <= hazardMoleChance;
			int moleTypeIndex = this.molesList[randomMoleIndex].GetMoleTypeIndex(flag);
			this.molesList[randomMoleIndex].ShowMole(this.currentLevel.showMoleDuration, moleTypeIndex);
			this.pickedMolesIndex.Add(randomMoleIndex, moleTypeIndex);
			if (flag)
			{
				this.levelHazardMolesPicked++;
			}
			else
			{
				this.levelGoodMolesPicked++;
			}
			return flag;
		}

		// Token: 0x060055A8 RID: 21928 RVA: 0x001AF820 File Offset: 0x001ADA20
		private void ResetGame()
		{
			foreach (Mole mole in this.molesList)
			{
				mole.ResetPosition();
			}
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x001AF870 File Offset: 0x001ADA70
		private void UpdateScoreUI(int totalScore, int _leftPlayerScore, int _rightPlayerScore)
		{
			if (this.currentLevel != null)
			{
				this.scoreText.text = string.Format("SCORE\n{0}/{1}", totalScore, this.currentLevel.GetMinScore(this.isMultiplayer));
				this.leftPlayerScoreText.text = _leftPlayerScore.ToString();
				this.rightPlayerScoreText.text = _rightPlayerScore.ToString();
			}
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x001AF8E0 File Offset: 0x001ADAE0
		private void UpdateLevelUI(int levelNumber)
		{
			this.arrowTargetRotation = Quaternion.Euler(0f, 0f, (float)(18 * (levelNumber - 1)));
			this.arrowRotationNeedsUpdate = true;
		}

		// Token: 0x060055AB RID: 21931 RVA: 0x001AF908 File Offset: 0x001ADB08
		private void UpdateArrowRotation()
		{
			Quaternion quaternion = Quaternion.Slerp(this.levelArrow.transform.localRotation, this.arrowTargetRotation, Time.deltaTime * 5f);
			if (Quaternion.Angle(quaternion, this.arrowTargetRotation) < 0.1f)
			{
				quaternion = this.arrowTargetRotation;
				this.arrowRotationNeedsUpdate = false;
			}
			this.levelArrow.transform.localRotation = quaternion;
		}

		// Token: 0x060055AC RID: 21932 RVA: 0x001AF96E File Offset: 0x001ADB6E
		private void UpdateTimerUI(int time)
		{
			if (time == this.previousTime)
			{
				return;
			}
			this.timeText.text = "TIME " + time.ToString();
			this.previousTime = time;
		}

		// Token: 0x060055AD RID: 21933 RVA: 0x001AF99D File Offset: 0x001ADB9D
		private void UpdateResultUI(WhackAMole.GameResult gameResult)
		{
			if (gameResult == WhackAMole.GameResult.LevelComplete)
			{
				this.resultText.text = "LEVEL COMPLETE";
				return;
			}
			if (gameResult == WhackAMole.GameResult.Win)
			{
				this.resultText.text = "YOU WIN!";
				return;
			}
			if (gameResult == WhackAMole.GameResult.GameOver)
			{
				this.resultText.text = "GAME OVER";
			}
		}

		// Token: 0x060055AE RID: 21934 RVA: 0x001AF9DC File Offset: 0x001ADBDC
		public void OnStartButtonPressed()
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.TimesUp || gameState == WhackAMole.GameState.Off)
			{
				base.GetView.RPC("WhackAMoleButtonPressed", 0, Array.Empty<object>());
			}
		}

		// Token: 0x060055AF RID: 21935 RVA: 0x001AFA0D File Offset: 0x001ADC0D
		[PunRPC]
		private void WhackAMoleButtonPressed(PhotonMessageInfo info)
		{
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x060055B0 RID: 21936 RVA: 0x001AFA1C File Offset: 0x001ADC1C
		[Rpc]
		private unsafe void RPC_WhackAMoleButtonPressed(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != 4)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", num);
						}
						else
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								ptr.Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 7) != 0)
							{
								info = RpcInfo.FromLocal(base.Runner, 0, 0);
								goto IL_12;
							}
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_12:
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x060055B1 RID: 21937 RVA: 0x001AFB5C File Offset: 0x001ADD5C
		private void WhackAMoleButtonPressedShared(PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "WhackAMoleButtonPressedShared");
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (vrrig)
			{
				this.playerName = vrrig.playerNameVisible;
				if (this.currentState == WhackAMole.GameState.Off)
				{
					this.playerId = info.Sender.UserId;
					if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
					{
						PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
					}
				}
			}
			this.SwitchState(WhackAMole.GameState.ContinuePressed);
		}

		// Token: 0x060055B2 RID: 21938 RVA: 0x001AFBDB File Offset: 0x001ADDDB
		private WhackAMole.GameResult GetGameResult()
		{
			if (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer))
			{
				return WhackAMole.GameResult.GameOver;
			}
			if (this.currentLevelIndex >= this.allLevels.Length - 1)
			{
				return WhackAMole.GameResult.Win;
			}
			return WhackAMole.GameResult.LevelComplete;
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x001AFC0D File Offset: 0x001ADE0D
		public int GetCurrentLevel()
		{
			if (this.currentLevel != null)
			{
				return this.currentLevel.levelNumber;
			}
			return 0;
		}

		// Token: 0x060055B4 RID: 21940 RVA: 0x001AFC2A File Offset: 0x001ADE2A
		public int GetTotalLevelNumbers()
		{
			if (this.allLevels != null)
			{
				return this.allLevels.Length;
			}
			return 0;
		}

		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x060055B5 RID: 21941 RVA: 0x001AFC3E File Offset: 0x001ADE3E
		// (set) Token: 0x060055B6 RID: 21942 RVA: 0x001AFC68 File Offset: 0x001ADE68
		[Networked]
		[NetworkedWeaved(0, 210)]
		public unsafe WhackAMole.WhackAMoleData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(WhackAMole.WhackAMoleData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(WhackAMole.WhackAMoleData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x060055B7 RID: 21943 RVA: 0x001AFC94 File Offset: 0x001ADE94
		public override void WriteDataFusion()
		{
			this.Data = new WhackAMole.WhackAMoleData(this.currentState, this.currentLevelIndex, this.currentScore, this.totalScore, this.bestScore, this.rightPlayerScore, this.highScorePlayerName, this.timer.GetRemainingTime(), this.gameEndedTime, this.gameId, this.pickedMolesIndex);
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x060055B8 RID: 21944 RVA: 0x001AFD00 File Offset: 0x001ADF00
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentLevelIndex, this.Data.CurrentScore, this.Data.TotalScore, this.Data.BestScore, this.Data.RightPlayerScore, this.Data.HighScorePlayerName.Value, this.Data.RemainingTime, this.Data.GameEndedTime, this.Data.GameId);
			for (int i = 0; i < this.Data.PickedMolesIndexCount; i++)
			{
				int randomMoleTypeIndex = this.Data.PickedMolesIndex[i];
				if (i >= 0 && i < this.molesList.Count && this.currentLevel)
				{
					this.molesList[i].ShowMole(this.currentLevel.showMoleDuration, randomMoleTypeIndex);
				}
			}
		}

		// Token: 0x060055B9 RID: 21945 RVA: 0x001AFE18 File Offset: 0x001AE018
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x060055BA RID: 21946 RVA: 0x001AFE28 File Offset: 0x001AE028
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x060055BB RID: 21947 RVA: 0x001AFE38 File Offset: 0x001AE038
		private void ReadDataShared(WhackAMole.GameState _currentState, int _currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float _remainingTime, float endedTime, int _gameId)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (_currentState != gameState)
			{
				this.SwitchState(_currentState);
			}
			this.currentLevelIndex = _currentLevelIndex;
			if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
			{
				this.currentLevel = this.allLevels[this.currentLevelIndex];
				this.UpdateLevelUI(this.currentLevel.levelNumber);
			}
			this.currentScore = cScore;
			this.totalScore = tScore;
			this.bestScore = bScore;
			this.rightPlayerScore = rPScore;
			this.leftPlayerScore = this.currentScore - this.rightPlayerScore;
			this.highScorePlayerName = hScorePName;
			this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
			this.remainingTime = _remainingTime;
			if (float.IsFinite(this.remainingTime) && this.currentLevel)
			{
				this.remainingTime = this.remainingTime.ClampSafe(0f, this.currentLevel.levelDuration);
				this.UpdateTimerUI((int)this.remainingTime);
			}
			if (float.IsFinite(endedTime))
			{
				this.gameEndedTime = endedTime.ClampSafe(0f, Time.time);
			}
			this.gameId = _gameId;
		}

		// Token: 0x060055BC RID: 21948 RVA: 0x001AFF8C File Offset: 0x001AE18C
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.timer.RestartTimer();
				this.timer.SetTimerDuration(this.remainingTime);
				this.curentTime = Time.time;
				if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
				{
					this.currentLevel = this.allLevels[this.currentLevelIndex];
				}
				this.SwitchState(this.currentState);
			}
		}

		// Token: 0x060055BF RID: 21951 RVA: 0x001B0090 File Offset: 0x001AE290
		[CompilerGenerated]
		private void <PickMoles>g__PickMolesFrom|85_0(List<Mole> moles, ref WhackAMole.<>c__DisplayClass85_0 A_2)
		{
			int num = Mathf.RoundToInt(Random.Range(A_2.minMoleCount, A_2.maxMoleCount));
			this.potentialMoles.Clear();
			foreach (Mole mole in moles)
			{
				if (mole.CanPickMole())
				{
					this.potentialMoles.Add(mole);
				}
			}
			int num2 = Mathf.Min(num, this.potentialMoles.Count);
			int num3 = Mathf.CeilToInt((float)num2 * A_2.hazardMoleChance);
			int num4 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num5 = Random.Range(0, this.potentialMoles.Count);
				if (this.PickSingleMole(this.molesList.IndexOf(this.potentialMoles[num5]), (num4 < num3) ? A_2.hazardMoleChance : 0f))
				{
					num4++;
				}
				this.potentialMoles.RemoveAt(num5);
			}
		}

		// Token: 0x060055C0 RID: 21952 RVA: 0x001B019C File Offset: 0x001AE39C
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x060055C1 RID: 21953 RVA: 0x001B01B4 File Offset: 0x001AE3B4
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x060055C2 RID: 21954 RVA: 0x001B01C8 File Offset: 0x001AE3C8
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_WhackAMoleButtonPressed@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
			behaviour.InvokeRpc = true;
			((WhackAMole)behaviour).RPC_WhackAMoleButtonPressed(info);
		}

		// Token: 0x0400629F RID: 25247
		public string machineId = "default";

		// Token: 0x040062A0 RID: 25248
		public GameObject molesContainerRight;

		// Token: 0x040062A1 RID: 25249
		[Tooltip("Only for co-op version")]
		public GameObject molesContainerLeft;

		// Token: 0x040062A2 RID: 25250
		public int betweenLevelPauseDuration = 3;

		// Token: 0x040062A3 RID: 25251
		public int countdownDuration = 5;

		// Token: 0x040062A4 RID: 25252
		public WhackAMoleLevelSO[] allLevels;

		// Token: 0x040062A5 RID: 25253
		[SerializeField]
		private GorillaTimer timer;

		// Token: 0x040062A6 RID: 25254
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040062A7 RID: 25255
		public GameObject levelArrow;

		// Token: 0x040062A8 RID: 25256
		public GameObject victoryFX;

		// Token: 0x040062A9 RID: 25257
		public ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x040062AA RID: 25258
		[SerializeField]
		private MeshRenderer[] zoneBasedMeshRenderers;

		// Token: 0x040062AB RID: 25259
		[Space]
		public AudioClip backgroundLoop;

		// Token: 0x040062AC RID: 25260
		public AudioClip errorClip;

		// Token: 0x040062AD RID: 25261
		public AudioClip counterClip;

		// Token: 0x040062AE RID: 25262
		public AudioClip levelCompleteClip;

		// Token: 0x040062AF RID: 25263
		public AudioClip winClip;

		// Token: 0x040062B0 RID: 25264
		public AudioClip gameOverClip;

		// Token: 0x040062B1 RID: 25265
		public AudioClip[] whackHazardClips;

		// Token: 0x040062B2 RID: 25266
		public AudioClip[] whackMonkeClips;

		// Token: 0x040062B3 RID: 25267
		[Space]
		public GameObject welcomeUI;

		// Token: 0x040062B4 RID: 25268
		public GameObject ongoingGameUI;

		// Token: 0x040062B5 RID: 25269
		public GameObject levelEndedUI;

		// Token: 0x040062B6 RID: 25270
		public GameObject ContinuePressedUI;

		// Token: 0x040062B7 RID: 25271
		public GameObject multiplyareScoresUI;

		// Token: 0x040062B8 RID: 25272
		[Space]
		public TextMeshPro scoreText;

		// Token: 0x040062B9 RID: 25273
		public TextMeshPro bestScoreText;

		// Token: 0x040062BA RID: 25274
		[Tooltip("Only for co-op version")]
		public TextMeshPro rightPlayerScoreText;

		// Token: 0x040062BB RID: 25275
		[Tooltip("Only for co-op version")]
		public TextMeshPro leftPlayerScoreText;

		// Token: 0x040062BC RID: 25276
		public TextMeshPro timeText;

		// Token: 0x040062BD RID: 25277
		public TextMeshPro counterText;

		// Token: 0x040062BE RID: 25278
		public TextMeshPro resultText;

		// Token: 0x040062BF RID: 25279
		public TextMeshPro levelEndedOptionsText;

		// Token: 0x040062C0 RID: 25280
		public TextMeshPro levelEndedCountdownText;

		// Token: 0x040062C1 RID: 25281
		public TextMeshPro levelEndedTotalScoreText;

		// Token: 0x040062C2 RID: 25282
		public TextMeshPro levelEndedCurrentScoreText;

		// Token: 0x040062C3 RID: 25283
		private List<Mole> rightMolesList;

		// Token: 0x040062C4 RID: 25284
		private List<Mole> leftMolesList;

		// Token: 0x040062C5 RID: 25285
		private List<Mole> molesList = new List<Mole>();

		// Token: 0x040062C6 RID: 25286
		private WhackAMoleLevelSO currentLevel;

		// Token: 0x040062C7 RID: 25287
		private int currentScore;

		// Token: 0x040062C8 RID: 25288
		private int totalScore;

		// Token: 0x040062C9 RID: 25289
		private int leftPlayerScore;

		// Token: 0x040062CA RID: 25290
		private int rightPlayerScore;

		// Token: 0x040062CB RID: 25291
		private int bestScore;

		// Token: 0x040062CC RID: 25292
		private float curentTime;

		// Token: 0x040062CD RID: 25293
		private int currentLevelIndex;

		// Token: 0x040062CE RID: 25294
		private float continuePressedTime;

		// Token: 0x040062CF RID: 25295
		private bool resetToFirstLevel;

		// Token: 0x040062D0 RID: 25296
		private Quaternion arrowTargetRotation;

		// Token: 0x040062D1 RID: 25297
		private bool arrowRotationNeedsUpdate;

		// Token: 0x040062D2 RID: 25298
		private List<Mole> potentialMoles = new List<Mole>();

		// Token: 0x040062D3 RID: 25299
		private Dictionary<int, int> pickedMolesIndex = new Dictionary<int, int>();

		// Token: 0x040062D4 RID: 25300
		private WhackAMole.GameState currentState;

		// Token: 0x040062D5 RID: 25301
		private WhackAMole.GameState lastState;

		// Token: 0x040062D6 RID: 25302
		private float remainingTime;

		// Token: 0x040062D7 RID: 25303
		private int previousTime = -1;

		// Token: 0x040062D8 RID: 25304
		private bool isMultiplayer;

		// Token: 0x040062D9 RID: 25305
		private float gameEndedTime;

		// Token: 0x040062DA RID: 25306
		private WhackAMole.GameResult curentGameResult;

		// Token: 0x040062DB RID: 25307
		private string playerName = string.Empty;

		// Token: 0x040062DC RID: 25308
		private string highScorePlayerName = string.Empty;

		// Token: 0x040062DD RID: 25309
		private ParticleSystem[] victoryParticles;

		// Token: 0x040062DE RID: 25310
		private int levelHazardMolesPicked;

		// Token: 0x040062DF RID: 25311
		private int levelGoodMolesPicked;

		// Token: 0x040062E0 RID: 25312
		private string playerId;

		// Token: 0x040062E1 RID: 25313
		private int gameId;

		// Token: 0x040062E2 RID: 25314
		private int levelHazardMolesHit;

		// Token: 0x040062E3 RID: 25315
		private static DateTime epoch = new DateTime(2024, 1, 1);

		// Token: 0x040062E4 RID: 25316
		private static int lastAssignedID;

		// Token: 0x040062E5 RID: 25317
		private bool wasMasterClient;

		// Token: 0x040062E6 RID: 25318
		private bool wasLocalPlayerInZone = true;

		// Token: 0x040062E7 RID: 25319
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 210)]
		[DrawIf("IsEditorWritable", true, 0, 0)]
		private WhackAMole.WhackAMoleData _Data;

		// Token: 0x02000DA0 RID: 3488
		public enum GameState
		{
			// Token: 0x040062E9 RID: 25321
			Off,
			// Token: 0x040062EA RID: 25322
			ContinuePressed,
			// Token: 0x040062EB RID: 25323
			Ongoing,
			// Token: 0x040062EC RID: 25324
			PickMoles,
			// Token: 0x040062ED RID: 25325
			TimesUp,
			// Token: 0x040062EE RID: 25326
			LevelStarted
		}

		// Token: 0x02000DA1 RID: 3489
		private enum GameResult
		{
			// Token: 0x040062F0 RID: 25328
			GameOver,
			// Token: 0x040062F1 RID: 25329
			Win,
			// Token: 0x040062F2 RID: 25330
			LevelComplete,
			// Token: 0x040062F3 RID: 25331
			Unknown
		}

		// Token: 0x02000DA2 RID: 3490
		[NetworkStructWeaved(210)]
		[StructLayout(2, Size = 840)]
		public struct WhackAMoleData : INetworkStruct
		{
			// Token: 0x17000827 RID: 2087
			// (get) Token: 0x060055C3 RID: 21955 RVA: 0x001B020C File Offset: 0x001AE40C
			// (set) Token: 0x060055C4 RID: 21956 RVA: 0x001B0214 File Offset: 0x001AE414
			public WhackAMole.GameState CurrentState { readonly get; set; }

			// Token: 0x17000828 RID: 2088
			// (get) Token: 0x060055C5 RID: 21957 RVA: 0x001B021D File Offset: 0x001AE41D
			// (set) Token: 0x060055C6 RID: 21958 RVA: 0x001B0225 File Offset: 0x001AE425
			public int CurrentLevelIndex { readonly get; set; }

			// Token: 0x17000829 RID: 2089
			// (get) Token: 0x060055C7 RID: 21959 RVA: 0x001B022E File Offset: 0x001AE42E
			// (set) Token: 0x060055C8 RID: 21960 RVA: 0x001B0236 File Offset: 0x001AE436
			public int CurrentScore { readonly get; set; }

			// Token: 0x1700082A RID: 2090
			// (get) Token: 0x060055C9 RID: 21961 RVA: 0x001B023F File Offset: 0x001AE43F
			// (set) Token: 0x060055CA RID: 21962 RVA: 0x001B0247 File Offset: 0x001AE447
			public int TotalScore { readonly get; set; }

			// Token: 0x1700082B RID: 2091
			// (get) Token: 0x060055CB RID: 21963 RVA: 0x001B0250 File Offset: 0x001AE450
			// (set) Token: 0x060055CC RID: 21964 RVA: 0x001B0258 File Offset: 0x001AE458
			public int BestScore { readonly get; set; }

			// Token: 0x1700082C RID: 2092
			// (get) Token: 0x060055CD RID: 21965 RVA: 0x001B0261 File Offset: 0x001AE461
			// (set) Token: 0x060055CE RID: 21966 RVA: 0x001B0269 File Offset: 0x001AE469
			public int RightPlayerScore { readonly get; set; }

			// Token: 0x1700082D RID: 2093
			// (get) Token: 0x060055CF RID: 21967 RVA: 0x001B0272 File Offset: 0x001AE472
			// (set) Token: 0x060055D0 RID: 21968 RVA: 0x001B0284 File Offset: 0x001AE484
			[Networked]
			[NetworkedWeaved(6, 129)]
			public unsafe NetworkString<_128> HighScorePlayerName
			{
				readonly get
				{
					return *(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName);
				}
				set
				{
					*(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName) = value;
				}
			}

			// Token: 0x1700082E RID: 2094
			// (get) Token: 0x060055D1 RID: 21969 RVA: 0x001B0297 File Offset: 0x001AE497
			// (set) Token: 0x060055D2 RID: 21970 RVA: 0x001B029F File Offset: 0x001AE49F
			public float RemainingTime { readonly get; set; }

			// Token: 0x1700082F RID: 2095
			// (get) Token: 0x060055D3 RID: 21971 RVA: 0x001B02A8 File Offset: 0x001AE4A8
			// (set) Token: 0x060055D4 RID: 21972 RVA: 0x001B02B0 File Offset: 0x001AE4B0
			public float GameEndedTime { readonly get; set; }

			// Token: 0x17000830 RID: 2096
			// (get) Token: 0x060055D5 RID: 21973 RVA: 0x001B02B9 File Offset: 0x001AE4B9
			// (set) Token: 0x060055D6 RID: 21974 RVA: 0x001B02C1 File Offset: 0x001AE4C1
			public int GameId { readonly get; set; }

			// Token: 0x17000831 RID: 2097
			// (get) Token: 0x060055D7 RID: 21975 RVA: 0x001B02CA File Offset: 0x001AE4CA
			// (set) Token: 0x060055D8 RID: 21976 RVA: 0x001B02D2 File Offset: 0x001AE4D2
			public int PickedMolesIndexCount { readonly get; set; }

			// Token: 0x17000832 RID: 2098
			// (get) Token: 0x060055D9 RID: 21977 RVA: 0x001B02DC File Offset: 0x001AE4DC
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedDictionary(17, 1, 1, typeof(ElementReaderWriterInt32), typeof(ElementReaderWriterInt32))]
			[NetworkedWeaved(139, 71)]
			public unsafe NetworkDictionary<int, int> PickedMolesIndex
			{
				get
				{
					return new NetworkDictionary<int, int>((int*)Native.ReferenceToPointer<FixedStorage@71>(ref this._PickedMolesIndex), 17, ElementReaderWriterInt32.GetInstance(), ElementReaderWriterInt32.GetInstance());
				}
			}

			// Token: 0x060055DA RID: 21978 RVA: 0x001B0308 File Offset: 0x001AE508
			public WhackAMoleData(WhackAMole.GameState state, int currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float remainingTime, float endedTime, int gameId, Dictionary<int, int> moleIndexs)
			{
				this.CurrentState = state;
				this.CurrentLevelIndex = currentLevelIndex;
				this.CurrentScore = cScore;
				this.TotalScore = tScore;
				this.BestScore = bScore;
				this.RightPlayerScore = rPScore;
				this.HighScorePlayerName = hScorePName;
				this.RemainingTime = remainingTime;
				this.GameEndedTime = endedTime;
				this.GameId = gameId;
				this.PickedMolesIndexCount = moleIndexs.Count;
				foreach (KeyValuePair<int, int> keyValuePair in moleIndexs)
				{
					this.PickedMolesIndex.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x040062FA RID: 25338
			[FixedBufferProperty(typeof(NetworkString<_128>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(24)]
			private FixedStorage@129 _HighScorePlayerName;

			// Token: 0x040062FF RID: 25343
			[FixedBufferProperty(typeof(NetworkDictionary<int, int>), typeof(UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32), 17, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(556)]
			private FixedStorage@71 _PickedMolesIndex;
		}
	}
}
