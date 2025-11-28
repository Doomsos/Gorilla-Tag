using System;
using System.IO;
using UnityEngine;

// Token: 0x02000609 RID: 1545
public interface IGameEntityZoneComponent
{
	// Token: 0x0600270F RID: 9999
	void OnZoneCreate();

	// Token: 0x06002710 RID: 10000
	void OnZoneInit();

	// Token: 0x06002711 RID: 10001
	void OnZoneClear(ZoneClearReason reason);

	// Token: 0x06002712 RID: 10002
	void OnCreateGameEntity(GameEntity entity);

	// Token: 0x06002713 RID: 10003
	void SerializeZoneData(BinaryWriter writer);

	// Token: 0x06002714 RID: 10004
	void DeserializeZoneData(BinaryReader reader);

	// Token: 0x06002715 RID: 10005
	void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity);

	// Token: 0x06002716 RID: 10006
	void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity);

	// Token: 0x06002717 RID: 10007
	void SerializeZonePlayerData(BinaryWriter writer, int actorNumber);

	// Token: 0x06002718 RID: 10008
	void DeserializeZonePlayerData(BinaryReader reader, int actorNumber);

	// Token: 0x06002719 RID: 10009
	bool IsZoneReady();

	// Token: 0x0600271A RID: 10010
	bool ShouldClearZone();

	// Token: 0x0600271B RID: 10011
	long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData);

	// Token: 0x0600271C RID: 10012
	bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr);
}
