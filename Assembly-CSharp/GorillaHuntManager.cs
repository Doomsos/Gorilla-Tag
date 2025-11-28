using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200078F RID: 1935
public sealed class GorillaHuntManager : GorillaGameManager
{
	// Token: 0x060032A7 RID: 12967 RVA: 0x000126CB File Offset: 0x000108CB
	public override GameModeType GameType()
	{
		return GameModeType.HuntDown;
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x0011137F File Offset: 0x0010F57F
	public override string GameModeName()
	{
		return "HUNTDOWN";
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x00111388 File Offset: 0x0010F588
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_HUNT_ROOM_LABEL", out result, "(HUNTDOWN GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_HUNT_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x060032AA RID: 12970 RVA: 0x001113B3 File Offset: 0x0010F5B3
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<HuntGameModeData>();
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x001113BC File Offset: 0x0010F5BC
	public override void StartPlaying()
	{
		base.StartPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(true);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentHunted.Count; i++)
			{
				this.tempPlayer = this.currentHunted[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentHunted.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < this.currentTarget.Count; i++)
			{
				this.tempPlayer = this.currentTarget[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentTarget.RemoveAt(i);
					i--;
				}
			}
			this.UpdateState();
		}
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x00111493 File Offset: 0x0010F693
	public override void StopPlaying()
	{
		base.StopPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
		base.StopAllCoroutines();
	}

	// Token: 0x060032AD RID: 12973 RVA: 0x001114B8 File Offset: 0x0010F6B8
	public override void ResetGame()
	{
		base.ResetGame();
		this.currentHunted.Clear();
		this.currentTarget.Clear();
		for (int i = 0; i < this.currentHuntedArray.Length; i++)
		{
			this.currentHuntedArray[i] = -1;
			this.currentTargetArray[i] = -1;
		}
		this.huntStarted = false;
		this.waitingToStartNextHuntGame = false;
		this.inStartCountdown = false;
		this.timeHuntGameEnded = 0.0;
		this.countDownTime = 0;
		this.timeLastSlowTagged = 0f;
	}

	// Token: 0x060032AE RID: 12974 RVA: 0x0011153C File Offset: 0x0010F73C
	public void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (NetworkSystem.Instance.RoomPlayerCount <= 3)
			{
				this.CleanUpHunt();
				this.huntStarted = false;
				this.waitingToStartNextHuntGame = false;
				this.iterator1 = 0;
				while (this.iterator1 < RoomSystem.PlayersInRoom.Count)
				{
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, RoomSystem.PlayersInRoom[this.iterator1], false);
					this.iterator1++;
				}
				return;
			}
			if (NetworkSystem.Instance.RoomPlayerCount > 3 && !this.huntStarted && !this.waitingToStartNextHuntGame && !this.inStartCountdown)
			{
				Utils.Log("<color=red> there are enough players</color>", this);
				base.StartCoroutine(this.StartHuntCountdown());
				return;
			}
			this.UpdateHuntState();
		}
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x00111603 File Offset: 0x0010F803
	public void CleanUpHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.currentHunted.Clear();
			this.currentTarget.Clear();
		}
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x00111627 File Offset: 0x0010F827
	public IEnumerator StartHuntCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.inStartCountdown)
		{
			this.inStartCountdown = true;
			this.countDownTime = 5;
			this.CleanUpHunt();
			while (this.countDownTime > 0)
			{
				yield return new WaitForSeconds(1f);
				this.countDownTime--;
			}
			this.StartHunt();
		}
		yield return null;
		yield break;
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x00111638 File Offset: 0x0010F838
	public void StartHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.huntStarted = true;
			this.waitingToStartNextHuntGame = false;
			this.countDownTime = 0;
			this.inStartCountdown = false;
			this.CleanUpHunt();
			this.iterator1 = 0;
			while (this.iterator1 < Enumerable.Count<NetPlayer>(NetworkSystem.Instance.AllNetPlayers))
			{
				if (this.currentTarget.Count < 10)
				{
					this.currentTarget.Add(NetworkSystem.Instance.AllNetPlayers[this.iterator1]);
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, NetworkSystem.Instance.AllNetPlayers[this.iterator1], false);
				}
				this.iterator1++;
			}
			this.RandomizePlayerList(ref this.currentTarget);
		}
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x001116F8 File Offset: 0x0010F8F8
	public void RandomizePlayerList(ref List<NetPlayer> listToRandomize)
	{
		for (int i = 0; i < listToRandomize.Count - 1; i++)
		{
			this.tempRandIndex = Random.Range(i, listToRandomize.Count);
			this.tempRandPlayer = listToRandomize[i];
			listToRandomize[i] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandPlayer;
		}
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x00111762 File Offset: 0x0010F962
	public IEnumerator HuntEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while ((double)Time.time < this.timeHuntGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.waitingToStartNextHuntGame)
			{
				base.StartCoroutine(this.StartHuntCountdown());
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x00111774 File Offset: 0x0010F974
	public void UpdateHuntState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.notHuntedCount = 0;
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (this.currentTarget.Contains(netPlayer) && !this.currentHunted.Contains(netPlayer))
				{
					this.notHuntedCount++;
				}
			}
			if (this.notHuntedCount <= 2 && this.huntStarted)
			{
				this.EndHuntGame();
			}
		}
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x00111814 File Offset: 0x0010FA14
	private void EndHuntGame()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, false);
			}
			this.huntStarted = false;
			this.timeHuntGameEnded = (double)Time.time;
			this.waitingToStartNextHuntGame = true;
			base.StartCoroutine(this.HuntEnd());
		}
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x001118A8 File Offset: 0x0010FAA8
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.waitingToStartNextHuntGame || this.countDownTime > 0 || GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Frozen)
		{
			return false;
		}
		if (this.currentHunted.Contains(myPlayer) && !this.currentHunted.Contains(otherPlayer) && Time.time > this.timeLastSlowTagged + 1f)
		{
			this.timeLastSlowTagged = Time.time;
			return true;
		}
		return this.IsTargetOf(myPlayer, otherPlayer);
	}

	// Token: 0x060032B7 RID: 12983 RVA: 0x0011191E File Offset: 0x0010FB1E
	public override bool LocalIsTagged(NetPlayer player)
	{
		return !this.waitingToStartNextHuntGame && this.countDownTime <= 0 && this.currentHunted.Contains(player);
	}

	// Token: 0x060032B8 RID: 12984 RVA: 0x00111940 File Offset: 0x0010FB40
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame)
		{
			if ((this.currentHunted.Contains(taggingPlayer) || !this.currentTarget.Contains(taggingPlayer)) && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				return;
			}
			if (this.IsTargetOf(taggingPlayer, taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
				this.currentHunted.Add(taggedPlayer);
				this.UpdateHuntState();
			}
		}
	}

	// Token: 0x060032B9 RID: 12985 RVA: 0x001119E4 File Offset: 0x0010FBE4
	public bool IsTargetOf(NetPlayer huntingPlayer, NetPlayer huntedPlayer)
	{
		return !this.currentHunted.Contains(huntingPlayer) && !this.currentHunted.Contains(huntedPlayer) && this.currentTarget.Contains(huntingPlayer) && this.currentTarget.Contains(huntedPlayer) && huntedPlayer == this.GetTargetOf(huntingPlayer);
	}

	// Token: 0x060032BA RID: 12986 RVA: 0x00111A38 File Offset: 0x0010FC38
	public NetPlayer GetTargetOf(NetPlayer netPlayer)
	{
		if (this.currentHunted.Contains(netPlayer) || !this.currentTarget.Contains(netPlayer))
		{
			return null;
		}
		this.tempTargetIndex = this.currentTarget.IndexOf(netPlayer);
		for (int num = (this.tempTargetIndex + 1) % this.currentTarget.Count; num != this.tempTargetIndex; num = (num + 1) % this.currentTarget.Count)
		{
			if (this.currentTarget[num] == netPlayer)
			{
				return null;
			}
			if (!this.currentHunted.Contains(this.currentTarget[num]) && this.currentTarget[num] != null)
			{
				return this.currentTarget[num];
			}
		}
		return null;
	}

	// Token: 0x060032BB RID: 12987 RVA: 0x00111AEC File Offset: 0x0010FCEC
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
		{
			RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			this.currentHunted.Add(taggedPlayer);
			this.UpdateHuntState();
		}
	}

	// Token: 0x060032BC RID: 12988 RVA: 0x00111B4F File Offset: 0x0010FD4F
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(player) && this.currentTarget.Contains(player);
	}

	// Token: 0x060032BD RID: 12989 RVA: 0x00111B75 File Offset: 0x0010FD75
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
	}

	// Token: 0x060032BE RID: 12990 RVA: 0x00111B89 File Offset: 0x0010FD89
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (!this.waitingToStartNextHuntGame && this.huntStarted)
			{
				this.currentHunted.Add(player);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060032BF RID: 12991 RVA: 0x00111BC4 File Offset: 0x0010FDC4
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentTarget.Contains(otherPlayer))
			{
				this.currentTarget.Remove(otherPlayer);
			}
			if (this.currentHunted.Contains(otherPlayer))
			{
				this.currentHunted.Remove(otherPlayer);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060032C0 RID: 12992 RVA: 0x00111C20 File Offset: 0x0010FE20
	private void CopyHuntDataListToArray()
	{
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < 10)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = -1;
			this.currentTargetArray[this.copyListToArrayIndex] = -1;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = this.currentHunted.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentHunted[this.copyListToArrayIndex] == null)
			{
				this.currentHunted.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = this.currentTarget.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentTarget[this.copyListToArrayIndex] == null)
			{
				this.currentTarget.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentHunted.Count)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = this.currentHunted[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentTarget.Count)
		{
			this.currentTargetArray[this.copyListToArrayIndex] = this.currentTarget[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x00111DA4 File Offset: 0x0010FFA4
	private void CopyHuntDataArrayToList()
	{
		this.currentTarget.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentTargetArray.Length)
		{
			if (this.currentTargetArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentTargetArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentTarget.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
		this.currentHunted.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentHuntedArray.Length)
		{
			if (this.currentHuntedArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentHuntedArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentHunted.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x00111EA1 File Offset: 0x001100A1
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x060032C3 RID: 12995 RVA: 0x00111EC2 File Offset: 0x001100C2
	public void CopyRoomDataToLocalData()
	{
		this.waitingToStartNextHuntGame = false;
		this.UpdateHuntState();
	}

	// Token: 0x060032C4 RID: 12996 RVA: 0x00111ED4 File Offset: 0x001100D4
	public override void OnSerializeRead(object newData)
	{
		HuntData huntData = (HuntData)newData;
		huntData.currentHuntedArray.CopyTo(this.currentHuntedArray, true);
		huntData.currentTargetArray.CopyTo(this.currentTargetArray, true);
		this.huntStarted = huntData.huntStarted;
		this.waitingToStartNextHuntGame = huntData.waitingToStartNextHuntGame;
		this.countDownTime = huntData.countDownTime;
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060032C5 RID: 12997 RVA: 0x00111F48 File Offset: 0x00110148
	public override object OnSerializeWrite()
	{
		this.CopyHuntDataListToArray();
		HuntData huntData = default(HuntData);
		huntData.currentHuntedArray.CopyFrom(this.currentHuntedArray, 0, this.currentHuntedArray.Length);
		huntData.currentTargetArray.CopyFrom(this.currentTargetArray, 0, this.currentTargetArray.Length);
		huntData.huntStarted = this.huntStarted;
		huntData.waitingToStartNextHuntGame = this.waitingToStartNextHuntGame;
		huntData.countDownTime = this.countDownTime;
		return huntData;
	}

	// Token: 0x060032C6 RID: 12998 RVA: 0x00111FD8 File Offset: 0x001101D8
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyHuntDataListToArray();
		stream.SendNext(this.currentHuntedArray[0]);
		stream.SendNext(this.currentHuntedArray[1]);
		stream.SendNext(this.currentHuntedArray[2]);
		stream.SendNext(this.currentHuntedArray[3]);
		stream.SendNext(this.currentHuntedArray[4]);
		stream.SendNext(this.currentHuntedArray[5]);
		stream.SendNext(this.currentHuntedArray[6]);
		stream.SendNext(this.currentHuntedArray[7]);
		stream.SendNext(this.currentHuntedArray[8]);
		stream.SendNext(this.currentHuntedArray[9]);
		stream.SendNext(this.currentTargetArray[0]);
		stream.SendNext(this.currentTargetArray[1]);
		stream.SendNext(this.currentTargetArray[2]);
		stream.SendNext(this.currentTargetArray[3]);
		stream.SendNext(this.currentTargetArray[4]);
		stream.SendNext(this.currentTargetArray[5]);
		stream.SendNext(this.currentTargetArray[6]);
		stream.SendNext(this.currentTargetArray[7]);
		stream.SendNext(this.currentTargetArray[8]);
		stream.SendNext(this.currentTargetArray[9]);
		stream.SendNext(this.huntStarted);
		stream.SendNext(this.waitingToStartNextHuntGame);
		stream.SendNext(this.countDownTime);
	}

	// Token: 0x060032C7 RID: 12999 RVA: 0x0011219C File Offset: 0x0011039C
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentHuntedArray[0] = (int)stream.ReceiveNext();
		this.currentHuntedArray[1] = (int)stream.ReceiveNext();
		this.currentHuntedArray[2] = (int)stream.ReceiveNext();
		this.currentHuntedArray[3] = (int)stream.ReceiveNext();
		this.currentHuntedArray[4] = (int)stream.ReceiveNext();
		this.currentHuntedArray[5] = (int)stream.ReceiveNext();
		this.currentHuntedArray[6] = (int)stream.ReceiveNext();
		this.currentHuntedArray[7] = (int)stream.ReceiveNext();
		this.currentHuntedArray[8] = (int)stream.ReceiveNext();
		this.currentHuntedArray[9] = (int)stream.ReceiveNext();
		this.currentTargetArray[0] = (int)stream.ReceiveNext();
		this.currentTargetArray[1] = (int)stream.ReceiveNext();
		this.currentTargetArray[2] = (int)stream.ReceiveNext();
		this.currentTargetArray[3] = (int)stream.ReceiveNext();
		this.currentTargetArray[4] = (int)stream.ReceiveNext();
		this.currentTargetArray[5] = (int)stream.ReceiveNext();
		this.currentTargetArray[6] = (int)stream.ReceiveNext();
		this.currentTargetArray[7] = (int)stream.ReceiveNext();
		this.currentTargetArray[8] = (int)stream.ReceiveNext();
		this.currentTargetArray[9] = (int)stream.ReceiveNext();
		this.huntStarted = (bool)stream.ReceiveNext();
		this.waitingToStartNextHuntGame = (bool)stream.ReceiveNext();
		this.countDownTime = (int)stream.ReceiveNext();
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x00112360 File Offset: 0x00110560
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		NetPlayer targetOf = this.GetTargetOf(forPlayer);
		if (this.currentHunted.Contains(forPlayer) || (this.huntStarted && targetOf == null))
		{
			return 3;
		}
		return 0;
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x00112394 File Offset: 0x00110594
	public override float[] LocalPlayerSpeed()
	{
		if (this.currentHunted.Contains(NetworkSystem.Instance.LocalPlayer) || (this.huntStarted && this.GetTargetOf(NetworkSystem.Instance.LocalPlayer) == null))
		{
			return new float[]
			{
				8.5f,
				1.3f
			};
		}
		if (GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Slowed)
		{
			return new float[]
			{
				5.5f,
				0.9f
			};
		}
		return new float[]
		{
			6.5f,
			1.1f
		};
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x00112423 File Offset: 0x00110623
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x0400410E RID: 16654
	public float tagCoolDown = 5f;

	// Token: 0x0400410F RID: 16655
	public int[] currentHuntedArray = new int[]
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

	// Token: 0x04004110 RID: 16656
	public List<NetPlayer> currentHunted = new List<NetPlayer>(10);

	// Token: 0x04004111 RID: 16657
	public int[] currentTargetArray = new int[]
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

	// Token: 0x04004112 RID: 16658
	public List<NetPlayer> currentTarget = new List<NetPlayer>(10);

	// Token: 0x04004113 RID: 16659
	public bool huntStarted;

	// Token: 0x04004114 RID: 16660
	public bool waitingToStartNextHuntGame;

	// Token: 0x04004115 RID: 16661
	public bool inStartCountdown;

	// Token: 0x04004116 RID: 16662
	public int countDownTime;

	// Token: 0x04004117 RID: 16663
	public double timeHuntGameEnded;

	// Token: 0x04004118 RID: 16664
	public float timeLastSlowTagged;

	// Token: 0x04004119 RID: 16665
	public object objRef;

	// Token: 0x0400411A RID: 16666
	private int iterator1;

	// Token: 0x0400411B RID: 16667
	private NetPlayer tempRandPlayer;

	// Token: 0x0400411C RID: 16668
	private int tempRandIndex;

	// Token: 0x0400411D RID: 16669
	private int notHuntedCount;

	// Token: 0x0400411E RID: 16670
	private int tempTargetIndex;

	// Token: 0x0400411F RID: 16671
	private NetPlayer tempPlayer;

	// Token: 0x04004120 RID: 16672
	private int copyListToArrayIndex;

	// Token: 0x04004121 RID: 16673
	private int copyArrayToListIndex;
}
