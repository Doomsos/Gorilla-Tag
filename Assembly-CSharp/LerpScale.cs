using System;
using UnityEngine;

// Token: 0x020009E2 RID: 2530
public class LerpScale : LerpComponent
{
	// Token: 0x06004086 RID: 16518 RVA: 0x00159580 File Offset: 0x00157780
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04005199 RID: 20889
	[Space]
	public Transform target;

	// Token: 0x0400519A RID: 20890
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x0400519B RID: 20891
	public Vector3 end = Vector3.one;

	// Token: 0x0400519C RID: 20892
	public Vector3 current;

	// Token: 0x0400519D RID: 20893
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
