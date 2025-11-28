using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class HitChecker : MonoBehaviour
{
	// Token: 0x06001D46 RID: 7494 RVA: 0x0009A7F4 File Offset: 0x000989F4
	public static void CheckHandHit(ref int collidersHitCount, LayerMask layerMask, float sphereRadius, ref RaycastHit nullHit, ref RaycastHit[] raycastHits, ref List<RaycastHit> raycastHitList, ref Vector3 spherecastSweep, ref GorillaTriggerColliderHandIndicator handIndicator)
	{
		spherecastSweep = handIndicator.transform.position - handIndicator.lastPosition;
		if (spherecastSweep.magnitude < 0.0001f)
		{
			spherecastSweep = Vector3.up * 0.0001f;
		}
		for (int i = 0; i < raycastHits.Length; i++)
		{
			raycastHits[i] = nullHit;
		}
		collidersHitCount = Physics.SphereCastNonAlloc(handIndicator.lastPosition, sphereRadius, spherecastSweep.normalized, raycastHits, spherecastSweep.magnitude, layerMask, 2);
		if (collidersHitCount > 0)
		{
			raycastHitList.Clear();
			for (int j = 0; j < raycastHits.Length; j++)
			{
				if (raycastHits[j].collider != null)
				{
					raycastHitList.Add(raycastHits[j]);
				}
			}
		}
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x0009A8D4 File Offset: 0x00098AD4
	public static bool CheckHandIn(ref bool anyHit, ref Collider[] colliderHit, float sphereRadius, int layerMask, ref GorillaTriggerColliderHandIndicator handIndicator, ref List<Collider> collidersToBeIn)
	{
		anyHit = (Physics.OverlapSphereNonAlloc(handIndicator.transform.position, sphereRadius, colliderHit, layerMask, 2) > 0);
		if (anyHit)
		{
			anyHit = false;
			for (int i = 0; i < colliderHit.Length; i++)
			{
				if (collidersToBeIn.Contains(colliderHit[i]))
				{
					anyHit = true;
					break;
				}
			}
		}
		return anyHit;
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x0009A928 File Offset: 0x00098B28
	public static int RayCastHitCompare(RaycastHit a, RaycastHit b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance == b.distance)
		{
			return 0;
		}
		return 1;
	}
}
