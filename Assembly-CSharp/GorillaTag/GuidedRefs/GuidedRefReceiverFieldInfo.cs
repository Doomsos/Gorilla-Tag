using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200103F RID: 4159
	[Serializable]
	public struct GuidedRefReceiverFieldInfo
	{
		// Token: 0x060068F6 RID: 26870 RVA: 0x00223298 File Offset: 0x00221498
		public GuidedRefReceiverFieldInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targetId = null;
			this.hubId = null;
			this.fieldId = 0;
		}

		// Token: 0x040077A2 RID: 30626
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040077A3 RID: 30627
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x040077A4 RID: 30628
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040077A5 RID: 30629
		[NonSerialized]
		public int fieldId;
	}
}
