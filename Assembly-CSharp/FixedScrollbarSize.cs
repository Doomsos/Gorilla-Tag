using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200008E RID: 142
public class FixedScrollbarSize : MonoBehaviour
{
	// Token: 0x060003A5 RID: 933 RVA: 0x000167E4 File Offset: 0x000149E4
	private void OnEnable()
	{
		this.EnforceScrollbarSize();
		CanvasUpdateRegistry.instance.Equals(null);
		Canvas.willRenderCanvases += new Canvas.WillRenderCanvases(this.EnforceScrollbarSize);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00016809 File Offset: 0x00014A09
	private void OnDisable()
	{
		Canvas.willRenderCanvases -= new Canvas.WillRenderCanvases(this.EnforceScrollbarSize);
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0001681C File Offset: 0x00014A1C
	private void EnforceScrollbarSize()
	{
		if (this.ScrollRect.horizontalScrollbar && this.ScrollRect.horizontalScrollbar.size != this.HorizontalBarSize)
		{
			this.ScrollRect.horizontalScrollbar.size = this.HorizontalBarSize;
		}
		if (this.ScrollRect.verticalScrollbar && this.ScrollRect.verticalScrollbar.size != this.VerticalBarSize)
		{
			this.ScrollRect.verticalScrollbar.size = this.VerticalBarSize;
		}
	}

	// Token: 0x0400041E RID: 1054
	public ScrollRect ScrollRect;

	// Token: 0x0400041F RID: 1055
	public float HorizontalBarSize = 0.2f;

	// Token: 0x04000420 RID: 1056
	public float VerticalBarSize = 0.2f;
}
