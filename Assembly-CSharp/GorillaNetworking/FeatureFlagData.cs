using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02000F09 RID: 3849
	[Serializable]
	internal class FeatureFlagData
	{
		// Token: 0x04006F21 RID: 28449
		public string name;

		// Token: 0x04006F22 RID: 28450
		public int value;

		// Token: 0x04006F23 RID: 28451
		public string valueType;

		// Token: 0x04006F24 RID: 28452
		public List<string> alwaysOnForUsers;
	}
}
