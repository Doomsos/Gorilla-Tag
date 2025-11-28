using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C5 RID: 197
[RequireComponent(typeof(SIGadgetProjectileType))]
[RequireComponent(typeof(Rigidbody))]
public class SIGadgetBlasterProjectile : MonoBehaviourTick
{
	// Token: 0x060004CA RID: 1226 RVA: 0x0001B79B File Offset: 0x0001999B
	public override void Tick()
	{
		if (Time.time > this.timeSpawned + this.maxLifetime)
		{
			this.parentBlaster.DespawnProjectile(this);
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001B7C0 File Offset: 0x000199C0
	public void InitializeProjectile()
	{
		this.rb.angularVelocity = Vector3.zero;
		this.rb.linearVelocity = base.transform.forward * this.startingVelocity;
		this.timeSpawned = Time.realtimeSinceStartup;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponentInChildren<AudioSource>();
		}
		this.audioSource.time = 0f;
		this.projectileType = base.GetComponent<SIGadgetProjectileType>();
		SIGadgetProjectileModifier[] components = base.GetComponents<SIGadgetProjectileModifier>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ModifyProjectile(this);
		}
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001B860 File Offset: 0x00019A60
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SIExclusionZone>() != null && Time.realtimeSinceStartup > this.timeSpawned + 0.02f)
		{
			if (this.exclusionZoneDespawnEffect != null)
			{
				SIGadgetBlasterProjectile.SpawnExplosion(this.exclusionZoneDespawnEffect, base.transform.position, base.transform.rotation);
			}
			this.DespawnProjectile();
			return;
		}
		SIPlayer componentInParent = other.GetComponentInParent<SIPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent == this.firedByPlayer)
		{
			return;
		}
		if (this.firedByPlayer != SIPlayer.LocalPlayer || componentInParent == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectileType.LocalProjectileHit(componentInParent);
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001B914 File Offset: 0x00019B14
	private void OnCollisionEnter(Collision collision)
	{
		this.projectileType.LocalProjectileHit(null);
		HitTargetNetworkState hitTargetNetworkState;
		if (collision.collider.gameObject.TryGetComponent<HitTargetNetworkState>(ref hitTargetNetworkState))
		{
			hitTargetNetworkState.TargetHit((Time.time - this.timeSpawned) * this.startingVelocity * -base.transform.forward + base.transform.position, base.transform.position);
		}
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001B98A File Offset: 0x00019B8A
	public void DespawnProjectile()
	{
		this.parentBlaster.DespawnProjectile(this);
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0001B998 File Offset: 0x00019B98
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, bool adjustForDirection = true)
	{
		this.KnockbackWithHaptics(directionAndMagnitude, this.hapticHitStrength, this.hapticHitDuration, adjustForDirection);
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001B9B0 File Offset: 0x00019BB0
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, float hapticStrength, float hapticDuration, bool adjustForDirection = true)
	{
		SIPlayer.LocalPlayer.PlayerKnockback(directionAndMagnitude, true, true);
		if (adjustForDirection)
		{
			Vector3 vector = GorillaTagger.Instance.leftHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			Vector3 vector2 = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			float num = 0.5f;
			float num2 = 45f;
			float num3 = Vector3.Angle(vector, directionAndMagnitude);
			float num4 = Vector3.Angle(vector2, directionAndMagnitude);
			float hapticStrength2 = (1f - Mathf.Max(num3 - num2, 0f) / (180f - num2)) * num + (1f - num);
			float hapticStrength3 = (1f - Mathf.Max(num4 - num2, 0f) / (180f - num2)) * num + (1f - num);
			SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength2, hapticDuration, true);
			SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength3, hapticDuration, true);
			return;
		}
		SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength, hapticDuration, true);
		SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength, hapticDuration, true);
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x0001BAC8 File Offset: 0x00019CC8
	public static GameObject SpawnExplosion(GameObject explosionPrefab, Vector3 position, Quaternion rotation)
	{
		if (SIGadgetBlasterProjectile.blasterProjectileExplosionPools == null)
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools = new Dictionary<int, List<GameObject>>();
		}
		if (SIGadgetBlasterProjectile.explosionTypeKey == null)
		{
			SIGadgetBlasterProjectile.explosionTypeKey = new Dictionary<GameObject, int>();
		}
		int instanceID = explosionPrefab.GetInstanceID();
		if (!SIGadgetBlasterProjectile.blasterProjectileExplosionPools.ContainsKey(instanceID))
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlasterProjectile.blasterProjectileExplosionPools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(explosionPrefab, position, rotation);
			SIGadgetBlasterProjectile.explosionTypeKey.Add(gameObject, instanceID);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x0001BB84 File Offset: 0x00019D84
	public static void DespawnExplosion(GameObject explosion)
	{
		SIGadgetBlasterProjectile.blasterProjectileExplosionPools[SIGadgetBlasterProjectile.explosionTypeKey[explosion]].Add(explosion);
		explosion.SetActive(false);
	}

	// Token: 0x0400059B RID: 1435
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectileExplosionPools;

	// Token: 0x0400059C RID: 1436
	[OnEnterPlay_SetNull]
	public static Dictionary<GameObject, int> explosionTypeKey;

	// Token: 0x0400059D RID: 1437
	[NonSerialized]
	public int poolId;

	// Token: 0x0400059E RID: 1438
	public SIGadgetProjectileType projectileType;

	// Token: 0x0400059F RID: 1439
	public Rigidbody rb;

	// Token: 0x040005A0 RID: 1440
	public GameObject hitEffect;

	// Token: 0x040005A1 RID: 1441
	public GameObject hitEffectPlayer;

	// Token: 0x040005A2 RID: 1442
	public float maxLifetime = 10f;

	// Token: 0x040005A3 RID: 1443
	[NonSerialized]
	public float timeSpawned;

	// Token: 0x040005A4 RID: 1444
	public float hapticHitStrength = 0.75f;

	// Token: 0x040005A5 RID: 1445
	public float hapticHitDuration = 0.1f;

	// Token: 0x040005A6 RID: 1446
	[NonSerialized]
	public SIGadgetBlaster parentBlaster;

	// Token: 0x040005A7 RID: 1447
	[NonSerialized]
	public int projectileId;

	// Token: 0x040005A8 RID: 1448
	[NonSerialized]
	public SIPlayer firedByPlayer;

	// Token: 0x040005A9 RID: 1449
	public float startingVelocity;

	// Token: 0x040005AA RID: 1450
	public const float EXCLUSION_ZONE_MINIMUM_LIFETIME = 0.02f;

	// Token: 0x040005AB RID: 1451
	public GameObject exclusionZoneDespawnEffect;

	// Token: 0x040005AC RID: 1452
	private AudioSource audioSource;
}
