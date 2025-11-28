using System;
using UnityEngine;

// Token: 0x02000241 RID: 577
public class CosmeticFan : MonoBehaviour
{
	// Token: 0x06000F1C RID: 3868 RVA: 0x00050513 File Offset: 0x0004E713
	private void Start()
	{
		this.spinUpRate = this.maxSpeed / this.spinUpDuration;
		this.spinDownRate = this.maxSpeed / this.spinDownDuration;
	}

	// Token: 0x06000F1D RID: 3869 RVA: 0x0005053C File Offset: 0x0004E73C
	public void Run()
	{
		this.targetSpeed = this.maxSpeed;
		if (this.spinUpDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinUpRate;
		}
		else
		{
			this.currentSpeed = this.maxSpeed;
		}
		base.enabled = true;
	}

	// Token: 0x06000F1E RID: 3870 RVA: 0x0005058A File Offset: 0x0004E78A
	public void Stop()
	{
		this.targetSpeed = 0f;
		if (this.spinDownDuration > 0f)
		{
			base.enabled = true;
			this.currentAccelRate = this.spinDownRate;
			return;
		}
		this.currentSpeed = 0f;
	}

	// Token: 0x06000F1F RID: 3871 RVA: 0x000505C3 File Offset: 0x0004E7C3
	public void InstantStop()
	{
		this.targetSpeed = 0f;
		this.currentSpeed = 0f;
		base.enabled = false;
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x000505E4 File Offset: 0x0004E7E4
	private void Update()
	{
		this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.targetSpeed, this.currentAccelRate * Time.deltaTime);
		base.transform.localRotation = base.transform.localRotation * Quaternion.AngleAxis(this.currentSpeed * Time.deltaTime, this.axis);
		if (this.currentSpeed == 0f && this.targetSpeed == 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x04001284 RID: 4740
	[SerializeField]
	private Vector3 axis;

	// Token: 0x04001285 RID: 4741
	[SerializeField]
	private float spinUpDuration = 0.3f;

	// Token: 0x04001286 RID: 4742
	[SerializeField]
	private float spinDownDuration = 0.3f;

	// Token: 0x04001287 RID: 4743
	[SerializeField]
	private float maxSpeed = 360f;

	// Token: 0x04001288 RID: 4744
	private float currentSpeed;

	// Token: 0x04001289 RID: 4745
	private float targetSpeed;

	// Token: 0x0400128A RID: 4746
	private float currentAccelRate;

	// Token: 0x0400128B RID: 4747
	private float spinUpRate;

	// Token: 0x0400128C RID: 4748
	private float spinDownRate;
}
