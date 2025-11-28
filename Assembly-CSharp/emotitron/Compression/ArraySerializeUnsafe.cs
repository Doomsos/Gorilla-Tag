using System;

namespace emotitron.Compression
{
	// Token: 0x02001132 RID: 4402
	public static class ArraySerializeUnsafe
	{
		// Token: 0x06006E7F RID: 28287 RVA: 0x0024322C File Offset: 0x0024142C
		public unsafe static void WriteSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E80 RID: 28288 RVA: 0x0024324C File Offset: 0x0024144C
		public unsafe static void AppendSigned(ulong* buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(buffer, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E81 RID: 28289 RVA: 0x0024326C File Offset: 0x0024146C
		public unsafe static void AddSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E82 RID: 28290 RVA: 0x0024328C File Offset: 0x0024148C
		public unsafe static void AddSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E83 RID: 28291 RVA: 0x002432AC File Offset: 0x002414AC
		public unsafe static void AddSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Append(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E84 RID: 28292 RVA: 0x002432CC File Offset: 0x002414CC
		public unsafe static void InjectSigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E85 RID: 28293 RVA: 0x002432EC File Offset: 0x002414EC
		public unsafe static void InjectSigned(this short value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E86 RID: 28294 RVA: 0x0024330C File Offset: 0x0024150C
		public unsafe static void InjectSigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E87 RID: 28295 RVA: 0x0024332C File Offset: 0x0024152C
		public unsafe static void PokeSigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E88 RID: 28296 RVA: 0x00243350 File Offset: 0x00241550
		public unsafe static void PokeSigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E89 RID: 28297 RVA: 0x00243374 File Offset: 0x00241574
		public unsafe static void PokeSigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006E8A RID: 28298 RVA: 0x00243398 File Offset: 0x00241598
		public unsafe static int ReadSigned(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E8B RID: 28299 RVA: 0x002433BC File Offset: 0x002415BC
		public unsafe static int PeekSigned(ulong* uPtr, int bitposition, int bits)
		{
			uint num = (uint)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006E8C RID: 28300 RVA: 0x002433E0 File Offset: 0x002415E0
		public unsafe static void Append(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (uPtr[num2] & num3) | value << num;
			uPtr[num2] = num4;
			uPtr[num2 + 1] = num4 >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x06006E8D RID: 28301 RVA: 0x00243438 File Offset: 0x00241638
		public unsafe static void Write(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			num = 64 - num;
			if (num < bits)
			{
				num4 = num3 >> num;
				num5 = value >> num;
				num2++;
				uPtr[num2] = ((uPtr[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x06006E8E RID: 28302 RVA: 0x002434BC File Offset: 0x002416BC
		public unsafe static ulong Read(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06006E8F RID: 28303 RVA: 0x00243520 File Offset: 0x00241720
		public unsafe static ulong Read(ulong* uPtr, int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = uPtr[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= uPtr[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x06006E90 RID: 28304 RVA: 0x0024357F File Offset: 0x0024177F
		public unsafe static void Add(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006E91 RID: 28305 RVA: 0x0024358B File Offset: 0x0024178B
		public unsafe static void Add(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E92 RID: 28306 RVA: 0x0024358B File Offset: 0x0024178B
		public unsafe static void Add(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E93 RID: 28307 RVA: 0x0024358B File Offset: 0x0024178B
		public unsafe static void Add(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E94 RID: 28308 RVA: 0x0024357F File Offset: 0x0024177F
		public unsafe static void AddUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E95 RID: 28309 RVA: 0x00243598 File Offset: 0x00241798
		public unsafe static void AddUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006E96 RID: 28310 RVA: 0x00243598 File Offset: 0x00241798
		public unsafe static void AddUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006E97 RID: 28311 RVA: 0x00243598 File Offset: 0x00241798
		public unsafe static void AddUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Append(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006E98 RID: 28312 RVA: 0x002435A5 File Offset: 0x002417A5
		public unsafe static void Inject(this ulong value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006E99 RID: 28313 RVA: 0x002435B0 File Offset: 0x002417B0
		public unsafe static void Inject(this uint value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E9A RID: 28314 RVA: 0x002435B0 File Offset: 0x002417B0
		public unsafe static void Inject(this ushort value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E9B RID: 28315 RVA: 0x002435B0 File Offset: 0x002417B0
		public unsafe static void Inject(this byte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E9C RID: 28316 RVA: 0x002435A5 File Offset: 0x002417A5
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006E9D RID: 28317 RVA: 0x002435BC File Offset: 0x002417BC
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006E9E RID: 28318 RVA: 0x002435C8 File Offset: 0x002417C8
		public unsafe static void InjectUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006E9F RID: 28319 RVA: 0x002435BC File Offset: 0x002417BC
		public unsafe static void InjectUnsigned(this sbyte value, ulong* uPtr, ref int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006EA0 RID: 28320 RVA: 0x002435D5 File Offset: 0x002417D5
		public unsafe static void Poke(this ulong value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, bits);
		}

		// Token: 0x06006EA1 RID: 28321 RVA: 0x002435E1 File Offset: 0x002417E1
		public unsafe static void Poke(this uint value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006EA2 RID: 28322 RVA: 0x002435E1 File Offset: 0x002417E1
		public unsafe static void Poke(this ushort value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006EA3 RID: 28323 RVA: 0x002435E1 File Offset: 0x002417E1
		public unsafe static void Poke(this byte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006EA4 RID: 28324 RVA: 0x002435D5 File Offset: 0x002417D5
		public unsafe static void InjectUnsigned(this long value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006EA5 RID: 28325 RVA: 0x002435C8 File Offset: 0x002417C8
		public unsafe static void InjectUnsigned(this int value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006EA6 RID: 28326 RVA: 0x002435C8 File Offset: 0x002417C8
		public unsafe static void PokeUnsigned(this short value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006EA7 RID: 28327 RVA: 0x002435C8 File Offset: 0x002417C8
		public unsafe static void PokeUnsigned(this sbyte value, ulong* uPtr, int bitposition, int bits)
		{
			ArraySerializeUnsafe.Write(uPtr, (ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006EA8 RID: 28328 RVA: 0x002435F0 File Offset: 0x002417F0
		public unsafe static void ReadOutUnsafe(ulong* sourcePtr, int sourcePos, ulong* targetPtr, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = ArraySerializeUnsafe.Read(sourcePtr, ref num, num2);
				ArraySerializeUnsafe.Write(targetPtr, value, ref targetPos, num2);
			}
			targetPos += bits;
		}

		// Token: 0x06006EA9 RID: 28329 RVA: 0x00243638 File Offset: 0x00241838
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAA RID: 28330 RVA: 0x002436C4 File Offset: 0x002418C4
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr;
					if (target == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAB RID: 28331 RVA: 0x00243750 File Offset: 0x00241950
		public unsafe static void ReadOutUnsafe(this ulong[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (ulong[] array = source)
			{
				ulong* uPtr;
				if (source == null || array.Length == 0)
				{
					uPtr = null;
				}
				else
				{
					uPtr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr2;
					if (target == null || array2.Length == 0)
					{
						uPtr2 = null;
					}
					else
					{
						uPtr2 = &array2[0];
					}
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAC RID: 28332 RVA: 0x002437D8 File Offset: 0x002419D8
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAD RID: 28333 RVA: 0x00243868 File Offset: 0x00241A68
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAE RID: 28334 RVA: 0x002438F8 File Offset: 0x00241AF8
		public unsafe static void ReadOutUnsafe(this uint[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (uint[] array = source)
			{
				uint* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EAF RID: 28335 RVA: 0x00243984 File Offset: 0x00241B84
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, ulong[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (ulong[] array2 = target)
				{
					ulong* uPtr;
					if (target == null || array2.Length == 0)
					{
						uPtr = null;
					}
					else
					{
						uPtr = &array2[0];
					}
					ulong* uPtr2 = (ulong*)ptr;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr2, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EB0 RID: 28336 RVA: 0x00243A10 File Offset: 0x00241C10
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, uint[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (uint[] array2 = target)
				{
					uint* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x06006EB1 RID: 28337 RVA: 0x00243AA0 File Offset: 0x00241CA0
		public unsafe static void ReadOutUnsafe(this byte[] source, int sourcePos, byte[] target, ref int targetPos, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = sourcePos;
			int i = bits;
			fixed (byte[] array = source)
			{
				byte* ptr;
				if (source == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				fixed (byte[] array2 = target)
				{
					byte* ptr2;
					if (target == null || array2.Length == 0)
					{
						ptr2 = null;
					}
					else
					{
						ptr2 = &array2[0];
					}
					ulong* uPtr = (ulong*)ptr;
					ulong* uPtr2 = (ulong*)ptr2;
					while (i > 0)
					{
						int num2 = (i > 64) ? 64 : i;
						ulong value = ArraySerializeUnsafe.Read(uPtr, ref num, num2);
						ArraySerializeUnsafe.Write(uPtr2, value, ref targetPos, num2);
						i -= num2;
					}
				}
			}
			targetPos += bits;
		}

		// Token: 0x04007FC9 RID: 32713
		private const string bufferOverrunMsg = "Byte buffer overrun. Dataloss will occur.";
	}
}
