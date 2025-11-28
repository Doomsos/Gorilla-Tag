using System;
using UnityEngine;

// Token: 0x020009DD RID: 2525
public static class Id128Ext
{
	// Token: 0x0600406C RID: 16492 RVA: 0x001593E0 File Offset: 0x001575E0
	public static Id128 ToId128(this Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x0600406D RID: 16493 RVA: 0x001593D8 File Offset: 0x001575D8
	public static Id128 ToId128(this Guid g)
	{
		return new Id128(g);
	}
}
