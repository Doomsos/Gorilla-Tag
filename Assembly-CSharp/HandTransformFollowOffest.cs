using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005B8 RID: 1464
[Serializable]
internal class HandTransformFollowOffest
{
	// Token: 0x060024E3 RID: 9443 RVA: 0x000C6730 File Offset: 0x000C4930
	internal void UpdatePositionRotation()
	{
		if (this.followTransform == null || this.targetTransforms == null)
		{
			return;
		}
		this.position = this.followTransform.position + this.followTransform.rotation * this.positionOffset * GTPlayer.Instance.scale;
		this.rotation = this.followTransform.rotation * this.rotationOffset;
		foreach (Transform transform in this.targetTransforms)
		{
			transform.position = this.position;
			transform.rotation = this.rotation;
		}
	}

	// Token: 0x0400307D RID: 12413
	internal Transform followTransform;

	// Token: 0x0400307E RID: 12414
	[SerializeField]
	private Transform[] targetTransforms;

	// Token: 0x0400307F RID: 12415
	[SerializeField]
	internal Vector3 positionOffset;

	// Token: 0x04003080 RID: 12416
	[SerializeField]
	internal Quaternion rotationOffset;

	// Token: 0x04003081 RID: 12417
	private Vector3 position;

	// Token: 0x04003082 RID: 12418
	private Quaternion rotation;
}
