using System;

// Token: 0x020009CF RID: 2511
public static class EnumUtilExt
{
	// Token: 0x06004021 RID: 16417 RVA: 0x001584BB File Offset: 0x001566BB
	public static string GetName<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToName[e];
	}

	// Token: 0x06004022 RID: 16418 RVA: 0x001584DF File Offset: 0x001566DF
	public static int GetIndex<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToIndex[e];
	}

	// Token: 0x06004023 RID: 16419 RVA: 0x00158503 File Offset: 0x00156703
	public static long GetLongValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		return EnumData<TEnum>.Shared.EnumToLong[e];
	}

	// Token: 0x06004024 RID: 16420 RVA: 0x00158604 File Offset: 0x00156804
	public static TEnum GetNextValue<TEnum>(this TEnum e) where TEnum : struct, Enum
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		return shared.Values[shared.EnumToIndex[e] + 1 % shared.Values.Length];
	}
}
