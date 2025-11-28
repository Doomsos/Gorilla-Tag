using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class GiantSnowflakeAudio : MonoBehaviour
{
	// Token: 0x0600305F RID: 12383 RVA: 0x00108CA0 File Offset: 0x00106EA0
	private void Start()
	{
		foreach (GiantSnowflakeAudio.SnowflakeScaleOverride snowflakeScaleOverride in this.audioOverrides)
		{
			if (base.transform.lossyScale.x < snowflakeScaleOverride.scaleMax)
			{
				base.GetComponent<GorillaSurfaceOverride>().overrideIndex = snowflakeScaleOverride.newOverrideIndex;
			}
		}
	}

	// Token: 0x04003F7B RID: 16251
	public List<GiantSnowflakeAudio.SnowflakeScaleOverride> audioOverrides;

	// Token: 0x02000753 RID: 1875
	[Serializable]
	public struct SnowflakeScaleOverride
	{
		// Token: 0x04003F7C RID: 16252
		public float scaleMax;

		// Token: 0x04003F7D RID: 16253
		[GorillaSoundLookup]
		public int newOverrideIndex;
	}
}
