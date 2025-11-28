using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001165 RID: 4453
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x06007037 RID: 28727 RVA: 0x00247B2B File Offset: 0x00245D2B
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06007038 RID: 28728 RVA: 0x00247B3B File Offset: 0x00245D3B
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x0400808E RID: 32910
		public Vector3 LocalEndVector = Vector3.right;
	}
}
