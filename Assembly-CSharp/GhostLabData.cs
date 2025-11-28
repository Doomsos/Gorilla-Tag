using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x0200017F RID: 383
[NetworkStructWeaved(11)]
[StructLayout(2, Size = 44)]
public struct GhostLabData : INetworkStruct
{
	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000A2C RID: 2604 RVA: 0x00036D89 File Offset: 0x00034F89
	// (set) Token: 0x06000A2D RID: 2605 RVA: 0x00036D91 File Offset: 0x00034F91
	public int DoorState { readonly get; set; }

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000A2E RID: 2606 RVA: 0x00036D9C File Offset: 0x00034F9C
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterNetworkBool))]
	[NetworkedWeaved(1, 10)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._OpenDoors), 10, ElementReaderWriterNetworkBool.GetInstance());
		}
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00036DC4 File Offset: 0x00034FC4
	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool flag = openDoors[i];
			this.OpenDoors.Set(i, flag);
		}
	}

	// Token: 0x04000C7C RID: 3196
	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ElementReaderWriterNetworkBool), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _OpenDoors;
}
