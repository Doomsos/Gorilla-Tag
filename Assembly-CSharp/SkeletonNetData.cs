using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000187 RID: 391
[NetworkStructWeaved(11)]
[StructLayout(2, Size = 44)]
public struct SkeletonNetData : INetworkStruct
{
	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000A70 RID: 2672 RVA: 0x00038CD5 File Offset: 0x00036ED5
	// (set) Token: 0x06000A71 RID: 2673 RVA: 0x00038CDD File Offset: 0x00036EDD
	public int CurrentState { readonly get; set; }

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x00038CE6 File Offset: 0x00036EE6
	// (set) Token: 0x06000A73 RID: 2675 RVA: 0x00038CF8 File Offset: 0x00036EF8
	[Networked]
	[NetworkedWeaved(1, 3)]
	public unsafe Vector3 Position
	{
		readonly get
		{
			return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position);
		}
		set
		{
			*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position) = value;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000A74 RID: 2676 RVA: 0x00038D0B File Offset: 0x00036F0B
	// (set) Token: 0x06000A75 RID: 2677 RVA: 0x00038D1D File Offset: 0x00036F1D
	[Networked]
	[NetworkedWeaved(4, 4)]
	public unsafe Quaternion Rotation
	{
		readonly get
		{
			return *(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation);
		}
		set
		{
			*(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation) = value;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000A76 RID: 2678 RVA: 0x00038D30 File Offset: 0x00036F30
	// (set) Token: 0x06000A77 RID: 2679 RVA: 0x00038D38 File Offset: 0x00036F38
	public int CurrentNode { readonly get; set; }

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x06000A78 RID: 2680 RVA: 0x00038D41 File Offset: 0x00036F41
	// (set) Token: 0x06000A79 RID: 2681 RVA: 0x00038D49 File Offset: 0x00036F49
	public int NextNode { readonly get; set; }

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x00038D52 File Offset: 0x00036F52
	// (set) Token: 0x06000A7B RID: 2683 RVA: 0x00038D5A File Offset: 0x00036F5A
	public int AngerPoint { readonly get; set; }

	// Token: 0x06000A7C RID: 2684 RVA: 0x00038D63 File Offset: 0x00036F63
	public SkeletonNetData(int state, Vector3 pos, Quaternion rot, int cNode, int nNode, int angerPoint)
	{
		this.CurrentState = state;
		this.Position = pos;
		this.Rotation = rot;
		this.CurrentNode = cNode;
		this.NextNode = nNode;
		this.AngerPoint = angerPoint;
	}

	// Token: 0x04000CCF RID: 3279
	[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@3 _Position;

	// Token: 0x04000CD0 RID: 3280
	[FixedBufferProperty(typeof(Quaternion), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(16)]
	private FixedStorage@4 _Rotation;
}
