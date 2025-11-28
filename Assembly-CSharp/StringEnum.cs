using System;
using UnityEngine;

// Token: 0x020009F5 RID: 2549
[Serializable]
public struct StringEnum<TEnum> where TEnum : struct, Enum
{
	// Token: 0x1700060A RID: 1546
	// (get) Token: 0x0600412C RID: 16684 RVA: 0x0015B46C File Offset: 0x0015966C
	public TEnum Value
	{
		get
		{
			return this.m_EnumValue;
		}
	}

	// Token: 0x0600412D RID: 16685 RVA: 0x0015B474 File Offset: 0x00159674
	public static implicit operator StringEnum<TEnum>(TEnum e)
	{
		return new StringEnum<TEnum>
		{
			m_EnumValue = e
		};
	}

	// Token: 0x0600412E RID: 16686 RVA: 0x0015B46C File Offset: 0x0015966C
	public static implicit operator TEnum(StringEnum<TEnum> se)
	{
		return se.m_EnumValue;
	}

	// Token: 0x0600412F RID: 16687 RVA: 0x0015B492 File Offset: 0x00159692
	public static bool operator ==(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return left.m_EnumValue.Equals(right.m_EnumValue);
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x0015B4B1 File Offset: 0x001596B1
	public static bool operator !=(StringEnum<TEnum> left, StringEnum<TEnum> right)
	{
		return !(left == right);
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x0015B4C0 File Offset: 0x001596C0
	public override bool Equals(object obj)
	{
		if (obj is StringEnum<TEnum>)
		{
			StringEnum<TEnum> stringEnum = (StringEnum<TEnum>)obj;
			return this.m_EnumValue.Equals(stringEnum.m_EnumValue);
		}
		return false;
	}

	// Token: 0x06004132 RID: 16690 RVA: 0x0015B4FA File Offset: 0x001596FA
	public override int GetHashCode()
	{
		return this.m_EnumValue.GetHashCode();
	}

	// Token: 0x04005220 RID: 21024
	[SerializeField]
	private TEnum m_EnumValue;
}
