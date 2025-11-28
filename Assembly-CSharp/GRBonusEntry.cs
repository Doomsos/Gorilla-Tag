using System;
using UnityEngine;

// Token: 0x0200068B RID: 1675
[Serializable]
public class GRBonusEntry
{
	// Token: 0x06002AD0 RID: 10960 RVA: 0x000E673A File Offset: 0x000E493A
	private GRBonusEntry()
	{
		GRBonusEntry.idCounter++;
		this.id = GRBonusEntry.idCounter;
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06002AD1 RID: 10961 RVA: 0x000E6759 File Offset: 0x000E4959
	// (set) Token: 0x06002AD2 RID: 10962 RVA: 0x000E6761 File Offset: 0x000E4961
	public int id { get; private set; }

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000E676A File Offset: 0x000E496A
	public int GetBonusValue()
	{
		return (int)(this.bonusValue * 100f);
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000E677C File Offset: 0x000E497C
	public override string ToString()
	{
		bool flag = this.customBonus != null;
		return string.Format("GRBonusEntry BonusType {0} AttributeType {1} BonusValue {2} Id {3} CustomBonusSet {4}", new object[]
		{
			this.bonusType,
			this.attributeType,
			this.bonusValue,
			this.id,
			flag
		});
	}

	// Token: 0x04003747 RID: 14151
	private static int idCounter;

	// Token: 0x04003748 RID: 14152
	public GRBonusEntry.GRBonusType bonusType;

	// Token: 0x04003749 RID: 14153
	public GRAttributeType attributeType;

	// Token: 0x0400374A RID: 14154
	[SerializeField]
	private float bonusValue;

	// Token: 0x0400374C RID: 14156
	public Func<int, GRBonusEntry, int> customBonus;

	// Token: 0x0200068C RID: 1676
	public enum GRBonusType
	{
		// Token: 0x0400374E RID: 14158
		None,
		// Token: 0x0400374F RID: 14159
		Additive,
		// Token: 0x04003750 RID: 14160
		Multiplicative
	}
}
