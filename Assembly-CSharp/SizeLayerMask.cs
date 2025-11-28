using System;
using UnityEngine;

// Token: 0x0200082F RID: 2095
[Serializable]
public class SizeLayerMask
{
	// Token: 0x170004F8 RID: 1272
	// (get) Token: 0x0600371D RID: 14109 RVA: 0x0012912C File Offset: 0x0012732C
	public int Mask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x0400468D RID: 18061
	[SerializeField]
	private bool affectLayerA = true;

	// Token: 0x0400468E RID: 18062
	[SerializeField]
	private bool affectLayerB = true;

	// Token: 0x0400468F RID: 18063
	[SerializeField]
	private bool affectLayerC = true;

	// Token: 0x04004690 RID: 18064
	[SerializeField]
	private bool affectLayerD = true;
}
