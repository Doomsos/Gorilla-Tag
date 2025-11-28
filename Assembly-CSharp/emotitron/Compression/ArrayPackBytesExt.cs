using System;

namespace emotitron.Compression
{
	// Token: 0x0200112F RID: 4399
	public static class ArrayPackBytesExt
	{
		// Token: 0x06006E19 RID: 28185 RVA: 0x002421C8 File Offset: 0x002403C8
		public unsafe static void WritePackedBytes(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num << 3);
		}

		// Token: 0x06006E1A RID: 28186 RVA: 0x00242200 File Offset: 0x00240400
		public static void WritePackedBytes(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06006E1B RID: 28187 RVA: 0x00242238 File Offset: 0x00240438
		public static void WritePackedBytes(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06006E1C RID: 28188 RVA: 0x00242270 File Offset: 0x00240470
		public static void WritePackedBytes(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06006E1D RID: 28189 RVA: 0x002422A8 File Offset: 0x002404A8
		public unsafe static ulong ReadPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits2) << 3;
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits3);
		}

		// Token: 0x06006E1E RID: 28190 RVA: 0x002422DC File Offset: 0x002404DC
		public static ulong ReadPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E1F RID: 28191 RVA: 0x00242310 File Offset: 0x00240510
		public static ulong ReadPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E20 RID: 28192 RVA: 0x00242344 File Offset: 0x00240544
		public static ulong ReadPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06006E21 RID: 28193 RVA: 0x00242378 File Offset: 0x00240578
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E22 RID: 28194 RVA: 0x00242398 File Offset: 0x00240598
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E23 RID: 28195 RVA: 0x002423BC File Offset: 0x002405BC
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E24 RID: 28196 RVA: 0x002423DC File Offset: 0x002405DC
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E25 RID: 28197 RVA: 0x00242400 File Offset: 0x00240600
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E26 RID: 28198 RVA: 0x00242420 File Offset: 0x00240620
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E27 RID: 28199 RVA: 0x00242444 File Offset: 0x00240644
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E28 RID: 28200 RVA: 0x00242464 File Offset: 0x00240664
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E29 RID: 28201 RVA: 0x00242488 File Offset: 0x00240688
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.WritePackedBytes(value2, ref bitposition, bits);
		}

		// Token: 0x06006E2A RID: 28202 RVA: 0x002424A8 File Offset: 0x002406A8
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}
	}
}
