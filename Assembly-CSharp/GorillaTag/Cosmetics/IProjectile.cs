using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001102 RID: 4354
	public interface IProjectile
	{
		// Token: 0x06006CFE RID: 27902
		void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep = -1);
	}
}
