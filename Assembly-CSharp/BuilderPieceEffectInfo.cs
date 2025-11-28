using System;
using UnityEngine;

// Token: 0x0200055D RID: 1373
[CreateAssetMenu(fileName = "BuilderPieceEffectInfo", menuName = "Gorilla Tag/Builder/EffectInfo", order = 0)]
public class BuilderPieceEffectInfo : ScriptableObject
{
	// Token: 0x04002D5F RID: 11615
	public GameObject placeVFX;

	// Token: 0x04002D60 RID: 11616
	public GameObject disconnectVFX;

	// Token: 0x04002D61 RID: 11617
	public GameObject grabbedVFX;

	// Token: 0x04002D62 RID: 11618
	public GameObject locationLockVFX;

	// Token: 0x04002D63 RID: 11619
	public GameObject recycleVFX;

	// Token: 0x04002D64 RID: 11620
	public GameObject tooHeavyVFX;
}
