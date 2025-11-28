using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B2 RID: 4274
	public class PickupableVariant : MonoBehaviour
	{
		// Token: 0x06006AFC RID: 27388 RVA: 0x00002789 File Offset: 0x00000989
		protected internal virtual void Release(HoldableObject holdable, Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
		}

		// Token: 0x06006AFD RID: 27389 RVA: 0x00002789 File Offset: 0x00000989
		protected internal virtual void Pickup(bool isAutoPickup = false)
		{
		}

		// Token: 0x06006AFE RID: 27390 RVA: 0x00002789 File Offset: 0x00000989
		protected internal virtual void DelayedPickup()
		{
		}
	}
}
