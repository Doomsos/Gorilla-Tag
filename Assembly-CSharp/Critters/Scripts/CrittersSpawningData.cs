using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x0200114D RID: 4429
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x06006FCF RID: 28623 RVA: 0x0024671C File Offset: 0x0024491C
		public void InitializeSpawnCollection()
		{
			for (int i = 0; i < this.SpawnParametersList.Count; i++)
			{
				for (int j = 0; j < this.SpawnParametersList[i].ChancesToSpawn; j++)
				{
					this.templateCollection.Add(i);
				}
			}
		}

		// Token: 0x06006FD0 RID: 28624 RVA: 0x00246768 File Offset: 0x00244968
		public int GetRandomTemplate()
		{
			int num = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[num];
		}

		// Token: 0x04008033 RID: 32819
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x04008034 RID: 32820
		private List<int> templateCollection = new List<int>();

		// Token: 0x0200114E RID: 4430
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x04008035 RID: 32821
			public CritterTemplate Template;

			// Token: 0x04008036 RID: 32822
			public int ChancesToSpawn;

			// Token: 0x04008037 RID: 32823
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
