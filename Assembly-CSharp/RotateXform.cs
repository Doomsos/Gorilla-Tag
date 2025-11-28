using System;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class RotateXform : MonoBehaviour
{
	// Token: 0x06000A06 RID: 2566 RVA: 0x00036574 File Offset: 0x00034774
	private void Update()
	{
		if (!this.xform)
		{
			return;
		}
		Vector3 vector = (this.mode == RotateXform.Mode.Local) ? this.xform.localEulerAngles : this.xform.eulerAngles;
		float num = Time.deltaTime * this.speedFactor;
		vector.x += this.speed.x * num;
		vector.y += this.speed.y * num;
		vector.z += this.speed.z * num;
		if (this.mode == RotateXform.Mode.Local)
		{
			this.xform.localEulerAngles = vector;
			return;
		}
		this.xform.eulerAngles = vector;
	}

	// Token: 0x04000C51 RID: 3153
	public Transform xform;

	// Token: 0x04000C52 RID: 3154
	public Vector3 speed = Vector3.zero;

	// Token: 0x04000C53 RID: 3155
	public RotateXform.Mode mode;

	// Token: 0x04000C54 RID: 3156
	public float speedFactor = 0.0625f;

	// Token: 0x02000176 RID: 374
	public enum Mode
	{
		// Token: 0x04000C56 RID: 3158
		Local,
		// Token: 0x04000C57 RID: 3159
		World
	}
}
