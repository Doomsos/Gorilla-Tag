using System;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001059 RID: 4185
	[Serializable]
	public struct CosmeticPart
	{
		// Token: 0x0400782F RID: 30767
		public GTAssetRef<GameObject> prefabAssetRef;

		// Token: 0x04007830 RID: 30768
		[Tooltip("Determines how the cosmetic part will be attached to the player.")]
		public CosmeticAttachInfo[] attachAnchors;

		// Token: 0x04007831 RID: 30769
		[NonSerialized]
		public ECosmeticPartType partType;
	}
}
