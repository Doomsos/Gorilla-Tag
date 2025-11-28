using System;
using System.Collections.Generic;

namespace GorillaTag.GuidedRefs.Internal
{
	// Token: 0x02001049 RID: 4169
	public class RelayInfo
	{
		// Token: 0x040077B5 RID: 30645
		[NonSerialized]
		public IGuidedRefTargetMono targetMono;

		// Token: 0x040077B6 RID: 30646
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> registeredFields;

		// Token: 0x040077B7 RID: 30647
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> resolvedFields;
	}
}
