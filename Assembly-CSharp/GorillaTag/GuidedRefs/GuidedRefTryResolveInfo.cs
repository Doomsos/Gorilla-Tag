using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001048 RID: 4168
	public struct GuidedRefTryResolveInfo
	{
		// Token: 0x040077B2 RID: 30642
		public int fieldId;

		// Token: 0x040077B3 RID: 30643
		public int index;

		// Token: 0x040077B4 RID: 30644
		[FormerlySerializedAs("target")]
		public IGuidedRefTargetMono targetMono;
	}
}
