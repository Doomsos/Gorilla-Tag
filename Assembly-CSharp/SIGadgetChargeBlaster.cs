using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class SIGadgetChargeBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x060004D7 RID: 1239 RVA: 0x0001BBD1 File Offset: 0x00019DD1
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x0001BBDE File Offset: 0x00019DDE
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.currentCharge = 0f;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x0001BBF8 File Offset: 0x00019DF8
	public void OnUpdateAuthority(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			if (this.CheckInput())
			{
				this.FireProjectile(0f, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
				return;
			}
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			if (this.CheckInput())
			{
				this.blaster.FireProjectileHaptics(this.chargeLevels[this.CurrentBlasterChargeLevel()].chargingHapticStrength, Time.fixedDeltaTime);
				return;
			}
			if (this.CurrentBlasterChargeLevel() > 0)
			{
				this.FireProjectile(this.currentCharge, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
			this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			return;
		case SIGadgetBlasterState.Cooldown:
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				if (this.CheckInput())
				{
					this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
					return;
				}
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001BD54 File Offset: 0x00019F54
	public void OnUpdateRemote(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
		case SIGadgetBlasterState.Cooldown:
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			break;
		default:
			return;
		}
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x0001BDA0 File Offset: 0x00019FA0
	public void SetStateShared()
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			this.currentCharge = 0f;
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge = 0f;
			this.blaster.blasterSource.clip = this.chargingClip;
			this.blaster.blasterSource.volume = this.chargeLevels[0].chargingVolume;
			this.blaster.blasterSource.loop = true;
			this.blaster.blasterSource.Play();
			break;
		case SIGadgetBlasterState.Cooldown:
			this.blaster.blasterSource.Stop();
			if (Time.time > this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.lastFired = Time.time;
			}
			break;
		}
		this.UpdateChargingVisuals();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001BE84 File Offset: 0x0001A084
	public void FireProjectile(float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
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
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				firedAtChargeLevel,
				fireId,
				position,
				rotation
			});
		}
		if (Mathf.Abs(this.currentCharge - firedAtChargeLevel) <= this.maxChargeDiff)
		{
			this.currentCharge = firedAtChargeLevel;
		}
		int num = this.CurrentBlasterChargeLevel();
		this.blaster.firingSource.clip = this.chargeLevels[num].firingClip;
		this.blaster.firingSource.volume = this.chargeLevels[num].firingVolume;
		this.chargeLevels[num].fireFX.Play();
		SIGadgetBlasterProjectile projectilePrefab = this.chargeLevels[num].projectilePrefab;
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.FireProjectileHaptics(this.chargeLevels[num].firingHapticStrength, this.chargeLevels[num].firingHapticDuration);
		}
		this.currentCharge = 0f;
		this.blaster.InstantiateProjectile(projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001C024 File Offset: 0x0001A224
	private void UpdateChargingVisuals()
	{
		bool flag = this.blaster.currentState == SIGadgetBlasterState.Charging;
		int num = this.CurrentBlasterChargeLevel();
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			bool flag2 = flag && i == num;
			if (this.chargeLevels[i].chargingFX.activeSelf != flag2)
			{
				this.chargeLevels[i].chargingFX.SetActive(flag2);
			}
		}
		if (this.blaster.blasterSource.clip != this.chargingClip)
		{
			this.blaster.blasterSource.clip = this.chargingClip;
		}
		this.blaster.blasterSource.volume = this.chargeLevels[num].chargingVolume;
		if (!flag && this.blaster.blasterSource.isPlaying)
		{
			this.blaster.blasterSource.Stop();
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001C110 File Offset: 0x0001A310
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		float num;
		if (!GameEntityManager.ValidateDataType<float>(data[0], out num))
		{
			return;
		}
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[1], out fireId))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
		{
			return;
		}
		if ((vector - this.blaster.firingPosition.position).magnitude > this.blaster.maxLagDistance)
		{
			return;
		}
		this.FireProjectile(num, fireId, vector, rotation);
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x00002789 File Offset: 0x00000989
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0001C1AC File Offset: 0x0001A3AC
	public int CurrentBlasterChargeLevel()
	{
		int result = -1;
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			if (this.currentCharge < this.chargeLevels[i].chargeThreshold)
			{
				return result;
			}
			result = i;
		}
		return result;
	}

	// Token: 0x040005AD RID: 1453
	[SerializeField]
	private float fireCooldown = 0.2f;

	// Token: 0x040005AE RID: 1454
	[SerializeField]
	private float chargeRatePerSecond = 20f;

	// Token: 0x040005AF RID: 1455
	public float maxChargeDiff = 5f;

	// Token: 0x040005B0 RID: 1456
	private float currentCharge;

	// Token: 0x040005B1 RID: 1457
	public AudioClip chargingClip;

	// Token: 0x040005B2 RID: 1458
	public SIGadgetChargeBlaster.BlasterChargeLevel[] chargeLevels;

	// Token: 0x040005B3 RID: 1459
	private SIGadgetBlaster blaster;

	// Token: 0x020000C9 RID: 201
	[Serializable]
	public struct BlasterChargeLevel
	{
		// Token: 0x040005B4 RID: 1460
		public float chargeThreshold;

		// Token: 0x040005B5 RID: 1461
		public float chargingVolume;

		// Token: 0x040005B6 RID: 1462
		public float firingVolume;

		// Token: 0x040005B7 RID: 1463
		public float chargingHapticStrength;

		// Token: 0x040005B8 RID: 1464
		public float firingHapticStrength;

		// Token: 0x040005B9 RID: 1465
		public float firingHapticDuration;

		// Token: 0x040005BA RID: 1466
		public AudioClip firingClip;

		// Token: 0x040005BB RID: 1467
		public ParticleSystem fireFX;

		// Token: 0x040005BC RID: 1468
		public GameObject chargingFX;

		// Token: 0x040005BD RID: 1469
		public SIGadgetBlasterProjectile projectilePrefab;
	}
}
