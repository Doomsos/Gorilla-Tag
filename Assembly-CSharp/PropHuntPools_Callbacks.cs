using System;

// Token: 0x02000212 RID: 530
internal class PropHuntPools_Callbacks
{
	// Token: 0x06000E9E RID: 3742 RVA: 0x0004DCFE File Offset: 0x0004BEFE
	internal void ListenForZoneChanged()
	{
		if (PropHuntPools_Callbacks._isListeningForZoneChanged)
		{
			return;
		}
		ZoneManagement.OnZoneChange += this._OnZoneChanged;
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0004DD1C File Offset: 0x0004BF1C
	private void _OnZoneChanged(ZoneData[] zoneDatas)
	{
		if (VRRigCache.Instance == null || VRRigCache.Instance.localRig == null || VRRigCache.Instance.localRig.Rig == null || VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone != GTZone.bayou)
		{
			return;
		}
		PropHuntPools_Callbacks._isListeningForZoneChanged = false;
		ZoneManagement.OnZoneChange -= this._OnZoneChanged;
		PropHuntPools.OnLocalPlayerEnteredBayou();
	}

	// Token: 0x040011B5 RID: 4533
	private const string preLog = "PropHuntPools_Callbacks: ";

	// Token: 0x040011B6 RID: 4534
	private const string preLogEd = "(editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x040011B7 RID: 4535
	private const string preLogBeta = "(beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x040011B8 RID: 4536
	private const string preErr = "ERROR!!!  PropHuntPools_Callbacks: ";

	// Token: 0x040011B9 RID: 4537
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x040011BA RID: 4538
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x040011BB RID: 4539
	internal static readonly PropHuntPools_Callbacks instance = new PropHuntPools_Callbacks();

	// Token: 0x040011BC RID: 4540
	private static bool _isListeningForZoneChanged;
}
