using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class SIChargeDisplay : MonoBehaviour
{
	// Token: 0x06000560 RID: 1376 RVA: 0x0001F860 File Offset: 0x0001DA60
	public void UpdateDisplay(int chargeCount)
	{
		for (int i = 0; i < this.chargeDisplay.Length; i++)
		{
			this.chargeDisplay[i].material = ((i < chargeCount) ? this.chargedMat : this.unchargedMat);
		}
	}

	// Token: 0x040006DD RID: 1757
	[SerializeField]
	private MeshRenderer[] chargeDisplay;

	// Token: 0x040006DE RID: 1758
	[SerializeField]
	private Material chargedMat;

	// Token: 0x040006DF RID: 1759
	[SerializeField]
	private Material unchargedMat;
}
