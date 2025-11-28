using System;

namespace GorillaTagScripts
{
	// Token: 0x02000DBB RID: 3515
	[Serializable]
	public class BuilderTableConfiguration
	{
		// Token: 0x0600568D RID: 22157 RVA: 0x001B3C1C File Offset: 0x001B1E1C
		public BuilderTableConfiguration()
		{
			this.version = 0;
			this.TableResourceLimits = new int[3];
			this.PlotResourceLimits = new int[3];
			this.updateCountdownDate = string.Empty;
		}

		// Token: 0x040063B9 RID: 25529
		public const int CONFIGURATION_VERSION = 0;

		// Token: 0x040063BA RID: 25530
		public int version;

		// Token: 0x040063BB RID: 25531
		public int[] TableResourceLimits;

		// Token: 0x040063BC RID: 25532
		public int[] PlotResourceLimits;

		// Token: 0x040063BD RID: 25533
		public int DroppedPieceLimit;

		// Token: 0x040063BE RID: 25534
		public string updateCountdownDate;
	}
}
