using System;

// Token: 0x02000C37 RID: 3127
internal class CircularBuffer<T>
{
	// Token: 0x1700072F RID: 1839
	// (get) Token: 0x06004CB6 RID: 19638 RVA: 0x0018E7E9 File Offset: 0x0018C9E9
	// (set) Token: 0x06004CB7 RID: 19639 RVA: 0x0018E7F1 File Offset: 0x0018C9F1
	public int Count { get; private set; }

	// Token: 0x17000730 RID: 1840
	// (get) Token: 0x06004CB8 RID: 19640 RVA: 0x0018E7FA File Offset: 0x0018C9FA
	// (set) Token: 0x06004CB9 RID: 19641 RVA: 0x0018E802 File Offset: 0x0018CA02
	public int Capacity { get; private set; }

	// Token: 0x06004CBA RID: 19642 RVA: 0x0018E80B File Offset: 0x0018CA0B
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x06004CBB RID: 19643 RVA: 0x0018E830 File Offset: 0x0018CA30
	public void Add(T value)
	{
		this.backingArray[this.nextWriteIdx] = value;
		this.lastWriteIdx = this.nextWriteIdx;
		this.nextWriteIdx = (this.nextWriteIdx + 1) % this.Capacity;
		if (this.Count < this.Capacity)
		{
			int count = this.Count;
			this.Count = count + 1;
		}
	}

	// Token: 0x06004CBC RID: 19644 RVA: 0x0018E88E File Offset: 0x0018CA8E
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x06004CBD RID: 19645 RVA: 0x0018E897 File Offset: 0x0018CA97
	public T Last()
	{
		return this.backingArray[this.lastWriteIdx];
	}

	// Token: 0x17000731 RID: 1841
	public T this[int logicalIdx]
	{
		get
		{
			if (logicalIdx < 0 || logicalIdx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("logicalIdx", logicalIdx, string.Format("Out of bounds index {0} into CircularBuffer with length {1}", logicalIdx, this.Count));
			}
			int num = (this.lastWriteIdx + this.Capacity - logicalIdx) % this.Capacity;
			return this.backingArray[num];
		}
	}

	// Token: 0x04005C7C RID: 23676
	private T[] backingArray;

	// Token: 0x04005C7F RID: 23679
	private int nextWriteIdx;

	// Token: 0x04005C80 RID: 23680
	private int lastWriteIdx;
}
