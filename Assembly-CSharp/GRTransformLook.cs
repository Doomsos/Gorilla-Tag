using System;
using UnityEngine;

// Token: 0x0200073E RID: 1854
public class GRTransformLook : MonoBehaviour
{
	// Token: 0x06002FE1 RID: 12257 RVA: 0x00105EB0 File Offset: 0x001040B0
	private void Awake()
	{
		if (this.followPlayer)
		{
			this.lookTarget = Camera.main.transform;
		}
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x00105ECA File Offset: 0x001040CA
	private void LateUpdate()
	{
		base.transform.LookAt(this.lookTarget);
		base.transform.rotation *= Quaternion.Euler(this.offsetRotation);
	}

	// Token: 0x04003ED1 RID: 16081
	public bool followPlayer;

	// Token: 0x04003ED2 RID: 16082
	public Transform lookTarget;

	// Token: 0x04003ED3 RID: 16083
	public Vector3 offsetRotation;
}
