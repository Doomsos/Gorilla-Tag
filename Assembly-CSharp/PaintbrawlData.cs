using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x0200050C RID: 1292
[NetworkStructWeaved(31)]
[StructLayout(2, Size = 124)]
public struct PaintbrawlData : INetworkStruct
{
	// Token: 0x1700037A RID: 890
	// (get) Token: 0x060020FB RID: 8443 RVA: 0x000AE8D4 File Offset: 0x000ACAD4
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(1, 10)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerLivesArray), 10, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x060020FC RID: 8444 RVA: 0x000AE8FC File Offset: 0x000ACAFC
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(11, 10)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerActorNumberArray), 10, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x060020FD RID: 8445 RVA: 0x000AE924 File Offset: 0x000ACB24
	[Networked]
	[Capacity(10)]
	[NetworkedWeavedArray(10, 1, typeof(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus))]
	[NetworkedWeaved(21, 10)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerStatusArray), 10, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	// Token: 0x04002BAF RID: 11183
	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	// Token: 0x04002BB0 RID: 11184
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@10 _playerLivesArray;

	// Token: 0x04002BB1 RID: 11185
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _playerActorNumberArray;

	// Token: 0x04002BB2 RID: 11186
	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 10, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@10 _playerStatusArray;
}
