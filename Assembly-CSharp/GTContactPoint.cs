using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020002BF RID: 703
[Serializable]
[StructLayout(2)]
public class GTContactPoint
{
	// Token: 0x040015D6 RID: 5590
	[NonSerialized]
	[FieldOffset(0)]
	public Matrix4x4 data;

	// Token: 0x040015D7 RID: 5591
	[NonSerialized]
	[FieldOffset(0)]
	public Vector4 data0;

	// Token: 0x040015D8 RID: 5592
	[NonSerialized]
	[FieldOffset(16)]
	public Vector4 data1;

	// Token: 0x040015D9 RID: 5593
	[NonSerialized]
	[FieldOffset(32)]
	public Vector4 data2;

	// Token: 0x040015DA RID: 5594
	[NonSerialized]
	[FieldOffset(48)]
	public Vector4 data3;

	// Token: 0x040015DB RID: 5595
	[FieldOffset(0)]
	public Vector3 contactPoint;

	// Token: 0x040015DC RID: 5596
	[FieldOffset(12)]
	public float radius;

	// Token: 0x040015DD RID: 5597
	[FieldOffset(16)]
	public Vector3 counterVelocity;

	// Token: 0x040015DE RID: 5598
	[FieldOffset(28)]
	public float timestamp;

	// Token: 0x040015DF RID: 5599
	[FieldOffset(32)]
	public Color color;

	// Token: 0x040015E0 RID: 5600
	[FieldOffset(48)]
	public GTContactType contactType;

	// Token: 0x040015E1 RID: 5601
	[FieldOffset(52)]
	public float lifetime = 1f;

	// Token: 0x040015E2 RID: 5602
	[FieldOffset(56)]
	public uint free = 1U;
}
