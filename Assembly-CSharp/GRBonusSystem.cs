using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068D RID: 1677
public class GRBonusSystem
{
	// Token: 0x06002AD5 RID: 10965 RVA: 0x000E67E4 File Offset: 0x000E49E4
	public void Init(GRAttributes attributes)
	{
		this.defaultAttributes = attributes;
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000E67ED File Offset: 0x000E49ED
	public GRAttributes GetDefaultAttributes()
	{
		return this.defaultAttributes;
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x000E67F8 File Offset: 0x000E49F8
	public void AddBonus(GRBonusEntry entry)
	{
		if (entry.bonusType == GRBonusEntry.GRBonusType.None)
		{
			return;
		}
		if (!this.currentAdditiveBonuses.ContainsKey(entry.attributeType))
		{
			this.currentAdditiveBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (!this.currentMultiplicativeBonuses.ContainsKey(entry.attributeType))
		{
			this.currentMultiplicativeBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Additive)
		{
			this.currentAdditiveBonuses[entry.attributeType].Add(entry);
			return;
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Multiplicative)
		{
			this.currentMultiplicativeBonuses[entry.attributeType].Add(entry);
		}
	}

	// Token: 0x06002AD8 RID: 10968 RVA: 0x000E68A4 File Offset: 0x000E4AA4
	public void RemoveBonus(GRBonusEntry entry)
	{
		foreach (List<GRBonusEntry> list in this.currentAdditiveBonuses.Values)
		{
			list.Remove(entry);
		}
		foreach (List<GRBonusEntry> list2 in this.currentMultiplicativeBonuses.Values)
		{
			list2.Remove(entry);
		}
	}

	// Token: 0x06002AD9 RID: 10969 RVA: 0x000E6944 File Offset: 0x000E4B44
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.defaultAttributes != null && this.defaultAttributes.defaultAttributes.ContainsKey(attributeType);
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x000E6968 File Offset: 0x000E4B68
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		if (this.defaultAttributes == null)
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes null.  Please fix configuration.", Array.Empty<object>());
			return 0;
		}
		if (!this.defaultAttributes.defaultAttributes.ContainsKey(attributeType))
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes Does not have entry for {0}.  Please fix configuration.", new object[]
			{
				attributeType
			});
			return 0;
		}
		int num = this.defaultAttributes.defaultAttributes[attributeType];
		if (this.currentAdditiveBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry in this.currentAdditiveBonuses[attributeType])
			{
				if (grbonusEntry.customBonus != null)
				{
					num = grbonusEntry.customBonus.Invoke(num, grbonusEntry);
				}
				else
				{
					num += grbonusEntry.GetBonusValue();
				}
			}
		}
		if (this.currentMultiplicativeBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry2 in this.currentMultiplicativeBonuses[attributeType])
			{
				if (grbonusEntry2.customBonus != null)
				{
					num = grbonusEntry2.customBonus.Invoke(num, grbonusEntry2);
				}
				else
				{
					float num2 = (float)grbonusEntry2.GetBonusValue() / 100f;
					num = (int)((float)num * num2);
				}
			}
		}
		return num;
	}

	// Token: 0x04003751 RID: 14161
	private GRAttributes defaultAttributes;

	// Token: 0x04003752 RID: 14162
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentAdditiveBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();

	// Token: 0x04003753 RID: 14163
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentMultiplicativeBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();
}
