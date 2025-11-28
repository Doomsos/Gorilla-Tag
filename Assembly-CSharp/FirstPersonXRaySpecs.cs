using System;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
public class FirstPersonXRaySpecs : MonoBehaviour
{
	// Token: 0x06001FE4 RID: 8164 RVA: 0x000A9B1D File Offset: 0x000A7D1D
	private void OnEnable()
	{
		GorillaBodyRenderer.SetAllSkeletons(true);
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000A9B25 File Offset: 0x000A7D25
	private void OnDisable()
	{
		GorillaBodyRenderer.SetAllSkeletons(false);
	}
}
