using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B7F RID: 2943
public class FriendSystem : MonoBehaviour
{
	// Token: 0x170006D2 RID: 1746
	// (get) Token: 0x060048E5 RID: 18661 RVA: 0x0017F20A File Offset: 0x0017D40A
	public FriendSystem.PlayerPrivacy LocalPlayerPrivacy
	{
		get
		{
			return this.localPlayerPrivacy;
		}
	}

	// Token: 0x1400007D RID: 125
	// (add) Token: 0x060048E6 RID: 18662 RVA: 0x0017F214 File Offset: 0x0017D414
	// (remove) Token: 0x060048E7 RID: 18663 RVA: 0x0017F24C File Offset: 0x0017D44C
	public event Action<List<FriendBackendController.Friend>> OnFriendListRefresh;

	// Token: 0x060048E8 RID: 18664 RVA: 0x0017F284 File Offset: 0x0017D484
	public void SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy privacyState)
	{
		this.localPlayerPrivacy = privacyState;
		FriendBackendController.PrivacyState privacyState2;
		switch (privacyState)
		{
		default:
			privacyState2 = FriendBackendController.PrivacyState.VISIBLE;
			break;
		case FriendSystem.PlayerPrivacy.PublicOnly:
			privacyState2 = FriendBackendController.PrivacyState.PUBLIC_ONLY;
			break;
		case FriendSystem.PlayerPrivacy.Hidden:
			privacyState2 = FriendBackendController.PrivacyState.HIDDEN;
			break;
		}
		FriendBackendController.Instance.SetPrivacyState(privacyState2);
	}

	// Token: 0x060048E9 RID: 18665 RVA: 0x0017F2C1 File Offset: 0x0017D4C1
	public void RefreshFriendsList()
	{
		FriendBackendController.Instance.GetFriends();
	}

	// Token: 0x060048EA RID: 18666 RVA: 0x0017F2D0 File Offset: 0x0017D4D0
	public void SendFriendRequest(NetPlayer targetPlayer, GTZone stationZone, FriendSystem.FriendRequestCallback callback)
	{
		FriendSystem.FriendRequestData friendRequestData = new FriendSystem.FriendRequestData
		{
			completionCallback = callback,
			sendingPlayerId = NetworkSystem.Instance.LocalPlayer.UserId.GetHashCode(),
			targetPlayerId = targetPlayer.UserId.GetHashCode(),
			localTimeSent = Time.time,
			zone = stationZone
		};
		this.pendingFriendRequests.Add(friendRequestData);
		FriendBackendController.Instance.AddFriend(targetPlayer);
	}

	// Token: 0x060048EB RID: 18667 RVA: 0x0017F34C File Offset: 0x0017D54C
	public void RemoveFriend(FriendBackendController.Friend friend, FriendSystem.FriendRemovalCallback callback = null)
	{
		this.pendingFriendRemovals.Add(new FriendSystem.FriendRemovalData
		{
			completionCallback = callback,
			targetPlayerId = friend.Presence.FriendLinkId.GetHashCode(),
			localTimeSent = Time.time
		});
		FriendBackendController.Instance.RemoveFriend(friend);
	}

	// Token: 0x060048EC RID: 18668 RVA: 0x0017F3A8 File Offset: 0x0017D5A8
	public bool HasPendingFriendRequest(GTZone zone, int senderId)
	{
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].zone == zone && this.pendingFriendRequests[i].sendingPlayerId == senderId)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060048ED RID: 18669 RVA: 0x0017F3F8 File Offset: 0x0017D5F8
	public bool CheckFriendshipWithPlayer(int targetActorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		if (player != null)
		{
			int hashCode = player.UserId.GetHashCode();
			List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
			for (int i = 0; i < friendsList.Count; i++)
			{
				if (friendsList[i] != null && friendsList[i].Presence != null && friendsList[i].Presence.FriendLinkId.GetHashCode() == hashCode)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060048EE RID: 18670 RVA: 0x0017F471 File Offset: 0x0017D671
	private void Awake()
	{
		if (FriendSystem.Instance == null)
		{
			FriendSystem.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060048EF RID: 18671 RVA: 0x0017F494 File Offset: 0x0017D694
	private void Start()
	{
		FriendBackendController.Instance.OnGetFriendsComplete += new Action<bool>(this.OnGetFriendsReturned);
		FriendBackendController.Instance.OnAddFriendComplete += new Action<NetPlayer, bool>(this.OnAddFriendReturned);
		FriendBackendController.Instance.OnRemoveFriendComplete += new Action<FriendBackendController.Friend, bool>(this.OnRemoveFriendReturned);
	}

	// Token: 0x060048F0 RID: 18672 RVA: 0x0017F4EC File Offset: 0x0017D6EC
	private void OnDestroy()
	{
		if (FriendBackendController.Instance != null)
		{
			FriendBackendController.Instance.OnGetFriendsComplete -= new Action<bool>(this.OnGetFriendsReturned);
			FriendBackendController.Instance.OnAddFriendComplete -= new Action<NetPlayer, bool>(this.OnAddFriendReturned);
			FriendBackendController.Instance.OnRemoveFriendComplete -= new Action<FriendBackendController.Friend, bool>(this.OnRemoveFriendReturned);
		}
	}

	// Token: 0x060048F1 RID: 18673 RVA: 0x0017F550 File Offset: 0x0017D750
	private void OnGetFriendsReturned(bool succeeded)
	{
		if (succeeded)
		{
			this.lastFriendsListRefresh = Time.time;
			switch (FriendBackendController.Instance.MyPrivacyState)
			{
			default:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Visible;
				break;
			case FriendBackendController.PrivacyState.PUBLIC_ONLY:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.PublicOnly;
				break;
			case FriendBackendController.PrivacyState.HIDDEN:
				this.localPlayerPrivacy = FriendSystem.PlayerPrivacy.Hidden;
				break;
			}
			Action<List<FriendBackendController.Friend>> onFriendListRefresh = this.OnFriendListRefresh;
			if (onFriendListRefresh == null)
			{
				return;
			}
			onFriendListRefresh.Invoke(FriendBackendController.Instance.FriendsList);
		}
	}

	// Token: 0x060048F2 RID: 18674 RVA: 0x0017F5C0 File Offset: 0x0017D7C0
	private void OnAddFriendReturned(NetPlayer targetPlayer, bool succeeded)
	{
		int hashCode = targetPlayer.UserId.GetHashCode();
		this.indexesToRemove.Clear();
		for (int i = 0; i < this.pendingFriendRequests.Count; i++)
		{
			if (this.pendingFriendRequests[i].targetPlayerId == hashCode)
			{
				FriendSystem.FriendRequestCallback completionCallback = this.pendingFriendRequests[i].completionCallback;
				if (completionCallback != null)
				{
					completionCallback(this.pendingFriendRequests[i].zone, this.pendingFriendRequests[i].sendingPlayerId, this.pendingFriendRequests[i].targetPlayerId, succeeded);
				}
				this.indexesToRemove.Add(i);
			}
			else if (this.pendingFriendRequests[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
			{
				this.indexesToRemove.Add(i);
			}
		}
		for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
		{
			this.pendingFriendRequests.RemoveAt(this.indexesToRemove[j]);
		}
	}

	// Token: 0x060048F3 RID: 18675 RVA: 0x0017F6CC File Offset: 0x0017D8CC
	private void OnRemoveFriendReturned(FriendBackendController.Friend friend, bool succeeded)
	{
		if (friend != null && friend.Presence != null)
		{
			int hashCode = friend.Presence.FriendLinkId.GetHashCode();
			this.indexesToRemove.Clear();
			for (int i = 0; i < this.pendingFriendRemovals.Count; i++)
			{
				if (this.pendingFriendRemovals[i].targetPlayerId == hashCode)
				{
					FriendSystem.FriendRemovalCallback completionCallback = this.pendingFriendRemovals[i].completionCallback;
					if (completionCallback != null)
					{
						completionCallback(hashCode, succeeded);
					}
					this.indexesToRemove.Add(i);
				}
				else if (this.pendingFriendRemovals[i].localTimeSent + this.friendRequestExpirationTime < Time.time)
				{
					this.indexesToRemove.Add(i);
				}
			}
			for (int j = this.indexesToRemove.Count - 1; j >= 0; j--)
			{
				this.pendingFriendRemovals.RemoveAt(this.indexesToRemove[j]);
			}
		}
	}

	// Token: 0x04005963 RID: 22883
	[OnEnterPlay_SetNull]
	public static volatile FriendSystem Instance;

	// Token: 0x04005964 RID: 22884
	[SerializeField]
	private float friendRequestExpirationTime = 10f;

	// Token: 0x04005965 RID: 22885
	private FriendSystem.PlayerPrivacy localPlayerPrivacy;

	// Token: 0x04005966 RID: 22886
	private List<FriendSystem.FriendRequestData> pendingFriendRequests = new List<FriendSystem.FriendRequestData>();

	// Token: 0x04005967 RID: 22887
	private List<FriendSystem.FriendRemovalData> pendingFriendRemovals = new List<FriendSystem.FriendRemovalData>();

	// Token: 0x04005968 RID: 22888
	private List<int> indexesToRemove = new List<int>();

	// Token: 0x0400596A RID: 22890
	private float lastFriendsListRefresh;

	// Token: 0x02000B80 RID: 2944
	// (Invoke) Token: 0x060048F6 RID: 18678
	public delegate void FriendRequestCallback(GTZone zone, int localId, int friendId, bool success);

	// Token: 0x02000B81 RID: 2945
	private struct FriendRequestData
	{
		// Token: 0x0400596B RID: 22891
		public GTZone zone;

		// Token: 0x0400596C RID: 22892
		public int sendingPlayerId;

		// Token: 0x0400596D RID: 22893
		public int targetPlayerId;

		// Token: 0x0400596E RID: 22894
		public float localTimeSent;

		// Token: 0x0400596F RID: 22895
		public FriendSystem.FriendRequestCallback completionCallback;
	}

	// Token: 0x02000B82 RID: 2946
	// (Invoke) Token: 0x060048FA RID: 18682
	public delegate void FriendRemovalCallback(int friendId, bool success);

	// Token: 0x02000B83 RID: 2947
	private struct FriendRemovalData
	{
		// Token: 0x04005970 RID: 22896
		public int targetPlayerId;

		// Token: 0x04005971 RID: 22897
		public float localTimeSent;

		// Token: 0x04005972 RID: 22898
		public FriendSystem.FriendRemovalCallback completionCallback;
	}

	// Token: 0x02000B84 RID: 2948
	private enum FriendRequestStatus
	{
		// Token: 0x04005974 RID: 22900
		Pending,
		// Token: 0x04005975 RID: 22901
		Succeeded,
		// Token: 0x04005976 RID: 22902
		Failed
	}

	// Token: 0x02000B85 RID: 2949
	public enum PlayerPrivacy
	{
		// Token: 0x04005978 RID: 22904
		Visible,
		// Token: 0x04005979 RID: 22905
		PublicOnly,
		// Token: 0x0400597A RID: 22906
		Hidden
	}
}
