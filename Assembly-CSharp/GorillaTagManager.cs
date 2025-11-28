using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020007DE RID: 2014
public class GorillaTagManager : GorillaGameManager
{
	// Token: 0x060034E9 RID: 13545 RVA: 0x0011CEB8 File Offset: 0x0011B0B8
	public override void Awake()
	{
		base.Awake();
		this.currentInfectedArray = new int[10];
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
	}

	// Token: 0x060034EA RID: 13546 RVA: 0x0011CEF4 File Offset: 0x0011B0F4
	public override void StartPlaying()
	{
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentInfected.Count; i++)
			{
				this.tempPlayer = this.currentInfected[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentInfected.RemoveAt(i);
					i--;
				}
			}
			if (this.currentIt != null && !this.currentIt.InRoom())
			{
				this.currentIt = null;
			}
			if (this.lastInfectedPlayer != null && !this.lastInfectedPlayer.InRoom())
			{
				this.lastInfectedPlayer = null;
			}
			this.UpdateState();
		}
	}

	// Token: 0x060034EB RID: 13547 RVA: 0x0011CFA1 File Offset: 0x0011B1A1
	public override void StopPlaying()
	{
		base.StopPlaying();
		base.StopAllCoroutines();
		this.lastTaggedActorNr.Clear();
	}

	// Token: 0x060034EC RID: 13548 RVA: 0x0011CFBC File Offset: 0x0011B1BC
	public override void ResetGame()
	{
		base.ResetGame();
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
		this.currentInfected.Clear();
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.allInfected = false;
		this.isCurrentlyTag = false;
		this.waitingToStartNextInfectionGame = false;
		this.currentIt = null;
		this.lastInfectedPlayer = null;
	}

	// Token: 0x060034ED RID: 13549 RVA: 0x0011D038 File Offset: 0x0011B238
	public virtual void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (GameMode.ParticipatingPlayers.Count < 1)
			{
				this.isCurrentlyTag = true;
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.currentIt = null;
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				return;
			}
			if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int num2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num2], true);
				this.lastInfectedPlayer = GameMode.ParticipatingPlayers[num2];
				return;
			}
			if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int num3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num3], false);
				return;
			}
			if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int num4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num4], true);
				return;
			}
			if (!this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x060034EE RID: 13550 RVA: 0x0011D1A6 File Offset: 0x0011B3A6
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateState();
		}
		this.inspectorLocalPlayerSpeed = this.LocalPlayerSpeed();
	}

	// Token: 0x060034EF RID: 13551 RVA: 0x0011D1CC File Offset: 0x0011B3CC
	protected virtual IEnumerator InfectionRoundEndingCoroutine()
	{
		while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
		{
			this.InfectionRoundStart();
		}
		yield return null;
		yield break;
	}

	// Token: 0x060034F0 RID: 13552 RVA: 0x0011D1DC File Offset: 0x0011B3DC
	protected virtual void InfectionRoundStart()
	{
		this.ClearInfectionState();
		GameMode.RefreshPlayers();
		List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
		if (participatingPlayers.Count > 0)
		{
			int num = Random.Range(0, participatingPlayers.Count);
			int num2 = 0;
			while (num2 < 10 && participatingPlayers[num] == this.lastInfectedPlayer)
			{
				num = Random.Range(0, participatingPlayers.Count);
				num2++;
			}
			this.AddInfectedPlayer(participatingPlayers[num], true);
			this.lastInfectedPlayer = participatingPlayers[num];
			this.lastTag = (double)Time.time;
		}
	}

	// Token: 0x060034F1 RID: 13553 RVA: 0x0011D260 File Offset: 0x0011B460
	public virtual void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.allInfected = true;
		foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(netPlayer))
			{
				this.allInfected = false;
				break;
			}
		}
		if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
		{
			this.InfectionRoundEnd();
		}
	}

	// Token: 0x060034F2 RID: 13554 RVA: 0x0011D2F4 File Offset: 0x0011B4F4
	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
		{
			if (this.currentIt == netPlayer)
			{
				if (withTagFreeze)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, netPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, netPlayer, false);
				break;
			}
		}
	}

	// Token: 0x060034F3 RID: 13555 RVA: 0x0011D378 File Offset: 0x0011B578
	protected virtual void InfectionRoundEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
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
			base.StartCoroutine(this.InfectionRoundEndingCoroutine());
		}
	}

	// Token: 0x060034F4 RID: 13556 RVA: 0x0011D414 File Offset: 0x0011B614
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	// Token: 0x060034F5 RID: 13557 RVA: 0x0011D450 File Offset: 0x0011B650
	public override bool LocalIsTagged(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x060034F6 RID: 13558 RVA: 0x0011D470 File Offset: 0x0011B670
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		if (this.LocalCanTag(NetworkSystem.Instance.LocalPlayer, taggedPlayer) && (double)Time.time > this.lastQuestTagTime + (double)this.tagCoolDown)
		{
			PlayerGameEvents.MiscEvent("GameModeTag", 1);
			this.lastQuestTagTime = (double)Time.time;
			if (!this.isCurrentlyTag)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x060034F7 RID: 13559 RVA: 0x0011D4CC File Offset: 0x0011B6CC
	protected float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpMultiplier;
	}

	// Token: 0x060034F8 RID: 13560 RVA: 0x0011D520 File Offset: 0x0011B720
	protected float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpLimit;
	}

	// Token: 0x060034F9 RID: 13561 RVA: 0x0011D574 File Offset: 0x0011B774
	protected float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	// Token: 0x060034FA RID: 13562 RVA: 0x0011D5C4 File Offset: 0x0011B7C4
	protected float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	// Token: 0x060034FB RID: 13563 RVA: 0x0011D614 File Offset: 0x0011B814
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
			this.taggedRig.SetTaggedBy(this.taggingRig);
			if (this.isCurrentlyTag)
			{
				if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.ChangeCurrentIt(taggedPlayer, true);
					this.lastTag = (double)Time.time;
					this.HandleTagBroadcast(taggedPlayer, taggingPlayer);
					GameMode.BroadcastTag(taggedPlayer, taggingPlayer);
					return;
				}
			}
			else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
			{
				if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
				{
					GorillaNot.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
					return;
				}
				this.HandleTagBroadcast(taggedPlayer, taggingPlayer);
				GameMode.BroadcastTag(taggedPlayer, taggingPlayer);
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.AddInfectedPlayer(taggedPlayer, true);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x060034FC RID: 13564 RVA: 0x0011D79C File Offset: 0x0011B99C
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggedRig == null || this.waitingToStartNextInfectionGame || (double)Time.time < this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown))
			{
				return;
			}
			if (this.isCurrentlyTag)
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.ChangeCurrentIt(taggedPlayer, false);
				return;
			}
			if (!this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.AddInfectedPlayer(taggedPlayer, false);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x060034FD RID: 13565 RVA: 0x0011D83C File Offset: 0x0011BA3C
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && thisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	// Token: 0x060034FE RID: 13566 RVA: 0x0011D450 File Offset: 0x0011B650
	public bool IsInfected(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x060034FF RID: 13567 RVA: 0x0011D895 File Offset: 0x0011BA95
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (didTutorial)
				{
					this.AddInfectedPlayer(player, false);
				}
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06003500 RID: 13568 RVA: 0x0011D8D4 File Offset: 0x0011BAD4
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while (this.currentInfected.Contains(otherPlayer))
			{
				this.currentInfected.Remove(otherPlayer);
			}
			if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber))
			{
				if (GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
			}
			else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.UpdateInfectionState();
			}
			this.UpdateState();
		}
	}

	// Token: 0x06003501 RID: 13569 RVA: 0x0011D994 File Offset: 0x0011BB94
	private void CopyInfectedListToArray()
	{
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			this.currentInfectedArray[this.iterator1] = -1;
			this.iterator1++;
		}
		this.iterator1 = this.currentInfected.Count - 1;
		while (this.iterator1 >= 0)
		{
			if (this.currentInfected[this.iterator1] == null)
			{
				this.currentInfected.RemoveAt(this.iterator1);
			}
			this.iterator1--;
		}
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfected.Count)
		{
			this.currentInfectedArray[this.iterator1] = this.currentInfected[this.iterator1].ActorNumber;
			this.iterator1++;
		}
	}

	// Token: 0x06003502 RID: 13570 RVA: 0x0011DA74 File Offset: 0x0011BC74
	private void CopyInfectedArrayToList()
	{
		this.currentInfected.Clear();
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			if (this.currentInfectedArray[this.iterator1] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentInfectedArray[this.iterator1]);
				if (this.tempPlayer != null)
				{
					this.currentInfected.Add(this.tempPlayer);
				}
			}
			this.iterator1++;
		}
	}

	// Token: 0x06003503 RID: 13571 RVA: 0x0011DAF9 File Offset: 0x0011BCF9
	protected virtual void ChangeCurrentIt(NetPlayer newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

	// Token: 0x06003504 RID: 13572 RVA: 0x0011DB15 File Offset: 0x0011BD15
	public void SetisCurrentlyTag(bool newTagSetting)
	{
		if (newTagSetting)
		{
			this.isCurrentlyTag = true;
		}
		else
		{
			this.isCurrentlyTag = false;
		}
		RoomSystem.SendSoundEffectAll(2, 0.25f, false);
	}

	// Token: 0x06003505 RID: 13573 RVA: 0x0011DB36 File Offset: 0x0011BD36
	public virtual void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
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

	// Token: 0x06003506 RID: 13574 RVA: 0x0011DB76 File Offset: 0x0011BD76
	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.waitingToStartNextInfectionGame = false;
	}

	// Token: 0x06003507 RID: 13575 RVA: 0x0011DB8A File Offset: 0x0011BD8A
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x06003508 RID: 13576 RVA: 0x0011DBAB File Offset: 0x0011BDAB
	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.isCurrentlyTag)
		{
			this.UpdateTagState(true);
			return;
		}
		this.UpdateInfectionState();
	}

	// Token: 0x06003509 RID: 13577 RVA: 0x0011DBE8 File Offset: 0x0011BDE8
	public override void OnSerializeRead(object newData)
	{
		TagData tagData = (TagData)newData;
		this.isCurrentlyTag = tagData.isCurrentlyTag;
		this.tempItInt = tagData.currentItID;
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		tagData.infectedPlayerList.CopyTo(this.currentInfectedArray, true);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x0600350A RID: 13578 RVA: 0x0011DC58 File Offset: 0x0011BE58
	public override object OnSerializeWrite()
	{
		this.CopyInfectedListToArray();
		TagData tagData = default(TagData);
		tagData.isCurrentlyTag = this.isCurrentlyTag;
		tagData.currentItID = ((this.currentIt != null) ? this.currentIt.ActorNumber : -1);
		tagData.infectedPlayerList.CopyFrom(this.currentInfectedArray, 0, this.currentInfectedArray.Length);
		return tagData;
	}

	// Token: 0x0600350B RID: 13579 RVA: 0x0011DCC8 File Offset: 0x0011BEC8
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyInfectedListToArray();
		stream.SendNext(this.isCurrentlyTag);
		stream.SendNext((this.currentIt != null) ? this.currentIt.ActorNumber : -1);
		stream.SendNext(this.currentInfectedArray[0]);
		stream.SendNext(this.currentInfectedArray[1]);
		stream.SendNext(this.currentInfectedArray[2]);
		stream.SendNext(this.currentInfectedArray[3]);
		stream.SendNext(this.currentInfectedArray[4]);
		stream.SendNext(this.currentInfectedArray[5]);
		stream.SendNext(this.currentInfectedArray[6]);
		stream.SendNext(this.currentInfectedArray[7]);
		stream.SendNext(this.currentInfectedArray[8]);
		stream.SendNext(this.currentInfectedArray[9]);
		base.WriteLastTagged(stream);
	}

	// Token: 0x0600350C RID: 13580 RVA: 0x0011DDD4 File Offset: 0x0011BFD4
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		bool flag = this.currentIt == NetworkSystem.Instance.LocalPlayer;
		bool flag2 = this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer);
		this.isCurrentlyTag = (bool)stream.ReceiveNext();
		this.tempItInt = (int)stream.ReceiveNext();
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		this.currentInfectedArray[0] = (int)stream.ReceiveNext();
		this.currentInfectedArray[1] = (int)stream.ReceiveNext();
		this.currentInfectedArray[2] = (int)stream.ReceiveNext();
		this.currentInfectedArray[3] = (int)stream.ReceiveNext();
		this.currentInfectedArray[4] = (int)stream.ReceiveNext();
		this.currentInfectedArray[5] = (int)stream.ReceiveNext();
		this.currentInfectedArray[6] = (int)stream.ReceiveNext();
		this.currentInfectedArray[7] = (int)stream.ReceiveNext();
		this.currentInfectedArray[8] = (int)stream.ReceiveNext();
		this.currentInfectedArray[9] = (int)stream.ReceiveNext();
		base.ReadLastTagged(stream);
		this.CopyInfectedArrayToList();
		if (this.isCurrentlyTag)
		{
			if (!flag && this.currentIt == NetworkSystem.Instance.LocalPlayer)
			{
				this.lastQuestTagTime = (double)Time.time;
				return;
			}
		}
		else if (!flag2 && this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
		{
			this.lastQuestTagTime = (double)Time.time;
		}
	}

	// Token: 0x0600350D RID: 13581 RVA: 0x00027DED File Offset: 0x00025FED
	public override GameModeType GameType()
	{
		return GameModeType.Infection;
	}

	// Token: 0x0600350E RID: 13582 RVA: 0x0011DF7B File Offset: 0x0011C17B
	public override string GameModeName()
	{
		return "INFECTION";
	}

	// Token: 0x0600350F RID: 13583 RVA: 0x0011DF84 File Offset: 0x0011C184
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_INFECTION_ROOM_LABEL", out result, "(INFECTION GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_INFECTION_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06003510 RID: 13584 RVA: 0x0011DFAF File Offset: 0x0011C1AF
	public override void AddFusionDataBehaviour(NetworkObject netObject)
	{
		netObject.AddBehaviour<TagGameModeData>();
	}

	// Token: 0x06003511 RID: 13585 RVA: 0x0011DFB8 File Offset: 0x0011C1B8
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		if (this.isCurrentlyTag && forPlayer == this.currentIt)
		{
			return 1;
		}
		if (this.currentInfected.Contains(forPlayer))
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06003512 RID: 13586 RVA: 0x0011DFE0 File Offset: 0x0011C1E0
	public override float[] LocalPlayerSpeed()
	{
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
			if (this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
			{
				this.playerSpeed[0] = this.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = this.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
			this.playerSpeed[1] = this.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
			return this.playerSpeed;
		}
	}

	// Token: 0x0400439C RID: 17308
	public new const int k_defaultMatIndex = 0;

	// Token: 0x0400439D RID: 17309
	public const int k_itMatIndex = 1;

	// Token: 0x0400439E RID: 17310
	public const int k_infectedMatIndex = 2;

	// Token: 0x0400439F RID: 17311
	public float tagCoolDown = 5f;

	// Token: 0x040043A0 RID: 17312
	public int infectedModeThreshold = 4;

	// Token: 0x040043A1 RID: 17313
	public const byte ReportTagEvent = 1;

	// Token: 0x040043A2 RID: 17314
	public const byte ReportInfectionTagEvent = 2;

	// Token: 0x040043A3 RID: 17315
	[NonSerialized]
	public List<NetPlayer> currentInfected = new List<NetPlayer>(10);

	// Token: 0x040043A4 RID: 17316
	[NonSerialized]
	public int[] currentInfectedArray;

	// Token: 0x040043A5 RID: 17317
	[NonSerialized]
	public NetPlayer currentIt;

	// Token: 0x040043A6 RID: 17318
	[NonSerialized]
	public NetPlayer lastInfectedPlayer;

	// Token: 0x040043A7 RID: 17319
	public double lastTag;

	// Token: 0x040043A8 RID: 17320
	public double timeInfectedGameEnded;

	// Token: 0x040043A9 RID: 17321
	public bool waitingToStartNextInfectionGame;

	// Token: 0x040043AA RID: 17322
	public bool isCurrentlyTag;

	// Token: 0x040043AB RID: 17323
	private int tempItInt;

	// Token: 0x040043AC RID: 17324
	private int iterator1;

	// Token: 0x040043AD RID: 17325
	private NetPlayer tempPlayer;

	// Token: 0x040043AE RID: 17326
	private bool allInfected;

	// Token: 0x040043AF RID: 17327
	public float[] inspectorLocalPlayerSpeed;

	// Token: 0x040043B0 RID: 17328
	private protected VRRig taggingRig;

	// Token: 0x040043B1 RID: 17329
	private protected VRRig taggedRig;

	// Token: 0x040043B2 RID: 17330
	private NetPlayer lastTaggedPlayer;

	// Token: 0x040043B3 RID: 17331
	private double lastQuestTagTime;
}
