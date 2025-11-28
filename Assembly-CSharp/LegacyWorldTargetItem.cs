using System;
using Photon.Realtime;

// Token: 0x02000418 RID: 1048
public class LegacyWorldTargetItem
{
	// Token: 0x060019D9 RID: 6617 RVA: 0x00089F04 File Offset: 0x00088104
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x00089F1A File Offset: 0x0008811A
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x04002346 RID: 9030
	public Player owner;

	// Token: 0x04002347 RID: 9031
	public int itemIdx;
}
