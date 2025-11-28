using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02000FB3 RID: 4019
	public static class CollectionExtensions
	{
		// Token: 0x060064EB RID: 25835 RVA: 0x0020F5EC File Offset: 0x0020D7EC
		public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T t in ts)
			{
				collection.Add(t);
			}
		}
	}
}
