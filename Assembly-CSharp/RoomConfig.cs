using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Realtime;

// Token: 0x020003DD RID: 989
public class RoomConfig
{
	// Token: 0x1700028D RID: 653
	// (get) Token: 0x06001837 RID: 6199 RVA: 0x00081C73 File Offset: 0x0007FE73
	public bool IsJoiningWithFriends
	{
		get
		{
			return this.joinFriendIDs != null && this.joinFriendIDs.Length != 0;
		}
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x00081C8C File Offset: 0x0007FE8C
	public void SetFriendIDs(List<string> friendIDs)
	{
		for (int i = 0; i < friendIDs.Count; i++)
		{
			if (friendIDs[i] == NetworkSystem.Instance.GetMyNickName())
			{
				friendIDs.RemoveAt(i);
				i--;
			}
		}
		this.joinFriendIDs = new string[friendIDs.Count];
		for (int j = 0; j < friendIDs.Count; j++)
		{
			this.joinFriendIDs[j] = friendIDs[j];
		}
	}

	// Token: 0x06001839 RID: 6201 RVA: 0x00081CFE File Offset: 0x0007FEFE
	public void ClearExpectedUsers()
	{
		if (this.joinFriendIDs == null || this.joinFriendIDs.Length == 0)
		{
			return;
		}
		this.joinFriendIDs = new string[0];
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x00081D20 File Offset: 0x0007FF20
	public RoomOptions ToPUNOpts()
	{
		return new RoomOptions
		{
			IsVisible = this.isPublic,
			IsOpen = this.isJoinable,
			MaxPlayers = this.MaxPlayers,
			CustomRoomProperties = this.CustomProps,
			PublishUserId = true,
			CustomRoomPropertiesForLobby = this.AutoCustomLobbyProps()
		};
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x00081D75 File Offset: 0x0007FF75
	public void SetFusionOpts(NetworkRunner runnerInst)
	{
		runnerInst.SessionInfo.IsVisible = this.isPublic;
		runnerInst.SessionInfo.IsOpen = this.isJoinable;
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x00081D99 File Offset: 0x0007FF99
	public static RoomConfig SPConfig()
	{
		return new RoomConfig
		{
			isPublic = false,
			isJoinable = false,
			MaxPlayers = 1
		};
	}

	// Token: 0x0600183D RID: 6205 RVA: 0x00081DB5 File Offset: 0x0007FFB5
	public static RoomConfig AnyPublicConfig()
	{
		return new RoomConfig
		{
			isPublic = true,
			isJoinable = true,
			createIfMissing = true,
			MaxPlayers = 10
		};
	}

	// Token: 0x0600183E RID: 6206 RVA: 0x00081DDC File Offset: 0x0007FFDC
	private string[] AutoCustomLobbyProps()
	{
		string[] array = new string[this.CustomProps.Count];
		int num = 0;
		foreach (DictionaryEntry dictionaryEntry in this.CustomProps)
		{
			array[num] = (string)dictionaryEntry.Key;
			num++;
		}
		return array;
	}

	// Token: 0x0400219E RID: 8606
	public const string Room_GameModePropKey = "gameMode";

	// Token: 0x0400219F RID: 8607
	public const string Room_PlatformPropKey = "platform";

	// Token: 0x040021A0 RID: 8608
	public bool isPublic;

	// Token: 0x040021A1 RID: 8609
	public bool isJoinable;

	// Token: 0x040021A2 RID: 8610
	public byte MaxPlayers;

	// Token: 0x040021A3 RID: 8611
	public Hashtable CustomProps = new Hashtable();

	// Token: 0x040021A4 RID: 8612
	public bool createIfMissing;

	// Token: 0x040021A5 RID: 8613
	public string[] joinFriendIDs;
}
