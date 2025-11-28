using System;

// Token: 0x02000101 RID: 257
public static class SIUpgradeTypeSystem
{
	// Token: 0x06000672 RID: 1650 RVA: 0x00024E86 File Offset: 0x00023086
	public static int GetPageId(this SIUpgradeType self)
	{
		return (int)(self / SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00024E8C File Offset: 0x0002308C
	public static int GetNodeId(this SIUpgradeType self)
	{
		return (int)(self % SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00024E92 File Offset: 0x00023092
	public static SIUpgradeType GetUpgradeType(int pageId, int nodeId)
	{
		return (SIUpgradeType)(pageId * 100 + nodeId);
	}
}
