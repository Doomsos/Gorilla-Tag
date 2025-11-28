using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200032F RID: 815
[CreateAssetMenu(fileName = "WatchableStringSO", menuName = "ScriptableObjects/WatchableStringSO")]
public class WatchableStringSO : ScriptableObject
{
	// Token: 0x170001DD RID: 477
	// (get) Token: 0x060013AE RID: 5038 RVA: 0x00072579 File Offset: 0x00070779
	// (set) Token: 0x060013AF RID: 5039 RVA: 0x00072581 File Offset: 0x00070781
	private string _value { get; set; }

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060013B0 RID: 5040 RVA: 0x0007258A File Offset: 0x0007078A
	// (set) Token: 0x060013B1 RID: 5041 RVA: 0x00072598 File Offset: 0x00070798
	public string Value
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
			foreach (Action<string> action in this.callbacks)
			{
				action.Invoke(value);
			}
		}
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x000725F8 File Offset: 0x000707F8
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<string>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0007262C File Offset: 0x0007082C
	public void AddCallback(Action<string> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			string value = this._value;
			foreach (Action<string> action in this.callbacks)
			{
				action.Invoke(value);
			}
		}
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0007269C File Offset: 0x0007089C
	public void RemoveCallback(Action<string> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x000726B1 File Offset: 0x000708B1
	public override string ToString()
	{
		return this.Value;
	}

	// Token: 0x04001E17 RID: 7703
	[TextArea]
	public string InitialValue;

	// Token: 0x04001E19 RID: 7705
	private EnterPlayID enterPlayID;

	// Token: 0x04001E1A RID: 7706
	private List<Action<string>> callbacks;
}
