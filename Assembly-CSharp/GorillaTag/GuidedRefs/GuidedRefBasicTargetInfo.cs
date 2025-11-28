using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001038 RID: 4152
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x04007791 RID: 30609
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x04007792 RID: 30610
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO[] hubIds;

		// Token: 0x04007793 RID: 30611
		[DebugOption]
		[SerializeField]
		public bool hackIgnoreDuplicateRegistration;
	}
}
