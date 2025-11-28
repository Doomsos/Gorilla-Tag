using System;
using UnityEngine;

// Token: 0x02000488 RID: 1160
public class TestManipulatableSpinner : MonoBehaviour
{
	// Token: 0x06001DAA RID: 7594 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06001DAB RID: 7595 RVA: 0x0009C15C File Offset: 0x0009A35C
	private void LateUpdate()
	{
		float angle = this.spinner.angle;
		base.transform.rotation = Quaternion.Euler(0f, angle * this.rotationScale, 0f);
	}

	// Token: 0x040027A0 RID: 10144
	public ManipulatableSpinner spinner;

	// Token: 0x040027A1 RID: 10145
	public float rotationScale = 1f;
}
