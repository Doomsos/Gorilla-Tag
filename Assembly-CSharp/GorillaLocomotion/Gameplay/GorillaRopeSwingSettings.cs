using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F9C RID: 3996
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x04007403 RID: 29699
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x04007404 RID: 29700
		public float frictionWhenNotHeld = 0.25f;
	}
}
