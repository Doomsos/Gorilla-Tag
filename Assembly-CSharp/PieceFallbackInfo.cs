using System;
using UnityEngine;

// Token: 0x02000580 RID: 1408
[Serializable]
public struct PieceFallbackInfo
{
	// Token: 0x04002E41 RID: 11841
	[Tooltip("Check if the piece has Material Options set and the default material is in a starter set")]
	public bool materialSwapThisPrefab;

	// Token: 0x04002E42 RID: 11842
	[Tooltip("A piece in a starter set with the same builder attach grid configuration\n(check BuilderSetManager _starterPieceSets for pieces in starter sets)")]
	public BuilderPiece prefab;
}
