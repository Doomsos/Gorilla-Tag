using System;
using UnityEngine;

// Token: 0x02000500 RID: 1280
public class DestroyIfNotQA : MonoBehaviour
{
	// Token: 0x060020D8 RID: 8408 RVA: 0x0005BBF3 File Offset: 0x00059DF3
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
