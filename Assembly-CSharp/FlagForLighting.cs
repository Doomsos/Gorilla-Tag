using System;
using UnityEngine;

// Token: 0x02000C4A RID: 3146
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x04005CB5 RID: 23733
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000C4B RID: 3147
	public enum TimeOfDay
	{
		// Token: 0x04005CB7 RID: 23735
		Sunrise,
		// Token: 0x04005CB8 RID: 23736
		TenAM,
		// Token: 0x04005CB9 RID: 23737
		Noon,
		// Token: 0x04005CBA RID: 23738
		ThreePM,
		// Token: 0x04005CBB RID: 23739
		Sunset,
		// Token: 0x04005CBC RID: 23740
		Night,
		// Token: 0x04005CBD RID: 23741
		RainingDay,
		// Token: 0x04005CBE RID: 23742
		RainingNight,
		// Token: 0x04005CBF RID: 23743
		None
	}
}
