using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class SIGadgetGrenadeGravity : SIGadgetGrenade
{
	// Token: 0x060005A6 RID: 1446 RVA: 0x000205AB File Offset: 0x0001E7AB
	protected override void OnEnable()
	{
		base.OnEnable();
		this.gravityField.SetActive(false);
		this.state = SIGadgetGrenadeGravity.State.Idle;
		this.stateRemainingDuration = -1f;
		this.isLocalPlayerInEffect = false;
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x000205D8 File Offset: 0x0001E7D8
	protected override void HandleActivated()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Idle)
		{
			this.activatedLocally = true;
			this.SetStateAuthority(SIGadgetGrenadeGravity.State.Activated);
			return;
		}
		this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleThrown()
	{
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00002789 File Offset: 0x00000989
	protected override void HandleHitSurface()
	{
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x000205F8 File Offset: 0x0001E7F8
	protected override void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			break;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Triggered);
				return;
			}
			break;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
				return;
			}
			if (this.freezePositionOnTrigger)
			{
				this.CheckReenabledFreezePosition();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00020674 File Offset: 0x0001E874
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeGravity.State state = (SIGadgetGrenadeGravity.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		if (this.freezePositionOnTrigger)
		{
			this.CheckReenabledFreezePosition();
		}
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x000206AC File Offset: 0x0001E8AC
	private void SetStateAuthority(SIGadgetGrenadeGravity.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x000206D0 File Offset: 0x0001E8D0
	private void SetState(SIGadgetGrenadeGravity.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			this.activatedLocally = false;
			this.stateRemainingDuration = -1f;
			this.mesh.material = this.idleMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration = this.counterDuration;
			this.mesh.material = this.activatedMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration = this.triggerDuration;
			this.mesh.material = this.triggeredMat;
			this.ActivateGravityEffect();
			return;
		default:
			return;
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00020783 File Offset: 0x0001E983
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00020792 File Offset: 0x0001E992
	private void ActivateGravityEffect()
	{
		this.gravityField.SetActive(true);
		if (this.freezePositionOnTrigger)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x000207C4 File Offset: 0x0001E9C4
	private void DeactivateGravityEffect()
	{
		this.gravityField.SetActive(false);
		if (this.isLocalPlayerInEffect)
		{
			this.isLocalPlayerInEffect = false;
			GTPlayer instance = GTPlayer.Instance;
			if (instance != null)
			{
				instance.UnsetGravityOverride(this);
			}
		}
		if (this.freezePositionOnTrigger && !this.thrownGadget.IsHeld())
		{
			this.rb.isKinematic = false;
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00020824 File Offset: 0x0001EA24
	private void CheckReenabledFreezePosition()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Triggered && !this.thrownGadget.IsHeld() && !this.rb.isKinematic)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00020870 File Offset: 0x0001EA70
	private void OnTriggerEnter(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = true;
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x000208B4 File Offset: 0x0001EAB4
	private void OnTriggerExit(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = false;
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x000208EC File Offset: 0x0001EAEC
	public void GravityOverrideFunction(GTPlayer player)
	{
		Vector3 vector = Physics.gravity * this.standardGravityMultiplier;
		Vector3 vector2 = Vector3.zero;
		if (!this.thrownGadget.IsHeldLocal())
		{
			vector2 = (base.transform.position - player.headCollider.transform.position).normalized * this.attractorStrength;
		}
		player.AddForce((vector + vector2) * player.scale, 5);
	}

	// Token: 0x04000704 RID: 1796
	[Header("Activation")]
	[SerializeField]
	private float counterDuration = 1f;

	// Token: 0x04000705 RID: 1797
	[Header("Gravity Effect")]
	[SerializeField]
	private GameObject gravityField;

	// Token: 0x04000706 RID: 1798
	[SerializeField]
	private bool freezePositionOnTrigger;

	// Token: 0x04000707 RID: 1799
	[SerializeField]
	private float triggerDuration = 5f;

	// Token: 0x04000708 RID: 1800
	[SerializeField]
	private float standardGravityMultiplier = 1f;

	// Token: 0x04000709 RID: 1801
	[SerializeField]
	private float attractorStrength;

	// Token: 0x0400070A RID: 1802
	[Header("FX")]
	[SerializeField]
	private MeshRenderer mesh;

	// Token: 0x0400070B RID: 1803
	[SerializeField]
	private Material idleMat;

	// Token: 0x0400070C RID: 1804
	[SerializeField]
	private Material activatedMat;

	// Token: 0x0400070D RID: 1805
	[SerializeField]
	private Material triggeredMat;

	// Token: 0x0400070E RID: 1806
	private SIGadgetGrenadeGravity.State state;

	// Token: 0x0400070F RID: 1807
	private float stateRemainingDuration;

	// Token: 0x04000710 RID: 1808
	private bool isLocalPlayerInEffect;

	// Token: 0x020000E9 RID: 233
	private enum State
	{
		// Token: 0x04000712 RID: 1810
		Idle,
		// Token: 0x04000713 RID: 1811
		Activated,
		// Token: 0x04000714 RID: 1812
		Triggered,
		// Token: 0x04000715 RID: 1813
		Count
	}
}
