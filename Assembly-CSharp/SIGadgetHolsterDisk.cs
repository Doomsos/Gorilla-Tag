using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class SIGadgetHolsterDisk : SIGadget, I_SIDisruptable
{
	// Token: 0x060005CD RID: 1485 RVA: 0x00020C9C File Offset: 0x0001EE9C
	private void Awake()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.referenceGadget.gameObject.SetActive(false);
		this.referenceTransform = this.referenceGadget.transform;
		this.cooldownTimer = 0f;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00020CD2 File Offset: 0x0001EED2
	private void Start()
	{
		this.CreateGadget();
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00020CDC File Offset: 0x0001EEDC
	private void CreateGadget()
	{
		this.gameEntity.manager.RequestCreateItem(this.referenceGadget.gameObject.name.GetStaticHash(), this.referenceGadget.transform.position, this.referenceGadget.transform.rotation, (long)this.gameEntity.GetNetId());
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00020D3C File Offset: 0x0001EF3C
	public void RegisterGadget(SIGadget gadget)
	{
		this.cachedGadget = gadget;
		this.grenadeGadget = this.cachedGadget.GetComponent<SIGadgetGrenade>();
		this.gadgetRB = this.cachedGadget.GetComponent<Rigidbody>();
		SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
		sigadgetGrenade.GrenadeFinished = (Action)Delegate.Combine(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		this.cachedGadget.gameObject.SetActive(false);
		this.GadgetRespawn();
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00020DB0 File Offset: 0x0001EFB0
	private new void OnDisable()
	{
		if (this.grenadeGadget != null)
		{
			SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
			sigadgetGrenade.GrenadeFinished = (Action)Delegate.Remove(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		}
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00020DE8 File Offset: 0x0001EFE8
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
		case SIGadgetHolsterDisk.State.Ready:
			break;
		case SIGadgetHolsterDisk.State.OnCooldown:
			this.cooldownTimer += dt;
			this.grenadeGadget.grenadeRenderer.material.SetFloat("_RespawnAmount", this.cooldownTimer / this.cooldownTime);
			if (this.cooldownTimer > this.cooldownTime)
			{
				this.SetState(SIGadgetHolsterDisk.State.Ready);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00020E60 File Offset: 0x0001F060
	private void SetState(SIGadgetHolsterDisk.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
			this.cooldownTimer = 0f;
			return;
		case SIGadgetHolsterDisk.State.OnCooldown:
			break;
		case SIGadgetHolsterDisk.State.Ready:
			this.cachedGadget.gameEntity.pickupable = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00020EB5 File Offset: 0x0001F0B5
	public void DiskSnappedToHolster()
	{
		this.cachedGadget.gameObject.SetActive(true);
		this.gameEntity.pickupable = false;
		this.GadgetRespawn();
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00020EDA File Offset: 0x0001F0DA
	public void DiskRemovedFromHolster()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.gameEntity.pickupable = true;
		this.cachedGadget.gameObject.SetActive(false);
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00020F00 File Offset: 0x0001F100
	public void GadgetRespawn()
	{
		this.cachedGadget.transform.parent = base.transform;
		this.cachedGadget.transform.localPosition = this.referenceTransform.localPosition;
		this.cachedGadget.transform.localRotation = this.referenceTransform.localRotation;
		this.cachedGadget.gameEntity.pickupable = false;
		this.gadgetRB.isKinematic = true;
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = 0f;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00020F88 File Offset: 0x0001F188
	public void Disrupt(float disruptTime)
	{
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = -disruptTime;
	}

	// Token: 0x0400072B RID: 1835
	public SIGadget referenceGadget;

	// Token: 0x0400072C RID: 1836
	public float cooldownTime;

	// Token: 0x0400072D RID: 1837
	private SIGadgetHolsterDisk.State state;

	// Token: 0x0400072E RID: 1838
	private float cooldownTimer;

	// Token: 0x0400072F RID: 1839
	private SIGadgetGrenade grenadeGadget;

	// Token: 0x04000730 RID: 1840
	private Rigidbody gadgetRB;

	// Token: 0x04000731 RID: 1841
	private SIGadget cachedGadget;

	// Token: 0x04000732 RID: 1842
	private Transform referenceTransform;

	// Token: 0x020000F1 RID: 241
	private enum State
	{
		// Token: 0x04000734 RID: 1844
		Unequipped,
		// Token: 0x04000735 RID: 1845
		OnCooldown,
		// Token: 0x04000736 RID: 1846
		Ready
	}
}
