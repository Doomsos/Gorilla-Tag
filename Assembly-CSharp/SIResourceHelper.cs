using System;
using System.Collections.Generic;

// Token: 0x0200012F RID: 303
public static class SIResourceHelper
{
	// Token: 0x06000817 RID: 2071 RVA: 0x0002BEE0 File Offset: 0x0002A0E0
	public static bool IsInOrder(this IList<SIResource.ResourceCost> cost)
	{
		SIResource.ResourceType resourceType = (SIResource.ResourceType)(-1);
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			if (resourceCost.type <= resourceType)
			{
				return false;
			}
			resourceType = resourceCost.type;
		}
		return true;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0002BF3C File Offset: 0x0002A13C
	public static bool IsValid(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null || cost.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount <= 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0002BFB8 File Offset: 0x0002A1B8
	public static bool IsValid_AllowZero(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount < 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0002C02C File Offset: 0x0002A22C
	public static SIResource.ResourceCategoryCost GetCategoryCosts(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		int num2 = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
			else
			{
				num2 += resourceCost.amount;
			}
		}
		return new SIResource.ResourceCategoryCost(num, num2);
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002C094 File Offset: 0x0002A294
	public static List<SIResource.ResourceCost> GetTotalResourceCost(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost resourceCost in additiveCosts)
		{
			list.Add(resourceCost);
		}
		return list;
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002C0E4 File Offset: 0x0002A2E4
	public static List<SIResource.ResourceCost> GetMax(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost resourceCost in additiveCosts)
		{
			list.Add(resourceCost);
		}
		list.Sort();
		return list;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002C13C File Offset: 0x0002A33C
	public static int GetAmount(this IList<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType)
	{
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == resourceType)
			{
				return resourceCost.amount;
			}
		}
		return 0;
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0002C194 File Offset: 0x0002A394
	public static void SetAmount(this List<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType, int amount)
	{
		for (int i = 0; i < costs.Count; i++)
		{
			SIResource.ResourceCost resourceCost = costs[i];
			if (resourceCost.type == resourceType)
			{
				resourceCost.amount = amount;
				costs[i] = resourceCost;
				return;
			}
		}
		costs.Add(new SIResource.ResourceCost(resourceType, amount));
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002C1E4 File Offset: 0x0002A3E4
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, SIResource.ResourceCost additiveCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == additiveCost.type)
			{
				resourceCost.amount += additiveCost.amount;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(additiveCost);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002C23C File Offset: 0x0002A43C
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCost)
	{
		foreach (SIResource.ResourceCost additiveCost2 in additiveCost)
		{
			baseCost.AddResourceCost(additiveCost2);
		}
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002C284 File Offset: 0x0002A484
	public static int GetTechPointCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002C2DC File Offset: 0x0002A4DC
	public static int GetMiscCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002C334 File Offset: 0x0002A534
	public static void SetResourceCost(this IList<SIResource.ResourceCost> costs, SIResource.ResourceCategoryCost desiredCosts)
	{
		costs.SetTechPointCost(desiredCosts.techPoints);
		costs.SetMiscCost(desiredCosts.misc);
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002C34E File Offset: 0x0002A54E
	public static void AddResourceCost(this IList<SIResource.ResourceCost> baseCost, SIResource.ResourceCategoryCost additiveCost)
	{
		baseCost.SetTechPointCost(baseCost.GetTechPointCost() + additiveCost.techPoints);
		baseCost.SetMiscCost(baseCost.GetMiscCost() + additiveCost.misc);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0002C378 File Offset: 0x0002A578
	public static void SetTechPointCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount = desiredCost;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.TechPoint, desiredCost));
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002C3C4 File Offset: 0x0002A5C4
	public static void SetMiscCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		int miscCost = baseCost.GetMiscCost();
		if (miscCost == desiredCost)
		{
			return;
		}
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount += desiredCost - miscCost;
				if (resourceCost.amount >= 1)
				{
					baseCost[i] = resourceCost;
					return;
				}
				baseCost.RemoveAt(i--);
				miscCost = baseCost.GetMiscCost();
				if (miscCost == desiredCost)
				{
					return;
				}
			}
		}
		if (desiredCost == miscCost)
		{
			return;
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.StrangeWood, desiredCost - miscCost));
	}
}
