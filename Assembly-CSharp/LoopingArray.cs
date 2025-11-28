using System;
using GorillaTag;

// Token: 0x02000C57 RID: 3159
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x1700073A RID: 1850
	// (get) Token: 0x06004D47 RID: 19783 RVA: 0x001908E4 File Offset: 0x0018EAE4
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x1700073B RID: 1851
	// (get) Token: 0x06004D48 RID: 19784 RVA: 0x001908EC File Offset: 0x0018EAEC
	public int CurrentIndex
	{
		get
		{
			return this.m_currentIndex;
		}
	}

	// Token: 0x1700073C RID: 1852
	public T this[int index]
	{
		get
		{
			return this.m_array[index];
		}
		set
		{
			this.m_array[index] = value;
		}
	}

	// Token: 0x06004D4B RID: 19787 RVA: 0x00190911 File Offset: 0x0018EB11
	public LoopingArray() : this(0)
	{
	}

	// Token: 0x06004D4C RID: 19788 RVA: 0x0019091A File Offset: 0x0018EB1A
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x06004D4D RID: 19789 RVA: 0x0019093B File Offset: 0x0018EB3B
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x06004D4E RID: 19790 RVA: 0x0019096F File Offset: 0x0018EB6F
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x06004D4F RID: 19791 RVA: 0x001909A4 File Offset: 0x0018EBA4
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x06004D50 RID: 19792 RVA: 0x001909E0 File Offset: 0x0018EBE0
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x06004D51 RID: 19793 RVA: 0x00002789 File Offset: 0x00000989
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x04005CDA RID: 23770
	private int m_length;

	// Token: 0x04005CDB RID: 23771
	private int m_currentIndex;

	// Token: 0x04005CDC RID: 23772
	private T[] m_array;

	// Token: 0x02000C58 RID: 3160
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x06004D52 RID: 19794 RVA: 0x001909E8 File Offset: 0x0018EBE8
		private Pool(int amount) : base(amount)
		{
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x001909F1 File Offset: 0x0018EBF1
		public Pool(int size, int amount) : this(size, amount, amount)
		{
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x001909FC File Offset: 0x0018EBFC
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x00190A13 File Offset: 0x0018EC13
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x04005CDD RID: 23773
		private readonly int m_size;
	}
}
