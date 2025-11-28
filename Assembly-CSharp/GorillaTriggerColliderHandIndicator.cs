using System;
using UnityEngine;

// Token: 0x02000929 RID: 2345
public class GorillaTriggerColliderHandIndicator : MonoBehaviourTick
{
	// Token: 0x06003BF1 RID: 15345 RVA: 0x0013C910 File Offset: 0x0013AB10
	public override void Tick()
	{
		this.currentVelocity = (base.transform.position - this.lastPosition) / Time.deltaTime;
		this.lastPosition = base.transform.position;
	}

	// Token: 0x06003BF2 RID: 15346 RVA: 0x0013C949 File Offset: 0x0013AB49
	private void OnTriggerEnter(Collider other)
	{
		if (this.throwableController != null)
		{
			this.throwableController.GrabbableObjectHover(this.isLeftHand);
		}
	}

	// Token: 0x04004C80 RID: 19584
	public Vector3 currentVelocity;

	// Token: 0x04004C81 RID: 19585
	public Vector3 lastPosition = Vector3.zero;

	// Token: 0x04004C82 RID: 19586
	public bool isLeftHand;

	// Token: 0x04004C83 RID: 19587
	public GorillaThrowableController throwableController;
}
