using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x0200107C RID: 4220
	[Serializable]
	public class EdMeshCombinedPrefabData
	{
		// Token: 0x060069DB RID: 27099 RVA: 0x00002789 File Offset: 0x00000989
		public void Clear()
		{
		}

		// Token: 0x0400791B RID: 31003
		public string path;

		// Token: 0x0400791C RID: 31004
		public List<Renderer> disabled = new List<Renderer>(512);

		// Token: 0x0400791D RID: 31005
		public List<GameObject> combined = new List<GameObject>(64);
	}
}
