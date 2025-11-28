using System;

namespace GorillaTag.Shared.Scripts.Utilities
{
	// Token: 0x0200104C RID: 4172
	public sealed class GTBitArray
	{
		// Token: 0x170009E6 RID: 2534
		public bool this[int idx]
		{
			get
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				return ((ulong)this._data[num] & (ulong)(1L << (num2 & 31))) > 0UL;
			}
			set
			{
				if (idx < 0 || idx >= this.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = idx / 32;
				int num2 = idx % 32;
				if (value)
				{
					this._data[num] |= 1U << num2;
					return;
				}
				this._data[num] &= ~(1U << num2);
			}
		}

		// Token: 0x06006932 RID: 26930 RVA: 0x002238FC File Offset: 0x00221AFC
		public GTBitArray(int length)
		{
			this.Length = length;
			this._data = ((length % 32 == 0) ? new uint[length / 32] : new uint[length / 32 + 1]);
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x06006933 RID: 26931 RVA: 0x00223954 File Offset: 0x00221B54
		public void Clear()
		{
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = 0U;
			}
		}

		// Token: 0x06006934 RID: 26932 RVA: 0x00223980 File Offset: 0x00221B80
		public void CopyFrom(GTBitArray other)
		{
			if (this.Length != other.Length)
			{
				throw new ArgumentException("Can only copy bit arrays of the same length.");
			}
			for (int i = 0; i < this._data.Length; i++)
			{
				this._data[i] = other._data[i];
			}
		}

		// Token: 0x040077DD RID: 30685
		public readonly int Length;

		// Token: 0x040077DE RID: 30686
		private readonly uint[] _data;
	}
}
