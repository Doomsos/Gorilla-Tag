using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x0200105A RID: 4186
	[Serializable]
	public struct CosmeticPartMirrorOption
	{
		// Token: 0x04007832 RID: 30770
		public ECosmeticPartMirrorAxis axis;

		// Token: 0x04007833 RID: 30771
		[Tooltip("This will multiply the local scale for the selected axis by -1.")]
		public bool negativeScale;
	}
}
