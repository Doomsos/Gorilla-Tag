using System;

// Token: 0x02000102 RID: 258
public struct SIUpgradeSet
{
	// Token: 0x06000675 RID: 1653 RVA: 0x00024E9A File Offset: 0x0002309A
	public void Clear()
	{
		this.backingBits = 0;
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00024EA3 File Offset: 0x000230A3
	public SIUpgradeSet(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00024EAC File Offset: 0x000230AC
	public int GetBits()
	{
		return this.backingBits;
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00024EA3 File Offset: 0x000230A3
	public void SetBits(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x00024EB4 File Offset: 0x000230B4
	public long GetCreateData(SIPlayer player)
	{
		return (long)this.backingBits << 32 | (long)player.ActorNr;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00024EC8 File Offset: 0x000230C8
	public void Add(SIUpgradeType upgrade)
	{
		this.backingBits |= 1 << upgrade.GetNodeId();
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00024EE2 File Offset: 0x000230E2
	public void Add(int nodeId)
	{
		this.backingBits |= 1 << nodeId;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00024EF7 File Offset: 0x000230F7
	public void Remove(SIUpgradeType upgrade)
	{
		this.backingBits &= ~(1 << upgrade.GetNodeId());
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00024F12 File Offset: 0x00023112
	public bool Contains(SIUpgradeType upgrade)
	{
		return (this.backingBits & 1 << upgrade.GetNodeId()) != 0;
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00024F2C File Offset: 0x0002312C
	public bool ContainsAny(params SIUpgradeType[] upgrades)
	{
		int num = 0;
		foreach (SIUpgradeType self in upgrades)
		{
			num |= 1 << self.GetNodeId();
		}
		return (this.backingBits & num) != 0;
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00024F68 File Offset: 0x00023168
	public string GetString(SITechTreePageId pageId)
	{
		string text = "";
		int i = this.backingBits;
		int num = 0;
		bool flag = true;
		while (i > 0)
		{
			if ((i & 1) != 0)
			{
				if (!flag)
				{
					text += "|";
				}
				text += SIUpgradeTypeSystem.GetUpgradeType((int)pageId, num).ToString();
				flag = false;
			}
			i >>= 1;
			num++;
		}
		return text;
	}

	// Token: 0x0400087F RID: 2175
	private int backingBits;
}
