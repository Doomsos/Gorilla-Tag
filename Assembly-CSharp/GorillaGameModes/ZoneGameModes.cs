using System;

namespace GorillaGameModes
{
	// Token: 0x02000D8D RID: 3469
	[Serializable]
	public struct ZoneGameModes
	{
		// Token: 0x04006216 RID: 25110
		public GTZone[] zone;

		// Token: 0x04006217 RID: 25111
		public GameModeType[] modes;

		// Token: 0x04006218 RID: 25112
		public GameModeType[] privateModes;
	}
}
