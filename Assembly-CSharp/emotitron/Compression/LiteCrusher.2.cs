using System;

namespace emotitron.Compression
{
	// Token: 0x0200113A RID: 4410
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x06006F3C RID: 28476
		public abstract ulong Encode(T val);

		// Token: 0x06006F3D RID: 28477
		public abstract T Decode(uint val);

		// Token: 0x06006F3E RID: 28478
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x06006F3F RID: 28479
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x06006F40 RID: 28480
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
