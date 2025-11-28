using System;
using System.Diagnostics;

namespace BuildSafe
{
	// Token: 0x02000EA6 RID: 3750
	public static class Callbacks
	{
		// Token: 0x02000EA7 RID: 3751
		[Conditional("UNITY_EDITOR")]
		public class DidReloadScripts : Attribute
		{
			// Token: 0x06005DBE RID: 23998 RVA: 0x001E16FD File Offset: 0x001DF8FD
			public DidReloadScripts(bool activeOnly = false)
			{
				this.activeOnly = activeOnly;
			}

			// Token: 0x04006BC0 RID: 27584
			public bool activeOnly;
		}
	}
}
