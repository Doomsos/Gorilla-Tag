using System;
using UnityEngine;

// Token: 0x0200069B RID: 1691
public class GRDistilleryDeposit : MonoBehaviour
{
	// Token: 0x06002B3E RID: 11070 RVA: 0x000E8108 File Offset: 0x000E6308
	private void Start()
	{
		this._distillery = base.GetComponentInParent<GRDistillery>();
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x00002789 File Offset: 0x00000989
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x040037B3 RID: 14259
	public float hapticStrength;

	// Token: 0x040037B4 RID: 14260
	public float hapticDuration;

	// Token: 0x040037B5 RID: 14261
	private GRDistillery _distillery;
}
