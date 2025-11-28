using System;

namespace emotitron.Compression
{
	// Token: 0x0200113F RID: 4415
	public static class ZigZagExt
	{
		// Token: 0x06006F54 RID: 28500 RVA: 0x00244EE9 File Offset: 0x002430E9
		public static ulong ZigZag(this long s)
		{
			return (ulong)(s << 1 ^ s >> 63);
		}

		// Token: 0x06006F55 RID: 28501 RVA: 0x00244EF3 File Offset: 0x002430F3
		public static long UnZigZag(this ulong u)
		{
			return (long)(u >> 1 ^ -(long)(u & 1UL));
		}

		// Token: 0x06006F56 RID: 28502 RVA: 0x00244EFE File Offset: 0x002430FE
		public static uint ZigZag(this int s)
		{
			return (uint)(s << 1 ^ s >> 31);
		}

		// Token: 0x06006F57 RID: 28503 RVA: 0x00244F08 File Offset: 0x00243108
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06006F58 RID: 28504 RVA: 0x00244F15 File Offset: 0x00243115
		public static ushort ZigZag(this short s)
		{
			return (ushort)((int)s << 1 ^ s >> 15);
		}

		// Token: 0x06006F59 RID: 28505 RVA: 0x00244F20 File Offset: 0x00243120
		public static short UnZigZag(this ushort u)
		{
			return (short)(u >> 1 ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06006F5A RID: 28506 RVA: 0x00244F2C File Offset: 0x0024312C
		public static byte ZigZag(this sbyte s)
		{
			return (byte)((int)s << 1 ^ s >> 7);
		}

		// Token: 0x06006F5B RID: 28507 RVA: 0x00244F36 File Offset: 0x00243136
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)(u >> 1 ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
