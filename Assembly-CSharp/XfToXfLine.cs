using System;
using UnityEngine;

// Token: 0x0200083A RID: 2106
public class XfToXfLine : MonoBehaviour
{
	// Token: 0x06003763 RID: 14179 RVA: 0x0012A703 File Offset: 0x00128903
	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x0012A711 File Offset: 0x00128911
	private void Update()
	{
		this.lineRenderer.SetPosition(0, this.pt0.transform.position);
		this.lineRenderer.SetPosition(1, this.pt1.transform.position);
	}

	// Token: 0x040046C4 RID: 18116
	public Transform pt0;

	// Token: 0x040046C5 RID: 18117
	public Transform pt1;

	// Token: 0x040046C6 RID: 18118
	private LineRenderer lineRenderer;
}
