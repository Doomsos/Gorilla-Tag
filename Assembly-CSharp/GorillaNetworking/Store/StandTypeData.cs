using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F47 RID: 3911
	public class StandTypeData
	{
		// Token: 0x0600620C RID: 25100 RVA: 0x001F9894 File Offset: 0x001F7A94
		public StandTypeData(string[] spawnData)
		{
			this.departmentID = spawnData[0];
			this.displayID = spawnData[1];
			this.standID = spawnData[2];
			this.bustType = spawnData[3];
			if (spawnData.Length == 5)
			{
				this.playFabID = spawnData[4];
			}
			Debug.Log(string.Concat(new string[]
			{
				"StoreStuff: StandTypeData: ",
				this.departmentID,
				"\n",
				this.displayID,
				"\n",
				this.standID,
				"\n",
				this.bustType,
				"\n",
				this.playFabID
			}));
		}

		// Token: 0x0600620D RID: 25101 RVA: 0x001F9978 File Offset: 0x001F7B78
		public StandTypeData(string departmentID, string displayID, string standID, HeadModel_CosmeticStand.BustType bustType, string playFabID)
		{
			this.departmentID = departmentID;
			this.displayID = displayID;
			this.standID = standID;
			this.bustType = bustType.ToString();
			this.playFabID = playFabID;
		}

		// Token: 0x040070B2 RID: 28850
		public string departmentID = "";

		// Token: 0x040070B3 RID: 28851
		public string displayID = "";

		// Token: 0x040070B4 RID: 28852
		public string standID = "";

		// Token: 0x040070B5 RID: 28853
		public string bustType = "";

		// Token: 0x040070B6 RID: 28854
		public string playFabID = "";

		// Token: 0x02000F48 RID: 3912
		public enum EStandDataID
		{
			// Token: 0x040070B8 RID: 28856
			departmentID,
			// Token: 0x040070B9 RID: 28857
			displayID,
			// Token: 0x040070BA RID: 28858
			standID,
			// Token: 0x040070BB RID: 28859
			bustType,
			// Token: 0x040070BC RID: 28860
			playFabID,
			// Token: 0x040070BD RID: 28861
			Count
		}
	}
}
