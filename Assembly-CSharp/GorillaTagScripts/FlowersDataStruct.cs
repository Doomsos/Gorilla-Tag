using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DE2 RID: 3554
	[NetworkStructWeaved(13)]
	[StructLayout(2, Size = 52)]
	public struct FlowersDataStruct : INetworkStruct
	{
		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x06005853 RID: 22611 RVA: 0x001C38BB File Offset: 0x001C1ABB
		// (set) Token: 0x06005854 RID: 22612 RVA: 0x001C38C3 File Offset: 0x001C1AC3
		public int FlowerCount { readonly get; set; }

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x06005855 RID: 22613 RVA: 0x001C38CC File Offset: 0x001C1ACC
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterByte))]
		[NetworkedWeaved(1, 6)]
		public NetworkLinkedList<byte> FlowerWateredData
		{
			get
			{
				return new NetworkLinkedList<byte>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerWateredData), 1, ElementReaderWriterByte.GetInstance());
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x06005856 RID: 22614 RVA: 0x001C38F0 File Offset: 0x001C1AF0
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(7, 6)]
		public NetworkLinkedList<int> FlowerStateData
		{
			get
			{
				return new NetworkLinkedList<int>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerStateData), 1, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06005857 RID: 22615 RVA: 0x001C3914 File Offset: 0x001C1B14
		public FlowersDataStruct(List<Flower> allFlowers)
		{
			this.FlowerCount = allFlowers.Count;
			foreach (Flower flower in allFlowers)
			{
				this.FlowerWateredData.Add(flower.IsWatered ? 1 : 0);
				this.FlowerStateData.Add((int)flower.GetCurrentState());
			}
		}

		// Token: 0x0400659B RID: 26011
		[FixedBufferProperty(typeof(NetworkLinkedList<byte>), typeof(UnityLinkedListSurrogate@ElementReaderWriterByte), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@6 _FlowerWateredData;

		// Token: 0x0400659C RID: 26012
		[FixedBufferProperty(typeof(NetworkLinkedList<int>), typeof(UnityLinkedListSurrogate@ElementReaderWriterInt32), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(28)]
		private FixedStorage@6 _FlowerStateData;
	}
}
