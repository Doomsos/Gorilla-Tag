using System;
using UnityEngine;

// Token: 0x0200051A RID: 1306
public class GorillaBodyPhysics : MonoBehaviour
{
	// Token: 0x06002146 RID: 8518 RVA: 0x000AF210 File Offset: 0x000AD410
	private void FixedUpdate()
	{
		this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
	}

	// Token: 0x04002BCE RID: 11214
	public GameObject bodyCollider;

	// Token: 0x04002BCF RID: 11215
	public Vector3 bodyColliderOffset;

	// Token: 0x04002BD0 RID: 11216
	public Transform headsetTransform;
}
