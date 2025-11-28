using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006EC RID: 1772
public class GRReadyRoom : MonoBehaviour
{
	// Token: 0x06002D67 RID: 11623 RVA: 0x000F585C File Offset: 0x000F3A5C
	public void RefreshRigs(List<VRRig> vrRigs)
	{
		for (int i = 0; i < this.nameDisplayPlates.Count; i++)
		{
			if (this.nameDisplayPlates != null)
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.nameDisplayPlates[i].RefreshPlayerName(vrRigs[i]);
				}
				else
				{
					this.nameDisplayPlates[i].Clear();
				}
			}
		}
	}

	// Token: 0x04003B0A RID: 15114
	public List<GRNameDisplayPlate> nameDisplayPlates;
}
