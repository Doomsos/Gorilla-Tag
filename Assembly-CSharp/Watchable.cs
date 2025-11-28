using System;
using System.Collections.Generic;

// Token: 0x0200032D RID: 813
public class Watchable<T>
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060013A0 RID: 5024 RVA: 0x00072330 File Offset: 0x00070530
	// (set) Token: 0x060013A1 RID: 5025 RVA: 0x00072338 File Offset: 0x00070538
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			T value2 = this._value;
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action.Invoke(value);
			}
		}
	}

	// Token: 0x060013A2 RID: 5026 RVA: 0x00072398 File Offset: 0x00070598
	public Watchable()
	{
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x000723AB File Offset: 0x000705AB
	public Watchable(T initial)
	{
		this._value = initial;
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x000723C8 File Offset: 0x000705C8
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			foreach (Action<T> action in this.callbacks)
			{
				action.Invoke(this._value);
			}
		}
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x00072430 File Offset: 0x00070630
	public void RemoveCallback(Action<T> callback)
	{
		this.callbacks.Remove(callback);
	}

	// Token: 0x04001E11 RID: 7697
	private T _value;

	// Token: 0x04001E12 RID: 7698
	private List<Action<T>> callbacks = new List<Action<T>>();
}
