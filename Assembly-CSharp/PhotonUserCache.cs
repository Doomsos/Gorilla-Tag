using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;

// Token: 0x02000B9D RID: 2973
public class PhotonUserCache : IInRoomCallbacks, IMatchmakingCallbacks
{
	// Token: 0x06004967 RID: 18791 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06004968 RID: 18792 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnJoinedRoom()
	{
	}

	// Token: 0x06004969 RID: 18793 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnLeftRoom()
	{
	}

	// Token: 0x0600496A RID: 18794 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerLeftRoom(Player player)
	{
	}

	// Token: 0x0600496B RID: 18795 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x0600496C RID: 18796 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
	{
	}

	// Token: 0x0600496D RID: 18797 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnCreatedRoom()
	{
	}

	// Token: 0x0600496E RID: 18798 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnPreLeavingRoom()
	{
	}

	// Token: 0x0600496F RID: 18799 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
	{
	}

	// Token: 0x06004970 RID: 18800 RVA: 0x00002789 File Offset: 0x00000989
	void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	// Token: 0x06004971 RID: 18801 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable changedProperties)
	{
	}

	// Token: 0x06004972 RID: 18802 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player player, Hashtable changedProperties)
	{
	}

	// Token: 0x06004973 RID: 18803 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
	}
}
