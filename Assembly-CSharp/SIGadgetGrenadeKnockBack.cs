using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class SIGadgetGrenadeKnockBack : SIGadgetGrenade
{
	// Token: 0x060005B6 RID: 1462 RVA: 0x00020993 File Offset: 0x0001EB93
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeKnockBack.State.Idle;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleActivated()
	{
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x000209A2 File Offset: 0x0001EBA2
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Triggered);
		}
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x000209B4 File Offset: 0x0001EBB4
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Thrown);
		}
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x000209C8 File Offset: 0x0001EBC8
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeKnockBack.State state = (SIGadgetGrenadeKnockBack.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x000209F2 File Offset: 0x0001EBF2
	private void SetStateAuthority(SIGadgetGrenadeKnockBack.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020A14 File Offset: 0x0001EC14
	private void SetState(SIGadgetGrenadeKnockBack.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeKnockBack.State.Idle:
		case SIGadgetGrenadeKnockBack.State.Thrown:
			break;
		case SIGadgetGrenadeKnockBack.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00020A54 File Offset: 0x0001EC54
	private void TriggerExplosion()
	{
		Vector3 vector = GTPlayer.Instance.transform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		if (this.explosionRadius * this.explosionRadius > sqrMagnitude)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			float num2 = 1f - num / this.explosionRadius;
			float speed = this.knockbackStrength * num2;
			GTPlayer.Instance.ApplyKnockback(vector.normalized, speed, false);
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished.Invoke();
	}

	// Token: 0x04000716 RID: 1814
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x04000717 RID: 1815
	[SerializeField]
	private float explosionRadius;

	// Token: 0x04000718 RID: 1816
	private SIGadgetGrenadeKnockBack.State state;

	// Token: 0x020000EB RID: 235
	private enum State
	{
		// Token: 0x0400071A RID: 1818
		Idle,
		// Token: 0x0400071B RID: 1819
		Thrown,
		// Token: 0x0400071C RID: 1820
		Triggered
	}
}
