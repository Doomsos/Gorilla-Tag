using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class SIBlasterDirectHitProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x06000499 RID: 1177 RVA: 0x0001A5EA File Offset: 0x000187EA
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001A5F8 File Offset: 0x000187F8
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001A6BC File Offset: 0x000188BC
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			playerHit.ActorNr
		});
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001A724 File Offset: 0x00018924
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		int num;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		int actorNumber;
		if (!GameEntityManager.ValidateDataType<int>(data[2], out actorNumber))
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		if (siplayer == null)
		{
			return;
		}
		if (siplayer != SIPlayer.LocalPlayer)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		float num2 = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num2 - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(vector2.normalized * this.knockbackSpeed, true);
	}

	// Token: 0x0400056A RID: 1386
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x0400056B RID: 1387
	public float knockbackSpeed;

	// Token: 0x0400056C RID: 1388
	public float upwardsAngle = 30f;
}
