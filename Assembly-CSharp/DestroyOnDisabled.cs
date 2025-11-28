using System;
using UnityEngine;

// Token: 0x02000C42 RID: 3138
public class DestroyOnDisabled : MonoBehaviour
{
	// Token: 0x06004D04 RID: 19716 RVA: 0x0005BBF3 File Offset: 0x00059DF3
	private void OnDisable()
	{
		Object.Destroy(base.gameObject);
	}
}
