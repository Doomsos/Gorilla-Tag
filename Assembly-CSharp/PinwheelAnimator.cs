using System;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public class PinwheelAnimator : MonoBehaviour
{
	// Token: 0x06000B35 RID: 2869 RVA: 0x0003CFC1 File Offset: 0x0003B1C1
	protected void OnEnable()
	{
		this.oldPos = this.spinnerTransform.position;
		this.spinSpeed = 0f;
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003CFE0 File Offset: 0x0003B1E0
	protected void LateUpdate()
	{
		Vector3 position = this.spinnerTransform.position;
		Vector3 forward = base.transform.forward;
		Vector3 vector = position - this.oldPos;
		float num = Mathf.Clamp(vector.magnitude / Time.deltaTime * Vector3.Dot(vector.normalized, forward) * this.spinSpeedMultiplier, -this.maxSpinSpeed, this.maxSpinSpeed);
		this.spinSpeed = Mathf.Lerp(this.spinSpeed, num, Time.deltaTime * this.damping);
		this.spinnerTransform.Rotate(Vector3.forward, this.spinSpeed * 360f * Time.deltaTime);
		this.oldPos = position;
	}

	// Token: 0x04000DA3 RID: 3491
	public Transform spinnerTransform;

	// Token: 0x04000DA4 RID: 3492
	[Tooltip("In revolutions per second.")]
	public float maxSpinSpeed = 4f;

	// Token: 0x04000DA5 RID: 3493
	public float spinSpeedMultiplier = 5f;

	// Token: 0x04000DA6 RID: 3494
	public float damping = 0.5f;

	// Token: 0x04000DA7 RID: 3495
	private Vector3 oldPos;

	// Token: 0x04000DA8 RID: 3496
	private float spinSpeed;
}
