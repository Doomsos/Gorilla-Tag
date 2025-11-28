using System;

// Token: 0x020005C6 RID: 1478
[Flags]
public enum CosmeticCritterAction
{
	// Token: 0x04003100 RID: 12544
	None = 0,
	// Token: 0x04003101 RID: 12545
	RPC = 1,
	// Token: 0x04003102 RID: 12546
	Spawn = 2,
	// Token: 0x04003103 RID: 12547
	Despawn = 4,
	// Token: 0x04003104 RID: 12548
	SpawnLinked = 8,
	// Token: 0x04003105 RID: 12549
	ShadeHeartbeat = 16
}
