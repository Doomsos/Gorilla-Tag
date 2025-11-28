using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006CE RID: 1742
public class GRHazardTower : MonoBehaviour, IGameEntityComponent, IGameProjectileLauncher
{
	// Token: 0x06002CB7 RID: 11447 RVA: 0x000F2734 File Offset: 0x000F0934
	public void OnEntityInit()
	{
		this.gameEntity.MinTimeBetweenTicks = 0.5f;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnThink));
		this.senseNearby.Setup(this.fireFrom);
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x000F278C File Offset: 0x000F098C
	public void OnThink()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.nextFireTime)
		{
			return;
		}
		GRHazardTower.tempRigs.Clear();
		GRHazardTower.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRHazardTower.tempRigs);
		this.senseNearby.UpdateNearby(GRHazardTower.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (vrrig == null)
		{
			return;
		}
		Vector3 vector = vrrig.transform.position;
		Vector3 vector2 = Vector3.up * 0.1f;
		vector += vector2;
		GhostReactorManager.Get(this.gameEntity).RequestFireProjectile(this.gameEntity.id, this.fireFrom.position, vector, PhotonNetwork.Time + 0.0);
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x000F2878 File Offset: 0x000F0A78
	public void OnFire(Vector3 fireFromPos, Vector3 fireAtPos, double fireAtTime)
	{
		Vector3 vector;
		if (this.gameEntity.IsAuthority() && GREnemyRanged.CalculateLaunchDirection(fireFromPos, fireAtPos, this.projectileSpeed, out vector))
		{
			this.gameEntity.manager.RequestCreateItem(this.projectilePrefab.name.GetStaticHash(), fireFromPos, Quaternion.LookRotation(vector, Vector3.up), (long)this.gameEntity.GetNetId());
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x00002789 File Offset: 0x00000989
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x00002789 File Offset: 0x00000989
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x04003A0F RID: 14863
	public GameEntity gameEntity;

	// Token: 0x04003A10 RID: 14864
	public GRSenseNearby senseNearby;

	// Token: 0x04003A11 RID: 14865
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003A12 RID: 14866
	public float projectileSpeed;

	// Token: 0x04003A13 RID: 14867
	public GameEntity projectilePrefab;

	// Token: 0x04003A14 RID: 14868
	public Transform fireFrom;

	// Token: 0x04003A15 RID: 14869
	public float fireChargeTime;

	// Token: 0x04003A16 RID: 14870
	public float fireCooldownTime;

	// Token: 0x04003A17 RID: 14871
	private double nextFireTime;

	// Token: 0x04003A18 RID: 14872
	private static List<VRRig> tempRigs = new List<VRRig>(16);
}
