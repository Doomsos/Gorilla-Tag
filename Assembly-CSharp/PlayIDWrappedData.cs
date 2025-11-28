using System;

// Token: 0x02000C69 RID: 3177
internal struct PlayIDWrappedData<T>
{
	// Token: 0x06004DB2 RID: 19890 RVA: 0x00192481 File Offset: 0x00190681
	public PlayIDWrappedData(T initialValue)
	{
		this.currentValue = initialValue;
		this.initialValue = initialValue;
		this.id = EnterPlayID.GetCurrent();
	}

	// Token: 0x17000740 RID: 1856
	// (get) Token: 0x06004DB3 RID: 19891 RVA: 0x0019249C File Offset: 0x0019069C
	// (set) Token: 0x06004DB4 RID: 19892 RVA: 0x001924B8 File Offset: 0x001906B8
	public T Value
	{
		get
		{
			if (!this.id.IsCurrent)
			{
				return this.initialValue;
			}
			return this.currentValue;
		}
		set
		{
			this.currentValue = value;
			this.id = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x04005CF4 RID: 23796
	private T currentValue;

	// Token: 0x04005CF5 RID: 23797
	private T initialValue;

	// Token: 0x04005CF6 RID: 23798
	private EnterPlayID id;
}
