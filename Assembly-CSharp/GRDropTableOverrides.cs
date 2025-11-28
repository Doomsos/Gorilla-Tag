using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069C RID: 1692
[CreateAssetMenu(fileName = "GhostReactorDropTableOverrides", menuName = "ScriptableObjects/GhostReactorDropTableOverride")]
public class GRDropTableOverrides : ScriptableObject
{
	// Token: 0x06002B41 RID: 11073 RVA: 0x000E80F8 File Offset: 0x000E62F8
	public GRBreakableItemSpawnConfig GetOverride(GRBreakableItemSpawnConfig table)
	{
		for (int i = 0; i < this.overrides.Count; i++)
		{
			if (this.overrides[i].table == table)
			{
				return this.overrides[i].overrideTable;
			}
		}
		return null;
	}

	// Token: 0x040037B6 RID: 14262
	public List<GRDropTableOverrides.DropTableOverride> overrides;

	// Token: 0x0200069D RID: 1693
	[Serializable]
	public class DropTableOverride
	{
		// Token: 0x040037B7 RID: 14263
		public GRBreakableItemSpawnConfig table;

		// Token: 0x040037B8 RID: 14264
		public GRBreakableItemSpawnConfig overrideTable;
	}
}
