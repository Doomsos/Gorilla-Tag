using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001065 RID: 4197
	[CreateAssetMenu(fileName = "UntitledSeason_SeasonSO", menuName = "- Gorilla Tag/SeasonSO", order = 0)]
	public class SeasonSO : ScriptableObject
	{
		// Token: 0x040078A4 RID: 30884
		[Delayed]
		public GTDateTimeSerializable releaseDate = new GTDateTimeSerializable(1);

		// Token: 0x040078A5 RID: 30885
		[Delayed]
		public string seasonName;
	}
}
