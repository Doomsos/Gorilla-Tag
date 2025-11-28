using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x020003C3 RID: 963
[NetworkStructWeaved(1)]
[Serializable]
[StructLayout(2, Size = 4)]
public struct HitTargetStruct : INetworkStruct
{
	// Token: 0x06001722 RID: 5922 RVA: 0x000807E8 File Offset: 0x0007E9E8
	public HitTargetStruct(int v)
	{
		this.Score = v;
	}

	// Token: 0x04002130 RID: 8496
	[FieldOffset(0)]
	public int Score;
}
