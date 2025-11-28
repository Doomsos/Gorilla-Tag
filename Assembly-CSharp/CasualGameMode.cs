using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public sealed class CasualGameMode : GorillaGameManager
{
	// Token: 0x06002108 RID: 8456 RVA: 0x000AEC74 File Offset: 0x000ACE74
	public override int MyMatIndex(NetPlayer player)
	{
		if (this.GetMyMaterial == null)
		{
			return 0;
		}
		return this.GetMyMaterial(player);
	}

	// Token: 0x06002109 RID: 8457 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x0600210A RID: 8458 RVA: 0x000743B1 File Offset: 0x000725B1
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x0600210B RID: 8459 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600210C RID: 8460 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600210D RID: 8461 RVA: 0x00002076 File Offset: 0x00000276
	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	// Token: 0x0600210E RID: 8462 RVA: 0x000AEC8C File Offset: 0x000ACE8C
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x0600210F RID: 8463 RVA: 0x000AEC95 File Offset: 0x000ACE95
	public override string GameModeName()
	{
		return "CASUAL";
	}

	// Token: 0x06002110 RID: 8464 RVA: 0x000AEC9C File Offset: 0x000ACE9C
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_CASUAL_ROOM_LABEL", out result, "(CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_CASUAL_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x04002BB6 RID: 11190
	public CasualGameMode.MyMatDelegate GetMyMaterial;

	// Token: 0x0200050F RID: 1295
	// (Invoke) Token: 0x06002113 RID: 8467
	public delegate int MyMatDelegate(NetPlayer player);
}
