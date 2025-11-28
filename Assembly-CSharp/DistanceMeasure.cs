using System;
using UnityEngine;

// Token: 0x020009CA RID: 2506
public class DistanceMeasure : MonoBehaviour
{
	// Token: 0x06004009 RID: 16393 RVA: 0x001581FF File Offset: 0x001563FF
	private void Awake()
	{
		if (this.from == null)
		{
			this.from = base.transform;
		}
		if (this.to == null)
		{
			this.to = base.transform;
		}
	}

	// Token: 0x04005136 RID: 20790
	public Transform from;

	// Token: 0x04005137 RID: 20791
	public Transform to;
}
