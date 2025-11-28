using System;

namespace GorillaTagScripts
{
	// Token: 0x02000DCB RID: 3531
	public struct BuilderPrivatePlotData
	{
		// Token: 0x0600578E RID: 22414 RVA: 0x001BE4C7 File Offset: 0x001BC6C7
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x040064D5 RID: 25813
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x040064D6 RID: 25814
		public int ownerActorNumber;

		// Token: 0x040064D7 RID: 25815
		public bool isUnderCapacityLeft;

		// Token: 0x040064D8 RID: 25816
		public bool isUnderCapacityRight;
	}
}
