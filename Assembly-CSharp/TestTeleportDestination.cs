using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200034B RID: 843
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class TestTeleportDestination : MonoBehaviour
{
	// Token: 0x06001423 RID: 5155 RVA: 0x00074174 File Offset: 0x00072374
	private void OnDrawGizmosSelected()
	{
		Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.magenta);
	}

	// Token: 0x04001EBD RID: 7869
	public GTZone[] zones;

	// Token: 0x04001EBE RID: 7870
	public GameObject teleportTransform;
}
