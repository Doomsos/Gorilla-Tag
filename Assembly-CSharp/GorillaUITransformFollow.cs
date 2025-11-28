using System;
using UnityEngine;

// Token: 0x020007EE RID: 2030
public class GorillaUITransformFollow : MonoBehaviour
{
	// Token: 0x0600355F RID: 13663 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x00121E64 File Offset: 0x00120064
	private void LateUpdate()
	{
		if (this.doesMove)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
		}
	}

	// Token: 0x04004498 RID: 17560
	public Transform transformToFollow;

	// Token: 0x04004499 RID: 17561
	public Vector3 offset;

	// Token: 0x0400449A RID: 17562
	public bool doesMove;
}
