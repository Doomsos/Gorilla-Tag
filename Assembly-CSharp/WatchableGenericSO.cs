using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032E RID: 814
public class WatchableGenericSO<T> : ScriptableObject
{
	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060013A6 RID: 5030 RVA: 0x0007243F File Offset: 0x0007063F
	// (set) Token: 0x060013A7 RID: 5031 RVA: 0x00072447 File Offset: 0x00070647
	private T _value { get; set; }

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060013A8 RID: 5032 RVA: 0x00072450 File Offset: 0x00070650
	// (set) Token: 0x060013A9 RID: 5033 RVA: 0x00072460 File Offset: 0x00070660
	public T Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action.Invoke(value);
			}
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x000724C0 File Offset: 0x000706C0
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x000724F4 File Offset: 0x000706F4
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action.Invoke(value);
			}
		}
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x00072564 File Offset: 0x00070764
	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x04001E13 RID: 7699
	public T InitialValue;

	// Token: 0x04001E15 RID: 7701
	private EnterPlayID enterPlayID;

	// Token: 0x04001E16 RID: 7702
	private List<Action<T>> callbacks;
}
