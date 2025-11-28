using System;
using System.Diagnostics;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class CrittersToolThrowable : CrittersActor
{
	// Token: 0x060002D2 RID: 722 RVA: 0x000111B2 File Offset: 0x0000F3B2
	public override void Initialize()
	{
		base.Initialize();
		this.hasBeenGrabbedByPlayer = false;
		this.shouldDisable = false;
		this.hasTriggeredSinceLastGrab = false;
		this._sqrActivationSpeed = this.requiredActivationSpeed * this.requiredActivationSpeed;
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000111E2 File Offset: 0x0000F3E2
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.hasBeenGrabbedByPlayer = true;
		this.hasTriggeredSinceLastGrab = false;
		this.OnPickedUp();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00011208 File Offset: 0x0000F408
	public void OnCollisionEnter(Collision collision)
	{
		if (LayerMaskExtensions.Contains(CrittersManager.instance.containerLayer, collision.gameObject.layer))
		{
			return;
		}
		if (this.requiresPlayerGrabBeforeActivate && !this.hasBeenGrabbedByPlayer)
		{
			return;
		}
		if (this._sqrActivationSpeed > 0f && collision.relativeVelocity.sqrMagnitude < this._sqrActivationSpeed)
		{
			return;
		}
		if (this.onlyTriggerOncePerGrab && this.hasTriggeredSinceLastGrab)
		{
			return;
		}
		if (this.onlyTriggerOnDirectCritterHit)
		{
			CrittersPawn component = collision.gameObject.GetComponent<CrittersPawn>();
			if (component != null && component.isActiveAndEnabled)
			{
				this.hasTriggeredSinceLastGrab = true;
				this.OnImpactCritter(component);
			}
		}
		else
		{
			Vector3 point = collision.contacts[0].point;
			Vector3 normal = collision.contacts[0].normal;
			this.hasTriggeredSinceLastGrab = true;
			this.OnImpact(point, normal);
		}
		if (this.destroyOnImpact)
		{
			this.shouldDisable = true;
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnImpactCritter(CrittersPawn impactedCritter)
	{
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnPickedUp()
	{
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x000112F4 File Offset: 0x0000F4F4
	[Conditional("DRAW_DEBUG")]
	protected void ShowDebugVisualization(Vector3 position, float scale, float duration = 0f)
	{
		if (!this.debugImpactPrefab)
		{
			return;
		}
		DelayedDestroyObject delayedDestroyObject = Object.Instantiate<DelayedDestroyObject>(this.debugImpactPrefab, position, Quaternion.identity);
		delayedDestroyObject.transform.localScale *= scale;
		if (duration != 0f)
		{
			delayedDestroyObject.lifetime = duration;
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x00011348 File Offset: 0x0000F548
	public override bool ProcessLocal()
	{
		bool result = base.ProcessLocal();
		if (this.shouldDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return result;
	}

	// Token: 0x060002DA RID: 730 RVA: 0x00011374 File Offset: 0x0000F574
	public override void TogglePhysics(bool enable)
	{
		if (enable)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = 1;
			this.rb.collisionDetectionMode = 1;
			return;
		}
		this.rb.isKinematic = true;
		this.rb.interpolation = 0;
		this.rb.collisionDetectionMode = 1;
	}

	// Token: 0x0400033B RID: 827
	[Header("Throwable")]
	public bool requiresPlayerGrabBeforeActivate = true;

	// Token: 0x0400033C RID: 828
	public float requiredActivationSpeed = 2f;

	// Token: 0x0400033D RID: 829
	public bool onlyTriggerOnDirectCritterHit;

	// Token: 0x0400033E RID: 830
	public bool destroyOnImpact = true;

	// Token: 0x0400033F RID: 831
	public bool onlyTriggerOncePerGrab = true;

	// Token: 0x04000340 RID: 832
	[Header("Debug")]
	[SerializeField]
	private DelayedDestroyObject debugImpactPrefab;

	// Token: 0x04000341 RID: 833
	private bool hasBeenGrabbedByPlayer;

	// Token: 0x04000342 RID: 834
	protected bool shouldDisable;

	// Token: 0x04000343 RID: 835
	private bool hasTriggeredSinceLastGrab;

	// Token: 0x04000344 RID: 836
	private float _sqrActivationSpeed;
}
