using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001166 RID: 4454
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x0600703A RID: 28730 RVA: 0x00247B83 File Offset: 0x00245D83
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.LatSegments = Mathf.Max(0, this.LatSegments);
		}

		// Token: 0x0600703B RID: 28731 RVA: 0x00247BB0 File Offset: 0x00245DB0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawSphere(base.transform.position, base.transform.rotation, this.Radius * base.transform.lossyScale.x, this.LatSegments, this.LongSegments, color, depthTest, style);
		}

		// Token: 0x0400808F RID: 32911
		public float Radius = 1f;

		// Token: 0x04008090 RID: 32912
		public int LatSegments = 12;

		// Token: 0x04008091 RID: 32913
		public int LongSegments = 12;
	}
}
