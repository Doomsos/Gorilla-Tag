using System;
using UnityEngine;

// Token: 0x02000CF1 RID: 3313
[Serializable]
public class VoiceLoudnessReactorAnimatorTarget
{
	// Token: 0x04005FD4 RID: 24532
	public Animator animator;

	// Token: 0x04005FD5 RID: 24533
	public bool useSmoothedLoudness;

	// Token: 0x04005FD6 RID: 24534
	public float animatorSpeedToLoudness = 1f;
}
