using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000F9F RID: 3999
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x04007415 RID: 29717
		public float minSlidePitch = 0.5f;

		// Token: 0x04007416 RID: 29718
		public float maxSlidePitch = 1f;

		// Token: 0x04007417 RID: 29719
		public float minSlideVolume;

		// Token: 0x04007418 RID: 29720
		public float maxSlideVolume = 0.2f;

		// Token: 0x04007419 RID: 29721
		public float maxSpeed = 10f;

		// Token: 0x0400741A RID: 29722
		public float gravityMulti = 1.1f;

		// Token: 0x0400741B RID: 29723
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x0400741C RID: 29724
		public float maxFriction = 1f;

		// Token: 0x0400741D RID: 29725
		public float maxFrictionSpeed = 15f;
	}
}
