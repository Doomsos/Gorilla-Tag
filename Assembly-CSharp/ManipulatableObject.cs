using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class ManipulatableObject : HoldableObject
{
	// Token: 0x06001D95 RID: 7573 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnStartManipulation(GameObject grabbingHand)
	{
	}

	// Token: 0x06001D96 RID: 7574 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnStopManipulation(GameObject releasingHand, Vector3 releaseVelocity)
	{
	}

	// Token: 0x06001D97 RID: 7575 RVA: 0x00002076 File Offset: 0x00000276
	protected virtual bool ShouldHandDetach(GameObject hand)
	{
		return false;
	}

	// Token: 0x06001D98 RID: 7576 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnHeldUpdate(GameObject hand)
	{
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnReleasedUpdate()
	{
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x0009BC4C File Offset: 0x00099E4C
	public virtual void LateUpdate()
	{
		if (this.isHeld)
		{
			if (this.holdingHand == null)
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
			this.OnHeldUpdate(this.holdingHand);
			if (this.ShouldHandDetach(this.holdingHand))
			{
				EquipmentInteractor.instance.ForceDropManipulatableObject(this);
				return;
			}
		}
		else
		{
			this.OnReleasedUpdate();
		}
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001D9C RID: 7580 RVA: 0x0009BCAC File Offset: 0x00099EAC
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool forLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, forLeftHand);
		this.isHeld = true;
		this.holdingHand = grabbingHand;
		this.OnStartManipulation(this.holdingHand);
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x0009BCF4 File Offset: 0x00099EF4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(flag).GetAverageVelocity(true, 0.15f, false);
		if (flag)
		{
			EquipmentInteractor.instance.leftHandHeldEquipment = null;
		}
		else
		{
			EquipmentInteractor.instance.rightHandHeldEquipment = null;
		}
		this.isHeld = false;
		this.holdingHand = null;
		this.OnStopManipulation(releasingHand, averageVelocity);
		return true;
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002792 RID: 10130
	protected bool isHeld;

	// Token: 0x04002793 RID: 10131
	protected GameObject holdingHand;
}
