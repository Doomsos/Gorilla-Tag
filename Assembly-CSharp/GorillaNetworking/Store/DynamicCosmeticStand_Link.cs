using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F41 RID: 3905
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x060061D8 RID: 25048 RVA: 0x001F824B File Offset: 0x001F644B
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x001F8259 File Offset: 0x001F6459
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x001F8267 File Offset: 0x001F6467
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x001F8274 File Offset: 0x001F6474
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x04007095 RID: 28821
		public DynamicCosmeticStand stand;
	}
}
