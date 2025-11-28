using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000F91 RID: 3985
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x040073AD RID: 29613
		public bool suppressWaterEffects;

		// Token: 0x040073AE RID: 29614
		public bool playBigSplash;

		// Token: 0x040073AF RID: 29615
		public bool playDrippingEffect = true;

		// Token: 0x040073B0 RID: 29616
		public bool scaleByPlayersScale;

		// Token: 0x040073B1 RID: 29617
		public bool overrideBoundingRadius;

		// Token: 0x040073B2 RID: 29618
		public float boundingRadiusOverride = 1f;
	}
}
