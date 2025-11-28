using System;

// Token: 0x020000FD RID: 253
[Serializable]
internal struct SIUpgradeBasedGeneric<T>
{
	// Token: 0x06000670 RID: 1648 RVA: 0x00024DC0 File Offset: 0x00022FC0
	public bool TryGetActiveValue(SIUpgradeSet withUpgrades, out T out_value)
	{
		out_value = default(T);
		bool result = false;
		for (int i = 0; i < this.entries.Length; i++)
		{
			if (this.entries[i].IsActive(withUpgrades))
			{
				result = true;
				out_value = this.entries[i].value;
			}
		}
		return result;
	}

	// Token: 0x0400081D RID: 2077
	public SIUpgradeBasedGenericEntry<T>[] entries;
}
