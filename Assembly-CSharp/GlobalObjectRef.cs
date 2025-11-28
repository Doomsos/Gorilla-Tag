using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020009D6 RID: 2518
[Serializable]
[StructLayout(2)]
public struct GlobalObjectRef
{
	// Token: 0x0600403B RID: 16443 RVA: 0x00158BAC File Offset: 0x00156DAC
	public static GlobalObjectRef ObjectToRefSlow(Object target)
	{
		return default(GlobalObjectRef);
	}

	// Token: 0x0600403C RID: 16444 RVA: 0x000743B1 File Offset: 0x000725B1
	public static Object RefToObjectSlow(GlobalObjectRef @ref)
	{
		return null;
	}

	// Token: 0x04005177 RID: 20855
	[FieldOffset(0)]
	public ulong targetObjectId;

	// Token: 0x04005178 RID: 20856
	[FieldOffset(8)]
	public ulong targetPrefabId;

	// Token: 0x04005179 RID: 20857
	[FieldOffset(16)]
	public Guid assetGUID;

	// Token: 0x0400517A RID: 20858
	[HideInInspector]
	[FieldOffset(32)]
	public int identifierType;

	// Token: 0x0400517B RID: 20859
	[NonSerialized]
	[FieldOffset(32)]
	private GlobalObjectRefType refType;
}
