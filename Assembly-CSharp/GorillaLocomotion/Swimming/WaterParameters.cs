using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F90 RID: 3984
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04007397 RID: 29591
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04007398 RID: 29592
		public GameObject splashEffect;

		// Token: 0x04007399 RID: 29593
		public float splashEffectScale = 1f;

		// Token: 0x0400739A RID: 29594
		public bool sendSplashEffectRPCs;

		// Token: 0x0400739B RID: 29595
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x0400739C RID: 29596
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x0400739D RID: 29597
		public Gradient splashColorBySpeedGradient;

		// Token: 0x0400739E RID: 29598
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x0400739F RID: 29599
		public GameObject rippleEffect;

		// Token: 0x040073A0 RID: 29600
		public float rippleEffectScale = 1f;

		// Token: 0x040073A1 RID: 29601
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x040073A2 RID: 29602
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x040073A3 RID: 29603
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x040073A4 RID: 29604
		public Color rippleSpriteColor = Color.white;

		// Token: 0x040073A5 RID: 29605
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x040073A6 RID: 29606
		public float postExitDripDuration = 1.5f;

		// Token: 0x040073A7 RID: 29607
		public float perDripTimeDelay = 0.2f;

		// Token: 0x040073A8 RID: 29608
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x040073A9 RID: 29609
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x040073AA RID: 29610
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x040073AB RID: 29611
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x040073AC RID: 29612
		public bool allowBubblesInVolume;
	}
}
