using System;

namespace emotitron.Compression
{
	// Token: 0x02001130 RID: 4400
	public static class ArraySegmentExt
	{
		// Token: 0x06006E2B RID: 28203 RVA: 0x002424C7 File Offset: 0x002406C7
		public static ArraySegment<byte> ExtractArraySegment(byte[] buffer, ref int bitposition)
		{
			return new ArraySegment<byte>(buffer, 0, bitposition + 7 >> 3);
		}

		// Token: 0x06006E2C RID: 28204 RVA: 0x002424D6 File Offset: 0x002406D6
		public static ArraySegment<ushort> ExtractArraySegment(ushort[] buffer, ref int bitposition)
		{
			return new ArraySegment<ushort>(buffer, 0, bitposition + 15 >> 4);
		}

		// Token: 0x06006E2D RID: 28205 RVA: 0x002424E6 File Offset: 0x002406E6
		public static ArraySegment<uint> ExtractArraySegment(uint[] buffer, ref int bitposition)
		{
			return new ArraySegment<uint>(buffer, 0, bitposition + 31 >> 5);
		}

		// Token: 0x06006E2E RID: 28206 RVA: 0x002424F6 File Offset: 0x002406F6
		public static ArraySegment<ulong> ExtractArraySegment(ulong[] buffer, ref int bitposition)
		{
			return new ArraySegment<ulong>(buffer, 0, bitposition + 63 >> 6);
		}

		// Token: 0x06006E2F RID: 28207 RVA: 0x00242508 File Offset: 0x00240708
		public static void Append(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E30 RID: 28208 RVA: 0x0024253C File Offset: 0x0024073C
		public static void Append(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E31 RID: 28209 RVA: 0x00242570 File Offset: 0x00240770
		public static void Append(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Append(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E32 RID: 28210 RVA: 0x002425A4 File Offset: 0x002407A4
		public static void Write(this ArraySegment<byte> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E33 RID: 28211 RVA: 0x002425D8 File Offset: 0x002407D8
		public static void Write(this ArraySegment<uint> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E34 RID: 28212 RVA: 0x0024260C File Offset: 0x0024080C
		public static void Write(this ArraySegment<ulong> buffer, ulong value, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			buffer.Array.Write(value, ref bitposition, bits);
			bitposition -= num;
		}

		// Token: 0x06006E35 RID: 28213 RVA: 0x00242640 File Offset: 0x00240840
		public static ulong Read(this ArraySegment<byte> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 3;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x06006E36 RID: 28214 RVA: 0x00242674 File Offset: 0x00240874
		public static ulong Read(this ArraySegment<uint> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 5;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x06006E37 RID: 28215 RVA: 0x002426A8 File Offset: 0x002408A8
		public static ulong Read(this ArraySegment<ulong> buffer, ref int bitposition, int bits)
		{
			int num = buffer.Offset << 6;
			bitposition += num;
			ulong result = buffer.Array.Read(ref bitposition, bits);
			bitposition -= num;
			return result;
		}

		// Token: 0x06006E38 RID: 28216 RVA: 0x002426DC File Offset: 0x002408DC
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006E39 RID: 28217 RVA: 0x0024270C File Offset: 0x0024090C
		public static void ReadOutSafe(this ArraySegment<byte> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 3;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006E3A RID: 28218 RVA: 0x0024273C File Offset: 0x0024093C
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}

		// Token: 0x06006E3B RID: 28219 RVA: 0x0024276C File Offset: 0x0024096C
		public static void ReadOutSafe(this ArraySegment<ulong> source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			int num = source.Offset << 6;
			srcStartPos += num;
			source.Array.ReadOutSafe(srcStartPos, target, ref bitposition, bits);
		}
	}
}
