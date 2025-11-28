using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA0 RID: 4000
	internal interface IGorillaGrabable
	{
		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x0600646A RID: 25706
		string name { get; }

		// Token: 0x0600646B RID: 25707
		bool MomentaryGrabOnly();

		// Token: 0x0600646C RID: 25708
		bool CanBeGrabbed(GorillaGrabber grabber);

		// Token: 0x0600646D RID: 25709
		void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition);

		// Token: 0x0600646E RID: 25710
		void OnGrabReleased(GorillaGrabber grabber);
	}
}
