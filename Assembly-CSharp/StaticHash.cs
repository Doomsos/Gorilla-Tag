using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000C82 RID: 3202
public static class StaticHash
{
	// Token: 0x06004E2E RID: 20014 RVA: 0x00195AC4 File Offset: 0x00193CC4
	[MethodImpl(256)]
	public static int Compute(int i)
	{
		uint num = (uint)(i + 2127912214 + (i << 12));
		num = (num ^ 3345072700U ^ num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U ^ num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ num >> 16);
	}

	// Token: 0x06004E2F RID: 20015 RVA: 0x00195B20 File Offset: 0x00193D20
	[MethodImpl(256)]
	public static int Compute(uint u)
	{
		uint num = u + 2127912214U + (u << 12);
		num = (num ^ 3345072700U ^ num >> 19);
		num = num + 374761393U + (num << 5);
		num = (num + 3550635116U ^ num << 9);
		num = num + 4251993797U + (num << 3);
		return (int)(num ^ 3042594569U ^ num >> 16);
	}

	// Token: 0x06004E30 RID: 20016 RVA: 0x00195B7C File Offset: 0x00193D7C
	[MethodImpl(256)]
	public unsafe static int Compute(float f)
	{
		return StaticHash.Compute(*Unsafe.As<float, int>(ref f));
	}

	// Token: 0x06004E31 RID: 20017 RVA: 0x00195B8C File Offset: 0x00193D8C
	[MethodImpl(256)]
	public static int Compute(float f1, float f2)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004E32 RID: 20018 RVA: 0x00195BAC File Offset: 0x00193DAC
	[MethodImpl(256)]
	public static int Compute(float f1, float f2, float f3)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		int i3 = StaticHash.Compute(f3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004E33 RID: 20019 RVA: 0x00195BD4 File Offset: 0x00193DD4
	[MethodImpl(256)]
	public static int Compute(float f1, float f2, float f3, float f4)
	{
		int i = StaticHash.Compute(f1);
		int i2 = StaticHash.Compute(f2);
		int i3 = StaticHash.Compute(f3);
		int i4 = StaticHash.Compute(f4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004E34 RID: 20020 RVA: 0x00195C04 File Offset: 0x00193E04
	[MethodImpl(256)]
	public static int Compute(long l)
	{
		ulong num = (ulong)(~(ulong)l + (l << 18));
		num ^= num >> 31;
		num *= 21UL;
		num ^= num >> 11;
		num += num << 6;
		num ^= num >> 22;
		return (int)num;
	}

	// Token: 0x06004E35 RID: 20021 RVA: 0x00195C40 File Offset: 0x00193E40
	[MethodImpl(256)]
	public static int Compute(long l1, long l2)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004E36 RID: 20022 RVA: 0x00195C60 File Offset: 0x00193E60
	[MethodImpl(256)]
	public static int Compute(long l1, long l2, long l3)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		int i3 = StaticHash.Compute(l3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004E37 RID: 20023 RVA: 0x00195C88 File Offset: 0x00193E88
	[MethodImpl(256)]
	public static int Compute(long l1, long l2, long l3, long l4)
	{
		int i = StaticHash.Compute(l1);
		int i2 = StaticHash.Compute(l2);
		int i3 = StaticHash.Compute(l3);
		int i4 = StaticHash.Compute(l4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004E38 RID: 20024 RVA: 0x00195CB8 File Offset: 0x00193EB8
	[MethodImpl(256)]
	public unsafe static int Compute(double d)
	{
		return StaticHash.Compute(*Unsafe.As<double, long>(ref d));
	}

	// Token: 0x06004E39 RID: 20025 RVA: 0x00195CC8 File Offset: 0x00193EC8
	[MethodImpl(256)]
	public static int Compute(double d1, double d2)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004E3A RID: 20026 RVA: 0x00195CE8 File Offset: 0x00193EE8
	[MethodImpl(256)]
	public static int Compute(double d1, double d2, double d3)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		int i3 = StaticHash.Compute(d3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004E3B RID: 20027 RVA: 0x00195D10 File Offset: 0x00193F10
	[MethodImpl(256)]
	public static int Compute(double d1, double d2, double d3, double d4)
	{
		int i = StaticHash.Compute(d1);
		int i2 = StaticHash.Compute(d2);
		int i3 = StaticHash.Compute(d3);
		int i4 = StaticHash.Compute(d4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004E3C RID: 20028 RVA: 0x00195D40 File Offset: 0x00193F40
	[MethodImpl(256)]
	public static int Compute(bool b)
	{
		if (!b)
		{
			return 1800329511;
		}
		return -1266253386;
	}

	// Token: 0x06004E3D RID: 20029 RVA: 0x00195D50 File Offset: 0x00193F50
	[MethodImpl(256)]
	public static int Compute(bool b1, bool b2)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004E3E RID: 20030 RVA: 0x00195D70 File Offset: 0x00193F70
	[MethodImpl(256)]
	public static int Compute(bool b1, bool b2, bool b3)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		int i3 = StaticHash.Compute(b3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004E3F RID: 20031 RVA: 0x00195D98 File Offset: 0x00193F98
	[MethodImpl(256)]
	public static int Compute(bool b1, bool b2, bool b3, bool b4)
	{
		int i = StaticHash.Compute(b1);
		int i2 = StaticHash.Compute(b2);
		int i3 = StaticHash.Compute(b3);
		int i4 = StaticHash.Compute(b4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004E40 RID: 20032 RVA: 0x00195DC8 File Offset: 0x00193FC8
	[MethodImpl(256)]
	public static int Compute(DateTime dt)
	{
		return StaticHash.Compute(dt.ToBinary());
	}

	// Token: 0x06004E41 RID: 20033 RVA: 0x00195DD8 File Offset: 0x00193FD8
	[MethodImpl(256)]
	public static int Compute(string s)
	{
		if (s == null || s.Length == 0)
		{
			return 0;
		}
		int i = s.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)s.get_Chars(num3);
			uint num4 = (uint)((uint)s.get_Chars(num3 + 1) << 11) ^ num;
			num = (num << 16 ^ num4);
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)s.get_Chars(num3);
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x06004E42 RID: 20034 RVA: 0x00195E80 File Offset: 0x00194080
	[MethodImpl(256)]
	public static int Compute(string s1, string s2)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004E43 RID: 20035 RVA: 0x00195EA0 File Offset: 0x001940A0
	[MethodImpl(256)]
	public static int Compute(string s1, string s2, string s3)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		int i3 = StaticHash.Compute(s3);
		return StaticHash.Compute(i, i2, i3);
	}

	// Token: 0x06004E44 RID: 20036 RVA: 0x00195EC8 File Offset: 0x001940C8
	[MethodImpl(256)]
	public static int Compute(string s1, string s2, string s3, string s4)
	{
		int i = StaticHash.Compute(s1);
		int i2 = StaticHash.Compute(s2);
		int i3 = StaticHash.Compute(s3);
		int i4 = StaticHash.Compute(s4);
		return StaticHash.Compute(i, i2, i3, i4);
	}

	// Token: 0x06004E45 RID: 20037 RVA: 0x00195EF8 File Offset: 0x001940F8
	[MethodImpl(256)]
	public static int Compute(byte[] bytes)
	{
		if (bytes == null || bytes.Length == 0)
		{
			return 0;
		}
		int i = bytes.Length;
		uint num = (uint)i;
		int num2 = i & 1;
		i >>= 1;
		int num3 = 0;
		while (i > 0)
		{
			num += (uint)bytes[num3];
			uint num4 = (uint)((int)bytes[num3 + 1] << 11 ^ (int)num);
			num = (num << 16 ^ num4);
			num3 += 2;
			num += num >> 11;
			i--;
		}
		if (num2 == 1)
		{
			num += (uint)bytes[num3];
			num ^= num << 11;
			num += num >> 17;
		}
		num ^= num << 3;
		num += num >> 5;
		num ^= num << 4;
		num += num >> 17;
		num ^= num << 25;
		return (int)(num + (num >> 6));
	}

	// Token: 0x06004E46 RID: 20038 RVA: 0x00195F8C File Offset: 0x0019418C
	[MethodImpl(256)]
	public static int Compute(int i1, int i2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint result = num;
		num += (uint)i1;
		num2 += (uint)i2;
		StaticHash.Finalize(ref num, ref num2, ref result);
		return (int)result;
	}

	// Token: 0x06004E47 RID: 20039 RVA: 0x00195FB8 File Offset: 0x001941B8
	[MethodImpl(256)]
	public static int Compute(int i1, int i2, int i3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004E48 RID: 20040 RVA: 0x00195FE8 File Offset: 0x001941E8
	[MethodImpl(256)]
	public static int Compute(int i1, int i2, int i3, int i4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += (uint)i1;
		num2 += (uint)i2;
		num3 += (uint)i3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += (uint)i4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004E49 RID: 20041 RVA: 0x00196028 File Offset: 0x00194228
	[MethodImpl(256)]
	public static int Compute(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += (uint)values[num5];
			num3 += (uint)values[num5 + 1];
			num4 += (uint)values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += (uint)values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += (uint)values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += (uint)values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06004E4A RID: 20042 RVA: 0x001960C4 File Offset: 0x001942C4
	[MethodImpl(256)]
	public static int Compute(uint[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 224428569;
		}
		int num = values.Length;
		uint num2 = (uint)(-559038737 + (num << 2));
		uint num3 = num2;
		uint num4 = num2;
		int num5 = 0;
		while (num - num5 > 3)
		{
			num2 += values[num5];
			num3 += values[num5 + 1];
			num4 += values[num5 + 2];
			StaticHash.Mix(ref num2, ref num3, ref num4);
			num5 += 3;
		}
		if (num - num5 > 2)
		{
			num4 += values[num5 + 2];
		}
		if (num - num5 > 1)
		{
			num3 += values[num5 + 1];
		}
		if (num - num5 > 0)
		{
			num2 += values[num5];
			StaticHash.Finalize(ref num2, ref num3, ref num4);
		}
		return (int)num4;
	}

	// Token: 0x06004E4B RID: 20043 RVA: 0x00196160 File Offset: 0x00194360
	[MethodImpl(256)]
	public static int Compute(uint u1, uint u2)
	{
		uint num = 3735928567U;
		uint num2 = num;
		uint result = num;
		num += u1;
		num2 += u2;
		StaticHash.Finalize(ref num, ref num2, ref result);
		return (int)result;
	}

	// Token: 0x06004E4C RID: 20044 RVA: 0x0019618C File Offset: 0x0019438C
	[MethodImpl(256)]
	public static int Compute(uint u1, uint u2, uint u3)
	{
		uint num = 3735928571U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004E4D RID: 20045 RVA: 0x001961BC File Offset: 0x001943BC
	[MethodImpl(256)]
	public static int Compute(uint u1, uint u2, uint u3, uint u4)
	{
		uint num = 3735928575U;
		uint num2 = num;
		uint num3 = num;
		num += u1;
		num2 += u2;
		num3 += u3;
		StaticHash.Mix(ref num, ref num2, ref num3);
		num += u4;
		StaticHash.Finalize(ref num, ref num2, ref num3);
		return (int)num3;
	}

	// Token: 0x06004E4E RID: 20046 RVA: 0x001961FC File Offset: 0x001943FC
	[MethodImpl(256)]
	public static int ComputeOrderAgnostic(int[] values)
	{
		if (values == null || values.Length == 0)
		{
			return 0;
		}
		uint num = (uint)StaticHash.Compute(values[0]);
		if (values.Length == 1)
		{
			return (int)num;
		}
		for (int i = 1; i < values.Length; i++)
		{
			num += (uint)StaticHash.Compute(values[i]);
		}
		return (int)num;
	}

	// Token: 0x06004E4F RID: 20047 RVA: 0x00196240 File Offset: 0x00194440
	[MethodImpl(256)]
	public static long Compute128To64(long a, long b)
	{
		ulong num = (ulong)((b ^ a) * -7070675565921424023L);
		num ^= num >> 47;
		long num2 = (a ^ (long)num) * -7070675565921424023L;
		return (num2 ^ (long)((ulong)num2 >> 47)) * -7070675565921424023L;
	}

	// Token: 0x06004E50 RID: 20048 RVA: 0x00196280 File Offset: 0x00194480
	[MethodImpl(256)]
	public static long Compute128To64(ulong a, ulong b)
	{
		ulong num = (b ^ a) * 11376068507788127593UL;
		num ^= num >> 47;
		ulong num2 = (a ^ num) * 11376068507788127593UL;
		return (long)((num2 ^ num2 >> 47) * 11376068507788127593UL);
	}

	// Token: 0x06004E51 RID: 20049 RVA: 0x001962BE File Offset: 0x001944BE
	[MethodImpl(256)]
	public static int ComputeTriple32(int i)
	{
		int num = i + 1;
		int num2 = (num ^ (int)((uint)num >> 17)) * -312814405;
		int num3 = (num2 ^ (int)((uint)num2 >> 11)) * -1404298415;
		int num4 = (num3 ^ (int)((uint)num3 >> 15)) * 830770091;
		return num4 ^ (int)((uint)num4 >> 14);
	}

	// Token: 0x06004E52 RID: 20050 RVA: 0x001962EC File Offset: 0x001944EC
	[MethodImpl(256)]
	public static int ReverseTriple32(int i)
	{
		uint num = (uint)(i ^ (int)((uint)i >> 14 ^ (uint)i >> 28));
		num *= 850532099U;
		num ^= (num >> 15 ^ num >> 30);
		num *= 1184763313U;
		num ^= (num >> 11 ^ num >> 22);
		num *= 2041073779U;
		num ^= num >> 17;
		return (int)(num - 1U);
	}

	// Token: 0x06004E53 RID: 20051 RVA: 0x00196344 File Offset: 0x00194544
	[MethodImpl(256)]
	private static void Mix(ref uint a, ref uint b, ref uint c)
	{
		a -= c;
		a ^= StaticHash.Rotate(c, 4);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 6);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 8);
		b += a;
		a -= c;
		a ^= StaticHash.Rotate(c, 16);
		c += b;
		b -= a;
		b ^= StaticHash.Rotate(a, 19);
		a += c;
		c -= b;
		c ^= StaticHash.Rotate(b, 4);
		b += a;
	}

	// Token: 0x06004E54 RID: 20052 RVA: 0x001963F8 File Offset: 0x001945F8
	[MethodImpl(256)]
	private static void Finalize(ref uint a, ref uint b, ref uint c)
	{
		c ^= b;
		c -= StaticHash.Rotate(b, 14);
		a ^= c;
		a -= StaticHash.Rotate(c, 11);
		b ^= a;
		b -= StaticHash.Rotate(a, 25);
		c ^= b;
		c -= StaticHash.Rotate(b, 16);
		a ^= c;
		a -= StaticHash.Rotate(c, 4);
		b ^= a;
		b -= StaticHash.Rotate(a, 14);
		c ^= b;
		c -= StaticHash.Rotate(b, 24);
	}

	// Token: 0x06004E55 RID: 20053 RVA: 0x00196497 File Offset: 0x00194697
	[MethodImpl(256)]
	private static uint Rotate(uint x, int k)
	{
		return x << k | x >> 32 - k;
	}

	// Token: 0x02000C83 RID: 3203
	[StructLayout(2)]
	private struct SingleInt32
	{
		// Token: 0x04005D50 RID: 23888
		[FieldOffset(0)]
		public float single;

		// Token: 0x04005D51 RID: 23889
		[FieldOffset(0)]
		public int int32;
	}

	// Token: 0x02000C84 RID: 3204
	[StructLayout(2)]
	private struct DoubleInt64
	{
		// Token: 0x04005D52 RID: 23890
		[FieldOffset(0)]
		public double @double;

		// Token: 0x04005D53 RID: 23891
		[FieldOffset(0)]
		public long int64;
	}
}
