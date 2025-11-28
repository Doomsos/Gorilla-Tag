using System;
using UnityEngine;

namespace emotitron.Compression
{
	// Token: 0x02001139 RID: 4409
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x06006F3A RID: 28474 RVA: 0x00244864 File Offset: 0x00242A64
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x04007FD7 RID: 32727
		[SerializeField]
		protected int bits;
	}
}
