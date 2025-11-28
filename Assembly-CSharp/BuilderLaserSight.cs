using System;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class BuilderLaserSight : MonoBehaviour
{
	// Token: 0x0600235D RID: 9053 RVA: 0x000B9811 File Offset: 0x000B7A11
	public void Awake()
	{
		if (this.lineRenderer == null)
		{
			this.lineRenderer = base.GetComponentInChildren<LineRenderer>();
		}
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = false;
		}
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000B9847 File Offset: 0x000B7A47
	public void SetPoints(Vector3 start, Vector3 end)
	{
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPosition(0, start);
		this.lineRenderer.SetPosition(1, end);
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000B986F File Offset: 0x000B7A6F
	public void Show(bool show)
	{
		if (this.lineRenderer != null)
		{
			this.lineRenderer.enabled = show;
		}
	}

	// Token: 0x04002E39 RID: 11833
	public LineRenderer lineRenderer;
}
