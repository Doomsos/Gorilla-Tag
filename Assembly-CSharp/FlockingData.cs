using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
[NetworkStructWeaved(337)]
[StructLayout(2, Size = 1348)]
public struct FlockingData : INetworkStruct
{
	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06002620 RID: 9760 RVA: 0x000CBE68 File Offset: 0x000CA068
	// (set) Token: 0x06002621 RID: 9761 RVA: 0x000CBE70 File Offset: 0x000CA070
	public int count { readonly get; set; }

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06002622 RID: 9762 RVA: 0x000CBE7C File Offset: 0x000CA07C
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 3, typeof(ElementReaderWriterVector3))]
	[NetworkedWeaved(1, 153)]
	public NetworkLinkedList<Vector3> Positions
	{
		get
		{
			return new NetworkLinkedList<Vector3>(Native.ReferenceToPointer<FixedStorage@153>(ref this._Positions), 30, ElementReaderWriterVector3.GetInstance());
		}
	}

	// Token: 0x170003D3 RID: 979
	// (get) Token: 0x06002623 RID: 9763 RVA: 0x000CBEA4 File Offset: 0x000CA0A4
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 4, typeof(ReaderWriter@UnityEngine_Quaternion))]
	[NetworkedWeaved(154, 183)]
	public NetworkLinkedList<Quaternion> Rotations
	{
		get
		{
			return new NetworkLinkedList<Quaternion>(Native.ReferenceToPointer<FixedStorage@183>(ref this._Rotations), 30, ReaderWriter@UnityEngine_Quaternion.GetInstance());
		}
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000CBECC File Offset: 0x000CA0CC
	public FlockingData(List<Flocking> items)
	{
		this.count = items.Count;
		foreach (Flocking flocking in items)
		{
			this.Positions.Add(flocking.pos);
			this.Rotations.Add(flocking.rot);
		}
	}

	// Token: 0x040031FA RID: 12794
	[FixedBufferProperty(typeof(NetworkLinkedList<Vector3>), typeof(UnityLinkedListSurrogate@ElementReaderWriterVector3), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@153 _Positions;

	// Token: 0x040031FB RID: 12795
	[FixedBufferProperty(typeof(NetworkLinkedList<Quaternion>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(616)]
	private FixedStorage@183 _Rotations;
}
