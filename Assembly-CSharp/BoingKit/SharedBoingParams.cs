using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011B3 RID: 4531
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06007242 RID: 29250 RVA: 0x002583A4 File Offset: 0x002565A4
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x040082BE RID: 33470
		public BoingWork.Params Params;
	}
}
