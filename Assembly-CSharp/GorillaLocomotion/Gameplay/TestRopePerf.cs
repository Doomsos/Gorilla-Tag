using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FA6 RID: 4006
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x060064A0 RID: 25760 RVA: 0x0020D1E5 File Offset: 0x0020B3E5
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x0400744E RID: 29774
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x0400744F RID: 29775
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x04007450 RID: 29776
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
