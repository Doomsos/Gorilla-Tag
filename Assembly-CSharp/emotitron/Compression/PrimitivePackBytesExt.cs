using System;

namespace emotitron.Compression
{
	// Token: 0x02001137 RID: 4407
	public static class PrimitivePackBytesExt
	{
		// Token: 0x06006EC7 RID: 28359 RVA: 0x00243F28 File Offset: 0x00242128
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06006EC8 RID: 28360 RVA: 0x00243F64 File Offset: 0x00242164
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06006EC9 RID: 28361 RVA: 0x00243FA0 File Offset: 0x002421A0
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06006ECA RID: 28362 RVA: 0x00243FDC File Offset: 0x002421DC
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
		}

		// Token: 0x06006ECB RID: 28363 RVA: 0x00244018 File Offset: 0x00242218
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06006ECC RID: 28364 RVA: 0x00244044 File Offset: 0x00242244
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06006ECD RID: 28365 RVA: 0x00244070 File Offset: 0x00242270
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006ECE RID: 28366 RVA: 0x00244090 File Offset: 0x00242290
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
