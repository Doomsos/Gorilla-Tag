using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001163 RID: 4451
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x06007031 RID: 28721 RVA: 0x002479C0 File Offset: 0x00245BC0
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007032 RID: 28722 RVA: 0x002479EC File Offset: 0x00245BEC
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion quaternion = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * quaternion * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04008088 RID: 32904
		public float Radius = 1f;

		// Token: 0x04008089 RID: 32905
		public int NumSegments = 64;

		// Token: 0x0400808A RID: 32906
		public float StartAngle;

		// Token: 0x0400808B RID: 32907
		public float ArcAngle = 60f;
	}
}
