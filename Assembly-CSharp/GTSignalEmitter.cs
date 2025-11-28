using System;
using UnityEngine;

// Token: 0x020007F3 RID: 2035
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x0600357B RID: 13691 RVA: 0x00122463 File Offset: 0x00120663
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x00122480 File Offset: 0x00120680
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x00122498 File Offset: 0x00120698
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x040044AC RID: 17580
	[Space]
	public GTSignalID signal;

	// Token: 0x040044AD RID: 17581
	public GTSignal.EmitMode emitMode;
}
