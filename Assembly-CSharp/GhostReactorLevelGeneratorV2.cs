using System;
using System.Collections.Generic;

// Token: 0x02000643 RID: 1603
public class GhostReactorLevelGeneratorV2
{
	// Token: 0x02000644 RID: 1604
	[Serializable]
	public struct TreeLevelConfig
	{
		// Token: 0x04003477 RID: 13431
		public int minHubs;

		// Token: 0x04003478 RID: 13432
		public int maxHubs;

		// Token: 0x04003479 RID: 13433
		public int minCaps;

		// Token: 0x0400347A RID: 13434
		public int maxCaps;

		// Token: 0x0400347B RID: 13435
		public List<GhostReactorSpawnConfig> sectionSpawnConfigs;

		// Token: 0x0400347C RID: 13436
		public List<GhostReactorSpawnConfig> endCapSpawnConfigs;

		// Token: 0x0400347D RID: 13437
		public List<GhostReactorLevelSection> hubs;

		// Token: 0x0400347E RID: 13438
		public List<GhostReactorLevelSection> endCaps;

		// Token: 0x0400347F RID: 13439
		public List<GhostReactorLevelSection> blockers;

		// Token: 0x04003480 RID: 13440
		public List<GhostReactorLevelSectionConnector> connectors;
	}
}
