using System;
using UnityEngine;

// Token: 0x02000CEC RID: 3308
[Serializable]
public class VoiceLoudnessReactorBlendShapeTarget
{
	// Token: 0x04005FB5 RID: 24501
	public SkinnedMeshRenderer SkinnedMeshRenderer;

	// Token: 0x04005FB6 RID: 24502
	public int BlendShapeIndex;

	// Token: 0x04005FB7 RID: 24503
	[Tooltip("Blend shape weight at minimum loudness ")]
	public float minValue;

	// Token: 0x04005FB8 RID: 24504
	[Tooltip("Blend shape weight at maximum loudness (use 100 for full weighting)\nA number higher than 100 can be used to have full weighting at lower voice loudness")]
	public float maxValue = 1f;

	// Token: 0x04005FB9 RID: 24505
	public bool UseSmoothedLoudness;
}
