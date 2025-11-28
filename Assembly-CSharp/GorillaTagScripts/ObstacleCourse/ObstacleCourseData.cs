using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E33 RID: 3635
	[NetworkStructWeaved(9)]
	[StructLayout(2, Size = 36)]
	public struct ObstacleCourseData : INetworkStruct
	{
		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06005ABF RID: 23231 RVA: 0x001D14C1 File Offset: 0x001CF6C1
		// (set) Token: 0x06005AC0 RID: 23232 RVA: 0x001D14C9 File Offset: 0x001CF6C9
		public int ObstacleCourseCount { readonly get; set; }

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x06005AC1 RID: 23233 RVA: 0x001D14D4 File Offset: 0x001CF6D4
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(1, 4)]
		public NetworkArray<int> WinnerActorNumber
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._WinnerActorNumber), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x06005AC2 RID: 23234 RVA: 0x001D14F8 File Offset: 0x001CF6F8
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(5, 4)]
		public NetworkArray<int> CurrentRaceState
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._CurrentRaceState), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06005AC3 RID: 23235 RVA: 0x001D151C File Offset: 0x001CF71C
		public ObstacleCourseData(List<ObstacleCourse> courses)
		{
			this.ObstacleCourseCount = courses.Count;
			int[] array = new int[this.ObstacleCourseCount];
			int[] array2 = new int[this.ObstacleCourseCount];
			for (int i = 0; i < courses.Count; i++)
			{
				array[i] = courses[i].winnerActorNumber;
				array2[i] = (int)courses[i].currentState;
			}
			this.WinnerActorNumber.CopyFrom(array, 0, this.ObstacleCourseCount);
			this.CurrentRaceState.CopyFrom(array2, 0, this.ObstacleCourseCount);
		}

		// Token: 0x040067F8 RID: 26616
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@4 _WinnerActorNumber;

		// Token: 0x040067F9 RID: 26617
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(20)]
		private FixedStorage@4 _CurrentRaceState;
	}
}
