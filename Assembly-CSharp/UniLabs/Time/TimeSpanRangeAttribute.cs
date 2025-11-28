using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000D71 RID: 3441
	[AttributeUsage(32767)]
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanRangeAttribute : Attribute
	{
		// Token: 0x06005447 RID: 21575 RVA: 0x001AA195 File Offset: 0x001A8395
		public TimeSpanRangeAttribute(string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x06005448 RID: 21576 RVA: 0x001AA1B2 File Offset: 0x001A83B2
		public TimeSpanRangeAttribute(string minGetter, string maxGetter, bool inline = false, TimeUnit snappingUnit = TimeUnit.Seconds)
		{
			this.MinGetter = minGetter;
			this.MaxGetter = maxGetter;
			this.SnappingUnit = snappingUnit;
			this.Inline = inline;
		}

		// Token: 0x040061BF RID: 25023
		public string MinGetter;

		// Token: 0x040061C0 RID: 25024
		public string MaxGetter;

		// Token: 0x040061C1 RID: 25025
		public TimeUnit SnappingUnit;

		// Token: 0x040061C2 RID: 25026
		public bool Inline;

		// Token: 0x040061C3 RID: 25027
		public string DisableMinMaxIf;
	}
}
