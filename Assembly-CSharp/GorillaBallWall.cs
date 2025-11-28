using System;
using UnityEngine;

// Token: 0x0200076D RID: 1901
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x06003191 RID: 12689 RVA: 0x0010D0F4 File Offset: 0x0010B2F4
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003192 RID: 12690 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x0400400D RID: 16397
	[OnEnterPlay_SetNull]
	public static volatile GorillaBallWall instance;
}
