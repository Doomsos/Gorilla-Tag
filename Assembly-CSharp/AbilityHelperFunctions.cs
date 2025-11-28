using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000661 RID: 1633
public static class AbilityHelperFunctions
{
	// Token: 0x060029D8 RID: 10712 RVA: 0x000E23B4 File Offset: 0x000E05B4
	public static float EaseOutPower(float t, float power)
	{
		return 1f - Mathf.Pow(1f - t, power);
	}

	// Token: 0x060029D9 RID: 10713 RVA: 0x000E23CC File Offset: 0x000E05CC
	public static int RandomRangeUnique(int minInclusive, int maxExclusive, int lastValue)
	{
		int num = maxExclusive - minInclusive;
		if (num <= 1)
		{
			return minInclusive;
		}
		int num2 = Random.Range(minInclusive, maxExclusive);
		if (num2 != lastValue)
		{
			return num2;
		}
		return (num2 + 1) % num;
	}

	// Token: 0x060029DA RID: 10714 RVA: 0x000E23F6 File Offset: 0x000E05F6
	public static int GetNavMeshWalkableArea()
	{
		if (AbilityHelperFunctions.navMeshWalkableArea == -1)
		{
			AbilityHelperFunctions.navMeshWalkableArea = NavMesh.GetAreaFromName("walkable");
		}
		return AbilityHelperFunctions.navMeshWalkableArea;
	}

	// Token: 0x060029DB RID: 10715 RVA: 0x000E2414 File Offset: 0x000E0614
	public static Vector3? GetLocationToInvestigate(Vector3 listenerLocation, float hearingRadius, Vector3? currentInvestigationLocation)
	{
		GameNoiseEvent gameNoiseEvent;
		NavMeshHit navMeshHit;
		if (GRNoiseEventManager.instance.GetMostRecentNoiseEventInRadius(listenerLocation, hearingRadius, out gameNoiseEvent) && NavMesh.SamplePosition(gameNoiseEvent.position, ref navMeshHit, 1f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
		{
			return new Vector3?(navMeshHit.position);
		}
		if (currentInvestigationLocation != null)
		{
			return currentInvestigationLocation;
		}
		return default(Vector3?);
	}

	// Token: 0x04003609 RID: 13833
	private static int navMeshWalkableArea = -1;
}
