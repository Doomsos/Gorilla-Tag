using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B9 RID: 4281
	public class TrashcanCosmetic : MonoBehaviour
	{
		// Token: 0x06006B3C RID: 27452 RVA: 0x00233074 File Offset: 0x00231274
		public void OnBasket(bool isLeftHand, Collider other)
		{
			SlingshotProjectile slingshotProjectile;
			if (other.TryGetComponent<SlingshotProjectile>(ref slingshotProjectile) && slingshotProjectile.GetDistanceTraveled() >= this.minScoringDistance)
			{
				UnityEvent onScored = this.OnScored;
				if (onScored != null)
				{
					onScored.Invoke();
				}
				slingshotProjectile.DestroyAfterRelease();
			}
		}

		// Token: 0x04007B91 RID: 31633
		public float minScoringDistance = 2f;

		// Token: 0x04007B92 RID: 31634
		public UnityEvent OnScored;
	}
}
