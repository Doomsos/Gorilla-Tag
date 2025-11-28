using System;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class ProximityEffectScoreCurvesSO : ScriptableObject
{
	// Token: 0x04001471 RID: 5233
	[Tooltip("How far apart the transforms are. A distance. Contributes 'red' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve distanceModifierCurve;

	// Token: 0x04001472 RID: 5234
	[Tooltip("How closely the transforms' Z vectors are pointed towards each other. A dot product. Contributes 'green' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve alignmentModifierCurve;

	// Token: 0x04001473 RID: 5235
	[Tooltip("Whether each transform is in front of the other transform. The average of two dot products. Contributes 'blue' to the debug line. Y value should be in the range 0-1.")]
	public AnimationCurve parallelModifierCurve;
}
