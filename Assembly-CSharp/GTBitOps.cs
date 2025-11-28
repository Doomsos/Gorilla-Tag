using System;
using System.Runtime.CompilerServices;

// Token: 0x020002C6 RID: 710
public static class GTBitOps
{
	// Token: 0x06001170 RID: 4464 RVA: 0x0005C655 File Offset: 0x0005A855
	[MethodImpl(256)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x06001171 RID: 4465 RVA: 0x0005C65F File Offset: 0x0005A85F
	[MethodImpl(256)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x06001172 RID: 4466 RVA: 0x0005C668 File Offset: 0x0005A868
	[MethodImpl(256)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x06001173 RID: 4467 RVA: 0x0005C678 File Offset: 0x0005A878
	[MethodImpl(256)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return bits >> index & valueMask;
	}

	// Token: 0x06001174 RID: 4468 RVA: 0x0005C682 File Offset: 0x0005A882
	[MethodImpl(256)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return bits >> info.index & info.valueMask;
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x0005C696 File Offset: 0x0005A896
	[MethodImpl(256)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return bits >> index & (1 << count) - 1;
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x0005C6A7 File Offset: 0x0005A8A7
	[MethodImpl(256)]
	public static bool ReadBit(int bits, int index)
	{
		return (bits >> index & 1) == 1;
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x0005C6B4 File Offset: 0x0005A8B4
	[MethodImpl(256)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = ((bits & info.clearMask) | (value & info.valueMask) << info.index);
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x0005C6D4 File Offset: 0x0005A8D4
	[MethodImpl(256)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0005C6E0 File Offset: 0x0005A8E0
	[MethodImpl(256)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = ((bits & clearMask) | (value & valueMask) << index);
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x0005C6F2 File Offset: 0x0005A8F2
	[MethodImpl(256)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0005C701 File Offset: 0x0005A901
	[MethodImpl(256)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = ((bits & ~((1 << count) - 1 << index)) | (value & (1 << count) - 1) << index);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0005C726 File Offset: 0x0005A926
	[MethodImpl(256)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0005C733 File Offset: 0x0005A933
	[MethodImpl(256)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = ((bits & ~(1 << index)) | (value ? 1 : 0) << index);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0005C74E File Offset: 0x0005A94E
	[MethodImpl(256)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0005C75A File Offset: 0x0005A95A
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x020002C7 RID: 711
	public readonly struct BitWriteInfo
	{
		// Token: 0x06001180 RID: 4480 RVA: 0x0005C76C File Offset: 0x0005A96C
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04001606 RID: 5638
		public readonly int index;

		// Token: 0x04001607 RID: 5639
		public readonly int valueMask;

		// Token: 0x04001608 RID: 5640
		public readonly int clearMask;
	}
}
