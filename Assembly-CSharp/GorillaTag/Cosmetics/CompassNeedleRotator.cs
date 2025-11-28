using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001093 RID: 4243
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x06006A2F RID: 27183 RVA: 0x0022A4D2 File Offset: 0x002286D2
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x06006A30 RID: 27184 RVA: 0x0022A4F0 File Offset: 0x002286F0
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, num, 0);
		}

		// Token: 0x040079B7 RID: 31159
		private const float smoothTime = 0.005f;

		// Token: 0x040079B8 RID: 31160
		private float currentVelocity;
	}
}
