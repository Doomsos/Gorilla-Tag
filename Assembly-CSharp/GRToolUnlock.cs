using System;
using UnityEngine;

// Token: 0x02000732 RID: 1842
public class GRToolUnlock : ScriptableObject
{
	// Token: 0x04003E26 RID: 15910
	public string toolName;

	// Token: 0x04003E27 RID: 15911
	public string toolId;

	// Token: 0x04003E28 RID: 15912
	public int unlockLevel;

	// Token: 0x04003E29 RID: 15913
	public int unlockCost;

	// Token: 0x04003E2A RID: 15914
	public GRToolUpgrade[] toolUpgrades;
}
