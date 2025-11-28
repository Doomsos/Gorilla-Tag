using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DEB RID: 3563
	public sealed class GorillaFreezeTagManager : GorillaTagManager
	{
		// Token: 0x060058C2 RID: 22722 RVA: 0x001867CB File Offset: 0x001849CB
		public override GameModeType GameType()
		{
			return GameModeType.FreezeTag;
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x001C6A32 File Offset: 0x001C4C32
		public override string GameModeName()
		{
			return "FREEZE TAG";
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x001C6A3C File Offset: 0x001C4C3C
		public override string GameModeNameRoomLabel()
		{
			string result;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_FREEZE_TAG_ROOM_LABEL", out result, "(FREEZE TAG GAME)"))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_FREEZE_TAG_ROOM_LABEL]");
			}
			return result;
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x001C6A67 File Offset: 0x001C4C67
		public override void Awake()
		{
			base.Awake();
			this.fastJumpLimitCached = this.fastJumpLimit;
			this.fastJumpMultiplierCached = this.fastJumpMultiplier;
			this.slowJumpLimitCached = this.slowJumpLimit;
			this.slowJumpMultiplierCached = this.slowJumpMultiplier;
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x001C6AA0 File Offset: 0x001C4CA0
		public override void UpdateState()
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (KeyValuePair<NetPlayer, float> keyValuePair in Enumerable.ToList<KeyValuePair<NetPlayer, float>>(this.currentFrozen))
				{
					if (Time.time - keyValuePair.Value >= this.freezeDuration)
					{
						this.currentFrozen.Remove(keyValuePair.Key);
						this.AddInfectedPlayer(keyValuePair.Key, false);
						RoomSystem.SendSoundEffectAll(11, 0.25f, false);
					}
				}
				if (GameMode.ParticipatingPlayers.Count < 1)
				{
					this.ResetGame();
					base.SetisCurrentlyTag(true);
					return;
				}
				if (this.isCurrentlyTag && this.currentIt == null)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				else if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
				{
					this.ResetGame();
					int num2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num2], true);
				}
				else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
				{
					this.ResetGame();
					base.SetisCurrentlyTag(true);
					int num3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num3], false);
				}
				else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
				{
					int num4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num4], true);
				}
				bool flag = true;
				foreach (NetPlayer player in GameMode.ParticipatingPlayers)
				{
					if (!this.IsFrozen(player) && !base.IsInfected(player))
					{
						flag = false;
						break;
					}
				}
				if (flag && !this.isCurrentlyTag)
				{
					this.InfectionRoundEnd();
				}
			}
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x001C6CD0 File Offset: 0x001C4ED0
		public override void Tick()
		{
			base.Tick();
			if (this.localVRRig)
			{
				this.localVRRig.IsFrozen = this.IsFrozen(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x001C6D00 File Offset: 0x001C4F00
		public override void StartPlaying()
		{
			base.StartPlaying();
			this.localVRRig = this.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (NetPlayer netPlayer in this.lastRoundInfectedPlayers.ToArray())
				{
					if (netPlayer != null && !netPlayer.InRoom)
					{
						this.lastRoundInfectedPlayers.Remove(netPlayer);
					}
				}
				foreach (NetPlayer netPlayer2 in this.currentRoundInfectedPlayers.ToArray())
				{
					if (netPlayer2 != null && !netPlayer2.InRoom)
					{
						this.currentRoundInfectedPlayers.Remove(netPlayer2);
					}
				}
			}
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x001C6DA4 File Offset: 0x001C4FA4
		public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.taggingRig = this.FindPlayerVRRig(taggingPlayer);
				this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
				if (this.taggingRig == null || this.taggedRig == null)
				{
					return;
				}
				Debug.LogWarning("Report TAG - tagged " + this.taggedRig.playerNameVisible + ", tagging " + this.taggingRig.playerNameVisible);
				if (this.isCurrentlyTag)
				{
					if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
					{
						base.AddLastTagged(taggedPlayer, taggingPlayer);
						this.ChangeCurrentIt(taggedPlayer, false);
						this.lastTag = (double)Time.time;
						return;
					}
				}
				else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && !this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.AddFrozenPlayer(taggedPlayer);
					return;
				}
				else if (!this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					this.UnfreezePlayer(taggedPlayer);
				}
			}
		}

		// Token: 0x060058CA RID: 22730 RVA: 0x001C6FD4 File Offset: 0x001C51D4
		public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
		{
			if (this.isCurrentlyTag)
			{
				return myPlayer == this.currentIt && myPlayer != otherPlayer;
			}
			return (this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(otherPlayer) && !this.currentInfected.Contains(otherPlayer)) || (!this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(myPlayer) && (this.currentInfected.Contains(otherPlayer) || this.currentFrozen.ContainsKey(otherPlayer)));
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001C7063 File Offset: 0x001C5263
		public override bool LocalIsTagged(NetPlayer player)
		{
			if (this.isCurrentlyTag)
			{
				return this.currentIt == player;
			}
			return this.currentInfected.Contains(player) || this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001C7093 File Offset: 0x001C5293
		public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GameMode.RefreshPlayers();
				if (!this.isCurrentlyTag && !base.IsInfected(player))
				{
					this.AddInfectedPlayer(player, true);
					this.currentRoundInfectedPlayers.Add(player);
				}
				this.UpdateInfectionState();
			}
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x001C70D1 File Offset: 0x001C52D1
		protected override IEnumerator InfectionRoundEndingCoroutine()
		{
			while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.currentFrozen.Clear();
				GameMode.RefreshPlayers();
				this.lastRoundInfectedPlayers.Clear();
				this.lastRoundInfectedPlayers.AddRange(this.currentRoundInfectedPlayers);
				this.currentRoundInfectedPlayers.Clear();
				List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
				int num = 0;
				if (participatingPlayers.Count > 0 && participatingPlayers.Count < this.infectMorePlayerLowerThreshold)
				{
					num = 1;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerLowerThreshold && participatingPlayers.Count < this.infectMorePlayerUpperThreshold)
				{
					num = 2;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerUpperThreshold)
				{
					num = 3;
				}
				for (int i = 0; i < num; i++)
				{
					this.TryAddNewInfectedPlayer();
				}
				this.lastTag = (double)Time.time;
			}
			yield return null;
			yield break;
		}

		// Token: 0x060058CE RID: 22734 RVA: 0x001C70E0 File Offset: 0x001C52E0
		public override void ResetGame()
		{
			base.ResetGame();
			this.currentFrozen.Clear();
			this.currentRoundInfectedPlayers.Clear();
			this.lastRoundInfectedPlayers.Clear();
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x0011DB56 File Offset: 0x0011BD56
		private new void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.currentInfected.Add(infectedPlayer);
				if (!withTagStop)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, infectedPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer, false);
				this.UpdateInfectionState();
			}
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x001C710C File Offset: 0x001C530C
		private void TryAddNewInfectedPlayer()
		{
			List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
			int num = Random.Range(0, participatingPlayers.Count);
			int num2 = 0;
			while (num2 < 10 && this.lastRoundInfectedPlayers.Contains(participatingPlayers[num]))
			{
				num = Random.Range(0, participatingPlayers.Count);
				num2++;
			}
			this.AddInfectedPlayer(participatingPlayers[num], true);
			this.currentRoundInfectedPlayers.Add(participatingPlayers[num]);
		}

		// Token: 0x060058D1 RID: 22737 RVA: 0x001C717A File Offset: 0x001C537A
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (this.isCurrentlyTag && forPlayer == this.currentIt)
			{
				return 14;
			}
			if (this.currentInfected.Contains(forPlayer))
			{
				return 14;
			}
			return 0;
		}

		// Token: 0x060058D2 RID: 22738 RVA: 0x001C71A4 File Offset: 0x001C53A4
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			NetPlayer netPlayer = rig.isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : rig.creator;
			rig.UpdateFrozenEffect(this.IsFrozen(netPlayer));
			int materialIndex = this.MyMatIndex(netPlayer);
			rig.ChangeMaterialLocal(materialIndex);
		}

		// Token: 0x060058D3 RID: 22739 RVA: 0x001C71E8 File Offset: 0x001C53E8
		private void UnfreezePlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Remove(taggedPlayer);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.UnTagged, taggedPlayer);
				RoomSystem.SendSoundEffectAll(10, 0.25f, true);
			}
		}

		// Token: 0x060058D4 RID: 22740 RVA: 0x001C7228 File Offset: 0x001C5428
		private void AddFrozenPlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && !this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Add(taggedPlayer, Time.time);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.FrozenTime, taggedPlayer);
				RoomSystem.SendSoundEffectAll(9, 0.25f, false);
				RoomSystem.SendSoundEffectToPlayer(12, 0.05f, taggedPlayer, false);
			}
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x001C7282 File Offset: 0x001C5482
		public bool IsFrozen(NetPlayer player)
		{
			return this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x001C7290 File Offset: 0x001C5490
		public override float[] LocalPlayerSpeed()
		{
			this.fastJumpLimit = this.fastJumpLimitCached;
			this.fastJumpMultiplier = this.fastJumpMultiplierCached;
			this.slowJumpLimit = this.slowJumpLimitCached;
			this.slowJumpMultiplier = this.slowJumpMultiplierCached;
			if (this.isCurrentlyTag)
			{
				if (NetworkSystem.Instance.LocalPlayer == this.currentIt)
				{
					this.playerSpeed[0] = this.fastJumpLimit;
					this.playerSpeed[1] = this.fastJumpMultiplier;
					return this.playerSpeed;
				}
				this.playerSpeed[0] = this.slowJumpLimit;
				this.playerSpeed[1] = this.slowJumpMultiplier;
				return this.playerSpeed;
			}
			else
			{
				if (!this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer) && !this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.playerSpeed[0] = base.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
					this.playerSpeed[1] = base.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
					return this.playerSpeed;
				}
				if (this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.fastJumpLimit = this.frozenPlayerFastJumpLimit;
					this.fastJumpMultiplier = this.frozenPlayerFastJumpMultiplier;
					this.slowJumpLimit = this.frozenPlayerSlowJumpLimit;
					this.slowJumpMultiplier = this.frozenPlayerSlowJumpMultiplier;
				}
				this.playerSpeed[0] = base.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = base.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x001C7414 File Offset: 0x001C5614
		public int GetFrozenHandTapAudioIndex()
		{
			int num = Random.Range(0, this.frozenHandTapIndices.Length);
			return this.frozenHandTapIndices[num];
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x001C7438 File Offset: 0x001C5638
		public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber) && GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				if (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				if (this.currentFrozen.ContainsKey(otherPlayer))
				{
					this.currentFrozen.Remove(otherPlayer);
				}
				this.UpdateState();
			}
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x001C74F0 File Offset: 0x001C56F0
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
			{
				vrrig.ForceResetFrozenEffect();
			}
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x001C754C File Offset: 0x001C574C
		public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeRead(stream, info);
			this.currentFrozen.Clear();
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int playerID = (int)stream.ReceiveNext();
				float num2 = (float)stream.ReceiveNext();
				NetPlayer player = NetworkSystem.Instance.GetPlayer(playerID);
				this.currentFrozen.Add(player, num2);
			}
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x001C75B8 File Offset: 0x001C57B8
		public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeWrite(stream, info);
			stream.SendNext(this.currentFrozen.Count);
			foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen)
			{
				stream.SendNext(keyValuePair.Key.ActorNumber);
				stream.SendNext(keyValuePair.Value);
			}
		}

		// Token: 0x040065FC RID: 26108
		public Dictionary<NetPlayer, float> currentFrozen = new Dictionary<NetPlayer, float>(10);

		// Token: 0x040065FD RID: 26109
		public float freezeDuration;

		// Token: 0x040065FE RID: 26110
		public int infectMorePlayerLowerThreshold = 6;

		// Token: 0x040065FF RID: 26111
		public int infectMorePlayerUpperThreshold = 10;

		// Token: 0x04006600 RID: 26112
		[Space]
		[Header("Frozen player jump settings")]
		public float frozenPlayerFastJumpLimit;

		// Token: 0x04006601 RID: 26113
		public float frozenPlayerFastJumpMultiplier;

		// Token: 0x04006602 RID: 26114
		public float frozenPlayerSlowJumpLimit;

		// Token: 0x04006603 RID: 26115
		public float frozenPlayerSlowJumpMultiplier;

		// Token: 0x04006604 RID: 26116
		[GorillaSoundLookup]
		public int[] frozenHandTapIndices;

		// Token: 0x04006605 RID: 26117
		private float fastJumpLimitCached;

		// Token: 0x04006606 RID: 26118
		private float fastJumpMultiplierCached;

		// Token: 0x04006607 RID: 26119
		private float slowJumpLimitCached;

		// Token: 0x04006608 RID: 26120
		private float slowJumpMultiplierCached;

		// Token: 0x04006609 RID: 26121
		private VRRig localVRRig;

		// Token: 0x0400660A RID: 26122
		private int hapticStrength;

		// Token: 0x0400660B RID: 26123
		private List<NetPlayer> currentRoundInfectedPlayers = new List<NetPlayer>(10);

		// Token: 0x0400660C RID: 26124
		private List<NetPlayer> lastRoundInfectedPlayers = new List<NetPlayer>(10);
	}
}
