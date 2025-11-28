using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class BeePerchPoint : MonoBehaviour
{
	// Token: 0x06000BAF RID: 2991 RVA: 0x000400FB File Offset: 0x0003E2FB
	public Vector3 GetPoint()
	{
		return base.transform.TransformPoint(this.localPosition);
	}

	// Token: 0x04000E77 RID: 3703
	[SerializeField]
	private Vector3 localPosition;
}
