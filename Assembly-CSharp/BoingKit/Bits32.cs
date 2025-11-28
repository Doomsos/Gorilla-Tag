using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011B7 RID: 4535
	[Serializable]
	public struct Bits32
	{
		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x0600725D RID: 29277 RVA: 0x002587F7 File Offset: 0x002569F7
		public int IntValue
		{
			get
			{
				return this.m_bits;
			}
		}

		// Token: 0x0600725E RID: 29278 RVA: 0x002587FF File Offset: 0x002569FF
		public Bits32(int bits = 0)
		{
			this.m_bits = bits;
		}

		// Token: 0x0600725F RID: 29279 RVA: 0x00258808 File Offset: 0x00256A08
		public void Clear()
		{
			this.m_bits = 0;
		}

		// Token: 0x06007260 RID: 29280 RVA: 0x00258811 File Offset: 0x00256A11
		public void SetBit(int index, bool value)
		{
			if (value)
			{
				this.m_bits |= 1 << index;
				return;
			}
			this.m_bits &= ~(1 << index);
		}

		// Token: 0x06007261 RID: 29281 RVA: 0x0025883E File Offset: 0x00256A3E
		public bool IsBitSet(int index)
		{
			return (this.m_bits & 1 << index) != 0;
		}

		// Token: 0x040082C9 RID: 33481
		[SerializeField]
		private int m_bits;
	}
}
