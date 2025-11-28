using System;
using System.Diagnostics;

namespace UniLabs.Time
{
	// Token: 0x02000D70 RID: 3440
	[Conditional("UNITY_EDITOR")]
	public class TimeSpanDrawerSettingsAttribute : Attribute
	{
		// Token: 0x06005443 RID: 21571 RVA: 0x001AA0E7 File Offset: 0x001A82E7
		public TimeSpanDrawerSettingsAttribute()
		{
		}

		// Token: 0x06005444 RID: 21572 RVA: 0x001AA0FD File Offset: 0x001A82FD
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, TimeUnit lowestUnit)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = lowestUnit;
		}

		// Token: 0x06005445 RID: 21573 RVA: 0x001AA121 File Offset: 0x001A8321
		public TimeSpanDrawerSettingsAttribute(TimeUnit highestUnit, bool drawMilliseconds = false)
		{
			this.HighestUnit = highestUnit;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x06005446 RID: 21574 RVA: 0x001AA14B File Offset: 0x001A834B
		public TimeSpanDrawerSettingsAttribute(bool drawMilliseconds)
		{
			this.HighestUnit = TimeUnit.Days;
			this.LowestUnit = (drawMilliseconds ? TimeUnit.Milliseconds : TimeUnit.Seconds);
		}

		// Token: 0x040061BD RID: 25021
		public TimeUnit HighestUnit = TimeUnit.Days;

		// Token: 0x040061BE RID: 25022
		public TimeUnit LowestUnit = TimeUnit.Seconds;
	}
}
