using System;

// Token: 0x020000DB RID: 219
public static class EAssetReleaseTier_Extensions
{
	// Token: 0x0600055E RID: 1374 RVA: 0x0001F851 File Offset: 0x0001DA51
	public static bool ShouldIncludeInBuild(this EAssetReleaseTier assetTier, EBuildReleaseTier buildTier)
	{
		return assetTier != EAssetReleaseTier.Disabled && assetTier <= (EAssetReleaseTier)buildTier;
	}
}
