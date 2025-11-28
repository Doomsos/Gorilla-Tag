using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class SIBlasterSprayProjectile : MonoBehaviour
{
	// Token: 0x060004AA RID: 1194 RVA: 0x0001AF93 File Offset: 0x00019193
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001AFA4 File Offset: 0x000191A4
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001B068 File Offset: 0x00019268
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		float num = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			vector,
			playerHit.ActorNr
		});
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001B130 File Offset: 0x00019330
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 4)
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
		Vector3 vector2;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector2))
		{
			return;
		}
		int actorNumber;
		if (!GameEntityManager.ValidateDataType<int>(data[3], out actorNumber))
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
			Object.Instantiate<GameObject>(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		GTPlayer.Instance.ApplyKnockback(vector2.normalized, this.knockbackSpeed, true);
	}

	// Token: 0x04000579 RID: 1401
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x0400057A RID: 1402
	public float knockbackSpeed;

	// Token: 0x0400057B RID: 1403
	public float verticalOffset = -0.133f;

	// Token: 0x0400057C RID: 1404
	public float upwardsAngle = 30f;
}
