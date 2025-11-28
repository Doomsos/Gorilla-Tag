using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000BA0 RID: 2976
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	// Token: 0x06004979 RID: 18809 RVA: 0x001815D4 File Offset: 0x0017F7D4
	private void LateUpdate()
	{
		Physics.SyncTransforms();
	}

	// Token: 0x0600497A RID: 18810 RVA: 0x001815E6 File Offset: 0x0017F7E6
	public static void AddSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Add(body);
	}

	// Token: 0x0600497B RID: 18811 RVA: 0x001815F3 File Offset: 0x0017F7F3
	public static void RemoveSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Remove(body);
	}

	// Token: 0x04005A04 RID: 23044
	private static readonly List<AutoSyncTransforms> k_syncList = new List<AutoSyncTransforms>(5);
}
