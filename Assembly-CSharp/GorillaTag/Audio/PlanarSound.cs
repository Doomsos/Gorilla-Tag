using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x0200106A RID: 4202
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x0600697E RID: 27006 RVA: 0x0022515E File Offset: 0x0022335E
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x0600697F RID: 27007 RVA: 0x00225184 File Offset: 0x00223384
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.parent.InverseTransformPoint(this.cameraXform.position);
			localPosition.y = 0f;
			if (this.limitDistance && localPosition.sqrMagnitude > this.maxDistance * this.maxDistance)
			{
				localPosition = localPosition.normalized * this.maxDistance;
			}
			transform.localPosition = localPosition;
		}

		// Token: 0x040078CE RID: 30926
		private Transform cameraXform;

		// Token: 0x040078CF RID: 30927
		private bool hasCamera;

		// Token: 0x040078D0 RID: 30928
		[SerializeField]
		private bool limitDistance;

		// Token: 0x040078D1 RID: 30929
		[SerializeField]
		private float maxDistance = 1f;
	}
}
