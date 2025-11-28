using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
[Serializable]
public class SITechTreeNode
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000880 RID: 2176 RVA: 0x0002DA2F File Offset: 0x0002BC2F
	// (set) Token: 0x06000881 RID: 2177 RVA: 0x0002DA37 File Offset: 0x0002BC37
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000882 RID: 2178 RVA: 0x0002DA40 File Offset: 0x0002BC40
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			return edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000883 RID: 2179 RVA: 0x0002DA60 File Offset: 0x0002BC60
	public bool IsDispensableGadget
	{
		get
		{
			return this.IsValid && this.unlockedGadgetPrefab;
		}
	}

	// Token: 0x04000A4B RID: 2635
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000A4C RID: 2636
	public SIUpgradeType upgradeType;

	// Token: 0x04000A4D RID: 2637
	public string nickName;

	// Token: 0x04000A4E RID: 2638
	public string description;

	// Token: 0x04000A4F RID: 2639
	public SIUpgradeType[] parentUpgrades;

	// Token: 0x04000A50 RID: 2640
	public GameEntity unlockedGadgetPrefab;

	// Token: 0x04000A51 RID: 2641
	public SIResource.ResourceCost[] nodeCost;

	// Token: 0x04000A52 RID: 2642
	public bool costOverride;
}
