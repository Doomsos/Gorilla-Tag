using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000453 RID: 1107
[NetworkStructWeaved(21)]
[Serializable]
[StructLayout(2, Size = 84)]
public struct ReliableStateData : INetworkStruct
{
	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001C1C RID: 7196 RVA: 0x00095706 File Offset: 0x00093906
	// (set) Token: 0x06001C1D RID: 7197 RVA: 0x0009570E File Offset: 0x0009390E
	public long Header { readonly get; set; }

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001C1E RID: 7198 RVA: 0x00095718 File Offset: 0x00093918
	[Networked]
	[Capacity(5)]
	[NetworkedWeavedArray(5, 2, typeof(ElementReaderWriterInt64))]
	[NetworkedWeaved(11, 10)]
	public NetworkArray<long> TransferrableStates
	{
		get
		{
			return new NetworkArray<long>(Native.ReferenceToPointer<FixedStorage@10>(ref this._TransferrableStates), 5, ElementReaderWriterInt64.GetInstance());
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001C1F RID: 7199 RVA: 0x0009573B File Offset: 0x0009393B
	// (set) Token: 0x06001C20 RID: 7200 RVA: 0x00095743 File Offset: 0x00093943
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06001C21 RID: 7201 RVA: 0x0009574C File Offset: 0x0009394C
	// (set) Token: 0x06001C22 RID: 7202 RVA: 0x00095754 File Offset: 0x00093954
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000314 RID: 788
	// (get) Token: 0x06001C23 RID: 7203 RVA: 0x0009575D File Offset: 0x0009395D
	// (set) Token: 0x06001C24 RID: 7204 RVA: 0x00095765 File Offset: 0x00093965
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06001C25 RID: 7205 RVA: 0x0009576E File Offset: 0x0009396E
	// (set) Token: 0x06001C26 RID: 7206 RVA: 0x00095776 File Offset: 0x00093976
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x17000316 RID: 790
	// (get) Token: 0x06001C27 RID: 7207 RVA: 0x0009577F File Offset: 0x0009397F
	// (set) Token: 0x06001C28 RID: 7208 RVA: 0x00095787 File Offset: 0x00093987
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001C29 RID: 7209 RVA: 0x00095790 File Offset: 0x00093990
	// (set) Token: 0x06001C2A RID: 7210 RVA: 0x00095798 File Offset: 0x00093998
	public long PackedBeads { readonly get; set; }

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001C2B RID: 7211 RVA: 0x000957A1 File Offset: 0x000939A1
	// (set) Token: 0x06001C2C RID: 7212 RVA: 0x000957A9 File Offset: 0x000939A9
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x04002626 RID: 9766
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ElementReaderWriterInt64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
