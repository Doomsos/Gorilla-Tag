using System;
using System.Collections.Generic;

// Token: 0x020009CD RID: 2509
public class EnumData<TEnum> where TEnum : struct, Enum
{
	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x0600400D RID: 16397 RVA: 0x00158235 File Offset: 0x00156435
	public static EnumData<TEnum> Shared { get; } = new EnumData<TEnum>();

	// Token: 0x0600400E RID: 16398 RVA: 0x0015823C File Offset: 0x0015643C
	private EnumData()
	{
		this.Names = Enum.GetNames(typeof(TEnum));
		this.Values = (TEnum[])Enum.GetValues(typeof(TEnum));
		int num = this.Names.Length;
		this.LongValues = new long[num];
		this.EnumToName = new Dictionary<TEnum, string>(num);
		this.NameToEnum = new Dictionary<string, TEnum>(num * 2);
		this.EnumToIndex = new Dictionary<TEnum, int>(num);
		this.IndexToEnum = new Dictionary<int, TEnum>(num);
		this.EnumToLong = new Dictionary<TEnum, long>(num);
		this.LongToEnum = new Dictionary<long, TEnum>(num);
		long num2 = long.MaxValue;
		long num3 = long.MinValue;
		for (int i = 0; i < this.Names.Length; i++)
		{
			string text = this.Names[i];
			TEnum tenum = this.Values[i];
			long num4 = Convert.ToInt64(tenum);
			this.LongValues[i] = num4;
			this.EnumToName[tenum] = text;
			this.NameToEnum[text] = tenum;
			this.NameToEnum.TryAdd(text.ToLowerInvariant(), tenum);
			this.EnumToIndex[tenum] = i;
			this.IndexToEnum[i] = tenum;
			this.EnumToLong[tenum] = num4;
			this.LongToEnum[num4] = tenum;
			num2 = Math.Min(num4, num2);
			num3 = Math.Max(num4, num3);
		}
		for (int j = 0; j < this.Names.Length; j++)
		{
			string text2 = this.Names[j];
			TEnum tenum2 = this.Values[j];
			this.NameToEnum[text2] = tenum2;
		}
		this.MinValue = this.LongToEnum[num2];
		this.MaxValue = this.LongToEnum[num3];
		this.MinInt = Convert.ToInt32(num2);
		this.MaxInt = Convert.ToInt32(num3);
		this.MinLong = num2;
		this.MaxLong = num3;
		long num5 = 0L;
		bool isBitMaskCompatible = true;
		foreach (long num6 in this.LongValues)
		{
			if (num6 != 0L && (num6 & num6 - 1L) != 0L && (num5 & num6) != num6)
			{
				isBitMaskCompatible = false;
				break;
			}
			num5 |= num6;
		}
		this.IsBitMaskCompatible = isBitMaskCompatible;
	}

	// Token: 0x04005139 RID: 20793
	public readonly string[] Names;

	// Token: 0x0400513A RID: 20794
	public readonly TEnum[] Values;

	// Token: 0x0400513B RID: 20795
	public readonly long[] LongValues;

	// Token: 0x0400513C RID: 20796
	public readonly bool IsBitMaskCompatible;

	// Token: 0x0400513D RID: 20797
	public readonly Dictionary<TEnum, string> EnumToName;

	// Token: 0x0400513E RID: 20798
	public readonly Dictionary<string, TEnum> NameToEnum;

	// Token: 0x0400513F RID: 20799
	public readonly Dictionary<TEnum, int> EnumToIndex;

	// Token: 0x04005140 RID: 20800
	public readonly Dictionary<int, TEnum> IndexToEnum;

	// Token: 0x04005141 RID: 20801
	public readonly Dictionary<TEnum, long> EnumToLong;

	// Token: 0x04005142 RID: 20802
	public readonly Dictionary<long, TEnum> LongToEnum;

	// Token: 0x04005143 RID: 20803
	public readonly TEnum MinValue;

	// Token: 0x04005144 RID: 20804
	public readonly TEnum MaxValue;

	// Token: 0x04005145 RID: 20805
	public readonly int MinInt;

	// Token: 0x04005146 RID: 20806
	public readonly int MaxInt;

	// Token: 0x04005147 RID: 20807
	public readonly long MinLong;

	// Token: 0x04005148 RID: 20808
	public readonly long MaxLong;
}
