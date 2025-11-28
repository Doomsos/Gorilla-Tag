using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000513 RID: 1299
[NetworkStructWeaved(23)]
[StructLayout(2, Size = 92)]
public struct HuntData : INetworkStruct
{
	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06002122 RID: 8482 RVA: 0x000AED60 File Offset: 0x000ACF60
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(3, 10)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentHuntedArray), 10, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06002123 RID: 8483 RVA: 0x000AED88 File Offset: 0x000ACF88
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(13, 10)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._currentTargetArray), 10, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x04002BB9 RID: 11193
	[FieldOffset(0)]
	public NetworkBool huntStarted;

	// Token: 0x04002BBA RID: 11194
	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	// Token: 0x04002BBB RID: 11195
	[FieldOffset(8)]
	public int countDownTime;

	// Token: 0x04002BBC RID: 11196
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@10 _currentHuntedArray;

	// Token: 0x04002BBD RID: 11197
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(52)]
	private FixedStorage@10 _currentTargetArray;
}
