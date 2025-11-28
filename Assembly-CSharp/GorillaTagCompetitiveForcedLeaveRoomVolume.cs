using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020007B5 RID: 1973
public class GorillaTagCompetitiveForcedLeaveRoomVolume : MonoBehaviour
{
	// Token: 0x060033CE RID: 13262 RVA: 0x00117230 File Offset: 0x00115430
	private void Start()
	{
		this.VolumeCollider = base.GetComponent<Collider>();
		this.CompetitiveManager = (GameMode.GetGameModeInstance(GameModeType.InfectionCompetitive) as GorillaTagCompetitiveManager);
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.RegisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x0011726A File Offset: 0x0011546A
	private void OnDestroy()
	{
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.UnregisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x00117288 File Offset: 0x00115488
	public bool ContainsPoint(Vector3 position)
	{
		SphereCollider sphereCollider = this.VolumeCollider as SphereCollider;
		if (sphereCollider != null)
		{
			return Vector3.SqrMagnitude(position - (sphereCollider.transform.position + sphereCollider.center)) <= sphereCollider.radius * sphereCollider.radius;
		}
		BoxCollider boxCollider = this.VolumeCollider as BoxCollider;
		return boxCollider != null && boxCollider.bounds.Contains(position);
	}

	// Token: 0x04004248 RID: 16968
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04004249 RID: 16969
	private Collider VolumeCollider;
}
