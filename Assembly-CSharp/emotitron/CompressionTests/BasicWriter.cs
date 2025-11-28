using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02001145 RID: 4421
	public class BasicWriter
	{
		// Token: 0x06006FAE RID: 28590 RVA: 0x002461FE File Offset: 0x002443FE
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x00246206 File Offset: 0x00244406
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06006FB0 RID: 28592 RVA: 0x0024621D File Offset: 0x0024441D
		public static byte BasicRead(byte[] buffer)
		{
			byte result = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return result;
		}

		// Token: 0x04008025 RID: 32805
		public static int pos;
	}
}
