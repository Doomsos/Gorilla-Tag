using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200054E RID: 1358
[NetworkBehaviourWeaved(0)]
public class MonkeBallGame : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06002232 RID: 8754 RVA: 0x000B2DD2 File Offset: 0x000B0FD2
	public Transform BallLauncher
	{
		get
		{
			return this._ballLauncher;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06002233 RID: 8755 RVA: 0x000B2DDA File Offset: 0x000B0FDA
	// (set) Token: 0x06002234 RID: 8756 RVA: 0x000B2DE2 File Offset: 0x000B0FE2
	public bool TickRunning { get; set; }

	// Token: 0x06002235 RID: 8757 RVA: 0x000B2DEC File Offset: 0x000B0FEC
	protected override void Awake()
	{
		base.Awake();
		MonkeBallGame.Instance = this;
		this.gameState = MonkeBallGame.GameState.None;
		this._callLimiters = new CallLimiter[8];
		this._callLimiters[0] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[1] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[2] = new CallLimiter(20, 10f, 0.5f);
		this._callLimiters[3] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[4] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[5] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[6] = new CallLimiter(20, 1f, 0.5f);
		this._callLimiters[7] = new CallLimiter(5, 10f, 0.5f);
		this.AssignNetworkListeners();
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000B2EE8 File Offset: 0x000B10E8
	private bool ValidateCallLimits(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info)
	{
		if (rpcCall < MonkeBallGame.RPC.SetGameState || rpcCall >= MonkeBallGame.RPC.Count)
		{
			return false;
		}
		bool flag = this._callLimiters[(int)rpcCall].CheckCallTime(Time.time);
		if (!flag)
		{
			this.ReportRPCCall(rpcCall, info, "Too many RPC Calls!");
		}
		return flag;
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000B2F23 File Offset: 0x000B1123
	private void ReportRPCCall(MonkeBallGame.RPC rpcCall, PhotonMessageInfo info, string susReason)
	{
		GorillaNot.instance.SendReport(string.Format("Reason: {0}   RPC: {1}", susReason, rpcCall), info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000B2F58 File Offset: 0x000B1158
	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			GameBallManager.Instance.AddGameBall(this.startingBalls[i].gameBall);
		}
		for (int j = 0; j < this.scoreboards.Count; j++)
		{
			this.scoreboards[j].Setup(this);
		}
		this.gameEndTime = -1.0;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x0000B3F9 File Offset: 0x000095F9
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x0000B40D File Offset: 0x0000960D
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000B2FD6 File Offset: 0x000B11D6
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		base.Despawned(runner, hasState);
		this.UnassignNetworkListeners();
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000B2FE8 File Offset: 0x000B11E8
	public void OnPlayerDestroy()
	{
		if (this._setStoredLocalPlayerColor)
		{
			PlayerPrefs.SetFloat("redValue", this._storedLocalPlayerColor.r);
			PlayerPrefs.SetFloat("greenValue", this._storedLocalPlayerColor.g);
			PlayerPrefs.SetFloat("blueValue", this._storedLocalPlayerColor.b);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000B3044 File Offset: 0x000B1244
	private void AssignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.OnPlayerJoined);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeft);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += new Action<NetPlayer>(this.OnMasterClientSwitched);
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000B30B4 File Offset: 0x000B12B4
	private void UnassignNetworkListeners()
	{
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(this.OnPlayerJoined);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeft);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= new Action<NetPlayer>(this.OnMasterClientSwitched);
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x000B3124 File Offset: 0x000B1324
	public void Tick()
	{
		if (this.IsMasterClient() && this.gameState != MonkeBallGame.GameState.None && this.gameEndTime >= 0.0 && PhotonNetwork.Time > this.gameEndTime)
		{
			this.gameEndTime = -1.0;
			this.RequestGameState(MonkeBallGame.GameState.PostGame);
		}
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		this.RefreshTime();
		if (this._forceSync)
		{
			this._forceSyncDelay -= Time.deltaTime;
			if (this._forceSyncDelay <= 0f)
			{
				this._forceSync = false;
				this.ForceSyncPlayersVisuals();
				this.RefreshTeamPlayers(false);
			}
		}
		if (this._forceOrigColorFix)
		{
			this._forceOrigColorDelay -= Time.deltaTime;
			if (this._forceOrigColorDelay <= 0f)
			{
				this._forceOrigColorFix = false;
				this.ForceOriginalColorSync();
			}
		}
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x000B31F4 File Offset: 0x000B13F4
	private void OnPlayerJoined(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		if (!this.IsMasterClient())
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", player.GetPlayerRef(), new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000B327C File Offset: 0x000B147C
	private void OnPlayerLeft(NetPlayer player)
	{
		this._forceSync = true;
		this._forceSyncDelay = 5f;
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.CleanupPlayer();
		}
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetTeamRPC", 0, new object[]
		{
			-1,
			player.GetPlayerRef()
		});
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000B32E8 File Offset: 0x000B14E8
	private void OnMasterClientSwitched(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int[] array;
		int[] array2;
		int[] array3;
		long[] array4;
		long[] array5;
		this.GetCurrentGameState(out array, out array2, out array3, out array4, out array5);
		this.photonView.RPC("RequestSetGameStateRPC", 1, new object[]
		{
			(int)this.gameState,
			this.gameEndTime,
			array,
			array2,
			array3,
			array4,
			array5
		});
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000B335C File Offset: 0x000B155C
	private void GetCurrentGameState(out int[] playerIds, out int[] playerTeams, out int[] scores, out long[] packedBallPosRot, out long[] packedBallVel)
	{
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		playerIds = new int[allNetPlayers.Length];
		playerTeams = new int[allNetPlayers.Length];
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			playerIds[i] = allNetPlayers[i].ActorNumber;
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[i].ActorNumber);
			if (gamePlayer != null)
			{
				playerTeams[i] = gamePlayer.teamId;
			}
			else
			{
				playerTeams[i] = -1;
			}
		}
		scores = new int[this.team.Count];
		for (int j = 0; j < this.team.Count; j++)
		{
			scores[j] = this.team[j].score;
		}
		packedBallPosRot = new long[this.startingBalls.Count];
		packedBallVel = new long[this.startingBalls.Count];
		for (int k = 0; k < this.startingBalls.Count; k++)
		{
			packedBallPosRot[k] = BitPackUtils.PackHandPosRotForNetwork(this.startingBalls[k].transform.position, this.startingBalls[k].transform.rotation);
			packedBallVel[k] = BitPackUtils.PackWorldPosForNetwork(this.startingBalls[k].gameBall.GetVelocity());
		}
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x000822D7 File Offset: 0x000804D7
	private bool IsMasterClient()
	{
		return PhotonNetwork.IsMasterClient;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000B34A2 File Offset: 0x000B16A2
	public MonkeBallGame.GameState GetGameState()
	{
		return this.gameState;
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000B34AA File Offset: 0x000B16AA
	public void RequestGameState(MonkeBallGame.GameState newGameState)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetGameStateRPC", 0, new object[]
		{
			(int)newGameState
		});
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000B34D8 File Offset: 0x000B16D8
	[PunRPC]
	private void SetGameStateRPC(int newGameState, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetGameState, info))
		{
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetGameState, info, "newGameState outside of enum range.");
			return;
		}
		this.SetGameState((MonkeBallGame.GameState)newGameState);
		if (newGameState == 1)
		{
			this.gameEndTime = info.SentServerTime + (double)this.gameDuration;
		}
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000B3540 File Offset: 0x000B1740
	private void SetGameState(MonkeBallGame.GameState newGameState)
	{
		this.gameState = newGameState;
		switch (this.gameState)
		{
		case MonkeBallGame.GameState.PreGame:
			this.OnEnterStatePreGame();
			return;
		case MonkeBallGame.GameState.Playing:
			this.OnEnterStatePlaying();
			return;
		case MonkeBallGame.GameState.PostScore:
			this.OnEnterStatePostScore();
			return;
		case MonkeBallGame.GameState.PostGame:
			this.OnEnterStatePostGame();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000B3590 File Offset: 0x000B1790
	private void OnEnterStatePreGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameStartFx();
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000B35C4 File Offset: 0x000B17C4
	private void OnEnterStatePlaying()
	{
		this._forceSync = true;
		this._forceSyncDelay = 0.1f;
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnterStatePostScore()
	{
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000B35D8 File Offset: 0x000B17D8
	private void OnEnterStatePostGame()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayGameEndFx();
		}
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000B360C File Offset: 0x000B180C
	[PunRPC]
	private void RequestSetGameStateRPC(int newGameState, double newGameEndTime, int[] playerIds, int[] playerTeams, int[] scores, long[] packedBallPosRot, long[] packedBallVel, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "RequestSetGameStateRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetGameState, info))
		{
			return;
		}
		if (playerIds.IsNullOrEmpty<int>() || playerTeams.IsNullOrEmpty<int>() || scores.IsNullOrEmpty<int>() || packedBallPosRot.IsNullOrEmpty<long>() || packedBallVel.IsNullOrEmpty<long>())
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "Array params are null or empty.");
			return;
		}
		if (newGameState < 0 || newGameState > 4)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameState outside of enum range.");
			return;
		}
		if (playerIds.Length != playerTeams.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "playerIDs and playerTeams are not the same length.");
			return;
		}
		if (scores.Length > this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "scores and team are not the same length.");
			return;
		}
		if (packedBallPosRot.Length != this.startingBalls.Count || packedBallPosRot.Length != packedBallVel.Length)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "packedBall arrays are not the same length.");
			return;
		}
		if (double.IsNaN(newGameEndTime) || double.IsInfinity(newGameEndTime))
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime is not valid.");
			return;
		}
		if (newGameEndTime < -1.0 || newGameEndTime > PhotonNetwork.Time + (double)this.gameDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetGameState, info, "newGameEndTime exceeds possible time limits.");
			return;
		}
		this.gameState = (MonkeBallGame.GameState)newGameState;
		this.gameEndTime = newGameEndTime;
		for (int i = 0; i < playerIds.Length; i++)
		{
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != playerIds[i])
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(playerIds[i]);
				if (!(gamePlayer == null))
				{
					gamePlayer.teamId = playerTeams[i];
					RigContainer rigContainer;
					if (playerTeams[i] >= 0 && playerTeams[i] < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerIds[i]), out rigContainer))
					{
						Color color = this.team[playerTeams[i]].color;
						rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
						rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
					}
				}
			}
		}
		this.RefreshTeamPlayers(false);
		for (int j = 0; j < scores.Length; j++)
		{
			this.SetScore(j, scores[j], false);
		}
		for (int k = 0; k < packedBallPosRot.Length; k++)
		{
			Vector3 position;
			Quaternion rotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(packedBallPosRot[k], out position, out rotation);
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				this.startingBalls[k].transform.position = position;
				this.startingBalls[k].transform.rotation = rotation;
				if ((this.startingBalls[k].transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					this.startingBalls[k].transform.position = this._neutralBallStartLocation.transform.position;
				}
				Vector3 velocity = BitPackUtils.UnpackWorldPosFromNetwork(packedBallVel[k]);
				num = 10000f;
				if (velocity.IsValid(num))
				{
					this.startingBalls[k].gameBall.SetVelocity(velocity);
					this.startingBalls[k].TriggerDelayedResync();
				}
			}
		}
		this._forceSync = true;
		this._forceSyncDelay = 5f;
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000B3976 File Offset: 0x000B1B76
	public void RequestResetGame()
	{
		this.photonView.RPC("RequestResetGameRPC", 0, Array.Empty<object>());
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000B3990 File Offset: 0x000B1B90
	[PunRPC]
	private void RequestResetGameRPC(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestResetGameRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestResetGame, info))
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(info.Sender.ActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		if (gamePlayer.teamId != this.resetButton.allowedTeamId)
		{
			return;
		}
		for (int i = 0; i < this.startingBalls.Count; i++)
		{
			this.RequestResetBall(this.startingBalls[i].gameBall.id, -1);
		}
		for (int j = 0; j < this.team.Count; j++)
		{
			this.RequestSetScore(j, 0);
		}
		this.RequestGameState(MonkeBallGame.GameState.PreGame);
		this.resetButton.ToggleReset(false, -1, true);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(false, -1, true);
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000B3A70 File Offset: 0x000B1C70
	public void ToggleResetButton(bool toggle, int teamId)
	{
		int otherTeam = this.GetOtherTeam(teamId);
		this.photonView.RPC("SetResetButtonRPC", 0, new object[]
		{
			toggle,
			otherTeam
		});
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000B3AB0 File Offset: 0x000B1CB0
	[PunRPC]
	private void SetResetButtonRPC(bool toggleReset, int teamId, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetResetButtonRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetResetButton, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetResetButton, info, "teamID exceeds possible range.");
			return;
		}
		this.resetButton.ToggleReset(toggleReset, teamId, false);
		if (this.centerResetButton != null)
		{
			this.centerResetButton.ToggleReset(toggleReset, teamId, false);
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000B3B2B File Offset: 0x000B1D2B
	public void OnBallGrabbed(GameBallId gameBallId)
	{
		if (this.gameState == MonkeBallGame.GameState.PreGame)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
		if (this.gameState == MonkeBallGame.GameState.PostScore)
		{
			this.SetGameState(MonkeBallGame.GameState.Playing);
		}
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000B3B50 File Offset: 0x000B1D50
	private void RefreshTime()
	{
		this._frameIndex++;
		if (this._frameIndex > 2)
		{
			this._frameIndex = 0;
		}
		if (this._frameIndex != 0)
		{
			return;
		}
		float num = (float)(this.gameEndTime - PhotonNetwork.Time);
		if (this.gameEndTime < 0.0)
		{
			num = 0f;
		}
		string timeString = Mathf.Max(num, 0f).ToString("#00.00");
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshTime(timeString);
		}
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000B3BEC File Offset: 0x000B1DEC
	public void RequestResetBall(GameBallId gameBallId, int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId >= 0)
		{
			this.LaunchBallWithTeam(gameBallId, teamId, this.team[teamId].ballLaunchPosition, this.team[teamId].ballLaunchVelocityRange, this.team[teamId].ballLaunchAngleXRange, this.team[teamId].ballLaunchAngleXRange);
			return;
		}
		this.LaunchBallNeutral(gameBallId);
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000B3C5C File Offset: 0x000B1E5C
	public void RequestScore(int teamId)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", 0, new object[]
		{
			teamId,
			this.team[teamId].score + 1
		});
		this.RequestGameState(MonkeBallGame.GameState.PostScore);
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000B3CC7 File Offset: 0x000B1EC7
	public void RequestSetScore(int teamId, int score)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetScoreRPC", 0, new object[]
		{
			teamId,
			score
		});
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000B3CFC File Offset: 0x000B1EFC
	[PunRPC]
	private void SetScoreRPC(int teamId, int score, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetScoreRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetScore, info))
		{
			return;
		}
		if (teamId < 0 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "teamID exceeds possible range.");
			return;
		}
		if (score != 0 && score != this.team[teamId].score + 1)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetScore, info, "Score is being set to a non-achievable value.");
			return;
		}
		this.SetScore(teamId, Mathf.Clamp(score, 0, 999), true);
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000B3D88 File Offset: 0x000B1F88
	private void SetScore(int teamId, int score, bool playFX = true)
	{
		if (teamId < 0 || teamId > this.team.Count)
		{
			return;
		}
		int score2 = this.team[teamId].score;
		this.team[teamId].score = score;
		if (playFX && score > score2)
		{
			this.PlayScoreFx();
			Color color = this.team[teamId].color;
			for (int i = 0; i < this.endZoneEffects.Length; i++)
			{
				this.endZoneEffects[i].startColor = color;
				this.endZoneEffects[i].Play();
			}
		}
		this.RefreshScore();
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000B3E20 File Offset: 0x000B2020
	private void RefreshScore()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].RefreshScore();
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000B3E54 File Offset: 0x000B2054
	private void PlayScoreFx()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].PlayScoreFx();
		}
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000B3E88 File Offset: 0x000B2088
	public MonkeBallTeam GetTeam(int teamId)
	{
		return this.team[teamId];
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000B3E96 File Offset: 0x000B2096
	public int GetOtherTeam(int teamId)
	{
		return (teamId + 1) % this.team.Count;
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x000B3EA8 File Offset: 0x000B20A8
	public void RequestSetTeam(int teamId)
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		this.photonView.RPC("RequestSetTeamRPC", 2, new object[]
		{
			teamId
		});
		bool flag = false;
		Color color = Color.white;
		if (teamId >= 0 && teamId < this.team.Count)
		{
			flag = true;
			if (!this._setStoredLocalPlayerColor)
			{
				this._storedLocalPlayerColor = new Color(PlayerPrefs.GetFloat("redValue", 1f), PlayerPrefs.GetFloat("greenValue", 1f), PlayerPrefs.GetFloat("blueValue", 1f));
				this._setStoredLocalPlayerColor = true;
			}
			this._forceOrigColorFix = false;
			color = this.team[teamId].color;
		}
		else
		{
			color = this._storedLocalPlayerColor;
			this._setStoredLocalPlayerColor = false;
		}
		PlayerPrefs.SetFloat("redValue", color.r);
		PlayerPrefs.SetFloat("greenValue", color.g);
		PlayerPrefs.SetFloat("blueValue", color.b);
		PlayerPrefs.Save();
		GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
		GorillaComputer.instance.UpdateColor(color.r, color.g, color.b);
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
			{
				color.r,
				color.g,
				color.b
			});
			if (flag)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", 0, Array.Empty<object>());
			}
			else
			{
				this._forceOrigColorFix = true;
				this._forceOrigColorDelay = 3f;
				CosmeticsController.instance.UpdateWornCosmetics(true);
			}
			this.ForceSyncPlayersVisuals();
		}
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000B4074 File Offset: 0x000B2274
	private MonkeBall GetMonkeBall(GameBallId gameBallId)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (!(gameBall == null))
		{
			return gameBall.GetComponent<MonkeBall>();
		}
		return null;
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000B40A0 File Offset: 0x000B22A0
	[PunRPC]
	private void RequestSetTeamRPC(int teamId, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestSetTeamRPC");
		if (!this.IsMasterClient())
		{
			return;
		}
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.RequestSetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.RequestSetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.photonView.RPC("SetTeamRPC", 0, new object[]
		{
			teamId,
			info.Sender
		});
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000B4118 File Offset: 0x000B2318
	[PunRPC]
	private void SetTeamRPC(int teamId, Player player, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetTeamRPC");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetTeam, info))
		{
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetTeam, info, "teamID exceeds possible range.");
			return;
		}
		this.SetTeamPlayer(teamId, player);
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000B4174 File Offset: 0x000B2374
	private void SetTeamPlayer(int teamId, Player player)
	{
		if (player == null)
		{
			return;
		}
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(player.ActorNumber);
		if (gamePlayer != null)
		{
			gamePlayer.teamId = teamId;
		}
		this.RefreshTeamPlayers(true);
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000B41A8 File Offset: 0x000B23A8
	private void RefreshTeamPlayers(bool playSounds)
	{
		int[] array = new int[this.team.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 0;
		}
		int num = 0;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int j = 0; j < allNetPlayers.Length; j++)
		{
			GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(allNetPlayers[j].ActorNumber);
			if (!(gamePlayer == null))
			{
				int teamId = gamePlayer.teamId;
				if (teamId >= 0)
				{
					array[teamId]++;
					num++;
				}
			}
		}
		for (int k = 0; k < this.scoreboards.Count; k++)
		{
			for (int l = 0; l < array.Length; l++)
			{
				this.scoreboards[k].RefreshTeamPlayers(l, array[l]);
			}
			if (playSounds)
			{
				if (this._currentPlayerTotal < num)
				{
					this.scoreboards[k].PlayPlayerJoinFx();
				}
				else if (this._currentPlayerTotal > num)
				{
					this.scoreboards[k].PlayPlayerLeaveFx();
				}
			}
		}
		this._currentPlayerTotal = num;
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000B42B8 File Offset: 0x000B24B8
	private void ForceSyncPlayersVisuals()
	{
		for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
		{
			int actorNumber = NetworkSystem.Instance.AllNetPlayers[i].ActorNumber;
			if (VRRigCache.Instance.localRig.Creator.ActorNumber != actorNumber)
			{
				GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(actorNumber);
				RigContainer rigContainer;
				if (!(gamePlayer == null) && gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
				{
					Color color = this.team[gamePlayer.teamId].color;
					rigContainer.Rig.InitializeNoobMaterialLocal(color.r, color.g, color.b);
					rigContainer.Rig.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
				}
			}
		}
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000B43A4 File Offset: 0x000B25A4
	private void ForceOriginalColorSync()
	{
		GameBallPlayer gamePlayer = GameBallPlayer.GetGamePlayer(VRRigCache.Instance.localRig.Creator.ActorNumber);
		if (gamePlayer == null || (gamePlayer.teamId >= 0 && gamePlayer.teamId < this.team.Count))
		{
			return;
		}
		Color storedLocalPlayerColor = this._storedLocalPlayerColor;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", 0, new object[]
			{
				storedLocalPlayerColor.r,
				storedLocalPlayerColor.g,
				storedLocalPlayerColor.b
			});
		}
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000B4449 File Offset: 0x000B2649
	public void RequestRestrictBallToTeam(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDuration);
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000B4459 File Offset: 0x000B2659
	public void RequestRestrictBallToTeamOnScore(GameBallId gameBallId, int teamId)
	{
		this.RestrictBallToTeam(gameBallId, teamId, this.restrictBallDurationAfterScore);
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000B446C File Offset: 0x000B266C
	private void RestrictBallToTeam(GameBallId gameBallId, int teamId, float restrictDuration)
	{
		if (!this.IsMasterClient())
		{
			return;
		}
		this.photonView.RPC("SetRestrictBallToTeam", 0, new object[]
		{
			gameBallId.index,
			teamId,
			restrictDuration
		});
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000B44BC File Offset: 0x000B26BC
	[PunRPC]
	private void SetRestrictBallToTeam(int gameBallIndex, int teamId, float restrictDuration, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "SetRestrictBallToTeam");
		if (!this.ValidateCallLimits(MonkeBallGame.RPC.SetRestrictBallToTeam, info))
		{
			return;
		}
		if (gameBallIndex < 0 || gameBallIndex >= this.startingBalls.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "gameBallIndex exceeds possible range.");
			return;
		}
		if (teamId < -1 || teamId >= this.team.Count)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "teamID exceeds possible range.");
			return;
		}
		if (float.IsNaN(restrictDuration) || float.IsInfinity(restrictDuration) || restrictDuration < 0f || restrictDuration > this.restrictBallDurationAfterScore + this.restrictBallDuration)
		{
			this.ReportRPCCall(MonkeBallGame.RPC.SetRestrictBallToTeam, info, "restrictDuration is not a feasible value.");
			return;
		}
		GameBallId gameBallId = new GameBallId(gameBallIndex);
		MonkeBall monkeBall = this.GetMonkeBall(gameBallId);
		bool flag = false;
		if (monkeBall != null)
		{
			flag = monkeBall.RestrictBallToTeam(teamId, restrictDuration);
		}
		if (flag)
		{
			for (int i = 0; i < this.shotclocks.Count; i++)
			{
				this.shotclocks[i].SetTime(teamId, restrictDuration);
			}
		}
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000B45BC File Offset: 0x000B27BC
	public void LaunchBallNeutral(GameBallId gameBallId)
	{
		this.LaunchBall(gameBallId, this._ballLauncher, this.ballLauncherVelocityRange.x, this.ballLauncherVelocityRange.y, this.ballLaunchAngleXRange.x, this.ballLaunchAngleXRange.y, this.ballLaunchAngleYRange.x, this.ballLaunchAngleYRange.y);
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000B4618 File Offset: 0x000B2818
	public void LaunchBallWithTeam(GameBallId gameBallId, int teamId, Transform launcher, Vector2 velocityRange, Vector2 angleXRange, Vector2 angleYRange)
	{
		this.LaunchBall(gameBallId, launcher, velocityRange.x, velocityRange.y, angleXRange.x, angleXRange.y, angleYRange.x, angleYRange.y);
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000B4658 File Offset: 0x000B2858
	private void LaunchBall(GameBallId gameBallId, Transform launcher, float minVelocity, float maxVelocity, float minXAngle, float maxXAngle, float minYAngle, float maxYAngle)
	{
		GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
		if (gameBall == null)
		{
			return;
		}
		gameBall.transform.position = launcher.transform.position;
		Quaternion rotation = launcher.transform.rotation;
		launcher.transform.Rotate(Vector3.up, Random.Range(minXAngle, maxXAngle));
		launcher.transform.Rotate(Vector3.right, Random.Range(minYAngle, maxYAngle));
		gameBall.transform.rotation = launcher.transform.rotation;
		Vector3 velocity = launcher.transform.forward * Random.Range(minVelocity, maxVelocity);
		launcher.transform.rotation = rotation;
		GameBallManager.Instance.RequestLaunchBall(gameBallId, velocity);
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x00002789 File Offset: 0x00000989
	public override void WriteDataFusion()
	{
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x00002789 File Offset: 0x00000989
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x00002789 File Offset: 0x00000989
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x00002789 File Offset: 0x00000989
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002CD6 RID: 11478
	public static MonkeBallGame Instance;

	// Token: 0x04002CD7 RID: 11479
	public List<MonkeBall> startingBalls;

	// Token: 0x04002CD8 RID: 11480
	public List<MonkeBallScoreboard> scoreboards;

	// Token: 0x04002CD9 RID: 11481
	public List<MonkeBallShotclock> shotclocks;

	// Token: 0x04002CDA RID: 11482
	public List<MonkeBallGoalZone> goalZones;

	// Token: 0x04002CDB RID: 11483
	[Space]
	public MonkeBallResetGame resetButton;

	// Token: 0x04002CDC RID: 11484
	public MonkeBallResetGame centerResetButton;

	// Token: 0x04002CDD RID: 11485
	[Space]
	public PhotonView photonView;

	// Token: 0x04002CDE RID: 11486
	public List<MonkeBallTeam> team;

	// Token: 0x04002CDF RID: 11487
	private int _currentPlayerTotal;

	// Token: 0x04002CE0 RID: 11488
	[Space]
	[Tooltip("The length of the game in seconds.")]
	public float gameDuration;

	// Token: 0x04002CE1 RID: 11489
	[Space]
	[Tooltip("If the ball should be reset to a team starting position after a score. If not set to true then the will reset back to a neutral starting position.")]
	public bool resetBallPositionOnScore = true;

	// Token: 0x04002CE2 RID: 11490
	[Tooltip("The duration in which a team is restricted from grabbing the ball after toss.")]
	public float restrictBallDuration = 5f;

	// Token: 0x04002CE3 RID: 11491
	[Tooltip("The duration in which a team is restricted from grabbing the ball after a score.")]
	public float restrictBallDurationAfterScore = 10f;

	// Token: 0x04002CE4 RID: 11492
	[Header("Neutral Launcher")]
	[SerializeField]
	private Transform _ballLauncher;

	// Token: 0x04002CE5 RID: 11493
	[Tooltip("The min/max random velocity of the ball when launched.")]
	public Vector2 ballLauncherVelocityRange = new Vector2(8f, 15f);

	// Token: 0x04002CE6 RID: 11494
	[Tooltip("The min/max random x-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleXRange = new Vector2(0f, 0f);

	// Token: 0x04002CE7 RID: 11495
	[Tooltip("The min/max random y-angle of the ball when launched.")]
	public Vector2 ballLaunchAngleYRange = new Vector2(0f, 0f);

	// Token: 0x04002CE8 RID: 11496
	[Space]
	[SerializeField]
	private Transform _neutralBallStartLocation;

	// Token: 0x04002CE9 RID: 11497
	[SerializeField]
	private ParticleSystem[] endZoneEffects;

	// Token: 0x04002CEB RID: 11499
	private MonkeBallGame.GameState gameState;

	// Token: 0x04002CEC RID: 11500
	public double gameEndTime;

	// Token: 0x04002CED RID: 11501
	private int _frameIndex;

	// Token: 0x04002CEE RID: 11502
	private bool _forceSync;

	// Token: 0x04002CEF RID: 11503
	private float _forceSyncDelay;

	// Token: 0x04002CF0 RID: 11504
	private bool _forceOrigColorFix;

	// Token: 0x04002CF1 RID: 11505
	private float _forceOrigColorDelay;

	// Token: 0x04002CF2 RID: 11506
	private Color _storedLocalPlayerColor;

	// Token: 0x04002CF3 RID: 11507
	private bool _setStoredLocalPlayerColor;

	// Token: 0x04002CF4 RID: 11508
	private CallLimiter[] _callLimiters;

	// Token: 0x0200054F RID: 1359
	public enum GameState
	{
		// Token: 0x04002CF6 RID: 11510
		None,
		// Token: 0x04002CF7 RID: 11511
		PreGame,
		// Token: 0x04002CF8 RID: 11512
		Playing,
		// Token: 0x04002CF9 RID: 11513
		PostScore,
		// Token: 0x04002CFA RID: 11514
		PostGame
	}

	// Token: 0x02000550 RID: 1360
	private enum RPC
	{
		// Token: 0x04002CFC RID: 11516
		SetGameState,
		// Token: 0x04002CFD RID: 11517
		RequestSetGameState,
		// Token: 0x04002CFE RID: 11518
		RequestResetGame,
		// Token: 0x04002CFF RID: 11519
		SetScore,
		// Token: 0x04002D00 RID: 11520
		RequestSetTeam,
		// Token: 0x04002D01 RID: 11521
		SetTeam,
		// Token: 0x04002D02 RID: 11522
		SetRestrictBallToTeam,
		// Token: 0x04002D03 RID: 11523
		SetResetButton,
		// Token: 0x04002D04 RID: 11524
		Count
	}
}
