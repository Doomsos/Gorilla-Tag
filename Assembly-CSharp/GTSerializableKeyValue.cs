using System;

// Token: 0x020002FD RID: 765
[Serializable]
public struct GTSerializableKeyValue<T1, T2>
{
	// Token: 0x060012BE RID: 4798 RVA: 0x00061E59 File Offset: 0x00060059
	public GTSerializableKeyValue(T1 k, T2 v)
	{
		this.k = k;
		this.v = v;
	}

	// Token: 0x04001746 RID: 5958
	public T1 k;

	// Token: 0x04001747 RID: 5959
	public T2 v;
}
