using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class SIGadgetGrenadeDisrupt : SIGadgetGrenade
{
	// Token: 0x0600059D RID: 1437 RVA: 0x00020473 File Offset: 0x0001E673
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeDisrupt.State.Idle;
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x00020484 File Offset: 0x0001E684
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeDisrupt.State state = (SIGadgetGrenadeDisrupt.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x000204AE File Offset: 0x0001E6AE
	private void SetStateAuthority(SIGadgetGrenadeDisrupt.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000204D0 File Offset: 0x0001E6D0
	private void SetState(SIGadgetGrenadeDisrupt.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeDisrupt.State.Idle:
		case SIGadgetGrenadeDisrupt.State.Thrown:
			break;
		case SIGadgetGrenadeDisrupt.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00020510 File Offset: 0x0001E710
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			I_SIDisruptable componentInParent = array[i].GetComponentInParent<I_SIDisruptable>();
			if (componentInParent != null)
			{
				componentInParent.Disrupt(this.disruptTime);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished.Invoke();
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleActivated()
	{
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00020588 File Offset: 0x0001E788
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Triggered);
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0002059A File Offset: 0x0001E79A
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Thrown);
		}
	}

	// Token: 0x040006FD RID: 1789
	public float disruptTime;

	// Token: 0x040006FE RID: 1790
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040006FF RID: 1791
	private SIGadgetGrenadeDisrupt.State state;

	// Token: 0x020000E7 RID: 231
	private enum State
	{
		// Token: 0x04000701 RID: 1793
		Idle,
		// Token: 0x04000702 RID: 1794
		Thrown,
		// Token: 0x04000703 RID: 1795
		Triggered
	}
}
