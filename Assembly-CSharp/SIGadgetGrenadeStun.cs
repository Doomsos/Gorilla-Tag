using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class SIGadgetGrenadeStun : SIGadgetGrenade
{
	// Token: 0x060005C0 RID: 1472 RVA: 0x00020AFB File Offset: 0x0001ECFB
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeStun.State.Idle;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleActivated()
	{
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00020B0A File Offset: 0x0001ED0A
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeStun.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Triggered);
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00020B1C File Offset: 0x0001ED1C
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeStun.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Thrown);
		}
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00020B30 File Offset: 0x0001ED30
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeStun.State state = (SIGadgetGrenadeStun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00020B5A File Offset: 0x0001ED5A
	private void SetStateAuthority(SIGadgetGrenadeStun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00020B7C File Offset: 0x0001ED7C
	private void SetState(SIGadgetGrenadeStun.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeStun.State.Idle:
		case SIGadgetGrenadeStun.State.Thrown:
			break;
		case SIGadgetGrenadeStun.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00020BBC File Offset: 0x0001EDBC
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius, UnityLayer.GorillaTagCollider.ToLayerMask());
		for (int i = 0; i < array.Length; i++)
		{
			VRRig componentInParent = array[i].GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				Vector3 vector = componentInParent.transform.position - base.transform.position;
				float magnitude = vector.magnitude;
				float num = 1f - magnitude / this.explosionRadius;
				float num2 = this.knockbackStrength * num;
				RoomSystem.LaunchPlayer(componentInParent.OwningNetPlayer, num2 * vector / magnitude);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, componentInParent.OwningNetPlayer);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Idle);
		}
	}

	// Token: 0x0400071D RID: 1821
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x0400071E RID: 1822
	[SerializeField]
	private float explosionRadius;

	// Token: 0x0400071F RID: 1823
	private SIGadgetGrenadeStun.State state;

	// Token: 0x020000ED RID: 237
	private enum State
	{
		// Token: 0x04000721 RID: 1825
		Idle,
		// Token: 0x04000722 RID: 1826
		Thrown,
		// Token: 0x04000723 RID: 1827
		Triggered
	}
}
