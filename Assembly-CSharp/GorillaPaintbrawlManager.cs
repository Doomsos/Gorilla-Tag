using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020007A9 RID: 1961
public sealed class GorillaPaintbrawlManager : GorillaGameManager
{
	// Token: 0x06003349 RID: 13129 RVA: 0x001147EC File Offset: 0x001129EC
	private void ActivatePaintbrawlBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.paintbrawlBalloons.gameObject.SetActive(enable);
		}
	}

	// Token: 0x0600334A RID: 13130 RVA: 0x0011481A File Offset: 0x00112A1A
	private bool HasFlag(GorillaPaintbrawlManager.PaintbrawlStatus state, GorillaPaintbrawlManager.PaintbrawlStatus statusFlag)
	{
		return (state & statusFlag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x0600334B RID: 13131 RVA: 0x00114822 File Offset: 0x00112A22
	public override GameModeType GameType()
	{
		return GameModeType.Paintbrawl;
	}

	// Token: 0x0600334C RID: 13132 RVA: 0x00114825 File Offset: 0x00112A25
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<BattleGameModeData>();
	}

	// Token: 0x0600334D RID: 13133 RVA: 0x0011482E File Offset: 0x00112A2E
	public override string GameModeName()
	{
		return "PAINTBRAWL";
	}

	// Token: 0x0600334E RID: 13134 RVA: 0x00114838 File Offset: 0x00112A38
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_PAINTBRAWL_ROOM_LABEL", out result, "(PAINTBRAWL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_PAINTBRAWL_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x0600334F RID: 13135 RVA: 0x00114864 File Offset: 0x00112A64
	private void ActivateDefaultSlingShot()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig != null && !Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
		}
	}

	// Token: 0x06003350 RID: 13136 RVA: 0x001148AD File Offset: 0x00112AAD
	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x001148C4 File Offset: 0x00112AC4
	public override void StartPlaying()
	{
		base.StartPlaying();
		this.ActivatePaintbrawlBalloons(true);
		this.VerifyPlayersInDict<int>(this.playerLives);
		this.VerifyPlayersInDict<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		this.VerifyPlayersInDict<float>(this.playerHitTimes);
		this.VerifyPlayersInDict<float>(this.playerStunTimes);
		this.CopyBattleDictToArray();
		this.UpdateBattleState();
	}

	// Token: 0x06003352 RID: 13138 RVA: 0x0011491C File Offset: 0x00112B1C
	public override void StopPlaying()
	{
		base.StopPlaying();
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			if (offlineVRRig.cosmeticSet.HasItem("Slingshot"))
			{
				instance.ApplyCosmeticItemToSet(offlineVRRig.cosmeticSet, itemFromDict, true, false);
			}
		}
		this.ActivatePaintbrawlBalloons(false);
		base.StopAllCoroutines();
		this.coroutineRunning = false;
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x0011498C File Offset: 0x00112B8C
	public override void ResetGame()
	{
		base.ResetGame();
		this.playerLives.Clear();
		this.playerStatusDict.Clear();
		this.playerHitTimes.Clear();
		this.playerStunTimes.Clear();
		for (int i = 0; i < this.playerActorNumberArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
			this.playerStatusArray[i] = GorillaPaintbrawlManager.PaintbrawlStatus.None;
		}
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x00114A00 File Offset: 0x00112C00
	private void VerifyPlayersInDict<T>(Dictionary<int, T> dict)
	{
		if (dict.Count < 1)
		{
			return;
		}
		int[] array = Enumerable.ToArray<int>(dict.Keys);
		for (int i = 0; i < array.Length; i++)
		{
			if (!Utils.PlayerInRoom(array[i]))
			{
				dict.Remove(array[i]);
			}
		}
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x00114A45 File Offset: 0x00112C45
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<PaintbrawlRPCs>();
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x00114A55 File Offset: 0x00112C55
	private void Transition(GorillaPaintbrawlManager.PaintbrawlState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x00114A80 File Offset: 0x00112C80
	public void UpdateBattleState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.currentState)
			{
			case GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers:
				if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.StartCountdown:
				if (this.teamBattle)
				{
					this.RandomizeTeams();
				}
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameStart:
				this.StartBattle();
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameRunning);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEnd);
					PlayerGameEvents.GameModeCompleteRound();
					GameMode.BroadcastRoundComplete();
				}
				if ((float)RoomSystem.PlayersInRoom.Count < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x00114B94 File Offset: 0x00112D94
	private bool CheckForGameEnd()
	{
		int num = 0;
		this.bcount = 0;
		this.rcount = 0;
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (this.playerLives.TryGetValue(netPlayer.ActorNumber, ref this.lives))
			{
				if (this.lives > 0)
				{
					num++;
					if (this.teamBattle && this.playerStatusDict.TryGetValue(netPlayer.ActorNumber, ref this.tempStatus))
					{
						if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam))
						{
							this.rcount++;
						}
						else if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam))
						{
							this.bcount++;
						}
					}
				}
			}
			else
			{
				this.playerLives.Add(netPlayer.ActorNumber, 0);
			}
		}
		return (this.teamBattle && (this.bcount == 0 || this.rcount == 0)) || (!this.teamBattle && num <= 1);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x00114CB8 File Offset: 0x00112EB8
	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f, false);
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					this.playerLives[netPlayer.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.GameStart;
		yield return null;
		yield break;
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x00114CC8 File Offset: 0x00112EC8
	public void StartBattle()
	{
		RoomSystem.SendSoundEffectAll(7, 0.5f, false);
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			this.playerLives[netPlayer.ActorNumber] = 3;
		}
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x00114D34 File Offset: 0x00112F34
	private bool EndBattleGame()
	{
		if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
		{
			RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.TaggedTime);
			RoomSystem.SendSoundEffectAll(2, 0.25f, false);
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x00114D69 File Offset: 0x00112F69
	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x00114D7F File Offset: 0x00112F7F
	public bool SlingshotHit(NetPlayer myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, ref this.lives) && this.lives > 0;
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x00114DA8 File Offset: 0x00112FA8
	public void ReportSlingshotHit(NetPlayer taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, player))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(player) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, ref this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			return;
		}
		if (this.GetPlayerLives(player) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, ref this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer))
				{
					this.tempView = rigContainer.Rig.netView;
				}
			}
		}
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x00114F44 File Offset: 0x00113144
	public override void HitPlayer(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient || this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, player, false);
		}
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x00114F90 File Offset: 0x00113190
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, ref this.lives) && this.lives > 0;
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x00114FB8 File Offset: 0x001131B8
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentState == GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaPaintbrawlManager.PaintbrawlStatus.None);
			this.CopyBattleDictToArray();
			if (this.teamBattle)
			{
				this.AddPlayerToCorrectTeam(newPlayer);
			}
		}
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x00115030 File Offset: 0x00113230
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x06003363 RID: 13155 RVA: 0x00115090 File Offset: 0x00113290
	public override void OnSerializeRead(object newData)
	{
		PaintbrawlData paintbrawlData = (PaintbrawlData)newData;
		paintbrawlData.playerActorNumberArray.CopyTo(this.playerActorNumberArray, true);
		paintbrawlData.playerLivesArray.CopyTo(this.playerLivesArray, true);
		paintbrawlData.playerStatusArray.CopyTo(this.playerStatusArray, true);
		this.currentState = paintbrawlData.currentPaintbrawlState;
		this.CopyArrayToBattleDict();
	}

	// Token: 0x06003364 RID: 13156 RVA: 0x001150F8 File Offset: 0x001132F8
	public override object OnSerializeWrite()
	{
		this.CopyBattleDictToArray();
		PaintbrawlData paintbrawlData = default(PaintbrawlData);
		paintbrawlData.playerActorNumberArray.CopyFrom(this.playerActorNumberArray, 0, this.playerActorNumberArray.Length);
		paintbrawlData.playerLivesArray.CopyFrom(this.playerLivesArray, 0, this.playerLivesArray.Length);
		paintbrawlData.playerStatusArray.CopyFrom(this.playerStatusArray, 0, this.playerStatusArray.Length);
		paintbrawlData.currentPaintbrawlState = this.currentState;
		return paintbrawlData;
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x00115180 File Offset: 0x00113380
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyBattleDictToArray();
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			stream.SendNext(this.playerActorNumberArray[i]);
			stream.SendNext(this.playerLivesArray[i]);
			stream.SendNext(this.playerStatusArray[i]);
		}
		stream.SendNext((int)this.currentState);
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x001151F0 File Offset: 0x001133F0
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerActorNumberArray[i] = (int)stream.ReceiveNext();
			this.playerLivesArray[i] = (int)stream.ReceiveNext();
			this.playerStatusArray[i] = (GorillaPaintbrawlManager.PaintbrawlStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaPaintbrawlManager.PaintbrawlState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x00115274 File Offset: 0x00113474
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaPaintbrawlManager.PaintbrawlStatus.None)
		{
			if (this.OnRedTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 11;
				}
			}
			else if (this.OnBlueTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 7;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 0;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 1;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 16;
				}
			}
		}
		return 0;
	}

	// Token: 0x06003368 RID: 13160 RVA: 0x001153C8 File Offset: 0x001135C8
	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(NetworkSystem.Instance.LocalPlayerID, ref this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	// Token: 0x06003369 RID: 13161 RVA: 0x001154A9 File Offset: 0x001136A9
	public override void Tick()
	{
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateBattleState();
		}
		this.ActivateDefaultSlingShot();
	}

	// Token: 0x0600336A RID: 13162 RVA: 0x001154CC File Offset: 0x001136CC
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			using (List<NetPlayer>.Enumerator enumerator2 = RoomSystem.PlayersInRoom.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ActorNumber == num)
					{
						this.playerInList = true;
					}
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	// Token: 0x0600336B RID: 13163 RVA: 0x00115588 File Offset: 0x00113788
	public int GetPlayerLives(NetPlayer player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, ref this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x001155B0 File Offset: 0x001137B0
	public bool PlayerInHitCooldown(NetPlayer player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, ref num) && num + this.hitCooldown > Time.time;
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x001155E4 File Offset: 0x001137E4
	public bool PlayerInStunCooldown(NetPlayer player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, ref num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x0011561E File Offset: 0x0011381E
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerStatus(NetPlayer player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, ref this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x00115641 File Offset: 0x00113841
	public bool OnRedTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x0011564C File Offset: 0x0011384C
	public bool OnRedTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x00115668 File Offset: 0x00113868
	public bool OnBlueTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x00115674 File Offset: 0x00113874
	public bool OnBlueTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x00115690 File Offset: 0x00113890
	public bool OnNoTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x001156A8 File Offset: 0x001138A8
	public bool OnNoTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x001156C4 File Offset: 0x001138C4
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		if (this.OnRedTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam;
		}
		if (this.OnBlueTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x001156E0 File Offset: 0x001138E0
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.GetPlayerTeam(playerStatus);
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x001156FC File Offset: 0x001138FC
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.GetPlayerLives(player) == 0;
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x00115708 File Offset: 0x00113908
	public bool OnSameTeam(GorillaPaintbrawlManager.PaintbrawlStatus playerA, GorillaPaintbrawlManager.PaintbrawlStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	// Token: 0x0600337A RID: 13178 RVA: 0x00115740 File Offset: 0x00113940
	public bool OnSameTeam(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x00115768 File Offset: 0x00113968
	public bool LocalCanHit(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x00115790 File Offset: 0x00113990
	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
		}
		this.keyValuePairs = Enumerable.ToArray<KeyValuePair<int, int>>(this.playerLives);
		int num = 0;
		while (num < this.playerLivesArray.Length && num < this.keyValuePairs.Length)
		{
			this.playerActorNumberArray[num] = this.keyValuePairs[num].Key;
			this.playerLivesArray[num] = this.keyValuePairs[num].Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(NetworkSystem.Instance.GetPlayer(this.keyValuePairs[num].Key));
			num++;
		}
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x0011584C File Offset: 0x00113A4C
	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != -1 && Utils.PlayerInRoom(this.playerActorNumberArray[i]))
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], ref this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x00115932 File Offset: 0x00113B32
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState | flag;
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x000DF85B File Offset: 0x000DDA5B
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlagExclusive(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return flag;
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x00115937 File Offset: 0x00113B37
	private GorillaPaintbrawlManager.PaintbrawlStatus ClearFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState & ~flag;
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x0011481A File Offset: 0x00112A1A
	private bool FlagIsSet(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return (currState & flag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x00115940 File Offset: 0x00113B40
	public void RandomizeTeams()
	{
		int[] array = new int[RoomSystem.PlayersInRoom.Count];
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = Enumerable.ToArray<int>(Enumerable.OrderBy<int, int>(array, (int x) => rand.Next()));
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = (rand.Next(0, 2) == 0) ? GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam : GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam;
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus2 = (paintbrawlStatus == GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam : GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam;
		for (int j = 0; j < RoomSystem.PlayersInRoom.Count; j++)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus3 = (array2[j] % 2 == 0) ? paintbrawlStatus2 : paintbrawlStatus;
			this.playerStatusDict[RoomSystem.PlayersInRoom[j].ActorNumber] = paintbrawlStatus3;
		}
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x00115A0C File Offset: 0x00113C0C
	public void AddPlayerToCorrectTeam(NetPlayer newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			if (this.playerStatusDict.ContainsKey(RoomSystem.PlayersInRoom[i].ActorNumber))
			{
				GorillaPaintbrawlManager.PaintbrawlStatus state = this.playerStatusDict[RoomSystem.PlayersInRoom[i].ActorNumber];
				this.rcount = (this.HasFlag(state, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((RoomSystem.PlayersInRoom.Count - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (RoomSystem.PlayersInRoom.Count - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		}
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x00115B30 File Offset: 0x00113D30
	private void InitializePlayerStatus()
	{
		this.keyValuePairsStatus = Enumerable.ToArray<KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>>(this.playerStatusDict);
		foreach (KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus> keyValuePair in this.keyValuePairsStatus)
		{
			this.playerStatusDict[keyValuePair.Key] = GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
		}
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x00115B80 File Offset: 0x00113D80
	private void UpdatePlayerStatus()
	{
		this.keyValuePairsStatus = Enumerable.ToArray<KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>>(this.playerStatusDict);
		foreach (KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus> keyValuePair in this.keyValuePairsStatus)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus playerTeam = this.GetPlayerTeam(keyValuePair.Value);
			if (this.playerLives.TryGetValue(keyValuePair.Key, ref this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated);
			}
			else if (this.playerHitTimes.TryGetValue(keyValuePair.Key, ref this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Hit);
			}
			else if (this.playerStunTimes.TryGetValue(keyValuePair.Key, ref this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Stunned);
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Grace);
				}
				else
				{
					this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal);
				}
			}
			else
			{
				this.playerStatusDict[keyValuePair.Key] = (playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal);
			}
		}
	}

	// Token: 0x040041C6 RID: 16838
	private float playerMin = 2f;

	// Token: 0x040041C7 RID: 16839
	public float tagCoolDown = 5f;

	// Token: 0x040041C8 RID: 16840
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	// Token: 0x040041C9 RID: 16841
	public Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusDict = new Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus>();

	// Token: 0x040041CA RID: 16842
	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	// Token: 0x040041CB RID: 16843
	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	// Token: 0x040041CC RID: 16844
	public int[] playerActorNumberArray = new int[]
	{
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1,
		-1
	};

	// Token: 0x040041CD RID: 16845
	public int[] playerLivesArray = new int[10];

	// Token: 0x040041CE RID: 16846
	public GorillaPaintbrawlManager.PaintbrawlStatus[] playerStatusArray = new GorillaPaintbrawlManager.PaintbrawlStatus[10];

	// Token: 0x040041CF RID: 16847
	public bool teamBattle = true;

	// Token: 0x040041D0 RID: 16848
	public int countDownTime;

	// Token: 0x040041D1 RID: 16849
	private float timeBattleEnded;

	// Token: 0x040041D2 RID: 16850
	public float hitCooldown = 3f;

	// Token: 0x040041D3 RID: 16851
	public float stunGracePeriod = 2f;

	// Token: 0x040041D4 RID: 16852
	public object objRef;

	// Token: 0x040041D5 RID: 16853
	private bool playerInList;

	// Token: 0x040041D6 RID: 16854
	private bool coroutineRunning;

	// Token: 0x040041D7 RID: 16855
	private int lives;

	// Token: 0x040041D8 RID: 16856
	private int outLives;

	// Token: 0x040041D9 RID: 16857
	private int bcount;

	// Token: 0x040041DA RID: 16858
	private int rcount;

	// Token: 0x040041DB RID: 16859
	private int randInt;

	// Token: 0x040041DC RID: 16860
	private float outHitTime;

	// Token: 0x040041DD RID: 16861
	private NetworkView tempView;

	// Token: 0x040041DE RID: 16862
	private KeyValuePair<int, int>[] keyValuePairs;

	// Token: 0x040041DF RID: 16863
	private KeyValuePair<int, GorillaPaintbrawlManager.PaintbrawlStatus>[] keyValuePairsStatus;

	// Token: 0x040041E0 RID: 16864
	private GorillaPaintbrawlManager.PaintbrawlStatus tempStatus;

	// Token: 0x040041E1 RID: 16865
	private GorillaPaintbrawlManager.PaintbrawlState currentState;

	// Token: 0x020007AA RID: 1962
	public enum PaintbrawlStatus
	{
		// Token: 0x040041E3 RID: 16867
		RedTeam = 1,
		// Token: 0x040041E4 RID: 16868
		BlueTeam,
		// Token: 0x040041E5 RID: 16869
		Normal = 4,
		// Token: 0x040041E6 RID: 16870
		Hit = 8,
		// Token: 0x040041E7 RID: 16871
		Stunned = 16,
		// Token: 0x040041E8 RID: 16872
		Grace = 32,
		// Token: 0x040041E9 RID: 16873
		Eliminated = 64,
		// Token: 0x040041EA RID: 16874
		None = 0
	}

	// Token: 0x020007AB RID: 1963
	public enum PaintbrawlState
	{
		// Token: 0x040041EC RID: 16876
		NotEnoughPlayers,
		// Token: 0x040041ED RID: 16877
		GameEnd,
		// Token: 0x040041EE RID: 16878
		GameEndWaiting,
		// Token: 0x040041EF RID: 16879
		StartCountdown,
		// Token: 0x040041F0 RID: 16880
		CountingDownToStart,
		// Token: 0x040041F1 RID: 16881
		GameStart,
		// Token: 0x040041F2 RID: 16882
		GameRunning
	}
}
