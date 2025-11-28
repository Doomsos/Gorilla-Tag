using System;
using UnityEngine;

// Token: 0x02000526 RID: 1318
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x04002C1F RID: 11295
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x04002C20 RID: 11296
	public float extraVelMultiplier = 1f;

	// Token: 0x04002C21 RID: 11297
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x04002C22 RID: 11298
	[HideInInspector]
	[NonSerialized]
	public float slidePercentageOverride = -1f;

	// Token: 0x04002C23 RID: 11299
	public bool sendOnTapEvent;

	// Token: 0x04002C24 RID: 11300
	public bool disablePushBackEffect;
}
