using System;
using UnityEngine;

// Token: 0x0200038A RID: 906
[RequireComponent(typeof(LineRenderer))]
public class LineRenderVelocityMapper : MonoBehaviour
{
	// Token: 0x060015A3 RID: 5539 RVA: 0x00079CC3 File Offset: 0x00077EC3
	private void Awake()
	{
		this._lr = base.GetComponent<LineRenderer>();
		this._lr.useWorldSpace = true;
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x00079CE0 File Offset: 0x00077EE0
	private void LateUpdate()
	{
		if (this.velocityEstimator == null)
		{
			return;
		}
		this._lr.SetPosition(0, this.velocityEstimator.transform.position);
		if (this.velocityEstimator.linearVelocity.sqrMagnitude > 0.1f)
		{
			this._lr.SetPosition(1, this.velocityEstimator.transform.position + this.velocityEstimator.linearVelocity.normalized * 0.2f);
			return;
		}
		this._lr.SetPosition(1, this.velocityEstimator.transform.position);
	}

	// Token: 0x0400200E RID: 8206
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400200F RID: 8207
	private LineRenderer _lr;
}
