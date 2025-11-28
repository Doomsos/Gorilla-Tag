using System;
using UnityEngine;

// Token: 0x020007ED RID: 2029
public class GorillaUIParent : MonoBehaviour
{
	// Token: 0x0600355D RID: 13661 RVA: 0x00121E2E File Offset: 0x0012002E
	private void Awake()
	{
		if (GorillaUIParent.instance == null)
		{
			GorillaUIParent.instance = this;
			return;
		}
		if (GorillaUIParent.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04004497 RID: 17559
	[OnEnterPlay_SetNull]
	public static volatile GorillaUIParent instance;
}
