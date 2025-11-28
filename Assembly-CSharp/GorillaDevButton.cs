using System;
using UnityEngine;

// Token: 0x0200090F RID: 2319
public class GorillaDevButton : GorillaPressableButton
{
	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x06003B37 RID: 15159 RVA: 0x00139039 File Offset: 0x00137239
	// (set) Token: 0x06003B38 RID: 15160 RVA: 0x00139041 File Offset: 0x00137241
	public bool on
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				this.isOn = value;
				this.UpdateColor();
			}
		}
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x00139059 File Offset: 0x00137259
	public new void OnEnable()
	{
		this.UpdateColor();
	}

	// Token: 0x04004B96 RID: 19350
	public DevButtonType Type;

	// Token: 0x04004B97 RID: 19351
	public LogType levelType;

	// Token: 0x04004B98 RID: 19352
	public DevConsoleInstance targetConsole;

	// Token: 0x04004B99 RID: 19353
	public int lineNumber;

	// Token: 0x04004B9A RID: 19354
	public bool repeatIfHeld;

	// Token: 0x04004B9B RID: 19355
	public float holdForSeconds;

	// Token: 0x04004B9C RID: 19356
	private Coroutine pressCoroutine;
}
