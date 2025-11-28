using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001162 RID: 4450
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x0600702E RID: 28718 RVA: 0x00247944 File Offset: 0x00245B44
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x0600702F RID: 28719
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04008083 RID: 32899
		public Color WireframeColor = Color.white;

		// Token: 0x04008084 RID: 32900
		public Color ShadededColor = Color.gray;

		// Token: 0x04008085 RID: 32901
		public bool Wireframe;

		// Token: 0x04008086 RID: 32902
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04008087 RID: 32903
		public bool DepthTest = true;
	}
}
