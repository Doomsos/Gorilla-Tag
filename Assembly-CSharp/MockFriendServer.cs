using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000B86 RID: 2950
public class MockFriendServer : MonoBehaviourPun
{
	// Token: 0x170006D3 RID: 1747
	// (get) Token: 0x060048FD RID: 18685 RVA: 0x0017F7EA File Offset: 0x0017D9EA
	public int LocalPlayerId
	{
		get
		{
			return PhotonNetwork.LocalPlayer.UserId.GetHashCode();
		}
	}

	// Token: 0x060048FE RID: 18686 RVA: 0x0017F7FC File Offset: 0x0017D9FC
	private void Awake()
	{
		if (MockFriendServer.Instance == null)
		{
			MockFriendServer.Instance = this;
			PhotonNetwork.AddCallbackTarget(this);
			NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnMultiplayerStarted);
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060048FF RID: 18687 RVA: 0x0017F850 File Offset: 0x0017DA50
	private void OnMultiplayerStarted()
	{
		this.RegisterLocalPlayer(this.LocalPlayerId);
	}

	// Token: 0x06004900 RID: 18688 RVA: 0x0017F860 File Offset: 0x0017DA60
	private void Update()
	{
		if (PhotonNetwork.InRoom && base.photonView.IsMine)
		{
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = 0; j < this.indexesToRemove.Count; j++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[j]);
			}
			this.indexesToRemove.Clear();
			for (int k = 0; k < this.friendRequests.Count; k++)
			{
				if (this.friendRequests[k].requestTime + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(k);
				}
				else if (this.friendRequests[k].completionTime < Time.time)
				{
					for (int l = k + 1; l < this.friendRequests.Count; l++)
					{
						int num;
						int num2;
						if (this.friendRequests[l].completionTime < Time.time && this.friendRequests[k].requestorPublicId == this.friendRequests[l].requesteePublicId && this.friendRequests[k].requesteePublicId == this.friendRequests[l].requestorPublicId && this.TryLookupPrivateId(this.friendRequests[k].requestorPublicId, out num) && this.TryLookupPrivateId(this.friendRequests[k].requesteePublicId, out num2))
						{
							this.AddFriend(this.friendRequests[k].requestorPublicId, this.friendRequests[k].requesteePublicId, num, num2);
							this.indexesToRemove.Add(l);
							this.indexesToRemove.Add(k);
							base.photonView.RPC("AddFriendPairRPC", 1, new object[]
							{
								this.friendRequests[k].requestorPublicId,
								this.friendRequests[k].requesteePublicId,
								num,
								num2
							});
							break;
						}
					}
				}
			}
			for (int m = 0; m < this.indexesToRemove.Count; m++)
			{
				this.friendRequests.RemoveAt(this.indexesToRemove[m]);
			}
		}
	}

	// Token: 0x06004901 RID: 18689 RVA: 0x0017FB0C File Offset: 0x0017DD0C
	public void RegisterLocalPlayer(int localPlayerPublicId)
	{
		int hashCode = PlayFabAuthenticator.instance.GetPlayFabPlayerId().GetHashCode();
		if (base.photonView.IsMine)
		{
			this.RegisterLocalPlayerInternal(localPlayerPublicId, hashCode);
			return;
		}
		base.photonView.RPC("RegisterLocalPlayerRPC", 2, new object[]
		{
			localPlayerPublicId,
			hashCode
		});
	}

	// Token: 0x06004902 RID: 18690 RVA: 0x0017FB6C File Offset: 0x0017DD6C
	public void RequestAddFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestAddFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestAddFriendRPC", 2, new object[]
		{
			this.LocalPlayerId,
			targetPlayerId
		});
	}

	// Token: 0x06004903 RID: 18691 RVA: 0x0017FBC4 File Offset: 0x0017DDC4
	public void RequestRemoveFriend(int targetPlayerId)
	{
		if (base.photonView.IsMine)
		{
			this.RequestRemoveFriendInternal(this.LocalPlayerId, targetPlayerId);
			return;
		}
		base.photonView.RPC("RequestRemoveFriendRPC", 2, new object[]
		{
			this.LocalPlayerId,
			targetPlayerId
		});
	}

	// Token: 0x06004904 RID: 18692 RVA: 0x0017FC1C File Offset: 0x0017DE1C
	public void GetFriendList(List<int> friendListResult)
	{
		int localPlayerId = this.LocalPlayerId;
		friendListResult.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if (this.friendPairList[i].publicIdPlayerA == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerB);
			}
			else if (this.friendPairList[i].publicIdPlayerB == localPlayerId)
			{
				friendListResult.Add(this.friendPairList[i].publicIdPlayerA);
			}
		}
	}

	// Token: 0x06004905 RID: 18693 RVA: 0x0017FCA4 File Offset: 0x0017DEA4
	private void RequestAddFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.friendRequests.Count; i++)
			{
				if (this.friendRequests[i].requestorPublicId == localPlayerPublicId && this.friendRequests[i].requesteePublicId == otherPlayerPublicId)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				float time = Time.time;
				float num = Random.Range(this.friendRequestCompletionDelayRange.x, this.friendRequestCompletionDelayRange.y);
				this.friendRequests.Add(new MockFriendServer.FriendRequest
				{
					requestorPublicId = localPlayerPublicId,
					requesteePublicId = otherPlayerPublicId,
					requestTime = time,
					completionTime = time + num
				});
			}
		}
	}

	// Token: 0x06004906 RID: 18694 RVA: 0x0017FD61 File Offset: 0x0017DF61
	[PunRPC]
	public void RequestAddFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestAddFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06004907 RID: 18695 RVA: 0x0017FD6C File Offset: 0x0017DF6C
	private void RequestRemoveFriendInternal(int localPlayerPublicId, int otherPlayerPublicId)
	{
		int privateIdA;
		int privateIdB;
		if (base.photonView.IsMine && this.TryLookupPrivateId(localPlayerPublicId, out privateIdA) && this.TryLookupPrivateId(otherPlayerPublicId, out privateIdB))
		{
			this.RemoveFriend(privateIdA, privateIdB);
		}
	}

	// Token: 0x06004908 RID: 18696 RVA: 0x0017FDA4 File Offset: 0x0017DFA4
	[PunRPC]
	public void RequestRemoveFriendRPC(int localPlayerPublicId, int otherPlayerPublicId, PhotonMessageInfo info)
	{
		this.RequestRemoveFriendInternal(localPlayerPublicId, otherPlayerPublicId);
	}

	// Token: 0x06004909 RID: 18697 RVA: 0x0017FDB0 File Offset: 0x0017DFB0
	private void RegisterLocalPlayerInternal(int publicId, int privateId)
	{
		if (base.photonView.IsMine)
		{
			bool flag = false;
			for (int i = 0; i < this.privateIdLookup.Count; i++)
			{
				if (publicId == this.privateIdLookup[i].playerPublicId || privateId == this.privateIdLookup[i].playerPrivateId)
				{
					MockFriendServer.PrivateIdEncryptionPlaceholder privateIdEncryptionPlaceholder = this.privateIdLookup[i];
					privateIdEncryptionPlaceholder.playerPublicId = publicId;
					privateIdEncryptionPlaceholder.playerPrivateId = privateId;
					this.privateIdLookup[i] = privateIdEncryptionPlaceholder;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.privateIdLookup.Add(new MockFriendServer.PrivateIdEncryptionPlaceholder
				{
					playerPublicId = publicId,
					playerPrivateId = privateId
				});
			}
		}
	}

	// Token: 0x0600490A RID: 18698 RVA: 0x0017FE62 File Offset: 0x0017E062
	[PunRPC]
	public void RegisterLocalPlayerRPC(int playerPublicId, int playerPrivateId, PhotonMessageInfo info)
	{
		this.RegisterLocalPlayerInternal(playerPublicId, playerPrivateId);
	}

	// Token: 0x0600490B RID: 18699 RVA: 0x0017FE6C File Offset: 0x0017E06C
	[PunRPC]
	public void AddFriendPairRPC(int publicIdA, int publicIdB, int privateIdA, int privateIdB, PhotonMessageInfo info)
	{
		this.AddFriend(publicIdA, publicIdB, privateIdA, privateIdB);
	}

	// Token: 0x0600490C RID: 18700 RVA: 0x0017FE7C File Offset: 0x0017E07C
	private void AddFriend(int publicIdA, int publicIdB, int privateIdA, int privateIdB)
	{
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				return;
			}
		}
		this.friendPairList.Add(new MockFriendServer.FriendPair
		{
			publicIdPlayerA = publicIdA,
			publicIdPlayerB = publicIdB,
			privateIdPlayerA = privateIdA,
			privateIdPlayerB = privateIdB
		});
	}

	// Token: 0x0600490D RID: 18701 RVA: 0x0017FF28 File Offset: 0x0017E128
	private void RemoveFriend(int privateIdA, int privateIdB)
	{
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.friendPairList.Count; i++)
		{
			if ((this.friendPairList[i].privateIdPlayerA == privateIdA && this.friendPairList[i].privateIdPlayerB == privateIdB) || (this.friendPairList[i].privateIdPlayerA == privateIdB && this.friendPairList[i].privateIdPlayerB == privateIdA))
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = 0; j < this.friendPairList.Count; j++)
		{
			this.friendPairList.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x0600490E RID: 18702 RVA: 0x0017FFE0 File Offset: 0x0017E1E0
	private bool TryLookupPrivateId(int publicId, out int privateId)
	{
		for (int i = 0; i < this.privateIdLookup.Count; i++)
		{
			if (this.privateIdLookup[i].playerPublicId == publicId)
			{
				privateId = this.privateIdLookup[i].playerPrivateId;
				return true;
			}
		}
		privateId = -1;
		return false;
	}

	// Token: 0x0400597B RID: 22907
	[OnEnterPlay_SetNull]
	public static volatile MockFriendServer Instance;

	// Token: 0x0400597C RID: 22908
	[SerializeField]
	private Vector2 friendRequestCompletionDelayRange = new Vector2(0.5f, 1f);

	// Token: 0x0400597D RID: 22909
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x0400597E RID: 22910
	private List<MockFriendServer.FriendPair> friendPairList = new List<MockFriendServer.FriendPair>();

	// Token: 0x0400597F RID: 22911
	private List<MockFriendServer.PrivateIdEncryptionPlaceholder> privateIdLookup = new List<MockFriendServer.PrivateIdEncryptionPlaceholder>();

	// Token: 0x04005980 RID: 22912
	private List<MockFriendServer.FriendRequest> friendRequests = new List<MockFriendServer.FriendRequest>();

	// Token: 0x04005981 RID: 22913
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x02000B87 RID: 2951
	public struct FriendPair
	{
		// Token: 0x04005982 RID: 22914
		public int publicIdPlayerA;

		// Token: 0x04005983 RID: 22915
		public int publicIdPlayerB;

		// Token: 0x04005984 RID: 22916
		public int privateIdPlayerA;

		// Token: 0x04005985 RID: 22917
		public int privateIdPlayerB;
	}

	// Token: 0x02000B88 RID: 2952
	public struct PrivateIdEncryptionPlaceholder
	{
		// Token: 0x04005986 RID: 22918
		public int playerPublicId;

		// Token: 0x04005987 RID: 22919
		public int playerPrivateId;
	}

	// Token: 0x02000B89 RID: 2953
	public struct FriendRequest
	{
		// Token: 0x04005988 RID: 22920
		public int requestorPublicId;

		// Token: 0x04005989 RID: 22921
		public int requesteePublicId;

		// Token: 0x0400598A RID: 22922
		public float requestTime;

		// Token: 0x0400598B RID: 22923
		public float completionTime;
	}
}
