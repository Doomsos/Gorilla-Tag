using System;
using UnityEngine;

// Token: 0x020008D9 RID: 2265
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x06003A12 RID: 14866 RVA: 0x00133467 File Offset: 0x00131667
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x06003A13 RID: 14867 RVA: 0x0013346F File Offset: 0x0013166F
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x06003A14 RID: 14868 RVA: 0x0013347E File Offset: 0x0013167E
	// (set) Token: 0x06003A15 RID: 14869 RVA: 0x00133486 File Offset: 0x00131686
	public float Min
	{
		get
		{
			return this._min;
		}
		set
		{
			this._min = value;
		}
	}

	// Token: 0x17000557 RID: 1367
	// (get) Token: 0x06003A16 RID: 14870 RVA: 0x0013348F File Offset: 0x0013168F
	// (set) Token: 0x06003A17 RID: 14871 RVA: 0x00133497 File Offset: 0x00131697
	public float Max
	{
		get
		{
			return this._max;
		}
		set
		{
			this._max = value;
		}
	}

	// Token: 0x17000558 RID: 1368
	// (get) Token: 0x06003A18 RID: 14872 RVA: 0x001334A0 File Offset: 0x001316A0
	// (set) Token: 0x06003A19 RID: 14873 RVA: 0x001334D5 File Offset: 0x001316D5
	public float normalized
	{
		get
		{
			if (!this.Range.Approx0(1E-06f))
			{
				return (this._value - this._min) / (this._max - this.Min);
			}
			return 0f;
		}
		set
		{
			this._value = this._min + Mathf.Clamp01(value) * (this._max - this._min);
		}
	}

	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x06003A1A RID: 14874 RVA: 0x001334F8 File Offset: 0x001316F8
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x06003A1B RID: 14875 RVA: 0x00133520 File Offset: 0x00131720
	public float Get()
	{
		return this._value;
	}

	// Token: 0x06003A1C RID: 14876 RVA: 0x00133528 File Offset: 0x00131728
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x04004955 RID: 18773
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04004956 RID: 18774
	[SerializeField]
	private float _min;

	// Token: 0x04004957 RID: 18775
	[SerializeField]
	private float _max = 1f;

	// Token: 0x04004958 RID: 18776
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
