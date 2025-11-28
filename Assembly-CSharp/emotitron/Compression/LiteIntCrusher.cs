using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x0200113E RID: 4414
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<int>
	{
		// Token: 0x06006F4B RID: 28491 RVA: 0x00244C4C File Offset: 0x00242E4C
		public LiteIntCrusher()
		{
			this.compressType = LiteIntCompressType.PackSigned;
			this.min = -128;
			this.max = 127;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(this.min, this.max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06006F4C RID: 28492 RVA: 0x00244CA2 File Offset: 0x00242EA2
		public LiteIntCrusher(LiteIntCompressType comType = LiteIntCompressType.PackSigned, int min = -128, int max = 127)
		{
			this.compressType = comType;
			this.min = min;
			this.max = max;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(min, max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x06006F4D RID: 28493 RVA: 0x00244CE4 File Offset: 0x00242EE4
		public override ulong WriteValue(int val, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
			{
				uint num = (uint)(val << 1 ^ val >> 31);
				buffer.WritePackedBytes((ulong)num, ref bitposition, 32);
				return (ulong)num;
			}
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)val, ref bitposition, 32);
				return (ulong)val;
			case LiteIntCompressType.Range:
			{
				ulong num2 = this.Encode(val);
				buffer.Write(num2, ref bitposition, this.bits);
				return num2;
			}
			default:
				return 0UL;
			}
		}

		// Token: 0x06006F4E RID: 28494 RVA: 0x00244D50 File Offset: 0x00242F50
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.PackUnsigned:
				buffer.WritePackedBytes((ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.Range:
				buffer.Write((ulong)cval, ref bitposition, this.bits);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006F4F RID: 28495 RVA: 0x00244DA0 File Offset: 0x00242FA0
		public override int ReadValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return buffer.ReadSignedPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (int)buffer.ReadPackedBytes(ref bitposition, 32);
			case LiteIntCompressType.Range:
			{
				uint val = (uint)buffer.Read(ref bitposition, this.bits);
				return this.Decode(val);
			}
			default:
				return 0;
			}
		}

		// Token: 0x06006F50 RID: 28496 RVA: 0x00244DF5 File Offset: 0x00242FF5
		public override ulong Encode(int value)
		{
			value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
			return (ulong)((long)(value - this.smallest));
		}

		// Token: 0x06006F51 RID: 28497 RVA: 0x00244E25 File Offset: 0x00243025
		public override int Decode(uint cvalue)
		{
			return (int)((ulong)cvalue + (ulong)((long)this.smallest));
		}

		// Token: 0x06006F52 RID: 28498 RVA: 0x00244E34 File Offset: 0x00243034
		public static void Recalculate(int min, int max, ref int smallest, ref int biggest, ref int bits)
		{
			if (min < max)
			{
				smallest = min;
				biggest = max;
			}
			else
			{
				smallest = max;
				biggest = min;
			}
			int maxvalue = biggest - smallest;
			bits = LiteCrusher.GetBitsForMaxValue((uint)maxvalue);
		}

		// Token: 0x06006F53 RID: 28499 RVA: 0x00244E64 File Offset: 0x00243064
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				base.GetType().Name,
				" ",
				this.compressType.ToString(),
				" mn: ",
				this.min.ToString(),
				" mx: ",
				this.max.ToString(),
				" sm: ",
				this.smallest.ToString()
			});
		}

		// Token: 0x04007FF1 RID: 32753
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04007FF2 RID: 32754
		[SerializeField]
		protected int min;

		// Token: 0x04007FF3 RID: 32755
		[SerializeField]
		protected int max;

		// Token: 0x04007FF4 RID: 32756
		[SerializeField]
		private int smallest;

		// Token: 0x04007FF5 RID: 32757
		[SerializeField]
		private int biggest;
	}
}
