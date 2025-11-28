using System;
using UnityEngine;

// Token: 0x0200052C RID: 1324
public class GorillaWalkingGrab : MonoBehaviour
{
	// Token: 0x06002174 RID: 8564 RVA: 0x000AF7D1 File Offset: 0x000AD9D1
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
		this.positionHistory = new Vector3[this.historySteps];
		this.historyIndex = 0;
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000AF7FC File Offset: 0x000AD9FC
	private void FixedUpdate()
	{
		this.historyIndex++;
		if (this.historyIndex >= this.historySteps)
		{
			this.historyIndex = 0;
		}
		this.positionHistory[this.historyIndex] = this.handToStickTo.transform.position;
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x00002076 File Offset: 0x00000276
	private bool MakeJump()
	{
		return false;
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x000AF884 File Offset: 0x000ADA84
	private void OnCollisionStay(Collision collision)
	{
		if (!this.MakeJump())
		{
			Vector3 vector = Vector3.ProjectOnPlane(this.positionHistory[(this.historyIndex != 0) ? (this.historyIndex - 1) : (this.historySteps - 1)] - this.handToStickTo.transform.position, collision.GetContact(0).normal);
			Vector3 vector2 = this.thisRigidbody.transform.position - this.handToStickTo.transform.position;
			this.playspaceRigidbody.MovePosition(this.playspaceRigidbody.transform.position + vector - vector2);
		}
	}

	// Token: 0x04002C2C RID: 11308
	public GameObject handToStickTo;

	// Token: 0x04002C2D RID: 11309
	public float ratioToUse;

	// Token: 0x04002C2E RID: 11310
	public float forceMultiplier;

	// Token: 0x04002C2F RID: 11311
	public int historySteps;

	// Token: 0x04002C30 RID: 11312
	public Rigidbody playspaceRigidbody;

	// Token: 0x04002C31 RID: 11313
	private Rigidbody thisRigidbody;

	// Token: 0x04002C32 RID: 11314
	private Vector3 lastPosition;

	// Token: 0x04002C33 RID: 11315
	private Vector3 maybeLastPositionIDK;

	// Token: 0x04002C34 RID: 11316
	private Vector3[] positionHistory;

	// Token: 0x04002C35 RID: 11317
	private int historyIndex;
}
