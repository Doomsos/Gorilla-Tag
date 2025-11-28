using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000640 RID: 1600
[CreateAssetMenu(fileName = "GhostReactorLevelDepthConfig", menuName = "ScriptableObjects/GhostReactorLevelDepthConfig")]
public class GhostReactorLevelDepthConfig : ScriptableObject
{
	// Token: 0x04003468 RID: 13416
	public string displayName;

	// Token: 0x04003469 RID: 13417
	public List<GhostReactorLevelGenConfig> configGenOptions = new List<GhostReactorLevelGenConfig>();

	// Token: 0x0400346A RID: 13418
	public List<GhostReactorLevelDepthConfig.LevelOption> options = new List<GhostReactorLevelDepthConfig.LevelOption>();

	// Token: 0x02000641 RID: 1601
	[Serializable]
	public class LevelOption
	{
		// Token: 0x0400346B RID: 13419
		public int weight = 100;

		// Token: 0x0400346C RID: 13420
		public GhostReactorLevelGenConfig levelConfig;
	}
}
