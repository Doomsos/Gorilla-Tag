using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FE3 RID: 4067
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060066E5 RID: 26341 RVA: 0x00217831 File Offset: 0x00215A31
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
