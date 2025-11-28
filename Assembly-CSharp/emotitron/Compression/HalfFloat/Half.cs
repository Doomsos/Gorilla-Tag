using System;
using System.Globalization;

namespace emotitron.Compression.HalfFloat
{
	// Token: 0x02001141 RID: 4417
	[Serializable]
	public struct Half : IConvertible, IComparable, IComparable<Half>, IEquatable<Half>, IFormattable
	{
		// Token: 0x06006F75 RID: 28533 RVA: 0x00245252 File Offset: 0x00243452
		public Half(float value)
		{
			this.value = HalfUtilities.Pack(value);
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06006F76 RID: 28534 RVA: 0x00245260 File Offset: 0x00243460
		public ushort RawValue
		{
			get
			{
				return this.value;
			}
		}

		// Token: 0x06006F77 RID: 28535 RVA: 0x00245268 File Offset: 0x00243468
		public static float[] ConvertToFloat(Half[] values)
		{
			float[] array = new float[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = HalfUtilities.Unpack(values[i].RawValue);
			}
			return array;
		}

		// Token: 0x06006F78 RID: 28536 RVA: 0x002452A4 File Offset: 0x002434A4
		public static Half[] ConvertToHalf(float[] values)
		{
			Half[] array = new Half[values.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Half(values[i]);
			}
			return array;
		}

		// Token: 0x06006F79 RID: 28537 RVA: 0x002452D8 File Offset: 0x002434D8
		public static bool IsInfinity(Half half)
		{
			return half == Half.PositiveInfinity || half == Half.NegativeInfinity;
		}

		// Token: 0x06006F7A RID: 28538 RVA: 0x002452F4 File Offset: 0x002434F4
		public static bool IsNaN(Half half)
		{
			return half == Half.NaN;
		}

		// Token: 0x06006F7B RID: 28539 RVA: 0x00245301 File Offset: 0x00243501
		public static bool IsNegativeInfinity(Half half)
		{
			return half == Half.NegativeInfinity;
		}

		// Token: 0x06006F7C RID: 28540 RVA: 0x0024530E File Offset: 0x0024350E
		public static bool IsPositiveInfinity(Half half)
		{
			return half == Half.PositiveInfinity;
		}

		// Token: 0x06006F7D RID: 28541 RVA: 0x0024531B File Offset: 0x0024351B
		public static bool operator <(Half left, Half right)
		{
			return left < right;
		}

		// Token: 0x06006F7E RID: 28542 RVA: 0x0024532D File Offset: 0x0024352D
		public static bool operator >(Half left, Half right)
		{
			return left > right;
		}

		// Token: 0x06006F7F RID: 28543 RVA: 0x0024533F File Offset: 0x0024353F
		public static bool operator <=(Half left, Half right)
		{
			return left <= right;
		}

		// Token: 0x06006F80 RID: 28544 RVA: 0x00245354 File Offset: 0x00243554
		public static bool operator >=(Half left, Half right)
		{
			return left >= right;
		}

		// Token: 0x06006F81 RID: 28545 RVA: 0x00245369 File Offset: 0x00243569
		public static bool operator ==(Half left, Half right)
		{
			return left.Equals(right);
		}

		// Token: 0x06006F82 RID: 28546 RVA: 0x00245373 File Offset: 0x00243573
		public static bool operator !=(Half left, Half right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06006F83 RID: 28547 RVA: 0x00245380 File Offset: 0x00243580
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x00245388 File Offset: 0x00243588
		public static implicit operator float(Half value)
		{
			return HalfUtilities.Unpack(value.value);
		}

		// Token: 0x06006F85 RID: 28549 RVA: 0x00245398 File Offset: 0x00243598
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06006F86 RID: 28550 RVA: 0x002453C8 File Offset: 0x002435C8
		public string ToString(string format)
		{
			if (format == null)
			{
				return this.ToString();
			}
			return string.Format(CultureInfo.CurrentCulture, this.ToString(format, CultureInfo.CurrentCulture), Array.Empty<object>());
		}

		// Token: 0x06006F87 RID: 28551 RVA: 0x00245410 File Offset: 0x00243610
		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, this.ToString(), Array.Empty<object>());
		}

		// Token: 0x06006F88 RID: 28552 RVA: 0x0024543C File Offset: 0x0024363C
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null)
			{
				this.ToString(formatProvider);
			}
			return string.Format(formatProvider, this.ToString(format, formatProvider), Array.Empty<object>());
		}

		// Token: 0x06006F89 RID: 28553 RVA: 0x00245475 File Offset: 0x00243675
		public override int GetHashCode()
		{
			return (int)(this.value * 3 / 2 ^ this.value);
		}

		// Token: 0x06006F8A RID: 28554 RVA: 0x00245488 File Offset: 0x00243688
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

		// Token: 0x06006F8B RID: 28555 RVA: 0x002454E0 File Offset: 0x002436E0
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

		// Token: 0x06006F8C RID: 28556 RVA: 0x00245554 File Offset: 0x00243754
		public static bool Equals(ref Half value1, ref Half value2)
		{
			return value1.value == value2.value;
		}

		// Token: 0x06006F8D RID: 28557 RVA: 0x00245564 File Offset: 0x00243764
		public bool Equals(Half other)
		{
			return other.value == this.value;
		}

		// Token: 0x06006F8E RID: 28558 RVA: 0x00245574 File Offset: 0x00243774
		public override bool Equals(object obj)
		{
			return obj != null && !(obj.GetType() != base.GetType()) && this.Equals((Half)obj);
		}

		// Token: 0x06006F8F RID: 28559 RVA: 0x002455A6 File Offset: 0x002437A6
		public TypeCode GetTypeCode()
		{
			return Type.GetTypeCode(typeof(Half));
		}

		// Token: 0x06006F90 RID: 28560 RVA: 0x002455B7 File Offset: 0x002437B7
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06006F91 RID: 28561 RVA: 0x002455C9 File Offset: 0x002437C9
		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06006F92 RID: 28562 RVA: 0x002455DB File Offset: 0x002437DB
		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.Char.");
		}

		// Token: 0x06006F93 RID: 28563 RVA: 0x002455E7 File Offset: 0x002437E7
		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException("Invalid cast from SlimMath.Half to System.DateTime.");
		}

		// Token: 0x06006F94 RID: 28564 RVA: 0x002455F3 File Offset: 0x002437F3
		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06006F95 RID: 28565 RVA: 0x00245605 File Offset: 0x00243805
		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06006F96 RID: 28566 RVA: 0x00245617 File Offset: 0x00243817
		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06006F97 RID: 28567 RVA: 0x00245629 File Offset: 0x00243829
		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06006F98 RID: 28568 RVA: 0x0024563B File Offset: 0x0024383B
		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06006F99 RID: 28569 RVA: 0x0024564D File Offset: 0x0024384D
		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06006F9A RID: 28570 RVA: 0x0024565F File Offset: 0x0024385F
		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06006F9B RID: 28571 RVA: 0x0024566C File Offset: 0x0024386C
		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return this.ToType(type, provider);
		}

		// Token: 0x06006F9C RID: 28572 RVA: 0x00245686 File Offset: 0x00243886
		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06006F9D RID: 28573 RVA: 0x00245698 File Offset: 0x00243898
		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x06006F9E RID: 28574 RVA: 0x002456AA File Offset: 0x002438AA
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
