using System;
using UnityEngine;

// Token: 0x020009ED RID: 2541
[Serializable]
public class OptionalRef<T> where T : Object
{
	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x060040C2 RID: 16578 RVA: 0x0015A52D File Offset: 0x0015872D
	// (set) Token: 0x060040C3 RID: 16579 RVA: 0x0015A535 File Offset: 0x00158735
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	// Token: 0x17000600 RID: 1536
	// (get) Token: 0x060040C4 RID: 16580 RVA: 0x0015A540 File Offset: 0x00158740
	// (set) Token: 0x060040C5 RID: 16581 RVA: 0x0015A568 File Offset: 0x00158768
	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	// Token: 0x060040C6 RID: 16582 RVA: 0x0015A594 File Offset: 0x00158794
	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		Object @object = r._target;
		return @object != null && @object;
	}

	// Token: 0x060040C7 RID: 16583 RVA: 0x0015A5C8 File Offset: 0x001587C8
	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x060040C8 RID: 16584 RVA: 0x0015A62C File Offset: 0x0015882C
	public static implicit operator Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04005200 RID: 20992
	[SerializeField]
	private bool _enabled;

	// Token: 0x04005201 RID: 20993
	[SerializeField]
	private T _target;
}
