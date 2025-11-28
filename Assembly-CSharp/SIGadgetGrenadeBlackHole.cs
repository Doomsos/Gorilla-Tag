using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class SIGadgetGrenadeBlackHole : SIGadgetGrenade
{
	// Token: 0x06000593 RID: 1427 RVA: 0x00020313 File Offset: 0x0001E513
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeBlackHole.State.Idle;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleActivated()
	{
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00020322 File Offset: 0x0001E522
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Triggered);
		}
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00020334 File Offset: 0x0001E534
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Thrown);
		}
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00020348 File Offset: 0x0001E548
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeBlackHole.State state = (SIGadgetGrenadeBlackHole.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00020372 File Offset: 0x0001E572
	private void SetStateAuthority(SIGadgetGrenadeBlackHole.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x00020394 File Offset: 0x0001E594
	private void SetState(SIGadgetGrenadeBlackHole.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeBlackHole.State.Idle:
		case SIGadgetGrenadeBlackHole.State.Thrown:
			break;
		case SIGadgetGrenadeBlackHole.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x000203D4 File Offset: 0x0001E5D4
	private void TriggerExplosion()
	{
		Vector3 vector = base.transform.position - GTPlayer.Instance.transform.position;
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
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Idle);
		}
	}

	// Token: 0x040006F6 RID: 1782
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x040006F7 RID: 1783
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040006F8 RID: 1784
	private SIGadgetGrenadeBlackHole.State state;

	// Token: 0x020000E5 RID: 229
	private enum State
	{
		// Token: 0x040006FA RID: 1786
		Idle,
		// Token: 0x040006FB RID: 1787
		Thrown,
		// Token: 0x040006FC RID: 1788
		Triggered
	}
}
