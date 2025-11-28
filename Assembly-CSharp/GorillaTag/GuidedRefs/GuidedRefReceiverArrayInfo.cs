using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001046 RID: 4166
	[Serializable]
	public struct GuidedRefReceiverArrayInfo
	{
		// Token: 0x0600691B RID: 26907 RVA: 0x0022334E File Offset: 0x0022154E
		public GuidedRefReceiverArrayInfo(bool useRecommendedDefaults)
		{
			this.resolveModes = (useRecommendedDefaults ? (GRef.EResolveModes.Runtime | GRef.EResolveModes.SceneProcessing) : GRef.EResolveModes.None);
			this.targets = Array.Empty<GuidedRefTargetIdSO>();
			this.hubId = null;
			this.fieldId = 0;
			this.resolveCount = 0;
		}

		// Token: 0x040077AA RID: 30634
		[Tooltip("Controls whether the array should be overridden by the guided refs.")]
		[SerializeField]
		public GRef.EResolveModes resolveModes;

		// Token: 0x040077AB RID: 30635
		[Tooltip("(Required) Used to filter down which relay the target can belong to. Only one GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO hubId;

		// Token: 0x040077AC RID: 30636
		[SerializeField]
		public GuidedRefTargetIdSO[] targets;

		// Token: 0x040077AD RID: 30637
		[NonSerialized]
		public int fieldId;

		// Token: 0x040077AE RID: 30638
		[NonSerialized]
		public int resolveCount;
	}
}
