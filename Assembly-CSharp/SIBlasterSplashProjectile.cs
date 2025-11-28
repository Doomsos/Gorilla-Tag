using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class SIBlasterSplashProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x060004A4 RID: 1188 RVA: 0x0001A9CB File Offset: 0x00018BCB
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001A9DC File Offset: 0x00018BDC
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (!this.projectile.firedByPlayer == SIPlayer.LocalPlayer)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		this.rigList.Clear();
		VRRigCache.Instance.GetActiveRigs(this.rigList);
		Vector3 position = this.projectile.transform.position;
		for (int i = this.rigList.Count - 1; i >= 0; i--)
		{
			if ((this.rigList[i].transform.position - position).magnitude < this.splashHitDistance)
			{
				Vector3 position2 = this.rigList[i].head.rigTarget.position;
				Vector3 position3 = this.rigList[i].bodyTransform.position;
				if (Physics.RaycastNonAlloc(position, position2 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, 1) != 0 && Physics.RaycastNonAlloc(position, position3 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, 1) != 0)
				{
					this.rigList.RemoveAt(i);
				}
			}
			else
			{
				this.rigList.RemoveAt(i);
			}
		}
		if (this.rigList.Count <= 0)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		this.TriggerSplashHitPlayers(this.rigList);
		this.projectile.DespawnProjectile();
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001AC04 File Offset: 0x00018E04
	public void TriggerSplashHitPlayers(List<VRRig> hitPlayers)
	{
		int[] array = new int[hitPlayers.Count];
		Vector3[] array2 = new Vector3[hitPlayers.Count];
		for (int i = 0; i < hitPlayers.Count; i++)
		{
			array[i] = ((hitPlayers[i] != null && hitPlayers[i].OwningNetPlayer != null) ? hitPlayers[i].OwningNetPlayer.ActorNumber : -1);
			Vector3 vector = hitPlayers[i].transform.position - this.projectile.transform.position;
			float num = Mathf.Max(0f, 1f - Mathf.Max(0f, vector.magnitude - this.fullSplashRadius) / (this.splashHitDistance - this.fullSplashRadius));
			array2[i] = vector.normalized * this.knockbackSpeed * num;
			if (hitPlayers[i] != null && hitPlayers[i].isLocal && num > 0f)
			{
				this.SplashHitLocalPlayer(array2[i]);
			}
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			array,
			array2
		});
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001AD6C File Offset: 0x00018F6C
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
		if (!vector.IsFinite())
		{
			return;
		}
		int[] array;
		if (!GameEntityManager.ValidateDataType<int[]>(data[2], out array))
		{
			return;
		}
		Vector3[] array2;
		if (!GameEntityManager.ValidateDataType<Vector3[]>(data[3], out array2))
		{
			return;
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (!array2[i].IsFinite())
			{
				return;
			}
		}
		if (array.Length > VRRigCache.Instance.GetAllRigs().Length)
		{
			return;
		}
		if (array.Length != array2.Length)
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			SIPlayer siplayer = SIPlayer.Get(array[j]);
			if (siplayer != null && siplayer == SIPlayer.LocalPlayer)
			{
				flag = true;
				this.SplashHitLocalPlayer(array2[j]);
			}
		}
		if (flag)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, base.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, base.transform.rotation);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001AEC0 File Offset: 0x000190C0
	public void SplashHitLocalPlayer(Vector3 directionAndMagnitude)
	{
		if (directionAndMagnitude.magnitude > this.knockbackSpeed * 1.05f)
		{
			return;
		}
		float num = Vector3.Angle(directionAndMagnitude.normalized, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(directionAndMagnitude.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(vector * directionAndMagnitude.magnitude, directionAndMagnitude.magnitude / this.knockbackSpeed * this.projectile.hapticHitStrength, this.projectile.hapticHitDuration, true);
	}

	// Token: 0x04000572 RID: 1394
	public float knockbackSpeed;

	// Token: 0x04000573 RID: 1395
	public float fullSplashRadius;

	// Token: 0x04000574 RID: 1396
	public float splashHitDistance;

	// Token: 0x04000575 RID: 1397
	public float upwardsAngle = 30f;

	// Token: 0x04000576 RID: 1398
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x04000577 RID: 1399
	private List<VRRig> rigList = new List<VRRig>();

	// Token: 0x04000578 RID: 1400
	private RaycastHit[] hits = new RaycastHit[20];
}
