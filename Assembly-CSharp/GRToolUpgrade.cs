using System;
using UnityEngine;

// Token: 0x02000733 RID: 1843
public class GRToolUpgrade : ScriptableObject
{
	// Token: 0x04003E2B RID: 15915
	public string upgradeName;

	// Token: 0x04003E2C RID: 15916
	public string description;

	// Token: 0x04003E2D RID: 15917
	public string upgradeId;

	// Token: 0x04003E2E RID: 15918
	[SerializeField]
	public GRToolUpgrade.ToolUpgradeLevel[] upgradeLevels;

	// Token: 0x02000734 RID: 1844
	[Serializable]
	public struct ToolUpgradeLevel
	{
		// Token: 0x04003E2F RID: 15919
		[SerializeField]
		public int Cost;

		// Token: 0x04003E30 RID: 15920
		[SerializeField]
		public float upgradeAmount;
	}
}
