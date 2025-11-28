using System;
using Unity.Mathematics;

// Token: 0x020002FE RID: 766
[Serializable]
public struct GTSimpleNameID
{
	// Token: 0x060012BF RID: 4799 RVA: 0x00061E6C File Offset: 0x0006006C
	static GTSimpleNameID()
	{
		if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".Length != 64 || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(0) != '0' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(9) != '9' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(10) != 'A' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(36) != 'a' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(62) != '_' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars(63) != '-')
		{
			throw new Exception("GTSimpleNameID: The constant string `_k_possibleChars` does not match the expected format. Did you change something without updating the logic?");
		}
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00061EF4 File Offset: 0x000600F4
	public unsafe static GTSimpleNameID FromString(string input)
	{
		if (input == null)
		{
			input = string.Empty;
		}
		GTSimpleNameID result = default(GTSimpleNameID);
		int num = math.min(input.Length, 41);
		result.U0 = (ulong)((long)num & 63L);
		int num2 = 6;
		int i = 0;
		while (i < num)
		{
			char c = input.get_Chars(i);
			byte b;
			if (c >= 'A')
			{
				if (c >= 'a')
				{
					if (c > 'z')
					{
						goto IL_A7;
					}
					b = (byte)(c - 'a' + '$');
				}
				else if (c > 'Z')
				{
					if (c != '_')
					{
						goto IL_A7;
					}
					b = 62;
				}
				else
				{
					b = (byte)(c - 'A' + '\n');
				}
			}
			else if (c >= '0')
			{
				if (c > '9')
				{
					goto IL_A7;
				}
				b = (byte)(c - '0');
			}
			else
			{
				if (c != '-')
				{
					goto IL_A7;
				}
				b = 63;
			}
			ulong num3 = (ulong)b;
			int num4 = num2 + i * 6;
			ulong* ptr = &result.U0;
			int num5 = num4 / 64;
			int num6 = num4 % 64;
			ulong num7 = 63UL;
			ulong num8 = num3 & num7;
			ulong num9 = ~(num7 << num6);
			ptr[num5] &= num9;
			ptr[num5] |= num8 << num6;
			int num10 = 64 - num6;
			if (num10 < 6 && num5 < 3)
			{
				int num11 = 6 - num10;
				ulong num12 = (1UL << num11) - 1UL;
				ulong num13 = num8 >> num10;
				ptr[num5 + 1] &= ~num12;
				ptr[num5 + 1] |= num13;
			}
			i++;
			continue;
			IL_A7:
			throw new ArgumentException(string.Format("Invalid character '{0}' in input string.", c), "input");
		}
		return result;
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00062074 File Offset: 0x00060274
	public override string ToString()
	{
		int num = math.min((int)(this.U0 & 63UL), 41);
		char[] array = new char[num];
		int num2 = 6;
		for (int i = 0; i < num; i++)
		{
			int bitOffset = num2 + i * 6;
			ulong num3 = GTSimpleNameID._Read6Bits(this, bitOffset);
			array[i] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".get_Chars((int)num3);
		}
		return new string(array);
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x000620D0 File Offset: 0x000602D0
	private unsafe static ulong _Read6Bits(in GTSimpleNameID cv, int bitOffset)
	{
		fixed (ulong* ptr = &cv.U0)
		{
			ulong* ptr2 = ptr;
			int num = bitOffset / 64;
			int num2 = bitOffset % 64;
			ulong num3 = ptr2[num] >> num2;
			int num4 = 64 - num2;
			if (num4 < 6 && num < 3)
			{
				int num5 = 6 - num4;
				ulong num6 = (1UL << num5) - 1UL;
				ulong num7 = ptr2[num + 1] & num6;
				num7 <<= num4;
				num3 |= num7;
			}
			return num3 & 63UL;
		}
	}

	// Token: 0x04001748 RID: 5960
	public ulong U0;

	// Token: 0x04001749 RID: 5961
	public ulong U1;

	// Token: 0x0400174A RID: 5962
	public ulong U2;

	// Token: 0x0400174B RID: 5963
	public ulong U3;

	// Token: 0x0400174C RID: 5964
	private const string _k_possibleChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";

	// Token: 0x0400174D RID: 5965
	private const int _k_maxLength = 41;

	// Token: 0x0400174E RID: 5966
	private const ulong _k_bitmask6Bits = 63UL;

	// Token: 0x0400174F RID: 5967
	private const ushort _k_indexOf_A = 10;

	// Token: 0x04001750 RID: 5968
	private const ushort _k_indexOf_a = 36;

	// Token: 0x04001751 RID: 5969
	private const ushort _k_indexOf_underscore = 62;

	// Token: 0x04001752 RID: 5970
	private const ushort _k_indexOf_hyphen = 63;
}
