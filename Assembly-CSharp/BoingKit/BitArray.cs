using System;

namespace BoingKit
{
	// Token: 0x020011B8 RID: 4536
	public struct BitArray
	{
		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06007262 RID: 29282 RVA: 0x00258830 File Offset: 0x00256A30
		public int[] Blocks
		{
			get
			{
				return this.m_aBlock;
			}
		}

		// Token: 0x06007263 RID: 29283 RVA: 0x00258838 File Offset: 0x00256A38
		private static int GetBlockIndex(int index)
		{
			return index / 4;
		}

		// Token: 0x06007264 RID: 29284 RVA: 0x0025883D File Offset: 0x00256A3D
		private static int GetSubIndex(int index)
		{
			return index % 4;
		}

		// Token: 0x06007265 RID: 29285 RVA: 0x00258844 File Offset: 0x00256A44
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

		// Token: 0x06007266 RID: 29286 RVA: 0x00258886 File Offset: 0x00256A86
		private static bool IsBitSet(int index, int[] blocks)
		{
			return (blocks[BitArray.GetBlockIndex(index)] & 1 << BitArray.GetSubIndex(index)) != 0;
		}

		// Token: 0x06007267 RID: 29287 RVA: 0x002588A0 File Offset: 0x00256AA0
		public BitArray(int capacity)
		{
			int num = (capacity + 4 - 1) / 4;
			this.m_aBlock = new int[num];
			this.Clear();
		}

		// Token: 0x06007268 RID: 29288 RVA: 0x002588C8 File Offset: 0x00256AC8
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

		// Token: 0x06007269 RID: 29289 RVA: 0x00258917 File Offset: 0x00256B17
		public void Clear()
		{
			this.SetAllBits(false);
		}

		// Token: 0x0600726A RID: 29290 RVA: 0x00258920 File Offset: 0x00256B20
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

		// Token: 0x0600726B RID: 29291 RVA: 0x00258953 File Offset: 0x00256B53
		public void SetBit(int index, bool value)
		{
			BitArray.SetBit(index, value, this.m_aBlock);
		}

		// Token: 0x0600726C RID: 29292 RVA: 0x00258962 File Offset: 0x00256B62
		public bool IsBitSet(int index)
		{
			return BitArray.IsBitSet(index, this.m_aBlock);
		}

		// Token: 0x040082CA RID: 33482
		private int[] m_aBlock;
	}
}
