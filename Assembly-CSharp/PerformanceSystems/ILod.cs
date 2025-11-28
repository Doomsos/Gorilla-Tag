using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000D83 RID: 3459
	public interface ILod
	{
		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060054C4 RID: 21700
		int CurrentLod { get; }

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060054C5 RID: 21701
		Vector3 Position { get; }

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060054C6 RID: 21702
		float[] LodRanges { get; }

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060054C7 RID: 21703
		UnityEvent[] OnLodRangeEvents { get; }

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060054C8 RID: 21704
		UnityEvent OnCulledEvent { get; }

		// Token: 0x060054C9 RID: 21705
		void UpdateLod(Vector3 refPos);
	}
}
