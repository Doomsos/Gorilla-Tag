using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02001141 RID: 4417
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06006F75 RID: 28533 RVA: 0x00245272 File Offset: 0x00243472
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06006F76 RID: 28534 RVA: 0x00245280 File Offset: 0x00243480
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06006F77 RID: 28535 RVA: 0x00245288 File Offset: 0x00243488
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06006F78 RID: 28536 RVA: 0x002452C4 File Offset: 0x002434C4
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06006F79 RID: 28537 RVA: 0x002452F8 File Offset: 0x002434F8
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06006F7A RID: 28538 RVA: 0x00245314 File Offset: 0x00243514
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06006F7B RID: 28539 RVA: 0x00245321 File Offset: 0x00243521
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06006F7C RID: 28540 RVA: 0x0024532E File Offset: 0x0024352E
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06006F7D RID: 28541 RVA: 0x0024533B File Offset: 0x0024353B
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06006F7E RID: 28542 RVA: 0x0024534D File Offset: 0x0024354D
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06006F7F RID: 28543 RVA: 0x0024535F File Offset: 0x0024355F
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06006F80 RID: 28544 RVA: 0x00245374 File Offset: 0x00243574
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x00245389 File Offset: 0x00243589
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06006F82 RID: 28546 RVA: 0x00245393 File Offset: 0x00243593
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06006F83 RID: 28547 RVA: 0x002453A0 File Offset: 0x002435A0
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x002453A8 File Offset: 0x002435A8
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06006F85 RID: 28549 RVA: 0x002453B8 File Offset: 0x002435B8
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06006F86 RID: 28550 RVA: 0x002453E8 File Offset: 0x002435E8
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06006F87 RID: 28551 RVA: 0x00245430 File Offset: 0x00243630
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06006F88 RID: 28552 RVA: 0x0024545C File Offset: 0x0024365C
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06006F89 RID: 28553 RVA: 0x00245495 File Offset: 0x00243695
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x06006F8A RID: 28554 RVA: 0x002454A8 File Offset: 0x002436A8
		public int CompareTo(Half value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			if (this != value)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(value))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06006F8B RID: 28555 RVA: 0x00245500 File Offset: 0x00243700
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Half))
			{
				throw new ArgumentException("The argument value must be a SlimMath.Half.");
			}
			Half half = (Half)value;
			if (this < half)
			{
				return -1;
			}
			if (this > half)
			{
				return 1;
			}
			if (this != half)
			{
				if (!Half.IsNaN(this))
				{
					return 1;
				}
				if (!Half.IsNaN(half))
				{
					return -1;
				}
			}
			return 0;
		}

		// Token: 0x06006F8C RID: 28556 RVA: 0x00245574 File Offset: 0x00243774
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06006F8D RID: 28557 RVA: 0x00245584 File Offset: 0x00243784
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06006F8E RID: 28558 RVA: 0x00245594 File Offset: 0x00243794
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06006F8F RID: 28559 RVA: 0x002455C6 File Offset: 0x002437C6
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06006F90 RID: 28560 RVA: 0x002455D7 File Offset: 0x002437D7
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06006F91 RID: 28561 RVA: 0x002455E9 File Offset: 0x002437E9
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06006F92 RID: 28562 RVA: 0x002455FB File Offset: 0x002437FB
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06006F93 RID: 28563 RVA: 0x00245607 File Offset: 0x00243807
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06006F94 RID: 28564 RVA: 0x00245613 File Offset: 0x00243813
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06006F95 RID: 28565 RVA: 0x00245625 File Offset: 0x00243825
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06006F96 RID: 28566 RVA: 0x00245637 File Offset: 0x00243837
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06006F97 RID: 28567 RVA: 0x00245649 File Offset: 0x00243849
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06006F98 RID: 28568 RVA: 0x0024565B File Offset: 0x0024385B
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x0024566D File Offset: 0x0024386D
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06006F9A RID: 28570 RVA: 0x0024567F File Offset: 0x0024387F
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06006F9B RID: 28571 RVA: 0x0024568C File Offset: 0x0024388C
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return this.ToType(type, provider);
		}

		// Token: 0x06006F9C RID: 28572 RVA: 0x002456A6 File Offset: 0x002438A6
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06006F9D RID: 28573 RVA: 0x002456B8 File Offset: 0x002438B8
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06006F9E RID: 28574 RVA: 0x002456CA File Offset: 0x002438CA
		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04008009 RID: 32777
		private ushort value;

		// Token: 0x0400800A RID: 32778
		public const int PrecisionDigits = 3;

		// Token: 0x0400800B RID: 32779
		public const int MantissaBits = 11;

		// Token: 0x0400800C RID: 32780
		public const int MaximumDecimalExponent = 4;

		// Token: 0x0400800D RID: 32781
		public const int MaximumBinaryExponent = 15;

		// Token: 0x0400800E RID: 32782
		public const int MinimumDecimalExponent = -4;

		// Token: 0x0400800F RID: 32783
		public const int MinimumBinaryExponent = -14;

		// Token: 0x04008010 RID: 32784
		public const int ExponentRadix = 2;

		// Token: 0x04008011 RID: 32785
		public const int AdditionRounding = 1;

		// Token: 0x04008012 RID: 32786
		public static readonly Half Epsilon = new Half(0.0004887581f);

		// Token: 0x04008013 RID: 32787
		public static readonly Half MaxValue = new Half(65504f);

		// Token: 0x04008014 RID: 32788
		public static readonly Half MinValue = new Half(6.103516E-05f);

		// Token: 0x04008015 RID: 32789
		public static readonly Half NaN = new Half(float.NaN);

		// Token: 0x04008016 RID: 32790
		public static readonly Half NegativeInfinity = new Half(float.NegativeInfinity);

		// Token: 0x04008017 RID: 32791
		public static readonly Half PositiveInfinity = new Half(float.PositiveInfinity);
	}
}
