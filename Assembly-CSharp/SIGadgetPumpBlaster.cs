using System;
using UnityEngine;

// Token: 0x020000CC RID: 204
[RequireComponent(typeof(GameTriggerInteractable))]
public class SIGadgetPumpBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x060004EE RID: 1262 RVA: 0x0001C73F File Offset: 0x0001A93F
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x0001C74C File Offset: 0x0001A94C
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.triggerInteractable = base.GetComponent<GameTriggerInteractable>();
		this.strokeLength = (this.pumpFullyClosed.position - this.pumpFullyOpen.position).magnitude;
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x0001C79C File Offset: 0x0001A99C
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Pumping)
			{
				return;
			}
			if (!this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			if (!this.pumpFullyOpened && vector3.magnitude > (1f - this.pumpThresholdPercent) * this.strokeLength)
			{
				this.pumpFullyOpened = true;
			}
			else if (this.pumpFullyOpen && vector3.magnitude < this.pumpThresholdPercent * this.strokeLength)
			{
				this.pumpFullyOpened = false;
				this.currentPumpChargeAmount = Mathf.Min(this.currentPumpChargeAmount + this.chargePerPump, this.maxPumpCharge);
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
			}
		}
		else
		{
			if (this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Pumping);
				return;
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				return;
			}
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001C998 File Offset: 0x0001AB98
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle && currentState == SIGadgetBlasterState.Pumping)
		{
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, this.currentPumpChargeAmount + Time.deltaTime * this.remotePumpChargePerSecond);
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001CA78 File Offset: 0x0001AC78
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.blaster.blasterSource.clip = this.idleClip;
			this.blaster.blasterSource.volume = this.idleVolume;
			this.pumpingTransform = null;
			return;
		}
		if (currentState != SIGadgetBlasterState.Pumping)
		{
			return;
		}
		bool flag;
		if (this.blaster.FindAttachedHand(out flag, true, true))
		{
			if (flag)
			{
				this.pumpingTransform = SIPlayer.Get(this.blaster.GetAttachedPlayerActorNumber()).gamePlayer.rightHand;
				return;
			}
			this.pumpingTransform = SIPlayer.Get(this.blaster.GetAttachedPlayerActorNumber()).gamePlayer.leftHand;
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001CB24 File Offset: 0x0001AD24
	public void AttemptFireProjectile(int fireId, float pumpChargeAmount, Vector3 position, Quaternion rotation)
	{
		if (pumpChargeAmount <= 0f)
		{
			return;
		}
		if (pumpChargeAmount - this.maxPumpDiff > this.currentPumpChargeAmount)
		{
			return;
		}
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, pumpChargeAmount);
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
		this.currentPumpChargeAmount = 0f;
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001CC08 File Offset: 0x0001AE08
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
		{
			return;
		}
		float pumpChargeAmount;
		if (!GameEntityManager.ValidateDataType<float>(data[1], out pumpChargeAmount))
		{
			return;
		}
		Vector3 position;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out position))
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
		{
			return;
		}
		this.AttemptFireProjectile(fireId, pumpChargeAmount, position, rotation);
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00002789 File Offset: 0x00000989
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x040005D9 RID: 1497
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x040005DA RID: 1498
	public AudioClip idleClip;

	// Token: 0x040005DB RID: 1499
	public AudioClip cooldownClip;

	// Token: 0x040005DC RID: 1500
	public float idleVolume;

	// Token: 0x040005DD RID: 1501
	public float cooldownVolume;

	// Token: 0x040005DE RID: 1502
	public AudioClip firingClip;

	// Token: 0x040005DF RID: 1503
	public float firingVolume;

	// Token: 0x040005E0 RID: 1504
	public ParticleSystem fireFX;

	// Token: 0x040005E1 RID: 1505
	public Transform pumpHandlePosition;

	// Token: 0x040005E2 RID: 1506
	public Transform pumpFullyClosed;

	// Token: 0x040005E3 RID: 1507
	public Transform pumpFullyOpen;

	// Token: 0x040005E4 RID: 1508
	private GameTriggerInteractable triggerInteractable;

	// Token: 0x040005E5 RID: 1509
	private SIGadgetBlaster blaster;

	// Token: 0x040005E6 RID: 1510
	private Transform pumpingTransform;

	// Token: 0x040005E7 RID: 1511
	public float currentPumpChargeAmount;

	// Token: 0x040005E8 RID: 1512
	public float maxPumpCharge = 1f;

	// Token: 0x040005E9 RID: 1513
	public float remotePumpChargePerSecond = 2f;

	// Token: 0x040005EA RID: 1514
	public float maxPumpDiff = 0.5f;

	// Token: 0x040005EB RID: 1515
	private float chargePerPump = 1f;

	// Token: 0x040005EC RID: 1516
	private bool pumpFullyOpened;

	// Token: 0x040005ED RID: 1517
	private float pumpThresholdPercent = 0.1f;

	// Token: 0x040005EE RID: 1518
	private float strokeLength;
}
