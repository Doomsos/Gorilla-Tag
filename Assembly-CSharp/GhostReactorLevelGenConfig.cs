using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000642 RID: 1602
[CreateAssetMenu(fileName = "GhostReactorLevelGenConfig", menuName = "ScriptableObjects/GhostReactorLevelGenConfig")]
public class GhostReactorLevelGenConfig : ScriptableObject
{
	// Token: 0x060028CB RID: 10443 RVA: 0x000D95A0 File Offset: 0x000D77A0
	private void OnValidate()
	{
		for (int i = 0; i < this.treeLevels.Count; i++)
		{
			GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig = this.treeLevels[i];
			treeLevelConfig.minHubs = Mathf.Abs(treeLevelConfig.minHubs);
			treeLevelConfig.maxHubs = Mathf.Abs(treeLevelConfig.maxHubs);
			treeLevelConfig.minCaps = Mathf.Abs(treeLevelConfig.minCaps);
			treeLevelConfig.maxCaps = Mathf.Abs(treeLevelConfig.maxCaps);
			if (treeLevelConfig.minHubs > treeLevelConfig.maxHubs)
			{
				treeLevelConfig.maxHubs = treeLevelConfig.minHubs;
			}
			if (treeLevelConfig.minCaps > treeLevelConfig.maxCaps)
			{
				treeLevelConfig.maxCaps = treeLevelConfig.minCaps;
			}
			this.treeLevels[i] = treeLevelConfig;
		}
		GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig2 = this.treeLevels[this.treeLevels.Count - 1];
		if (treeLevelConfig2.minHubs > 0 || treeLevelConfig2.maxHubs > 0)
		{
			Debug.LogError("Ghost Reactor Level Gen Setup Error: The last tree level can only spawn end caps around the furthest level of hubs. Otherwise it would spawn hubs without a further level to spawn end caps around them");
			treeLevelConfig2.minHubs = 0;
			treeLevelConfig2.maxHubs = 0;
			this.treeLevels[this.treeLevels.Count - 1] = treeLevelConfig2;
		}
		using (List<GREnemyCount>.Enumerator enumerator = this.minEnemyKills.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Count < 0)
				{
					Debug.LogError("Ghost Reactor Level Gen Setup Error: cannot have negative required enemy kills");
				}
			}
		}
	}

	// Token: 0x0400346D RID: 13421
	public int shiftDuration;

	// Token: 0x0400346E RID: 13422
	public int coresRequired;

	// Token: 0x0400346F RID: 13423
	public int shiftBonus;

	// Token: 0x04003470 RID: 13424
	public int sentientCoresRequired;

	// Token: 0x04003471 RID: 13425
	public int maxPlayerDeaths = -1;

	// Token: 0x04003472 RID: 13426
	public List<GREnemyCount> minEnemyKills = new List<GREnemyCount>();

	// Token: 0x04003473 RID: 13427
	[ColorUsage(true, true)]
	public Color ambientLight = Color.black;

	// Token: 0x04003474 RID: 13428
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();

	// Token: 0x04003475 RID: 13429
	public List<GRBonusEntry> enemyGlobalBonuses = new List<GRBonusEntry>();

	// Token: 0x04003476 RID: 13430
	public GRDropTableOverrides dropTableOverrides;
}
