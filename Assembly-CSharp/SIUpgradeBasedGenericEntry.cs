using System;
using UnityEngine;

// Token: 0x020000FE RID: 254
[Serializable]
internal struct SIUpgradeBasedGenericEntry<T>
{
	// Token: 0x06000671 RID: 1649 RVA: 0x00024E18 File Offset: 0x00023018
	public bool IsActive(SIUpgradeSet withUpgrades)
	{
		bool flag = true;
		if (this.activeRequirements.Length != 0)
		{
			flag = false;
			foreach (SIUpgradeType upgrade in this.activeRequirements)
			{
				if (withUpgrades.Contains(upgrade))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (SIUpgradeType upgrade2 in this.inactiveRequirements)
			{
				if (withUpgrades.Contains(upgrade2))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	// Token: 0x0400081E RID: 2078
	public T value;

	// Token: 0x0400081F RID: 2079
	[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
	public SIUpgradeType[] activeRequirements;

	// Token: 0x04000820 RID: 2080
	[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
	public SIUpgradeType[] inactiveRequirements;
}
