using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02001045 RID: 4165
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06006918 RID: 26904
		// (set) Token: 0x06006919 RID: 26905
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x0600691A RID: 26906
		Object GuidedRefTargetObject { get; }
	}
}
