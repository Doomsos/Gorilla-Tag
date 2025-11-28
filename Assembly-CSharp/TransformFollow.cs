using System;
using UnityEngine;

// Token: 0x020008FA RID: 2298
public class TransformFollow : MonoBehaviour
{
	// Token: 0x06003AC1 RID: 15041 RVA: 0x0013641F File Offset: 0x0013461F
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x00136434 File Offset: 0x00134634
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		Vector3 vector;
		Quaternion quaternion;
		this.transformToFollow.GetPositionAndRotation(ref vector, ref quaternion);
		base.transform.SetPositionAndRotation(vector + quaternion * this.offset, quaternion);
	}

	// Token: 0x04004A27 RID: 18983
	public Transform transformToFollow;

	// Token: 0x04004A28 RID: 18984
	public Vector3 offset;

	// Token: 0x04004A29 RID: 18985
	public Vector3 prevPos;
}
