using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001127 RID: 4391
	public class WormInApple : MonoBehaviour
	{
		// Token: 0x06006DDD RID: 28125 RVA: 0x00241228 File Offset: 0x0023F428
		public void OnHandTap()
		{
			if (this.blendShapeCosmetic && this.blendShapeCosmetic.GetBlendValue() > 0.5f)
			{
				UnityEvent onHandTapped = this.OnHandTapped;
				if (onHandTapped == null)
				{
					return;
				}
				onHandTapped.Invoke();
			}
		}

		// Token: 0x04007F81 RID: 32641
		[SerializeField]
		private UpdateBlendShapeCosmetic blendShapeCosmetic;

		// Token: 0x04007F82 RID: 32642
		public UnityEvent OnHandTapped;
	}
}
