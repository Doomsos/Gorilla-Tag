using System;
using UnityEngine;

// Token: 0x0200052B RID: 1323
public class GorillaTriggerBoxTeleport : GorillaTriggerBox
{
	// Token: 0x06002172 RID: 8562 RVA: 0x000AF795 File Offset: 0x000AD995
	public override void OnBoxTriggered()
	{
		this.cameraOffest.GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
		this.cameraOffest.transform.position = this.teleportLocation;
	}

	// Token: 0x04002C2A RID: 11306
	public Vector3 teleportLocation;

	// Token: 0x04002C2B RID: 11307
	public GameObject cameraOffest;
}
