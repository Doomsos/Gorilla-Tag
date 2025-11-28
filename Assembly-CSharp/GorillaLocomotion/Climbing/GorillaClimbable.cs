using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000FAD RID: 4013
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x060064CD RID: 25805 RVA: 0x0020ECE3 File Offset: 0x0020CEE3
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x04007484 RID: 29828
		public bool snapX;

		// Token: 0x04007485 RID: 29829
		public bool snapY;

		// Token: 0x04007486 RID: 29830
		public bool snapZ;

		// Token: 0x04007487 RID: 29831
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04007488 RID: 29832
		public AudioClip clip;

		// Token: 0x04007489 RID: 29833
		public AudioClip clipOnFullRelease;

		// Token: 0x0400748A RID: 29834
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x0400748B RID: 29835
		public bool climbOnlyWhileSmall;

		// Token: 0x0400748C RID: 29836
		public bool IsPlayerAttached;

		// Token: 0x0400748D RID: 29837
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x0400748E RID: 29838
		[NonSerialized]
		public Collider colliderCache;
	}
}
