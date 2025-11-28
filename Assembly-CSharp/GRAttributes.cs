using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000682 RID: 1666
public class GRAttributes : MonoBehaviour
{
	// Token: 0x06002AA0 RID: 10912 RVA: 0x000E5B7C File Offset: 0x000E3D7C
	private void Awake()
	{
		foreach (GRAttributes.GRAttributePair grattributePair in this.startingAttributes)
		{
			this.defaultAttributes[grattributePair.type] = (int)(grattributePair.value * 100f);
		}
		this.bonusSystem.Init(this);
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000E5BF4 File Offset: 0x000E3DF4
	public bool HasBeenInitialized()
	{
		return this.bonusSystem.GetDefaultAttributes() != null;
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000E5C07 File Offset: 0x000E3E07
	public void AddAttribute(GRAttributeType type, float value)
	{
		this.defaultAttributes[type] = (int)(value * 100f);
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000E5C1D File Offset: 0x000E3E1D
	public void AddBonus(GRBonusEntry entry)
	{
		this.bonusSystem.AddBonus(entry);
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x000E5C2B File Offset: 0x000E3E2B
	public void RemoveBonus(GRBonusEntry entry)
	{
		this.bonusSystem.RemoveBonus(entry);
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x000E5C3C File Offset: 0x000E3E3C
	public float CalculateFinalFloatValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		float result = 0f;
		if (num > 0)
		{
			result = (float)num / 100f;
		}
		return result;
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x000E5C6C File Offset: 0x000E3E6C
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		if (num > 0)
		{
			num /= 100;
		}
		return num;
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x000E5C90 File Offset: 0x000E3E90
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.bonusSystem.HasValueForAttribute(attributeType);
	}

	// Token: 0x0400370A RID: 14090
	[SerializeField]
	private List<GRAttributes.GRAttributePair> startingAttributes;

	// Token: 0x0400370B RID: 14091
	[NonSerialized]
	private GRBonusSystem bonusSystem = new GRBonusSystem();

	// Token: 0x0400370C RID: 14092
	public Dictionary<GRAttributeType, int> defaultAttributes = new Dictionary<GRAttributeType, int>();

	// Token: 0x02000683 RID: 1667
	[Serializable]
	public struct GRAttributePair
	{
		// Token: 0x0400370D RID: 14093
		public GRAttributeType type;

		// Token: 0x0400370E RID: 14094
		public float value;
	}
}
