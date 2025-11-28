using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000F5D RID: 3933
	public static class ListMethodsExtensions
	{
		// Token: 0x06006286 RID: 25222 RVA: 0x001FB598 File Offset: 0x001F9798
		public static void RemoveAllNullItems<T>(this List<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
				}
			}
		}
	}
}
