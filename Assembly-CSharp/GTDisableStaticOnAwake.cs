using System;
using UnityEngine;

// Token: 0x020002C9 RID: 713
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x0600119B RID: 4507 RVA: 0x0005CB58 File Offset: 0x0005AD58
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
