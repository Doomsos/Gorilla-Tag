using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000515 RID: 1301
[NetworkStructWeaved(12)]
[StructLayout(2, Size = 48)]
public struct TagData : INetworkStruct
{
	// Token: 0x17000386 RID: 902
	// (get) Token: 0x0600212B RID: 8491 RVA: 0x000AEE6C File Offset: 0x000AD06C
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(2, 10)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._infectedPlayerList), 10, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x0600212C RID: 8492 RVA: 0x000AEE93 File Offset: 0x000AD093
	// (set) Token: 0x0600212D RID: 8493 RVA: 0x000AEE9B File Offset: 0x000AD09B
	public int currentItID { readonly get; set; }

	// Token: 0x04002BC0 RID: 11200
	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	// Token: 0x04002BC1 RID: 11201
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@10 _infectedPlayerList;
}
