using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x06000B44 RID: 2884 RVA: 0x0003D323 File Offset: 0x0003B523
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x0003D356 File Offset: 0x0003B556
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x0003D364 File Offset: 0x0003B564
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x0003D372 File Offset: 0x0003B572
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x0003D3A4 File Offset: 0x0003B5A4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x0003D428 File Offset: 0x0003B628
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 vector = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, vector, ref raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x0003D518 File Offset: 0x0003B718
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x04000DBF RID: 3519
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04000DC0 RID: 3520
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x04000DC1 RID: 3521
	public float tipCollisionRadius = 0.05f;

	// Token: 0x04000DC2 RID: 3522
	public LayerMask tipCollisionLayerMask;

	// Token: 0x04000DC3 RID: 3523
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04000DC4 RID: 3524
	public float cooldown = 5f;

	// Token: 0x04000DC5 RID: 3525
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x04000DC6 RID: 3526
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x04000DC7 RID: 3527
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04000DC8 RID: 3528
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x04000DC9 RID: 3529
	private Vector3 tipTargetLocalPosition;

	// Token: 0x04000DCA RID: 3530
	private bool hasEffectsGameObject;

	// Token: 0x04000DCB RID: 3531
	private bool effectsHaveBeenPlayed;
}
