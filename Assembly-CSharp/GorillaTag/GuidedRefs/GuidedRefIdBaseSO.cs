using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200103C RID: 4156
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x060068F0 RID: 26864 RVA: 0x00002789 File Offset: 0x00000989
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x060068F2 RID: 26866 RVA: 0x000178ED File Offset: 0x00015AED
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
