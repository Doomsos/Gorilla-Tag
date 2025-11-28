using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001047 RID: 4167
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x040077AF RID: 30639
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x040077B0 RID: 30640
		public int fieldId;

		// Token: 0x040077B1 RID: 30641
		public int index;
	}
}
