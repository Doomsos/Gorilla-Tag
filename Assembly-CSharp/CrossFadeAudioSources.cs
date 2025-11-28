using System;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class CrossFadeAudioSources : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x0600259E RID: 9630 RVA: 0x000C9425 File Offset: 0x000C7625
	public void Play()
	{
		if (this.source1)
		{
			this.source1.Play();
		}
		if (this.source2)
		{
			this.source2.Play();
		}
	}

	// Token: 0x0600259F RID: 9631 RVA: 0x000C9457 File Offset: 0x000C7657
	public void Stop()
	{
		if (this.source1)
		{
			this.source1.Stop();
		}
		if (this.source2)
		{
			this.source2.Stop();
		}
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000C948C File Offset: 0x000C768C
	private void Update()
	{
		if (!this.source1 || !this.source2)
		{
			return;
		}
		float num = this._curve.Evaluate(this._lerp);
		float num2;
		if (this.tween)
		{
			num2 = MathUtils.Xlerp(this._lastT, num, Time.deltaTime, this.tweenSpeed);
		}
		else
		{
			num2 = (this.lerpByClipLength ? this._curve.Evaluate((float)this.source1.timeSamples / (float)this.source1.clip.samples) : num);
		}
		this._lastT = num2;
		this.source2.volume = num2;
		this.source1.volume = 1f - num2;
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000C9542 File Offset: 0x000C7742
	public float Get()
	{
		return this._lerp;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000C954A File Offset: 0x000C774A
	public void Set(float f)
	{
		this._lerp = Mathf.Clamp01(f);
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060025A3 RID: 9635 RVA: 0x000C9558 File Offset: 0x000C7758
	// (set) Token: 0x060025A4 RID: 9636 RVA: 0x00002789 File Offset: 0x00000989
	public float Min
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060025A5 RID: 9637 RVA: 0x000C955F File Offset: 0x000C775F
	// (set) Token: 0x060025A6 RID: 9638 RVA: 0x00002789 File Offset: 0x00000989
	public float Max
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060025A7 RID: 9639 RVA: 0x000C955F File Offset: 0x000C775F
	public float Range
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060025A8 RID: 9640 RVA: 0x000C9566 File Offset: 0x000C7766
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x0400313C RID: 12604
	[SerializeField]
	private float _lerp;

	// Token: 0x0400313D RID: 12605
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400313E RID: 12606
	[Space]
	[SerializeField]
	private AudioSource source1;

	// Token: 0x0400313F RID: 12607
	[SerializeField]
	private AudioSource source2;

	// Token: 0x04003140 RID: 12608
	[Space]
	public bool lerpByClipLength;

	// Token: 0x04003141 RID: 12609
	public bool tween;

	// Token: 0x04003142 RID: 12610
	public float tweenSpeed = 16f;

	// Token: 0x04003143 RID: 12611
	private float _lastT;
}
