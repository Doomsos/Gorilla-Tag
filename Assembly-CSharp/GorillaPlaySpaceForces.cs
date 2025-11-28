using System;
using UnityEngine;

// Token: 0x02000522 RID: 1314
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x0600215E RID: 8542 RVA: 0x000AF5AC File Offset: 0x000AD7AC
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000AF609 File Offset: 0x000AD809
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x04002C0C RID: 11276
	public GameObject rightHand;

	// Token: 0x04002C0D RID: 11277
	public GameObject leftHand;

	// Token: 0x04002C0E RID: 11278
	public Collider bodyCollider;

	// Token: 0x04002C0F RID: 11279
	private Collider leftHandCollider;

	// Token: 0x04002C10 RID: 11280
	private Collider rightHandCollider;

	// Token: 0x04002C11 RID: 11281
	public Transform rightHandTransform;

	// Token: 0x04002C12 RID: 11282
	public Transform leftHandTransform;

	// Token: 0x04002C13 RID: 11283
	private Rigidbody leftHandRigidbody;

	// Token: 0x04002C14 RID: 11284
	private Rigidbody rightHandRigidbody;

	// Token: 0x04002C15 RID: 11285
	public Vector3 bodyColliderOffset;

	// Token: 0x04002C16 RID: 11286
	public float forceConstant;

	// Token: 0x04002C17 RID: 11287
	private Vector3 lastLeftHandPosition;

	// Token: 0x04002C18 RID: 11288
	private Vector3 lastRightHandPosition;

	// Token: 0x04002C19 RID: 11289
	private Rigidbody playspaceRigidbody;

	// Token: 0x04002C1A RID: 11290
	public Transform headsetTransform;
}
