using System;

// Token: 0x0200013A RID: 314
public class SIResourceRegion : SpawnRegion<GameEntity, SIResourceRegion>
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x0600085A RID: 2138 RVA: 0x0002D4AC File Offset: 0x0002B6AC
	// (set) Token: 0x0600085B RID: 2139 RVA: 0x0002D4B4 File Offset: 0x0002B6B4
	public float LastSpawnTime { get; set; }

	// Token: 0x04000A3A RID: 2618
	public SIResource resourcePrefab;
}
