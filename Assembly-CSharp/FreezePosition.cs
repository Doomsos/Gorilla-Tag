using System;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class FreezePosition : MonoBehaviour
{
	// Token: 0x060003A9 RID: 937 RVA: 0x000168C7 File Offset: 0x00014AC7
	private void FixedUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x000168C7 File Offset: 0x00014AC7
	private void LateUpdate()
	{
		if (this.target)
		{
			this.target.localPosition = this.localPosition;
		}
	}

	// Token: 0x04000421 RID: 1057
	public Transform target;

	// Token: 0x04000422 RID: 1058
	public Vector3 localPosition;
}
