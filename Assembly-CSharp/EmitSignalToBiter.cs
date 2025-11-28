using System;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class EmitSignalToBiter : GTSignalEmitter
{
	// Token: 0x060025BA RID: 9658 RVA: 0x000C9948 File Offset: 0x000C7B48
	public override void Emit()
	{
		if (this.onEdibleState == EmitSignalToBiter.EdibleState.None)
		{
			return;
		}
		if (!this.targetEdible)
		{
			return;
		}
		if (this.targetEdible.lastBiterActorID == -1)
		{
			return;
		}
		TransferrableObject.ItemStates itemState = this.targetEdible.itemState;
		if (itemState - TransferrableObject.ItemStates.State0 <= 1 || itemState == TransferrableObject.ItemStates.State2 || itemState == TransferrableObject.ItemStates.State3)
		{
			int num = (int)itemState;
			if ((this.onEdibleState & (EmitSignalToBiter.EdibleState)num) == (EmitSignalToBiter.EdibleState)num)
			{
				GTSignal.Emit(this.targetEdible.lastBiterActorID, this.signal, Array.Empty<object>());
			}
		}
	}

	// Token: 0x060025BB RID: 9659 RVA: 0x00002789 File Offset: 0x00000989
	public override void Emit(int targetActor)
	{
	}

	// Token: 0x060025BC RID: 9660 RVA: 0x00002789 File Offset: 0x00000989
	public override void Emit(params object[] data)
	{
	}

	// Token: 0x04003164 RID: 12644
	[Space]
	public EdibleHoldable targetEdible;

	// Token: 0x04003165 RID: 12645
	[Space]
	[SerializeField]
	private EmitSignalToBiter.EdibleState onEdibleState;

	// Token: 0x020005D9 RID: 1497
	[Flags]
	private enum EdibleState
	{
		// Token: 0x04003167 RID: 12647
		None = 0,
		// Token: 0x04003168 RID: 12648
		State0 = 1,
		// Token: 0x04003169 RID: 12649
		State1 = 2,
		// Token: 0x0400316A RID: 12650
		State2 = 4,
		// Token: 0x0400316B RID: 12651
		State3 = 8
	}
}
