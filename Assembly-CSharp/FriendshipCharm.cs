using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class FriendshipCharm : HoldableObject
{
	// Token: 0x06001C8C RID: 7308 RVA: 0x000976C0 File Offset: 0x000958C0
	private void Awake()
	{
		this.parent = base.transform.parent;
	}

	// Token: 0x06001C8D RID: 7309 RVA: 0x000976D4 File Offset: 0x000958D4
	private void LateUpdate()
	{
		if (!this.isBroken && (this.lineStart.transform.position - this.lineEnd.transform.position).IsLongerThan(this.breakBraceletLength * GTPlayer.Instance.scale))
		{
			this.DestroyBracelet();
		}
	}

	// Token: 0x06001C8E RID: 7310 RVA: 0x0009772C File Offset: 0x0009592C
	public void OnEnable()
	{
		this.interactionPoint.enabled = true;
		this.meshRenderer.enabled = true;
		this.isBroken = false;
		this.UpdatePosition();
	}

	// Token: 0x06001C8F RID: 7311 RVA: 0x00097753 File Offset: 0x00095953
	private void DestroyBracelet()
	{
		this.interactionPoint.enabled = false;
		this.isBroken = true;
		Debug.Log("LeaveGroup: bracelet destroyed");
		FriendshipGroupDetection.Instance.LeaveParty();
	}

	// Token: 0x06001C90 RID: 7312 RVA: 0x0009777C File Offset: 0x0009597C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		bool flag = grabbingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(this, flag);
		GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 2f);
		base.transform.SetParent(flag ? this.leftHandHoldAnchor : this.rightHandHoldAnchor);
		base.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06001C91 RID: 7313 RVA: 0x00097804 File Offset: 0x00095A04
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		bool forLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		EquipmentInteractor.instance.UpdateHandEquipment(null, forLeftHand);
		this.UpdatePosition();
		return base.OnRelease(zoneReleased, releasingHand);
	}

	// Token: 0x06001C92 RID: 7314 RVA: 0x00097840 File Offset: 0x00095A40
	private void UpdatePosition()
	{
		base.transform.SetParent(this.parent);
		base.transform.localPosition = this.releasePosition.localPosition;
		base.transform.localRotation = this.releasePosition.localRotation;
	}

	// Token: 0x06001C93 RID: 7315 RVA: 0x00097880 File Offset: 0x00095A80
	private void OnCollisionEnter(Collision other)
	{
		if (!this.isBroken)
		{
			return;
		}
		if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
		{
			return;
		}
		this.meshRenderer.enabled = false;
		this.UpdatePosition();
	}

	// Token: 0x06001C94 RID: 7316 RVA: 0x00002789 File Offset: 0x00000989
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06001C95 RID: 7317 RVA: 0x00002789 File Offset: 0x00000989
	public override void DropItemCleanup()
	{
	}

	// Token: 0x04002697 RID: 9879
	[SerializeField]
	private InteractionPoint interactionPoint;

	// Token: 0x04002698 RID: 9880
	[SerializeField]
	private Transform rightHandHoldAnchor;

	// Token: 0x04002699 RID: 9881
	[SerializeField]
	private Transform leftHandHoldAnchor;

	// Token: 0x0400269A RID: 9882
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x0400269B RID: 9883
	[SerializeField]
	private Transform lineStart;

	// Token: 0x0400269C RID: 9884
	[SerializeField]
	private Transform lineEnd;

	// Token: 0x0400269D RID: 9885
	[SerializeField]
	private Transform releasePosition;

	// Token: 0x0400269E RID: 9886
	[SerializeField]
	private float breakBraceletLength;

	// Token: 0x0400269F RID: 9887
	[SerializeField]
	private LayerMask breakItemLayerMask;

	// Token: 0x040026A0 RID: 9888
	private Transform parent;

	// Token: 0x040026A1 RID: 9889
	private bool isBroken;
}
