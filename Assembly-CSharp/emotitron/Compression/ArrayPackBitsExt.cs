using System;

namespace emotitron.Compression
{
	// Token: 0x0200112E RID: 4398
	public static class ArrayPackBitsExt
	{
		// Token: 0x06006E07 RID: 28167 RVA: 0x00241EDC File Offset: 0x002400DC
		public unsafe static void WritePackedBits(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num);
		}

		// Token: 0x06006E08 RID: 28168 RVA: 0x00241F10 File Offset: 0x00240110
		public static void WritePackedBits(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06006E09 RID: 28169 RVA: 0x00241F44 File Offset: 0x00240144
		public static void WritePackedBits(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)((long)num), ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06006E0A RID: 28170 RVA: 0x00241F78 File Offset: 0x00240178
		public static void WritePackedBits(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			int num = value.UsedBitCount();
			int bits2 = bits.UsedBitCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num);
		}

		// Token: 0x06006E0B RID: 28171 RVA: 0x00241FA8 File Offset: 0x002401A8
		public unsafe static ulong ReadPackedBits(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits2);
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits3);
		}

		// Token: 0x06006E0C RID: 28172 RVA: 0x00241FD4 File Offset: 0x002401D4
		public static ulong ReadPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E0D RID: 28173 RVA: 0x00242000 File Offset: 0x00240200
		public static ulong ReadPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E0E RID: 28174 RVA: 0x0024202C File Offset: 0x0024022C
		public static ulong ReadPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E0F RID: 28175 RVA: 0x00242058 File Offset: 0x00240258
		public unsafe static void WriteSignedPackedBits(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArrayPackBitsExt.WritePackedBits(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E10 RID: 28176 RVA: 0x00242078 File Offset: 0x00240278
		public unsafe static int ReadSignedPackedBits(ulong* buffer, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E11 RID: 28177 RVA: 0x0024209C File Offset: 0x0024029C
		public static void WriteSignedPackedBits(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E12 RID: 28178 RVA: 0x002420BC File Offset: 0x002402BC
		public static int ReadSignedPackedBits(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E13 RID: 28179 RVA: 0x002420E0 File Offset: 0x002402E0
		public static void WriteSignedPackedBits(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E14 RID: 28180 RVA: 0x00242100 File Offset: 0x00240300
		public static int ReadSignedPackedBits(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E15 RID: 28181 RVA: 0x00242124 File Offset: 0x00240324
		public static void WriteSignedPackedBits(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBits((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E16 RID: 28182 RVA: 0x00242144 File Offset: 0x00240344
		public static int ReadSignedPackedBits(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E17 RID: 28183 RVA: 0x00242168 File Offset: 0x00240368
		public static void WriteSignedPackedBits64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.WritePackedBits(value2, ref bitposition, bits);
		}

		// Token: 0x06006E18 RID: 28184 RVA: 0x00242188 File Offset: 0x00240388
		public static long ReadSignedPackedBits64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBits(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}
	}
}
