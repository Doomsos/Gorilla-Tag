using System;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class CloudUmbrellaCloud : MonoBehaviour
{
	// Token: 0x06000B2A RID: 2858 RVA: 0x0003CBF8 File Offset: 0x0003ADF8
	protected void Awake()
	{
		this.umbrellaXform = this.umbrella.transform;
		this.cloudScaleXform = this.cloudRenderer.transform;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x0003CC1C File Offset: 0x0003AE1C
	protected void LateUpdate()
	{
		float num = Vector3.Dot(this.umbrellaXform.up, Vector3.up);
		float num2 = Mathf.Clamp01(this.scaleCurve.Evaluate(num));
		this.rendererOn = ((num2 > 0.09f && num2 < 0.1f) ? this.rendererOn : (num2 > 0.1f));
		this.cloudRenderer.enabled = this.rendererOn;
		this.cloudScaleXform.localScale = new Vector3(num2, num2, num2);
		this.cloudRotateXform.up = Vector3.up;
	}

	// Token: 0x04000D8C RID: 3468
	public UmbrellaItem umbrella;

	// Token: 0x04000D8D RID: 3469
	public Transform cloudRotateXform;

	// Token: 0x04000D8E RID: 3470
	public Renderer cloudRenderer;

	// Token: 0x04000D8F RID: 3471
	public AnimationCurve scaleCurve;

	// Token: 0x04000D90 RID: 3472
	private const float kHideAtScale = 0.1f;

	// Token: 0x04000D91 RID: 3473
	private const float kHideAtScaleTolerance = 0.01f;

	// Token: 0x04000D92 RID: 3474
	private bool rendererOn;

	// Token: 0x04000D93 RID: 3475
	private Transform umbrellaXform;

	// Token: 0x04000D94 RID: 3476
	private Transform cloudScaleXform;
}
