using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02001145 RID: 4421
	public class BasicWriter
	{
		// Token: 0x06006FAE RID: 28590 RVA: 0x002461DE File Offset: 0x002443DE
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x002461E6 File Offset: 0x002443E6
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06006FB0 RID: 28592 RVA: 0x002461FD File Offset: 0x002443FD
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
