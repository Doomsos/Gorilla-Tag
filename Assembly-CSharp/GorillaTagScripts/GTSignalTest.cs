using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DF3 RID: 3571
	public class GTSignalTest : GTSignalListener
	{
		// Token: 0x0400662F RID: 26159
		public MeshRenderer[] targets = new MeshRenderer[0];

		// Token: 0x04006630 RID: 26160
		[Space]
		public MeshRenderer target;

		// Token: 0x04006631 RID: 26161
		public List<GTSignalListener> listeners = new List<GTSignalListener>(12);
	}
}
