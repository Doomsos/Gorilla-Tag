using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006FD RID: 1789
[Serializable]
public class GRSenseLineOfSight
{
	// Token: 0x06002DEA RID: 11754 RVA: 0x000F96C6 File Offset: 0x000F78C6
	public bool HasLineOfSight(Vector3 headPos, Vector3 targetPos)
	{
		return GRSenseLineOfSight.HasLineOfSight(headPos, targetPos, this.sightDist, this.visibilityMask.value, this.rayCastMode);
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x000F96E8 File Offset: 0x000F78E8
	public static bool HasLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask, GRSenseLineOfSight.RaycastMode rayCastMode = GRSenseLineOfSight.RaycastMode.Geometry)
	{
		switch (rayCastMode)
		{
		case GRSenseLineOfSight.RaycastMode.Geometry:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		case GRSenseLineOfSight.RaycastMode.Navmesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryAndNavMesh:
			return GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask) && GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist);
		case GRSenseLineOfSight.RaycastMode.GeometryOrNavMesh:
			return GRSenseLineOfSight.HasNavmeshLineOfSight(headPos, targetPos, sightDist) || GRSenseLineOfSight.HasGeoLineOfSight(headPos, targetPos, sightDist, layerMask);
		default:
			return false;
		}
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000F9750 File Offset: 0x000F7950
	public static bool HasGeoLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist, int layerMask)
	{
		float num = Vector3.Distance(targetPos, headPos);
		return num <= sightDist && Physics.RaycastNonAlloc(new Ray(headPos, targetPos - headPos), GRSenseLineOfSight.visibilityHits, Mathf.Min(num, sightDist), layerMask, 1) < 1;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x000F9790 File Offset: 0x000F7990
	public static bool HasNavmeshLineOfSight(Vector3 headPos, Vector3 targetPos, float sightDist)
	{
		NavMeshHit navMeshHit;
		NavMeshHit navMeshHit2;
		return (targetPos - headPos).sqrMagnitude <= sightDist * sightDist && NavMesh.SamplePosition(headPos, ref navMeshHit, 1f, -1) && !NavMesh.Raycast(navMeshHit.position, targetPos, ref navMeshHit2, -1);
	}

	// Token: 0x04003BF1 RID: 15345
	public float sightDist;

	// Token: 0x04003BF2 RID: 15346
	public LayerMask visibilityMask;

	// Token: 0x04003BF3 RID: 15347
	public GRSenseLineOfSight.RaycastMode rayCastMode;

	// Token: 0x04003BF4 RID: 15348
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x020006FE RID: 1790
	public enum RaycastMode
	{
		// Token: 0x04003BF6 RID: 15350
		Geometry,
		// Token: 0x04003BF7 RID: 15351
		Navmesh,
		// Token: 0x04003BF8 RID: 15352
		GeometryAndNavMesh,
		// Token: 0x04003BF9 RID: 15353
		GeometryOrNavMesh
	}
}
