using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001092 RID: 4242
	[ExecuteAlways]
	public class ScubaWatchWearable : MonoBehaviour
	{
		// Token: 0x06006A2D RID: 27181 RVA: 0x0022A39C File Offset: 0x0022859C
		protected void Update()
		{
			GTPlayer instance = GTPlayer.Instance;
			if (this.onLeftHand)
			{
				if (instance.LeftHandWaterVolume != null)
				{
					this.currentDepth = Mathf.Max(-instance.LeftHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastLeftHandPosition), 0f);
				}
				else
				{
					this.currentDepth = 0f;
				}
			}
			else if (instance.RightHandWaterVolume != null)
			{
				this.currentDepth = Mathf.Max(-instance.RightHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastRightHandPosition), 0f);
			}
			else
			{
				this.currentDepth = 0f;
			}
			float num = (this.currentDepth - this.depthRange.x) / (this.depthRange.y - this.depthRange.x);
			float num2 = Mathf.Lerp(this.dialRotationRange.x, this.dialRotationRange.y, num);
			this.dialNeedle.localRotation = this.initialDialRotation * Quaternion.AngleAxis(num2, this.dialRotationAxis);
		}

		// Token: 0x040079B0 RID: 31152
		public bool onLeftHand;

		// Token: 0x040079B1 RID: 31153
		[Tooltip("The transform that will be rotated to indicate the current depth.")]
		public Transform dialNeedle;

		// Token: 0x040079B2 RID: 31154
		[Tooltip("If your rotation is not zeroed out then click the Auto button to use the current rotation as 0.")]
		public Quaternion initialDialRotation;

		// Token: 0x040079B3 RID: 31155
		[Tooltip("The range of depth values that the dial will rotate between.")]
		public Vector2 depthRange = new Vector2(0f, 20f);

		// Token: 0x040079B4 RID: 31156
		[Tooltip("The range of rotation values that the dial will rotate between.")]
		public Vector2 dialRotationRange = new Vector2(0f, 360f);

		// Token: 0x040079B5 RID: 31157
		[Tooltip("The axis that the dial will rotate around.")]
		public Vector3 dialRotationAxis = Vector3.right;

		// Token: 0x040079B6 RID: 31158
		[Tooltip("The current depth of the player.")]
		[DebugOption]
		private float currentDepth;
	}
}
