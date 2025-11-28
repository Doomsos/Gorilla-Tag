using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009D9 RID: 2521
[Serializable]
public struct GTSturdyEnum<TEnum> : ISerializationCallbackReceiver where TEnum : struct, Enum
{
	// Token: 0x170005F5 RID: 1525
	// (get) Token: 0x0600403E RID: 16446 RVA: 0x00158C09 File Offset: 0x00156E09
	// (set) Token: 0x0600403F RID: 16447 RVA: 0x00158C11 File Offset: 0x00156E11
	public TEnum Value { readonly get; private set; }

	// Token: 0x06004040 RID: 16448 RVA: 0x00158C1C File Offset: 0x00156E1C
	public static implicit operator GTSturdyEnum<TEnum>(TEnum value)
	{
		return new GTSturdyEnum<TEnum>
		{
			Value = value
		};
	}

	// Token: 0x06004041 RID: 16449 RVA: 0x00158C3A File Offset: 0x00156E3A
	public static implicit operator TEnum(GTSturdyEnum<TEnum> sturdyEnum)
	{
		return sturdyEnum.Value;
	}

	// Token: 0x06004042 RID: 16450 RVA: 0x00158C44 File Offset: 0x00156E44
	public void OnBeforeSerialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (!shared.IsBitMaskCompatible)
		{
			this.m_stringValuePairs = new GTSturdyEnum<TEnum>.EnumPair[1];
			GTSturdyEnum<TEnum>.EnumPair[] stringValuePairs = this.m_stringValuePairs;
			int num = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum value = this.Value;
			enumPair.Name = value.ToString();
			enumPair.FallbackValue = this.Value;
			stringValuePairs[num] = enumPair;
			return;
		}
		long num2 = Convert.ToInt64(this.Value);
		if (num2 == 0L)
		{
			GTSturdyEnum<TEnum>.EnumPair[] array = new GTSturdyEnum<TEnum>.EnumPair[1];
			int num3 = 0;
			GTSturdyEnum<TEnum>.EnumPair enumPair = default(GTSturdyEnum<TEnum>.EnumPair);
			TEnum value = this.Value;
			enumPair.Name = value.ToString();
			enumPair.FallbackValue = this.Value;
			array[num3] = enumPair;
			this.m_stringValuePairs = array;
			return;
		}
		List<GTSturdyEnum<TEnum>.EnumPair> list = new List<GTSturdyEnum<TEnum>.EnumPair>(shared.Values.Length);
		for (int i = 0; i < shared.Values.Length; i++)
		{
			long num4 = shared.LongValues[i];
			if (num4 != 0L && (num2 & num4) == num4)
			{
				TEnum fallbackValue = shared.Values[i];
				list.Add(new GTSturdyEnum<TEnum>.EnumPair
				{
					Name = fallbackValue.ToString(),
					FallbackValue = fallbackValue
				});
			}
		}
		this.m_stringValuePairs = list.ToArray();
	}

	// Token: 0x06004043 RID: 16451 RVA: 0x00158D8C File Offset: 0x00156F8C
	public void OnAfterDeserialize()
	{
		EnumData<TEnum> shared = EnumData<TEnum>.Shared;
		if (this.m_stringValuePairs == null || this.m_stringValuePairs.Length == 0)
		{
			if (shared.IsBitMaskCompatible)
			{
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), 0L));
				return;
			}
			this.Value = default(TEnum);
			return;
		}
		else
		{
			if (shared.IsBitMaskCompatible)
			{
				long num = 0L;
				foreach (GTSturdyEnum<TEnum>.EnumPair enumPair in this.m_stringValuePairs)
				{
					TEnum tenum;
					long num2;
					if (shared.NameToEnum.TryGetValue(enumPair.Name, ref tenum))
					{
						num |= shared.EnumToLong[tenum];
					}
					else if (shared.EnumToLong.TryGetValue(enumPair.FallbackValue, ref num2))
					{
						num |= num2;
					}
				}
				this.Value = (TEnum)((object)Enum.ToObject(typeof(TEnum), num));
				return;
			}
			GTSturdyEnum<TEnum>.EnumPair enumPair2 = this.m_stringValuePairs[0];
			TEnum value;
			if (shared.NameToEnum.TryGetValue(enumPair2.Name, ref value))
			{
				this.Value = value;
				return;
			}
			this.Value = enumPair2.FallbackValue;
			return;
		}
	}

	// Token: 0x04005182 RID: 20866
	[SerializeField]
	private GTSturdyEnum<TEnum>.EnumPair[] m_stringValuePairs;

	// Token: 0x020009DA RID: 2522
	[Serializable]
	private struct EnumPair
	{
		// Token: 0x04005183 RID: 20867
		public string Name;

		// Token: 0x04005184 RID: 20868
		public TEnum FallbackValue;
	}
}
