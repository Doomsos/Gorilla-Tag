using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002D1 RID: 721
[Serializable]
public struct GorillaPosRotConstraint
{
	// Token: 0x04001643 RID: 5699
	[Tooltip("Transform that should be moved, rotated, and scaled to match the `source` Transform in world space.")]
	public Transform follower;

	// Token: 0x04001644 RID: 5700
	[Tooltip("Bone that `follower` should match. Set to `None` to assign a specific Transform within the same prefab.")]
	public GTHardCodedBones.SturdyEBone sourceGorillaBone;

	// Token: 0x04001645 RID: 5701
	[Tooltip("Transform that `follower` should match. This is overridden at runtime if `sourceGorillaBone` is not `None`. If set in inspector, then it should be only set to a child of the the prefab this component belongs to.")]
	public Transform source;

	// Token: 0x04001646 RID: 5702
	public string sourceRelativePath;

	// Token: 0x04001647 RID: 5703
	[Tooltip("Offset to be applied to the follower's position.")]
	public Vector3 positionOffset;

	// Token: 0x04001648 RID: 5704
	[Tooltip("Offset to be applied to the follower's rotation.")]
	public Quaternion rotationOffset;
}
