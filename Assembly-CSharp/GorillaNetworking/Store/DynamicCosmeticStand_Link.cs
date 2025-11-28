using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F41 RID: 3905
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x060061D8 RID: 25048 RVA: 0x001F826B File Offset: 0x001F646B
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x001F8279 File Offset: 0x001F6479
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x001F8287 File Offset: 0x001F6487
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x060061DB RID: 25051 RVA: 0x001F8294 File Offset: 0x001F6494
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x04007095 RID: 28821
		public DynamicCosmeticStand stand;
	}
}
