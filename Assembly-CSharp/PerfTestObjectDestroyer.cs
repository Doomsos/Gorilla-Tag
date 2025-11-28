using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200034A RID: 842
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestObjectDestroyer : MonoBehaviour
{
	// Token: 0x06001421 RID: 5153 RVA: 0x00074166 File Offset: 0x00072366
	private void Start()
	{
		Object.DestroyImmediate(base.gameObject, true);
	}
}
