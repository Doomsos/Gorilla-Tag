using System;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001044 RID: 4164
	public interface IGuidedRefReceiverMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06006913 RID: 26899
		bool GuidedRefTryResolveReference(GuidedRefTryResolveInfo target);

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06006914 RID: 26900
		// (set) Token: 0x06006915 RID: 26901
		int GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x06006916 RID: 26902
		void OnAllGuidedRefsResolved();

		// Token: 0x06006917 RID: 26903
		void OnGuidedRefTargetDestroyed(int fieldId);
	}
}
