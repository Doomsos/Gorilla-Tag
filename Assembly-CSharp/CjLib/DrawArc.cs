using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001160 RID: 4448
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x06007028 RID: 28712 RVA: 0x0024778F File Offset: 0x0024598F
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007029 RID: 28713 RVA: 0x002477C8 File Offset: 0x002459C8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x0400807A RID: 32890
		public float Radius = 1f;

		// Token: 0x0400807B RID: 32891
		public int NumSegments = 64;

		// Token: 0x0400807C RID: 32892
		public float StartAngle;

		// Token: 0x0400807D RID: 32893
		public float ArcAngle = 60f;
	}
}
