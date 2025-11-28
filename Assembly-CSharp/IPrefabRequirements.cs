using System;
using System.Collections.Generic;

// Token: 0x020000E0 RID: 224
public interface IPrefabRequirements
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000562 RID: 1378
	IEnumerable<GameEntity> RequiredPrefabs { get; }
}
