using System;
using emotitron.Compression.HalfFloat;
using emotitron.Compression.Utilities;

namespace emotitron.Compression
{
	// Token: 0x02001138 RID: 4408
	public static class PrimitiveSerializeExt
	{
		// Token: 0x06006ECF RID: 28367 RVA: 0x002440B2 File Offset: 0x002422B2
		public static void Inject(this ByteConverter value, ref ulong buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED0 RID: 28368 RVA: 0x002440C2 File Offset: 0x002422C2
		public static void Inject(this ByteConverter value, ref uint buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED1 RID: 28369 RVA: 0x002440D2 File Offset: 0x002422D2
		public static void Inject(this ByteConverter value, ref ushort buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED2 RID: 28370 RVA: 0x002440E2 File Offset: 0x002422E2
		public static void Inject(this ByteConverter value, ref byte buffer, ref int bitposition, int bits)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED3 RID: 28371 RVA: 0x002440F4 File Offset: 0x002422F4
		public static ulong WriteSigned(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006ED4 RID: 28372 RVA: 0x00244114 File Offset: 0x00242314
		public static void InjectSigned(this long value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED5 RID: 28373 RVA: 0x00244127 File Offset: 0x00242327
		public static void InjectSigned(this int value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED6 RID: 28374 RVA: 0x00244127 File Offset: 0x00242327
		public static void InjectSigned(this short value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED7 RID: 28375 RVA: 0x00244127 File Offset: 0x00242327
		public static void InjectSigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006ED8 RID: 28376 RVA: 0x0024413C File Offset: 0x0024233C
		public static int ReadSigned(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006ED9 RID: 28377 RVA: 0x00244160 File Offset: 0x00242360
		public static uint WriteSigned(this uint buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006EDA RID: 28378 RVA: 0x00244180 File Offset: 0x00242380
		public static void InjectSigned(this long value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EDB RID: 28379 RVA: 0x00244193 File Offset: 0x00242393
		public static void InjectSigned(this int value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EDC RID: 28380 RVA: 0x00244193 File Offset: 0x00242393
		public static void InjectSigned(this short value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EDD RID: 28381 RVA: 0x00244193 File Offset: 0x00242393
		public static void InjectSigned(this sbyte value, ref uint buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EDE RID: 28382 RVA: 0x002441A8 File Offset: 0x002423A8
		public static int ReadSigned(this uint buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006EDF RID: 28383 RVA: 0x002441CC File Offset: 0x002423CC
		public static ushort WriteSigned(this ushort buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006EE0 RID: 28384 RVA: 0x002441EC File Offset: 0x002423EC
		public static void InjectSigned(this long value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE1 RID: 28385 RVA: 0x002441FF File Offset: 0x002423FF
		public static void InjectSigned(this int value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE2 RID: 28386 RVA: 0x002441FF File Offset: 0x002423FF
		public static void InjectSigned(this short value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE3 RID: 28387 RVA: 0x002441FF File Offset: 0x002423FF
		public static void InjectSigned(this sbyte value, ref ushort buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE4 RID: 28388 RVA: 0x00244214 File Offset: 0x00242414
		public static int ReadSigned(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006EE5 RID: 28389 RVA: 0x00244238 File Offset: 0x00242438
		public static byte WriteSigned(this byte buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06006EE6 RID: 28390 RVA: 0x00244258 File Offset: 0x00242458
		public static void InjectSigned(this long value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE7 RID: 28391 RVA: 0x0024426B File Offset: 0x0024246B
		public static void InjectSigned(this int value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)(value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE8 RID: 28392 RVA: 0x0024426B File Offset: 0x0024246B
		public static void InjectSigned(this short value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EE9 RID: 28393 RVA: 0x0024426B File Offset: 0x0024246B
		public static void InjectSigned(this sbyte value, ref byte buffer, ref int bitposition, int bits)
		{
			((uint)((int)value << 1 ^ value >> 31)).Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006EEA RID: 28394 RVA: 0x00244280 File Offset: 0x00242480
		public static int ReadSigned(this byte buffer, ref int bitposition, int bits)
		{
			uint num = buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06006EEB RID: 28395 RVA: 0x002442A1 File Offset: 0x002424A1
		public static ulong WritetBool(this ulong buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06006EEC RID: 28396 RVA: 0x002442B3 File Offset: 0x002424B3
		public static uint WritetBool(this uint buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06006EED RID: 28397 RVA: 0x002442C5 File Offset: 0x002424C5
		public static ushort WritetBool(this ushort buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06006EEE RID: 28398 RVA: 0x002442D7 File Offset: 0x002424D7
		public static byte WritetBool(this byte buffer, bool value, ref int bitposition)
		{
			return buffer.Write((ulong)(value ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x06006EEF RID: 28399 RVA: 0x002442E9 File Offset: 0x002424E9
		public static void Inject(this bool value, ref ulong buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06006EF0 RID: 28400 RVA: 0x002442FB File Offset: 0x002424FB
		public static void Inject(this bool value, ref uint buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06006EF1 RID: 28401 RVA: 0x0024430D File Offset: 0x0024250D
		public static void Inject(this bool value, ref ushort buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06006EF2 RID: 28402 RVA: 0x0024431F File Offset: 0x0024251F
		public static void Inject(this bool value, ref byte buffer, ref int bitposition)
		{
			((ulong)(value ? 1L : 0L)).Inject(ref buffer, ref bitposition, 1);
		}

		// Token: 0x06006EF3 RID: 28403 RVA: 0x00244331 File Offset: 0x00242531
		public static bool ReadBool(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0UL;
		}

		// Token: 0x06006EF4 RID: 28404 RVA: 0x00244340 File Offset: 0x00242540
		public static bool ReadtBool(this uint buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06006EF5 RID: 28405 RVA: 0x0024434F File Offset: 0x0024254F
		public static bool ReadBool(this ushort buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06006EF6 RID: 28406 RVA: 0x0024435E File Offset: 0x0024255E
		public static bool ReadBool(this byte buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) != 0U;
		}

		// Token: 0x06006EF7 RID: 28407 RVA: 0x00244370 File Offset: 0x00242570
		public static ulong Write(this ulong buffer, ulong value, ref int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06006EF8 RID: 28408 RVA: 0x002443AC File Offset: 0x002425AC
		public static uint Write(this uint buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = uint.MaxValue >> 32 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06006EF9 RID: 28409 RVA: 0x002443E8 File Offset: 0x002425E8
		public static ushort Write(this ushort buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 65535U >> 16 - bits << bitposition;
			buffer = (ushort)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06006EFA RID: 28410 RVA: 0x00244424 File Offset: 0x00242624
		public static byte Write(this byte buffer, ulong value, ref int bitposition, int bits = 64)
		{
			uint num = (uint)value << bitposition;
			uint num2 = 255U >> 8 - bits << bitposition;
			buffer = (byte)(((uint)buffer & ~num2) | (num2 & num));
			bitposition += bits;
			return buffer;
		}

		// Token: 0x06006EFB RID: 28411 RVA: 0x0024445F File Offset: 0x0024265F
		public static void Inject(this ulong value, ref ulong buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006EFC RID: 28412 RVA: 0x00244470 File Offset: 0x00242670
		public static void Inject(this ulong value, ref ulong buffer, int bitposition, int bits = 64)
		{
			ulong num = value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
		}

		// Token: 0x06006EFD RID: 28413 RVA: 0x002444A3 File Offset: 0x002426A3
		public static void Inject(this uint value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006EFE RID: 28414 RVA: 0x002444B4 File Offset: 0x002426B4
		public static void Inject(this uint value, ref ulong buffer, int bitposition, int bits = 32)
		{
			ulong num = (ulong)value << bitposition;
			ulong num2 = ulong.MaxValue >> 64 - bits << bitposition;
			buffer &= ~num2;
			buffer |= (num2 & num);
		}

		// Token: 0x06006EFF RID: 28415 RVA: 0x002444A3 File Offset: 0x002426A3
		public static void Inject(this ushort value, ref ulong buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F00 RID: 28416 RVA: 0x002444E8 File Offset: 0x002426E8
		public static void Inject(this ushort value, ref ulong buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F01 RID: 28417 RVA: 0x002444A3 File Offset: 0x002426A3
		public static void Inject(this byte value, ref ulong buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F02 RID: 28418 RVA: 0x002444E8 File Offset: 0x002426E8
		public static void Inject(this byte value, ref ulong buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F03 RID: 28419 RVA: 0x0024445F File Offset: 0x0024265F
		public static void InjectUnsigned(this long value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F04 RID: 28420 RVA: 0x002444F8 File Offset: 0x002426F8
		public static void InjectUnsigned(this int value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F05 RID: 28421 RVA: 0x002444F8 File Offset: 0x002426F8
		public static void InjectUnsigned(this short value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F06 RID: 28422 RVA: 0x002444F8 File Offset: 0x002426F8
		public static void InjectUnsigned(this sbyte value, ref ulong buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F07 RID: 28423 RVA: 0x00244507 File Offset: 0x00242707
		public static void Inject(this ulong value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F08 RID: 28424 RVA: 0x00244515 File Offset: 0x00242715
		public static void Inject(this ulong value, ref uint buffer, int bitposition, int bits = 64)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F09 RID: 28425 RVA: 0x00244524 File Offset: 0x00242724
		public static void Inject(this uint value, ref uint buffer, ref int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0A RID: 28426 RVA: 0x00244533 File Offset: 0x00242733
		public static void Inject(this uint value, ref uint buffer, int bitposition, int bits = 32)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0B RID: 28427 RVA: 0x00244524 File Offset: 0x00242724
		public static void Inject(this ushort value, ref uint buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0C RID: 28428 RVA: 0x00244533 File Offset: 0x00242733
		public static void Inject(this ushort value, ref uint buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0D RID: 28429 RVA: 0x00244524 File Offset: 0x00242724
		public static void Inject(this byte value, ref uint buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0E RID: 28430 RVA: 0x00244533 File Offset: 0x00242733
		public static void Inject(this byte value, ref uint buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F0F RID: 28431 RVA: 0x00244507 File Offset: 0x00242707
		public static void InjectUnsigned(this long value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F10 RID: 28432 RVA: 0x00244543 File Offset: 0x00242743
		public static void InjectUnsigned(this int value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F11 RID: 28433 RVA: 0x00244543 File Offset: 0x00242743
		public static void InjectUnsigned(this short value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F12 RID: 28434 RVA: 0x00244543 File Offset: 0x00242743
		public static void InjectUnsigned(this sbyte value, ref uint buffer, ref int bitposition, int bits = 64)
		{
			buffer = buffer.Write((ulong)((long)value), ref bitposition, bits);
		}

		// Token: 0x06006F13 RID: 28435 RVA: 0x00244552 File Offset: 0x00242752
		public static void Inject(this ulong value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F14 RID: 28436 RVA: 0x00244560 File Offset: 0x00242760
		public static void Inject(this ulong value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F15 RID: 28437 RVA: 0x0024456F File Offset: 0x0024276F
		public static void Inject(this uint value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F16 RID: 28438 RVA: 0x0024457E File Offset: 0x0024277E
		public static void Inject(this uint value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F17 RID: 28439 RVA: 0x0024456F File Offset: 0x0024276F
		public static void Inject(this ushort value, ref ushort buffer, ref int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F18 RID: 28440 RVA: 0x0024457E File Offset: 0x0024277E
		public static void Inject(this ushort value, ref ushort buffer, int bitposition, int bits = 16)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F19 RID: 28441 RVA: 0x0024456F File Offset: 0x0024276F
		public static void Inject(this byte value, ref ushort buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F1A RID: 28442 RVA: 0x0024457E File Offset: 0x0024277E
		public static void Inject(this byte value, ref ushort buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F1B RID: 28443 RVA: 0x0024458E File Offset: 0x0024278E
		public static void Inject(this ulong value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F1C RID: 28444 RVA: 0x0024459C File Offset: 0x0024279C
		public static void Inject(this ulong value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write(value, ref bitposition, bits);
		}

		// Token: 0x06006F1D RID: 28445 RVA: 0x002445AB File Offset: 0x002427AB
		public static void Inject(this uint value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F1E RID: 28446 RVA: 0x002445BA File Offset: 0x002427BA
		public static void Inject(this uint value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F1F RID: 28447 RVA: 0x002445AB File Offset: 0x002427AB
		public static void Inject(this ushort value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F20 RID: 28448 RVA: 0x002445BA File Offset: 0x002427BA
		public static void Inject(this ushort value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F21 RID: 28449 RVA: 0x002445AB File Offset: 0x002427AB
		public static void Inject(this byte value, ref byte buffer, ref int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F22 RID: 28450 RVA: 0x002445BA File Offset: 0x002427BA
		public static void Inject(this byte value, ref byte buffer, int bitposition, int bits = 8)
		{
			buffer = buffer.Write((ulong)value, ref bitposition, bits);
		}

		// Token: 0x06006F23 RID: 28451 RVA: 0x002445CA File Offset: 0x002427CA
		[Obsolete("Argument order changed")]
		public static ulong Extract(this ulong value, int bits, ref int bitposition)
		{
			return value.Extract(bits, ref bitposition);
		}

		// Token: 0x06006F24 RID: 28452 RVA: 0x002445D4 File Offset: 0x002427D4
		public static ulong Read(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F25 RID: 28453 RVA: 0x002445FC File Offset: 0x002427FC
		[Obsolete("Use Read instead.")]
		public static ulong Extract(this ulong value, ref int bitposition, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			ulong result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F26 RID: 28454 RVA: 0x00244624 File Offset: 0x00242824
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static ulong Extract(this ulong value, int bits)
		{
			ulong num = ulong.MaxValue >> 64 - bits;
			return value & num;
		}

		// Token: 0x06006F27 RID: 28455 RVA: 0x00244640 File Offset: 0x00242840
		public static uint Read(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F28 RID: 28456 RVA: 0x00244668 File Offset: 0x00242868
		[Obsolete("Use Read instead.")]
		public static uint Extract(this uint value, ref int bitposition, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			uint result = value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F29 RID: 28457 RVA: 0x00244690 File Offset: 0x00242890
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static uint Extract(this uint value, int bits)
		{
			uint num = uint.MaxValue >> 32 - bits;
			return value & num;
		}

		// Token: 0x06006F2A RID: 28458 RVA: 0x002446AC File Offset: 0x002428AC
		public static uint Read(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F2B RID: 28459 RVA: 0x002446D8 File Offset: 0x002428D8
		[Obsolete("Use Read instead.")]
		public static uint Extract(this ushort value, ref int bitposition, int bits)
		{
			uint num = 65535U >> 16 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F2C RID: 28460 RVA: 0x00244704 File Offset: 0x00242904
		public static uint Read(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F2D RID: 28461 RVA: 0x00244730 File Offset: 0x00242930
		[Obsolete("Use Read instead.")]
		public static uint Extract(this byte value, ref int bitposition, int bits)
		{
			uint num = 255U >> 8 - bits;
			uint result = (uint)value >> bitposition & num;
			bitposition += bits;
			return result;
		}

		// Token: 0x06006F2E RID: 28462 RVA: 0x0024475C File Offset: 0x0024295C
		[Obsolete("Always include the [ref int bitposition] argument. Extracting from position 0 would be better handled with a mask operation.")]
		public static byte Extract(this byte value, int bits)
		{
			uint num = 255U >> 8 - bits;
			return (byte)((uint)value & num);
		}

		// Token: 0x06006F2F RID: 28463 RVA: 0x0024477A File Offset: 0x0024297A
		public static void Inject(this float f, ref ulong buffer, ref int bitposition)
		{
			buffer = buffer.Write(f, ref bitposition, 32);
		}

		// Token: 0x06006F30 RID: 28464 RVA: 0x00244793 File Offset: 0x00242993
		public static float ReadFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x06006F31 RID: 28465 RVA: 0x002447A8 File Offset: 0x002429A8
		[Obsolete("Use Read instead.")]
		public static float ExtractFloat(this ulong buffer, ref int bitposition)
		{
			return buffer.Extract(ref bitposition, 32);
		}

		// Token: 0x06006F32 RID: 28466 RVA: 0x002447C0 File Offset: 0x002429C0
		public static ushort InjectAsHalfFloat(this float f, ref ulong buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x06006F33 RID: 28467 RVA: 0x002447E4 File Offset: 0x002429E4
		public static ushort InjectAsHalfFloat(this float f, ref uint buffer, ref int bitposition)
		{
			ushort num = HalfUtilities.Pack(f);
			buffer = buffer.Write((ulong)num, ref bitposition, 16);
			return num;
		}

		// Token: 0x06006F34 RID: 28468 RVA: 0x00244807 File Offset: 0x00242A07
		public static float ReadHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x06006F35 RID: 28469 RVA: 0x00244818 File Offset: 0x00242A18
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this ulong buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x06006F36 RID: 28470 RVA: 0x00244829 File Offset: 0x00242A29
		public static float ReadHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Read(ref bitposition, 16));
		}

		// Token: 0x06006F37 RID: 28471 RVA: 0x0024483A File Offset: 0x00242A3A
		[Obsolete("Use Read instead.")]
		public static float ExtractHalfFloat(this uint buffer, ref int bitposition)
		{
			return HalfUtilities.Unpack((ushort)buffer.Extract(ref bitposition, 16));
		}

		// Token: 0x06006F38 RID: 28472 RVA: 0x0024484B File Offset: 0x00242A4B
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref uint buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x06006F39 RID: 28473 RVA: 0x00244856 File Offset: 0x00242A56
		[Obsolete("Argument order changed")]
		public static void Inject(this ulong value, ref ulong buffer, int bits, ref int bitposition)
		{
			value.Inject(ref buffer, ref bitposition, bits);
		}

		// Token: 0x04007FD6 RID: 32726
		private const string overrunerror = "Write buffer overrun. writepos + bits exceeds target length. Data loss will occur.";
	}
}
