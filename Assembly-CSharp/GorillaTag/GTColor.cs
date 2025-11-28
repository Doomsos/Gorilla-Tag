using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FDA RID: 4058
	public static class GTColor
	{
		// Token: 0x060066CE RID: 26318 RVA: 0x00217454 File Offset: 0x00215654
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x02000FDB RID: 4059
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x060066CF RID: 26319 RVA: 0x002174B7 File Offset: 0x002156B7
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x0400755E RID: 30046
			public Vector2 h;

			// Token: 0x0400755F RID: 30047
			public Vector2 s;

			// Token: 0x04007560 RID: 30048
			public Vector2 v;
		}
	}
}
