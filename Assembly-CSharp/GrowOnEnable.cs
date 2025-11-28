using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class GrowOnEnable : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060006C9 RID: 1737 RVA: 0x00025D9B File Offset: 0x00023F9B
	private void Awake()
	{
		this._targetScale = base.transform.localScale;
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x00025DAE File Offset: 0x00023FAE
	private void OnEnable()
	{
		this._lerpVal = 0f;
		this._curve = AnimationCurves.GetCurveForEase(this.easeType);
		this.UpdateScale();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00025DD8 File Offset: 0x00023FD8
	private void OnDisable()
	{
		base.transform.localScale = this._targetScale;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000073 RID: 115
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x00025DF1 File Offset: 0x00023FF1
	// (set) Token: 0x060006CD RID: 1741 RVA: 0x00025DF9 File Offset: 0x00023FF9
	public bool TickRunning { get; set; }

	// Token: 0x060006CE RID: 1742 RVA: 0x00025E02 File Offset: 0x00024002
	public void Tick()
	{
		this._lerpVal = Mathf.Clamp01(this._lerpVal + Time.deltaTime / this.growDuration);
		this.UpdateScale();
		if (this._lerpVal >= 1f)
		{
			TickSystem<object>.RemoveTickCallback(this);
		}
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x00025E3B File Offset: 0x0002403B
	private void UpdateScale()
	{
		base.transform.localScale = this._targetScale * this._curve.Evaluate(this._lerpVal);
	}

	// Token: 0x040008AC RID: 2220
	[SerializeField]
	private float growDuration = 1f;

	// Token: 0x040008AD RID: 2221
	[SerializeField]
	private AnimationCurves.EaseType easeType = AnimationCurves.EaseType.EaseOutBack;

	// Token: 0x040008AE RID: 2222
	private AnimationCurve _curve;

	// Token: 0x040008AF RID: 2223
	private Vector3 _targetScale;

	// Token: 0x040008B0 RID: 2224
	private float _lerpVal;
}
