using System;
using UnityEngine;

// Token: 0x02000BCE RID: 3022
public abstract class RandomContainer<T> : ScriptableObject
{
	// Token: 0x170006EF RID: 1775
	// (get) Token: 0x06004AE1 RID: 19169 RVA: 0x00187E6C File Offset: 0x0018606C
	public T lastItem
	{
		get
		{
			return this._lastItem;
		}
	}

	// Token: 0x170006F0 RID: 1776
	// (get) Token: 0x06004AE2 RID: 19170 RVA: 0x00187E74 File Offset: 0x00186074
	public int lastItemIndex
	{
		get
		{
			return this._lastItemIndex;
		}
	}

	// Token: 0x06004AE3 RID: 19171 RVA: 0x00187E7C File Offset: 0x0018607C
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

	// Token: 0x06004AE4 RID: 19172 RVA: 0x00187EDC File Offset: 0x001860DC
	public void Reset()
	{
		this.ResetRandom(default(int?));
		this._lastItem = default(T);
		this._lastItemIndex = -1;
	}

	// Token: 0x06004AE5 RID: 19173 RVA: 0x00187F0B File Offset: 0x0018610B
	private void Awake()
	{
		this.Reset();
	}

	// Token: 0x06004AE6 RID: 19174 RVA: 0x00187F13 File Offset: 0x00186113
	public virtual T GetItem(int index)
	{
		return this.items[index];
	}

	// Token: 0x06004AE7 RID: 19175 RVA: 0x00187F24 File Offset: 0x00186124
	public virtual T NextItem()
	{
		this._lastItemIndex = (this.distinct ? this._rnd.NextIntWithExclusion(0, this.items.Length, this._lastItemIndex) : this._rnd.NextInt(0, this.items.Length));
		T t = this.items[this._lastItemIndex];
		this._lastItem = t;
		return t;
	}

	// Token: 0x04005B12 RID: 23314
	public T[] items = new T[0];

	// Token: 0x04005B13 RID: 23315
	public int seed;

	// Token: 0x04005B14 RID: 23316
	public bool staticSeed;

	// Token: 0x04005B15 RID: 23317
	public bool distinct = true;

	// Token: 0x04005B16 RID: 23318
	[Space]
	[NonSerialized]
	private int _seed;

	// Token: 0x04005B17 RID: 23319
	[NonSerialized]
	private T _lastItem;

	// Token: 0x04005B18 RID: 23320
	[NonSerialized]
	private int _lastItemIndex = -1;

	// Token: 0x04005B19 RID: 23321
	[NonSerialized]
	private SRand _rnd;
}
