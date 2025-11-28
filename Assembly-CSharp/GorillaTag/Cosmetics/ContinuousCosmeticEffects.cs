using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010C5 RID: 4293
	public class ContinuousCosmeticEffects : MonoBehaviour
	{
		// Token: 0x06006BA9 RID: 27561 RVA: 0x0023538E File Offset: 0x0023358E
		public void ApplyAll(float f)
		{
			this.continuousProperties.ApplyAll(f);
		}

		// Token: 0x04007C23 RID: 31779
		[FormerlySerializedAs("properties")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;
	}
}
