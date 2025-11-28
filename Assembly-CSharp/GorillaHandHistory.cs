using System;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public class GorillaHandHistory : MonoBehaviour
{
	// Token: 0x06002151 RID: 8529 RVA: 0x000AF468 File Offset: 0x000AD668
	private void Start()
	{
		this.direction = default(Vector3);
		this.lastPosition = default(Vector3);
	}

	// Token: 0x06002152 RID: 8530 RVA: 0x000AF482 File Offset: 0x000AD682
	private void FixedUpdate()
	{
		this.direction = this.lastPosition - base.transform.position;
		this.lastLastPosition = this.lastPosition;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x04002BE0 RID: 11232
	public Vector3 direction;

	// Token: 0x04002BE1 RID: 11233
	private Vector3 lastPosition;

	// Token: 0x04002BE2 RID: 11234
	private Vector3 lastLastPosition;
}
