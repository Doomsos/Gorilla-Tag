using System;
using UnityEngine;

// Token: 0x02000809 RID: 2057
public class FreeHoverboardInstance : MonoBehaviour
{
	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x06003622 RID: 13858 RVA: 0x00125ADC File Offset: 0x00123CDC
	// (set) Token: 0x06003623 RID: 13859 RVA: 0x00125AE4 File Offset: 0x00123CE4
	public Rigidbody Rigidbody { get; private set; }

	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x06003624 RID: 13860 RVA: 0x00125AED File Offset: 0x00123CED
	// (set) Token: 0x06003625 RID: 13861 RVA: 0x00125AF5 File Offset: 0x00123CF5
	public Color boardColor { get; private set; }

	// Token: 0x06003626 RID: 13862 RVA: 0x00125B00 File Offset: 0x00123D00
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x00125B48 File Offset: 0x00123D48
	public void SetColor(Color col)
	{
		this.colorMaterial.color = col;
		this.boardColor = col;
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x00125B60 File Offset: 0x00123D60
	private void Update()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(base.transform.TransformPoint(this.sphereCastCenter), base.transform.TransformVector(Vector3.down)), this.sphereCastRadius, ref raycastHit, 1f, this.hoverRaycastMask.value))
		{
			this.hasHoverPoint = true;
			this.hoverPoint = raycastHit.point;
			this.hoverNormal = raycastHit.normal;
			return;
		}
		this.hasHoverPoint = false;
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x00125BDC File Offset: 0x00123DDC
	private void FixedUpdate()
	{
		if (this.hasHoverPoint)
		{
			float num = Vector3.Dot(base.transform.TransformPoint(this.sphereCastCenter) - this.hoverPoint, this.hoverNormal);
			if (num < this.hoverHeight)
			{
				base.transform.position += this.hoverNormal * (this.hoverHeight - num);
				this.Rigidbody.linearVelocity = Vector3.ProjectOnPlane(this.Rigidbody.linearVelocity, this.hoverNormal);
				Vector3 vector = Quaternion.Inverse(base.transform.rotation) * this.Rigidbody.angularVelocity;
				vector.x *= this.avelocityDragWhileHovering;
				vector.z *= this.avelocityDragWhileHovering;
				this.Rigidbody.angularVelocity = base.transform.rotation * vector;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, this.hoverNormal), this.hoverNormal), this.hoverRotationLerp);
			}
		}
	}

	// Token: 0x04004584 RID: 17796
	public int ownerActorNumber;

	// Token: 0x04004585 RID: 17797
	public int boardIndex;

	// Token: 0x04004586 RID: 17798
	[SerializeField]
	private Vector3 sphereCastCenter;

	// Token: 0x04004587 RID: 17799
	[SerializeField]
	private float sphereCastRadius;

	// Token: 0x04004588 RID: 17800
	[SerializeField]
	private LayerMask hoverRaycastMask;

	// Token: 0x04004589 RID: 17801
	[SerializeField]
	private float hoverHeight;

	// Token: 0x0400458A RID: 17802
	[SerializeField]
	private float hoverRotationLerp;

	// Token: 0x0400458B RID: 17803
	[SerializeField]
	private float avelocityDragWhileHovering;

	// Token: 0x0400458C RID: 17804
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x0400458E RID: 17806
	private Material colorMaterial;

	// Token: 0x0400458F RID: 17807
	private bool hasHoverPoint;

	// Token: 0x04004590 RID: 17808
	private Vector3 hoverPoint;

	// Token: 0x04004591 RID: 17809
	private Vector3 hoverNormal;
}
