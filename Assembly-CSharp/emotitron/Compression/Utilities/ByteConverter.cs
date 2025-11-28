using System;
using System.Runtime.InteropServices;

namespace emotitron.Compression.Utilities
{
	// Token: 0x02001140 RID: 4416
	[StructLayout(2)]
	public struct ByteConverter
	{
		// Token: 0x17000A6D RID: 2669
		public byte this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.byte0;
				case 1:
					return this.byte1;
				case 2:
					return this.byte2;
				case 3:
					return this.byte3;
				case 4:
					return this.byte4;
				case 5:
					return this.byte5;
				case 6:
					return this.byte6;
				case 7:
					return this.byte7;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06006F5D RID: 28509 RVA: 0x00244FB4 File Offset: 0x002431B4
		public static implicit operator ByteConverter(byte[] bytes)
		{
			ByteConverter result = default(ByteConverter);
			int num = bytes.Length;
			result.byte0 = bytes[0];
			if (num > 0)
			{
				result.byte1 = bytes[1];
			}
			if (num > 1)
			{
				result.byte2 = bytes[2];
			}
			if (num > 2)
			{
				result.byte3 = bytes[3];
			}
			if (num > 3)
			{
				result.byte4 = bytes[4];
			}
			if (num > 4)
			{
				result.byte5 = bytes[5];
			}
			if (num > 5)
			{
				result.byte6 = bytes[3];
			}
			if (num > 6)
			{
				result.byte7 = bytes[7];
			}
			return result;
		}

		// Token: 0x06006F5E RID: 28510 RVA: 0x00245038 File Offset: 0x00243238
		public static implicit operator ByteConverter(byte val)
		{
			return new ByteConverter
			{
				byte0 = val
			};
		}

		// Token: 0x06006F5F RID: 28511 RVA: 0x00245058 File Offset: 0x00243258
		public static implicit operator ByteConverter(sbyte val)
		{
			return new ByteConverter
			{
				int8 = val
			};
		}

		// Token: 0x06006F60 RID: 28512 RVA: 0x00245078 File Offset: 0x00243278
		public static implicit operator ByteConverter(char val)
		{
			return new ByteConverter
			{
				character = val
			};
		}

		// Token: 0x06006F61 RID: 28513 RVA: 0x00245098 File Offset: 0x00243298
		public static implicit operator ByteConverter(uint val)
		{
			return new ByteConverter
			{
				uint32 = val
			};
		}

		// Token: 0x06006F62 RID: 28514 RVA: 0x002450B8 File Offset: 0x002432B8
		public static implicit operator ByteConverter(int val)
		{
			return new ByteConverter
			{
				int32 = val
			};
		}

		// Token: 0x06006F63 RID: 28515 RVA: 0x002450D8 File Offset: 0x002432D8
		public static implicit operator ByteConverter(ulong val)
		{
			return new ByteConverter
			{
				uint64 = val
			};
		}

		// Token: 0x06006F64 RID: 28516 RVA: 0x002450F8 File Offset: 0x002432F8
		public static implicit operator ByteConverter(long val)
		{
			return new ByteConverter
			{
				int64 = val
			};
		}

		// Token: 0x06006F65 RID: 28517 RVA: 0x00245118 File Offset: 0x00243318
		public static implicit operator ByteConverter(float val)
		{
			return new ByteConverter
			{
				float32 = val
			};
		}

		// Token: 0x06006F66 RID: 28518 RVA: 0x00245138 File Offset: 0x00243338
		public static implicit operator ByteConverter(double val)
		{
			return new ByteConverter
			{
				float64 = val
			};
		}

		// Token: 0x06006F67 RID: 28519 RVA: 0x00245158 File Offset: 0x00243358
		public static implicit operator ByteConverter(bool val)
		{
			return new ByteConverter
			{
				int32 = (val ? 1 : 0)
			};
		}

		// Token: 0x06006F68 RID: 28520 RVA: 0x0024517C File Offset: 0x0024337C
		public void ExtractByteArray(byte[] targetArray)
		{
			int num = targetArray.Length;
			targetArray[0] = this.byte0;
			if (num > 0)
			{
				targetArray[1] = this.byte1;
			}
			if (num > 1)
			{
				targetArray[2] = this.byte2;
			}
			if (num > 2)
			{
				targetArray[3] = this.byte3;
			}
			if (num > 3)
			{
				targetArray[4] = this.byte4;
			}
			if (num > 4)
			{
				targetArray[5] = this.byte5;
			}
			if (num > 5)
			{
				targetArray[6] = this.byte6;
			}
			if (num > 6)
			{
				targetArray[7] = this.byte7;
			}
		}

		// Token: 0x06006F69 RID: 28521 RVA: 0x002451EF File Offset: 0x002433EF
		public static implicit operator byte(ByteConverter bc)
		{
			return bc.byte0;
		}

		// Token: 0x06006F6A RID: 28522 RVA: 0x002451F7 File Offset: 0x002433F7
		public static implicit operator sbyte(ByteConverter bc)
		{
			return bc.int8;
		}

		// Token: 0x06006F6B RID: 28523 RVA: 0x002451FF File Offset: 0x002433FF
		public static implicit operator char(ByteConverter bc)
		{
			return bc.character;
		}

		// Token: 0x06006F6C RID: 28524 RVA: 0x00245207 File Offset: 0x00243407
		public static implicit operator ushort(ByteConverter bc)
		{
			return bc.uint16;
		}

		// Token: 0x06006F6D RID: 28525 RVA: 0x0024520F File Offset: 0x0024340F
		public static implicit operator short(ByteConverter bc)
		{
			return bc.int16;
		}

		// Token: 0x06006F6E RID: 28526 RVA: 0x00245217 File Offset: 0x00243417
		public static implicit operator uint(ByteConverter bc)
		{
			return bc.uint32;
		}

		// Token: 0x06006F6F RID: 28527 RVA: 0x0024521F File Offset: 0x0024341F
		public static implicit operator int(ByteConverter bc)
		{
			return bc.int32;
		}

		// Token: 0x06006F70 RID: 28528 RVA: 0x00245227 File Offset: 0x00243427
		public static implicit operator ulong(ByteConverter bc)
		{
			return bc.uint64;
		}

		// Token: 0x06006F71 RID: 28529 RVA: 0x0024522F File Offset: 0x0024342F
		public static implicit operator long(ByteConverter bc)
		{
			return bc.int64;
		}

		// Token: 0x06006F72 RID: 28530 RVA: 0x00245237 File Offset: 0x00243437
		public static implicit operator float(ByteConverter bc)
		{
			return bc.float32;
		}

		// Token: 0x06006F73 RID: 28531 RVA: 0x0024523F File Offset: 0x0024343F
		public static implicit operator double(ByteConverter bc)
		{
			return bc.float64;
		}

		// Token: 0x06006F74 RID: 28532 RVA: 0x00245247 File Offset: 0x00243447
		public static implicit operator bool(ByteConverter bc)
		{
			return bc.int32 != 0;
		}

		// Token: 0x04007FF6 RID: 32758
		[FieldOffset(0)]
		public float float32;

		// Token: 0x04007FF7 RID: 32759
		[FieldOffset(0)]
		public double float64;

		// Token: 0x04007FF8 RID: 32760
		[FieldOffset(0)]
		public sbyte int8;

		// Token: 0x04007FF9 RID: 32761
		[FieldOffset(0)]
		public short int16;

		// Token: 0x04007FFA RID: 32762
		[FieldOffset(0)]
		public ushort uint16;

		// Token: 0x04007FFB RID: 32763
		[FieldOffset(0)]
		public char character;

		// Token: 0x04007FFC RID: 32764
		[FieldOffset(0)]
		public int int32;

		// Token: 0x04007FFD RID: 32765
		[FieldOffset(0)]
		public uint uint32;

		// Token: 0x04007FFE RID: 32766
		[FieldOffset(0)]
		public long int64;

		// Token: 0x04007FFF RID: 32767
		[FieldOffset(0)]
		public ulong uint64;

		// Token: 0x04008000 RID: 32768
		[FieldOffset(0)]
		public byte byte0;

		// Token: 0x04008001 RID: 32769
		[FieldOffset(1)]
		public byte byte1;

		// Token: 0x04008002 RID: 32770
		[FieldOffset(2)]
		public byte byte2;

		// Token: 0x04008003 RID: 32771
		[FieldOffset(3)]
		public byte byte3;

		// Token: 0x04008004 RID: 32772
		[FieldOffset(4)]
		public byte byte4;

		// Token: 0x04008005 RID: 32773
		[FieldOffset(5)]
		public byte byte5;

		// Token: 0x04008006 RID: 32774
		[FieldOffset(6)]
		public byte byte6;

		// Token: 0x04008007 RID: 32775
		[FieldOffset(7)]
		public byte byte7;

		// Token: 0x04008008 RID: 32776
		[FieldOffset(4)]
		public uint uint16_B;
	}
}
