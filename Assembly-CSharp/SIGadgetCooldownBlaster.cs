using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class SIGadgetCooldownBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x060004E2 RID: 1250 RVA: 0x0001C217 File Offset: 0x0001A417
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001C224 File Offset: 0x0001A424
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.blaster.firingSource.clip = this.firingClip;
		this.blaster.firingSource.volume = this.firingVolume;
		this.blaster.firingSource.loop = false;
		this.blaster.blasterSource.clip = this.cooldownClip;
		this.blaster.blasterSource.volume = this.cooldownVolume;
		this.blaster.blasterSource.loop = false;
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001C2B8 File Offset: 0x0001A4B8
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Cooldown)
			{
				return;
			}
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.FireProjectileHaptics(this.availableToFireHapticStrength, 0.02f);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
		}
		else
		{
			if (!this.CheckInput())
			{
				this.triggerHeldDown = false;
				return;
			}
			if (!this.triggerHeldDown)
			{
				this.triggerHeldDown = true;
				this.FireProjectile(this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0001C374 File Offset: 0x0001A574
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
		}
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0001C394 File Offset: 0x0001A594
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.cooldownIndicator.sharedMaterial = this.readyToFireMaterial;
			return;
		}
		if (currentState != SIGadgetBlasterState.Cooldown)
		{
			return;
		}
		this.blaster.lastFired = Time.time;
		this.cooldownIndicator.sharedMaterial = this.onCooldownMaterial;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001C3E8 File Offset: 0x0001A5E8
	public void FireProjectile(int fireId, Vector3 position, Quaternion rotation)
	{
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			if (Time.time < this.blaster.lastFired + this.fireCooldown)
			{
				return;
			}
			this.blaster.FireProjectileHaptics(this.firingHapticStrength, this.firingHapticDuration);
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.blasterSource.time = 0f;
		this.blaster.blasterSource.Play();
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001C4D8 File Offset: 0x0001A6D8
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
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
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rotation))
		{
			return;
		}
		if ((vector - this.blaster.firingPosition.position).magnitude > this.blaster.maxLagDistance)
		{
			return;
		}
		this.FireProjectile(fireId, vector, rotation);
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00002789 File Offset: 0x00000989
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x040005BE RID: 1470
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x040005BF RID: 1471
	public float fireCooldown = 0.5f;

	// Token: 0x040005C0 RID: 1472
	public float availableToFireHapticStrength = 0.1f;

	// Token: 0x040005C1 RID: 1473
	public float availableToFireHapticDuration = 0.01f;

	// Token: 0x040005C2 RID: 1474
	public float firingHapticStrength = 0.25f;

	// Token: 0x040005C3 RID: 1475
	public float firingHapticDuration = 0.01f;

	// Token: 0x040005C4 RID: 1476
	public AudioClip firingClip;

	// Token: 0x040005C5 RID: 1477
	public AudioClip cooldownClip;

	// Token: 0x040005C6 RID: 1478
	public float firingVolume;

	// Token: 0x040005C7 RID: 1479
	public float cooldownVolume;

	// Token: 0x040005C8 RID: 1480
	public ParticleSystem fireFX;

	// Token: 0x040005C9 RID: 1481
	public MeshRenderer cooldownIndicator;

	// Token: 0x040005CA RID: 1482
	public Material readyToFireMaterial;

	// Token: 0x040005CB RID: 1483
	public Material onCooldownMaterial;

	// Token: 0x040005CC RID: 1484
	private bool triggerHeldDown;

	// Token: 0x040005CD RID: 1485
	private SIGadgetBlaster blaster;
}
