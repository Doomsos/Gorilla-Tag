using System;
using UnityEngine;

// Token: 0x02000CF0 RID: 3312
[Serializable]
public class VoiceLoudnessReactorGameObjectEnableTarget
{
	// Token: 0x04005FCF RID: 24527
	public GameObject GameObject;

	// Token: 0x04005FD0 RID: 24528
	public float Threshold;

	// Token: 0x04005FD1 RID: 24529
	public bool TurnOnAtThreshhold = true;

	// Token: 0x04005FD2 RID: 24530
	public bool UseSmoothedLoudness;

	// Token: 0x04005FD3 RID: 24531
	public float Scale = 1f;
}
