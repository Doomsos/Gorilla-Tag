using System;
using UnityEngine;

// Token: 0x02000389 RID: 905
public class LineRendererDraw : MonoBehaviour
{
	// Token: 0x0600159F RID: 5535 RVA: 0x00079C62 File Offset: 0x00077E62
	public void SetUpLine(Transform[] points)
	{
		this.lr.positionCount = points.Length;
		this.points = points;
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x00079C7C File Offset: 0x00077E7C
	private void LateUpdate()
	{
		for (int i = 0; i < this.points.Length; i++)
		{
			this.lr.SetPosition(i, this.points[i].position);
		}
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x00079CB5 File Offset: 0x00077EB5
	public void Enable(bool enable)
	{
		this.lr.enabled = enable;
	}

	// Token: 0x0400200C RID: 8204
	public LineRenderer lr;

	// Token: 0x0400200D RID: 8205
	public Transform[] points;
}
