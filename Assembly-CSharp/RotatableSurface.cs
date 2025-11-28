using System;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class RotatableSurface : MonoBehaviour
{
	// Token: 0x06001FCA RID: 8138 RVA: 0x000A97E0 File Offset: 0x000A79E0
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.localRotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x04002A28 RID: 10792
	public ManipulatableSpinner spinner;

	// Token: 0x04002A29 RID: 10793
	public float rotationScale = 1f;
}
