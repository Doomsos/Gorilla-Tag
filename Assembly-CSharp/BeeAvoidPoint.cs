using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class BeeAvoidPoint : MonoBehaviour
{
	// Token: 0x06000BAC RID: 2988 RVA: 0x000400CB File Offset: 0x0003E2CB
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(base.gameObject);
		FlockingManager.RegisterAvoidPoint(base.gameObject);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x000400E3 File Offset: 0x0003E2E3
	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(base.gameObject);
		FlockingManager.UnregisterAvoidPoint(base.gameObject);
	}
}
