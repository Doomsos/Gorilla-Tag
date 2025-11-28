using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001164 RID: 4452
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x06007034 RID: 28724 RVA: 0x00247AB0 File Offset: 0x00245CB0
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007035 RID: 28725 RVA: 0x00247ADA File Offset: 0x00245CDA
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x0400808C RID: 32908
		public float Radius = 1f;

		// Token: 0x0400808D RID: 32909
		public int NumSegments = 64;
	}
}
