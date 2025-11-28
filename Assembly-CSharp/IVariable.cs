using System;

// Token: 0x02000814 RID: 2068
public interface IVariable<T> : IVariable
{
	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x0600366A RID: 13930 RVA: 0x00126C51 File Offset: 0x00124E51
	// (set) Token: 0x0600366B RID: 13931 RVA: 0x00126C59 File Offset: 0x00124E59
	T Value
	{
		get
		{
			return this.Get();
		}
		set
		{
			this.Set(value);
		}
	}

	// Token: 0x0600366C RID: 13932
	T Get();

	// Token: 0x0600366D RID: 13933
	void Set(T value);

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x0600366E RID: 13934 RVA: 0x00126C62 File Offset: 0x00124E62
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
