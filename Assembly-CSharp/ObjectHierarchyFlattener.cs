using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020002DF RID: 735
[DefaultExecutionOrder(2001)]
public class ObjectHierarchyFlattener : MonoBehaviour
{
	// Token: 0x060011FB RID: 4603 RVA: 0x0005EDD8 File Offset: 0x0005CFD8
	private void ResetTransform()
	{
		if (this.originalParentGO.activeInHierarchy)
		{
			return;
		}
		base.transform.SetParent(this.originalParentTransform);
		this.isAttachedToOverride = false;
		base.transform.localPosition = this.originalLocalPosition;
		base.transform.localRotation = this.originalLocalRotation;
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0005EE3E File Offset: 0x0005D03E
	public void CrumbDisabled()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		}
		base.Invoke("ResetTransform", 0f);
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x0005EE68 File Offset: 0x0005D068
	public void InvokeLateUpdate()
	{
		if (this.maintainRelativeScale)
		{
			base.transform.localScale = Vector3.Scale(this.originalParentTransform.lossyScale, this.originalScale);
		}
		base.transform.rotation = this.originalParentTransform.rotation * this.originalLocalRotation;
		base.transform.position = this.originalParentTransform.position + base.transform.rotation * this.calcOffset * (this.originalParentTransform.lossyScale.x / this.originalParentScale) * this.originalParentScale;
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x0005EF18 File Offset: 0x0005D118
	private void OnEnable()
	{
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.RegisterOHF(this);
		}
		if (!this.isAttachedToOverride)
		{
			this.originalParentTransform = base.transform.parent;
			this.originalParentGO = this.originalParentTransform.gameObject;
			this.originalLocalPosition = base.transform.localPosition;
			this.originalLocalRotation = base.transform.localRotation;
			this.originalParentScale = base.transform.parent.lossyScale.x;
			this.originalScale = base.transform.localScale;
			this.calcOffset = Vector3.Scale(this.originalLocalPosition, this.originalScale);
			FlattenerCrumb flattenerCrumb = this.originalParentGO.GetComponent<FlattenerCrumb>();
			if (flattenerCrumb == null)
			{
				flattenerCrumb = this.originalParentGO.AddComponent<FlattenerCrumb>();
			}
			flattenerCrumb.AddFlattenerReference(this);
		}
		base.transform.SetParent((this.overrideParentTransform != null) ? this.overrideParentTransform : null);
		this.isAttachedToOverride = true;
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x0005F016 File Offset: 0x0005D216
	private void OnDisable()
	{
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		base.Invoke("ResetTransformIfStillDisabled", 0f);
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x0005F02E File Offset: 0x0005D22E
	private void ResetTransformIfStillDisabled()
	{
		if (!base.isActiveAndEnabled)
		{
			this.ResetTransform();
		}
	}

	// Token: 0x0400169E RID: 5790
	public const int k_monoDefaultExecutionOrder = 2001;

	// Token: 0x0400169F RID: 5791
	[DebugReadout]
	private GameObject originalParentGO;

	// Token: 0x040016A0 RID: 5792
	private Transform originalParentTransform;

	// Token: 0x040016A1 RID: 5793
	private Vector3 originalLocalPosition;

	// Token: 0x040016A2 RID: 5794
	private Vector3 calcOffset;

	// Token: 0x040016A3 RID: 5795
	private Quaternion originalLocalRotation;

	// Token: 0x040016A4 RID: 5796
	private Vector3 originalScale;

	// Token: 0x040016A5 RID: 5797
	private float originalParentScale;

	// Token: 0x040016A6 RID: 5798
	public bool trackTransformOfParent;

	// Token: 0x040016A7 RID: 5799
	public bool maintainRelativeScale;

	// Token: 0x040016A8 RID: 5800
	private FlattenerCrumb crumb;

	// Token: 0x040016A9 RID: 5801
	public Transform overrideParentTransform;

	// Token: 0x040016AA RID: 5802
	private bool isAttachedToOverride;
}
