using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B7B RID: 2939
public class FriendingManager : MonoBehaviourPun, IPunObservable, IGorillaSliceableSimple
{
	// Token: 0x060048B2 RID: 18610 RVA: 0x0017D3C0 File Offset: 0x0017B5C0
	private void Awake()
	{
		if (FriendingManager.Instance == null)
		{
			FriendingManager.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060048B3 RID: 18611 RVA: 0x0017D3E8 File Offset: 0x0017B5E8
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.ValidateState);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.ValidateState);
	}

	// Token: 0x060048B4 RID: 18612 RVA: 0x0017D458 File Offset: 0x0017B658
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
			NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.ValidateState);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.ValidateState);
		}
	}

	// Token: 0x060048B5 RID: 18613 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060048B6 RID: 18614 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060048B7 RID: 18615 RVA: 0x0017D4D5 File Offset: 0x0017B6D5
	public void SliceUpdate()
	{
		this.AuthorityUpdate();
	}

	// Token: 0x060048B8 RID: 18616 RVA: 0x0017D4E0 File Offset: 0x0017B6E0
	private void AuthorityUpdate()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				if (this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4)
				{
					FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
					int num = 4;
					float num2 = (Time.time - friendStationData.progressBarStartTime) / this.progressBarDuration;
					if (num2 < 1f)
					{
						int num3 = Mathf.RoundToInt(num2 * (float)num);
						friendStationData.state = num3 + FriendingManager.FriendStationState.ButtonConfirmationTimer0;
					}
					else
					{
						base.photonView.RPC("NotifyClientsFriendRequestReadyRPC", 0, new object[]
						{
							friendStationData.zone
						});
						friendStationData.state = FriendingManager.FriendStationState.WaitingOnRequestBoth;
					}
					this.activeFriendStationData[i] = friendStationData;
				}
			}
		}
	}

	// Token: 0x060048B9 RID: 18617 RVA: 0x0017D5CD File Offset: 0x0017B7CD
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		this.ValidateState();
	}

	// Token: 0x060048BA RID: 18618 RVA: 0x0017D5D8 File Offset: 0x0017B7D8
	private void ValidateState()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
			if (friendStationData.actorNumberA != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA) == null)
			{
				friendStationData.actorNumberA = -1;
			}
			if (friendStationData.actorNumberB != -1 && NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB) == null)
			{
				friendStationData.actorNumberB = -1;
			}
			if (friendStationData.actorNumberA == -1 || friendStationData.actorNumberB == -1)
			{
				friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
			}
			this.activeFriendStationData[i] = friendStationData;
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x060048BB RID: 18619 RVA: 0x0017D680 File Offset: 0x0017B880
	private void UpdateFriendingStations()
	{
		for (int i = 0; i < this.activeFriendStationData.Count; i++)
		{
			FriendingStation friendingStation;
			if (this.friendingStations.TryGetValue(this.activeFriendStationData[i].zone, ref friendingStation))
			{
				friendingStation.UpdateState(this.activeFriendStationData[i]);
			}
		}
	}

	// Token: 0x060048BC RID: 18620 RVA: 0x0017D6D5 File Offset: 0x0017B8D5
	public void RegisterFriendingStation(FriendingStation friendingStation)
	{
		if (!this.friendingStations.ContainsKey(friendingStation.Zone))
		{
			this.friendingStations.Add(friendingStation.Zone, friendingStation);
		}
	}

	// Token: 0x060048BD RID: 18621 RVA: 0x0017D6FC File Offset: 0x0017B8FC
	public void UnregisterFriendingStation(FriendingStation friendingStation)
	{
		this.friendingStations.Remove(friendingStation.Zone);
	}

	// Token: 0x060048BE RID: 18622 RVA: 0x0017D710 File Offset: 0x0017B910
	private void DebugLogFriendingStations()
	{
		string text = string.Format("Friending Stations: Count: {0} ", this.friendingStations.Count);
		foreach (KeyValuePair<GTZone, FriendingStation> keyValuePair in this.friendingStations)
		{
			text += string.Format("Station Zone {0}", keyValuePair.Key);
		}
		Debug.Log(text);
	}

	// Token: 0x060048BF RID: 18623 RVA: 0x0017D79C File Offset: 0x0017B99C
	public void PlayerEnteredStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = zone;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					num = i;
					if (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.actorNumberA = netPlayer.ActorNumber;
						if (friendStationData.actorNumberA != -1 && friendStationData.actorNumberB != -1)
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData;
					}
					else if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.actorNumberB = netPlayer.ActorNumber;
						if (friendStationData2.actorNumberA != -1 && friendStationData2.actorNumberB != -1)
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusBoth;
						}
						else
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						}
						this.activeFriendStationData[i] = friendStationData2;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth)
					{
						base.photonView.RPC("CheckFriendStatusRequestRPC", 0, new object[]
						{
							this.activeFriendStationData[i].zone,
							this.activeFriendStationData[i].actorNumberA,
							this.activeFriendStationData[i].actorNumberB
						});
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			if (num < 0)
			{
				this.activeFriendStationData.Add(new FriendingManager.FriendStationData
				{
					zone = zone,
					actorNumberA = netPlayer.ActorNumber,
					actorNumberB = -1,
					state = FriendingManager.FriendStationState.WaitingForPlayers
				});
			}
			this.UpdateFriendingStations();
		}
	}

	// Token: 0x060048C0 RID: 18624 RVA: 0x0017D9E0 File Offset: 0x0017BBE0
	public void PlayerExitedStation(GTZone zone, NetPlayer netPlayer)
	{
		if (netPlayer != null && netPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.localPlayerZone = GTZone.none;
		}
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int num = -1;
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if ((this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == -1) || (this.activeFriendStationData[i].actorNumberA == -1 && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.actorNumberA = -1;
						friendStationData.actorNumberB = -1;
						friendStationData.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData;
						num = i;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberA != -1 && this.activeFriendStationData[i].actorNumberA != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberB == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.actorNumberB = -1;
						friendStationData2.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData2;
						break;
					}
					if (this.activeFriendStationData[i].actorNumberB != -1 && this.activeFriendStationData[i].actorNumberB != netPlayer.ActorNumber && this.activeFriendStationData[i].actorNumberA == netPlayer.ActorNumber)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.actorNumberA = -1;
						friendStationData3.state = FriendingManager.FriendStationState.WaitingForPlayers;
						this.activeFriendStationData[i] = friendStationData3;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			this.UpdateFriendingStations();
			if (num >= 0)
			{
				base.photonView.RPC("StationNoLongerActiveRPC", 1, new object[]
				{
					this.activeFriendStationData[num].zone
				});
				this.activeFriendStationData.RemoveAt(num);
			}
		}
	}

	// Token: 0x060048C1 RID: 18625 RVA: 0x0017DC20 File Offset: 0x0017BE20
	private void PlayerPressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.ButtonConfirmationTimer0;
						friendStationData.progressBarStartTime = Time.time;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060048C2 RID: 18626 RVA: 0x0017DDC0 File Offset: 0x0017BFC0
	private void PlayerUnpressedButton(GTZone zone, int playerId)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					bool flag = this.activeFriendStationData[i].state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.activeFriendStationData[i].state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4;
					if (flag && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerA;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (flag && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonPlayerB;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA && this.activeFriendStationData[i].actorNumberB == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB && this.activeFriendStationData[i].actorNumberA == playerId))
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060048C3 RID: 18627 RVA: 0x0017DF61 File Offset: 0x0017C161
	private void CheckFriendStatusRequest(GTZone zone, int actorNumberA, int actorNumberB)
	{
		FriendSystem.Instance.OnFriendListRefresh -= new Action<List<FriendBackendController.Friend>>(this.CheckFriendStatusOnFriendListRefresh);
		FriendSystem.Instance.OnFriendListRefresh += new Action<List<FriendBackendController.Friend>>(this.CheckFriendStatusOnFriendListRefresh);
		FriendSystem.Instance.RefreshFriendsList();
	}

	// Token: 0x060048C4 RID: 18628 RVA: 0x0017DFA0 File Offset: 0x0017C1A0
	private void CheckFriendStatusOnFriendListRefresh(List<FriendBackendController.Friend> friendList)
	{
		FriendSystem.Instance.OnFriendListRefresh -= new Action<List<FriendBackendController.Friend>>(this.CheckFriendStatusOnFriendListRefresh);
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == this.localPlayerZone)
			{
				int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				int num = -1;
				if (this.activeFriendStationData[i].actorNumberA == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberB;
				}
				else if (this.activeFriendStationData[i].actorNumberB == actorNumber)
				{
					num = this.activeFriendStationData[i].actorNumberA;
				}
				if (num != -1 && FriendSystem.Instance.CheckFriendshipWithPlayer(num))
				{
					base.photonView.RPC("CheckFriendStatusResponseRPC", 2, new object[]
					{
						this.localPlayerZone,
						num,
						true
					});
					return;
				}
				base.photonView.RPC("CheckFriendStatusResponseRPC", 2, new object[]
				{
					this.localPlayerZone,
					num,
					false
				});
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060048C5 RID: 18629 RVA: 0x0017E0E0 File Offset: 0x0017C2E0
	private void CheckFriendStatusResponse(GTZone zone, int responderActorNumber, int friendTargetActorNumber, bool friends)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (this.activeFriendStationData[i].actorNumberA == -1 || this.activeFriendStationData[i].actorNumberB == -1)
					{
						break;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA && this.activeFriendStationData[i].actorNumberA == responderActorNumber) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB && this.activeFriendStationData[i].actorNumberB == responderActorNumber))
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData.state = FriendingManager.FriendStationState.AlreadyFriends;
						}
						else
						{
							friendStationData.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberA == responderActorNumber)
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB;
						}
						else
						{
							friendStationData2.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnFriendStatusBoth && this.activeFriendStationData[i].actorNumberB == responderActorNumber)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						if (friends)
						{
							friendStationData3.state = FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA;
						}
						else
						{
							friendStationData3.state = FriendingManager.FriendStationState.WaitingOnButtonBoth;
						}
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060048C6 RID: 18630 RVA: 0x0017E29C File Offset: 0x0017C49C
	private void SendFriendRequestIfApplicable(GTZone zone)
	{
		int i = 0;
		while (i < this.activeFriendStationData.Count)
		{
			if (this.activeFriendStationData[i].zone == zone)
			{
				FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				NetPlayer netPlayer = null;
				if (friendStationData.actorNumberA == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberB);
				}
				else if (friendStationData.actorNumberB == actorNumber)
				{
					netPlayer = NetworkSystem.Instance.GetNetPlayerByID(friendStationData.actorNumberA);
				}
				if (netPlayer == null)
				{
					return;
				}
				FriendingStation friendingStation;
				if (this.friendingStations.TryGetValue(friendStationData.zone, ref friendingStation) && (GTPlayer.Instance.HeadCenterPosition - friendingStation.transform.position).sqrMagnitude < this.requiredProximityToStation * this.requiredProximityToStation)
				{
					FriendSystem.Instance.SendFriendRequest(netPlayer, friendStationData.zone, new FriendSystem.FriendRequestCallback(this.FriendRequestCallback));
				}
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060048C7 RID: 18631 RVA: 0x0017E398 File Offset: 0x0017C598
	private void FriendRequestCompletedAuthority(GTZone zone, int playerId, bool succeeded)
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			int i = 0;
			while (i < this.activeFriendStationData.Count)
			{
				if (this.activeFriendStationData[i].zone == zone)
				{
					if (!succeeded)
					{
						FriendingManager.FriendStationData friendStationData = this.activeFriendStationData[i];
						friendStationData.state = FriendingManager.FriendStationState.RequestFailed;
						this.activeFriendStationData[i] = friendStationData;
						return;
					}
					if ((this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerA && this.activeFriendStationData[i].actorNumberA == playerId) || (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestPlayerB && this.activeFriendStationData[i].actorNumberB == playerId))
					{
						FriendingManager.FriendStationData friendStationData2 = this.activeFriendStationData[i];
						friendStationData2.state = FriendingManager.FriendStationState.Friends;
						this.activeFriendStationData[i] = friendStationData2;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberA == playerId)
					{
						FriendingManager.FriendStationData friendStationData3 = this.activeFriendStationData[i];
						friendStationData3.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerB;
						this.activeFriendStationData[i] = friendStationData3;
						return;
					}
					if (this.activeFriendStationData[i].state == FriendingManager.FriendStationState.WaitingOnRequestBoth && this.activeFriendStationData[i].actorNumberB == playerId)
					{
						FriendingManager.FriendStationData friendStationData4 = this.activeFriendStationData[i];
						friendStationData4.state = FriendingManager.FriendStationState.WaitingOnRequestPlayerA;
						this.activeFriendStationData[i] = friendStationData4;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x060048C8 RID: 18632 RVA: 0x0017E530 File Offset: 0x0017C730
	private void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success)
	{
		if (base.photonView.IsMine)
		{
			this.FriendRequestCompletedAuthority(zone, PhotonNetwork.LocalPlayer.ActorNumber, success);
			return;
		}
		base.photonView.RPC("FriendRequestCompletedRPC", 2, new object[]
		{
			zone,
			success
		});
	}

	// Token: 0x060048C9 RID: 18633 RVA: 0x0017E588 File Offset: 0x0017C788
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.activeFriendStationData.Count);
			for (int i = 0; i < this.activeFriendStationData.Count; i++)
			{
				FriendingManager.<OnPhotonSerializeView>g__SendFriendStationData|31_0(stream, this.activeFriendStationData[i]);
			}
		}
		else if (stream.IsReading && info.Sender.IsMasterClient)
		{
			int num = (int)stream.ReceiveNext();
			if (num >= 0 && num <= 10)
			{
				this.activeFriendStationData.Clear();
				for (int j = 0; j < num; j++)
				{
					this.activeFriendStationData.Add(FriendingManager.<OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(stream));
				}
			}
		}
		this.UpdateFriendingStations();
	}

	// Token: 0x060048CA RID: 18634 RVA: 0x0017E638 File Offset: 0x0017C838
	[PunRPC]
	public void CheckFriendStatusRequestRPC(GTZone zone, int actorNumberA, int actorNumberB, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusRequest(zone, actorNumberA, actorNumberB);
	}

	// Token: 0x060048CB RID: 18635 RVA: 0x0017E6A0 File Offset: 0x0017C8A0
	[PunRPC]
	public void CheckFriendStatusResponseRPC(GTZone zone, int friendTargetActorNumber, bool friends, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "CheckFriendStatusRequestRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.CheckFriendStatusResponse(zone, info.Sender.ActorNumber, friendTargetActorNumber, friends);
	}

	// Token: 0x060048CC RID: 18636 RVA: 0x0017E714 File Offset: 0x0017C914
	[PunRPC]
	public void FriendButtonPressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonPressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerPressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x060048CD RID: 18637 RVA: 0x0017E784 File Offset: 0x0017C984
	[PunRPC]
	public void FriendButtonUnpressedRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendButtonUnpressedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.PlayerUnpressedButton(zone, info.Sender.ActorNumber);
	}

	// Token: 0x060048CE RID: 18638 RVA: 0x0017E7F4 File Offset: 0x0017C9F4
	[PunRPC]
	public void StationNoLongerActiveRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "StationNoLongerActiveRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		FriendingStation friendingStation;
		if (info.Sender.IsMasterClient && this.friendingStations.TryGetValue(zone, ref friendingStation))
		{
			friendingStation.UpdateState(new FriendingManager.FriendStationData
			{
				zone = zone,
				actorNumberA = -1,
				actorNumberB = -1,
				state = FriendingManager.FriendStationState.WaitingForPlayers
			});
		}
	}

	// Token: 0x060048CF RID: 18639 RVA: 0x0017E89C File Offset: 0x0017CA9C
	[PunRPC]
	public void NotifyClientsFriendRequestReadyRPC(GTZone zone, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "NotifyClientsFriendRequestReadyRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.SendFriendRequestIfApplicable(zone);
	}

	// Token: 0x060048D0 RID: 18640 RVA: 0x0017E900 File Offset: 0x0017CB00
	[PunRPC]
	public void FriendRequestCompletedRPC(GTZone zone, bool succeeded, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "FriendRequestCompletedRPC");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender), out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[12].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.FriendRequestCompletedAuthority(zone, info.Sender.ActorNumber, succeeded);
	}

	// Token: 0x060048D2 RID: 18642 RVA: 0x0017E9AC File Offset: 0x0017CBAC
	[CompilerGenerated]
	internal static void <OnPhotonSerializeView>g__SendFriendStationData|31_0(PhotonStream stream, FriendingManager.FriendStationData data)
	{
		stream.SendNext((int)data.zone);
		stream.SendNext(data.actorNumberA);
		stream.SendNext(data.actorNumberB);
		stream.SendNext((int)data.state);
	}

	// Token: 0x060048D3 RID: 18643 RVA: 0x0017EA00 File Offset: 0x0017CC00
	[CompilerGenerated]
	internal static FriendingManager.FriendStationData <OnPhotonSerializeView>g__ReceiveFriendStationData|31_1(PhotonStream stream)
	{
		return new FriendingManager.FriendStationData
		{
			zone = (GTZone)((int)stream.ReceiveNext()),
			actorNumberA = (int)stream.ReceiveNext(),
			actorNumberB = (int)stream.ReceiveNext(),
			state = (FriendingManager.FriendStationState)((int)stream.ReceiveNext())
		};
	}

	// Token: 0x0400593D RID: 22845
	[OnEnterPlay_SetNull]
	public static volatile FriendingManager Instance;

	// Token: 0x0400593E RID: 22846
	[SerializeField]
	private float progressBarDuration = 3f;

	// Token: 0x0400593F RID: 22847
	[SerializeField]
	private float requiredProximityToStation = 3f;

	// Token: 0x04005940 RID: 22848
	private List<FriendingManager.FriendStationData> activeFriendStationData = new List<FriendingManager.FriendStationData>(10);

	// Token: 0x04005941 RID: 22849
	private Dictionary<GTZone, FriendingStation> friendingStations = new Dictionary<GTZone, FriendingStation>();

	// Token: 0x04005942 RID: 22850
	private GTZone localPlayerZone = GTZone.none;

	// Token: 0x02000B7C RID: 2940
	public enum FriendStationState
	{
		// Token: 0x04005944 RID: 22852
		NotInRoom,
		// Token: 0x04005945 RID: 22853
		WaitingForPlayers,
		// Token: 0x04005946 RID: 22854
		WaitingOnFriendStatusBoth,
		// Token: 0x04005947 RID: 22855
		WaitingOnFriendStatusPlayerA,
		// Token: 0x04005948 RID: 22856
		WaitingOnFriendStatusPlayerB,
		// Token: 0x04005949 RID: 22857
		WaitingOnButtonBoth,
		// Token: 0x0400594A RID: 22858
		WaitingOnButtonPlayerA,
		// Token: 0x0400594B RID: 22859
		WaitingOnButtonPlayerB,
		// Token: 0x0400594C RID: 22860
		ButtonConfirmationTimer0,
		// Token: 0x0400594D RID: 22861
		ButtonConfirmationTimer1,
		// Token: 0x0400594E RID: 22862
		ButtonConfirmationTimer2,
		// Token: 0x0400594F RID: 22863
		ButtonConfirmationTimer3,
		// Token: 0x04005950 RID: 22864
		ButtonConfirmationTimer4,
		// Token: 0x04005951 RID: 22865
		WaitingOnRequestBoth,
		// Token: 0x04005952 RID: 22866
		WaitingOnRequestPlayerA,
		// Token: 0x04005953 RID: 22867
		WaitingOnRequestPlayerB,
		// Token: 0x04005954 RID: 22868
		RequestFailed,
		// Token: 0x04005955 RID: 22869
		Friends,
		// Token: 0x04005956 RID: 22870
		AlreadyFriends
	}

	// Token: 0x02000B7D RID: 2941
	public struct FriendStationData
	{
		// Token: 0x04005957 RID: 22871
		public GTZone zone;

		// Token: 0x04005958 RID: 22872
		public int actorNumberA;

		// Token: 0x04005959 RID: 22873
		public int actorNumberB;

		// Token: 0x0400595A RID: 22874
		public FriendingManager.FriendStationState state;

		// Token: 0x0400595B RID: 22875
		public float progressBarStartTime;
	}
}
