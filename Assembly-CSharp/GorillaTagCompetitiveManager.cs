using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020007B6 RID: 1974
public class GorillaTagCompetitiveManager : GorillaTagManager
{
	// Token: 0x060033D2 RID: 13266 RVA: 0x001172F8 File Offset: 0x001154F8
	public float GetRoundDuration()
	{
		return this.roundDuration;
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x00117300 File Offset: 0x00115500
	public GorillaTagCompetitiveManager.GameState GetCurrentGameState()
	{
		return this.gameState;
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x00117308 File Offset: 0x00115508
	public bool IsMatchActive()
	{
		return this.gameState == GorillaTagCompetitiveManager.GameState.Playing;
	}

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x060033D5 RID: 13269 RVA: 0x00117314 File Offset: 0x00115514
	// (remove) Token: 0x060033D6 RID: 13270 RVA: 0x00117348 File Offset: 0x00115548
	public static event Action<GorillaTagCompetitiveManager.GameState> onStateChanged;

	// Token: 0x14000059 RID: 89
	// (add) Token: 0x060033D7 RID: 13271 RVA: 0x0011737C File Offset: 0x0011557C
	// (remove) Token: 0x060033D8 RID: 13272 RVA: 0x001173B0 File Offset: 0x001155B0
	public static event Action<float> onUpdateRemainingTime;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x060033D9 RID: 13273 RVA: 0x001173E4 File Offset: 0x001155E4
	// (remove) Token: 0x060033DA RID: 13274 RVA: 0x00117418 File Offset: 0x00115618
	public static event Action<NetPlayer> onPlayerJoined;

	// Token: 0x1400005B RID: 91
	// (add) Token: 0x060033DB RID: 13275 RVA: 0x0011744C File Offset: 0x0011564C
	// (remove) Token: 0x060033DC RID: 13276 RVA: 0x00117480 File Offset: 0x00115680
	public static event Action<NetPlayer> onPlayerLeft;

	// Token: 0x1400005C RID: 92
	// (add) Token: 0x060033DD RID: 13277 RVA: 0x001174B4 File Offset: 0x001156B4
	// (remove) Token: 0x060033DE RID: 13278 RVA: 0x001174E8 File Offset: 0x001156E8
	public static event Action onRoundStart;

	// Token: 0x1400005D RID: 93
	// (add) Token: 0x060033DF RID: 13279 RVA: 0x0011751C File Offset: 0x0011571C
	// (remove) Token: 0x060033E0 RID: 13280 RVA: 0x00117550 File Offset: 0x00115750
	public static event Action onRoundEnd;

	// Token: 0x1400005E RID: 94
	// (add) Token: 0x060033E1 RID: 13281 RVA: 0x00117584 File Offset: 0x00115784
	// (remove) Token: 0x060033E2 RID: 13282 RVA: 0x001175B8 File Offset: 0x001157B8
	public static event Action<NetPlayer, NetPlayer> onTagOccurred;

	// Token: 0x060033E3 RID: 13283 RVA: 0x001175EB File Offset: 0x001157EB
	public static void RegisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Add(scoreboard);
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x001175F8 File Offset: 0x001157F8
	public static void DeregisterScoreboard(GorillaTagCompetitiveScoreboard scoreboard)
	{
		GorillaTagCompetitiveManager.scoreboards.Remove(scoreboard);
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x00117608 File Offset: 0x00115808
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.scoring = base.GetComponentInChildren<RankedMultiplayerScore>();
		if (this.scoring != null)
		{
			this.scoring.Initialize();
		}
		VRRig.LocalRig.EnableRankedTimerWatch(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableRankedTimerWatch(true);
			}
		}
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x00117680 File Offset: 0x00115880
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableRankedTimerWatch(false);
		if (this.scoring != null)
		{
			this.scoring.ResetMatch();
			this.scoring.Unsubscribe();
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, null, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x060033E7 RID: 13287 RVA: 0x0011771B File Offset: 0x0011591B
	public override void ResetGame()
	{
		base.ResetGame();
		this.gameState = GorillaTagCompetitiveManager.GameState.None;
	}

	// Token: 0x060033E8 RID: 13288 RVA: 0x0011772A File Offset: 0x0011592A
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<GorillaTagCompetitiveRPCs>();
	}

	// Token: 0x060033E9 RID: 13289 RVA: 0x0011773C File Offset: 0x0011593C
	public override void Tick()
	{
		if (this.stateRemainingTime > 0f)
		{
			this.stateRemainingTime -= Time.deltaTime;
			if (this.stateRemainingTime <= 0f)
			{
				this.UpdateState();
			}
			Action<float> action = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action != null)
			{
				action.Invoke(this.stateRemainingTime);
			}
		}
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > this.waitingForPlayerPingRoomDuration)
			{
				this.PingRoom();
				this.lastWaitingForPlayerPingRoomTime = Time.time;
			}
			if (Time.time - this.lastWaitingForPlayerPingRoomTime > 3f)
			{
				this.ShowDebugPing = false;
			}
		}
		this.UpdateScoreboards();
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x001177E8 File Offset: 0x001159E8
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.PingRoom();
			this.lastWaitingForPlayerPingRoomTime = Time.time;
		}
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x00117810 File Offset: 0x00115A10
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (newPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			using (List<GorillaTagCompetitiveForcedLeaveRoomVolume>.Enumerator enumerator = this.forceLeaveRoomVolumes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ContainsPoint(VRRig.LocalRig.transform.position))
					{
						NetworkSystem.Instance.ReturnToSinglePlayer();
						return;
					}
				}
			}
			object obj;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GorillaTagCompetitiveServerApi.Instance.RequestCreateMatchId(delegate(string id)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("matchId", id);
					PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
				});
			}
			else if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", ref obj))
			{
				GorillaTagCompetitiveServerApi.Instance.RequestValidateMatchJoin((string)obj, delegate(bool valid)
				{
					if (!valid)
					{
						Debug.LogError("ValidateMatchJoin failed. Leaving room!");
						NetworkSystem.Instance.ReturnToSinglePlayer();
					}
				});
			}
		}
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerJoined;
		if (action != null)
		{
			action.Invoke(newPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(true);
		}
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x00117944 File Offset: 0x00115B44
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		Action<NetPlayer> action = GorillaTagCompetitiveManager.onPlayerLeft;
		if (action != null)
		{
			action.Invoke(otherPlayer);
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(otherPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableRankedTimerWatch(false);
		}
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x00117984 File Offset: 0x00115B84
	public RankedMultiplayerScore GetScoring()
	{
		return this.scoring;
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x0011798C File Offset: 0x00115B8C
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer) && this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound;
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x001179AF File Offset: 0x00115BAF
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown && this.gameState != GorillaTagCompetitiveManager.GameState.PostRound && base.LocalIsTagged(player);
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x001179CC File Offset: 0x00115BCC
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		base.ReportTag(taggedPlayer, taggingPlayer);
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x001179D6 File Offset: 0x00115BD6
	public override GameModeType GameType()
	{
		return GameModeType.InfectionCompetitive;
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x001179DA File Offset: 0x00115BDA
	public override string GameModeName()
	{
		return "COMP-INFECT";
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x001179E4 File Offset: 0x00115BE4
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_COMP_INF_ROOM_LABEL", out result, "(COMP-INFECT GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_COMP_INF_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x00002076 File Offset: 0x00000276
	public override bool CanJoinFrienship(NetPlayer player)
	{
		return false;
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x00117A0F File Offset: 0x00115C0F
	public override void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing && this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
		}
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x00117A38 File Offset: 0x00115C38
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (!this.currentInfected.Contains(taggingPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) && VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			VRRig rig = rigContainer2.Rig;
			VRRig rig2 = rigContainer.Rig;
			if (!rig.IsPositionInRange(rig2.transform.position, 6f) && !rig.CheckTagDistanceRollback(rig2, 6f, 0.2f))
			{
				return;
			}
			if (!NetworkSystem.Instance.IsMasterClient && this.gameState == GorillaTagCompetitiveManager.GameState.Playing && !this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.currentInfected.Add(taggedPlayer);
			}
			Action<NetPlayer, NetPlayer> action = GorillaTagCompetitiveManager.onTagOccurred;
			if (action == null)
			{
				return;
			}
			action.Invoke(taggedPlayer, taggingPlayer);
		}
	}

	// Token: 0x060033F7 RID: 13303 RVA: 0x00117AFC File Offset: 0x00115CFC
	private void SetState(GorillaTagCompetitiveManager.GameState newState)
	{
		if (newState != this.gameState)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.gameState;
			this.gameState = newState;
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.EnterStateWaitingForPlayers();
				break;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.EnterStateStartingCountdown();
				break;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.EnterStatePlaying();
				break;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.EnterStatePostRound();
				break;
			}
			Action<GorillaTagCompetitiveManager.GameState> action = GorillaTagCompetitiveManager.onStateChanged;
			if (action != null)
			{
				action.Invoke(this.gameState);
			}
			Action<float> action2 = GorillaTagCompetitiveManager.onUpdateRemainingTime;
			if (action2 != null)
			{
				action2.Invoke(this.stateRemainingTime);
			}
			if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action3 = GorillaTagCompetitiveManager.onRoundStart;
				if (action3 != null)
				{
					action3.Invoke();
				}
			}
			else if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				Action action4 = GorillaTagCompetitiveManager.onRoundEnd;
				if (action4 != null)
				{
					action4.Invoke();
				}
			}
			GTDev.Log<string>(string.Format("!! Competitive SetState: {0} at: {1}", this.gameState, Time.time), null);
		}
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x00117BE2 File Offset: 0x00115DE2
	private void EnterStateWaitingForPlayers()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			base.SetisCurrentlyTag(true);
			base.ClearInfectionState();
		}
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x00117C00 File Offset: 0x00115E00
	private void EnterStateStartingCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			base.ClearInfectionState();
			GameMode.RefreshPlayers();
			this.CheckForInfected();
			this.stateRemainingTime = this.startCountdownDuration;
		}
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x00117C4C File Offset: 0x00115E4C
	private void EnterStatePlaying()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.roundDuration;
			this.PingRoom();
		}
		this.DisplayScoreboardPredictedResults(false);
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x00117C89 File Offset: 0x00115E89
	private void EnterStatePostRound()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.isCurrentlyTag)
			{
				base.SetisCurrentlyTag(false);
			}
			this.currentIt = null;
			this.stateRemainingTime = this.postRoundDuration;
		}
		this.DisplayScoreboardPredictedResults(true);
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x00117CC0 File Offset: 0x00115EC0
	public override void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.gameState)
			{
			case GorillaTagCompetitiveManager.GameState.None:
				this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
				return;
			case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
				this.UpdateStateWaitingForPlayers();
				return;
			case GorillaTagCompetitiveManager.GameState.StartingCountdown:
				this.UpdateStateStartingCountdown();
				return;
			case GorillaTagCompetitiveManager.GameState.Playing:
				this.UpdateStatePlaying();
				return;
			case GorillaTagCompetitiveManager.GameState.PostRound:
				this.UpdateStatePostRound();
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x00117D20 File Offset: 0x00115F20
	private void UpdateStateWaitingForPlayers()
	{
		if (this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
			return;
		}
		if (this.isCurrentlyTag && this.currentIt == null)
		{
			int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
		}
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x00117D70 File Offset: 0x00115F70
	private void UpdateStateStartingCountdown()
	{
		if (!this.IsInfectionPossible())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.Playing);
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x00117D9D File Offset: 0x00115F9D
	private void UpdateStatePlaying()
	{
		if (this.IsGameInvalid())
		{
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
			return;
		}
		if (this.stateRemainingTime < 0f)
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		if (this.IsEveryoneTagged())
		{
			this.HandleInfectionRoundComplete();
			return;
		}
		this.CheckForInfected();
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x00117DD8 File Offset: 0x00115FD8
	private void HandleInfectionRoundComplete()
	{
		foreach (NetPlayer player in GameMode.ParticipatingPlayers)
		{
			RoomSystem.SendSoundEffectToPlayer(2, 0.25f, player, true);
		}
		PlayerGameEvents.GameModeCompleteRound();
		GameMode.BroadcastRoundComplete();
		this.lastTaggedActorNr.Clear();
		this.waitingToStartNextInfectionGame = true;
		this.timeInfectedGameEnded = (double)Time.time;
		this.SetState(GorillaTagCompetitiveManager.GameState.PostRound);
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x00117E60 File Offset: 0x00116060
	private void UpdateStatePostRound()
	{
		if (this.stateRemainingTime < 0f)
		{
			if (this.IsInfectionPossible())
			{
				this.SetState(GorillaTagCompetitiveManager.GameState.StartingCountdown);
				return;
			}
			this.SetState(GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		}
	}

	// Token: 0x06003402 RID: 13314 RVA: 0x00117E88 File Offset: 0x00116088
	private void PingRoom()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", ref obj))
		{
			GorillaTagCompetitiveServerApi.Instance.RequestPingRoom((string)obj, delegate
			{
				this.ShowDebugPing = true;
			});
		}
	}

	// Token: 0x1700049E RID: 1182
	// (get) Token: 0x06003403 RID: 13315 RVA: 0x00117EC9 File Offset: 0x001160C9
	// (set) Token: 0x06003404 RID: 13316 RVA: 0x00117ED1 File Offset: 0x001160D1
	public bool ShowDebugPing { get; set; }

	// Token: 0x06003405 RID: 13317 RVA: 0x00117EDA File Offset: 0x001160DA
	private bool IsGameInvalid()
	{
		return GameMode.ParticipatingPlayers.Count <= 1;
	}

	// Token: 0x06003406 RID: 13318 RVA: 0x00117EEC File Offset: 0x001160EC
	private bool IsInfectionPossible()
	{
		return GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold;
	}

	// Token: 0x06003407 RID: 13319 RVA: 0x00117F04 File Offset: 0x00116104
	private bool IsEveryoneTagged()
	{
		bool result = true;
		foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(netPlayer))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x00117F64 File Offset: 0x00116164
	private void CheckForInfected()
	{
		if (this.currentInfected.Count == 0)
		{
			int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
			this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num], true);
		}
	}

	// Token: 0x06003409 RID: 13321 RVA: 0x00117FA1 File Offset: 0x001161A1
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
		stream.SendNext(this.stateRemainingTime);
	}

	// Token: 0x0600340A RID: 13322 RVA: 0x00117FD0 File Offset: 0x001161D0
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		base.OnSerializeRead(stream, info);
		GorillaTagCompetitiveManager.GameState state = (GorillaTagCompetitiveManager.GameState)stream.ReceiveNext();
		this.stateRemainingTime = (float)stream.ReceiveNext();
		this.SetState(state);
	}

	// Token: 0x0600340B RID: 13323 RVA: 0x0011801C File Offset: 0x0011621C
	public void UpdateScoreboards()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = this.scoring.GetSortedScores();
		if (this.gameState == GorillaTagCompetitiveManager.GameState.Playing)
		{
			this.lastActiveTime = Time.time;
		}
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].UpdateScores(this.gameState, this.lastActiveTime, sortedScores, this.scoring.PlayerRankedTiers, this.scoring.ProjectedEloDeltas, this.currentInfected, this.scoring.Progression);
		}
	}

	// Token: 0x0600340C RID: 13324 RVA: 0x001180A4 File Offset: 0x001162A4
	public void DisplayScoreboardPredictedResults(bool bShow)
	{
		for (int i = 0; i < GorillaTagCompetitiveManager.scoreboards.Count; i++)
		{
			GorillaTagCompetitiveManager.scoreboards[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x0600340D RID: 13325 RVA: 0x001180D7 File Offset: 0x001162D7
	public void RegisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		if (!this.forceLeaveRoomVolumes.Contains(volume))
		{
			this.forceLeaveRoomVolumes.Add(volume);
		}
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x001180F3 File Offset: 0x001162F3
	public void UnregisterForcedLeaveVolume(GorillaTagCompetitiveForcedLeaveRoomVolume volume)
	{
		this.forceLeaveRoomVolumes.Remove(volume);
	}

	// Token: 0x0400424A RID: 16970
	[SerializeField]
	private float startCountdownDuration = 3f;

	// Token: 0x0400424B RID: 16971
	[SerializeField]
	private float roundDuration = 300f;

	// Token: 0x0400424C RID: 16972
	[SerializeField]
	private float postRoundDuration = 15f;

	// Token: 0x0400424D RID: 16973
	[SerializeField]
	private float waitingForPlayerPingRoomDuration = 60f;

	// Token: 0x0400424E RID: 16974
	private GorillaTagCompetitiveManager.GameState gameState;

	// Token: 0x0400424F RID: 16975
	private float stateRemainingTime;

	// Token: 0x04004250 RID: 16976
	private float lastActiveTime;

	// Token: 0x04004251 RID: 16977
	private float lastWaitingForPlayerPingRoomTime;

	// Token: 0x04004259 RID: 16985
	private RankedMultiplayerScore scoring;

	// Token: 0x0400425A RID: 16986
	private List<GorillaTagCompetitiveForcedLeaveRoomVolume> forceLeaveRoomVolumes = new List<GorillaTagCompetitiveForcedLeaveRoomVolume>();

	// Token: 0x0400425B RID: 16987
	private static List<GorillaTagCompetitiveScoreboard> scoreboards = new List<GorillaTagCompetitiveScoreboard>();

	// Token: 0x020007B7 RID: 1975
	public enum GameState
	{
		// Token: 0x0400425E RID: 16990
		None,
		// Token: 0x0400425F RID: 16991
		WaitingForPlayers,
		// Token: 0x04004260 RID: 16992
		StartingCountdown,
		// Token: 0x04004261 RID: 16993
		Playing,
		// Token: 0x04004262 RID: 16994
		PostRound
	}
}
