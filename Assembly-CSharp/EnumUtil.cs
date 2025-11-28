using System;
using System.Collections.Generic;

// Token: 0x020009CE RID: 2510
public static class EnumUtil
{
	// Token: 0x06004010 RID: 16400 RVA: 0x00158488 File Offset: 0x00156688
	public static string[] GetNames<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<string>(EnumData<TEnum>.Shared.Names);
	}

	// Token: 0x06004011 RID: 16401 RVA: 0x00158499 File Offset: 0x00156699
	public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<TEnum>(EnumData<TEnum>.Shared.Values);
	}

	// Token: 0x06004012 RID: 16402 RVA: 0x001584AA File Offset: 0x001566AA
	public static long[] GetLongValues<TEnum>() where TEnum : struct, Enum
	{
		return ArrayUtils.Clone<long>(EnumData<TEnum>.Shared.LongValues);
	}

	// Token: 0x06004013 RID: 16403 RVA: 0x001584BB File Offset: 0x001566BB
	public static string EnumToName<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06004014 RID: 16404 RVA: 0x001584CD File Offset: 0x001566CD
	public static TEnum NameToEnum<TEnum>(string n) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[n];
	}

	// Token: 0x06004015 RID: 16405 RVA: 0x001584DF File Offset: 0x001566DF
	public static int EnumToIndex<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06004016 RID: 16406 RVA: 0x001584F1 File Offset: 0x001566F1
	public static TEnum IndexToEnum<TEnum>(int i) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.IndexToEnum[i];
	}

	// Token: 0x06004017 RID: 16407 RVA: 0x00158503 File Offset: 0x00156703
	public static long EnumToLong<TEnum>(TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06004018 RID: 16408 RVA: 0x00158515 File Offset: 0x00156715
	public static TEnum LongToEnum<TEnum>(long l) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[l];
	}

	// Token: 0x06004019 RID: 16409 RVA: 0x00158527 File Offset: 0x00156727
	public static TEnum GetValue<TEnum>(int index) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.Values[index];
	}

	// Token: 0x0600401A RID: 16410 RVA: 0x001584DF File Offset: 0x001566DF
	public static int GetIndex<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[value];
	}

	// Token: 0x0600401B RID: 16411 RVA: 0x001584BB File Offset: 0x001566BB
	public static string GetName<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[value];
	}

	// Token: 0x0600401C RID: 16412 RVA: 0x001584CD File Offset: 0x001566CD
	public static TEnum GetValue<TEnum>(string name) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.NameToEnum[name];
	}

	// Token: 0x0600401D RID: 16413 RVA: 0x00158503 File Offset: 0x00156703
	public static long GetLongValue<TEnum>(TEnum value) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[value];
	}

	// Token: 0x0600401E RID: 16414 RVA: 0x00158515 File Offset: 0x00156715
	public static TEnum GetValue<TEnum>(long longValue) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.LongToEnum[longValue];
	}

	// Token: 0x0600401F RID: 16415 RVA: 0x00158539 File Offset: 0x00156739
	public static TEnum[] SplitBitmask<TEnum>(TEnum bitmask) where TEnum : struct, Enum
	{
		return EnumUtil.SplitBitmask<TEnum>(Convert.ToInt64(bitmask));
	}

	// Token: 0x06004020 RID: 16416 RVA: 0x0015854C File Offset: 0x0015674C
	public static TEnum[] SplitBitmask<TEnum>(long bitmaskLong) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			throw new ArgumentException("The enum type " + typeof(TEnum).Name + " is not bitmask-compatible.");
		}
		if (bitmaskLong == 0L)
		{
			return new TEnum[]
			{
				(TEnum)((object)Enum.ToObject(typeof(TEnum), 0L))
			};
		}
		List<TEnum> list = new List<TEnum>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			TEnum tenum = shared.Values[i];
			long num = shared.LongValues[i];
			if (num != 0L && (bitmaskLong & num) == num)
			{
				list.Add(tenum);
			}
		}
		return list.ToArray();
	}
}
