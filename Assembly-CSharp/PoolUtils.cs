using System;
using UnityEngine;

// Token: 0x02000C5E RID: 3166
public static class PoolUtils
{
	// Token: 0x06004D83 RID: 19843 RVA: 0x00191DCC File Offset: 0x0018FFCC
	public static int GameObjHashCode(GameObject obj)
	{
		return obj.tag.GetHashCode();
	}
}
