using System;
using Photon.Realtime;

// Token: 0x02000418 RID: 1048
public class LegacyWorldTargetItem
{
	// Token: 0x060019D9 RID: 6617 RVA: 0x00089EE4 File Offset: 0x000880E4
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x060019DA RID: 6618 RVA: 0x00089EFA File Offset: 0x000880FA
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
