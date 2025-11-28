using System;
using Critters.Scripts;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class ReleaseFoodWhenUpsideDown : MonoBehaviour
{
	// Token: 0x06000362 RID: 866 RVA: 0x000140C5 File Offset: 0x000122C5
	private void Awake()
	{
		this.latch = false;
	}

	// Token: 0x06000363 RID: 867 RVA: 0x000140D0 File Offset: 0x000122D0
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.dispenser.heldByPlayer)
		{
			return;
		}
		if (Vector3.Angle(base.transform.up, Vector3.down) < this.angle)
		{
			if (this.latch)
			{
				return;
			}
			this.latch = true;
			if (this.nextSpawnTime > (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
			{
				return;
			}
			this.nextSpawnTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) + (double)this.spawnDelay;
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.Food, this.foodSubIndex);
			if (!crittersActor.IsNull())
			{
				CrittersFood crittersFood = (CrittersFood)crittersActor;
				crittersFood.MoveActor(this.spawnPoint.position, this.spawnPoint.rotation, false, true, true);
				crittersFood.SetImpulseVelocity(Vector3.zero, Vector3.zero);
				crittersFood.SpawnData(this.maxFood, this.startingFood, this.startingSize);
				return;
			}
		}
		else
		{
			this.latch = false;
		}
	}

	// Token: 0x040003EF RID: 1007
	public CrittersFoodDispenser dispenser;

	// Token: 0x040003F0 RID: 1008
	public float angle = 30f;

	// Token: 0x040003F1 RID: 1009
	private bool latch;

	// Token: 0x040003F2 RID: 1010
	public Transform spawnPoint;

	// Token: 0x040003F3 RID: 1011
	public float maxFood;

	// Token: 0x040003F4 RID: 1012
	public float startingFood;

	// Token: 0x040003F5 RID: 1013
	public float startingSize;

	// Token: 0x040003F6 RID: 1014
	public int foodSubIndex;

	// Token: 0x040003F7 RID: 1015
	public float spawnDelay = 0.6f;

	// Token: 0x040003F8 RID: 1016
	private double nextSpawnTime;
}
