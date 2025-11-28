using System;
using UnityEngine;

// Token: 0x020009F5 RID: 2549
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x0600412C RID: 16684 RVA: 0x0015B44C File Offset: 0x0015964C
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x0600412D RID: 16685 RVA: 0x0015B454 File Offset: 0x00159654
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x0600412E RID: 16686 RVA: 0x0015B44C File Offset: 0x0015964C
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x0600412F RID: 16687 RVA: 0x0015B472 File Offset: 0x00159672
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x0015B491 File Offset: 0x00159691
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x0015B4A0 File Offset: 0x001596A0
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x06004132 RID: 16690 RVA: 0x0015B4DA File Offset: 0x001596DA
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x04005220 RID: 21024
	[SerializeField]
	private TEnum m_EnumValue;
}
