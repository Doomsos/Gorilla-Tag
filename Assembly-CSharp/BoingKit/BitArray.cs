using System;

namespace BoingKit
{
	// Token: 0x020011B8 RID: 4536
	public struct BitArray
	{
		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06007262 RID: 29282 RVA: 0x00258850 File Offset: 0x00256A50
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06007263 RID: 29283 RVA: 0x00258858 File Offset: 0x00256A58
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06007264 RID: 29284 RVA: 0x0025885D File Offset: 0x00256A5D
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06007265 RID: 29285 RVA: 0x00258864 File Offset: 0x00256A64
		private static void SetBit(int index, bool value, int[] blocks)
		{
			int blockIndex = BitArray.GetBlockIndex(index);
			int subIndex = BitArray.GetSubIndex(index);
			if (value)
			{
				blocks[blockIndex] |= 1 << subIndex;
				return;
			}
			blocks[blockIndex] &= ~(1 << subIndex);
		}

		// Token: 0x06007266 RID: 29286 RVA: 0x002588A6 File Offset: 0x00256AA6
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x06007267 RID: 29287 RVA: 0x002588C0 File Offset: 0x00256AC0
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06007268 RID: 29288 RVA: 0x002588E8 File Offset: 0x00256AE8
		public void Resize(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			if (num <= this.m_aBlock.Length)
			{
				return;
			}
			int[] array = new int[num];
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				array[i] = this.m_aBlock[i];
				i++;
			}
			this.m_aBlock = array;
		}

		// Token: 0x06007269 RID: 29289 RVA: 0x00258937 File Offset: 0x00256B37
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x0600726A RID: 29290 RVA: 0x00258940 File Offset: 0x00256B40
		public void SetAllBits(bool value)
		{
			int num = value ? -1 : 1;
			int i = 0;
			int num2 = this.m_aBlock.Length;
			while (i < num2)
			{
				this.m_aBlock[i] = num;
				i++;
			}
		}

		// Token: 0x0600726B RID: 29291 RVA: 0x00258973 File Offset: 0x00256B73
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x0600726C RID: 29292 RVA: 0x00258982 File Offset: 0x00256B82
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x040082CA RID: 33482
		private int[] m_aBlock;
	}
}
