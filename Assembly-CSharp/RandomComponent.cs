using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000BCD RID: 3021
public abstract class RandomComponent<T> : MonoBehaviour
{
	// Token: 0x170006ED RID: 1773
	// (get) Token: 0x06004AD8 RID: 19160 RVA: 0x00187D11 File Offset: 0x00185F11
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x170006EE RID: 1774
	// (get) Token: 0x06004AD9 RID: 19161 RVA: 0x00187D19 File Offset: 0x00185F19
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x06004ADA RID: 19162 RVA: 0x00187D24 File Offset: 0x00185F24
	public void ResetRandom(int? seedValue = null)
	{
		if (!this.staticSeed)
		{
			this._seed = (seedValue ?? StaticHash.Compute(DateTime.UtcNow.Ticks));
		}
		else
		{
			this._seed = this.seed;
		}
		this._rnd = new SRand(this._seed);
	}

	// Token: 0x06004ADB RID: 19163 RVA: 0x00187D84 File Offset: 0x00185F84
	public void Reset()
	{
		this.ResetRandom(default(int?));
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x06004ADC RID: 19164 RVA: 0x00187DB3 File Offset: 0x00185FB3
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06004ADD RID: 19165 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnNextItem(T item)
	{
	}

	// Token: 0x06004ADE RID: 19166 RVA: 0x00187DBB File Offset: 0x00185FBB
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x06004ADF RID: 19167 RVA: 0x00187DCC File Offset: 0x00185FCC
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		this.OnNextItem(t);
		UnityEvent<T> unityEvent = this.onNextItem;
		if (unityEvent != null)
		{
			unityEvent.Invoke(t);
		}
		return t;
	}

	// Token: 0x04005B09 RID: 23305
	public T[] items = new T[0];

	// Token: 0x04005B0A RID: 23306
	public int seed;

	// Token: 0x04005B0B RID: 23307
	public bool staticSeed;

	// Token: 0x04005B0C RID: 23308
	public bool distinct = true;

	// Token: 0x04005B0D RID: 23309
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04005B0E RID: 23310
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04005B0F RID: 23311
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04005B10 RID: 23312
	[NonSerialized]
	private SRand _rnd;

	// Token: 0x04005B11 RID: 23313
	public UnityEvent<T> onNextItem;
}
