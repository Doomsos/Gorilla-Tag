using System;
using UnityEngine;

// Token: 0x020008DA RID: 2266
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x06003A1E RID: 14878 RVA: 0x0013357F File Offset: 0x0013177F
	// (set) Token: 0x06003A1F RID: 14879 RVA: 0x00133587 File Offset: 0x00131787
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = (value as Object);
		}
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x0013359C File Offset: 0x0013179C
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		return @object != null && @object != null;
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x001335C4 File Offset: 0x001317C4
	public static implicit operator T(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x0013360C File Offset: 0x0013180C
	public static implicit operator Object(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04004959 RID: 18777
	[SerializeField]
	private Object _target;
}
