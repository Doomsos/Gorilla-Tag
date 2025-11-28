using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065B RID: 1627
[CreateAssetMenu(fileName = "GhostReactorSpawnConfig", menuName = "ScriptableObjects/GhostReactorSpawnConfig")]
public class GhostReactorSpawnConfig : ScriptableObject
{
	// Token: 0x040035A4 RID: 13732
	public List<GhostReactorSpawnConfig.EntitySpawnGroup> entitySpawnGroups;

	// Token: 0x0200065C RID: 1628
	public enum SpawnPointType
	{
		// Token: 0x040035A6 RID: 13734
		Enemy,
		// Token: 0x040035A7 RID: 13735
		Collectible,
		// Token: 0x040035A8 RID: 13736
		Barrier,
		// Token: 0x040035A9 RID: 13737
		HazardLiquid,
		// Token: 0x040035AA RID: 13738
		Phantom,
		// Token: 0x040035AB RID: 13739
		Pest,
		// Token: 0x040035AC RID: 13740
		Crate,
		// Token: 0x040035AD RID: 13741
		Tool,
		// Token: 0x040035AE RID: 13742
		ChaosSeed,
		// Token: 0x040035AF RID: 13743
		HazardTower,
		// Token: 0x040035B0 RID: 13744
		SpawnPointTypeCount
	}

	// Token: 0x0200065D RID: 1629
	[Serializable]
	public struct EntitySpawnGroup
	{
		// Token: 0x040035B1 RID: 13745
		public GhostReactorSpawnConfig.SpawnPointType spawnPointType;

		// Token: 0x040035B2 RID: 13746
		public GameEntity entity;

		// Token: 0x040035B3 RID: 13747
		public GRBreakableItemSpawnConfig randomEntity;

		// Token: 0x040035B4 RID: 13748
		public int spawnCount;
	}
}
