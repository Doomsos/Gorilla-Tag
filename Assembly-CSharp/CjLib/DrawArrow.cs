using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001161 RID: 4449
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x0600702B RID: 28715 RVA: 0x0024786C File Offset: 0x00245A6C
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x0600702C RID: 28716 RVA: 0x002478D0 File Offset: 0x00245AD0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x0400807E RID: 32894
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x0400807F RID: 32895
		public float ConeRadius = 0.05f;

		// Token: 0x04008080 RID: 32896
		public float ConeHeight = 0.1f;

		// Token: 0x04008081 RID: 32897
		public float StemThickness = 0.05f;

		// Token: 0x04008082 RID: 32898
		public int NumSegments = 8;
	}
}
