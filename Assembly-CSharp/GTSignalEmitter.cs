using System;
using UnityEngine;

// Token: 0x020007F3 RID: 2035
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x0600357B RID: 13691 RVA: 0x00122483 File Offset: 0x00120683
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x001224A0 File Offset: 0x001206A0
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x001224B8 File Offset: 0x001206B8
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
