using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameObjectScheduling
{
	// Token: 0x0200115B RID: 4443
	public class GameObjectSchedulerEventDispatcher : MonoBehaviour
	{
		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06007018 RID: 28696 RVA: 0x0024762D File Offset: 0x0024582D
		public UnityEvent OnScheduledActivation
		{
			get
			{
				return this.onScheduledActivation;
			}
		}

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06007019 RID: 28697 RVA: 0x00247635 File Offset: 0x00245835
		public UnityEvent OnScheduledDeactivation
		{
			get
			{
				return this.onScheduledDeactivation;
			}
		}

		// Token: 0x0400806C RID: 32876
		[SerializeField]
		private UnityEvent onScheduledActivation;

		// Token: 0x0400806D RID: 32877
		[SerializeField]
		private UnityEvent onScheduledDeactivation;
	}
}
