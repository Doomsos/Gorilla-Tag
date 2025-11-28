using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000FAC RID: 4012
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060064CA RID: 25802 RVA: 0x0020ECA4 File Offset: 0x0020CEA4
		public HandHold target
		{
			get
			{
				HandHold result;
				if (this.reference.TryResolve<HandHold>(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060064CB RID: 25803 RVA: 0x0020ECC4 File Offset: 0x0020CEC4
		public GameObject targetObject
		{
			get
			{
				GameObject result;
				if (this.reference.TryResolve(out result))
				{
					return result;
				}
				return null;
			}
		}

		// Token: 0x04007483 RID: 29827
		[SerializeField]
		public XSceneRef reference;
	}
}
